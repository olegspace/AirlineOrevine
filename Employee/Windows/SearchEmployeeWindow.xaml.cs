using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DbContext.Database;
using DbContext.Enums;
using DbContext.Models;
using EmployeeDb = DbContext.Models.Employee;

namespace Employee.Windows
{
    public partial class SearchEmployeeWindow : Window
    {
        private readonly AirlineOrevineDbContext _dbContext;
        private Crew Crew { get; set; }

        public SearchEmployeeWindow(AirlineOrevineDbContext dbContext, Crew crew)
        {
            InitializeComponent();
            this._dbContext = dbContext;
            DataContext = this;

            Crew = crew;

            RefreshEmployeeGrid();
        }

        private void RefreshEmployeeGrid()
        {
            var search = SearchTextBox.Text.ToLower();
            EmployeeGrid.ItemsSource = _dbContext.Employees
                .Where(x => (x.FirstName.ToLower().Contains(search) || x.LastName.ToLower().Contains(search) ||
                             x.Patronymic.ToLower().Contains(search)) && x.EmployeeType != EmployeeTypes.Cashier)
                .ToList();
            EmployeeGrid.Items.Refresh();
        }

        private void ChoiseEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeGrid.SelectedItems.Count == 1)
            {
                if (Crew.Employees == null)
                {
                    Crew.Employees = new List<EmployeeDb>();
                }

                if (Crew.Employees.Any(x => x.Id == ((EmployeeDb)EmployeeGrid.SelectedItems[0]!).Id))
                {
                    MessageBox.Show("Данный сотрудник уже есть в экипаже", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                Crew.Employees.Add((EmployeeDb)EmployeeGrid.SelectedItems[0]!);
            }

            Close();
        }

        private void SearchEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshEmployeeGrid();
        }
    }
}