using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DropNet.Exceptions;
using DropNet.Models;
using DropboxSync.CQRS;
using DropboxSync.Core;
using DropboxSync.DomainEvents;
using DropboxSync.ViewModels.Pages;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Xna.Framework.Media;
using RestSharp;

namespace DropboxSync.Commands.Handlers
{
    public class SyncImagesNowCommandHandler : ICommandHandler<SyncImagesNowCommand>
    {
        private UserSettings settings;
        public SyncImagesNowCommandHandler()
        {
            settings = new SettingsResolver<UserSettings>().Settings;
            Files = new Dictionary<string, List<string>>();
        }

        private int totalImagesToUpload = 0;
        private int imagesUploaded = 0;
        private int CurrentProgressPercent
        {
            get { return imagesUploaded * 100 / totalImagesToUpload; }
        }
        private Action<CommandExecutingProgress> _progress;
        private Action<object> _success;
        public void Handle(SyncImagesNowCommand command, Action<object> success, Action<object> error, Action<CommandExecutingProgress> progress)
        {
            if (!command.Pictures.Any()) return;
            _progress = progress;
            _success = success;
            if (settings.UploadOnlyOnWifi)
                if (!GetIsWifiAvailable()) throw new WifiRequiredException("Must be connected to WiFi to upload. Go to settings to change");
            var uploaded = App.Settings.Uploaded;

            picturesToUpload = new Queue<Picture>(command.Pictures.Where(pic => !uploaded.Where(c => c.Album == "Camera Roll").Select(c => c.OriginalFilename).Contains(pic.Name)));
            totalImagesToUpload = picturesToUpload.Count();
            Upload();
        }

        public bool IsCancelled { get; set; }
        public void Cancel()
        {
            IsCancelled = true;
            _progress(new CommandExecutingProgress()
            {
                ProgressPercent = CurrentProgressPercent,
                ProgressText = String.Format("Uploading {0} (Cancelling)", image.Name)
            });

        }

        private Picture image;
        private string uploadPath;
        private void Upload()
        {
            if (!picturesToUpload.Any() || IsCancelled)
            {
                _success(null);
                return;
            }

            image = picturesToUpload.Dequeue();
            _progress(new CommandExecutingProgress()
                          {
                              ProgressPercent = CurrentProgressPercent,
                              ProgressText = String.Format("Uploading {0}", image.Name)
                          });

            var settings = new SettingsResolver<UserSettings>().Settings;
            string basePath = settings.DropboxRootFolder.TrimEnd('/');
            uploadPath = "";
            if (settings.UploadToSubFolder)
            {
                uploadPath = basePath + "/" + new FolderNameSubstitutions().ConstructFolderName(settings.FolderNameTemplate, image.Date);
            }
            fileName = image.Name;
            if (!Files.ContainsKey(uploadPath))
            {
                App.GlobalClient.GetMetaDataAsync(uploadPath, GetMetadataSuccess, GetMetaDataFailure);
            }
            else
            {
                UploadCurrentFile();

            }
        }

        private void UploadCurrentFile()
        {
            string fileName = image.Name;
            string fileExtension = Path.GetExtension(fileName);
            string fileNameOnly = Path.GetFileNameWithoutExtension(fileName);
            string newFileName = fileNameOnly;
            int i = 1;
            while (Files[uploadPath].Contains(newFileName + fileExtension))
            {
                newFileName = String.Format("{0}_{1}", fileNameOnly, i);
                i++;
            }
            string fullNewFileName = newFileName + fileExtension;
            if (!Files[uploadPath].Contains(fullNewFileName))
                Files[uploadPath].Add(fullNewFileName);

            App.GlobalClient.UploadFileAsync(uploadPath, fullNewFileName, image.GetImage(), UploadFileSuccess, UploadFileError);
        }

        private Dictionary<string, List<string>> Files { get; set; }
        private void GetMetaDataFailure(DropboxException dropboxException)
        {
            Files.Add(uploadPath, new List<string>());
        }

        private void GetMetadataSuccess(MetaData metaData)
        {
            List<string> files = metaData.Contents.Where(c => !c.Is_Dir && !c.Is_Deleted).Select(c => c.Name).ToList();
            Files.Add(uploadPath, files);
            UploadCurrentFile();
        }


        private string fileName;
        private Queue<Picture> picturesToUpload = new Queue<Picture>();

        private void UploadFileSuccess(RestResponse obj)
        {

            App.Raise(new PictureUploadedToDropbox() { Picture = image, NewFilename = fileName, Destination = uploadPath });
            var mainPageViewModel = App.ViewModel.CurrentViewModel as MainPageViewModel;
            if (mainPageViewModel != null)
                App.Executor.ExecuteCommand(new UpdatePicturesToSyncCommand() { MainPageViewModel = mainPageViewModel });
            imagesUploaded++;
            Upload();
        }

        private void UploadFileError(DropboxException obj)
        {
            Upload();
        }
        /// <summary> 
        /// Caution: MAY be slow. So better call it on background thread. 
        /// </summary> 
        /// <returns></returns> 
        private static bool GetIsWifiAvailable()
        {
            // if WiFi isn't enabled in settings, we don't need to call NetworkInterface.NetworkInterfaceType. However this fails in emulator. 
            if (!DeviceNetworkInformation.IsWiFiEnabled)
                return false;

            // CAUTION! It's reported that getting NetworkInterfaceType needs up to 60seconds! 
            switch (Microsoft.Phone.Net.NetworkInformation.NetworkInterface.NetworkInterfaceType)
            {
                case NetworkInterfaceType.Wireless80211:
                    return true;
                default:
                    return false;
            }
        }
    }

    public class WifiRequiredException : Exception
    {
        public WifiRequiredException(string message)
            : base(message)
        {


        }
    }
}