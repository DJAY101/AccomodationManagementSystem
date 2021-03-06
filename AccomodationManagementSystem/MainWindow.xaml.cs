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
using System.Windows.Navigation;
using System.Windows.Shapes;


using System.Diagnostics;
using AccomodationManagementSystem.VacancyDatabaseClasses;
using System.Windows.Threading;
using System.IO;

namespace AccomodationManagementSystem
{


    // Defines the vacancy data class, which will become the Item's class that is passed into the table
    public class vacancyData
    {
        //The room number of the row
        public int roomNumber { get; set; }
        //colour of cell in the row
        public List<Brush>? vacancyColour { get; set; }
        //booking id's of cell in the row
        public List<int>? bookingsIDs { get; set; }
        //the text the cell would display
        public List<string>? cellText { get; set; }
        public Brush? roomCellColour { get; set; } = new SolidColorBrush(Color.FromRgb(255, 255, 255));

    }


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        DateTime loadedMonth = new DateTime(1, 1, 1);
        private int datesGenerated = 30;
        private bool randomCellColours = true;

        public MainWindow()
        {
            InitializeComponent();
            LoadUserSettings();
            SetupTable();
            LoadCurrentMonth();
            GenerateTable();
            startDailyTimer();
        }

        public void LoadUserSettings() {
            //accesses the user settings database and sets the settings for loading the table
            using (LoginDataContext context = new LoginDataContext())
            {
                datesGenerated = context.m_loginInfo.Where(userInfo => userInfo.user == "ADMIN").FirstOrDefault().columnGenerated_S;
                randomCellColours = context.m_loginInfo.Where(userInfo => userInfo.user == "ADMIN").FirstOrDefault().randomCellColour_S;
            }
        }

        //Setup table is run once set the table settings EG column resizing and editing
        private void SetupTable()
        {
            //setup for the vacancy table
            vacancyTable.IsReadOnly = true;
            vacancyTable.CanUserResizeColumns = false;
            vacancyTable.CanUserResizeRows = false;
            vacancyTable.CanUserSortColumns = false;
            vacancyTable.CanUserReorderColumns =false;

            vacancyTable.FrozenColumnCount = 1; // the first column (room column) is frozing when scrolling horizontally 

            //set table selection to one cell
            vacancyTable.SelectionUnit = DataGridSelectionUnit.Cell;
            vacancyTable.SelectionMode = DataGridSelectionMode.Single;

            //apply a header style to the table
            var style = new Style(typeof(System.Windows.Controls.Primitives.DataGridColumnHeader));
            style.Setters.Add(new Setter { Property = System.Windows.Controls.Primitives.DataGridColumnHeader.FontSizeProperty, Value = 25.0 });
            style.Seal();

            vacancyTable.ColumnHeaderStyle = style;

        }

        private void LoadCurrentMonth()
        {
            //selects the current month to show
            loadedMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            updateCurrentMonthText();
        }


        // converts strings in the form of dd-MM-yyyy to a DateTime Object
        private DateTime DatabaseDateTimeStringToDateTime(string date)
        {
            return new DateTime(int.Parse(date.Split("-")[2]), int.Parse(date.Split("-")[1]), int.Parse(date.Split("-")[0]));
        }

        public bool test() {
            return false;
        }
        private void loadFirstColumn() {
            //create a new column object
            DataGridTextColumn firstColumn = new DataGridTextColumn();

            // Binds the content of the first column to the "roomNumber" variable of the Item
            firstColumn.Binding = new Binding("roomNumber");
            
            //Sets the header to "Room"
            firstColumn.Header = "Room";
            //create a new style for the column textblocks
            var styleColumn = new Style(typeof(TextBlock));
            styleColumn.Setters.Add(new Setter { Property = TextBlock.FontSizeProperty, Value = 30.0 });
            styleColumn.Seal();


            //apply the style
            firstColumn.ElementStyle = styleColumn;
            //add the column to the table
            vacancyTable.Columns.Add(firstColumn);
        }

        // Refreshes/generates the table

