using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Migrations;
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
        BookEntities context;
        List<Table> books;
        List<Publishers> publishers;
        List<Table> recent_books;
        List<Table> old_books;

        public MainWindow()
        {
            InitializeComponent();
            context = new BookEntities();
            books = new List<Table>();
            publishers = new List<Publishers>();
            recent_books = new List<Table>();
            old_books = new List<Table>();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (Table table in context.Table)
            {
                books.Add(table);
            }
            foreach (Publishers publisher in context.Publishers)
            {
                publishers.Add(publisher);
            }
            BooksGrid.ItemsSource = books;
            PublishersGrid.ItemsSource = publishers;
        }

        private void Tab_Old_Loaded(object sender, RoutedEventArgs e)
        {
            old_books = books.Where(s => s.Year < 1975).ToList();
            OldBooksGrid.ItemsSource = old_books;
        }
        private void Tab_Recent_Loaded(object sender, RoutedEventArgs e)
        {
            using (var new_context = new BookEntities())
            {
                var query = from book in new_context.Table where book.Year >= 2010 select book;
                recent_books = query.ToList();
                RecentBooksGrid.ItemsSource = recent_books;
            }
        }
        private void Tab_PublishedByHarperCollins_Loaded(object sender, RoutedEventArgs e)
        {
            var published_by_harper_collins = context.Table.SqlQuery("SELECT * FROM dbo.[Table] WHERE Publisher = 2;").ToList();
            PublishedByHarperCollinsGrid.ItemsSource = published_by_harper_collins;
        }
        private void Tab_Join_Loaded(object sender, RoutedEventArgs e)
        {

            var join_tables = context.Table.Join(context.Publishers, t => t.Publisher, m => m.Publisher_id, (t, m) 
                => new {t.Id, t.ISBN, t.Name, t.Authors, PublisherName = m.Publisher, t.Year }).ToList();
            Join.ItemsSource = join_tables;
        }
        private DataGrid GetActiveGrid()
        {
            if (tabControl.SelectedItem is TabItem selectedTab)
            {
                return selectedTab.Content as DataGrid;
            }
            return null;
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataGrid activeGrid = GetActiveGrid();
                if (activeGrid == null) return;
                using (var new_context = new BookEntities())
                {
                    if (activeGrid.Name == "BooksGrid")
                    {
                        activeGrid.ItemsSource = new_context.Table.ToList();
                        MessageBox.Show("Books reloaded");
                    }
                    else if (activeGrid.Name == "PublishersGrid")
                    {
                        activeGrid.ItemsSource = new_context.Publishers.ToList();
                        MessageBox.Show("Publishers reloaded");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataGrid activeGrid = GetActiveGrid();
                if (activeGrid == null) return;            
            
                foreach (var item in activeGrid.ItemsSource)
                {
                
                    if (activeGrid == BooksGrid)
                    {
                        context.Table.AddOrUpdate((Table)item);
                    }
                    else if (activeGrid == PublishersGrid)
                    {
                        context.Publishers.AddOrUpdate((Publishers)item);
                    }
                }                
                context.SaveChanges();
                MessageBox.Show($"{(activeGrid.Name == "BooksGrid" ? "Books" : "Publishers")} saved");
            }
            catch (Exception ex) 
            { 
                MessageBox.Show(ex.Message);            
            }
}

        private void btnDelete_click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataGrid activeGrid = GetActiveGrid();
                if (activeGrid == null || (activeGrid.Name != "BooksGrid" && activeGrid.Name != "PublishersGrid")) return;
                if (activeGrid.Name == "BooksGrid")
                {
                    if (activeGrid.SelectedCells.Count > 0)
                    {
                        for (int i = 0; i < activeGrid.SelectedCells.Count; i++)
                        {
                            var selected = (Table)activeGrid.SelectedCells[i].Item;
                            context.Table.Remove(selected);
                        }
                        context.SaveChanges();
                        activeGrid.ItemsSource = context.Table.ToList();
                        MessageBox.Show("Row(s) deleted");
                    }
                    else MessageBox.Show("Select row(s) for this operation");
                }
                else
                {
                    if (activeGrid.SelectedCells.Count > 0)
                    {
                        for (int i = 0; i < activeGrid.SelectedCells.Count; i++)
                        {
                            var selected = (Publishers)activeGrid.SelectedCells[i].Item;
                            context.Publishers.Remove(selected);
                        }
                        context.SaveChanges();
                        activeGrid.ItemsSource = context.Publishers.ToList();
                        MessageBox.Show("Row(s) deleted");
                    }
                    else MessageBox.Show("Select row(s) for this operation");
                }
            }
            catch (Exception ex) 
            { 
                MessageBox.Show(ex.Message);            
            }
        }

        private void Paste(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                try
                {
                    string clipboardData = Clipboard.GetText();
                    if (string.IsNullOrWhiteSpace(clipboardData)) return;

                    DataGrid activeGrid = GetActiveGrid();
                    if (activeGrid == null || activeGrid.ItemsSource == null) return;
                    var firstCell = activeGrid.SelectedCells[0];
                    var indexC = activeGrid.Columns.IndexOf(firstCell.Column);
                
                    var entityType = activeGrid.ItemsSource.Cast<object>().FirstOrDefault()?.GetType();
                    if (entityType == null) return;

                    var newItem = Activator.CreateInstance(entityType);
                    var properties = entityType.GetProperties();
                    string[] input = clipboardData.Split('\t');
                    string[] values = new string[Math.Min(properties.Length - indexC, input.Length)];
                    Array.Copy(input, values, values.Length);

                    for (int i = indexC; i < Math.Min(values.Length, properties.Length) + indexC; i++)
                    {                    
                        properties[i].SetValue(newItem, Convert.ChangeType(values[i - indexC], properties[i].PropertyType));                  
                    }

                    context.Entry(newItem).State = System.Data.Entity.EntityState.Added;
                    (activeGrid.ItemsSource as IList)?.Add(newItem); 
      
                    activeGrid.Items.Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }        
    }
}
