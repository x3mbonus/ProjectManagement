using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
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
               
        // GET: api/ProjectItems/inProgress
        [HttpGet("inProgress")]
        public async Task<ActionResult<IEnumerable<ProjectItem>>> GetInProgressProjectItems()
        {
            return await _projectItemRepository.GetInProgressItemsAsync();
        }

        [HttpGet("inProgress/xls")]
        public async Task<IActionResult> GetInProgressExcel()
        {
            var items = await _projectItemRepository.GetInProgressItemsAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("In progress tasks");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Id";
                worksheet.Cell(currentRow, 2).Value = "Name";
                foreach (var item in items)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = item.Id;
                    worksheet.Cell(currentRow, 2).Value = item.Name;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "users.xlsx");
                }
            }
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