        public void GenerateTable()
        {

            //DateTime TimerStart = DateTime.Now;
            List<string> loadedDates = new List<string>();

            //Clears the current table
            vacancyTable.Columns.Clear();
            vacancyTable.Items.Clear();

            //loads the first column with all the room numbers
            loadFirstColumn();

            //The first and last day of the current loaded month
            DateTime firstDay = loadedMonth;
            DateTime lastDay = loadedMonth.AddDays(datesGenerated);

            //iterates through all the dates of the month (sets up all the columns and binds their cell style)
            for (int i = 0; i < (lastDay-firstDay).Days; i++)
            {
                //create a new column and give it a header of its date
                DataGridTextColumn column = new DataGridTextColumn();
                column.Header = firstDay.AddDays(i).ToString("dd-MM");
                //binds the cell text attribute of the item, to the columns cells
                column.Binding = new Binding("cellText[" + i + "]");

                //If the header date is the current date then change the colour of the header cell
                if (firstDay.AddDays(i).Date == DateTime.Now.Date)
                {
                    Brush CurrentDateHeaderColour = new SolidColorBrush(Color.FromRgb(218, 106, 67));
                    Brush White = new SolidColorBrush(Color.FromRgb(255, 255, 255));

                    Style CurrentDateHeaderStyle = new Style();

                    CurrentDateHeaderStyle.Setters.Add(new Setter() { Property = BackgroundProperty, Value = CurrentDateHeaderColour });
                    CurrentDateHeaderStyle.Setters.Add(new Setter() { Property = ForegroundProperty, Value = White });
                    CurrentDateHeaderStyle.Setters.Add(new Setter { Property = TextBlock.FontSizeProperty, Value = 30.0 });
                    CurrentDateHeaderStyle.Seal();
                    column.HeaderStyle = CurrentDateHeaderStyle;
                }


                Brush selectedColour = new SolidColorBrush(Color.FromRgb(255, 80, 35));

                //colour the cell depending on the whether the room is booked (binding the cell background colour to the vacancyColour atribute of the item)
                Style cStyle = new Style(typeof(DataGridCell));
                cStyle.Setters.Add(new Setter() { Property = BackgroundProperty, Value = new Binding("vacancyColour[" + i + "]") });
                cStyle.Setters.Add(new Setter() { Property = ForegroundProperty, Value = new SolidColorBrush(Color.FromRgb(255, 255, 255)) });

                //trigger when a cell is selected change its cell colour
                Trigger selectedTrig = new Trigger() { Property = DataGridCell.IsSelectedProperty, Value = true };
                selectedTrig.Setters.Add(new Setter() { Property = BackgroundProperty, Value = selectedColour });
                cStyle.Triggers.Add(selectedTrig);
                // apply the style to the column
                cStyle.Seal();
                column.CellStyle = cStyle;

                vacancyTable.Columns.Add(column);
                loadedDates.Add(firstDay.AddDays(i).ToString("dd-MM-yyyy"));
            }

            //Accessing the database for accomodations
            using (AccomodationContext context = new AccomodationContext())
            {
                //Goes through all the rooms available in the database
                foreach (var room in context.m_rooms)
                {
                    //create empty lists that will be filled to pass as an item into the table.
                    List<string> firstName = new List<string>();
                    List<Brush>? vacancyColours = new List<Brush>();
                    List<int> bookingIDS = new List<int>();

                    //create brush colours
                    Brush white = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    Brush bookedColour = new SolidColorBrush(Color.FromRgb(81, 105, 252)); // default booked Colour


                    //create a dictionary with the key as the booking ID and the value as a list of dates that the booking occupies
                    Dictionary<int, IEnumerable<string>> bookedDates = new Dictionary<int, IEnumerable<string>>();

                    //Loop through all bookings that has booked the current room (from the foreach loop)
                    foreach (bookingInfo booking in context.m_bookings.Where(booking => booking.RoomId == room.id))
                    {
                        //variable showing the check in and check out date as a DateTime Object of the current booking
                        DateTime m_checkInDate = DatabaseDateTimeStringToDateTime(booking.CheckInDate);
                        DateTime m_checkOutDate = DatabaseDateTimeStringToDateTime(booking.CheckOutDate);

                        //the dates that the current booking ocupies in the form string
                        List<string> stayDates = new List<string>();

                        //loops through from check in date to check out date adding all days of stay into the list above
                        for (int dateOffset = 0; dateOffset < (m_checkOutDate - m_checkInDate).Days; dateOffset++)
                        {
                            stayDates.Add(m_checkInDate.AddDays(dateOffset).ToString("dd-MM-yyyy"));
                        }
                        //create a new list which has the intersection of the current booking dates and the dates loaded in the table (the month dates)
                        var intersectionDates = loadedDates.Intersect(stayDates);
                        //if there is any dates that intersect then add the booking with its dates into the dictionary
                        if (intersectionDates.Count() != 0)
                        {
                            bookedDates.Add(booking.id, loadedDates.Intersect(stayDates));
                        }
                    }
                    //creates a read only list required to sort booked dates
                    IOrderedEnumerable<KeyValuePair<int, IEnumerable<string>>> bookedDatesSorted;

                    //if the dictionary is not empty then sort it otherwise the sorted dictionary is null
                    if (bookedDates.Count() != 0)
                    {
                        bookedDatesSorted = from entry in bookedDates orderby int.Parse(entry.Value.ElementAt(0).Split("-")[2] + entry.Value.ElementAt(0).Split("-")[1] + entry.Value.ElementAt(0).Split("-")[0]) ascending select entry; //sorts the bookedDates of the month by the check in date
                    }
                    else
                    {
                        bookedDatesSorted = null;
                    }

                    int bookDatesOffsetCounter = 0; // an offset to loop thorugh all the values in the dictionary
                    //loop through all the dates of the month
                    for (int dayOffset = 0; dayOffset < (lastDay - firstDay).Days; dayOffset++)
                    {
                        if (bookedDatesSorted != null && bookDatesOffsetCounter <= bookedDatesSorted.Count() - 1) // if the sorted dictionary is empty or reached the end of the dictionary (from the counter) then proceed to fill the rest of the cells with default colour and no text
                        {
                                if (firstDay.AddDays(dayOffset).Date == DatabaseDateTimeStringToDateTime(bookedDatesSorted.ElementAt(bookDatesOffsetCounter).Value.ElementAt(0)).Date) // if the current date from the loop is the same as the check in date in the dictionary then proceed to fill the cells another colour and add text in the first cell
                                {
                                    //loops through all the dates the booking occupies
                                    foreach (var bookedDate in bookedDatesSorted.ElementAt(bookDatesOffsetCounter).Value)
                                    {
                                    //if it's on the first date then add a cell text showing the first name of the booking else fill with nothing
                                    if (bookedDate == bookedDatesSorted.ElementAt(bookDatesOffsetCounter).Value.ElementAt(0))
                                    {
                                        //adds the first name of the booking if present otherwise show no name
                                        firstName.Add((context.m_bookings.Find(bookedDatesSorted.ElementAt(bookDatesOffsetCounter).Key).FirstName == "") ? "No Name": context.m_bookings.Find(bookedDatesSorted.ElementAt(bookDatesOffsetCounter).Key).FirstName);
                                    }
                                    else {
                                        firstName.Add("");
                                    }
                                        // fill the cell with the booked colour and add the bookingID to that cell
                                        vacancyColours.Add(bookedColour);
                                        bookingIDS.Add(bookedDatesSorted.ElementAt(bookDatesOffsetCounter).Key);
                                    }
                                    // Adds an offset to iterate through all the bookings and offsets the loop counter as the foreach loop above adds colours and text to the dates already
                                    dayOffset += bookedDatesSorted.ElementAt(bookDatesOffsetCounter).Value.Count() - 1;
                                    bookDatesOffsetCounter++;

                                    if (randomCellColours)
                                    {
                                        //Random Booking Colour
                                        Random random = new Random();
                                        List<Brush> randomBrushes = new List<Brush>() { new SolidColorBrush(Color.FromRgb(206, 47, 251)), new SolidColorBrush(Color.FromRgb(103, 70, 235)), new SolidColorBrush(Color.FromRgb(75, 139, 250)), new SolidColorBrush(Color.FromRgb(57, 188, 227)), new SolidColorBrush(Color.FromRgb(65, 144, 129)) };
                                        bookedColour = randomBrushes[(int)(random.NextDouble() * 5)];
                                    }
                                }
                                else
                                {
                                    
                                    //add nothing
                                    firstName.Add("");
                                    vacancyColours.Add(white);
                                    bookingIDS.Add(-1);
                                }

                        }
                        else
                        {
                            //add nothing
                            firstName.Add("");
                            vacancyColours.Add(white);
                            bookingIDS.Add(-1);
                        }
                    }
                    // adds the item into the table, (item are all the info for one row)
                    vacancyTable.Items.Add(new vacancyData() { roomNumber = room.id, cellText = firstName, vacancyColour = vacancyColours, bookingsIDs = bookingIDS });
                }
            }
            //Trace.WriteLine("Load Time ms : " + (DateTime.Now - TimerStart).Milliseconds.ToString());
        }


