using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CMHAVoucherFinder.Models;

namespace CMHAVoucherFinder.Controllers
{
    public class PropertyImagesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: PropertyImages
        public ActionResult Index()
        {
            return View(db.PropertyImages.ToList());
        }

        // GET: PropertyImages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PropertyImage propertyImage = db.PropertyImages.Find(id);
            if (propertyImage == null)
            {
                return HttpNotFound();
            }
            return View(propertyImage);
        }

        // GET: PropertyImages/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PropertyImages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PropertyImageId,Name,FilePath")] PropertyImage propertyImage)
        {
            if (ModelState.IsValid)
            {
                db.PropertyImages.Add(propertyImage);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(propertyImage);
        }

        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (file != null)
            {
                
                string pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(
                                       Server.MapPath("~/images/"), pic);
                // file is uploaded
                file.SaveAs(path);

                // save the image path path to the database or you can send image 
                // directly to database
                // in-case if you want to store byte[] ie. for DB
                using (MemoryStream ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    byte[] array = ms.GetBuffer();
                }

            }
            // after successfully uploading redirect the user
            return RedirectToAction("Index", "PropertyImages");
        }

        // GET: PropertyImages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PropertyImage propertyImage = db.PropertyImages.Find(id);
            if (propertyImage == null)
            {
                return HttpNotFound();
            }
            return View(propertyImage);
        }

        // POST: PropertyImages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PropertyImageId,Name,FilePath")] PropertyImage propertyImage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(propertyImage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(propertyImage);
        }

        // GET: PropertyImages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PropertyImage propertyImage = db.PropertyImages.Find(id);
            if (propertyImage == null)
            {
                return HttpNotFound();
            }
            return View(propertyImage);
        }

        // POST: PropertyImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PropertyImage propertyImage = db.PropertyImages.Find(id);
            db.PropertyImages.Remove(propertyImage);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
