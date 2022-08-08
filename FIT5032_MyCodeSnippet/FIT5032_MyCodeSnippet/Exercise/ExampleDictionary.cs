using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FIT5032_MyCodeSnippet.Exercise
{
    public class ExampleDictionary
    {
        public void Example()
        {
            Dictionary<int, Student> StudentDictionary = new Dictionary<int, Student>();

            Student S1 = new Student("Nicolas", "28785959");
            Student S2 = new Student("Jaimee", "27834231");

            StudentDictionary.Add(1,S1);
            StudentDictionary.Add(2, S2);

            Student Result = new Student("", "");

            StudentDictionary.TryGetValue(1, out Result);
        }
    }
}