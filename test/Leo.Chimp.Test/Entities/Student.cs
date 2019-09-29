using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Leo.Chimp.Test.Entities
{
    public class Student : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid SchoolId { get; set; }
        public DateTime Birthday { get; set; }

        public School MySchool { get; set; }
    }
}
