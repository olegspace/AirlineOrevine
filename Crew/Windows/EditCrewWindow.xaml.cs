using System.Linq;
using System.Windows;
using System.Windows.Input;
using DbContext.Database;
using Employee.Windows;
using Microsoft.EntityFrameworkCore;
using CrewDb = DbContext.Models.Crew;
using EmployeeDb = DbContext.Models.Employee;

namespace Crew.Windows
{
    public partial class EditCrewWindow : Window
    {
        private readonly AirlineOrevineDbContext _dbContext;
        public CrewDb Crew { get; set; }
        private bool IsNewCrew{ get; set; } = false;
        public EditCrewWindow(AirlineOrevineDbContext dbContext, CrewDb crew, bool isNewCrew)
        {
            Cursor = Cursors.Wait;
            InitializeComponent();
            _dbContext = dbContext;
            DataContext = this;
            Crew = crew;
            IsNewCrew = isNewCrew;            
            RefreshFlightCrewtGrid();
            Cursor = Cursors.Arrow;
        }

        private void RefreshFlightCrewtGrid()
        {
            Cursor = Cursors.Wait;
            FlightCrewGrid.ItemsSource = Crew.Employees?.ToList();
            FlightCrewGrid.Items.Refresh();
            Cursor = Cursors.Arrow;
        }

        private void DeleteEmployeeButtonClick(object sender, RoutedEventArgs e)
        {
            if (FlightCrewGrid.SelectedItems.Count > 0)
            {
                for (int i = 0; i < FlightCrewGrid.SelectedItems.Count; i++)
                {
                    if (FlightCrewGrid.SelectedItems[i] is EmployeeDb employee)
                    {
                        Crew.Employees.Remove(employee);
                    }
                }
            }
            RefreshFlightCrewtGrid();
        }

        private void AddEmployeeButtonClick(object sender, RoutedEventArgs e)
        {            
            var searchEmployeeWindow = new SearchEmployeeWindow(_dbContext, Crew);
            searchEmployeeWindow.ShowDialog();
            RefreshFlightCrewtGrid();
        }

    

        private void SaveButton(object sender, RoutedEventArgs e)
        {            
            if (Crew.Title == null)
            {
                MessageBox.Show("Укажите название экипажа");
                return;
            }
     
            try
            {
                if (IsNewCrew)
                {
                    _dbContext.Crews.Add(Crew);
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
