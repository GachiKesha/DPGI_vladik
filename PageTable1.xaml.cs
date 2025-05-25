using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace DPGI_vladik
{
    /// <summary>
    /// Interaction logic for PageTable1.xaml
    /// </summary>
    public partial class PageTable1 : Page
    {
        private Stack<HistoryEntry> deletedStack;
        private List<DataGridRow> foundRows = new List<DataGridRow>();
        private int currentFoundIndex = -1;
        public PageTable1()
        {
            InitializeComponent();
            deletedStack = new Stack<HistoryEntry>();
            HistoryGrid.ItemsSource = App.HistoryCollection;
        }

        private void HistoryGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();
        }

        public void UndoCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = deletedStack != null && deletedStack.Count > 0;
            if (e.CanExecute) Debug.WriteLine("Undo can execute");
            else Debug.WriteLine("Undo can not execute");
        }
        public void FindCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = App.HistoryCollection.Any();
            if (e.CanExecute) Debug.WriteLine("Find can execute");
            else Debug.WriteLine("Find can not execute");
        }
        public void DeleteCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = HistoryGrid != null && HistoryGrid.SelectedItem != null;
            if (e.CanExecute) Debug.WriteLine("Delete can execute");
            else Debug.WriteLine("Delete can not execute");
        }
        private void UndoCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (deletedStack.Count > 0)
            {
                var restored = deletedStack.Pop();
                App.HistoryCollection.Insert(0, restored);
                HistoryGrid.Items.Refresh();
            }
        }
        private void FindCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SearchPanel.Visibility = Visibility.Visible;
            SearchTextBox.Focus();
            ClearHighlight();
        }
        private void DeleteCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (HistoryGrid.SelectedItem is HistoryEntry selected)
            {
                deletedStack.Push(selected);
                App.HistoryCollection.Remove(selected);
                HistoryGrid.Items.Refresh();
            }
        }
        private void CloseSearchPanel_Click(object sender, RoutedEventArgs e)
        {
            SearchPanel.Visibility = Visibility.Collapsed;
            ClearHighlight();
            Keyboard.ClearFocus();
        }

        private void SearchNext_Click(object sender, RoutedEventArgs e)
        {
            if (foundRows.Count == 0) return;
            currentFoundIndex = (currentFoundIndex + 1) % foundRows.Count;
            HighlightRow(currentFoundIndex);
        }

        private void SearchPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (foundRows.Count == 0) return;
            currentFoundIndex = (currentFoundIndex - 1 + foundRows.Count) % foundRows.Count;
            HighlightRow(currentFoundIndex);
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ClearHighlight();
            string search = SearchTextBox.Text.Trim();
            if (string.IsNullOrEmpty(search)) return;

            foundRows.Clear();

            for (int i = 0; i < HistoryGrid.Items.Count; i++)
            {
                var item = HistoryGrid.Items[i] as HistoryEntry;
                if (item == null) continue;

                if (item.InputValue.Contains(search) || item.OutputValue.Contains(search))
                {
                    var row = (DataGridRow)HistoryGrid.ItemContainerGenerator.ContainerFromIndex(i);
                    if (row != null)
                    {
                        foundRows.Add(row);
                    }
                }
            }

            if (foundRows.Count > 0)
            {
                currentFoundIndex = 0;
                HighlightRow(currentFoundIndex);
            }
        }
        private void HighlightRow(int index)
        {
            ClearHighlight();

            var row = foundRows[index];
            if (row != null)
            {
                row.IsSelected = true;
                row.Background = Brushes.LightGoldenrodYellow;
                HistoryGrid.ScrollIntoView(row.Item);
            }
        }

        private void ClearHighlight()
        {
            foreach (var row in foundRows)
            {
                row.Background = Brushes.White;
                row.IsSelected = false;
            }
        }
    }

    public class HistoryEntry
    {
        public string InputValue { get; set; }
        public string OutputValue { get; set; }
        public DateTime Timestamp { get; set; } 
    }
}
