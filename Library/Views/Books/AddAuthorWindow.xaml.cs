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
    public partial class AddAuthorWindow : Window
    {
        private LibraryContext _context;

        public AddAuthorWindow()
        {
            InitializeComponent();
            _context = new LibraryContext();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FirstNameBox.Text) || string.IsNullOrWhiteSpace(LastNameBox.Text))
            {
                ErrorText.Text = "Имя и фамилия обязательны";
                return;
            }

            var author = new Author
            {
                FirstName = FirstNameBox.Text.Trim(),
                LastName = LastNameBox.Text.Trim(),
                Country = CountryBox.Text.Trim()
            };

            _context.Authors.Add(author);
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
