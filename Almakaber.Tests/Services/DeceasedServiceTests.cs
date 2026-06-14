using Almakaber.BLL.DTOs.Deceased;
using Almakaber.BLL.Helpers;
using Almakaber.BLL.Services.Implementations;
using Almakaber.DAL.Entities;
using Almakaber.DAL.Repositories.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace Almakaber.Tests.Services
{
    public class DeceasedServiceTests
    {
        private readonly Mock<IGenericRepository<Deceased>> _deceasedRepoMock;
        private readonly Mock<IGenericRepository<Grave>> _graveRepoMock; 
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IFileService> _fileServiceMock; 
        private readonly DeceasedService _deceasedService;

        public DeceasedServiceTests()
        {
            _deceasedRepoMock = new Mock<IGenericRepository<Deceased>>();
            _mapperMock = new Mock<IMapper>();
            _fileServiceMock = new Mock<IFileService>();
            _graveRepoMock = new Mock<IGenericRepository<Grave>>();

            _deceasedService = new DeceasedService(
                _deceasedRepoMock.Object,
                _mapperMock.Object,
                _fileServiceMock.Object,
                _graveRepoMock.Object
            );
        }

        [Fact]
        public async Task AddDeceasedAsync_WhenGraveDoesNotExist_ShouldThrowArgumentException()
        {
            var createDto = new CreateDeceasedDto { GraveId = 99 }; 

            _graveRepoMock
                .Setup(repo => repo.GetByIdAsync(createDto.GraveId))
                .ReturnsAsync((Grave)null);

            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _deceasedService.AddDeceasedAsync(createDto));

            Assert.Equal("رقم المقبرة المحدد غير موجود في النظام.", exception.Message);

            _deceasedRepoMock.Verify(repo => repo.AddAsync(It.IsAny<Deceased>()), Times.Never);
        }

        [Fact]
        public async Task AddDeceasedAsync_WithValidDataAndPhoto_ShouldSaveAndReturnDto()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(100);

            var createDto = new CreateDeceasedDto { GraveId = 1, FullName = "أحمد", Photo = fileMock.Object };
            var fakeGrave = new Grave { Id = 1 };
            var fakeDeceased = new Deceased { Id = 1, FullName = "أحمد", GraveId = 1 };
            var expectedDto = new DeceasedDto { Id = 1, FullName = "أحمد" };

            _graveRepoMock.Setup(repo => repo.GetByIdAsync(createDto.GraveId)).ReturnsAsync(fakeGrave);

            _fileServiceMock.Setup(f => f.SaveFileAsync(createDto.Photo, "deceased"))
                            .ReturnsAsync("/images/deceased/test-image.png");

            _mapperMock.Setup(m => m.Map<Deceased>(createDto)).Returns(fakeDeceased);
            _mapperMock.Setup(m => m.Map<DeceasedDto>(fakeDeceased)).Returns(expectedDto);

            _deceasedRepoMock.Setup(repo => repo.AddAsync(fakeDeceased)).Returns(Task.CompletedTask);
            _deceasedRepoMock.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _deceasedService.AddDeceasedAsync(createDto);

            Assert.NotNull(result);
            Assert.Equal("أحمد", result.FullName);

            _fileServiceMock.Verify(f => f.SaveFileAsync(createDto.Photo, "deceased"), Times.Once);

            _deceasedRepoMock.Verify(repo => repo.AddAsync(fakeDeceased), Times.Once);
            _deceasedRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }
    }
}