using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data;

namespace WebApi.Repositories.Impl
{
    public class ProjectStatusesService : IProjectStatusesService
    {
        private readonly ProjectsDbContext _context;

        public ProjectStatusesService(ProjectsDbContext context)
        {
            _context = context;
        }

        public async Task UpdateProjectStatusesAsync()
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
