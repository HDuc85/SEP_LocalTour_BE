using AutoMapper;
using LocalTour.Services.Abstract;
using LocalTour.Services.ViewModel;
using LocalTour.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using LocalTour.WebApi.Helper;
using Microsoft.AspNetCore.Authorization;

namespace LocalTour.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserReportController : ControllerBase
    {
        private readonly IUserReportService _userReportService;
        private readonly IMapper _mapper;

        public UserReportController(IUserReportService userReportService, IMapper mapper)
        {
            _userReportService = userReportService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserReportRequest>>> GetAllUserReports()
        {
            var reports = await _userReportService.GetAllReports();
            var reportRequests = _mapper.Map<IEnumerable<UserReportRequest>>(reports);
            return Ok(reportRequests);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserReportRequest>> GetUserReportById(int id)
        {
            var report = await _userReportService.GetReportById(id);
            if (report == null) return NotFound();

            var reportRequest = _mapper.Map<UserReportRequest>(report);
            return Ok(reportRequest);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<UserReportRequest>> CreateUserReport([FromBody] UserReportRequest request)
        {
            try
            {
                if (request == null) return BadRequest();


                var createdReport = await _userReportService.CreateReport(request, User.GetUserId());

                var createdReportRequest = _mapper.Map<UserReportRequest>(createdReport);
                return createdReportRequest;
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUserReport(int id, [FromBody] UserReportRequest request)
        {
            if (request == null) return BadRequest();

            var reportEntity = _mapper.Map<UserReport>(request);
            reportEntity.Id = id;

            if (!await _userReportService.UpdateReport(reportEntity))
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUserReport(int id)
        {
            var response = await _userReportService.DeleteReport(id);
            if (!response.Success)
            {
                return NotFound(response.Message);
            }
            return NoContent();
        }
    }
}
