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
using System.Text.RegularExpressions;

namespace AccomodationManagementSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    public partial class MainWindow : Window
    {
        DateTime loadedMonth = new DateTime(1, 1, 1);


        public MainWindow()
        {
            InitializeComponent();
            SetupTable();
            Brush blue = new SolidColorBrush(Color.FromRgb(0, 0, 255));
            Brush red = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            List<Brush> colours = new List<Brush>() { red, blue };


            List<string> nother = new List<string>();
            nother.Add("nope");
            nother.Add("sure");
            roomNumber cool = new roomNumber() { room = 3, book = "hiii", bookings = nother, colour = colours };
            roomNumber cool2 = new roomNumber() { room = 4, book = "hii2i", bookings = nother, colour = colours };
            List<roomNumber> temp = new List<roomNumber>();
            temp.Add(cool);
            temp.Add(cool2);

            //vacancyTable.ItemsSource = temp;


            LoadCurrentMonth();
            GenerateTable();
        }


        public class roomNumber
        {

            public int room { get; set; }
            public string? book { get; set; }
            public List<string>? bookings { get; set; }
            public List<Brush>? colour { get; set; }

        }




        private void SetupTable()
        {
            //setup for the vacancy table
            vacancyTable.IsReadOnly = true;
            vacancyTable.CanUserResizeColumns = false;
            vacancyTable.CanUserResizeRows = false;
            vacancyTable.CanUserSortColumns = false;
            vacancyTable.SelectionUnit = DataGridSelectionUnit.Cell;
            vacancyTable.SelectionMode = DataGridSelectionMode.Single;

            var style = new Style(typeof(System.Windows.Controls.Primitives.DataGridColumnHeader));
            style.Setters.Add(new Setter { Property = System.Windows.Controls.Primitives.DataGridColumnHeader.FontSizeProperty, Value = 25.0 });
            style.Seal();

            vacancyTable.ColumnHeaderStyle = style;

        }

        private void LoadCurrentMonth()
        {
            //selects the current month to show
            loadedMonth = DateTime.Now;
            CurrentMonth.Text = loadedMonth.ToString("MMMM - yyyy");

        }



        public class vacancyData
        {

            public int roomNumber { get; set; }
            public List<Brush>? vacancyColour { get; set; }
            public List<int>? bookingsIDs { get; set; }
            public string? firstName { get; set; }
            public List<string>? vacancy { get; set; }

        }

        private DateTime DatabaseDateTimeStringToDateTime(string date)
        {
            int year = int.Parse(date.Split("-")[2]);
            int month = int.Parse(date.Split("-")[1]);
            int day = int.Parse(date.Split("-")[0]);
            return new DateTime(year, month, day);
        }
        public void GenerateTable()
        {
            vacancyTable.Columns.Clear();
            vacancyTable.Items.Clear();

            DataGridTextColumn firstColumn = new DataGridTextColumn();

            var rand = new Random();


            firstColumn.Binding = new Binding("roomNumber");
            firstColumn.Header = "Room";

            var styleColumn = new Style(typeof(TextBlock));
            styleColumn.Setters.Add(new Setter { Property = TextBlock.FontSizeProperty, Value = 30.0 });
            styleColumn.Seal();

            firstColumn.ElementStyle = styleColumn;

            vacancyTable.Columns.Add(firstColumn);

            //The first and last day of the current loaded month
            DateTime firstDay = new DateTime(loadedMonth.Year, loadedMonth.Month, 1);
            DateTime lastDay = new DateTime(loadedMonth.Year, loadedMonth.Month, firstDay.AddMonths(1).AddDays(-1).Day);
            List<string> loadedDates = new List<string>();

            //generates all the dates of the month
            for (int i = 0; i < lastDay.Day; i++)
            {
                //create a new column and give it a header of its date
                DataGridTextColumn column = new DataGridTextColumn();
                column.Header = firstDay.AddDays(i).ToString("dd-MM");
                //binds the vacancy text to the columns cells
                column.Binding = new Binding("vacancy[" + i + "]");

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


                Brush selectedColour = new SolidColorBrush(Color.FromRgb(0, 0, 0));

                //colour the cell depending on the vacancy status
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


            using (AccomodationContext context = new AccomodationContext())
            {
                //Load all rooms from database
                foreach (var room in context.m_rooms)
                {
                    List<string> accomodations = new List<string>();
                    List<Brush>? vacancyColours = new List<Brush>();
                    List<int> bookingIDS = new List<int>();
                    Brush white = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    Brush bookedColour = new SolidColorBrush(Color.FromRgb(81, 105, 252));


                    //Regex regex = new Regex("-");
                    //var thing = context.m_bookings.Where(booking => booking.RoomId == room.id).Where(booking => new DateTime(int.Parse(regex.Split(booking.CheckInDate)[2]), int.Parse(regex.Split(booking.CheckInDate)[1]), int.Parse(regex.Split(booking.CheckInDate)[0])).ToString("MM-yyyy") == loadedMonth.ToString("MM-yyyy"));
                    Dictionary<int, IEnumerable<string>> bookedDates = new Dictionary<int, IEnumerable<string>>();
                    //Loop through all bookings that has the current room
                    foreach (bookingInfo booking in context.m_bookings.Where(booking => booking.RoomId == room.id))
                    {
                        DateTime m_checkInDate = DatabaseDateTimeStringToDateTime(booking.CheckInDate);
                        DateTime m_checkOutDate = DatabaseDateTimeStringToDateTime(booking.CheckOutDate);
                        List<string> stayDates = new List<string>();
                        //loops through from check in date to check out date adding all days of stay into the list above
                        for (int dateOffset = 0; dateOffset < (m_checkOutDate - m_checkInDate).Days; dateOffset++)
                        {
                            stayDates.Add(m_checkInDate.AddDays(dateOffset).ToString("dd-MM-yyyy"));
                        }
                        var intersectionDates = loadedDates.Intersect(stayDates);
                        if (intersectionDates.Count() != 0)
                        {
                            bookedDates.Add(booking.id, loadedDates.Intersect(stayDates));
                        }
                    }

                    IOrderedEnumerable<KeyValuePair<int, IEnumerable<string>>> bookedDatesSorted;
                    if (bookedDates.Count() != 0)
                    {
                        bookedDatesSorted = from entry in bookedDates orderby int.Parse(entry.Value.ElementAt(0).Split("-")[0]) ascending select entry; //sorts the bookedDates of the month by the check in date
                    }
                    else
                    {
                        bookedDatesSorted = null;
                    }

                    int bookDatesOffsetCounter = 0;
                    for (int dayOffset = 0; dayOffset < lastDay.Day; dayOffset++)
                    {
                        if (bookedDatesSorted != null)
                        {
                            if (bookDatesOffsetCounter <= bookedDatesSorted.Count() - 1)
                            {
                                if (firstDay.AddDays(dayOffset).Date == DatabaseDateTimeStringToDateTime(bookedDatesSorted.ElementAt(bookDatesOffsetCounter).Value.ElementAt(0)).Date)
                                {
                                    accomodations.Add(context.m_bookings.Find(bookedDatesSorted.ElementAt(bookDatesOffsetCounter).Key).FirstName);
                                    for (int i = 0; i < bookedDatesSorted.ElementAt(bookDatesOffsetCounter).Value.Count() - 1; i++) { accomodations.Add(""); }
                                    foreach (var bookedDate in bookedDatesSorted.ElementAt(bookDatesOffsetCounter).Value)
                                    {
                                        vacancyColours.Add(bookedColour);
                                        bookingIDS.Add(bookedDatesSorted.ElementAt(bookDatesOffsetCounter).Key);
                                    }
                                    dayOffset += bookedDatesSorted.ElementAt(bookDatesOffsetCounter).Value.Count() - 1;
                                    bookDatesOffsetCounter++;

                                }
                                else
                                {
                                    accomodations.Add("");
                                    vacancyColours.Add(white);
                                    bookingIDS.Add(-1);
                                }
                            }
                            else
                            {
                                accomodations.Add("");
                                vacancyColours.Add(white);
                                bookingIDS.Add(-1);
                            }
                        }
                        else
                        {
                            accomodations.Add("");
                            vacancyColours.Add(white);
                            bookingIDS.Add(-1);
                        }
                    }
                    vacancyTable.Items.Add(new vacancyData() { roomNumber = room.id, vacancy = accomodations, vacancyColour = vacancyColours, bookingsIDs = bookingIDS });
                }
            }


        }


        private void nextMonth_B_Click(object sender, RoutedEventArgs e)
        {
            //load next month
            loadedMonth = loadedMonth.AddMonths(1);
            CurrentMonth.Text = loadedMonth.ToString("MMMM - yyyy");
            GenerateTable();
        }

        private void previousMonth_B_Click(object sender, RoutedEventArgs e)
        {
            //load previous month
            loadedMonth = loadedMonth.AddMonths(-1);
            CurrentMonth.Text = loadedMonth.ToString("MMMM - yyyy");
            GenerateTable();

        }

        private void AddBooking_B_Click(object sender, RoutedEventArgs e)
        {
            if (validateCell())
            {
                vacancyData currentItem = (vacancyData)vacancyTable.SelectedCells.FirstOrDefault().Item;

                if (currentItem.bookingsIDs[int.Parse(vacancyTable.SelectedCells.FirstOrDefault().Column.Header.ToString().Split("-")[0]) - 1] == -1)
                {
                    string selectedDate = vacancyTable.SelectedCells.FirstOrDefault().Column.Header + "-" + loadedMonth.Year.ToString();
                    AddBookingWindow bookingWindow = new AddBookingWindow(selectedDate, currentItem.roomNumber);
                    bookingWindow.ShowDialog();
                }
                else
                {
                    MessageBox.Show("There is already a booking on the selected cell", "Error Adding Booking");
                }
            }
        }
        private void AddRoom_B_Click(object sender, RoutedEventArgs e)
        {
            AddRoomWindow addRoomWindow = new AddRoomWindow();
            addRoomWindow.ShowDialog();
        }

        private bool validateCell()
        {
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
            if (validateCell())
            {
                vacancyData currentItem = (vacancyData)vacancyTable.SelectedCells.FirstOrDefault().Item;
                int selectedBookingID = currentItem.bookingsIDs[int.Parse(vacancyTable.SelectedCells.FirstOrDefault().Column.Header.ToString().Split("-")[0]) - 1];
                if (selectedBookingID != -1)
                {
                    using (AccomodationContext context = new AccomodationContext())
                    {

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
        }
    }
}
