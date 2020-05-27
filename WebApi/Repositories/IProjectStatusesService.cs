using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data;

namespace WebApi.Repositories
{
    public interface IProjectStatusesService
    {
        Task UpdateProjectStatusesAsync();
    }
}
