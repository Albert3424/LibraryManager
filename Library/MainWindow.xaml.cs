using Library.Data;
using Library.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Library
{
    public partial class MainWindow : Window
    {
        private LibraryContext _context;
        private enum ActiveTab { Books, Readers, Loans }
        private ActiveTab _currentTab = ActiveTab.Books;

        private List<Book> _allBooks = new List<Book>();
        private List<Reader> _allReaders = new List<Reader>();
        private List<BookLoan> _allLoans = new List<BookLoan>();
        private ICollectionView _booksView;
        private ICollectionView _readersView;
        private ICollectionView _loansView;

        public MainWindow()
        {
            InitializeComponent();
            _context = new LibraryContext();
            LoadData();
        }

        private void LoadData()
        {
            using (var context = new LibraryContext())
            {
                _allBooks = context.Books.Include(b => b.Author).Include(b => b.Genre).ToList();
                _allReaders = context.Readers.ToList();
                _allLoans = context.BookLoans.Include(l => l.Book).Include(l => l.Reader).ToList();

                _booksView = CollectionViewSource.GetDefaultView(_allBooks);
                _readersView = CollectionViewSource.GetDefaultView(_allReaders);
                _loansView = CollectionViewSource.GetDefaultView(_allLoans);

                BooksGrid.ItemsSource = _booksView;
                ReadersGrid.ItemsSource = _readersView;
                LoansGrid.ItemsSource = _loansView;
            }

            UpdateStatusBar();
        }

        private void UpdateStatusBar()
        {
            RecordCountText.Text = $"Книг: {BooksGrid.Items.Count} | Читателей: {ReadersGrid.Items.Count} | Выдач: {LoansGrid.Items.Count}";
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainTabControl.SelectedItem is TabItem tab)
            {
                _currentTab = tab.Name switch
                {
                    "TabBooks" => ActiveTab.Books,
                    "TabReaders" => ActiveTab.Readers,
                    "TabLoans" => ActiveTab.Loans,
                    _ => ActiveTab.Books
                };

                UpdateFilterItems();
                ApplyFilter();
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = SearchBox.Text.Trim().ToLower();
            ApplySearchAndFilter(searchText);
        }

        private void ClearSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "";
            FilterCombo.SelectedIndex = 0;
            ApplySearchAndFilter("");
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            bool? result = false;

            switch (_currentTab)
            {
                case ActiveTab.Books:
                    result = new Views.Books.AddBookWindow { Owner = this }.ShowDialog();
                    break;
                case ActiveTab.Readers:
                    result = new Views.Readers.AddReaderWindow { Owner = this }.ShowDialog();
                    break;
                case ActiveTab.Loans:
                    result = new Views.Loans.AddLoanWindow { Owner = this }.ShowDialog();
                    break;
            }

            if (result == true)
                LoadData();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            switch (_currentTab)
            {
                case ActiveTab.Books:
                    var book = BooksGrid.SelectedItem as Book;
                    if (book == null) { MessageBox.Show("Выберите книгу"); return; }
                    new Views.Books.EditBookWindow(book) { Owner = this }.ShowDialog();
                    LoadData();
                    break;

                case ActiveTab.Readers:
                    var reader = ReadersGrid.SelectedItem as Reader;
                    if (reader == null) { MessageBox.Show("Выберите читателя"); return; }
                    new Views.Readers.EditReaderWindow(reader) { Owner = this }.ShowDialog();
                    LoadData();
                    break;

                case ActiveTab.Loans:
                    var loan = LoansGrid.SelectedItem as BookLoan;
                    if (loan == null) { MessageBox.Show("Выберите запись о выдаче"); return; }
                    new Views.Loans.EditLoanWindow(loan) { Owner = this }.ShowDialog();
                    LoadData();
                    break;
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            switch (_currentTab)
            {
                case ActiveTab.Books:
                    var book = BooksGrid.SelectedItem as Book;
                    if (book == null) { MessageBox.Show("Выберите книгу"); return; }

                    using (var freshContext = new LibraryContext())
                    {
                        if (freshContext.BookLoans.Any(l => l.BookId == book.Id))
                        {
                            MessageBox.Show("Нельзя удалить книгу, которая выдана", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }

                    if (MessageBox.Show($"Удалить книгу '{book.Title}'?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        _context.Books.Remove(book);
                        _context.SaveChanges();
                        LoadData();
                    }
                    break;

                case ActiveTab.Readers:
                    var reader = ReadersGrid.SelectedItem as Reader;
                    if (reader == null) { MessageBox.Show("Выберите читателя"); return; }

                    using (var freshContext = new LibraryContext())
                    {
                        if (freshContext.BookLoans.Any(l => l.ReaderId == reader.Id))
                        {
                            MessageBox.Show("Нельзя удалить читателя, у которого есть выдачи", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }

                    if (MessageBox.Show($"Удалить читателя {reader.FirstName} {reader.LastName}?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        _context.Readers.Remove(reader);
                        _context.SaveChanges();
                        LoadData();
                    }
                    break;

                case ActiveTab.Loans:
                    var loan = LoansGrid.SelectedItem as BookLoan;
                    if (loan == null) { MessageBox.Show("Выберите запись о выдаче"); return; }

                    if (loan.ReturnDate == null)
                    {
                        MessageBox.Show("Нельзя удалить выдачу без даты возврата. Сначала верните книгу.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (MessageBox.Show($"Удалить выдачу книги '{loan.Book.Title}'?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        _context.BookLoans.Remove(loan);
                        _context.SaveChanges();
                        LoadData();
                    }
                    break;
            }
        }
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            _context.Dispose();
            _context = new LibraryContext();
            LoadData();
            UpdateFilterItems();
            ApplyFilter();
        }

        private void Reports_Click(object sender, RoutedEventArgs e)
        {
            new Views.ReportsWindow { Owner = this }.ShowDialog();
        }

        private void Exit_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();
        private void About_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Библиотека — курсовая работа");

        protected override void OnClosed(EventArgs e)
        {
            _context.Dispose();
            base.OnClosed(e);
        }

        private void UpdateFilterItems()
        {
            FilterCombo.Items.Clear();

            switch (_currentTab)
            {
                case ActiveTab.Books:
                    FilterCombo.Items.Add("Все книги");
                    FilterCombo.Items.Add("Только доступные");
                    FilterCombo.Items.Add("Только выданные");
                    break;

                case ActiveTab.Readers:
                    FilterCombo.Items.Add("Все читатели");
                    FilterCombo.Items.Add("С задолженностью");
                    FilterCombo.Items.Add("Без задолженности");
                    break;

                case ActiveTab.Loans:
                    FilterCombo.Items.Add("Все выдачи");
                    FilterCombo.Items.Add("Только активные");
                    FilterCombo.Items.Add("Только возвращённые");
                    break;
            }

            FilterCombo.SelectedIndex = 0;
        }

        private void ApplySearchAndFilter(string searchText)
        {
            var filter = FilterCombo.SelectedItem?.ToString();

            switch (_currentTab)
            {
                case ActiveTab.Books:
                    _booksView.Filter = item =>
                    {
                        var book = (Book)item;

                        bool matchFilter = true;
                        if (filter == "Только доступные") matchFilter = book.IsAvailable;
                        else if (filter == "Только выданные") matchFilter = !book.IsAvailable;

                        bool matchSearch = string.IsNullOrEmpty(searchText) ||
                                           book.Title.ToLower().Contains(searchText) ||
                                           book.Author.LastName.ToLower().Contains(searchText);

                        return matchFilter && matchSearch;
                    };
                    _booksView.Refresh();
                    break;

                case ActiveTab.Readers:
                    _readersView.Filter = item =>
                    {
                        var reader = (Reader)item;

                        bool matchFilter = true;
                        if (filter == "С задолженностью")
                            matchFilter = reader.BookLoans.Any(l => l.ReturnDate == null);
                        else if (filter == "Без задолженности")
                            matchFilter = !reader.BookLoans.Any(l => l.ReturnDate == null);

                        bool matchSearch = string.IsNullOrEmpty(searchText) ||
                                           reader.FirstName.ToLower().Contains(searchText) ||
                                           reader.LastName.ToLower().Contains(searchText) ||
                                           reader.Email.ToLower().Contains(searchText);

                        return matchFilter && matchSearch;
                    };
                    _readersView.Refresh();
                    break;

                case ActiveTab.Loans:
                    _loansView.Filter = item =>
                    {
                        var loan = (BookLoan)item;

                        bool matchFilter = true;
                        if (filter == "Только активные") matchFilter = loan.ReturnDate == null;
                        else if (filter == "Только возвращённые") matchFilter = loan.ReturnDate != null;

                        bool matchSearch = string.IsNullOrEmpty(searchText) ||
                                           loan.Book.Title.ToLower().Contains(searchText) ||
                                           loan.Reader.LastName.ToLower().Contains(searchText);

                        return matchFilter && matchSearch;
                    };
                    _loansView.Refresh();
                    break;
            }

            UpdateStatusBar();
        }

        private void ApplyFilter()
        {
            var searchText = SearchBox.Text.Trim().ToLower();
            ApplySearchAndFilter(searchText);
        }

        private void ResetFilters()
        {
            _booksView.Filter = null;
            _readersView.Filter = null;
            _loansView.Filter = null;

            _booksView.Refresh();
            _readersView.Refresh();
            _loansView.Refresh();

            UpdateStatusBar();
        }

        private void FilterCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }
    }
}