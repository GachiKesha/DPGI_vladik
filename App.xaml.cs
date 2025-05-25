using System.Collections.ObjectModel;
using System.Windows;

namespace DPGI_vladik
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ObservableCollection<HistoryEntry> HistoryCollection { get; } = new ObservableCollection<HistoryEntry>();
    }
}
