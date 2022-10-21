using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinalAssignment.Models
{
    public class Image
    {
        [Required] 
        public int ImageID { get; set; }

        public string Path { get; set; }

        [Required(ErrorMessage = "You can't leave image name blank!")]
        [StringLength(100, ErrorMessage = "Filename can't be longer than 100 characters!")]
        public string Name { get; set; }


    }
}