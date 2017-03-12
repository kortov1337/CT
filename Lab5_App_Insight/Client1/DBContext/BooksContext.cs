using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Client1.Models;
namespace Client1.DBContext
{
    public class BooksContext:DbContext
    {
       /* public BooksContext():base(@"Server=tcp:bstu1.database.windows.net,1433;InitialCatalog=bstudb1;
         PersistSecurityInfo=False;UserID=bstu;Password=Test4067;MultipleActiveResultSets=False;Encrypt=True;
         TrustServerCertificate=False;ConnectionTimeout=30;")
        {

        }*/
        public DbSet<BooksModels> Books { get; set; }
    }

}