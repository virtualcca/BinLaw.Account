using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236
using BinLaw.Account.Common;

namespace BinLaw.Account.Controls
{
    public sealed partial class DateBarPickerControl : UserControl
    {
        private DateTime _oriStartTime;
        private DateTime _oriEndTime;


        public DateTime StartDateTime
        {
            get { return (DateTime)GetValue(StartDateProperty); }
            set { SetValue(StartDateProperty, value); }
        }



        public DateTime EndDateTime
        {
            get { return (DateTime)GetValue(EndDateProperty); }
            set { SetValue(EndDateProperty, value); }
        }



        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        #region [ DependencyProperty ]


        public static readonly DependencyProperty StartDateProperty = DependencyProperty.Register(
           "StartDateTime", typeof(DateTime), typeof(DateBarPickerControl), new PropertyMetadata(DateTime.Now.AddMonths(-1).Date,
           (o, e) =>
           {
               var sender = o as DateBarPickerControl;
               if (sender != null)
               {
                   sender.EndDatePicker.MinYear = new DateTimeOffset((DateTime)e.NewValue);
               }
           }));

        public static readonly DependencyProperty EndDateProperty = DependencyProperty.Register(
            "EndDateTime", typeof(DateTime), typeof(DateBarPickerControl), new PropertyMetadata(DateTime.Now.Date, (o, e) =>
            {
                var sender = o as DateBarPickerControl;
                if (sender != null)
                {
                    sender.StartDatePicker.MaxYear = new DateTimeOffset((DateTime)e.NewValue);
                }
            }));

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
          "Command", typeof(ICommand), typeof(DateBarPickerControl), new PropertyMetadata(default(ICommand)));
        #endregion

        public DateBarPickerControl()
        {
            this.InitializeComponent();

            StartDateTime = DateTime.Now.AddMonths(-1).Date;
            _oriStartTime = StartDateTime;
            StartDatePicker.Date = new DateTimeOffset(StartDateTime);

            EndDateTime = DateTime.Now.Date;
            _oriEndTime = EndDateTime;
            EndDatePicker.Date = new DateTimeOffset(EndDateTime);

        }

        private void StartDatePicker_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            _oriStartTime = StartDateTime;
            StartDateTime = e.NewDate.DateTime;
        }

        private void EndDatePicker_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            _oriEndTime = EndDateTime;
            EndDateTime = e.NewDate.DateTime;
        }


        private void ToogleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            
        }

        private void ToogleButton_Checked(object sender, RoutedEventArgs e)
        {
        }

        private void AcceptAppbarButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (Command != null)
                Command.Execute(null);
            ToggleButton.IsChecked = false;
        }

        private void CancelAppbarButton_OnClick(object sender, RoutedEventArgs e)
        {
            StartDateTime = _oriStartTime;
            EndDateTime = _oriEndTime;
            ToggleButton.IsChecked = false;
        }

        private void DataBar_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is ToggleButton && !(sender is DateBarPickerControl))
                ToggleButton.IsChecked = false;
        }
    }
}
