using System;
using System.Collections.Generic;
using System.Text;
using Leo.Chimp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Leo.Chimp.Domain.Repositories
{
    public class SchoolRepository: EfCoreRepository<School>,ISchoolRepository
    {
        public SchoolRepository(DbContext context) : base(context)
        {
        }
    }
    public interface ISchoolRepository : IRepository<School>
    {
    }
}
