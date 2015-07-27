using System;
using System.Threading.Tasks;
using Windows.Storage;
using BinLaw.Account.FE.Common;
using BinLaw.Account.FE.Foundation.Helper;
using BinLaw.Account.FE.Foundation.Interface;
using BinLaw.Account.Service;
using BinLaw.Account.View;
using GalaSoft.MvvmLight.Command;

namespace BinLaw.Account.ViewModel
{
    public class InitialFirstViewModel
    {
        private INavigationService _navigationService;
        private DataService _dataService;

        public InitialFirstViewModel(INavigationService navigationService, DataService dataService)
        {
            _navigationService = navigationService;
            _dataService = dataService;
        }

        private RelayCommand _onPageLoadedCommand;
        private RelayCommand _navigateToMainPage;

        public RelayCommand OnPageLoadedCommand
        {
            get
            {
                return _onPageLoadedCommand ?? (_onPageLoadedCommand = new RelayCommand(async () =>
                {
                    await initialingTask();

                    if (AppSettingHelper.GetValueOrDefault(Constants.VERISION_FILENAME, string.Empty) !=
               string.Empty)
                    {
                        //await UpdateMonthAssert(7);
                        //await UpdateMonthAssert(8);
                        //await UpdateMonthAssert(9);
                    }

                    if (AppSettingHelper.GetValueOrDefault(Constants.APP_DEVICEID, string.Empty) == string.Empty)
                    {
                        AppSettingHelper.AddOrUpdateValue(Constants.APP_DEVICEID, Guid.NewGuid().ToString("N"));
                    }
                }));
            }
        }

        public RelayCommand NavigateToMainPage
        {
            get
            {
                return _navigateToMainPage ?? (_navigateToMainPage = new RelayCommand(() =>
                    {
                        _navigationService.Navigate<MainPage>();

                        //Final Update VersionInfo To AppSetting
                        AppSettingHelper.AddOrUpdateValue(Constants.VERISION_FILENAME, DeviceHelper.GetCurrentAppVersion());
                    }));
            }
        }

        private async Task initialingTask()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            var str = await ResHelper.ReadResourceAsync(
            Constants.CATEGORY_FILEPATH);
            var file = await localFolder.CreateFileAsync(Constants.CATEGORY_FILENAME, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, str);

        }

    }
}
