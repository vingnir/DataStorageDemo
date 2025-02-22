using System;
using System.Threading.Tasks;
using Business.Dtos;
using Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/projects")]
    public class ProjectsController(
        IProjectService projectService,
        IStaffService staffService,
        IServiceService serviceService,
        ICustomerService customerService) : ControllerBase
    {
        private readonly IProjectService _projectService = projectService;
        private readonly IStaffService _staffService = staffService;
        private readonly IServiceService _serviceService = serviceService;
        private readonly ICustomerService _customerService = customerService;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var projects = await _projectService.GetAllProjectsAsync();
            return Ok(projects);
        }

        [HttpGet("{projectNumber}")]
        public async Task<IActionResult> Get(string projectNumber)
        {
            var project = await _projectService.GetProjectByNumberAsync(projectNumber);
            return project == null ? NotFound(new { message = $"Project '{projectNumber}' not found." }) : Ok(project);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] ProjectDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Invalid project data." });

            var newNumber = await _projectService.CreateProjectAsync(dto);
            return CreatedAtAction(nameof(Get), new { projectNumber = newNumber }, new { message = "Project created successfully!", projectNumber = newNumber });
        }

        [HttpPost("create-details")]
        public async Task<IActionResult> CreateProjectWithDetails([FromBody] ProjectCreateDetailedDto model)
        {
            if (model == null)
                return BadRequest("No data provided.");

            var newNumber = await _projectService.CreateProjectWithDetailsAsync(model);
            return CreatedAtAction(nameof(Get), new { projectNumber = newNumber },
                new { message = "Project created successfully!", projectNumber = newNumber });
        }

        [HttpPut("{projectNumber}")]
        public async Task<IActionResult> UpdateProject(string projectNumber, [FromBody] ProjectDto dto)
        {
            await _projectService.UpdateProjectAsync(dto);
            return NoContent();
        }

        [HttpDelete("{projectNumber}")]
        public async Task<IActionResult> DeleteProject(string projectNumber)
        {
            var success = await _projectService.DeleteProjectAsync(projectNumber);
            return success ? NoContent() : NotFound();
        }

        [HttpGet("statuses")]
        public async Task<IActionResult> GetStatuses()
        {
            var statuses = await _projectService.GetProjectStatusesAsync();
            return Ok(statuses);
        }

        [HttpGet("services")]
        public async Task<IActionResult> GetServices()
        {
            var services = await _serviceService.GetAllServicesAsync();
            return Ok(services);
        }

        [HttpGet("staff")]
        public async Task<IActionResult> GetStaff()
        {
            var staff = await _staffService.GetAllStaffAsync();
            return Ok(staff);
        }

        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomers()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(customers);
        }
    }
}
