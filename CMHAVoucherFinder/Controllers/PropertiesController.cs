using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using CMHAVoucherFinder.Models;
using Microsoft.AspNet.Identity;
using PagedList;


namespace CMHAVoucherFinder
{
    public class PropertiesController : Controller
    {
        private double distance(double lat1, double lon1, double lat2, double lon2, char unit)
        {
            double theta = lon1 - lon2;
            double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) +
                          Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = dist * 60 * 1.1515;
            if (unit == 'K')
                dist = dist * 1.609344;
            else if (unit == 'N')
                dist = dist * 0.8684;
            return (dist);

        }

        private double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }

        private double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }


        protected UserManager<ApplicationUser> UserManager { get; set; }

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Properties
        [Authorize]
        public async Task<ViewResult> Index()
        {
            string userId = User.Identity.GetUserId();
            ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == userId);
            if (currentUser.UserType == UserTypes.Landlord)
            {
                return View(await db.Properties.Where(u => u.UserId == userId).ToListAsync());
            }
            return View("Browse", db.Properties.ToPagedList(1, 10));
        }


        public async Task<ActionResult> BrowseBy(string zipcode, string currentfilter, int maxdistance, string accessible, int minrent,
            int maxrent, string propertytype, string sortorder, int? page)
        {
            ViewBag.CurrentSort = sortorder;
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortorder) ? "price_desc" : "";
            ViewBag.DateSortParm = sortorder == "Date" ? "date_desc" : "Date";

            if (zipcode != null)
            {
                page = 1;
            }
            else
            {
                zipcode = currentfilter;
            }

            ViewBag.CurrentFilter = zipcode;


            int zipint;
            if (!int.TryParse(zipcode, out zipint))
            {
                TempData["shortMessage"] = "Invalid Zip Code Entered. Please Try Again.";
                
                return RedirectToAction("Index", "Home");
            }
            if (zipint > 99999 || zipint < 10000)
            {
                TempData["shortMessage"] = "Invalid Zip Code Entered. Please Try Again.";
                
                return RedirectToAction("Index", "Home");
            }
                

            var requestUri = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false",
                Uri.EscapeDataString(zipcode));

            var request = WebRequest.Create(requestUri);
            var response = request.GetResponse();
            var xdoc = XDocument.Load(response.GetResponseStream());

            if (xdoc.Element("GeocodeResponse").Element("status").ToString() == "ZERO_RESULTS")
            {
                return RedirectToAction("Index", "Home");
            }


            var result = xdoc.Element("GeocodeResponse").Element("result");
            var locationElement = result.Element("geometry").Element("location");
            var lat1 = (double) locationElement.Element("lat");
            var long1 = (double) locationElement.Element("lng");

            var property = new Property();
            if (accessible == "Yes")
                property.IsPropertyAccessible = YesNo.Yes;
            else if (accessible == "No")
                property.IsPropertyAccessible = YesNo.No;
            else
                property.IsPropertyAccessible = YesNo.Unspecified;


            if (propertytype == "Any Type")
                property.PropertyType = PropertyTypes.Unspecified;
            else if (propertytype == "House")
                property.PropertyType = PropertyTypes.House;
            else if (propertytype == "Apartment")
                property.PropertyType = PropertyTypes.Apartment;
            else if (propertytype == "Townhouse")
                property.PropertyType = PropertyTypes.Townhouse;
            else if (propertytype == "Duplex")
                property.PropertyType = PropertyTypes.Duplex;

            foreach (var prop in db.Properties)
            {
                prop.DistanceFromSearch = distance(lat1, long1, prop.Latitude, prop.Longitude, 'M');
                db.Entry(prop).State = EntityState.Modified;

            }
            await db.SaveChangesAsync();

            var propertylist = new List<Property>();
            if (property.PropertyType != PropertyTypes.Unspecified && property.IsPropertyAccessible != YesNo.Unspecified)
                propertylist = 
                    new List<Property>(db.Properties.Where(u => u.DistanceFromSearch <= maxdistance)
                        .Where(u => u.Price >= minrent)
                        .Where(u => u.Price <= maxrent)
                        .Where(u => u.PropertyType == property.PropertyType)
                        .Where(u => u.IsPropertyAccessible == property.IsPropertyAccessible)
                        .OrderBy(u => u.DistanceFromSearch));
            else if (property.IsPropertyAccessible != YesNo.Unspecified)
                propertylist = 
                    new List<Property>(db.Properties.Where(u => u.DistanceFromSearch <= maxdistance)
                        .Where(u => u.Price >= minrent)
                        .Where(u => u.Price <= maxrent)
                        .Where(u => u.IsPropertyAccessible == property.IsPropertyAccessible)
                        .OrderBy(u => u.DistanceFromSearch));
            else if (property.PropertyType != PropertyTypes.Unspecified)
                propertylist = 
                    new List<Property>(db.Properties.Where(u => u.DistanceFromSearch <= maxdistance)
                        .Where(u => u.Price >= minrent)
                        .Where(u => u.Price <= maxrent)
                        .Where(u => u.PropertyType == property.PropertyType)
                        .OrderBy(u => u.DistanceFromSearch));
            else
            propertylist = 
                    new List<Property>(db.Properties.Where(u => u.DistanceFromSearch <= maxdistance)
                        .Where(u => u.Price >= minrent)
                        .Where(u => u.Price <= maxrent)
                        .OrderBy(u => u.DistanceFromSearch));

            switch (sortorder)
            {
                case "price_desc":
                    propertylist = new List<Property>(propertylist.OrderByDescending(s => s.Price));
                    break;
                case "Date":
                    propertylist = new List<Property>(propertylist.OrderBy(s => s.DateAvailable));
                    break;
                case "date_desc":
                    propertylist = new List<Property>(propertylist.OrderByDescending(s => s.DateAvailable));
                    break;
                default:
                    propertylist = new List<Property>(propertylist.OrderBy(s => s.Price));
                    break;
            }

            ViewData["zipcode"] = zipcode;
            ViewData["minrent"] = minrent;
            ViewData["maxrent"] = maxrent;
            ViewData["isaccessible"] = accessible;
            ViewData["propertytype"] = propertytype;

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            
            return View("Browse", propertylist.ToPagedList(pageNumber, pageSize));
        }



