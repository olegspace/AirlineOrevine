using System.Linq;
using System.Windows;
using DbContext.Database;
using DbContext.Models;
using FlightDb = DbContext.Models.Flight;

namespace Flight.Windows
{
    public partial class SearchFlightWindow : Window
    {
        private readonly AirlineOrevineDbContext _dbContext;
        private Departure Departure { get; set; }

        public SearchFlightWindow(AirlineOrevineDbContext dbContext, Departure departure)
        {
            InitializeComponent();
            this._dbContext = dbContext;
            DataContext = this;
            Departure = departure;

            RefreshFlightGrid();
        }

        private void RefreshFlightGrid()
        {
            var search = SearchTextBox.Text.ToLower();
            FlightGrid.ItemsSource = _dbContext.Flights
                .Where(x => x.Route.StartingPoint.Name.ToLower().Contains(search) ||
                            x.Route.EndingPoint.Name.ToLower().Contains(search) ||
                            x.Route.Id.ToString().Contains(search))
                .ToList();
            FlightGrid.Items.Refresh();
        }

        private void ChoiceFlightButton_Click(object sender, RoutedEventArgs e)
        {
            if (FlightGrid.SelectedItems.Count == 1)
            {
                Departure.Flight = ((FlightDb)FlightGrid.SelectedItems[0]!);
            }

            Close();
        }

        private void SearchFlightButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshFlightGrid();
        }
    }
}