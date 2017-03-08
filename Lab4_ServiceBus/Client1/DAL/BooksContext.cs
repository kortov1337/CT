using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Client1.Models;
namespace Client1.DAL
{
    public class BooksContext:DbContext
    {
        public DbSet<BooksModels> Books { get; set; }
    }

}