// GET: Properties/Details/5
        [Authorize]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Property property = await db.Properties.FindAsync(id);
            if (property == null)
            {
                return HttpNotFound();
            }

            if (property.UserId == User.Identity.GetUserId())
                return View(property);
            return View("ViewProperty", property);
        }

        // GET: Properties/Create
        [Authorize]
        public ActionResult Create()
        {
            string userId = User.Identity.GetUserId();
            ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == userId);
            if (currentUser.UserType == UserTypes.Landlord)
                return View();
            return View("Browse", db.Properties.ToPagedList(1, 10));
        }

        // POST: Properties/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
        [Bind(
            Include =
                "PropertyId,IsPropertyAccessible,PropertyName,StreetAddress,City,State,ZipCode,DateAvailable,Price,User,PropertyDescription,UserId,CeilingFans,Furnished,Fireplace,CablePaid,HeatStyle,DishWasher,Stove,GarbageDisposal,Refrigerator,Microwave,SwimmingPool,GatedCommunity,LawnCareIncluded,ParkingType,FencedYard,PatioPorch,IsSmokingAllowed,LotSize,PestControl,TenantPaysElectric,TenantPaysWater,TenantPaysSewer,TenantPaysHeat,ElectricStatus,WaterStatus,SewerStatus,HeatStatus,IsParkingClose,NoStepEntry,RampedEntry,Doorway32OrWider,AccessiblePathInHome,AutomaticEntryDoor,LeverStyleDoorHandles,SingleLevelOrFirstFloow,InsideSteps,OutsideSteps,Latitude,Longitude,UserId"
        )] Property property)
        {
            if (ModelState.IsValid)
            {
                //Get Lat and long coordinates. If no lat and long found for address, return error.
                var address = property.StreetAddress + " " + property.City + ", " + property.State + " " +
                              property.ZipCode;
                var requestUri =
                    string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false",
                        Uri.EscapeDataString(address));

                var request = WebRequest.Create(requestUri);
                var response = request.GetResponse();
                var xdoc = XDocument.Load(response.GetResponseStream());

                if (xdoc.Element("GeocodeResponse").Element("status").ToString() == "ZERO_RESULTS")
                {
                    return RedirectToAction("AddressNotFound", "Properties");
                }


                var result = xdoc.Element("GeocodeResponse").Element("result");
                var locationElement = result.Element("geometry").Element("location");
                property.Latitude = (double) locationElement.Element("lat");
                property.Longitude = (double) locationElement.Element("lng");

                property.UserId = User.Identity.GetUserId();
                db.Properties.Add(property);
                await db.SaveChangesAsync();
                return RedirectToAction("AdditionalDetails", new {id = property.PropertyId});
            }

            return View(property);
        }

        // GET: Properties/Edit/5
        [Authorize]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Property property = await db.Properties.FindAsync(id);
            if (property == null)
            {
                return HttpNotFound();
            }
            if (property.UserId == User.Identity.GetUserId())
                return View(property);
            return View("Index");
        }

        // POST: Properties/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(
        [Bind(
            Include =
                "PropertyId,IsPropertyAccessible,PropertyName,StreetAddress,City,State,ZipCode,DateAvailable,Price,User,PropertyDescription,UserId,CeilingFans,Furnished,Fireplace,CablePaid,HeatStyle,DishWasher,Stove,GarbageDisposal,Refrigerator,Microwave,SwimmingPool,GatedCommunity,LawnCareIncluded,ParkingType,FencedYard,PatioPorch,IsSmokingAllowed,LotSize,PestControl,TenantPaysElectric,TenantPaysWater,TenantPaysSewer,TenantPaysHeat,ElectricStatus,WaterStatus,SewerStatus,HeatStatus,IsParkingClose,NoStepEntry,RampedEntry,Doorway32OrWider,AccessiblePathInHome,AutomaticEntryDoor,LeverStyleDoorHandles,SingleLevelOrFirstFloow,InsideSteps,OutsideSteps,Latitude,Longitude,Beds,Baths,UserId"
        )] Property property)
        {
            if (ModelState.IsValid)
            {
                db.Entry(property).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(property);
        }

        // GET: Properties/Delete/5
        [Authorize]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Property property = await db.Properties.FindAsync(id);
            if (property == null)
            {
                return HttpNotFound();
            }
            if (property.UserId == User.Identity.GetUserId())
                return View(property);
            return View("Index");
        }

        // POST: Properties/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Property property = await db.Properties.FindAsync(id);
            db.Properties.Remove(property);
            await db.SaveChangesAsync();
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

        public ActionResult UploadImageFile(HttpPostedFileBase file, int PropertyId)
        {
            if (file != null)
            {
                int id = PropertyId;
                Property property = db.Properties.SingleOrDefault(u => u.PropertyId == id);
                string pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(
                    Server.MapPath("~/images/"), pic);
                Image image = Image.FromFile(path);
                Image thumb = image.GetThumbnailImage(120, 120, () => false, IntPtr.Zero);
                thumb.Save(Path.ChangeExtension(path, "thumb"));
                // file is uploaded
                file.SaveAs(path);

                PropertyImage propertyImage = new PropertyImage();
                propertyImage.Property = property;
                propertyImage.FilePath = path;
                propertyImage.ThumbFilePath = path + "thumb";


                db.PropertyImages.Add(propertyImage);
                db.SaveChanges();

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

            return RedirectToAction("Index", "Properties");
        }

        public async Task<ActionResult> UploadImage(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Property property = await db.Properties.FindAsync(id);
            if (property == null)
            {
                return HttpNotFound();
            }
            if (property.UserId == User.Identity.GetUserId())
                return View(property);
            return View("Index");
        }

        public async Task<ActionResult> Browse(int? page)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var properties = await db.Properties.ToListAsync();
            return View(properties.ToPagedList(pageNumber, pageSize));
        }

        [Authorize]
        public async Task<ActionResult> AddToFavorites(int id)
        {
            FavoriteProperty favorite = new FavoriteProperty
            {
                PropertyId = id,
                ApplicationUserId = User.Identity.GetUserId()
            };
            foreach (var prop in db.FavoriteProperties)
            {
                if (prop.PropertyId == favorite.PropertyId && prop.ApplicationUserId == favorite.ApplicationUserId)
                {
                    return RedirectToAction("Browse");
                }
            }
            db.FavoriteProperties.Add(favorite);
            await db.SaveChangesAsync();
            return RedirectToAction("Browse");
        }

        public bool IsInFavorites(int id)
        {
            FavoriteProperty favorite = new FavoriteProperty
            {
                PropertyId = id,
                ApplicationUserId = User.Identity.GetUserId()
            };
            foreach (var prop in db.FavoriteProperties)
            {
                if (prop.PropertyId == favorite.PropertyId && prop.ApplicationUserId == favorite.ApplicationUserId)
                {
                    return true;
                }
            }
            return false;
        }

        public ActionResult ViewFavorites()
        {
            string userId = User.Identity.GetUserId();
            List<Property> favPropList = new List<Property>();
            foreach (var fav in db.FavoriteProperties)
            {
                if (fav.ApplicationUserId == userId)
                {
                    if (!favPropList.Contains(db.Properties.SingleOrDefault(u => u.PropertyId == fav.PropertyId)))
                        favPropList.Add(db.Properties.SingleOrDefault(u => u.PropertyId == fav.PropertyId));
                }
            }
            return View(favPropList);
        }

        public async Task<ActionResult> ViewProperty(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Property property = await db.Properties.FindAsync(id);
            if (property == null)
            {
                return HttpNotFound();
            }
            return View("ViewProperty", "~/Views/Shared/_LayoutSlider.cshtml", property);
        }

        public ActionResult RemoveFromFavorites(int id)
        {
            string userId = User.Identity.GetUserId();
            foreach (var prop in db.FavoriteProperties)
            {
                if (prop.PropertyId == id && prop.ApplicationUserId == userId)
                {
                    db.FavoriteProperties.Remove(prop);

                }

            }
            db.SaveChanges();

            //List<Property> favPropList = new List<Property>();
            //foreach (var fav in db.FavoriteProperties)
            //{
            //    if (fav.ApplicationUserId == userId)
            //    {
            //        if (!favPropList.Contains(db.Properties.SingleOrDefault(u => u.PropertyId == fav.PropertyId)))
            //            favPropList.Add(db.Properties.SingleOrDefault(u => u.PropertyId == fav.PropertyId));
            //    }
            //}

            return View("ViewFavorites");
        }

        public ActionResult AdditionalDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Property property = db.Properties.Find(id);
            if (property == null)
            {
                return HttpNotFound();
            }
            var userId = property.UserId;
            ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == userId);
            if (property.UserId == User.Identity.GetUserId() || currentUser.UserType != UserTypes.Landlord)
                return View(property);
            return View("ViewProperty", property);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AdditionalDetails(
        [Bind(
            Include =
                "PropertyId,IsPropertyAccessible,PropertyName,StreetAddress,City,State,ZipCode,DateAvailable,Price,User,PropertyDescription,UserId,CeilingFans,Furnished,Fireplace,CablePaid,HeatStyle,DishWasher,Stove,GarbageDisposal,Refrigerator,Microwave,SwimmingPool,GatedCommunity,LawnCareIncluded,ParkingType,FencedYard,PatioPorch,IsSmokingAllowed,LotSize,PestControl,TenantPaysElectric,TenantPaysWater,TenantPaysSewer,TenantPaysHeat,ElectricStatus,WaterStatus,SewerStatus,HeatStatus,IsParkingClose,NoStepEntry,RampedEntry,Doorway32OrWider,AccessiblePathInHome,AutomaticEntryDoor,LeverStyleDoorHandles,SingleLevelOrFirstFloow,InsideSteps,OutsideSteps,Latitude,Longitude,Beds,Baths,UserId"
        )] Property property)
        {
            if (ModelState.IsValid)
            {
                db.Entry(property).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Details", new {id = property.PropertyId});
            }
            return RedirectToAction("ViewProperty", new {id = property.PropertyId});
        }

        public ActionResult MapsTest()
        {
            return View("MapsTest");
        }

        public ActionResult AddressNotFound()
        {
            return View();
        }
    }
}

