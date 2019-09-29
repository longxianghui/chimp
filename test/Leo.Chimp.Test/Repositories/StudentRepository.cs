using System;
using System.Collections.Generic;
using System.Text;
using Leo.Chimp.Test.Entities;
using Microsoft.EntityFrameworkCore;

namespace Leo.Chimp.Test.Repositories
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
