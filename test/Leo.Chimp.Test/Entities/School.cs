using System;
using System.Collections.Generic;
using System.Text;

namespace Leo.Chimp.Test.Entities
{
    public class School : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
