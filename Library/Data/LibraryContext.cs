using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Library.Models;

namespace Library.Data
{
    internal class LibraryContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Reader> Readers { get; set; }
        public DbSet<BookLoan> BookLoans { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=LibraryDb;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BookLoan>()
                .HasOne(bl => bl.Book)
                .WithMany(b => b.BookLoans)
                .HasForeignKey(bl => bl.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BookLoan>()
                .HasOne(bl => bl.Reader)
                .WithMany(r => r.BookLoans)
                .HasForeignKey(bl => bl.ReaderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
