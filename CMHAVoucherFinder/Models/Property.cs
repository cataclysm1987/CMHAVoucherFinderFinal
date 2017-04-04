using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using CMHAVoucherFinder.Models;
using static Microsoft.AspNet.Identity.IdentityExtensions;

namespace CMHAVoucherFinder.Models
{
    public enum PropertyTypes { Unspecified, Apartment, House, Duplex, Townhouse}

    public enum HeatStyles { Unspecified, Boiler, Gas, Electric }

    public enum ParkingTypes { Unspecified, Driveway, Lot, None, Other }

    public enum PatioPorches { Unspecified, Patio, Porch, None }

    public enum YesNo { Unspecified, Yes, No }


    public class Property : IEnumerable<PropertyImage>
    {
        public int PropertyId { get; set; }

        [Required]
        [Display(Name = "Property Name")]
        public string PropertyName { get; set; }
        [Required]
        [Display(Name = "Street Address")]
        public string StreetAddress { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        [Display(Name = "Zip Code")]
        public int ZipCode { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Available")]
        public DateTime? DateAvailable { get; set; }
        [Required]
        [Display(Name = "Monthly Rent")]
        public double Price { get; set; }
        [Required]
        [Display(Name = "Deposit")]
        public double Deposit { get; set; }
        [Required]
        [Display(Name = "Beds")]
        public int Beds { get; set; }
        [Required]
        [Display(Name = "Baths")]
        public int Baths { get; set; }
        [Display(Name = "Square Feet")]
        public int SquareFeet { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Year Built")]
        public int YearBuilt { get; set; }
        [Required]
        [Display(Name = "Property Description")]
        public string PropertyDescription { get; set; }

        //Select list items for beds and baths
        public List<SelectListItem> BedBathList = new List<System.Web.Mvc.SelectListItem>() 
        {
            new SelectListItem { Value = "0", Text = "0" },
            new SelectListItem { Value = "1", Text = "1" }, 
            new SelectListItem { Value = "2", Text = "2" },
            new SelectListItem { Value = "3", Text = "3" },
            new SelectListItem { Value = "4", Text = "4" },
            new SelectListItem { Value = "5", Text = "5" }
        };

        //GeoLocation entities added by Google API upon creation
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public void SetCoordinates()
        {
            var requestUri = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false", Uri.EscapeDataString(StreetAddress + " " + City + ", " + State + " " + ZipCode));

            var request = WebRequest.Create(requestUri);
            var response = request.GetResponse();
            var xdoc = XDocument.Load(response.GetResponseStream());

            if (!(xdoc.Element("GeocodeResponse").Element("status").ToString() == "ZERO_RESULTS"))
            {
                var result = xdoc.Element("GeocodeResponse").Element("result");
                var locationElement = result.Element("geometry").Element("location");
                Latitude = (double)locationElement.Element("lat");
                Longitude = (double)locationElement.Element("lng");
            }
        }

        //Temporary distance placeholder for active searches
        public double DistanceFromSearch { get; set; }

        public string UserId { get; set; }

        //Basic Details
        [Display(Name = "Property Type")]
        public PropertyTypes PropertyType { get; set; }
        [Display(Name = "55 Plus Only")]
        public YesNo FiftyFivePlusOnly { get; set; }
        

        //Indoor values
        [Display(Name = "Does Property Have Ceiling Fans?")]
        public YesNo CeilingFans { get; set; }
        [Display(Name = "Is Property Furnished?")]
        public YesNo Furnished { get; set; }
        [Display(Name = "Does Property Have a Fireplace?")]
        public YesNo Fireplace { get; set; }
        [Display(Name = "Does the Landlord Pay the Cable?")]
        public YesNo CablePaid { get; set; }
        [Display(Name = "What Type of Heat is Present?")]
        public HeatStyles HeatStyle { get; set; }

        //Kitchen
        [Display(Name = "Does Property Have a Dishwasher?")]
        public YesNo DishWasher { get; set; }
        [Display(Name = "Does Property Have a Stove?")]
        public YesNo Stove { get; set; }
        [Display(Name = "Does Property Have a Garbage Disposal?")]
        public YesNo GarbageDisposal { get; set; }
        [Display(Name = "Does Property Have a Refrigerator?")]
        public YesNo Refrigerator { get; set; }
        [Display(Name = "Does Property Have a Microwave?")]
        public YesNo Microwave { get; set; }

        //Outdoor
        [Display(Name = "Does Property Have a Swimming Pool?")]
        public YesNo SwimmingPool { get; set; }
        [Display(Name = "Is the Property Gated?")]
        public YesNo GatedCommunity { get; set; }
        [Display(Name = "Does Property Include Lawn Care?")]
        public YesNo LawnCareIncluded { get; set; }
        [Display(Name = "What Parking Type is Available?")]
        public ParkingTypes ParkingType { get; set; }
        [Display(Name = "Does Property Have a Fenced Yard?")]
        public YesNo FencedYard { get; set; }
        [Display(Name = "Is a Patio or Porch Available?")]
        public PatioPorches PatioPorch { get; set; }

        //Other
        [Display(Name = "Does Property Allow Smoking?")]
        public YesNo IsSmokingAllowed { get; set; }
        [Display(Name = "What is the Lot Size?")]
        public int LotSize { get; set; }
        [Display(Name = "Does Property Provide Pest Control?")]
        public YesNo PestControl { get; set; }


        //Who Pays What
        [Display(Name = "Does Tenant Pay Electric?")]
        public YesNo TenantPaysElectric { get; set; }
        [Display(Name = "Does Tenant Pay Water?")]
        public YesNo TenantPaysWater { get; set; }
        [Display(Name = "Does Tenant Pay Sewer?")]
        public YesNo TenantPaysSewer { get; set; }
        [Display(Name = "Does Tenant Pay Heat?")]
        public YesNo TenantPaysHeat { get; set; }

        [Display(Name = "Electric Status: ")]
        public string ElectricStatus { get; set; }
        [Display(Name = "Water Status: ")]
        public string WaterStatus { get; set; }
        [Display(Name = "Sewer Status: ")]
        public string SewerStatus { get; set; }
        [Display(Name = "Heat Status: ")]
        public string HeatStatus { get; set; }

        //Accessibility
        [Display(Name = "Is Property Handicp Accessible?")]
        public YesNo IsPropertyAccessible { get; set; }
        [Display(Name = "Is Parking Close?")]
        public YesNo IsParkingClose { get; set; }
        [Display(Name = "Is There a No Step Entry?")]
        public YesNo NoStepEntry { get; set; }
        [Display(Name = "Is There a Ramp Entry?")]
        public YesNo RampedEntry { get; set; }
        [Display(Name = "Are Doors 32 Inches or Wider?")]
        public YesNo Doorway32OrWider { get; set; }
        [Display(Name = "Are Paths in Home 32 Inches or Wider")]
        public YesNo AccessiblePathInHome { get; set; }
        [Display(Name = "Is There an Automatic Entry Door?")]
        public YesNo AutomaticEntryDoor { get; set; }
        [Display(Name = "Are There Lever Style Door Handles?")]
        public YesNo LeverStyleDoorHandles { get; set; }
        [Display(Name = "Single Level or First Floor?")]
        public YesNo SingleLevelOrFirstFloow { get; set; }
        [Display(Name = "Number of Inside Steps: ")]
        public int InsideSteps { get; set; }
        [Display(Name = "Number of Outside Steps: ")]
        public int OutsideSteps { get; set; }



        public Property()
        {
            var PropertyImages = new List<PropertyImage>();
        }

        public virtual ICollection<PropertyImage> PropertyImages { get; set; }

        public ApplicationUser User { get; set; }
        public IEnumerator<PropertyImage> GetEnumerator()
        {
            return PropertyImages.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}