        private void nextMonth_B_Click(object sender, RoutedEventArgs e)
        {
            //load next month
            loadedMonth = loadedMonth.AddMonths(1);
            updateCurrentMonthText();
            GenerateTable();
        }

        private void previousMonth_B_Click(object sender, RoutedEventArgs e)
        {
            //load previous month
            loadedMonth = loadedMonth.AddMonths(-1);
            updateCurrentMonthText();
            GenerateTable();

        }

        private void AddBooking_B_Click(object sender, RoutedEventArgs e)
        {
            if (!validateCell()) return; // checks if a valid cell is selected

            //variables to find the selected element in the items for the selected date
            DataGridColumn selectedColumn = vacancyTable.SelectedCells.FirstOrDefault().Column;
            string year = (loadedMonth.Month == 12 && vacancyTable.SelectedCells.FirstOrDefault().Column.Header.ToString().Split("-")[1] == "01") ? loadedMonth.AddYears(1).ToString("-yyyy") : loadedMonth.ToString("-yyyy");
            DateTime selectedDate = DatabaseDateTimeStringToDateTime(selectedColumn.Header.ToString() + year);
            int selectedElement = (selectedDate - loadedMonth).Days;

            //a variable showing the current item that includes the selected cell
            vacancyData currentItem = (vacancyData)vacancyTable.SelectedCells.FirstOrDefault().Item; 

            //checks if the selected cell already has a booking ID atteched to it, if there is then display an error in adding a booking, (-1 shows there is no booking ID's)
            if (currentItem.bookingsIDs[selectedElement] == -1)
            {
                string selectedDate_S = vacancyTable.SelectedCells.FirstOrDefault().Column.Header + "-" + loadedMonth.Year.ToString();
                AddBookingWindow bookingWindow = new AddBookingWindow(selectedDate_S, currentItem.roomNumber);
                bookingWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("There is already a booking on the selected cell", "Error Adding Booking");
            }
        }


