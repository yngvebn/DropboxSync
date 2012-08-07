using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using DropboxSync.CQRS;
using DropboxSync.Commands;
using DropboxSync.Commands.Handlers;
using DropboxSync.Core;
using DropboxSync.DomainEvents;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Media;

namespace DropboxSync.ViewModels.Pages
{
    [PageDefinition("/Home.xaml")]
    public class MainPageViewModel : ViewModel
    {
        private List<UploadedPicture> _recentPictures;

        public MainPageViewModel()
        {
            _recentPictures = new List<UploadedPicture>();
            BackgroundWorker initialization = new BackgroundWorker();
            initialization.DoWork += new DoWorkEventHandler(initialization_DoWork);
            initialization.RunWorkerAsync();
            
            CanStartUploading = true;
            DropboxInfo = new DropboxInfo();
            UploadPictureToDropboxCommand = new DelegateCommand(this.UploadPictureToDropbox);
            SyncNowCommand = new DelegateCommand(this.SyncNow);
            DropboxInfo.PropertyChanged += new PropertyChangedEventHandler(DropboxInfo_PropertyChanged);
            App.On<PictureSyncCountUpdated>(PictureSyncCountUpdatedHandler);
            App.On<PictureUploadedToDropbox>(PictureUploadedToDropboxHandler);
        }

        void initialization_DoWork(object sender, DoWorkEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(InitRecentPictures);
        }



        private void InitRecentPictures()
        {
            var query =
                App.Settings.Uploaded.Where(c => c.TimeStamp.HasValue).OrderByDescending(c => c.TimeStamp).Take(10);
                        
            foreach(var item in query)
            {
                var uploadedPicture = new UploadedPicture(item);
                _recentPictures.Add(uploadedPicture);
            }
            NotifyPropertyChanged(this, "RecentPictures");
        }

        private void PictureUploadedToDropboxHandler(IDomainEvent domainEvent)
        {
            var eventData = domainEvent as PictureUploadedToDropbox;
            _recentPictures.Add(new UploadedPicture(new UploadedFile(){ Album = eventData.Picture.Album.Name, Destination = eventData.Destination, NewfileName  = eventData.NewFilename, OriginalFilename = eventData.Picture.Name, TimeStamp = DateTime.Now }));
            NotifyPropertyChanged(this, "RecentPictures");
        }

        private void PictureSyncCountUpdatedHandler(IDomainEvent domainEvent)
        {
            var eventData = domainEvent as PictureSyncCountUpdated;
            DropboxInfo.PictureCount = eventData.NewCount;
        }

        void DropboxInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PictureCount")
                NotifyPropertyChanged(this, "SyncNowText");
        }


        public WebBrowser Browser { get; set; }

        public ICommand UploadPictureToDropboxCommand { get; private set; }
        public ICommand SyncNowCommand { get; private set; }
        public DropboxInfo DropboxInfo { get; private set; }

        public ObservableCollection<UploadedPicture> RecentPictures
        {
            get { return new ObservableCollection<UploadedPicture>(_recentPictures); }
        }

        public string SyncNowText
        {
            get
            {
                if (!CanStartUploading)
                    return "Syncing (click to stop)";
                if (DropboxInfo.PictureCount == 0)
                    return "All in sync";
                return string.Format("Sync {0} pictures now", DropboxInfo.PictureCount);
            }
        }

        private void SyncNow(object args)
        {
            if (!CanStartUploading)
            {
                App.Executor.Cancel<SyncImagesNowCommand>();
            }
            else
            {
                try
                {
                    CanStartUploading = false;
                    MediaLibrary mediaLibrary = new MediaLibrary();
                    var cameraRoll = mediaLibrary.RootPictureAlbum.Albums.Single(c => c.Name == "Camera Roll");
                    var result = App.Executor.ExecuteCommand(new SyncImagesNowCommand()
                    {
                        Pictures = cameraRoll.Pictures.ToList()
                    }, success: Success, progress: Progress);
                    if(result.Status == CommandStatus.Error)
                    {
                        App.ViewModel.DisplayMessage(result.Message, "");
                        Success(null);
                    }
                }
                catch
                {
                    CanStartUploading = true;
                }
                finally
                {

                }
            }
        }

        public bool CanStartUploading { get; private set; }

        private void Success(object obj)
        {
            CanStartUploading = true;
            DisplayProgressMessage(null);
        }

        private void Progress(CommandExecutingProgress commandExecutingProgress)
        {
            DisplayProgressMessage(String.Format("{0} ({1}%)", commandExecutingProgress.ProgressText, commandExecutingProgress.ProgressPercent));
        }

        public void UploadPictureToDropbox(object args)
        {

        }

        public bool IsDropboxAuthenticated
        {
            get { return App.Settings.IsDropboxAuthenticated; }
        }

        public bool IsNotAuthenticated
        {
            get { return !IsDropboxAuthenticated; }
        }

        public override void Init(PhoneApplicationPage phoneApplicationPage)
        {
            if (App.Settings.IsDropboxAuthenticated)
            {
                App.Executor.ExecuteCommand(new PopulateMetadataCommand() { MainPageViewModel = this });
                App.Executor.ExecuteCommand(new UpdatePicturesToSyncCommand());
            }
            base.Init(phoneApplicationPage);
        }

    }

    public class UploadedPicture
    {
        public UploadedPicture(UploadedFile file)
        {
            MediaLibrary mediaLibrary = new MediaLibrary();
            var img = mediaLibrary.RootPictureAlbum.Albums.Single(c => c.Name == file.Album).Pictures.Single(
                c => c.Name == file.OriginalFilename);
            Image = new BitmapImage();
            Image.SetSource(img.GetThumbnail());
            File = file;
        }
        public BitmapImage Image { get; private set; }
        public UploadedFile File { get; private set; }
    }

    public class DropboxInfo : INotifyPropertyChanged
    {
        public string DisplayName { get; set; }
        public int PictureCount { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
