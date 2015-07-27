using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;

namespace BinLaw.Account.FE.Foundation.Extension
{
    public static class StorageFolderExtension
    {
        public static async Task<bool> CheckFileExisted(this StorageFolder folder, string fileName)
        {
            try
            {
                await folder.GetFileAsync(fileName);
                return true;
            }
            catch
            {
                return false;
            }

        }
    }
}
