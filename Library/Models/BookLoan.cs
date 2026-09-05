using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models
{
    public class BookLoan
    {
        public int Id { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public int BookId { get; set; }
        public int ReaderId { get; set; }

        public Book Book { get; set; }
        public Reader Reader { get; set; }
    }
}
