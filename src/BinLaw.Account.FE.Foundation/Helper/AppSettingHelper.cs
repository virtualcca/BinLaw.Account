using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace BinLaw.Account.FE.Foundation.Helper
{
    public class AppSettingHelper
    {
        private static readonly ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
        private static readonly StorageFolder folder = ApplicationData.Current.LocalFolder;

        public static void AddOrUpdateValue(string key, object value)
        {

            if (settings.Values.ContainsKey(key))
            {
                if (settings.Values[key] != value)
                    settings.Values[key] = value;
            }
            else
                settings.Values.Add(key, value);
        }

        public static T GetValueOrDefault<T>(string key, T defaultValue)
        {
            T value;

            if (settings.Values.ContainsKey(key))
            {
                value = (T)settings.Values[key];
            }
            else
            {
                value = defaultValue;
            }
            return value;
        }

        public async static void SaveFile(string fileName, string content)
        {
            IStorageFolder destFolder = null;
            string destFileName;
            if (fileName.Contains("\\"))
            {
                string subFolderName = fileName.Split('\\')[0];
                if (!string.IsNullOrEmpty(subFolderName))
                {
                    bool IsFolderExist = true;
                    try
                    {
                        destFolder = await folder.GetFolderAsync(subFolderName);
                    }
                    catch
                    {
                        IsFolderExist = false;
                    }
                    if (!IsFolderExist)
                        destFolder = await folder.CreateFolderAsync(subFolderName);
                }
                else
                    destFolder = folder;

                destFileName = fileName.Split('\\')[1];
            }
            else
            {
                destFolder = folder;
                destFileName = fileName;
            }

            StorageFile destFile = await destFolder.CreateFileAsync(destFileName, CreationCollisionOption.ReplaceExisting);

            await FileIO.WriteTextAsync(destFile, content);
        }

        public async static Task<string> LoadFile(string fileName)
        {
            string content = string.Empty;

            try
            {
                StorageFile sourceFile = await folder.GetFileAsync(fileName);
                content = await FileIO.ReadTextAsync(sourceFile);
            }
            catch
            { }

            return content;
        }

        public async static Task<StorageFile> GetPackagedFile(string filePath)
        {
            StorageFolder installFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            string folderName = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileName(filePath);
            if (folderName != null)
            {
                StorageFolder subFolder = await installFolder.GetFolderAsync(folderName);
                return await subFolder.GetFileAsync(fileName);
            }
            else
            {
                return await installFolder.GetFileAsync(fileName);
            }
        }

    }
}
