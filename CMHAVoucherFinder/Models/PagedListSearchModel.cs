using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;

namespace CMHAVoucherFinder.Models
{
    public class PagedListSearchModel
    {
        public string Zipcode { get; set; }
        public int Maxdistance { get; set; }
        public int Minrent { get; set; }
        public int Maxrent { get; set; }
        public YesNo IsAccessible { get; set; }
        public PropertyTypes PropertyType { get; set; }


        public IPagedList<Property> PropertyList { get; set; }
    }
}