using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data;

namespace WebApi.Repositories
{
    /// <summary>
    /// Service to handle project statuses
    /// </summary>
    public interface IProjectStatusesService
    {
        /// <summary>
        /// Keep project statuses up to date
        /// We expect project to be more oftenly read, then written
        /// So lets store project status in DB, and update it when tasks are updated
        /// For now we update all projects
        /// To optimize - we can pass changed task Id, and decrease updated projects scope
        /// </summary>
        /// <returns></returns>
        Task UpdateProjectStatusesAsync();
    }
}
