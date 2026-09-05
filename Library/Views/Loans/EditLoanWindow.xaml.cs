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
    public partial class EditLoanWindow : Window
    {
        private LibraryContext _context;
        private BookLoan _loan;

        public EditLoanWindow(BookLoan loan)
        {
            InitializeComponent();
            _context = new LibraryContext();
            _loan = loan;

            BookTitle.Text = _loan.Book?.Title ?? "Неизвестно";
            ReaderName.Text = _loan.Reader != null ? $"{_loan.Reader.FirstName} {_loan.Reader.LastName}" : "Неизвестно";
            ReturnDatePicker.SelectedDate = DateTime.Today;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (ReturnDatePicker.SelectedDate == null)
            {
                ErrorText.Text = "Выберите дату возврата";
                return;
            }

            try
            {
                var book = _context.Books.Find(_loan.BookId);
                if (book != null)
                {
                    book.IsAvailable = true;
                }

                var loanFromDb = _context.BookLoans.Find(_loan.Id);
                if (loanFromDb == null)
                {
                    ErrorText.Text = "Запись о выдаче не найдена";
                    return;
                }

                loanFromDb.ReturnDate = ReturnDatePicker.SelectedDate.Value;
                _context.SaveChanges();

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"Ошибка: {ex.Message}";
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
