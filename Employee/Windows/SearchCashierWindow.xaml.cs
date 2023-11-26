using System.Linq;
using System.Windows;
using DbContext.Database;
using DbContext.Enums;
using DbContext.Models;
using EmployeeDb = DbContext.Models.Employee;

namespace Employee.Windows
{
    public partial class SearchCashierWindow : Window
    {
        private readonly AirlineOrevineDbContext _dbContext;
        
        private Ticket Ticket { get; set; }
        
        
        public SearchCashierWindow(AirlineOrevineDbContext dbContext, Ticket ticket)
        {
            InitializeComponent();
            _dbContext = dbContext;
            DataContext = this;

            Ticket = ticket;

            RefreshCashierGrid();
        }

        private void RefreshCashierGrid()
        {
            var search = SearchTextBox.Text.ToLower();
            CashierGrid.ItemsSource = _dbContext.Employees
                .Where(x => (x.FirstName.ToLower().Contains(search) || 
                x.LastName.ToLower().Contains(search) || 
                x.Patronymic.ToLower().Contains(search)) && x.EmployeeType == EmployeeTypes.Cashier)
                .ToList();
            CashierGrid.Items.Refresh();
        }

        private void ChoiseEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            if (CashierGrid.SelectedItems.Count == 1)
            {
               
                Ticket.Employee = (EmployeeDb)CashierGrid.SelectedItems[0]!;
            }
            Close();
        }

        private void SearchEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshCashierGrid();
        }
    }
}