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
using AccomodationManagementSystem.VacancyDatabaseClasses;

namespace AccomodationManagementSystem
{
    /// <summary>
    /// Interaction logic for AddRoomWindow.xaml
    /// </summary>
    public partial class AddRoomWindow : Window
    {
        private AccomodationContext context = new AccomodationContext();

        public AddRoomWindow()
        {
            InitializeComponent();
        }

        private void Save_B_Click(object sender, RoutedEventArgs e)
        {
            if (validateResponse()) {
                
                roomInfo temp = new roomInfo() { id = int.Parse(RoomNumber_TB.Text), RoomType = RoomType_TB.Text };
                context.Add(temp);
                context.SaveChanges();
                MessageBox.Show("Save Successful", "New Room");
                this.Close();
                Application.Current.Windows.OfType<MainWindow>().FirstOrDefault().GenerateTable();
            }
            
        }
        private bool validateResponse() {

            if (RoomType_TB.Text == null) { MessageBox.Show("Please enter a room type. EG Queen", "Error Saving"); return false; };
            if (!int.TryParse(RoomNumber_TB.Text, out int number) && number < 0) { MessageBox.Show("Please enter a room number. EG 1, 2, 3 ...", "Error Saving"); return false; }
            if (context.m_rooms.Find(int.Parse(RoomNumber_TB.Text)) != null) { MessageBox.Show("There is already an existing room with that number", "Error Saving"); return false; } 
            return true;
        
        }
    }
}
