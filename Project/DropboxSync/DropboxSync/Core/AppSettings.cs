using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using DropboxSync.Commands.Handlers;
using DropboxSync.DomainEvents;

namespace DropboxSync.Core
{
    public class AppSettings : INotifyPropertyChanged
    {
        public AppSettings()
        {
            Uploaded = new List<UploadedFile>();

            App.On<PictureUploadedToDropbox>(PictureUploadedToDropbox);
        }

        private void PictureUploadedToDropbox(IDomainEvent domainEvent)
        {
            var data = domainEvent as PictureUploadedToDropbox;
            if(!Uploaded.Any(c => c.Album == data.Picture.Album.Name && c.OriginalFilename == data.Picture.Name))
            {
                Uploaded.Add(new UploadedFile()
                                      {
                                          Album = data.Picture.Album.Name,
                                          Destination = data.Destination,
                                          NewfileName = data.NewFilename,
                                          OriginalFilename = data.Picture.Name,
                                          TimeStamp = DateTime.Now
                                      });
            }
            App.SaveSettings();
        }

        public string DropboxUserToken { get; set; }
        public string DropboxUserSecret { get; set; }
        public List<UploadedFile> Uploaded { get; set; }
        public bool IsDropboxAuthenticated
        {
            get { return !String.IsNullOrEmpty(DropboxUserSecret) && !String.IsNullOrEmpty(DropboxUserToken); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class UploadedFile
    {
        public string Album { get; set; }
        public string OriginalFilename { get; set; }
        public string Destination { get; set; }
        public string NewfileName { get; set; }
        public DateTime? TimeStamp { get; set; }
    }
}
