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

        public async Task<List<ProjectItem>> GetInProgressItemsAsync()
        {
            return await _context.ProjectItems
                .Where(x => x.State == State.InProgress)
                .ToListAsync();
        }

        public async Task<ProjectItem> FindByIdAsync(int id)
        {
            return await _context.ProjectItems.FindAsync(id);
        }

        public async Task<bool> UpdateItem(ProjectItem projectItem)
        {
            var parent = await GetValidParentAsync(projectItem);
            if (parent == null)
            {
                return false;
            }
            
            _context.Entry(projectItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                await UpdateProjectStatusesAsync(projectItem);
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
            var parent = await GetValidParentAsync(projectItem);
            if (parent == null)
            {
                return 0;
            }

            _context.ProjectItems.Add(projectItem);
            await _context.SaveChangesAsync();
            await UpdateProjectStatusesAsync(projectItem);

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
            var result = await _context.SaveChangesAsync();
            await UpdateProjectStatusesAsync(projectItem);
            return result;
        }

        private bool ProjectItemExists(int id)
        {
            return _context.ProjectItems.Any(e => e.Id == id);
        }


        private async Task<ProjectItem> GetValidParentAsync(ProjectItem projectItem)
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
                    return null; //Project cann not be child of task
                }

                //We need to check for cycles here too ...
                //And all this logic better to exctract to some services layer

                return parent;
            }
            return null;       
        }

        private async Task UpdateProjectStatusesAsync(ProjectItem projectItem)
        {
            var commandText = @"
                with tree (id, rootId, type, state) as 
                    (select id, id, 0, 0 from projectItems where type = 0
                    union all
                    select p.id, t.rootId, p.type, p.state
                    from projectItems p
                        join tree t on t.id = p.ParentId
                    ),

                grouped (id, countCompleted, countInProgress, countAll) as
                    (select rootId as id,  
	                    sum(case when state = 2 then 1 else 0 end) as countCompleted,
	                    sum(case when state = 1 then 1 else 0 end) as countInProgress,
	                    sum(1) - 1 as countAll
                    from tree
                    where type = 1 or id = rootId
                    group by rootId)

                update projectItems
                set state = case 
                    when g.countCompleted > 0 and g.countCompleted = g.countAll then 2
                    when g.countInProgress > 0 then 1
                    else 0 end
                from projectItems
                join grouped g on g.id = projectItems.id";

            await _context.Database.ExecuteSqlRawAsync(commandText);
        }
    }
}
