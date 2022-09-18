using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace FIT5032Assignment.Models
{
    public class Appointment
    {
        //[Key]
        public int ID { get; set; }
        private Client appointmentClient;

        public Appointment()
        {

        }

        public Appointment(Client newClient)
        {
            appointmentClient = newClient;
        }
    }
}