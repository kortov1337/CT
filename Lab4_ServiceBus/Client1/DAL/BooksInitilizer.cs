using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Client1.Models;

namespace Client1.DAL
{
    public class BooksInitilizer
    {
        public class BooksInitializer : DropCreateDatabaseIfModelChanges<BooksContext>
        {
            protected override void Seed(BooksContext context)
            {
                var books = new List<BooksModels>
                {
                    new BooksModels {title="dsadsa",author="dfggwe",branchOfScience="qwetwsac",binding="sadaq",numberOfPage=213,publishingHouse="cCsqwds" },
                    new BooksModels {title="dssa",author="dwe",branchOfScience="qwsac",binding="saq",numberOfPage=13,publishingHouse="casdqad" }
                };
                books.ForEach(s => context.Books.Add(s));
                context.SaveChanges();
            }
        }
    }
}