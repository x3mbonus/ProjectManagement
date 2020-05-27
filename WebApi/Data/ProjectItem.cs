using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Data
{
    /// <summary>
    /// Project item (can be project or task, but stored in one table
    /// We could use inheritance for that, but for now lets keep it simple
    /// </summary>
    public class ProjectItem
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Parent id
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// Type, project or task
        /// </summary>
        public ItemType Type { get; set; }
        
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Start
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// State (can be updated only for task, for projects is calculated)
        /// </summary>
        public State State { get; set; }
    }
}
