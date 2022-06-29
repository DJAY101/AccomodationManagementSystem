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


        public class roomNumber {

            public int room { get; set; }
            public string? book { get; set; }
            public List<string>? bookings { get; set; }
            public List<Brush>? colour { get; set; }

        }




        private void SetupTable() {
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

        private void LoadCurrentMonth() {
            //selects the current month to show
            loadedMonth = DateTime.Now;
            CurrentMonth.Text = loadedMonth.ToString("MMMM - yyyy");

        }

        //private void GenerateTable() { 
        //    //clears the table
        //    vacancyTable.Columns.Clear();
        //    vacancyTable.Items.Clear();

        //    //displays the first column where the room numbers are shown
        //    DataGridTextColumn c1 = new DataGridTextColumn();
        //    c1.Header = "Room";
        //    c1.Binding = new Binding("room");

        //    var styleColumn = new Style(typeof(TextBlock));
        //    styleColumn.Setters.Add(new Setter { Property = TextBlock.FontSizeProperty, Value = 30.0 });
        //    styleColumn.Seal();

        //    c1.ElementStyle = styleColumn;
        //    vacancyTable.Columns.Add(c1);

        //    // adds the room numbers to the first column
        //    for (int i = 1; i < 10; i++)
        //    {
        //        Dictionary<string, string> yee = new Dictionary<string, string>();
        //        yee.Add("book1", "hello");
        //        yee.Add("book2", "hello2");
        //        roomNumber temp = new roomNumber() { room = i, book="hi"};
        //        vacancyTable.Items.Add(temp);
        //    }


        //    // adds the dates of the month onto the columns
        //    DateTime firstDay = new DateTime(loadedMonth.Year, loadedMonth.Month, 1);
        //    DateTime lastDay = new DateTime(loadedMonth.Year, loadedMonth.Month, firstDay.AddMonths(1).AddDays(-1).Day);
        //    for (int i = 0; i < lastDay.Day; i++) {
        //        DataGridTextColumn column = new DataGridTextColumn();
        //        column.Header = firstDay.AddDays(i).ToString("dd-MM");
        //        if (i == 1)
        //        {
        //            column.Binding = new Binding("book");
        //        }
        //        vacancyTable.Columns.Add(column);
        //    }


        //}

        public class vacancyData {

            public int roomNumber { get; set; }
            public List<Brush>? vacancyColour { get; set; }
            public int bookingID { get; set; }
            public string? firstName { get; set; }
            public List<string>? vacancy { get; set; }



        }
        private void GenerateTable() {
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
            
            //add the 10 rooms into the datagrid
            for (int i = 1; i < 11; i++) {
                List<string> accomodations = new List<string>();
                List<Brush>? vacancyColours = new List<Brush>();
                Brush blue = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                Brush bookedColour = new SolidColorBrush(Color.FromRgb(81, 105, 252));
                //temporary generate random vacancy
                for (int c = 0; c < 30; c++)
                {
                    if (rand.NextDouble() < 0.5)
                    {
                        accomodations.Add("yes");
                        vacancyColours.Add(blue);
                        
                    }
                    else { 
                        accomodations.Add("no"); 
                        vacancyColours.Add(bookedColour);
                    }
                }

                vacancyTable.Items.Add(new vacancyData() { roomNumber = i, vacancy = accomodations, vacancyColour = vacancyColours});

            }
            //generates all the dates of the month
            DateTime firstDay = new DateTime(loadedMonth.Year, loadedMonth.Month, 1);
            DateTime lastDay = new DateTime(loadedMonth.Year, loadedMonth.Month, firstDay.AddMonths(1).AddDays(-1).Day);
            for (int i = 0; i < lastDay.Day; i++)
            {
                //create a new column and give it a header of its date
                DataGridTextColumn column = new DataGridTextColumn();
                column.Header = firstDay.AddDays(i).ToString("dd-MM");
                //binds the vacancy text to the columns cells
                column.Binding = new Binding("vacancy["+i+"]");

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

                //trigger when a cell is selected change its cell colour
                Trigger selectedTrig = new Trigger() { Property = DataGridCell.IsSelectedProperty, Value = true };
                selectedTrig.Setters.Add(new Setter() { Property = BackgroundProperty, Value = selectedColour });
                cStyle.Triggers.Add(selectedTrig);
                // apply the style to the column
                cStyle.Seal();
                column.CellStyle = cStyle;

                vacancyTable.Columns.Add(column);

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

            if (vacancyTable.SelectedCells.FirstOrDefault().Column == null)
            {
                MessageBox.Show("Please select a cell", "Error");
            }
            else if(vacancyTable.SelectedCells.FirstOrDefault().Column.Header.ToString() == "Room") {
                MessageBox.Show("Please select a valid cell", "Error");
            }
            else
            {
                vacancyData currentItem = (vacancyData)vacancyTable.SelectedCells.FirstOrDefault().Item;
                string selectedDate = vacancyTable.SelectedCells.FirstOrDefault().Column.Header + "-" + loadedMonth.Year.ToString();
                AddBookingWindow bookingWindow = new AddBookingWindow(selectedDate, currentItem.roomNumber);
                bookingWindow.ShowDialog();
            }
        }
    }
}
