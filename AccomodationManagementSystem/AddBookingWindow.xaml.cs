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
    /// Interaction logic for AddBookingWindow.xaml
    /// </summary>
    public partial class AddBookingWindow : Window
    {
        private DateTime dateFrom;
        private int roomNumber;
        private int stayDuration = 0;
        public AddBookingWindow(string date, int roomNumber)
        {
            InitializeComponent();
            this.dateFrom = new DateTime(int.Parse(date.Split("-")[2]), int.Parse(date.Split("-")[1]), int.Parse(date.Split("-")[0]));
            this.roomNumber = roomNumber;
            loadInfoTexts();
            //set checkout calendar start date
            checkOut_DP.DisplayDateStart = dateFrom.AddDays(1);
        }

        private void loadInfoTexts()
        {
            DateFrom_T.Text = "Check-In: " + dateFrom.ToString("dd-MM-yyyy");
            title_T.Text = "Booking Entry - Room " + roomNumber.ToString();

        }

        private void checkOut_DP_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (checkOut_DP.SelectedDate != null)
            {
                TimeSpan stayDuration = (DateTime)checkOut_DP.SelectedDate - dateFrom;
                this.stayDuration = stayDuration.Days;
                StayDuration_T.Text = "Number of Nights: " + stayDuration.Days.ToString();
            }
            else
            {
                StayDuration_T.Text = "Number of Nights: 0";
                this.stayDuration = 0;

            }
            updateCost();

        }
        private void updateCost()
        {
            if (float.TryParse(DailyRate_TB.Text, out float rate))
            {
                Total_T.Text = "Total: $" + Math.Round(rate * stayDuration, 2).ToString();
                GST_T.Text = "GST: $" + Math.Round(rate * stayDuration * 0.1, 2).ToString();
                WithoutGST_T.Text = "Total Without GST: $" + Math.Round((rate * stayDuration) - (rate * stayDuration * 0.1), 2);
            }
        }

        private void DailyRate_TB_TextChanged(object sender, TextChangedEventArgs e)
        {
            updateCost();
        }

        private void Save_B_Click(object sender, RoutedEventArgs e)
        {
            if (!validateResponses()) return;
            using (AccomodationContext context = new AccomodationContext())
            {
                bookingInfo temp = new bookingInfo() { FirstName = FirstName_TB.Text, Surname = Surname_TB.Text, ArrivalTime = CheckInTime_TB.Text, ExtraDetails = ExtraDetails_TB.Text, BookTime = DateTime.Now.ToString("dd-MM-yyyy"), CheckInDate = dateFrom.ToString("dd-MM-yyyy"), CheckOutDate = ((DateTime)checkOut_DP.SelectedDate).ToString("dd-MM-yyyy"), DailyRate = float.Parse(DailyRate_TB.Text), PhoneNumber = PhoneNumber_TB.Text, RoomId = roomNumber };
                context.Add(temp);
                context.SaveChanges();
            }
            this.Close();

        }

        private bool validateResponses()
        {

            bool checkoutDateSelected = checkOut_DP.SelectedDate != null;
            bool validRate = float.TryParse(DailyRate_TB.Text, out float rate);
            if (!checkoutDateSelected)
            {
                MessageBox.Show("Please select a checkout date.", "Error Saving");
            }
            else if (!validRate)
            {
                MessageBox.Show("Please enter a valid nightly rate.", "Error Saving");
            }
            else
            {
                return true;
            }
            return false;


        }
    }
}
