using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinalAssignment.Models
{
    //Represents an image (https://unsplash.com/photos/yo01Z-9HQAw - Homepage Banner - https://images.unsplash.com/photo-1505751172876-fa1923c5c528?ixlib=rb-4.0.3&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=2340&q=80)
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