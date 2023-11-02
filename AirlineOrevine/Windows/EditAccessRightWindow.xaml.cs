using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DbContext.Database;
using DbContext.Models;
using Microsoft.EntityFrameworkCore;

namespace AirlineOrevine.Windows
{
    public partial class EditAccessRightWindow : Window
    {
        private readonly AirlineOrevineDbContext _dbContext;

        private List<AccessRight> AccessRights { get; set; }
        public List<FormTypes> FormTypeColumn { get; set; }
        private User User { get; set; }
        public EditAccessRightWindow(AirlineOrevineDbContext dbContext, User user)
        {
            InitializeComponent();
            _dbContext = dbContext;
            DataContext = this;
            User = user;
            UserLoginTextBox.Text = user.Login;
            FormTypeColumn = Enum.GetValues(typeof(FormTypes)).Cast<FormTypes>().ToList();
            AccessRights = User.AccessRights.ToList();
            AccessRightGrid.ItemsSource = AccessRights;
        }

        private void SaveAccessRightButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
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
