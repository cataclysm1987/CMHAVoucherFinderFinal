using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using CMHAVoucherFinder.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
using static System.Web.Mvc.DataAnnotationsModelMetadata;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.VisualBasic.ApplicationServices;

namespace CMHAVoucherFinder.Models
{
    public enum UserTypes { Renter, Landlord }

    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {


        public virtual ICollection<FavoriteProperty> FavoriteProperties { get; set; }
        public virtual ICollection<Property> Property { get; set; }
        public UserTypes UserType { get; set; }

        //Landlord values
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }


        public ApplicationUser()
        {
            var Properties = new List<Property>();
            var FavoriteProperties = new List<FavoriteProperty>();
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
           
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            

            return userIdentity;
        }

        
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Property> Properties { get; set; }

        public DbSet<PropertyImage> PropertyImages { get; set; }

        public DbSet<FavoriteProperty> FavoriteProperties { get; set; }
    }
}