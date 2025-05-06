using System;
using System.Collections.Generic;
using System.Data;
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
using static System.Net.Mime.MediaTypeNames;

namespace DPGI_vladik
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AdoAssistant dbHelper = new AdoAssistant();

        public MainWindow()
        {
            InitializeComponent();
            BooksGrid.ItemsSource = dbHelper.TableLoad().DefaultView;
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            BooksGrid.ItemsSource = dbHelper.TableLoad().DefaultView;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            dbHelper.SaveChanges();
        }

        private void btnDelete_click(object sender, RoutedEventArgs e)
        {
            if (BooksGrid.SelectedItem is DataRowView row)
            {
                row.Delete();
            }
            else MessageBox.Show("Select row(s) for this operation");
        }

        private void Paste(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                string clipboardData = Clipboard.GetText();
                if (string.IsNullOrWhiteSpace(clipboardData))
                    return;

                string[] values = clipboardData.Split('\t');

                var item = BooksGrid.CurrentItem;

                if (item is DataRowView row && !row.IsNew)
                {
                    dbHelper.Paste(values, row);
                }
                else
                {
                    dbHelper.Paste(values);                    
                }
            }
        }
    }
}
