using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Leo.Chimp.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Leo.Chimp.Example.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ISchoolRepository _schoolRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ValuesController(ISchoolRepository schoolRepository, IUnitOfWork unitOfWork)
        {
            _schoolRepository = schoolRepository;
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var schools = await _schoolRepository.TableNoTracking.ToPagedListAsync(1, 10);
            return Ok(schools);
        }
    }
}
