using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
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

        public List<Appointment> Appointments { get; set; }

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

        public string DropDownList { get { return LocationID + " - " + Name + " " + Description; } }
    }
}