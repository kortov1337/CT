using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Client1.DAL;
using Client1.Models;

namespace DAL
{
    public class BookRepository : IRepository<BooksModels>
    {
        private BooksContext db;

        public BookRepository(BooksContext context)
        {
            this.db = context;
        }

        public IEnumerable<BooksModels> GetAll()
        {
            return db.Books;
        }

        public BooksModels Get(int? id)
        {
            return db.Books.Find(id);
        }

        public void Create(BooksModels book)
        {
            db.Books.Add(book);
        }

        public void CreateDist(BooksModels book)
        {
            BusUsage.InitServerEndpoint("ServerUI");
            Operation operation = new Operation(book, "Create",-1);
            BusUsage.SendCreate("ServerUI", operation).GetAwaiter().GetResult();
        }

        public void Update(BooksModels book)
        {
            db.Entry(book).State = EntityState.Modified;
        }

        public void Delete(int? id)
        {
            BooksModels book = db.Books.Find(id);
            if (book != null)
                db.Books.Remove(book);
        }
    }
}
