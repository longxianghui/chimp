using System;
using System.Collections.Generic;
using System.Text;
using Leo.Chimp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Leo.Chimp.Domain.Repositories
{
    public class StudentRepository: EfCoreRepository<Student>,IStudentRepository
    {
        public StudentRepository(DbContext context) : base(context)
        {
        }
    }

    public interface IStudentRepository : IRepository<Student>
    {

    }
}
