using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using AirlineOrevine;
using DbContext.Database;
using DbContext.Enums;
using DbContext.Models;
using Departure.Windows;
using Employee.Windows;
using Microsoft.EntityFrameworkCore;
using Passenger.Windows;
using TicketDb = DbContext.Models.Ticket;
using EmployeeDb = DbContext.Models.Employee;
using DepartureDb = DbContext.Models.Departure;
using PassengerDb = DbContext.Models.Passenger;

namespace Ticket.Windows
{
    public partial class EditTicketWindow : Window
    {
        private readonly AirlineOrevineDbContext _dbContext;
        public TicketDb Ticket { get; set; }
        private bool IsNewTicket { get; set; } = false;
        public List<ServiceTypes> ServiceClassColumn { get; set; }
        public EditTicketWindow(AirlineOrevineDbContext dbContext, TicketDb ticket, bool isNewTicket)
        {
            InitializeComponent();
            _dbContext = dbContext;
            DataContext = this;
            Ticket = ticket;
            IsNewTicket = isNewTicket;
            ServiceClassColumn = Enum.GetValues(typeof(ServiceTypes)).Cast<ServiceTypes>().ToList();

            if (isNewTicket)
            {
                Ticket.PurchaseDate = DateTime.Now;
            }

            RefreshCashierGrid();
            RefreshDepartureGrid();
            RefreshPassengerGrid();
        }

        private void RefreshCashierGrid()
        {

            List<EmployeeDb> newEmployee = new List<EmployeeDb>();
            if (Ticket.Employee != null)
            {
                newEmployee.Add(Ticket.Employee);
            }

            CashierGrid.ItemsSource = newEmployee.ToList();
            CashierGrid.Items.Refresh();
        }  
        private void RefreshDepartureGrid()
        {

            List<DepartureDb> newDeparture = new List<DepartureDb>();
            if (Ticket.Departure != null)
            {
                newDeparture.Add(Ticket.Departure);
            }

            DepartureGrid.ItemsSource = newDeparture.ToList();
            DepartureGrid.Items.Refresh();
        }   
        private void RefreshPassengerGrid()
        {

            List<PassengerDb> newPassenger = new List<PassengerDb>();
            if (Ticket.Passenger != null)
            {
                newPassenger.Add(Ticket.Passenger);
            }

            PassengerGrid.ItemsSource = newPassenger.ToList();
            PassengerGrid.Items.Refresh();
        }


        private void AddDepartureButton_Click(object sender, RoutedEventArgs e)
        {
            var searchDepartureWindow = new SearchDepartureWindow(_dbContext, Ticket);
            searchDepartureWindow.ShowDialog();
            RefreshDepartureGrid();
        }

        private void AddCashierButton_Click(object sender, RoutedEventArgs e)
        {
            var SearchCashierWindow = new SearchCashierWindow(_dbContext, Ticket);
            SearchCashierWindow.ShowDialog();
            RefreshCashierGrid();
        }

        private void AddPassengerButton_Click(object sender, RoutedEventArgs e)
        {
            var searchPassengerWindow = new SearchPassengerWindow(_dbContext, Ticket);
            searchPassengerWindow.ShowDialog();
            RefreshPassengerGrid();

        }

        private void SaveTicketButton_Click(object sender, RoutedEventArgs e)
        {
            if (Ticket.Departure == null)
            {
                MessageBox.Show("Укажите вылет", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (Ticket.Passenger == null)
            {
                MessageBox.Show("Укажите пассажира", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (Ticket.Employee == null)
            {
                MessageBox.Show("Укажите кассира", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (string.IsNullOrEmpty(Ticket.Place))
            {
                MessageBox.Show("Укажите место", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if ((Ticket.CheckoutNumber.ToString() == null) || Ticket.CheckoutNumber.ToString() == "0")
            {
                MessageBox.Show("Укажите кассу", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            try
            {
                if (IsNewTicket)
                {
                    _dbContext.Tickets.Add(Ticket);
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
