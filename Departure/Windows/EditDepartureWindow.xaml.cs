using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using AirlineOrevine;
using Crew.Windows;
using DbContext.Database;
using Flight.Windows;
using Liner.Windows;
using Microsoft.EntityFrameworkCore;
using DepartureDb = DbContext.Models.Departure;
using FlightDb = DbContext.Models.Flight;

namespace Departure.Windows
{
    public partial class EditDepartureWindow : Window
    {
        private readonly AirlineOrevineDbContext _dbContext;

        public DepartureDb Departure { get; set; }

        private bool IsNewDeparture { get; set; } = false;

        public EditDepartureWindow(AirlineOrevineDbContext dbContext, DepartureDb departure, bool isNewDeparture)
        {
            InitializeComponent();
            _dbContext = dbContext;
            DataContext = this;
            Departure = departure;
            IsNewDeparture = isNewDeparture;

            if (isNewDeparture)
            {
                Departure.DepartureTime = DateTime.Now;
                Departure.ArrivalTime = DateTime.Now;
            }
            RefreshFlightGrid();
        }

        private void RefreshFlightGrid()
        {
            List<FlightDb> newFlights = new List<FlightDb>();
            if (Departure.Flight != null)
            {
                newFlights.Add(Departure.Flight);
            }

            FlightGrid.ItemsSource = newFlights.ToList();
            FlightGrid.Items.Refresh();
        }
        
        private void AddFlightButton_Click(object sender, RoutedEventArgs e)
        {
            var searchFlightWindow = new SearchFlightWindow(_dbContext, Departure);
            searchFlightWindow.ShowDialog();
            RefreshFlightGrid();
        }

        private void SaveDepartureButton_Click(object sender, RoutedEventArgs e)
        {
            if (Departure.Crew == null)
            {
                MessageBox.Show("Укажите экипаж");
                return;
            }

            if (Departure.Liner == null)
            {
                MessageBox.Show("Укажите авиалайнер");
                return;
            }

            if (Departure.Flight == null)
            {
                MessageBox.Show("Укажите рейс");
                return;
            }

            try
            {
                if (IsNewDeparture)
                {
                    _dbContext.Departures.Add(Departure);
                }

                _dbContext.SaveChanges();
                Close();
            }
            catch (DbUpdateException exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void AddCrewButton_Click(object sender, RoutedEventArgs e)
        {
            var searchCrewWindow = new SearchCrewWindow(_dbContext, Departure);
            searchCrewWindow.ShowDialog();
            
            if (Departure.Crew is not null)
            {
                CrewTextBox.Text = Departure.Crew.Title;
            }
        }

        private void AddLinerButton_Click(object sender, RoutedEventArgs e)
        {
            var searchLinerWindow = new SearchLinerWindow(_dbContext, Departure);
            searchLinerWindow.ShowDialog();
            
            if (Departure.Liner is not null)
            {
                LinerTextBox.Text = Departure.Liner.Name;
            }
        }
    }
}