        private bool validateCell()
        {
            //chcecks if there is an cell is selected and if the cell is not in the first column (the first column shows all the room numbers and not the bookings)
            if (vacancyTable.SelectedCells.FirstOrDefault().Column == null)
            {
                MessageBox.Show("Please select a cell", "Error");
                return false;
            }
            else if (vacancyTable.SelectedCells.FirstOrDefault().Column.Header.ToString() == "Room")
            {
                MessageBox.Show("Please select a valid cell", "Error");
                return false;
            }
            return true;
        }

        private void DeleteBooking_B_Click(object sender, RoutedEventArgs e)
        {
            //check if a valid cell is selected
            if (!validateCell()) return;


            //variables to find the selected element in the items for the selected date
            DataGridColumn selectedColumn = vacancyTable.SelectedCells.FirstOrDefault().Column;
            string year = (loadedMonth.Month == 12 && vacancyTable.SelectedCells.FirstOrDefault().Column.Header.ToString().Split("-")[1] == "01") ? loadedMonth.AddYears(1).ToString("-yyyy") : loadedMonth.ToString("-yyyy");
            DateTime selectedDate = DatabaseDateTimeStringToDateTime(selectedColumn.Header.ToString() + year);
            int selectedElement = (selectedDate - loadedMonth).Days;

            //variable showing the current item selected and the booking ID attached to the cell
            vacancyData currentItem = (vacancyData)vacancyTable.SelectedCells.FirstOrDefault().Item;
            int selectedBookingID = currentItem.bookingsIDs[selectedElement];

            //if there is no booking then skip deletion and display an error
            if (selectedBookingID != -1)
            {
                //accessing the database
                using (AccomodationContext context = new AccomodationContext())
                {
                    //create a prompt to make sure the admin want's to delete the booking, if they are sure then delete the booking
                    if (MessageBox.Show("Are you sure you want to delete " + context.m_bookings.Find(selectedBookingID).FirstName + " " + context.m_bookings.Find(selectedBookingID).Surname + "'s Booking?", "Booking Deletion", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        context.m_bookings.Remove(context.m_bookings.Find(selectedBookingID));
                        context.SaveChanges();
                        GenerateTable();
                    }

                }
            }
            else
            {
                MessageBox.Show("No bookings selected", "Error");
            }



        }

        private void Logout_B_Click(object sender, RoutedEventArgs e)
        {
            //Logout of main screen and open login window up
            loginScreen loginWindow = new loginScreen();
            loginWindow.Show();
            this.Close();

        }

        private void CurrentMonth_B_Click(object sender, RoutedEventArgs e)
        {
            //Load the current month
            LoadCurrentMonth();
            updateCurrentMonthText();
            GenerateTable();
        }

        //called when a new cell is selected, this is used to update all the information in the details panel
        private void vacancyTable_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            //if there is not a valid cell selected then do nothing
            if (vacancyTable.SelectedCells.Count == 0) return;
            if (vacancyTable.SelectedCells.FirstOrDefault().Column.Header.ToString() == "Room") return;


            //variables to find the selected element in the items for the selected date
            DataGridColumn selectedColumn = vacancyTable.SelectedCells.FirstOrDefault().Column;
            vacancyData selectedItem = (vacancyData)vacancyTable.SelectedCells.FirstOrDefault().Item;
            string year = (loadedMonth.Month == 12 && vacancyTable.SelectedCells.FirstOrDefault().Column.Header.ToString().Split("-")[1] == "01") ? loadedMonth.AddYears(1).ToString("-yyyy") : loadedMonth.ToString("-yyyy");
            DateTime selectedDate = DatabaseDateTimeStringToDateTime(selectedColumn.Header.ToString() + year);
            int selectedElement = (selectedDate - loadedMonth).Days;

            using (AccomodationContext context = new AccomodationContext()) {
                bool vacant = (selectedItem.bookingsIDs.ElementAt(selectedElement) == -1) ? true : false;

                //if the room is vacant then display the add button else display the edit and delete button
                if (vacant)
                {
                    AddBooking_B.Visibility = Visibility.Visible;
                    EditBooking_B.Visibility = Visibility.Collapsed;
                    DeleteBooking_B.Visibility = Visibility.Collapsed;
                } else
                {
                    AddBooking_B.Visibility = Visibility.Collapsed;
                    EditBooking_B.Visibility = Visibility.Visible;
                    DeleteBooking_B.Visibility = Visibility.Visible;
                }

                //below grabs information from the database on the selected cell and sets the details.
                Vacant_T.Text = (selectedItem.bookingsIDs.ElementAt(selectedElement) == -1) ? "Vacant: True": "Vacant: False";
                RoomNumber_T.Text = "Room Number: " + selectedItem.roomNumber.ToString();
                RoomType_T.Text = "Room Type: " + context.m_rooms.Find(selectedItem.roomNumber).RoomType;

                int bookingID = selectedItem.bookingsIDs.ElementAt(selectedElement);
                BookingID_T.Text = vacant ? "Booking ID: NA" : "Booking ID: " + (selectedItem.bookingsIDs.ElementAt(selectedElement)).ToString();
                if (!vacant)
                {
                    FullName_T.Text = (context.m_bookings.Find(bookingID).FirstName == "") ? "Full Name: NA" : "Full Name: " + (context.m_bookings.Find(bookingID).FirstName + " " + context.m_bookings.Find(bookingID).Surname);
                    PhoneNumber_T.Text = (context.m_bookings.Find(bookingID).PhoneNumber == "") ? "Phone Number: NA" : "Phone Number: " + context.m_bookings.Find(bookingID).PhoneNumber;
                    ArrivalTime_T.Text = (context.m_bookings.Find(bookingID).ArrivalTime == "") ? "Arrival Time: NA" : "Arrival Time: " + context.m_bookings.Find(bookingID).ArrivalTime;
                    ExtraDetails_T.Text = (context.m_bookings.Find(bookingID).ExtraDetails == "") ? "Extra Details: NA" : "Extra Details: " + context.m_bookings.Find(bookingID).ExtraDetails;
                    CheckInDate_T.Text = "Check-in Date: " + context.m_bookings.Find(bookingID).CheckInDate;
                    CheckOutDate_T.Text = "Check-out Date: " + context.m_bookings.Find(bookingID).CheckOutDate;
                    int nights = (DatabaseDateTimeStringToDateTime(context.m_bookings.Find(bookingID).CheckOutDate) - DatabaseDateTimeStringToDateTime(context.m_bookings.Find(bookingID).CheckInDate)).Days;
                    Nights_T.Text = "Nights: " + nights.ToString();
                    float dailyRate = context.m_bookings.Find(bookingID).DailyRate;
                    DailyRate_T.Text = "Daily Rate: $" + dailyRate.ToString();
                    TotalPrice_T.Text = "Total Price: $" + Math.Round(dailyRate*nights, 2).ToString();
                    GST_T.Text = "GST: $" + Math.Round(dailyRate*nights*0.1, 2).ToString();
                    TotalNoGST_T.Text = "Total Without GST: $" + Math.Round((dailyRate * nights) - (dailyRate * nights * 0.1), 2).ToString();

                }
                else
                {
                    FullName_T.Text = "Full Name: NA";
                    PhoneNumber_T.Text = "Phone Number: NA";
                    ArrivalTime_T.Text = "Arrival Time: NA";
                    CheckInDate_T.Text = "Check-in Date: NA";
                    CheckOutDate_T.Text = "Check-out Date: NA";
                    Nights_T.Text = "Nights: NA";
                    DailyRate_T.Text = "Daily Rate: NA";
                    TotalPrice_T.Text = "Total Price: NA";
                    GST_T.Text = "GST: NA";
                    TotalNoGST_T.Text = "Total Without GST: NA";
                    
                }

            }

        }


