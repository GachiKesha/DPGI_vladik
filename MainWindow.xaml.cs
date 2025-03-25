using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using Microsoft.Win32;

namespace DPGI_vladik
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            string input = InputTextBox.Text;
            int fromBase = int.Parse(((ComboBoxItem)FromBaseComboBox.SelectedItem).Tag.ToString());
            int toBase = int.Parse(((ComboBoxItem)ToBaseComboBox.SelectedItem).Tag.ToString());

            try
            {
                long decimalValue = Convert.ToInt64(input, fromBase);
                string result = Convert.ToString(decimalValue, toBase).ToUpper();
                ResultTextBox.Text = result;
            }            
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Number is too big.");
            }
            catch (OverflowException)
            {
                MessageBox.Show("Number is too big.");
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid input. Please enter a valid number.");
            }
        }
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            InputTextBox.Clear();
            ResultTextBox.Clear();
            FromBaseComboBox.SelectedIndex = 2; // Default to Decimal
            ToBaseComboBox.SelectedIndex = 3;   // Default to Hex
        }
        private void SwapButton_Click(object sender, RoutedEventArgs e)
        {
            InputTextBox.Text = ResultTextBox.Text;
            ResultTextBox.Clear();
            var temp = FromBaseComboBox.SelectedIndex;
            FromBaseComboBox.SelectedIndex = ToBaseComboBox.SelectedIndex;
            ToBaseComboBox.SelectedIndex = temp;
            ConvertButton_Click(sender, e);
        }
        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(ResultTextBox.Text);
        }        
    }
}
