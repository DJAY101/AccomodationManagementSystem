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
            setup();


        }

        private void setup() {
            loadInfoTexts();
            setCheckOutAvailableDates();
            checkOut_DP.SelectedDate = dateFrom.AddDays(1); //default the checkout date to be the day after check in

        }

        // converts strings in the form of dd-MM-yyyy to a DateTime Object
        private DateTime DatabaseDateTimeStringToDateTime(string date)
        {
            return new DateTime(int.Parse(date.Split("-")[2]), int.Parse(date.Split("-")[1]), int.Parse(date.Split("-")[0]));
        }

        private void setCheckOutAvailableDates() {
            //set checkout calendar start date
            checkOut_DP.DisplayDateStart = dateFrom.AddDays(1);
            //set checkout calender end date to the next date that has a booking
            using (AccomodationContext context = new AccomodationContext()) {

                //get all bookings that contains the current selected room id and store in a read only list
                IEnumerable<bookingInfo> bookings = context.m_bookings.Where(booking => booking.RoomId == roomNumber);
                //sort the bookings by the checkin date
                var sortedBookings = from entry in bookings orderby DatabaseDateTimeStringToDateTime(entry.CheckInDate).Ticks ascending select entry;
                // going through each booking in sorted bookings
                foreach (bookingInfo booking in sortedBookings) {
                    //if the booking's checkin date is greater than the new booking's check in date then set the checkout limit to this bookings checkin date.
                    if (DatabaseDateTimeStringToDateTime(booking.CheckInDate).Ticks >= dateFrom.AddDays(1).Ticks) {
                        checkOut_DP.DisplayDateEnd = DatabaseDateTimeStringToDateTime(booking.CheckInDate);
                        break;
                    }

                }
            
            }

        }

        private void loadInfoTexts()
        {
            //load text elements from the database
            using (AccomodationContext context = new AccomodationContext())
            {
                DateFrom_T.Text = "Check-In: " + dateFrom.ToString("dd-MM-yyyy");
                title_T.Text = "Booking Entry - Room " + roomNumber.ToString() + " " + context.m_rooms.Find(roomNumber).RoomType + " Room";
            }
        }

        private void checkOut_DP_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            //When the checkout date has been changed check if its null if so then number of nights is 0 otherwise calculate the number of nights
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
            //updates the cost texts on the window according to the current provided information
            if (float.TryParse(DailyRate_TB.Text, out float rate))
            {
                Total_T.Text = "Total: $" + Math.Round(rate * stayDuration, 2).ToString();
                GST_T.Text = "GST: $" + Math.Round(rate * stayDuration * 0.1, 2).ToString();
                WithoutGST_T.Text = "Total Without GST: $" + Math.Round((rate * stayDuration) - (rate * stayDuration * 0.1), 2);
            }
        }

        private void DailyRate_TB_TextChanged(object sender, TextChangedEventArgs e)
        {
            //when the daily rate has been changed update the cost
            updateCost();
        }

        private void Save_B_Click(object sender, RoutedEventArgs e)
        {
            if (!validateResponses()) return; // if the response is valid then continue

            //save an entry of this booking to the database
            using (AccomodationContext context = new AccomodationContext())
            {
                bookingInfo temp = new bookingInfo() { FirstName = FirstName_TB.Text, Surname = Surname_TB.Text, ArrivalTime = CheckInTime_TB.Text, ExtraDetails = ExtraDetails_TB.Text, BookTime = DateTime.Now.ToString("dd-MM-yyyy"), CheckInDate = dateFrom.ToString("dd-MM-yyyy"), CheckOutDate = ((DateTime)checkOut_DP.SelectedDate).ToString("dd-MM-yyyy"), DailyRate = float.Parse(DailyRate_TB.Text), PhoneNumber = PhoneNumber_TB.Text, RoomId = roomNumber };
                context.Add(temp);
                context.SaveChanges();
            }

            Application.Current.Windows.OfType<MainWindow>().FirstOrDefault().GenerateTable(); //regenerate the table on the main window

            //show a dialogue that the booking has been saved then close this window
            MessageBox.Show("Saved Successfully", "Booking");
            this.Close();

        }

        private bool validateResponses()
        {
            //validates the response, checks if a checkout date has been selected and a valid nightly rate has been entered

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
