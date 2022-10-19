using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinalAssignment.Models
{

    public class Appointment
    {
        public int AppointmentID { get; set; }
        public Client Client { get; set; }
        public Doctor Doctor { get; set; }
        public string ClientString { get; set; }
        public string DoctorString { get; set; }

        public string LocationString { get; set; }

        public Location Location { get; set; }


        [Required(ErrorMessage = "You can't leave this field blank!")]
        [DataType(DataType.Date)]
        public DateTime DateAndTime { get; set; }

        public Image Image { get; set; }


    }
}