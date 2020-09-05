using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Leo.Chimp.Domain.Entities
{
    [Table("school")]
    public class School
    {
        public School()
        {
            Students = new List<Student>();
        }
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }

        public List<Student> Students { get; set; }
    }
}
