using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FinalAssignment.Models
{
    public class Doctor
    {
        public int DoctorID { get; set; }
        [Required(ErrorMessage = "You can't leave this field blank!")]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required(ErrorMessage = "You can't leave this field blank!")]
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }
        public string Address { get; set; }
        public string Suburb { get; set; }
        public string Postcode { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public List<Appointment> Appointments { get; set; }

        public float Rating
        {
            get
            {
                float sum = 0;
                int numAppointments = 0;
                var tempContext = new PracticeContext();
                var appointments = tempContext.Appointments.Include(d=>d.Doctor).ToList();
                for (int i = 0; i < appointments.Count; i++)
                {
                    if (appointments[i].Doctor.DoctorID == DoctorID)
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

        public string DropDownList { get { return DoctorID + " - " + FirstName + " " + LastName; } }
    }
}