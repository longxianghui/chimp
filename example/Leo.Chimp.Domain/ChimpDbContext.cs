using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Leo.Chimp.Domain
{
    public class ChimpDbContext : BaseDbContext
    {
        public ChimpDbContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //your code
        }
    }
}
