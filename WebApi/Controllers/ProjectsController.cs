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

        // ✅ Get All Projects
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var projects = await _projectService.GetAllProjectsAsync();
                return Ok(projects);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [GetAll] Error: {ex.Message}");
                return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
            }
        }

        // ✅ Get Project by Number
        [HttpGet("{projectNumber}")]
        public async Task<IActionResult> Get(string projectNumber)
        {
            try
            {
                var project = await _projectService.GetProjectByNumberAsync(projectNumber);
                return project == null
                    ? NotFound(new { message = $"Project '{projectNumber}' not found." })
                    : Ok(project);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [Get] Error: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred retrieving the project.", error = ex.Message });
            }
        }

        // ✅ Create a Simple Project
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] ProjectDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Invalid project data." });

            try
            {
                var newNumber = await _projectService.CreateProjectAsync(dto);
                return CreatedAtAction(nameof(Get), new { projectNumber = newNumber },
                    new { message = "Project created successfully!", projectNumber = newNumber });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [CreateProject] Error: {ex.Message}");
                return StatusCode(500, new { message = "Failed to create project.", error = ex.Message });
            }
        }

        // ✅ Create a Detailed Project
        [HttpPost("create-details")]
        public async Task<IActionResult> CreateProjectWithDetails([FromBody] ProjectCreateDetailedDto model)
        {
            if (model == null)
                return BadRequest(new { message = "No data provided." });

            try
            {
                var newNumber = await _projectService.CreateProjectWithDetailsAsync(model);
                return CreatedAtAction(nameof(Get), new { projectNumber = newNumber },
                    new { message = "Project created successfully!", projectNumber = newNumber });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [CreateProjectWithDetails] Error: {ex.Message}");
                return StatusCode(500, new { message = "Failed to create project with details.", error = ex.Message });
            }
        }

        // ✅ Update Project
        [HttpPut("{projectNumber}")]
        public async Task<IActionResult> UpdateProject(string projectNumber, [FromBody] ProjectDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Invalid project data." });

            try
            {
                await _projectService.UpdateProjectAsync(dto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Project '{projectNumber}' not found." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [UpdateProject] Error: {ex.Message}");
                return StatusCode(500, new { message = "Failed to update project.", error = ex.Message });
            }
        }

        // ✅ Delete Project
        [HttpDelete("{projectNumber}")]
        public async Task<IActionResult> DeleteProject(string projectNumber)
        {
            try
            {
                var success = await _projectService.DeleteProjectAsync(projectNumber);
                return success ? NoContent() : NotFound(new { message = $"Project '{projectNumber}' not found." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [DeleteProject] Error: {ex.Message}");
                return StatusCode(500, new { message = "Failed to delete project.", error = ex.Message });
            }
        }

        // ✅ Get All Statuses
        [HttpGet("statuses")]
        public async Task<IActionResult> GetStatuses()
        {
            try
            {
                var statuses = await _projectService.GetProjectStatusesAsync();
                return Ok(statuses);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [GetStatuses] Error: {ex.Message}");
                return StatusCode(500, new { message = "Failed to retrieve statuses.", error = ex.Message });
            }
        }

        // ✅ Get All Services
        [HttpGet("services")]
        public async Task<IActionResult> GetServices()
        {
            try
            {
                var services = await _serviceService.GetAllServicesAsync();
                return Ok(services);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [GetServices] Error: {ex.Message}");
                return StatusCode(500, new { message = "Failed to retrieve services.", error = ex.Message });
            }
        }

        // ✅ Get All Staff
        [HttpGet("staff")]
        public async Task<IActionResult> GetStaff()
        {
            try
            {
                var staff = await _staffService.GetAllStaffAsync();
                return Ok(staff);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [GetStaff] Error: {ex.Message}");
                return StatusCode(500, new { message = "Failed to retrieve staff.", error = ex.Message });
            }
        }

        // ✅ Get All Customers
        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomers()
        {
            try
            {
                var customers = await _customerService.GetAllCustomersAsync();
                return Ok(customers);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [GetCustomers] Error: {ex.Message}");
                return StatusCode(500, new { message = "Failed to retrieve customers.", error = ex.Message });
            }
        }
    }
}
