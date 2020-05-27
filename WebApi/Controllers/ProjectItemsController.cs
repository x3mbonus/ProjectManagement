using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Exceptions;
using WebApi.Repositories;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectItemsController : ControllerBase
    {
        private readonly IProjectItemRepository _projectItemRepository;

        public ProjectItemsController(IProjectItemRepository projectItemRepository)
        {
            _projectItemRepository = projectItemRepository;
        }

        // GET: api/ProjectItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectItem>>> GetProjectItems()
        {
            return await _projectItemRepository.GetItemsAsync();
        }

        // GET: api/ProjectItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectItem>> GetProjectItem(int id)
        {
            var projectItem = await _projectItemRepository.FindByIdAsync(id);

            if (projectItem == null)
            {
                return NotFound();
            }

            return projectItem;
        }

        // PUT: api/ProjectItems/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProjectItem(int id, ProjectItem projectItem)
        {   

            if (id != projectItem.Id)
            {
                return BadRequest();
            }

            try
            {
                await _projectItemRepository.UpdateItem(projectItem);
            }
            catch(NotFoundException ex)
            {
                return NotFound();
            }           

            return NoContent();
        }

        // POST: api/ProjectItems
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<int>> PostProjectItem(ProjectItem projectItem)
        {
            var id = await _projectItemRepository.CreateItemAsync(projectItem);
            if (id == 0)
            {
                return BadRequest();
            }
            return CreatedAtAction("GetProjectItem", new { id }, id);
        }

        // DELETE: api/ProjectItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProjectItem(int id)
        {
            try
            {
                await _projectItemRepository.DeleteItemAsync(id);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
