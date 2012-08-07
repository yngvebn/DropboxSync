using System;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;

namespace DropboxSync.Core
{
    public class SettingsResolver<T> where T: class, new ()
    {
        public T Settings { get; private set; }
        private IsolatedStorageFile isolatedStorageFile;
        
        public SettingsResolver()
        {
            try{
                isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication();
                ResolveAll();
            }
            catch(Exception ex)
            {
                
            }
        }

        private void ResolveAll()
        {

            var sFile = typeof(T).FullName;
            Debug.Assert(sFile != null, "sFile != null");
            if(!isolatedStorageFile.FileExists(sFile))
            {
                var dataFile = isolatedStorageFile.CreateFile(sFile);

                dataFile.Close();
                Settings = new T();
                Save();
            }
            using(var sr = new StreamReader(new IsolatedStorageFileStream(sFile, FileMode.Open, FileAccess.ReadWrite, isolatedStorageFile)))
            {
                var json = sr.ReadToEnd();
                Settings = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
            }
            Save();
        }

        public void Save()
        {
            var sFile = typeof(T).FullName;
            Debug.Assert(sFile != null, "sFile != null");
            using (var sw = new StreamWriter(new IsolatedStorageFileStream(sFile, FileMode.Truncate, FileAccess.ReadWrite, isolatedStorageFile)))
            {
                sw.Write(Newtonsoft.Json.JsonConvert.SerializeObject(Settings));
            }
        }


        public static void Save(T settings)
        {
            var isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication();
            var sFile = typeof(T).FullName;
            Debug.Assert(sFile != null, "sFile != null");
            if (isolatedStorageFile.FileExists(sFile))
            {
                isolatedStorageFile.DeleteFile(sFile);
                var dataFile = isolatedStorageFile.CreateFile(sFile);
                dataFile.Close();
            }
            using (var sw = new StreamWriter(new IsolatedStorageFileStream(sFile, FileMode.Open, FileAccess.ReadWrite, IsolatedStorageFile.GetUserStoreForApplication())))
            {
                sw.Write(Newtonsoft.Json.JsonConvert.SerializeObject(settings));
            }
        }
    }
}