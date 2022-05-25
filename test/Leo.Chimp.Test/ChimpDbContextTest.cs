using System;
using System.Collections.Generic;
using System.Text;
using Leo.Chimp.Test.Entities;
using Leo.Chimp.Test.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Leo.Chimp.Test
{

    public class ChimpDbContextTest 
    {
        private readonly DbType dbType;

        private readonly IUnitOfWork _unitOfWork;
        private readonly ISchoolRepository _schoolRepository;
        public ChimpDbContextTest()
        {
            var services = new ServiceCollection();
            dbType = DbType.MYSQL;
            InitChimpTestDb.Start(services, dbType);
            var sp = services.BuildServiceProvider();
            _unitOfWork = sp.GetRequiredService<IUnitOfWork>();
            _schoolRepository = sp.GetRequiredService<ISchoolRepository>();
        }

        [Fact]
        public School Insert()
        {
            var school = new School
            {
                Name = Guid.NewGuid().ToString(),
                Id = Guid.NewGuid()
            };
            _schoolRepository.Insert(school);
            _unitOfWork.SaveChanges();
            return school;
        }

        [Fact]
        public void GetById()
        {
            var newSchool = Insert();
            var school = _schoolRepository.GetById(newSchool.Id);
            Assert.True(school != null);
        }
    }
}
