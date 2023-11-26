using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Airport.Windows;
using DbContext.Database;
using Microsoft.EntityFrameworkCore;
using RouteDb = DbContext.Models.Route;
using AirportDb = DbContext.Models.Airport;

namespace Route.Windows
{
    public partial class EditRouteWindow : Window
    {
        private readonly AirlineOrevineDbContext _dbContext;

        public RouteDb Route { get; set; }

        private bool IsNewRoute { get; set; }

        public EditRouteWindow(AirlineOrevineDbContext dbContext, RouteDb route, bool isNewRoute)
        {
            InitializeComponent();
            _dbContext = dbContext;
            DataContext = this;
            Route = route;
            IsNewRoute = isNewRoute;

            RefreshWayPointGrid();
        }

        private void RefreshWayPointGrid()
        {
            WayPointGrid.ItemsSource = Route.WayPoints?.ToList();
            WayPointGrid.Items.Refresh();
        }

        private void AddStartingPointButton_Click(object sender, RoutedEventArgs e)
        {
            AddAirport(StartingPointTextBox, "Starting");
        }

        private void AddEndingPointButton_Click(object sender, RoutedEventArgs e)
        {
            AddAirport(EndingPointTextBox, "Ending");
        }

        private void AddAirport(TextBox textBox, string routeType)
        {
            var searchAirportWindow = new SearchAirportWindow(_dbContext, Route, routeType);
            searchAirportWindow.ShowDialog();
            
            if (Route.StartingPoint is not null)
            {
                if (routeType == "Starting")
                {
                    textBox.Text = Route.StartingPoint.Name;
                }
            }

            if (Route.EndingPoint is not null)
            {
                if (routeType == "Ending")
                {
                    textBox.Text = Route.EndingPoint.Name;
                }
            }
        }

        private void SaveRouteButton_Click(object sender, RoutedEventArgs e)
        {
            if (Route.StartingPoint == null)
            {
                MessageBox.Show("Укажите начальный пункт", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (Route.EndingPoint == null)
            {
                MessageBox.Show("Укажите конечный пункт", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                if (IsNewRoute)
                {
                    _dbContext.Routes.Add(Route);
                }

                _dbContext.SaveChanges();
                Close();
            }
            catch (DbUpdateException exception)
            {
                MessageBox.Show(exception.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddWayPointButtonClick(object sender, RoutedEventArgs e)
        {
            AddAirport(EndingPointTextBox, "Way");
            RefreshWayPointGrid();
        }

        private void DeleteWayPointButton_Click(object sender, RoutedEventArgs e)
        {
            if (WayPointGrid.SelectedItems.Count > 0)
            {
                for (int i = 0; i < WayPointGrid.SelectedItems.Count; i++)
                {
                    if (WayPointGrid.SelectedItems[i] is AirportDb airport)
                    {
                        Route.WayPoints.Remove(airport);
                    }
                }
            }
            RefreshWayPointGrid();
        }
    }
}