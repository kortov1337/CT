using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Client1.DBContext;
using Client1.Models;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights;

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
            TrackEvent("Create item", new Dictionary<string, string>
                {
                    { "Operation", "Title" },
                    { "Create", book.title }
                });
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
            TrackEvent("Update item", new Dictionary<string, string>
                {
                    { "Operation", "Title" },
                    { "Update", book.title }
                });
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
                TrackEvent("Delete item", new Dictionary<string, string>
                {
                    { "Operation", "Title" },
                    { "Delete", book.title }
                });
                //BusStuff.SendDelete("ServerUI", operation);
            }

        }

        public void TrackException(Exception ex)
        {
            ApplicationInsight.Instance.TrackException(ex);
        }

        public void TrackPageView(string name)
        {
            ApplicationInsight.Instance.TrackPageView(name);
        }

        public void TrackEvent(string eventName, Dictionary<string, string> options = null)
        {
            ApplicationInsight.Instance.TrackEvent(eventName, options);
        }
    }

    internal class ApplicationInsight
    {
        #region Fields 

        private const string Key = @"82ec0bae-4a4a-4a0c-b24e-99c099e4f10f";

        private static readonly Lazy<ApplicationInsight> _instance
            = new Lazy<ApplicationInsight>(() => new ApplicationInsight());

        private readonly TelemetryClient _tc;

        #endregion

        #region Constructors

        private ApplicationInsight()
        {
            _tc = InitializeTelemetry();
        }

        #endregion

        #region Properties

        public static ApplicationInsight Instance
        {
            get { return _instance.Value; }
        }

        #endregion

        #region Methods

        private TelemetryClient InitializeTelemetry()
        {
            var tc = new TelemetryClient
            {
                InstrumentationKey = Key
            };

            tc.Context.User.Id = Environment.UserName;
            tc.Context.Session.Id = Guid.NewGuid().ToString();
            tc.Context.Device.OperatingSystem = Environment.OSVersion.ToString();

            return tc;
        }

        public void TrackPageView(string name)
        {
            _tc.TrackPageView(name);
        }

        public void TrackException(Exception ex)
        {
            _tc.TrackException(ex);
        }

        public void TrackEvent(string eventName, Dictionary<string, string> options = null)
        {
            _tc.TrackEvent(eventName, options);
        }

        public void TrackMetric(string metricName, int value)
        {
            _tc.TrackMetric(metricName, value);
        }

        public void TrackTrace(string eventName, int number)
        {
            _tc.TrackTrace(
                "The number is even",
                SeverityLevel.Warning,
                new Dictionary<string, string> { { "number", number.ToString() } }
            );
        }

        public void Flush()
        {
            _tc.Flush();
        }

        #endregion
    }

}
