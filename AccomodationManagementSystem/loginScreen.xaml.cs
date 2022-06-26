using System;
using System.Collections.Generic;
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
    /// Interaction logic for loginScreen.xaml
    /// </summary>
    public partial class loginScreen : Window
    {
        public loginScreen()
        {
            InitializeComponent();
        }
        private void buttonBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            buttonBorder.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#56B3F5");
        }

        private void buttonBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            buttonBorder.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#919AF4");
        }

        private void buttonBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            using (LoginDataContext context = new LoginDataContext()) {  // create context of SQL database
                var loginInfo = context.m_loginInfo.Where(info => info.user == "ADMIN").FirstOrDefault(); // the login info of the user
                if (loginInfo is not null) { // check there is a value within it
                    if (loginInfo.Password == LoginPasswordBox.Password) //check the password entered matches the database
                    {
                        //creates the main window and closes the current one
                        MainWindow MainApp = new MainWindow();
                        MainApp.Show();
                        this.Close();
                    }
                    else {
                        // if the password does not match show it.
                        errorLabel.Content = "Incorrect Password";
                    }
                }
            
            }
        }

    }



}
