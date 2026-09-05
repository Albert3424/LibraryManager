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
    public partial class EditReaderWindow : Window
    {
        private LibraryContext _context;
        private Reader _reader;

        public EditReaderWindow(Reader reader)
        {
            InitializeComponent();
            _context = new LibraryContext();
            _reader = reader;

            FirstNameBox.Text = _reader.FirstName;
            LastNameBox.Text = _reader.LastName;
            EmailBox.Text = _reader.Email;
            PhoneBox.Text = _reader.Phone;
            AddressBox.Text = _reader.Address;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FirstNameBox.Text) ||
                string.IsNullOrWhiteSpace(LastNameBox.Text))
            {
                ErrorText.Text = "Имя и фамилия обязательны";
                return;
            }

            _context.Attach(_reader);
            _context.Entry(_reader).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            _reader.FirstName = FirstNameBox.Text.Trim();
            _reader.LastName = LastNameBox.Text.Trim();
            _reader.Email = EmailBox.Text.Trim();
            _reader.Phone = PhoneBox.Text.Trim();
            _reader.Address = AddressBox.Text.Trim();

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
