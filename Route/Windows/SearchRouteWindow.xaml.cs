using System.Linq;
using System.Windows;
using DbContext.Database;
using DbContext.Models;
using RouteDb = DbContext.Models.Route;

namespace Route.Windows
{
    public partial class SearchRouteWindow : Window
    {
        private readonly AirlineOrevineDbContext _dbContext;
        private Flight Flight { get; set; }
        
        public SearchRouteWindow(AirlineOrevineDbContext dbContext, Flight flight)
        {
            InitializeComponent();
            _dbContext = dbContext;
            DataContext = this;
            Flight = flight;
            RefreshRouteGrid();
        }

        private void RefreshRouteGrid()
        {
            var search = SearchTextBox.Text.ToLower();
            RouteGrid.ItemsSource = _dbContext.Routes
                .Where(x => x.StartingPoint.Name.ToLower().Contains(search) || x.EndingPoint.Name.ToLower().Contains(search) || x.Id.ToString().Contains(search))
                .ToList();
            RouteGrid.Items.Refresh();
        }

        private void ChoiceRouteButton_Click(object sender, RoutedEventArgs e)
        {
            if (RouteGrid.SelectedItems.Count == 1)
            {
                Flight.Route = ((RouteDb)RouteGrid.SelectedItems[0]!);
            }
            Close();
        }

        private void SearchRouteButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshRouteGrid();
        }
    }
}