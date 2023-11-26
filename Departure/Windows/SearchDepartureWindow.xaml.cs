using System.Linq;
using System.Windows;
using AirlineOrevine;
using DbContext.Database;
using DbContext.Models;
using DepartureDb = DbContext.Models.Departure;

namespace Departure.Windows
{
    public partial class SearchDepartureWindow : Window
    {
        private readonly AirlineOrevineDbContext _dbContext;
        private Ticket Ticket { get; set; }

        public SearchDepartureWindow(AirlineOrevineDbContext dbContext, Ticket ticket)
        {
            InitializeComponent();
            _dbContext = dbContext;
            DataContext = this;

            Ticket = ticket;

            RefreshDepartureGrid();
        }

        private void RefreshDepartureGrid()
        {
            var search = SearchTextBox.Text.ToLower();
            DepartureGrid.ItemsSource = _dbContext.Departures
                .Where(x => x.Crew.Title.ToLower().Contains(search) || x.Flight.Title.ToLower().Contains(search) ||
                            x.Liner.Name.ToLower().Contains(search) ||
                            x.Flight.Route.StartingPoint.Name.ToLower().Contains(search) ||
                            x.Flight.Route.StartingPoint.City.ToLower().Contains(search) ||
                            x.Flight.Route.StartingPoint.Country.ToLower().Contains(search) ||
                            x.Flight.Route.EndingPoint.Name.ToLower().Contains(search) ||
                            x.Flight.Route.EndingPoint.City.ToLower().Contains(search) ||
                            x.Flight.Route.EndingPoint.Country.ToLower().Contains(search))
                .ToList();
            DepartureGrid.Items.Refresh();
        }

        private void ChoiceDepartureButton_Click(object sender, RoutedEventArgs e)
        {
            if (DepartureGrid.SelectedItems.Count == 1)
            {
                Ticket.Departure = (DepartureDb)DepartureGrid.SelectedItems[0]!;
            }

            Close();
        }

        private void SearchDepartureButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshDepartureGrid();
        }
    }
}