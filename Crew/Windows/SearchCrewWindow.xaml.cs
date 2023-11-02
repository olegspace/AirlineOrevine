using System.Linq;
using System.Windows;
using DbContext.Database;
using DbContext.Models;
using CrewDb = DbContext.Models.Crew;

namespace Crew.Windows
{
    public partial class SearchCrewWindow : Window
    {
        private readonly AirlineOrevineDbContext _dbContext;
        private Departure Departure { get; set; }
   
        public SearchCrewWindow(AirlineOrevineDbContext dbContext, Departure departure)
        {
            InitializeComponent();
            this._dbContext = dbContext;
            DataContext = this;
            Departure = departure;

            RefreshCrewGrid();
        }

        private void RefreshCrewGrid()
        {
            var search = SearchTextBox.Text.ToLower();
            CrewGrid.ItemsSource = _dbContext.Crews
                .Where(x => x.Title.ToLower().Contains(search))
                .ToList();
            CrewGrid.Items.Refresh();
        }

        private void ChoiсeCrewButton_Click(object sender, RoutedEventArgs e)
        {
            if (CrewGrid.SelectedItems.Count == 1)
            {
                Departure.Crew = (CrewDb)CrewGrid.SelectedItems[0]!;
          
            }
            Close();
        }

        private void SearchCrewButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshCrewGrid();
        }
    }
}