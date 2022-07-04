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

using System.IO;
using Microsoft.Win32;
using System.Diagnostics;



namespace AccomodationManagementSystem
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            loadCurrentSettings();

        }
        private void loadCurrentSettings() {

            using (LoginDataContext context = new LoginDataContext())
            {
                RandomCellColour_CB.IsChecked = context.m_loginInfo.Where(userInfo => userInfo.user == "ADMIN").FirstOrDefault().randomCellColour_S;
                ColumnGenerated_TB.Text = context.m_loginInfo.Where(userInfo => userInfo.user == "ADMIN").FirstOrDefault().columnGenerated_S.ToString();
            }
            
        }

        private bool ValidateResponses() {

            if (ColumnGenerated_TB.Text == "")
            {
                MessageBox.Show("Please enter a valid columns shown value", "Error Saving");
                return false;
            }
            if (!int.TryParse(ColumnGenerated_TB.Text, out int value)) {
                MessageBox.Show("Please enter a valid columns shown value", "Error Saving");
                return false;
            }

            return true;

        }

        private void EditRoom_B_Click(object sender, RoutedEventArgs e)
        {
            EditRoomWindow editRoom = new EditRoomWindow();
            editRoom.ShowDialog();
        }

        private void AddRoom_B_Click(object sender, RoutedEventArgs e)
        {
            // when the add room button is clicked then create the add room window
            AddRoomWindow addRoomWindow = new AddRoomWindow();
            addRoomWindow.ShowDialog();
        }

        private void LoadBackUp_B_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (!Directory.Exists(Environment.CurrentDirectory + "\\BackUps"))
            {
                System.IO.Directory.CreateDirectory(Environment.CurrentDirectory + "\\BackUps");
            }
            openFileDialog.Filter = "Database Files (*.db)|*.db";
            openFileDialog.InitialDirectory = Environment.CurrentDirectory + "\\BackUps";
            openFileDialog.ShowDialog();
            
            if (openFileDialog.FileName == Environment.CurrentDirectory + "\\accommodation.db")
            {
                MessageBox.Show("You cannot select the current database to load", "Error");
                return;
            }

            if(MessageBox.Show("Are you sure you want to load the backup, all data in the current database will be overwritten and lost?", "Load Backup", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    //variable storing the path to the current database
                    string dataBasePath = Environment.CurrentDirectory + "\\accommodation.db";
                    File.Delete(dataBasePath);
                    //copy the current database to the backupdatabase file
                    File.Copy(openFileDialog.FileName, dataBasePath, true);

                }
                catch
                {
                    MessageBox.Show("Error loading backup", "Load Backup");
                    return;
                }
                MessageBox.Show("Successfully Loaded Backup", "Load Backup");
                Application.Current.Windows.OfType<MainWindow>().FirstOrDefault().GenerateTable();


            }

        }

        private void ChangePass_B_Click(object sender, RoutedEventArgs e)
        {

            InputPrompt prompt = new InputPrompt("User Confirmation", "Current Password", true);
            prompt.ShowDialog();
            using(LoginDataContext context = new LoginDataContext())
            {

                if (prompt.input == context.m_loginInfo.Where(userInfo => userInfo.user == "ADMIN").FirstOrDefault().Password)
                {
                    CreatePassword createPassword = new CreatePassword(true);
                    createPassword.ShowDialog();
                } else
                {
                    MessageBox.Show("Wrong Password", "Error");
                }

            }



        }

        private void Save_B_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateResponses()) return;
            using (LoginDataContext context = new LoginDataContext())
            {
                context.m_loginInfo.Where(userInfo => userInfo.user == "ADMIN").FirstOrDefault().randomCellColour_S = (bool)RandomCellColour_CB.IsChecked;
                Trace.WriteLine(int.Parse(ColumnGenerated_TB.Text));
                context.m_loginInfo.Where(userInfo => userInfo.user == "ADMIN").FirstOrDefault().columnGenerated_S = int.Parse(ColumnGenerated_TB.Text);
                this.Close();
                context.SaveChanges();
            }
            Application.Current.Windows.OfType<MainWindow>().FirstOrDefault().LoadUserSettings();

            Application.Current.Windows.OfType<MainWindow>().FirstOrDefault().GenerateTable();


        }
    }
}
