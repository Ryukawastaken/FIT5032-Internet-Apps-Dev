using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinalAssignment.Models
{
    //Enum for different types of appointments
    public enum appointment_type
    {
        Consultation,
        Checkup,
        BloodTest,
        Vaccination
    }

    //Enum for different rating scores for doctors and appointments
    public enum rating_score
    {
        Horrible = 1,
        Okay = 2,
        Good = 3,
        Great = 4,
        Fantastic = 5
    }

    //Represents an appointment
    public class Appointment
    {
        [Required]
        public int AppointmentID { get; set; }

        public Client Client { get; set; }

        public Doctor Doctor { get; set; }

        public rating_score DoctorRating { get; set; }

        public rating_score AppointmentRating { get; set; }

        [Required]
        public appointment_type AppointmentType { get; set; }

        public Schedule Schedule { get; set; }

        [Required]
        public string ClientString { get; set; }

        [Required]
        public string DoctorString { get; set; }

        [Required]
        public string LocationString { get; set; }

        public Location Location { get; set; }


        [Required(ErrorMessage = "You can't leave the date blank!")]
        [DataType(DataType.Date)]
        public DateTime DateAndTime { get; set; }

        [Required(ErrorMessage = "You must have a duration!")]
        [Range(0,480,ErrorMessage = "Duration must be between 0 and 480 minutes!")]
        public int Duration { get; set; }

        public string ImageName { get; set; }
        public Image Image { get; set; }


    }
}