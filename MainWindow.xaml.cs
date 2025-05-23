using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;

namespace DPGI_vladik
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Entities context;
        List<Numbers> numbers;
        public MainWindow()
        {
            InitializeComponent();
            context = new Entities();
            numbers = new List<Numbers>();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (Numbers number in context.Numbers)
            {
                numbers.Add(number);
            }
            ConvertedNotes.ItemsSource = numbers;
        }
        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            string input = InputTextBox.Text;
            int fromBase = int.Parse(((ComboBoxItem)FromBaseComboBox.SelectedItem).Tag.ToString());
            int toBase = int.Parse(((ComboBoxItem)ToBaseComboBox.SelectedItem).Tag.ToString());

            try
            {
                long decimalValue = Convert.ToInt64(input, fromBase);
                var existing = numbers.FirstOrDefault(n => n.Decimal == decimalValue);

                string result;

                if (existing == null)
                {
                    string binary = Convert.ToString(decimalValue, 2);
                    string octal = Convert.ToString(decimalValue, 8);
                    string hex = Convert.ToString(decimalValue, 16).ToUpper();

                    var newEntry = new Numbers
                    {
                        Decimal = decimalValue,
                        Binary = binary,
                        Octal = octal,
                        Hexidecimal = hex
                    };

                    context.Numbers.Add(newEntry);
                    context.SaveChanges();

                    numbers.Add(newEntry);
                    ConvertedNotes.Items.Refresh();
                    existing = newEntry;
                }
                switch (toBase)
                {
                    case 2:
                        result = existing.Binary;
                        break;
                    case 8:
                        result = existing.Octal;
                        break;
                    case 10:
                        result = existing.Decimal.ToString();
                        break;
                    case 16:
                        result = existing.Hexidecimal;
                        break;
                    default:
                        throw new NotSupportedException("Unsupported base");
                }

                ResultTextBox.Text = result;
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Value is too big.");
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
            context.Numbers.RemoveRange(context.Numbers);
            context.SaveChanges();
            numbers.Clear();
            ConvertedNotes.Items.Refresh();
        }
        private void SwapButton_Click(object sender, RoutedEventArgs e)
        {
            InputTextBox.Text = ResultTextBox.Text;
            ResultTextBox.Clear();
            (ToBaseComboBox.SelectedIndex, FromBaseComboBox.SelectedIndex) = (FromBaseComboBox.SelectedIndex, ToBaseComboBox.SelectedIndex);
            ConvertButton_Click(sender, e);
        }
        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(ResultTextBox.Text);
        }        
    }
}
