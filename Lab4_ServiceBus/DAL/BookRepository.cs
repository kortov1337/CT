using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Client1.DBContext;
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
          ///  BooksContext db = new BooksContext();
            db.Books.Add(book);
        }

        public void CreateDist(BooksModels book)
        {
            BusStuff.InitServerEndpoint("ServerUI").GetAwaiter().GetResult();
            Operation operation = new Operation(book, "Create");
            BusStuff.Send("ServerUI", operation);
        }

        public void Update(BooksModels book)
        {
            db.Entry(book).State = EntityState.Modified;
        }

        public void UpdateDist(BooksModels book)
        {
            BusStuff.InitServerEndpoint("ServerUI").GetAwaiter().GetResult();
            Operation operation = new Operation(book, "Update");
            BusStuff.Send("ServerUI", operation);
        }

        public void Delete(int? id)
        {
            BooksModels book = db.Books.Find(id);
            if (book != null)
                db.Books.Remove(book);
        }

        public void DeleteDist(int? id)
        {
            BooksModels book = db.Books.Find(id);
            if (book != null)
            { 
                BusStuff.InitServerEndpoint("ServerUI").GetAwaiter().GetResult();
                Operation operation = new Operation(book, "Delete");
                BusStuff.Send("ServerUI", operation);
                //BusStuff.SendDelete("ServerUI", operation);
            }

        }
    }
}
