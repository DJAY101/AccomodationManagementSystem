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

namespace AccomodationManagementSystem
{
    /// <summary>
    /// Interaction logic for InputPrompt.xaml
    /// </summary>
    public partial class InputPrompt : Window
    {

        public string input;
        bool inputHidden;
        public InputPrompt(string title, string label, bool HideInput = false)
        {
            InitializeComponent();

            Title_T.Text = title;
            Label_T.Text = label;
            inputHidden = HideInput;
            if(!HideInput)
            {
                inputBox_P.Visibility = Visibility.Collapsed;
                inputBox_TB.Visibility = Visibility.Visible;
            }

        }

        private void Accept_B_Click(object sender, RoutedEventArgs e)
        {
            if (inputHidden)
            {
                input = inputBox_P.Password;
            } else
            {
                input = inputBox_TB.Text;
            }
            this.Close();
        }

        private void Cancel_B_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
