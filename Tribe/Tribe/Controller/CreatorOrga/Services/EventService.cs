using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Tribe.Bib.Models.CommunityManagement;
using Tribe.Data;

namespace Tribe.Controller.CreatorOrga.Services
{
    public interface IEventService
    {
        Task<Event?> GetByIdAsync(string id);
        Task<IEnumerable<Event>> GetByOrganizationAsync(string organizationId);
        Task<Event> CreateAsync(Event ev);
        Task<bool> UpdateAsync(string id, Event ev);
        Task<bool> DeleteAsync(string id);
    }

    public class EventService : IEventService
    {
        private readonly OrgaDbContext _db;
        public EventService(OrgaDbContext db) => _db = db;

        public async Task<Event?> GetByIdAsync(string id) => await _db.Events.FindAsync(id);

        public async Task<IEnumerable<Event>> GetByOrganizationAsync(string organizationId) =>
            await _db.Events.AsNoTracking().Where(e => e.OrganizationId == organizationId).ToListAsync();

        public async Task<Event> CreateAsync(Event ev)
        {
            _db.Events.Add(ev);
            await _db.SaveChangesAsync();
            return ev;
        }

        public async Task<bool> UpdateAsync(string id, Event ev)
        {
            var existing = await _db.Events.FindAsync(id);
            if (existing == null) return false;
            existing.Title = ev.Title;
            existing.Description = ev.Description;
            existing.StartTime = ev.StartTime;
            existing.EndTime = ev.EndTime;
            existing.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var existing = await _db.Events.FindAsync(id);
            if (existing == null) return false;
            _db.Events.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}

