using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace FinalAssignment.Models
{
    public class Client
    {
        public int ClientID { get; set; }
        [Required(ErrorMessage = "You can't leave this field blank!")]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required(ErrorMessage = "You can't leave this field blank!")]
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }
        [Required(ErrorMessage = "You can't leave this field blank!")]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "Medicare Number must be 9 digits.")]
        [RegularExpression(@"(\S)+", ErrorMessage = "White space not allowed!")]
        public string MedicareNo { get; set; }
        [Required(ErrorMessage = "You can't leave this field blank!")]
        [StringLength(1, MinimumLength = 1, ErrorMessage = "Medicare Unique Identifier must be 1 digit.")]
        [RegularExpression(@"(\S)+", ErrorMessage = "White space not allowed!")]
        public string MedicareUniqueID { get; set; }
        public string Address { get; set; }
        public string Suburb { get; set; }
        public string Postcode { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public List<Appointment> Appointments { get; set; }
        [NotMapped]
        public string DropDownList { get { return ClientID + " - " + FirstName + " " + LastName; } }

    }
}