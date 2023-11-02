using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DbContext.Database;
using DbContext.Enums;
using DbContext.Models;
using Microsoft.EntityFrameworkCore;
using EmployeeDb = DbContext.Models.Employee;

namespace Employee.Windows
{
    public partial class EditEmployeeWindow : Window
    {

        private readonly AirlineOrevineDbContext _dbContext;
        
        public EmployeeDb Employee { get; set; }
        
        private bool IsNewEmployee { get; set; }
        
        public List<EmployeeTypes> EmployeeTypeColumn { get; set; }
        

        public EditEmployeeWindow(AirlineOrevineDbContext dbContext, EmployeeDb employee, bool isNewEmployee)
        {
            InitializeComponent();
            EmployeeTypeColumn = Enum.GetValues(typeof(EmployeeTypes)).Cast<EmployeeTypes>().ToList();
            _dbContext = dbContext;
            DataContext = this;
            Employee = employee;
            IsNewEmployee = isNewEmployee;
        }

        private void SaveEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Employee.FirstName))
            {
                MessageBox.Show("Укажите имя");
                return;
            }
            if (string.IsNullOrEmpty(Employee.LastName))
            {
                MessageBox.Show("Укажите фамилию");
                return;
            }
            if (string.IsNullOrEmpty(Employee.Patronymic))
            {
                MessageBox.Show("Укажите отчество");
                return;
            }
            try
            {
                if (IsNewEmployee)
                {
                    _dbContext.Employees.Add(Employee);
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
