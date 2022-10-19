using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinalAssignment.Models
{
    public class Event
    {
        public int EventID { get; set; }

        public string Name { get; set; }
        [Required(ErrorMessage = "You can't leave this field blank!")]
        [DataType(DataType.Date)]
        public DateTime StartTime { get; set; }
        [Required(ErrorMessage = "You can't leave this field blank!")]
        [DataType(DataType.Date)]
        public DateTime EndTime { get; set; }

        public TimeSpan Length
        {
            get { return EndTime - StartTime; }
        }
    }
}