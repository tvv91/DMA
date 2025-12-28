using Web.Interfaces;
using Web.Models;

namespace Web.Services
{
    public class DigitizationService : IDigitizationService
    {
        private readonly IDigitizationRepository _digitizationRepository;

        public DigitizationService(IDigitizationRepository digitizationRepository)
        {
            _digitizationRepository = digitizationRepository;
        }

        public async Task<IEnumerable<Digitization>> GetByAlbumIdAsync(int albumId)
        {
            return await _digitizationRepository.GetByAlbumIdAsync(albumId);
        }

        public async Task<Digitization> AddAsync(Digitization digitization)
        {
            digitization.AddedDate = DateTime.Now;
            return await _digitizationRepository.AddAsync(digitization);
        }

        public async Task<Digitization> UpdateAsync(Digitization digitization)
        {
            digitization.UpdateDate = DateTime.UtcNow;
            return await _digitizationRepository.UpdateAsync(digitization);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _digitizationRepository.DeleteAsync(id);
        }
    }
}

