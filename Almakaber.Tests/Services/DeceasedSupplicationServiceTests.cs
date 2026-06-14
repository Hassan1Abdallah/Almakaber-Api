using Almakaber.BLL.Services.Implementations;
using Almakaber.DAL.Entities;
using Almakaber.DAL.Repositories.Interfaces;
using Moq; 
using System.Linq.Expressions;
using Xunit;

namespace Almakaber.Tests.Services
{
    public class DeceasedSupplicationServiceTests
    {
        private readonly Mock<IGenericRepository<DeceasedSupplication>> _counterRepoMock;
        private readonly Mock<IGenericRepository<Supplication>> _supplicationRepoMock; 
        private readonly DeceasedSupplicationService _service;

        public DeceasedSupplicationServiceTests()
        {
            _counterRepoMock = new Mock<IGenericRepository<DeceasedSupplication>>();
            _supplicationRepoMock = new Mock<IGenericRepository<Supplication>>();

            _service = new DeceasedSupplicationService(_counterRepoMock.Object, _supplicationRepoMock.Object);
        }

        [Fact]
        public async Task IncrementCounterAsync_WhenRecordExists_ShouldIncrementAndUpdate()
        {
            int deceasedId = 1, supplicationId = 1;
            string userId = "user-123";

            var existingRecord = new DeceasedSupplication
            {
                DeceasedId = deceasedId,
                SupplicationId = supplicationId,
                UserId = userId,
                Counter = 5
            };

            _counterRepoMock
                .Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<DeceasedSupplication, bool>>>()))
                .ReturnsAsync(new List<DeceasedSupplication> { existingRecord });

            var result = await _service.IncrementCounterAsync(deceasedId, supplicationId, userId);

            Assert.True(result);
            Assert.Equal(6, existingRecord.Counter);

            _counterRepoMock.Verify(repo => repo.Update(existingRecord), Times.Once);
            _counterRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            _counterRepoMock.Verify(repo => repo.AddAsync(It.IsAny<DeceasedSupplication>()), Times.Never);
        }

        [Fact]
        public async Task IncrementCounterAsync_WhenRecordDoesNotExist_ShouldCreateNewWithCounterOne()
        {
            _counterRepoMock
                .Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<DeceasedSupplication, bool>>>()))
                .ReturnsAsync(new List<DeceasedSupplication>());

            var result = await _service.IncrementCounterAsync(1, 1, "user-123");

            Assert.True(result);

            _counterRepoMock.Verify(repo => repo.AddAsync(It.Is<DeceasedSupplication>(ds => ds.Counter == 1)), Times.Once);
            _counterRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            _counterRepoMock.Verify(repo => repo.Update(It.IsAny<DeceasedSupplication>()), Times.Never);
        }
    }
}