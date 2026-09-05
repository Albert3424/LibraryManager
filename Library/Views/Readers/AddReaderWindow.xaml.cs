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

namespace Library.Views.Readers
{
    public partial class AddReaderWindow : Window
    {
        private LibraryContext _context;

        public AddReaderWindow()
        {
            InitializeComponent();
            _context = new LibraryContext();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FirstNameBox.Text) ||
                string.IsNullOrWhiteSpace(LastNameBox.Text))
            {
                ErrorText.Text = "Имя и фамилия обязательны";
                return;
            }

            var reader = new Reader
            {
                FirstName = FirstNameBox.Text.Trim(),
                LastName = LastNameBox.Text.Trim(),
                Email = EmailBox.Text.Trim(),
                Phone = PhoneBox.Text.Trim(),
                Address = AddressBox.Text.Trim()
            };

            _context.Readers.Add(reader);
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
