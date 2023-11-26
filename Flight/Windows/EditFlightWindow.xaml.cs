using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DbContext.Database;
using Microsoft.EntityFrameworkCore;
using Route.Windows;
using FlightDb = DbContext.Models.Flight;
using RouteDb = DbContext.Models.Route;

namespace Flight.Windows
{
    public partial class EditFlightWindow : Window
    {
        private readonly AirlineOrevineDbContext _dbContext;
        public FlightDb Flight { get; set; }
        private bool IsNewFlight { get; set; } = false;

        public EditFlightWindow(AirlineOrevineDbContext dbContext, FlightDb flight, bool isNewFlight)
        {
            InitializeComponent();
            _dbContext = dbContext;
            DataContext = this;
            Flight = flight;
            IsNewFlight = isNewFlight;
            RefreshRouteGrid();
        }

        private void RefreshRouteGrid()
        {
            List<RouteDb> newRoutes = new List<RouteDb>();
            if (Flight.Route != null)
            {
                newRoutes.Add(Flight.Route);
            }

            RouteGrid.ItemsSource = newRoutes.ToList();
            RouteGrid.Items.Refresh();
        }


        private void AddRouteButton_Click(object sender, RoutedEventArgs e)
        {
            var searchRouteWindow = new SearchRouteWindow(_dbContext, Flight);
            searchRouteWindow.ShowDialog();
            RefreshRouteGrid();
        }

        private void SaveFlightButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Flight.Title))
            {
                MessageBox.Show("Укажите название рейса", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (Flight.Route == null)
            {
                MessageBox.Show("Выберите маршрут", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                if (IsNewFlight)
                {
                    _dbContext.Flights.Add(Flight);
                }

                _dbContext.SaveChanges();
                Close();
            }
            catch (DbUpdateException exception)
            {
                MessageBox.Show(exception.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}