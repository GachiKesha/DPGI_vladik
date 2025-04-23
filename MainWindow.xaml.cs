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
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            BooksGrid.ItemsSource = dbHelper.TableLoad().DefaultView;
        }

        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            if (txtISBN.Text == "" || txtName.Text == "" || txtAuthors.Text == "" || txtPublisher.Text == "")
            {
                MessageBox.Show("Please enter full data");
                return;
            }
            dbHelper.TableInsert(txtISBN.Text, txtName.Text, txtAuthors.Text, txtPublisher.Text, int.Parse(txtYear.Text));
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (txtISBN.Text == "" || txtName.Text == "" || txtAuthors.Text == "" || txtPublisher.Text == "")
            {
                MessageBox.Show("Please enter full data");
                return;
            }
            dbHelper.TableUpdate(txtISBN.Text, txtName.Text, txtAuthors.Text, txtPublisher.Text, int.Parse(txtYear.Text));
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (txtISBN.Text == "")
            {
                MessageBox.Show("Please enter ISBN");
                return;
            }
            dbHelper.TableDelete(txtISBN.Text);
        }
        private void btnFindByYear_Click(object sender, RoutedEventArgs e)
        {
            int year;
            if (int.TryParse(txtYear.Text, out year))
            {
                BooksGrid.ItemsSource = dbHelper.FindBookByYear(year).DefaultView;
            }
            else
            {
                MessageBox.Show("Please enter a valid year.");
            }
        }
        private void btnFindByISBN_Click(object sender, RoutedEventArgs e)
        {
            if (txtISBN.Text == "")
            {
                MessageBox.Show("Please enter ISBN");
                return;
            }
            BooksGrid.ItemsSource = dbHelper.FindBookByISBN(txtISBN.Text).DefaultView;
        }
    }
}
