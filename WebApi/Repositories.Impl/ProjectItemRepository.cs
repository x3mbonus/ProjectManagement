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

        public async Task<List<ProjectItem>> GetItemsAsync()
        {
            return await _context.ProjectItems.ToListAsync();
        }

        public async Task<ProjectItem> FindByIdAsync(int id)
        {
            return await _context.ProjectItems.FindAsync(id);
        }

        public async Task<bool> UpdateItem(ProjectItem projectItem)
        {
            if (!await IsParentValidAsync(projectItem))
            {
                return false;
            }
            
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
            return true;
        }

        public async Task<int> CreateItemAsync(ProjectItem projectItem)
        {
            if (!await IsParentValidAsync(projectItem))
            {
                return 0;
            }

            _context.ProjectItems.Add(projectItem);
            await _context.SaveChangesAsync();

            return projectItem.Id;
        }

        public async Task<int> DeleteItemAsync(int id)
        {
            var projectItem = await _context.ProjectItems.FindAsync(id);
            if (projectItem == null)
            {
                throw new NotFoundException();
            }

            _context.ProjectItems.Remove(projectItem);
            return await _context.SaveChangesAsync();
        }

        private bool ProjectItemExists(int id)
        {
            return _context.ProjectItems.Any(e => e.Id == id);
        }


        private async Task<bool> IsParentValidAsync(ProjectItem projectItem)
        {
            if (projectItem.ParentId.HasValue)
            {
                var parent = await FindByIdAsync(projectItem.ParentId.Value);

                if (parent == null)
                {
                    throw new NotFoundException("Parent was not found");
                }

                if (parent.Type == ItemType.Task &&
                    projectItem.Type == ItemType.Project)
                {
                    return false; //Project cann not be child of task
                }

                //We need to check for cycles here too ...
                //And all this logic better to exctract to some services layer
            }
            return true;
        }
    }
}
