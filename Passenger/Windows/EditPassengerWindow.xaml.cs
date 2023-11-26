using System;
using System.Windows;
using AirlineOrevine;
using DbContext.Database;
using Microsoft.EntityFrameworkCore;
using PassengerDb = DbContext.Models.Passenger;

namespace Passenger.Windows
{
    public partial class EditPassengerWindow : Window
    {
        private readonly AirlineOrevineDbContext _dbContext;
        public PassengerDb Passenger { get; set; }
        
        private bool IsNewPassenger { get; set; } = false;
        public EditPassengerWindow(AirlineOrevineDbContext dbContext, PassengerDb passenger, bool isNewPassenger)
        {
            InitializeComponent();
            _dbContext = dbContext;
            DataContext = this;
            Passenger = passenger;
            IsNewPassenger = isNewPassenger;
            
            if (IsNewPassenger)
            {
                Passenger.DateOfIssue = DateOnly.FromDateTime(DateTime.Now);
            }
        }

        private void SavePassengerButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Passenger.FirstName))
            {
                MessageBox.Show("Укажите имя", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (string.IsNullOrEmpty(Passenger.LastName))
            {
                MessageBox.Show("Укажите фамилию", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (string.IsNullOrEmpty(Passenger.Patronymic))
            {
                MessageBox.Show("Укажите отчество", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (Passenger.PassportSeries.ToString() == null || Passenger.PassportSeries.ToString() == "0")
            {
                MessageBox.Show("Укажите серию", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if ((Passenger.PassportId.ToString() == null) || Passenger.PassportId.ToString() == "0")
            {
                MessageBox.Show("Укажите номер", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (string.IsNullOrEmpty(Passenger.IssuedBy))
            {
                MessageBox.Show("Укажите кем выдан паспорт", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            try
            {
                if (IsNewPassenger)
                {
                    _dbContext.Passengers.Add(Passenger);
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
