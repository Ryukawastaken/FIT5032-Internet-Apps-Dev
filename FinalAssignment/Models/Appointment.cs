using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinalAssignment.Models
{
    public enum appointment_type
    {
        Consultation,
        Checkup,
        BloodTest,
        Vaccination
    }

    public enum rating_score
    {
        Horrible = 1,
        Okay = 2,
        Good = 3,
        Great = 4,
        Fantastic = 5
    }
    public class Appointment
    {
        public int AppointmentID { get; set; }
        public Client Client { get; set; }
        public Doctor Doctor { get; set; }
        public rating_score DoctorRating { get; set; }
        public rating_score AppointmentRating { get; set; }

        public appointment_type AppointmentType { get; set; }

        public Schedule Schedule { get; set; }

        public string ClientString { get; set; }
        public string DoctorString { get; set; }
        public string LocationString { get; set; }

        public Location Location { get; set; }


        [Required(ErrorMessage = "You can't leave this field blank!")]
        [DataType(DataType.Date)]
        public DateTime DateAndTime { get; set; }
        public DateTime OldDateAndTime { get; set; }
        [Required(ErrorMessage = "You can't leave this field blank!")]
        public int Duration { get; set; }

        public string ImageName { get; set; }
        public Image Image { get; set; }


    }
}