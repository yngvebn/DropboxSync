using System;

namespace DropboxSync.Core
{
    public class PageDefinitionAttribute : Attribute
    {
        public string RelativePath { get; set; }

        public PageDefinitionAttribute(string relativePath)
        {
            RelativePath = relativePath;
        }
        public Uri RelativeUri
        {
            get
            {
                return new Uri(RelativePath, UriKind.Relative);
            }
        }
    }
}
