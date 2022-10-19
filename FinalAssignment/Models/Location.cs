using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinalAssignment.Models
{
    public class Location
    {
        public int LocationID { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }
        [DisplayFormat(DataFormatString = "{0:###.########}")]
        public float Latitude { get; set; }
        [DisplayFormat(DataFormatString = "{0:###.########}")]
        public float Longitude { get; set; }

        public string DropDownList { get { return LocationID + " - " + Name + " " + Description; } }
    }
}