        private void SearchMonth_B_Click(object sender, RoutedEventArgs e)
        {
            //when search month button is clicked, toggle the visibility of the calendar
            if (searchMonth_C.IsVisible) { 
                searchMonth_C.Visibility = Visibility.Collapsed;
            } else
            {
                searchMonth_C.DisplayMode = CalendarMode.Year;
                searchMonth_C.Visibility= Visibility.Visible;
            }
        }

        private void searchMonth_C_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            //if a date is selected from the calendar then load that month
            if (searchMonth_C.SelectedDate == null) return;

            loadedMonth = (DateTime) searchMonth_C.SelectedDate;
            searchMonth_C.Visibility = Visibility.Collapsed;
            updateCurrentMonthText();
            GenerateTable();
        }


        //updates the title showing the current month loaded in the table
        private void updateCurrentMonthText() {
            
            //if the next month is not in the next year then display in the form of yyyy MMMM - MMMM else yyyy MMMM - yyyy MMMM
            if (loadedMonth.Year != loadedMonth.AddDays(datesGenerated).Year)
            {
                CurrentMonth.Text = loadedMonth.ToString("yyyy MMMM") + " To " + loadedMonth.AddDays(datesGenerated).ToString("yyyy MMMM");
            } else
            {
                CurrentMonth.Text = loadedMonth.ToString("yyyy MMMM") + " - " + loadedMonth.AddDays(datesGenerated).ToString("MMMM");
            }
        }

        private void EditBooking_B_Click(object sender, RoutedEventArgs e)
        {
            if (!validateCell()) return; 

            //variables to find the selected element in the items for the selected date
            DataGridColumn selectedColumn = vacancyTable.SelectedCells.FirstOrDefault().Column;
            string year = (loadedMonth.Month == 12 && vacancyTable.SelectedCells.FirstOrDefault().Column.Header.ToString().Split("-")[1] == "01") ? loadedMonth.AddYears(1).ToString("-yyyy") : loadedMonth.ToString("-yyyy");
            DateTime selectedDate = DatabaseDateTimeStringToDateTime(selectedColumn.Header.ToString() + year);
            int selectedElement = (selectedDate - loadedMonth).Days;

            if (((vacancyData)vacancyTable.SelectedCells.FirstOrDefault().Item).bookingsIDs[selectedElement] == -1) return; //if there is no booking on the date then exit function

            //using the database to find the checkin date of the selected booking
            using (AccomodationContext context = new AccomodationContext())
            {
                int bookingId = ((vacancyData)vacancyTable.SelectedCells.FirstOrDefault().Item).bookingsIDs[selectedElement];

                DateTime checkInDate = DatabaseDateTimeStringToDateTime(context.m_bookings.Find(bookingId).CheckInDate);

                //create a new Addbookingwindow but telling it that it's in edit mode and passing in the right values
                AddBookingWindow editRoomWindow = new AddBookingWindow(checkInDate.ToString("dd-MM-yyyy"), ((vacancyData)vacancyTable.SelectedCells.FirstOrDefault().Item).roomNumber, true, bookingId);
                editRoomWindow.ShowDialog();
            }


        }
        //creates a global dispatcher timer
        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        //starts the daily timer which will call new date when a new date arrives
        private void startDailyTimer() {
            dispatcherTimer.Interval = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(1).Day, 0, 0, 0) - DateTime.Now;
            //dispatcherTimer.Interval = TimeSpan.FromSeconds(5);
            dispatcherTimer.Tick -= newDate;
            dispatcherTimer.Tick += newDate;
            dispatcherTimer.Start();
        }
        //function that is called when a new date arrives
        private void newDate(object sender, EventArgs e) { 
            dispatcherTimer.Stop(); //temporary stop the dispatch timer
            GenerateTable(); // regenerate the table
            backupDatabase(); //make a backup of today's database
            startDailyTimer(); // restart the timer
        }
        //creates a backup of the current database into the backup folder through copying
        private void backupDatabase()
        {
            try {
                //makes sure the folder exists
                if (!Directory.Exists(Environment.CurrentDirectory + "\\BackUps"))
                {
                    System.IO.Directory.CreateDirectory(Environment.CurrentDirectory + "\\BackUps");
                }

                //variable storing the path to the current database and the backup database
                string backupPath = Environment.CurrentDirectory + "\\BackUps\\accommodation" + DateTime.Now.ToString("_dd-MM-yyyy") + ".db";
                string dataBasePath = Environment.CurrentDirectory + "\\accommodation.db";

                //copy the current database to the backupdatabase file
                File.Copy(dataBasePath, backupPath);

            }
            catch {
                Trace.WriteLine("Backup Saving Error");
            }
        }

        //open settings window
        private void Settings_B_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }
        //load support card PDF
        private void Support_B_Click(object sender, RoutedEventArgs e)
        {
            string filename = Environment.CurrentDirectory + "\\SoftwareSupportCard.pdf";
            System.Diagnostics.Process.Start("explorer", filename);
        }
    }
}
