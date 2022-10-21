using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinalAssignment.Models
{
    public class Schedule
    {
        public int ScheduleID { get; set; }

        public string Name { get; set; }
        
        public DateTime StartTime { get; set; }
        
        public DateTime EndTime { get; set; }

        public TimeSpan Length
        {
            get { return EndTime - StartTime; }
        }
    }
}