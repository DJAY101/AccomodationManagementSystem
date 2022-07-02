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
    /// Interaction logic for EditRoomWindow.xaml
    /// </summary>
    public partial class EditRoomWindow : Window
    {
        private Dictionary<int, string> rooms_D = new Dictionary<int, string>();
        private List<int> rooms_L = new List<int>();
        public EditRoomWindow()
        {
            InitializeComponent();
            loadAllRooms();
            RoomSelecter_CB.ItemsSource = rooms_L;

        }
        private void loadAllRooms() {

            using (AccomodationContext context = new AccomodationContext()) { 
            
                foreach(roomInfo room in context.m_rooms)
                {
                    rooms_D.Add(room.id, room.RoomType);
                }
                rooms_L = rooms_D.Keys.ToList();

            }
        
        }

        private void RoomSelecter_CB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RoomSelecter_CB.SelectedIndex == -1) return;
            RoomType_TB.Text = rooms_D[(int)RoomSelecter_CB.SelectedValue];
        }

        private void Delete_B_Click(object sender, RoutedEventArgs e)
        {
            if (RoomSelecter_CB.SelectedIndex == -1) return;
            if (MessageBox.Show("Are you sure you want to delete room " + ((int) RoomSelecter_CB.SelectedValue).ToString() + "? \nThis action is permanent and all bookings contained in the room will be deleted!", "Room Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No) return;

            using (AccomodationContext context = new AccomodationContext()) {
               var bookingDeletion = context.m_bookings.Where(booking => booking.RoomId == (int)RoomSelecter_CB.SelectedValue);
                foreach (var booking in bookingDeletion) {
                    context.m_bookings.Remove(booking);
                }
                context.m_rooms.Remove(context.m_rooms.Find((int)RoomSelecter_CB.SelectedValue));
                context.SaveChanges();
            }
            this.Close();
            Application.Current.Windows.OfType<MainWindow>().FirstOrDefault().GenerateTable();
        }

        private void Save_B_Click(object sender, RoutedEventArgs e)
        {
            if (RoomSelecter_CB.SelectedIndex == -1) { MessageBox.Show("Please select a room to edit changes.", "Error Saving");  return;}
            if (RoomType_TB.Text == "") { MessageBox.Show("Please complete the field room type.\nE.G. Queen, Double, Single", "Error Saving"); return; }

            using (AccomodationContext context = new AccomodationContext()) { 
            
                context.m_rooms.Find((int) RoomSelecter_CB.SelectedItem).RoomType = RoomType_TB.Text;
                context.SaveChanges();
                this.Close();
                MessageBox.Show("Successfully Saved", "Room Edit");
            }


        }
    }
}
