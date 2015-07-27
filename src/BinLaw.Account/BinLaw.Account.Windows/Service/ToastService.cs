using System;
using Windows.UI.Notifications;
using BinLaw.Account.FE.Foundation.Interface;
using NotificationsExtensions.ToastContent;

namespace BinLaw.Account.Service
{
    public class ToastService : IToastService
    {
        private IToastText01 toastContent;

        public void ShowMessage(string content)
        {
            if (toastContent == null)
            {
                toastContent = ToastContentFactory.CreateToastText01();
                toastContent.Duration = ToastDuration.Short;
                toastContent.Audio.Content = ToastAudioContent.Silent;
            }
            toastContent.TextBodyWrap.Text = content;
            ToastNotification toast = toastContent.CreateNotification();

            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }


    }
}
