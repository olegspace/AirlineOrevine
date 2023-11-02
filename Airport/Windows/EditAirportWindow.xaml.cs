using System.Windows;
using DbContext.Database;
using Microsoft.EntityFrameworkCore;
using AirportDb = DbContext.Models.Airport;

namespace Airport.Windows
{
    public partial class EditAirportWindow : Window
    {
        private readonly AirlineOrevineDbContext _dbContext;
        public AirportDb Airport { get; set; }
        private bool IsNewAirport { get; set; }
        public EditAirportWindow(AirlineOrevineDbContext dbContext, AirportDb airport, bool isNewAirport)
        {
            InitializeComponent();
            _dbContext = dbContext;
            DataContext = this;
            Airport = airport;
            IsNewAirport = isNewAirport;
        }

        private void SaveButton(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Airport.Name))
            {
                MessageBox.Show("Укажите название");
                return;
            }
            if (string.IsNullOrEmpty(Airport.City))
            {
                MessageBox.Show("Укажите город");
                return;
            }
            if (string.IsNullOrEmpty(Airport.Country))
            {
                MessageBox.Show("Укажите страну");
                return;
            }
            try
            {
                if (IsNewAirport)
                {
                    _dbContext.Airports.Add(Airport);
                }
                _dbContext.SaveChanges();
                Close();
            }
            catch (DbUpdateException exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
    }
}
