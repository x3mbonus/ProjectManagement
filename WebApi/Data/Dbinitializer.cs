using System;
using System.Linq;

namespace WebApi.Data
{

    public static class DbInitializer
    {
        public static void Initialize(ProjectsDbContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.ProjectItems.Any())
            {
                return;   // DB has been seeded
            }

            var p1 = new ProjectItem { ParentId = null, Name = "1. Project 1", StartDate = new DateTime(2020, 06, 01), EndDate = new DateTime(2020, 06, 30), Type = ItemType.Project };
            context.ProjectItems.Add(p1);
            context.SaveChanges();

            var p11 = new ProjectItem { ParentId = p1.Id, Name = "1.1. SubProject 1", StartDate = new DateTime(2020, 06, 01), EndDate = new DateTime(2020, 06, 10), Type = ItemType.Project };
            context.ProjectItems.Add(p11);
            context.SaveChanges();

            var p111 = new ProjectItem { ParentId = p11.Id, Name = "1.1.1. Task 1", StartDate = new DateTime(2020, 06, 01), EndDate = new DateTime(2020, 06, 10), Type = ItemType.Task };
            var p112 = new ProjectItem { ParentId = p11.Id, Name = "1.1.2. Task 2", StartDate = new DateTime(2020, 06, 01), EndDate = new DateTime(2020, 06, 10), Type = ItemType.Task };
            var p113 = new ProjectItem { ParentId = p11.Id, Name = "1.1.3. Task 3", StartDate = new DateTime(2020, 06, 01), EndDate = new DateTime(2020, 06, 10), Type = ItemType.Task };
            context.ProjectItems.Add(p111);
            context.ProjectItems.Add(p112);
            context.ProjectItems.Add(p113);
            context.SaveChanges();

            var p1131 = new ProjectItem { ParentId = p113.Id, Name = "1.1.3.1 Task 3.1", StartDate = new DateTime(2020, 06, 01), EndDate = new DateTime(2020, 06, 10), Type = ItemType.Task };
            context.ProjectItems.Add(p1131);
            context.SaveChanges();

            var p12 = new ProjectItem { ParentId = p1.Id, Name = "1.2. SubProject 2", StartDate = new DateTime(2020, 06, 01), EndDate = new DateTime(2020, 06, 10), Type = ItemType.Project };
            var p13 = new ProjectItem { ParentId = p1.Id, Name = "1.3. SubProject 3", StartDate = new DateTime(2020, 06, 01), EndDate = new DateTime(2020, 06, 10), Type = ItemType.Project };
            context.ProjectItems.Add(p12);
            context.ProjectItems.Add(p13);
            context.SaveChanges();

            var p131 = new ProjectItem { ParentId = p13.Id, Name = "1.3.1. Task 1", StartDate = new DateTime(2020, 06, 01), EndDate = new DateTime(2020, 06, 10), Type = ItemType.Task };
            context.ProjectItems.Add(p131);
            context.SaveChanges();
        }
    }
}