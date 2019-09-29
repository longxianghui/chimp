using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Leo.Chimp.Test.Entities;
using Leo.Chimp.Test.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Leo.Chimp.Test
{
    public class DapperTest
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISchoolRepository _schoolRepository;

        public DapperTest()
        {
            var services = new ServiceCollection();
            services.AddKing(opt => { opt.UseMySql("server = 10.0.0.146;database=king;uid=root;password=123456;"); });
            //services.AddKing(opt => { opt.UseSqlServer("Server=10.0.0.99;Database=king;Uid=sa;Pwd=Fuluerp123"); });
            var sp = services.BuildServiceProvider();
            _unitOfWork = sp.GetRequiredService<IUnitOfWork>();
            _schoolRepository = sp.GetRequiredService<ISchoolRepository>();
        }

        [Fact]
        public async Task QueryAsync()
        {
            var schools = await _unitOfWork.QueryAsync<School>("select * from school");
            Assert.True(schools.Any());
        }

        [Fact]
        public async Task QueryPagedListAsync()
        {
            var schools = await _unitOfWork.QueryPagedListAsync<School>(1, 10, "select * from school order by id");
            Assert.True(schools.Item.Any());
        }

        [Fact]
        public async Task ExecuteAsync()
        {
            var school = new School
            {
                Id = Guid.NewGuid(),
                Name = "school"
            };

            await _unitOfWork.ExecuteAsync("insert school(id,name) values(@Id,@Name)",
                school);

            var newSchool = await _unitOfWork.QueryAsync<School>("select * from school where id =@id",
                new { Id = school.Id });

            Assert.True(school.Name == newSchool.First().Name);
        }

        [Fact]
        public async Task GetConnection()
        {
            //可以直接使用dapper
            var schools = await _unitOfWork.GetConnection().QueryAsync("select * from school");
            Assert.True(schools.Any());
        }

        [Fact]
        public async Task Transaction()
        {
            var school1 = new School
            {
                Id = Guid.NewGuid(),
                Name = "school1"
            };

            var school2 = new School
            {
                Id = Guid.NewGuid(),
                Name = "school2"
            };

            using (var tran = _unitOfWork.BeginTransaction())
            {
                try
                {
                    await _unitOfWork.ExecuteAsync("insert school(id,name) values(@Id,@Name)",
                        school1);
                    await _unitOfWork.ExecuteAsync("insert school(id,name) values(@Id,@Name)",
                        school2);
                    throw new Exception();
                    tran.Commit();
                }
                catch (Exception e)
                {
                    tran.Rollback();
                }
            }
            var newSchool1 = await _unitOfWork.QueryAsync<School>("select * from school where id =@id",
                new { Id = school1.Id });
            var newSchool2 = await _unitOfWork.QueryAsync<School>("select * from school where id =@id",
                new { Id = school2.Id });
            Assert.False(newSchool1.Any() || newSchool2.Any());

        }

        [Fact]
        public async Task HybridTransaction()
        {
            var school1 = new School
            {
                Id = Guid.NewGuid(),
                Name = "school1"
            };

            var school2 = new School
            {
                Id = Guid.NewGuid(),
                Name = "school2"
            };
            using (var tran = _unitOfWork.BeginTransaction())
            {
                try
                {
                    await _schoolRepository.InsertAsync(school1);
                    await _unitOfWork.SaveChangesAsync();

                    await _unitOfWork.ExecuteAsync("insert school(id,name) values(@Id,@Name)",
                        school2);
                    throw new Exception();
                    tran.Commit();
                }
                catch (Exception e)
                {
                    tran.Rollback();
                }
            }
            var newSchool1 = await _unitOfWork.QueryAsync<School>("select * from school where id =@id",
                new { Id = school1.Id });
            var newSchool2 = await _unitOfWork.QueryAsync<School>("select * from school where id =@id",
                new { Id = school2.Id });
            Assert.False(newSchool1.Any() || newSchool2.Any());
        }

    }
}
