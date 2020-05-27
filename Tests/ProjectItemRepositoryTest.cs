using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data;
using WebApi.Exceptions;
using WebApi.Repositories;
using WebApi.Repositories.Impl;
using Xunit;

namespace Tests
{
    public class ProjectItemRepositoryTest : IDisposable
    {
        private readonly ProjectsDbContext _context;
        private readonly ProjectItemRepository _service;
        private readonly Mock<IProjectStatusesService> _projectStatusesServiceMock = new Mock<IProjectStatusesService>();


        public ProjectItemRepositoryTest()
        {
            //TODO: For simplification we can use inmemory Db instead of Mock
            //In real app we would have service layer with logic, which uses interfaces of repository layer
            //For testing we would mock that interfaces
            var options = new DbContextOptionsBuilder<ProjectsDbContext>() 
                .UseInMemoryDatabase(databaseName: "ProjectsDatabase")
                .Options;
            // Insert seed data into the database using one instance of the context
            using (var context = new ProjectsDbContext(options))
            {
                SeedData(context);
            }

            // Use a clean instance of the context to run the test
            _context = new ProjectsDbContext(options);

            _service = new ProjectItemRepository(_context, _projectStatusesServiceMock.Object);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        private void SeedData(ProjectsDbContext context)
        {
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

        [Fact]
        public async Task GetItemsAsync_Success()
        {
            //Arrange
            //Action
            var items = await _service.GetItemsAsync();

            //Assert
            Assert.True(items.Count > 0);
        }

        [Fact]
        public async Task GetInProgressItemsAsync_NoTasks()
        {
            //Arrange

            //Action
            var items = await _service.GetInProgressItemsAsync();

            //Assert
            Assert.Empty(items);
        }

        [Fact]
        public async Task FindByIdAsync_NotFound()
        {
            //Arrange

            //Action
            var item = await _service.FindByIdAsync(-1);

            //Assert
            Assert.Null(item);
        }



        [Fact]
        public async Task FindByIdAsync_Found()
        {
            //Arrange
            var expected = await _context.ProjectItems.FirstAsync();

            //Action
            var item = await _service.FindByIdAsync(expected.Id);

            //Assert
            Assert.Equal(expected, item);
        }

        [Fact]
        public async Task CreateItemAsync_RootItem()
        {
            //Arrange
            //Action
            var id = await _service.CreateItemAsync(
                new ProjectItem
                {
                    Name = "name",
                });

            //Assert
            Assert.True(id > 0);
            var item = await _context.ProjectItems.FindAsync(id);
                
            Assert.NotNull(item);
            Assert.Equal("name", item.Name);
        }



        [Fact]
        public async Task CreateItemAsync_IncorrectParent()
        {
            //Arrange
            //Action
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                 await _service.CreateItemAsync(
                     new ProjectItem
                     {
                         Name = "name",
                         ParentId = 99999
                     }));
            //Assert
        }

        //TODO: other tests
    }
}
