using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236
using BinLaw.Account.FE.Common;
using BinLaw.Account.FE.Foundation.Helper;
using BinLaw.Account.ViewModel;

namespace BinLaw.Account.Controls
{


    public sealed partial class AppLockControl : UserControl
    {
        private readonly string _password;

        public static readonly DependencyProperty IsLockProperty = DependencyProperty.Register(
            "IsLock", typeof(bool), typeof(AppLockControl), new PropertyMetadata(default(bool)));

        public bool IsLock
        {
            get { return (bool)GetValue(IsLockProperty); }
            set { SetValue(IsLockProperty, value); }
        }

        public AppLockControl()
        {
            this.InitializeComponent();
            _password = AppSettingHelper.GetValueOrDefault(Constants.APP_PASSWORD, string.Empty);
        }

        private void NumberBtn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.Content != null)
            {
                if (btn.Content.ToString() == "x")
                {
                    PasswordBox.Password = PasswordBox.Password.Length>0?PasswordBox.Password.Remove(PasswordBox.Password.Length - 1, 1):string.Empty;
                }
                else
                {
                    PasswordBox.Password += btn.Content.ToString();
                }

            }

        }

        private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Password == _password)
            {
                ViewModelLocator.AppViewModel.IsLock = false;
            }
        }
    }
}
