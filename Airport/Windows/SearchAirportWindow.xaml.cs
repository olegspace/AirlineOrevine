using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DbContext.Database;
using DbContext.Models;
using AirportDb = DbContext.Models.Airport;

namespace Airport.Windows
{
    public partial class SearchAirportWindow : Window
    {
        private readonly AirlineOrevineDbContext _dbContext;
        private Route Route { get; set; }
        private string RouteType { get; set; }


        public SearchAirportWindow(AirlineOrevineDbContext dbContext, Route route, string routeType)
        {
            InitializeComponent();
            this._dbContext = dbContext;
            DataContext = this;

            Route = route;
            RouteType = routeType;
            RefreshAirportGrid();
        }

        private void RefreshAirportGrid()
        {
            var search = SearchTextBox.Text.ToLower();
            AirportGrid.ItemsSource = _dbContext.Airports
                .Where(x => x.City.ToLower().Contains(search) || x.Country.ToLower().Contains(search) ||
                            x.Name.ToLower().Contains(search))
                .ToList();
            AirportGrid.Items.Refresh();
        }

        private void SearchAirportButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshAirportGrid();
        }

        private void ChoiseAirportButton_Click(object sender, RoutedEventArgs e)
        {
            if (AirportGrid.SelectedItems.Count == 1)
            {
                if (RouteType == "Starting")
                {
                    Route.StartingPoint = (AirportDb)AirportGrid.SelectedItems[0]!;
                }

                if (RouteType == "Ending")
                {
                    Route.EndingPoint = (AirportDb)AirportGrid.SelectedItems[0]!;
                }

                if (RouteType == "Way")
                {
                    if (Route.WayPoints == null)
                    {
                        Route.WayPoints = new List<AirportDb>();
                    }

                    if (Route.WayPoints.Any(x => x.Id == ((AirportDb)AirportGrid.SelectedItems[0]!).Id))
                    {
                        MessageBox.Show("Данный промежуточный пункт уже присутствует", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    Route.WayPoints.Add((AirportDb)AirportGrid.SelectedItems[0]!);
                }
            }

            Close();
        }
    }
}