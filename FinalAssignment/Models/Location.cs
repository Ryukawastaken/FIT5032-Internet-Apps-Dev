using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FinalAssignment.Models
{
    public class Location
    {
        [Required]
        public int LocationID { get; set; }

        [Required(ErrorMessage = "You can't leave Location name blank!")]
        [StringLength(100, ErrorMessage = "Location name must be less than 100 characters!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "You can't leave description blank!")]
        [StringLength(100, ErrorMessage = "Description must be less than 100 characters!")]
        public string Description { get; set; }

        [Required(ErrorMessage = "You can't leave Latitude blank!")]
        [Range(-180,180,ErrorMessage = "Must be between -180 and 180")]
        [DisplayFormat(DataFormatString = "{0:###.########}")]
        public float Latitude { get; set; }

        [Required(ErrorMessage = "You can't leave Longitude blank!")]
        [Range(-180, 180, ErrorMessage = "Must be between -180 and 180")]
        [DisplayFormat(DataFormatString = "{0:###.########}")]
        public float Longitude { get; set; }



        public List<Appointment> Appointments { get; set; }

        //Represents the rating of this location, calculated by averaging the rating of all of their appointments
        public float Rating {
            get
            {
                float sum = 0;
                int numAppointments = 0;
                var tempContext = new PracticeContext();
                var appointments = tempContext.Appointments.Include(l => l.Location).ToList();
                for (int i = 0; i < appointments.Count; i++)
                {
                    if (appointments[i].Location.LocationID == LocationID)
                    {
                        sum += (int)appointments[i].DoctorRating;
                        numAppointments++;
                    }
                }

                if (numAppointments > 0)
                {
                    return sum / numAppointments;
                }
                return 0;
            }
        }

        //Get concatanated string for dropdown lists
        [NotMapped]
        public string DropDownList { get { return LocationID + " - " + Name + " " + Description; } }
    }
}