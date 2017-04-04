using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CMHAVoucherFinder.Models
{
    public class FavoriteProperty
    {
        public int FavoritePropertyId { get; set; }
        public int PropertyId { get; set; }
        public string ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}