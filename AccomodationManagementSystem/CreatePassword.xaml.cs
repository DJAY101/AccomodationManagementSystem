using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AccomodationManagementSystem
{
    /// <summary>
    /// Interaction logic for CreatePassword.xaml
    /// </summary>
    public partial class CreatePassword : Window
    {
        public CreatePassword()
        {
            InitializeComponent();
        }


        private void buttonBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            buttonBorder.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#56B3F5");
            Trace.WriteLine("In");
        }

        private void buttonBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            buttonBorder.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#919AF4");
            Trace.WriteLine("Out");
        }

        private void buttonBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (CreatePasswordBox.Password == "") {
                errorLabel.Content = "Please Enter a Password";
            } else if(ConfirmPasswordBox.Password == "")
            {
                errorLabel.Content = "Please Confirm the Password";
            } else if ( CreatePasswordBox.Password != ConfirmPasswordBox.Password) {
                errorLabel.Content = "The Passwords do not match";
            }
        }
    }
}
