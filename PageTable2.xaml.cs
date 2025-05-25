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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DPGI_vladik
{
    /// <summary>
    /// Interaction logic for PageTable2.xaml
    /// </summary>
    public partial class PageTable2 : Page
    {
        public PageTable2()
        {
            InitializeComponent();
            LoadData();
        }
        private void LoadData()
        {
            using (var context = new Entities())
            {
                ConvertedNotes.ItemsSource = context.Numbers.ToList();
            }
        }
    }
}
