using System;
using System.Collections.Generic;
using System.Linq;

namespace DropboxSync.ViewModels.Pages
{
    public class DropboxFolder
    {

        public string Name { get; set; }
        public List<DropboxFolder> Folders { get; set; }

        public string Path { get; set; }

        public string ParentPath
        {
            get
            {
                if (!Path.Equals("/"))
                {
                    var segments = Path.Split('/');
                    var newPath = "";
                    for (var i = 0; i < segments.Count() - 1; i++)
                    {
                        if (String.IsNullOrEmpty(segments[i])) continue;
                        newPath += "/"+segments[i];
                    }
                    return newPath;
                }
                else
                {
                    return Path;
                }

            }
        }

        public DropboxFolder()
        {
            Folders = new List<DropboxFolder>();
        }

        public DropboxFolder(string name, string path)
        {
            Name = name;
            Path = path;
        }
    }
}