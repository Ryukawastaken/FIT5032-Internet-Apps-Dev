using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FinalAssignment.Models
{
    public class Doctor
    {
        public int DoctorID { get; set; }
        [Required(ErrorMessage = "You can't leave First Name field blank!")]
        [StringLength(50, ErrorMessage = "First Name can't be more than 50 characters!")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "You can't leave Last Name field blank!")]
        [StringLength(50, ErrorMessage = "Last Name can't be more than 50 characters!")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "You can't leave Date of Birth field blank!")]
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }

        [Required(ErrorMessage = "You can't leave address field blank!")]
        [StringLength(100, ErrorMessage = "Address can't be longer than 100 characters!")]
        public string Address { get; set; }

        [Required(ErrorMessage = "You can't leave suburb field blank!")]
        [StringLength(50, ErrorMessage = "Address can't be longer than 50 characters!")]
        public string Suburb { get; set; }

        [Required(ErrorMessage = "You can't leave postcode field blank!")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "Postcode must be 4 digits.")]
        [Range(1000, 9999, ErrorMessage = "Post code must be between 1000 and 9999!")]
        [RegularExpression(@"(\S)+", ErrorMessage = "White space not allowed!")]
        public string Postcode { get; set; }

        [Required(ErrorMessage = "You can't leave phone number field blank!")]
        [StringLength(10, ErrorMessage = "Phone number must be 10 digits")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "You can't leave email address field blank!")]
        [DataType(DataType.EmailAddress)]
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

        [NotMapped]
        public string DropDownList { get { return DoctorID + " - " + FirstName + " " + LastName; } }
    }
}