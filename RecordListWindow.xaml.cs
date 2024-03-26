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

namespace SnakeGame
{
    /// <summary>
    /// Логика взаимодействия для RecordListWindow.xaml
    /// </summary>
    public partial class RecordListWindow : Window
    {
        public RecordListWindow()
        {
            InitializeComponent();
            DataGridRecords.ItemsSource = Entities.GetContext().Records.OrderByDescending(x => x.Record).Take(5).ToList();
        }

        /*private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Entities.GetContext().ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                DataGridRecords.ItemsSource = Entities.GetContext().Records.OrderBy(x => x.Record).Take(5).ToList();
            }
        }*/
    }
}
