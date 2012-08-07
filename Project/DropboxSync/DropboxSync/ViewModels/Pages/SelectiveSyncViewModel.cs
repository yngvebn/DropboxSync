using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using DropboxSync.CQRS;
using DropboxSync.Commands;
using DropboxSync.Core;
using DropboxSync.DomainEvents;
using Microsoft.Xna.Framework.Media;

namespace DropboxSync.ViewModels.Pages
{
    [PageDefinition("/SelectiveSync.xaml")]
    public class SelectiveSyncViewModel : ViewModel
    {
        public SelectiveSyncViewModel()
        {
            CanStartUploading = true;
            Images = new ObservableCollection<ImageListItem>();
            _mediaLibrary = new MediaLibrary();
            SelectAlbumCommand = new DelegateCommand(SelectAlbum);
            SyncSelectedCommand = new DelegateCommand(SyncSelected);
            App.On<PictureUploadedToDropbox>(Handler);
            hideUploaded = new SettingsResolver<UserSettings>().Settings.HideUploaded;
            LoadAlbums();
        }

        private bool hideUploaded;
        private void Handler(IDomainEvent domainEvent)
        {
            var data = domainEvent as PictureUploadedToDropbox;
            if (hideUploaded)
                RemoveImage(data.Picture);
        }

        private void RemoveImage(Picture picture)
        {
            var image =
                Images.SingleOrDefault(c => c.Picture.Name == picture.Name && c.Picture.Album.Name == picture.Album.Name);
            if (image != null)
                Images.Remove(image);
        }

        private void SyncSelected(object args)
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
                    var result = App.Executor.ExecuteCommand(new SyncImagesNowCommand()
                    {
                        Pictures = Images.Where(c => c.Selected).Select(c => c.Picture).ToList()
                    }, success: Success, progress: Progress);
                    if (result.Status == CommandStatus.Error)
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
        public bool HasSelected
        {
            get { return Images.Any(c => c.Selected); }
        }
        private readonly MediaLibrary _mediaLibrary;
        private void LoadAlbums()
        {

            Albums = new ObservableCollection<AlbumListItem>(_mediaLibrary.RootPictureAlbum.Albums.Where(c => c.Pictures.Any()).Select(c => new AlbumListItem(c, this)));
        }

        public ICommand SyncSelectedCommand { get; private set; }
        public ICommand SelectAlbumCommand { get; private set; }
        public void SelectAlbum(object args)
        {
            backWasPressed = false;
            Images.Clear();
            currentAlbum = Albums.Single(c => c.AlbumName == args.ToString()).Album;
            LoadImages(currentAlbum);
            NotifyPropertyChanged(this, "IsNotInAlbum");
            NotifyPropertyChanged(this, "IsInsideAlbum");
            NotifyPropertyChanged(this, "Images");
        }

        private void LoadImages(PictureAlbum pictureAlbum)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerAsync();
        }

        private bool backWasPressed;
        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
                                DisplayProgressMessage("Populating album..."));
            int pageSize = 5;
            int currentPage = 0;
            bool first = true;
            IEnumerable<Picture> pictures = currentAlbum.Pictures;
            var uploadedPictures = App.Settings.Uploaded;
            var images = pictures.Skip(currentPage * pageSize).Take(pageSize).ToList();
            while (images.Any() && !backWasPressed)
            {
                foreach (var image in images)
                {
                    var shouldDisplay = true;
                    if (hideUploaded)
                    {
                        if (uploadedPictures.Any(c => c.Album == image.Album.Name && c.OriginalFilename == image.Name))
                        {
                            shouldDisplay = false;
                        }
                    }
                    if (shouldDisplay)
                        AddImage(image);
                }


                NotifyPropertyChanged(this, "Images");
                currentPage++;
                images = currentAlbum.Pictures.Skip(currentPage * pageSize).Take(pageSize).ToList();
            }
            Deployment.Current.Dispatcher.BeginInvoke(() =>
                                DisplayProgressMessage(null));
        }
        public void AddImage(Picture image)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => Images.Add(new ImageListItem(image, this)));

        }
        public ObservableCollection<AlbumListItem> Albums { get; set; }
        public ObservableCollection<ImageListItem> Images { get; set; }




        void imageLoader_DoWork(object sender, DoWorkEventArgs e)
        {

        }
        private PictureAlbum currentAlbum;
        public bool IsNotInAlbum
        {
            get { return !IsInsideAlbum; }
        }
        public bool IsInsideAlbum
        {
            get { return currentAlbum != null; }
        }

        public void BackClicked()
        {
            backWasPressed = true;
            if (!CanStartUploading && IsNotInAlbum)
            {
                App.ViewModel.DisplayConfirm("Navigating away stop the upload-process. Are you sure?", "Go back?", NavigateBack, StopNavigation);
            }
            else
            {
                NavigateBack();
            }

        }

        private void StopNavigation()
        {

        }

        private void NavigateBack()
        {
            if (IsNotInAlbum) App.ViewModel.NavigateBack();

            currentAlbum = null;
            NotifyPropertyChanged(this, "IsNotInAlbum");
            NotifyPropertyChanged(this, "IsInsideAlbum");
            NotifyPropertyChanged(this, "Images");
        }
    }

    public class ImageListItem : INotifyPropertyChanged
    {
        public ImageListItem(Picture picture, SelectiveSyncViewModel parent)
        {
            Picture = picture;
            Parent = parent;
            Thumb = new BitmapImage();
            BackgroundWorker imageLoader = new BackgroundWorker();
            imageLoader.DoWork += new DoWorkEventHandler(imageLoader_DoWork);
            imageLoader.RunWorkerAsync();

            SelectCommand = new DelegateCommand(SelectImage);
        }

        void imageLoader_DoWork(object sender, DoWorkEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => Thumb.SetSource(Picture.GetThumbnail()));

        }
        private void SelectImage(object o)
        {
            if (Parent.CanStartUploading)
                Selected = !Selected;
        }
        public ICommand SelectCommand { get; private set; }
        public bool Selected { get; set; }
        public Picture Picture { get; private set; }
        public SelectiveSyncViewModel Parent { get; private set; }
        public BitmapImage Thumb { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class AlbumListItem
    {
        public AlbumListItem(PictureAlbum album, SelectiveSyncViewModel parent)
        {
            AlbumName = album.Name;
            Album = album;
            Parent = parent;
            Thumb = new BitmapImage();
            Thumb.SetSource(album.Pictures.First().GetThumbnail());
        }

        public PictureAlbum Album { get; private set; }
        public SelectiveSyncViewModel Parent { get; set; }
        public string AlbumName { get; private set; }
        public BitmapImage Thumb { get; private set; }
    }
}