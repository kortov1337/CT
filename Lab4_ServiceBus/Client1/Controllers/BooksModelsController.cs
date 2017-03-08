using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DAL;
using Client1.Models;

namespace Client1.Controllers
{
    public class BooksModelsController : Controller
    {
       // private BooksContext db = new BooksContext();
        UnitOfWork unitOfWork = new UnitOfWork();
        // GET: BooksModels
        public BooksModelsController()
        {
           // unitOfWork = new UnitOfWork();
        }
        public ActionResult Index()
        {
            
            var books = unitOfWork.Books.GetAll();
            return View(books);
        }

        // GET: BooksModels/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BooksModels booksModels =
                unitOfWork.Books.Get(id);// db.Books.Find(id);
            if (booksModels == null)
            {
                return HttpNotFound();
            }
            return View(booksModels);
        }

        // GET: BooksModels/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BooksModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,title,author,branchOfScience,numberOfPage,publishingHouse,binding")] BooksModels booksModels)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Books.CreateDist(booksModels);
                //unitOfWork.Books.Create(booksModels);
                // db.Books.Add(booksModels);
                //db.SaveChanges();
              //  unitOfWork.Save();
                return RedirectToAction("Index");
            }

            return View(booksModels);
        }

        // GET: BooksModels/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BooksModels booksModels = unitOfWork.Books.Get(id);//db.Books.Find(id);
            if (booksModels == null)
            {
                return HttpNotFound();
            }
            return View(booksModels);
        }

        // POST: BooksModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,title,author,branchOfScience,numberOfPage,publishingHouse,binding")] BooksModels booksModels)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Books.Update(booksModels);
                unitOfWork.Save();
                /*db.Entry(booksModels).State = EntityState.Modified;
                db.SaveChanges();*/
                return RedirectToAction("Index");
            }
            return View(booksModels);
        }

        // GET: BooksModels/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BooksModels booksModels = unitOfWork.Books.Get(id);
            if (booksModels == null)
            {
                return HttpNotFound();
            }
            return View(booksModels);
        }

        // POST: BooksModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //BooksModels booksModels = unitOfWork.Books.Get(id);
            unitOfWork.Books.Delete(id);
            unitOfWork.Save();
            //db.Books.Remove(booksModels);
           // db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                unitOfWork.Dispose();
                //db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
