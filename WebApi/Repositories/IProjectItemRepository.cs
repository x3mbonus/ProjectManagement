using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data;

namespace WebApi.Repositories
{
    /// <summary>
    /// Project items repo
    /// </summary>
    public interface IProjectItemRepository
    {
        /// <summary>
        /// Gat all items (no paging)
        /// </summary>
        /// <returns></returns>
        Task<List<ProjectItem>> GetItemsAsync();

        /// <summary>
        /// Get in progress items
        /// </summary>
        /// <returns></returns>
        Task<List<ProjectItem>> GetInProgressItemsAsync();

        /// <summary>
        /// Find item by id
        /// </summary>
        Task<ProjectItem> FindByIdAsync(int id);

        /// <summary>
        /// create new item
        /// </summary>
        Task<int> CreateItemAsync(ProjectItem projectItem);

        /// <summary>
        /// Update item (all fields)
        /// </summary>
        Task<bool> UpdateItem(ProjectItem projectItem);

        /// <summary>
        /// Delete item
        /// </summary>
        Task<int> DeleteItemAsync(int id);
    }
}
