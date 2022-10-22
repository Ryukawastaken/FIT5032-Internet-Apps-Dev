using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace FinalAssignment.Models
{
    //Represents a single client
    public class Client
    {
        [Required]
        public int ClientID { get; set; }

        [Required(ErrorMessage = "You can't leave First Name field blank!")]
        [StringLength(50,ErrorMessage = "First Name can't be more than 50 characters!")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "You can't leave Last Name field blank!")]
        [StringLength(50, ErrorMessage = "Last Name can't be more than 50 characters!")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "You can't leave Date of Birth field blank!")]
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }

        [Required(ErrorMessage = "You can't leave Medicare Number field blank!")]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "Medicare Number must be 9 digits.")]
        [Range(100000000,999999999,ErrorMessage = "Medicare Number must be numeric!")]
        [RegularExpression(@"(\S)+", ErrorMessage = "White space not allowed!")]
        public string MedicareNo { get; set; }

        [Required(ErrorMessage = "You can't leave Unique ID field blank!")]
        [StringLength(1, MinimumLength = 1, ErrorMessage = "Medicare Unique Identifier must be 1 digit.")]
        [Range(1,9,ErrorMessage = "Unique ID must be between 1 and 9!")]
        [RegularExpression(@"(\S)+", ErrorMessage = "White space not allowed!")]
        public string MedicareUniqueID { get; set; }

        [Required(ErrorMessage = "You can't leave address field blank!")]
        [StringLength(100,ErrorMessage = "Address can't be longer than 100 characters!")]
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
        [StringLength(10, ErrorMessage ="Phone number must be 10 digits")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "You can't leave email address field blank!")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public List<Appointment> Appointments { get; set; }


        //Get concatanated string for dropdown lists
        [NotMapped]
        public string DropDownList { get { return ClientID + " - " + FirstName + " " + LastName; } }

    }
}