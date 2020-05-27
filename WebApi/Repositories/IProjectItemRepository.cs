using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data;

namespace WebApi.Repositories
{
    public interface IProjectItemRepository
    {
        Task<List<ProjectItem>> GetItemsAsync();

        Task<ProjectItem> FindByIdAsync(int id);

        Task<bool> UpdateItem(ProjectItem projectItem);

        Task<int> CreateItemAsync(ProjectItem projectItem);

        Task<int> DeleteItemAsync(int id);
    }
}
