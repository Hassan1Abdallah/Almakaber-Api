using Almakaber.BLL.DTOs.Supplications;
using Almakaber.BLL.Services.Interfaces;
using Almakaber.DAL.Entities;
using Almakaber.DAL.Repositories.Interfaces;

namespace Almakaber.BLL.Services.Implementations
{
    public class DeceasedSupplicationService : IDeceasedSupplicationService
    {
        private readonly IGenericRepository<DeceasedSupplication> _counterRepo;
        private readonly IGenericRepository<Supplication> _supplicationRepo;

        public DeceasedSupplicationService(
            IGenericRepository<DeceasedSupplication> counterRepo,
            IGenericRepository<Supplication> supplicationRepo)
        {
            _counterRepo = counterRepo;
            _supplicationRepo = supplicationRepo;
        }

        public async Task<IEnumerable<DeceasedSupplicationDto>> GetCountersForDeceasedAsync(int deceasedId, string userId)
        {
            var allSupplications = await _supplicationRepo.GetAllAsync();

            var userCounters = await _counterRepo.FindAsync(c => c.DeceasedId == deceasedId && c.UserId == userId);

            var result = allSupplications.Select(sup => new DeceasedSupplicationDto
            {
                SupplicationId = sup.Id,
                Title = sup.Title,
                Content = sup.Content,
                UserCounter = userCounters.FirstOrDefault(c => c.SupplicationId == sup.Id)?.Counter ?? 0
            });

            return result;
        }

        public async Task<bool> IncrementCounterAsync(int deceasedId, int supplicationId, string userId)
        {
            var record = (await _counterRepo.FindAsync(c =>
                c.DeceasedId == deceasedId &&
                c.SupplicationId == supplicationId &&
                c.UserId == userId)).FirstOrDefault();

            if (record == null)
            {
                var newRecord = new DeceasedSupplication
                {
                    DeceasedId = deceasedId,
                    SupplicationId = supplicationId,
                    UserId = userId
                };
                await _counterRepo.AddAsync(newRecord);
            }
            else
            {
                record.IncrementCounter();
                _counterRepo.Update(record);
            }

            await _counterRepo.SaveChangesAsync();
            return true;
        }
        public async Task<bool> ToggleAnnualReminderAsync(int deceasedId, string userId)
        {
            var record = await _counterRepo.FindAsync(c => c.DeceasedId == deceasedId && c.UserId == userId);
            var existingRecord = record.FirstOrDefault();

            if (existingRecord == null)
            {
                var newRecord = new DeceasedSupplication
                {
                    DeceasedId = deceasedId,
                    UserId = userId,
                    Counter = 0,
                    WantsAnnualReminder = true
                };
                await _counterRepo.AddAsync(newRecord);
            }
            else
            {
                existingRecord.WantsAnnualReminder = !existingRecord.WantsAnnualReminder;
                _counterRepo.Update(existingRecord);
            }

            await _counterRepo.SaveChangesAsync();
            return true;
        }
    }
}