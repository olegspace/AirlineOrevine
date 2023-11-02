using System.Linq;
using System.Windows;
using DbContext.Database;
using DbContext.Models;
using PassengerDb = DbContext.Models.Passenger;

namespace Passenger.Windows
{
    public partial class SearchPassengerWindow : Window
    {
        private readonly AirlineOrevineDbContext _dbContext;
        private Ticket Ticket { get; set; }

        public SearchPassengerWindow(AirlineOrevineDbContext dbContext, Ticket ticket)
        {
            InitializeComponent();
            _dbContext = dbContext;
            DataContext = this;

            Ticket = ticket;

            RefreshPassengerGrid();
        }

        private void RefreshPassengerGrid()
        {
            var search = SearchTextBox.Text.ToLower();
            PassengerGrid.ItemsSource = _dbContext.Passengers
                .Where(x => x.FirstName.ToLower().Contains(search) || x.LastName.ToLower().Contains(search) ||
                            x.Patronymic.ToLower().Contains(search) || x.PassportId.ToString().Contains(search) ||
                            x.PassportSeries.ToString().Contains(search))
                .ToList();
            PassengerGrid.Items.Refresh();
        }

        private void ChoicePassengerButton_Click(object sender, RoutedEventArgs e)
        {
            if (PassengerGrid.SelectedItems.Count == 1)
            {
                Ticket.Passenger = (PassengerDb)PassengerGrid.SelectedItems[0]!;
            }

            Close();
        }

        private void SearchPassengerButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshPassengerGrid();
        }
    }
}