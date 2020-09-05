using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Leo.Chimp.Test.Entities;
using Leo.Chimp.Test.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Leo.Chimp.Test
{
    public class EfRepositoryTest
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly ISchoolRepository _schoolRepository;

        public EfRepositoryTest()
        {
            var services = new ServiceCollection();

            services.AddChimp(opt =>
            {
                //opt.UseMySql("server = 10.0.0.146;database=chimp;uid=root;password=123456;");
                opt.UseMySql("Server=192.168.5.5;Database=Chimp;Uid=root;Pwd='luodaoyi';SslMode=none");
            });

            //services.AddChimp(opt =>
            //{
            //    opt.UseSqlServer("Server=10.0.0.99;Database=chimp;Uid=sa;Pwd=Fuluerp123");
            //});
            var sp = services.BuildServiceProvider();
            _unitOfWork = sp.GetRequiredService<IUnitOfWork>();
            _schoolRepository = sp.GetRequiredService<ISchoolRepository>();
        }


        [Fact]
        public School Insert()
        {
            var school = new School
            {
                Name = $"EF_Insert_{Guid.NewGuid()}",
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

        [Fact]
        public void PagedList()
        {
            var schools = _schoolRepository.TableNoTracking.ToPagedList(1, 10);
            Assert.True(schools != null);
        }

        [Fact]
        public async Task PagedListAsync()
        {
            var schools = await _schoolRepository.TableNoTracking.ToPagedListAsync(1, 10);
            Assert.True(schools != null);
        }

        [Fact]
        public List<School> Inserts()
        {
            var schools = new List<School>();
            for (int i = 0; i < 100; i++)
            {
                var school = new School
                {
                    Id = Guid.NewGuid(),
                    Name = $"EF_Inserts_{Guid.NewGuid()}",
                };
                schools.Add(school);
            }
            _schoolRepository.Insert(schools);
            _unitOfWork.SaveChanges();

            foreach (var item in schools)
            {
                var school = _schoolRepository.GetById(item.Id);
                if (school == null)
                {
                    Assert.True(false);
                }
            }
            return schools;
        }
        [Fact]
        public async Task InsertAsync()
        {
            var school = new School
            {
                Id = Guid.NewGuid(),
                Name = $"EF_InsertAsync_{Guid.NewGuid()}"
            };
            await _schoolRepository.InsertAsync(school);
            await _unitOfWork.SaveChangesAsync();
        }


        [Fact]
        public async Task GetByIdAsync()
        {
            var newSchool = Insert();

            var school = await _schoolRepository.GetByIdAsync(newSchool.Id);
            Assert.True(school != null);
        }

        [Fact]
        public async Task InsertsAsync()
        {
            var schools = new List<School>();
             
            for (int i = 0; i < 100; i++)
            {
                var school = new School
                {
                    Id = Guid.NewGuid(),
                    Name = $"{"EF_InsertsAsync_"}_{i}"
                };
                schools.Add(school);
            }
            await _schoolRepository.InsertAsync(schools);
            await _unitOfWork.SaveChangesAsync();

            foreach (var item in schools)
            {
                var school = await _schoolRepository.GetByIdAsync(item.Id);
                if (school == null)
                {
                    Assert.True(false);
                }
            }
        }
        [Fact]
        public void Update()
        {
            var school = Insert();
            school.Name = $"EF_Update_{Guid.NewGuid()}";
            _schoolRepository.Update(school);
            _unitOfWork.SaveChanges();
        }
        [Fact]
        public void Updates()
        {
            var schools = Inserts();

            schools.ForEach(x => x.Name = Guid.NewGuid().ToString());
            _schoolRepository.Update(schools);
            _unitOfWork.SaveChanges();

            foreach (var item in schools)
            {
                var school = _schoolRepository.GetById(item.Id);
                if (school.Name != item.Name)
                {
                    Assert.True(false);
                }
            }
        }
        [Fact]
        public void UpdateNoSelect()
        {
            var data = Insert();
            // 这里要使用相同的对象来Update, 如果重新new了一个对象 哪怕id相同 那么efcore 3则会认为不是同一个对象 就会报错:
            // System.InvalidOperationException : The instance of entity type 'School' cannot be tracked because another instance with the same key value for {'Id'} is already being tracked. When attaching existing entities, ensure that only one entity instance with a given key value is attached. Consider using 'DbContextOptionsBuilder.EnableSensitiveDataLogging' to see the conflicting key values.

            // 错误示范:
            /*
            var school = new School
            {
                Id = data.Id,
                Name = Guid.NewGuid().ToString()
            };
            _schoolRepository.Update(school, x => x.Name);
            */

            data.Name = $"EF_UpdateNoSelect_{Guid.NewGuid()}";

            _schoolRepository.Update(data, x => x.Name);
            _unitOfWork.SaveChanges();
            //这里不能使用 _schoolRepository.GetById(data.Id); 查询出来的结果和数据库不一致
            var newSchool = _schoolRepository.TableNoTracking.First(x => x.Id == data.Id);
            Assert.True(newSchool.Name == data.Name);
        }


        [Fact]
        public void Delete()
        {
            var school = Insert();

            _schoolRepository.Delete(school);
            _unitOfWork.SaveChanges();

            var newSchool = _schoolRepository.Table.FirstOrDefault(x => x.Id == school.Id);
            Assert.True(newSchool == null);
        }



        [Fact]
        public void Deletes()
        {
            var schools = new List<School>();
            for (int i = 0; i < 100; i++)
            {
                var school = new School
                {
                    Id = Guid.NewGuid(),
                    Name = Guid.NewGuid().ToString()
                };
                schools.Add(school);
            }
            _schoolRepository.Insert(schools);
            _unitOfWork.SaveChanges();

            foreach (var item in schools)
            {
                var school = _schoolRepository.GetById(item.Id);
                if (school == null)
                {
                    Assert.True(false);
                }
            }
            _schoolRepository.Delete(schools);
            _unitOfWork.SaveChanges();

            foreach (var item in schools)
            {
                var school = _schoolRepository.GetById(item.Id);
                if (school != null)
                {
                    Assert.True(false);
                }
            }
        }



        [Fact]
        public void DeleteByLambda()
        {
            var data = Insert();

            _schoolRepository.Delete(x => x.Id == data.Id);
            _unitOfWork.SaveChanges();

            var newSchool = _schoolRepository.Table.FirstOrDefault(x => x.Id == data.Id);
            Assert.True(newSchool == null);
        }
    }
}
