using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.RegularExpressions;

namespace Leo.Chimp.Test.Entities
{
    [Table("student")]
    public class Student : IEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid SchoolId { get; set; }
        public DateTime Birthday { get; set; }

        public School MySchool { get; set; }
    }
}
