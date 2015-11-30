using System;
using System.IO;
using System.Xml.Linq;
using System.IO.IsolatedStorage;

using Microsoft.Xna.Framework;

namespace Leda.Core.Asset_Management
{
    public class FileManager
    {
        public static XDocument LoadXMLContentFile(string fileName)
        {
            XDocument loadedDoc = null;

            using (Stream xmlStream = TitleContainer.OpenStream(fileName)) { loadedDoc = XDocument.Load(xmlStream); }

            return loadedDoc;
        }

        public static bool FileExists(string fileName)
        {
            return (IsolatedStorageFile.GetUserStoreForDomain().FileExists(fileName));
        }

        public static XDocument LoadXMLFile(string fileName)
        {
            XDocument loadedDoc = null;

            if (FileExists(fileName))
            {
                using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForDomain())
                {
                    using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(fileName, FileMode.Open, isoStore))
                    {
                        loadedDoc = XDocument.Load(isoStream);
                    }
                }
            }

            return loadedDoc;
        }

        public static void SaveXMLFile(string fileName, XDocument fileContent)
        {
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForDomain())
            {
                using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(fileName, FileMode.Create, isoStore))
                {
					using (StreamWriter writer = new StreamWriter(isoStream))
					{
						writer.Write(fileContent);
					}
                }
            }
        }

        public static void DeleteFile(string fileName)
        {
            if (FileExists(fileName)) { IsolatedStorageFile.GetUserStoreForDomain().DeleteFile(fileName); }
        }

        public static bool FolderExists(string folderPath)
        {
            return IsolatedStorageFile.GetUserStoreForDomain().DirectoryExists(folderPath);
        }

        public static void CreateFolder(string folderPath)
        {
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForDomain())
            {
                if (!isoStore.DirectoryExists(folderPath))
                {
                    string workingPath = "";
                    foreach (string s in folderPath.Split('/'))
                    {
                        workingPath = string.Concat(workingPath, s);
                        if (!isoStore.DirectoryExists(workingPath)) { isoStore.CreateDirectory(workingPath); }
                        workingPath = string.Concat(workingPath, "/");
                    }
                }
            }
        }

        public static string[] FileListForFolder(string folderPath)
        {
            if (FolderExists(folderPath)) { return IsolatedStorageFile.GetUserStoreForDomain().GetFileNames(folderPath); }

            return null;
        }
    }
}