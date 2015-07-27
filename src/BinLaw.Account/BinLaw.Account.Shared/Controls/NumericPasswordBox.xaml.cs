using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
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

namespace BinLaw.Account.Controls
{
    public partial class NumericPasswordBox : UserControl
    {
        #region Password

        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Password.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(NumericPasswordBox), new PropertyMetadata(null));

        #endregion

        #region MaxLength

        public int MaxLength
        {
            get { return (int)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxLengthProperty =
            DependencyProperty.Register("MaxLength", typeof(int), typeof(NumericPasswordBox), new PropertyMetadata(4,
                (o, e) =>
                {
                    var tb = o as TextBox;
                    if (tb != null)
                        tb.MaxLength = (int) e.NewValue;
                }));

        #endregion

        public NumericPasswordBox()
        {
            InitializeComponent();
            Password = string.Empty;
            PasswordTextBox.MaxLength = 4;
        }

        private void PasswordTextBox_KeyUp(object sender, RoutedEventArgs e)
        {
                
            if (Password==null||Password.Length == PasswordTextBox.Text.Length - 1)
                Password += PasswordTextBox.Text[PasswordTextBox.Text.Length - 1];
            else if (PasswordTextBox.Text.Length == 0)
            {
                return;
            }
            else
            {
                Password = Password.Remove(Password.Length - 1);
            }

            //replace text by *
            PasswordTextBox.Text = Regex.Replace(Password, @".", "●");

            //take cursor to end of string
            PasswordTextBox.SelectionStart = PasswordTextBox.Text.Length;
        }
    }
}
