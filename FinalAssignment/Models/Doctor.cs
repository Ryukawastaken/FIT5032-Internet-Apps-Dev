using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    }
}