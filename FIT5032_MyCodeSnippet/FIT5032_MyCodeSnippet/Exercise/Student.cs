using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FIT5032_MyCodeSnippet.Exercise
{
    public class Student
    {
        public String Name { get; set; }
        public String PhoneNumber { get; set; }

        public Student(String newName, String newPhoneNumber)
        {
            Name = newName;
            PhoneNumber = newPhoneNumber;
        }
    }
}