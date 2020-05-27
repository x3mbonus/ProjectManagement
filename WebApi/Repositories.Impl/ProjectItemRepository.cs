using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data;
using WebApi.Exceptions;

namespace WebApi.Repositories.Impl
{
    public class ProjectItemRepository : IProjectItemRepository
    {
        private readonly ProjectsDbContext _context;

        public ProjectItemRepository(ProjectsDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProjectItem>> GetProjectItems()
        {
            return await _context.ProjectItems.ToListAsync();
        }

        public async Task<ProjectItem> GetProjectItem(int id)
        {
            return await _context.ProjectItems.FindAsync(id);
        }

        public async Task PutProjectItem(ProjectItem projectItem)
        {
            _context.Entry(projectItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectItemExists(projectItem.Id))
                {
                    throw new NotFoundException();
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<int> PostProjectItem(ProjectItem projectItem)
        {
            _context.ProjectItems.Add(projectItem);
            await _context.SaveChangesAsync();

            return projectItem.Id;
        }

        public async Task<ProjectItem> DeleteProjectItem(int id)
        {
            var projectItem = await _context.ProjectItems.FindAsync(id);
            if (projectItem == null)
            {
                throw new NotFoundException();
            }

            _context.ProjectItems.Remove(projectItem);
            await _context.SaveChangesAsync();

            return projectItem;
        }

        private bool ProjectItemExists(int id)
        {
            return _context.ProjectItems.Any(e => e.Id == id);
        }
    }
}
