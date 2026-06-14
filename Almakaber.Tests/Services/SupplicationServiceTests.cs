using Almakaber.BLL.DTOs.Supplications;
using Almakaber.BLL.Services.Implementations;
using Almakaber.DAL.Entities;
using Almakaber.DAL.Repositories.Interfaces;
using AutoMapper;
using Moq;
using Xunit;

namespace Almakaber.Tests.Services
{
    public class SupplicationServiceTests
    {
        private readonly Mock<IGenericRepository<Supplication>> _supplicationRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly SupplicationService _supplicationService;

        public SupplicationServiceTests()
        {
            _supplicationRepoMock = new Mock<IGenericRepository<Supplication>>();
            _mapperMock = new Mock<IMapper>();

            _supplicationService = new SupplicationService(_supplicationRepoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetSupplicationByIdAsync_WhenSupplicationExists_ShouldReturnDto()
        {
            int supplicationId = 1;

            var fakeSupplication = new Supplication { Id = supplicationId, Content = "اللهم اغفر له" };

            var fakeDto = new SupplicationDto { Id = supplicationId, Content = "اللهم اغفر له" };

            _supplicationRepoMock
                .Setup(repo => repo.GetByIdAsync(supplicationId))
                .ReturnsAsync(fakeSupplication);

            _mapperMock
                .Setup(m => m.Map<SupplicationDto>(fakeSupplication))
                .Returns(fakeDto);

            var result = await _supplicationService.GetSupplicationByIdAsync(supplicationId);

            Assert.NotNull(result);

            Assert.Equal("اللهم اغفر له", result.Content);

            _supplicationRepoMock.Verify(repo => repo.GetByIdAsync(supplicationId), Times.Once);
        }


        [Fact]
        public async Task GetSupplicationByIdAsync_WhenSupplicationDoesNotExist_ShouldReturnNull()
        {
            int nonExistingId = 999; 

            _supplicationRepoMock
                .Setup(repo => repo.GetByIdAsync(nonExistingId))
                .ReturnsAsync((Supplication)null);

            var result = await _supplicationService.GetSupplicationByIdAsync(nonExistingId);

            Assert.Null(result);
        }

        [Fact]
        public async Task AddSupplicationAsync_WithValidData_ShouldSaveAndReturnDto()
        {
            var createDto = new CreateSupplicationDto { Content = "اللهم ارحمه" };
            var fakeSupplication = new Supplication { Id = 1, Content = "اللهم ارحمه" };
            var expectedDto = new SupplicationDto { Id = 1, Content = "اللهم ارحمه" };

            _mapperMock.Setup(m => m.Map<Supplication>(createDto)).Returns(fakeSupplication);
            _mapperMock.Setup(m => m.Map<SupplicationDto>(fakeSupplication)).Returns(expectedDto);

            _supplicationRepoMock.Setup(repo => repo.AddAsync(fakeSupplication)).Returns(Task.CompletedTask);
            _supplicationRepoMock.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _supplicationService.AddSupplicationAsync(createDto);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("اللهم ارحمه", result.Content);

            _supplicationRepoMock.Verify(repo => repo.AddAsync(fakeSupplication), Times.Once);
            _supplicationRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }
    }
}