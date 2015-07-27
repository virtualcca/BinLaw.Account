using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236
using BinLaw.Account.Helper;
using BinLaw.Account.Service;

namespace BinLaw.Account.Controls
{
    public sealed partial class ToastPrompt : UserControl
    {
        private DispatcherTimer timer;
        private bool isShow;

        public ToastPrompt()
        {
            this.InitializeComponent();
            if (timer == null)
            {
                timer = new DispatcherTimer();
            }

            var halfWidth = App.ContentFrame.ActualWidth * 0.5;
            var fullWidth = App.ContentFrame.ActualWidth;

            LayoutRoot.Width = halfWidth;
            var inAnimation = StoryboardToastIn.Children[0] as DoubleAnimation;
            inAnimation.From = fullWidth;
            inAnimation.To = halfWidth;

            var outAnimation = StoryboardPopupOut.Children[0] as DoubleAnimation;
            outAnimation.From = halfWidth;
            outAnimation.To = fullWidth;


            StoryboardToastIn.Completed += StoryboardToastIn_Completed;
            StoryboardPopupOut.Completed += StoryboardPopupOut_Completed;
        }

        public void ShowMessage(string content)
        {
            txtContent.Text = content;
            if (!isShow)
                StoryboardToastIn.Begin();
            else
            {
                StoryboardPopupOut.Begin();
                StoryboardToastIn.Begin();
            }
            isShow = true;
        }

        void StoryboardToastIn_Completed(object sender, object e)
        {
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        void timer_Tick(object sender, object e)
        {
            timer.Stop();
            StoryboardPopupOut.Begin();
        }



        void StoryboardPopupOut_Completed(object sender, object e)
        {
            StoryboardPopupOut.Stop();            //停止消失动画
            isShow = false;
            ToastService.popup.Child = null;
            ToastService.popup.IsOpen = false;    //关闭Popup
        }

        private void LayoutRoot_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            timer.Stop();
        }
    }
}
