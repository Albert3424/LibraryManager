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
    public partial class EditBookWindow : Window
    {
        private LibraryContext _context;
        private Book _book;

        public EditBookWindow(Book book)
        {
            InitializeComponent();
            _context = new LibraryContext();
            _book = book;

            LoadComboBoxes();
            LoadBookData();
        }

        private void LoadComboBoxes()
        {
            AuthorCombo.ItemsSource = _context.Authors.ToList();
            GenreCombo.ItemsSource = _context.Genres.ToList();

            AuthorCombo.SelectedItem = _context.Authors.Find(_book.AuthorId);
            GenreCombo.SelectedItem = _context.Genres.Find(_book.GenreId);
        }

        private void LoadBookData()
        {
            TitleBox.Text = _book.Title;
            YearBox.Text = _book.Year?.ToString();
            AvailableCheck.IsChecked = _book.IsAvailable;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleBox.Text))
            {
                ErrorText.Text = "Название обязательно";
                return;
            }

            if (AuthorCombo.SelectedItem == null || GenreCombo.SelectedItem == null)
            {
                ErrorText.Text = "Выберите автора и жанр";
                return;
            }

            if (!int.TryParse(YearBox.Text, out int year) && !string.IsNullOrWhiteSpace(YearBox.Text))
            {
                ErrorText.Text = "Год должен быть числом";
                return;
            }

            var bookFromDb = _context.Books.Find(_book.Id);
            if (bookFromDb == null)
            {
                ErrorText.Text = "Книга не найдена";
                return;
            }

            bookFromDb.Title = TitleBox.Text.Trim();
            bookFromDb.AuthorId = ((Author)AuthorCombo.SelectedItem).Id;
            bookFromDb.GenreId = ((Genre)GenreCombo.SelectedItem).Id;
            bookFromDb.Year = string.IsNullOrWhiteSpace(YearBox.Text) ? null : year;
            bookFromDb.IsAvailable = AvailableCheck.IsChecked ?? true;

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
