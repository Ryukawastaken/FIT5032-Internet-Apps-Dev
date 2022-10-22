using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FIT5032_MyViewModel.Models
{
    public class SampleFormViewModel
    {
    }

    public class FormOneViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(30, ErrorMessage = "Last name can't be more than 30 characters!")]
        public string LastName { get; set; }
    }
}