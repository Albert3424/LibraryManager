using Library.Data;
using Library.Models;
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

namespace Library.Views.Loans
{
    public partial class AddLoanWindow : Window
    {
        private LibraryContext _context;

        public AddLoanWindow()
        {
            InitializeComponent();
            _context = new LibraryContext();
            LoadComboBoxes();
            LoanDatePicker.SelectedDate = DateTime.Today;
        }

        private void LoadComboBoxes()
        {
            BookCombo.ItemsSource = _context.Books.Where(b => b.IsAvailable).ToList();
            ReaderCombo.ItemsSource = _context.Readers.ToList();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (BookCombo.SelectedItem == null)
            {
                ErrorText.Text = "Выберите книгу";
                return;
            }

            if (ReaderCombo.SelectedItem == null)
            {
                ErrorText.Text = "Выберите читателя";
                return;
            }

            if (LoanDatePicker.SelectedDate == null)
            {
                ErrorText.Text = "Выберите дату выдачи";
                return;
            }

            var loan = new BookLoan
            {
                BookId = ((Book)BookCombo.SelectedItem).Id,
                ReaderId = ((Reader)ReaderCombo.SelectedItem).Id,
                LoanDate = LoanDatePicker.SelectedDate.Value
            };

            _context.BookLoans.Add(loan);
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
