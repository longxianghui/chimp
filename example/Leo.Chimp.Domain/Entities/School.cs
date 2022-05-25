using System;
using System.Collections.Generic;
using System.Text;

namespace Leo.Chimp.Domain.Entities
{
    public class School
    {
        public School()
        {
            Students = new List<Student>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public List<Student> Students { get; set; }
    }
}
