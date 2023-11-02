using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using AirlineOrevine;
using AirlineOrevine.Files;
using Common.Utilities;
using DbContext.Database;
using DbContext.Enums;
using DbContext.Models;
using Microsoft.EntityFrameworkCore;
using LinerDb = DbContext.Models.Liner;

namespace Liner.Windows
{
    public partial class EditLinerWindow : Window
    {
        private readonly AirlineOrevineDbContext _dbContext;
        
        public BitmapImage? BitmapPhoto { get; set; }

        public LinerDb Liner { get; set; }

        private bool IsNewLiner { get; set; } = false;
        
        public List<LinerTypes> LinerTypeColumn { get; set; }
        
        public EditLinerWindow(AirlineOrevineDbContext dbContext, LinerDb liner, bool isNewLiner)
        {
            InitializeComponent();
            DataContext = this;
            Liner = liner;
            IsNewLiner = isNewLiner;
            LinerTypeColumn = Enum.GetValues(typeof(LinerTypes)).Cast<LinerTypes>().ToList();
            _dbContext = dbContext;
            PhotoImage.Source = BitmapPhoto;

            if (isNewLiner)
            {
                Liner.DateOfIssue = DateTime.Now;
                Liner.InspectionDate = DateTime.Now;
            }

            Photo.LoadPhoto(Liner.Photo ?? LinerPhoto.img, PhotoImage, liner);
        }

        private void SaveLinerButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Liner.Name))
            {
                MessageBox.Show("Укажите имя");
                return;
            }
            try
            {
                if(IsNewLiner)
                {
                    _dbContext.Liners.Add(Liner);
                }
     
                _dbContext.SaveChanges();
                Close();
            }
            catch (DbUpdateException exception)
            {
                MessageBox.Show(exception.Message);
            }

        }

        private void AddPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            Photo.AddPhoto(Liner, PhotoImage);
        }
    }
}
