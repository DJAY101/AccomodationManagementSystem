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
        bool editPassword;
        public CreatePassword(bool editPassword=false)
        {
            InitializeComponent();
            this.editPassword = editPassword;
            if (editPassword) loadEditPasswordWindow();
        }
        public CreatePassword()
        {
            InitializeComponent();
            this.editPassword = false;
        }

            private void loadEditPasswordWindow()
        {
            Title_L.Content = "Edit Password";
            Password_L.Content = "New Password";
            button_L.Content = "Save";
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

            if (CreatePasswordBox.Password == "") {
                errorLabel.Content = "Please Enter a Password";
            } else if(ConfirmPasswordBox.Password == "")
            {
                errorLabel.Content = "Please Confirm the Password";
            } else if ( CreatePasswordBox.Password != ConfirmPasswordBox.Password) {
                errorLabel.Content = "The Passwords do not match";
            }

            if (CreatePasswordBox.Password == ConfirmPasswordBox.Password) {
                using (LoginDataContext context = new LoginDataContext())
                {
                    if (editPassword)
                    {
                        context.m_loginInfo.Where(userInfo => userInfo.user == "ADMIN").FirstOrDefault().Password = ConfirmPasswordBox.Password;
                        context.SaveChanges();
                        this.Close();
                        return;
                    }

                    //creates a new login info into the database with the chosen password
                    loginInfo temp = new loginInfo();
                    temp.Password = ConfirmPasswordBox.Password;
                    temp.user = "ADMIN";
                    context.m_loginInfo.Add(temp);
                    context.SaveChanges();
                }
                //opens the main window and closes the current one
                MainWindow MainApp = new MainWindow();
                MainApp.Show();
                this.Close();

            }


        }
    }
}
