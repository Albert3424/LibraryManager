using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Library.Models
{
    public class Reader
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public ICollection<BookLoan> BookLoans { get; set; } = new List<BookLoan>();
    }
}
