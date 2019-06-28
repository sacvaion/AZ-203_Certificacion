using ConsoleApp1.Negocio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.AccesoDatos
{
    public class BookContext:DbContext
    {
        public BookContext(string ConString) : base(ConString)
        {

        }
        public DbSet<Book> Books { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
                .ToTable("Books")
                .HasKey(c => c.BookId);
        }
    }
}
