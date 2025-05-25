using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DPGI_vladik
{
    /// <summary>
    /// Interaction logic for PageMain.xaml
    /// </summary>
    public partial class PageMain : Page
    {
        Entities context;
        List<Numbers> numbers;
        public PageMain()
        {
            InitializeComponent();
            context = new Entities();
            numbers = context.Numbers.ToList();
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
                string InputStr;
                string OutputStr;

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
                    existing = newEntry;
                }
                switch (toBase)
                {
                    case 2:
                        result = existing.Binary;
                        OutputStr = "0b" + result;
                        break;
                    case 8:
                        result = existing.Octal;
                        OutputStr = "0" + result;
                        break;
                    case 10:
                        result = existing.Decimal.ToString();
                        OutputStr = result;
                        break;
                    case 16:
                        result = existing.Hexidecimal;
                        OutputStr = "0x" + result;
                        break;
                    default:
                        throw new NotSupportedException("Unsupported base");
                }

                ResultTextBox.Text = result;

                switch (fromBase)
                {
                    case 2:
                        InputStr = "0b" + input;
                        break;
                    case 8:
                        InputStr = "0" + input;
                        break;
                    case 10:
                        InputStr = input;
                        break;
                    case 16:
                        InputStr = "0x" + input;
                        break;
                    default:
                        throw new NotSupportedException("Unsupported base");
                }                
                App.HistoryCollection.Add(new HistoryEntry 
                {
                    InputValue = InputStr,
                    OutputValue = OutputStr,
                    Timestamp = DateTime.Now
                });
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Value is too big.");
            }
            catch (OverflowException)
            {
                MessageBox.Show("Number is too big.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
