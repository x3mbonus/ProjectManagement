using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Data
{
    public class ProjectsDbContext: DbContext
    {

        public ProjectsDbContext(DbContextOptions<ProjectsDbContext> options)
            : base(options)
        {
        }

        public DbSet<ProjectItem> ProjectItems { get; set; }
    }
}
