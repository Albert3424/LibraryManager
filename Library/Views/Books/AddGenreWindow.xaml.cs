using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Library.Data;
using Library.Models;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Library.Views.Books
{
    public partial class AddGenreWindow : Window
    {
        private LibraryContext _context;

        public AddGenreWindow()
        {
            InitializeComponent();
            _context = new LibraryContext();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                ErrorText.Text = "Название обязательно";
                return;
            }

            var genre = new Genre
            {
                Name = NameBox.Text.Trim(),
                Description = DescriptionBox.Text.Trim()
            };

            _context.Genres.Add(genre);
            _context.SaveChanges();

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
