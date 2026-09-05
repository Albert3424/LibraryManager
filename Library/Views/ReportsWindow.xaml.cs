using Library.Data;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Library.Views
{
    public partial class ReportsWindow : Window
    {
        private LibraryContext _context;

        public ReportsWindow()
        {
            InitializeComponent();
            _context = new LibraryContext();
            LoadChart();
        }

        private void LoadChart()
        {
            var genreData = _context.Genres
                .Select(g => new
                {
                    GenreName = g.Name,
                    BookCount = g.Books.Count()
                })
                .Where(x => x.BookCount > 0)
                .ToList();

            var seriesCollection = new SeriesCollection();

            foreach (var item in genreData)
            {
                seriesCollection.Add(new PieSeries
                {
                    Title = item.GenreName,
                    Values = new ChartValues<int> { item.BookCount },
                    DataLabels = true
                });
            }

            if (seriesCollection.Count == 0)
            {
                seriesCollection.Add(new PieSeries
                {
                    Title = "Нет данных",
                    Values = new ChartValues<int> { 1 },
                    DataLabels = true
                });
            }

            GenreChart.Series = seriesCollection;
        }
    }
}
