using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Security.Credentials;
using Windows.System;
using BinLaw.Account.FE.Common;
using BinLaw.Account.FE.Foundation.Helper;
using BinLaw.Account.FE.Foundation.Interface;
using BinLaw.Account.Model;
using BinLaw.Account.Service;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.WindowsAzure.MobileServices;

namespace BinLaw.Account.ViewModel.Setting
{
    public class SettingViewModel : ViewModelBase
    {
        #region [ Data ]
        private MobileServiceUser user;
        private bool _isLogin;
        private bool _isHavePassword;
        private bool _isEditPassword;
        private readonly DataService _dataService;
        private readonly IToastService _toastService;
        private readonly ResourceLoader _resourceLoader;

        private string _password;

        #region 是否登陆 IsLogin
        /// <summary>
        /// 是否登陆
        /// </summary>
        public bool IsLogin
        {
            get { return _isLogin; }
            set { Set(() => IsLogin, ref _isLogin, value); }
        } 
        #endregion

        #region 是否已经设置密码 IsHavePassword
        /// <summary>
        /// 是否已经设置密码
        /// </summary>
        public bool IsHavePassword
        {
            get { return _isHavePassword; }
            set { Set(() => IsHavePassword, ref _isHavePassword, value); }
        } 
        #endregion

        #region 是否编辑密码 IsEditPassword
        /// <summary>
        /// 是否编辑密码
        /// </summary>
        public bool IsEditPassword
        {
            get { return _isEditPassword; }
            set { Set(() => IsEditPassword, ref _isEditPassword, value); }
        } 
        #endregion

        #region 已输入的密码 Password
        /// <summary>
        /// 已输入的密码
        /// </summary>
        public string Password
        {
            get { return _password; }
            set { Set(() => Password, ref _password, value); }
        } 
        #endregion

        
        #endregion

        #region [ Ctor ]
        public SettingViewModel(DataService dataService, IToastService toastService)
        {
            _dataService = dataService;
            _toastService = toastService;
            _resourceLoader = new ResourceLoader();
            if (IsInDesignMode)
            {
                IsHavePassword = true;
            }
        } 
        #endregion

        #region [ Command ]
        private RelayCommand _onPageLoadedCommand;
        private RelayCommand _connectAccountCommand;
        private RelayCommand _disconnectAccountCommand;
        private RelayCommand _setPasswordCommand;
        private RelayCommand _passwordToggleSwitchCommand;
        private RelayCommand _passwordEditCommand;
        private RelayCommand _privacyStatementCommand;

        #region 页面加载完毕后执行 OnPageLoadCommand
        /// <summary>
        /// It will execute when page loaded
        /// 当页面载入完毕后执行的事件
        /// </summary>
        public RelayCommand OnPageLoadedCommand
        {
            get
            {
                return _onPageLoadedCommand ?? (_onPageLoadedCommand = new RelayCommand(() =>
                    {
                        IsLogin = App.MobileService.CurrentUser != null;
                        IsHavePassword = AppSettingHelper.GetValueOrDefault(Constants.APP_PASSWORD, string.Empty) !=
                                     string.Empty;
                        IsEditPassword = false;
                    }));
            }
        }
        #endregion

        #region 连接微软账户 ConnectAccountCommand
        /// <summary>
        /// Connect Microsoft Account
        /// 连接微软账户
        /// </summary>
        public RelayCommand ConnectAccountCommand
        {
            get
            {
                return _connectAccountCommand ?? (_connectAccountCommand = new RelayCommand(async () =>
                {
                    await AuthenticateAsync();
                    await _dataService.PullAsync<BillModel>().ConfigureAwait(false);
                }));
            }
        } 
        #endregion

        #region 取消账户连接 DisConnectAccountCommand
        /// <summary>
        /// Disconnect Microsoft Account, Clear authenticated data
        /// 取消账户连接
        /// </summary>
        public RelayCommand DisConnectAccountCommand
        {
            get { return _disconnectAccountCommand ?? (_disconnectAccountCommand = new RelayCommand(DisAuthenticateAsync)); }
        }
        #endregion

        #region 设置App密码 SetPasswordCommand
        /// <summary>
        /// Set app password
        /// 设置App密码
        /// </summary>
        public RelayCommand SetPasswordCommand
        {
            get
            {
                return _setPasswordCommand ?? (_setPasswordCommand = new RelayCommand(() =>
                {
                    if (!string.IsNullOrWhiteSpace(Password) && Password.Length == 4)
                    {
                        AppSettingHelper.AddOrUpdateValue(Constants.APP_PASSWORD, Password);
                    }
                    else
                    {
                        _toastService.ShowMessage(_resourceLoader.GetString("ValidPassword"));
                        Password = string.Empty;
                    }
                    Password = string.Empty;
                    IsEditPassword = false;
                    _toastService.ShowMessage(_resourceLoader.GetString("SetSuccess"));
                }));
            }
        }
        #endregion

        #region 切换是否需要密码 PasswordToggleSwitchCommand
        /// <summary>
        /// Toggle it should have password 
        /// 切换是否需要密码
        /// </summary>
        public RelayCommand PasswordToggleSwitchCommand
        {
            get
            {
                return _passwordToggleSwitchCommand ?? (_passwordToggleSwitchCommand = new RelayCommand(() =>
            {
                if (!IsHavePassword)
                    AppSettingHelper.AddOrUpdateValue(Constants.APP_PASSWORD, string.Empty);
                else
                    IsEditPassword = true;
            }));
            }
        } 
        #endregion

        #region 编辑密码 PasswordEditCommand
        /// <summary>
        /// Edit password,when password is set
        /// 编辑密码
        /// </summary>
        public RelayCommand PasswordEditCommand
        {
            get
            {
                return _passwordEditCommand ?? (_passwordEditCommand = new RelayCommand(() =>
                    {
                        IsEditPassword = true;
                    }));
            }
        } 
        #endregion

        #region 隐私协议 PrivacyStatementCommand
        /// <summary>
        /// Privacy Statement
        /// 隐私协议
        /// </summary>
        public RelayCommand PrivacyStatementCommand
        {
            get
            {
                return _privacyStatementCommand ?? (_privacyStatementCommand = new RelayCommand(async () =>
                {
                    await Launcher.LaunchUriAsync(new Uri("http://mybill.azurewebsites.net/PrivacyStatement"));
                }));
            }
        } 
        #endregion
        #endregion

        #region [ Method ]
        private async Task AuthenticateAsync()
        {
            var provider = "MicrosoftAccount";

            // Use the PasswordVault to securely store and access credentials.
            PasswordVault vault = new PasswordVault();

            try
            {
                // Login with the identity provider.
                user = await App.MobileService
                    .LoginAsync(provider);

                // Create and store the user credentials.
                PasswordCredential credential = new PasswordCredential(provider,
                    user.UserId, user.MobileServiceAuthenticationToken);
                vault.Add(credential);
                IsLogin = true;
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                Debug.WriteLine("You must log in. Login Required");
            }

        }

        private void DisAuthenticateAsync()
        {
            App.MobileService.Logout();
            var provider = "MicrosoftAccount";

            // Use the PasswordVault to securely store and access credentials.
            PasswordVault vault = new PasswordVault();
            PasswordCredential credential = null;

            try
            {
                // Try to get an existing credential from the vault.
                credential = vault.FindAllByResource(provider).FirstOrDefault();
                IsLogin = false;
            }
            catch (Exception)
            {
                // When there is no matching resource an error occurs, which we ignore.
            }
            if (credential != null)
            {
                vault.Remove(credential);
            }
        }
        #endregion

    }
}
