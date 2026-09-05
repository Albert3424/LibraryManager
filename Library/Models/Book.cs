using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Library.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int? Year { get; set; }
        public bool IsAvailable { get; set; } = true;
        public int AuthorId { get; set; }
        public int GenreId { get; set; }
        public Author Author { get; set; }
        public Genre Genre { get; set; }

        public ICollection<BookLoan> BookLoans { get; set; } = new List<BookLoan>();
    }
}
