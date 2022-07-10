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
        private int bookingID = -1;
        private bool editBooking = false;
        public AddBookingWindow(string date, int roomNumber, bool editBooking = false, int bookingID = -1)
        {
            InitializeComponent();
            this.dateFrom = new DateTime(int.Parse(date.Split("-")[2]), int.Parse(date.Split("-")[1]), int.Parse(date.Split("-")[0]));
            this.roomNumber = roomNumber;
            this.editBooking = editBooking;
            this.bookingID = bookingID;
            this.Title = editBooking ? "Edit Booking Window" : "Add Booking Window";
            setup();


        }

        private void setup() {
            loadInfoTexts();
            setCheckInAvailableDates();
            setCheckOutAvailableDates();
            if (editBooking)
            {
                using (AccomodationContext context = new AccomodationContext())
                {
                    checkOut_DP.SelectedDate = DatabaseDateTimeStringToDateTime(context.m_bookings.Find(bookingID).CheckOutDate);
                }
            } else {
                checkOut_DP.SelectedDate = dateFrom.AddDays(1); //default the checkout date to be the day after check in
            }
        }

        // converts strings in the form of dd-MM-yyyy to a DateTime Object
        private DateTime DatabaseDateTimeStringToDateTime(string date)
        {
            return new DateTime(int.Parse(date.Split("-")[2]), int.Parse(date.Split("-")[1]), int.Parse(date.Split("-")[0]));
        }


        private void setCheckInAvailableDates() {
        
            List<DateTime> unavailableDates = new List<DateTime>();
            using (AccomodationContext context = new AccomodationContext()) {

                List<bookingInfo> bookings;

                //checks if the window is in editing booking or adding booking mode, if its in editing mode then dont take into account the current booking's stay
                if (editBooking)
                {
                    bookings = context.m_bookings.Where(booking => booking.id != bookingID).Where(booking => booking.RoomId == roomNumber).ToList();
                }
                else { 
                    bookings = context.m_bookings.Where(booking => booking.RoomId == roomNumber).ToList();
                }
                //loops through all bookings attached to the selected room
                foreach (bookingInfo booking in bookings)
                {
                    DateTime checkIn = DatabaseDateTimeStringToDateTime(booking.CheckInDate);
                    DateTime checkOut = DatabaseDateTimeStringToDateTime(booking.CheckOutDate);
                    //loops from the check in to the check out date adding each date into the unavailable dates list
                    for (int dayOffset = 0; dayOffset < (checkOut-checkIn).Days; dayOffset++)
                    {
                        unavailableDates.Add(checkIn.AddDays(dayOffset));
                    }
                }
            }
            CheckIn_DP.BlackoutDates.Clear();
            foreach (DateTime date in unavailableDates)
            {
                //sets the blackoutdates for the dates that are unavailable
                CheckIn_DP.BlackoutDates.Add(new CalendarDateRange(date));
            }

        }
        private void setCheckOutAvailableDates() {
            //set checkout calendar start date
            checkOut_DP.DisplayDateStart = dateFrom.AddDays(1);
            //set checkout calender end date to the next date that has a booking
            using (AccomodationContext context = new AccomodationContext()) {

                IEnumerable<bookingInfo> bookings;
                //get all bookings that contains the current selected room id and store in a read only list
                if (editBooking)
                {
                    bookings = context.m_bookings.Where(booking => booking.id != bookingID).Where(booking => booking.RoomId == roomNumber);
                }
                else
                {
                    bookings = context.m_bookings.Where(booking => booking.RoomId == roomNumber);
                }
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
        //loads the title
        private void loadInfoTexts()
        {
            //load text elements from the database
            using (AccomodationContext context = new AccomodationContext())
            {
                CheckIn_DP.SelectedDate = dateFrom;
                title_T.Text = ((editBooking)? "Edit " : "") + "Booking Entry - Room " + roomNumber.ToString() + " " + context.m_rooms.Find(roomNumber).RoomType + " Room";
                if (editBooking)
                {
                    loadFromDatabase();
                }

            }
        }

        //if the window is in edit mode then load the booking details into text box
        private void loadFromDatabase() {
            using (AccomodationContext context = new AccomodationContext()) { 
                bookingInfo booking = context.m_bookings.Find(bookingID);
                FirstName_TB.Text = booking.FirstName;
                Surname_TB.Text = booking.Surname;
                PhoneNumber_TB.Text = booking.PhoneNumber;
                CheckInTime_TB.Text = booking.ArrivalTime;
                ExtraDetails_TB.Text = booking.ExtraDetails;
                DailyRate_TB.Text = booking.DailyRate.ToString();
                checkOut_DP.SelectedDate = DatabaseDateTimeStringToDateTime(booking.CheckOutDate);
            
            }
        }

        //when the check out date is changed
        private void checkOut_DP_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            updateStayDuration();
        }

        //when the check in date changes then update the available check out dates
        private void CheckIn_DP_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if(CheckIn_DP.SelectedDate != null) { dateFrom = (DateTime)CheckIn_DP.SelectedDate.Value; checkOut_DP.SelectedDate = CheckIn_DP.SelectedDate.Value.AddDays(1);}
            

            setCheckOutAvailableDates();
            updateStayDuration();
        }

        private void updateStayDuration() {

            //When the checkout date has been changed check if its null if so then number of nights is 0 otherwise calculate the number of nights
            if (checkOut_DP.SelectedDate != null && CheckIn_DP.SelectedDate != null)
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
                WithoutGST_T.Text = "Total Without GST: $" + Math.Round((rate * stayDuration) - (rate * stayDuration * 0.1), 2).ToString();
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
                if (editBooking)
                {
                    context.Remove(context.m_bookings.Find(bookingID));
                }
                context.Add(temp);
                context.SaveChanges();

                //foreach (var item in context.m_bookings)
                //{
                //    context.m_bookings.Remove(item);
                //}
                //context.SaveChanges();

                //for (int room = 1; room <= 10; room++)
                //{
                //    for (int dayOffset = 0; dayOffset <= 365*10; dayOffset++)
                //    {
                //        bookingInfo temp = new bookingInfo() { CheckInDate = DateTime.Now.AddDays(dayOffset).ToString("dd-MM-yyyy"), CheckOutDate = DateTime.Now.AddDays(dayOffset + 1).ToString("dd-MM-yyyy"), DailyRate = 100, FirstName = room.ToString() + dayOffset.ToString(), RoomId = room };
                //        context.Add(temp);


                //    }
                //}
                //context.SaveChanges();

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
            bool checkinDateSelected = CheckIn_DP.SelectedDate != null;
            bool validRate = float.TryParse(DailyRate_TB.Text, out float rate) && rate >= 0;
            if (!checkoutDateSelected)
            {
                MessageBox.Show("Please select a check-out date.", "Error Saving");
            }
            else if (!checkinDateSelected)
            {
                MessageBox.Show("Please select a check-in date.", "Error Saving");
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
