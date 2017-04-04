using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMHAVoucherFinder.Models
{
    public class PropertyImage
    {
        
        public int PropertyImageId { get; set; }
        public string Name { get; set; }
        public string FilePath { get; set; }
        public string ThumbFilePath { get; set; }

        public virtual Property Property { get; set; }
    }
}