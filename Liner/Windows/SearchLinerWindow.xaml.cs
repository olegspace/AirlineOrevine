using System.Linq;
using System.Windows;
using AirlineOrevine;
using DbContext.Database;
using DbContext.Models;
using LinerDb = DbContext.Models.Liner;

namespace Liner.Windows
{
    public partial class SearchLinerWindow : Window
    {
        private readonly AirlineOrevineDbContext dbContext;
        public Departure Departure { get; set; }
  
        public SearchLinerWindow(AirlineOrevineDbContext dbContext, Departure departure)
        {
            InitializeComponent();
            this.dbContext = dbContext;
            DataContext = this;
        
            Departure = departure;

            RefreshLinerGrid();
        }

        private void RefreshLinerGrid()
        {
            var search = SearchTextBox.Text.ToLower();
            LinerGrid.ItemsSource = dbContext.Liners
                .Where(x => x.Name.ToLower().Contains(search))
                .ToList();
            LinerGrid.Items.Refresh();
        }

        private void SearchLinerButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshLinerGrid();
        }

        private void ChoiseLinerButton_Click(object sender, RoutedEventArgs e)
        {
            if (LinerGrid.SelectedItems.Count == 1)
            {
                Departure.Liner = (LinerDb)LinerGrid.SelectedItems[0]!;
             
            }
            Close();
        }
    }
}