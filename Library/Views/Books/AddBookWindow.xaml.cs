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
    public partial class AddBookWindow : Window
    {
        private LibraryContext _context;

        public AddBookWindow()
        {
            InitializeComponent();
            _context = new LibraryContext();
            LoadComboBoxes();
        }

        private void LoadComboBoxes()
        {
            AuthorCombo.ItemsSource = _context.Authors.ToList();
            GenreCombo.ItemsSource = _context.Genres.ToList();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleBox.Text))
            {
                ErrorText.Text = "Название обязательно";
                return;
            }

            if (AuthorCombo.SelectedItem == null)
            {
                ErrorText.Text = "Выберите автора";
                return;
            }

            if (GenreCombo.SelectedItem == null)
            {
                ErrorText.Text = "Выберите жанр";
                return;
            }

            if (!int.TryParse(YearBox.Text, out int year) && !string.IsNullOrWhiteSpace(YearBox.Text))
            {
                ErrorText.Text = "Год должен быть числом";
                return;
            }

            var book = new Book
            {
                Title = TitleBox.Text.Trim(),
                AuthorId = ((Author)AuthorCombo.SelectedItem).Id,
                GenreId = ((Genre)GenreCombo.SelectedItem).Id,
                Year = string.IsNullOrWhiteSpace(YearBox.Text) ? null : year,
                IsAvailable = AvailableCheck.IsChecked ?? true
            };

            _context.Books.Add(book);
            _context.SaveChanges();

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void AddAuthor_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddAuthorWindow();
            window.Owner = this;
            if (window.ShowDialog() == true)
            {
                LoadComboBoxes();
            }
        }

        private void AddGenre_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddGenreWindow();
            window.Owner = this;
            if (window.ShowDialog() == true)
            {
                LoadComboBoxes();
            }
        }
    }
}
