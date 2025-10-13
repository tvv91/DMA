using Microsoft.EntityFrameworkCore;
using Web.Common;
using Web.Db;
using Web.Enums;
using Web.Extentions;
using Web.Interfaces;
using Web.Models;

namespace Web.Implementation
{
    public class EquipmentRepository : IEquipmentRepository
    {
        private readonly DMADbContext _context;

        public EquipmentRepository(DMADbContext ctx) => _context = ctx;

        public async Task<IManufacturer> AddAsync(IManufacturer entity, EntityType type)
        {
            _context.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(int id, EntityType type)
        {
            var item = await GetByIdAsync(id, type);
            if (item == null) return false;

            _context.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IManufacturer?> GetByIdAsync(int id, EntityType type)
        {
            return type switch
            {
                EntityType.Adc => await _context.Set<Adc>().Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Id == id),
                EntityType.Player => await _context.Set<Player>().Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Id == id),
                EntityType.Amplifier => await _context.Set<Amplifier>().Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Id == id),
                EntityType.Cartridge => await _context.Set<Cartridge>().Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Id == id),
                EntityType.Wire => await _context.Set<Wire>().Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Id == id),
                _ => throw new ArgumentOutOfRangeException(nameof(type))
            };
        }

        public async Task<PagedResult<IManufacturer>> GetListAsync(int page, int pageSize, EntityType type)
        {
            IQueryable<IManufacturer> query = type switch
            {
                EntityType.Adc => _context.Set<Adc>().Include(x => x.Manufacturer),
                EntityType.Player => _context.Set<Player>().Include(x => x.Manufacturer),
                EntityType.Amplifier => _context.Set<Amplifier>().Include(x => x.Manufacturer),
                EntityType.Cartridge => _context.Set<Cartridge>().Include(x => x.Manufacturer),
                EntityType.Wire => _context.Set<Wire>().Include(x => x.Manufacturer),
                _ => throw new ArgumentOutOfRangeException(nameof(type))
            };

            return await query.ToPagedResultAsync(page, pageSize, x => x.Id);
        }

        public async Task<IManufacturer> UpdateAsync(IManufacturer entity, EntityType type)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
