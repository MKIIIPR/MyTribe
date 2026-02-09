using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Tribe.Bib.Models.CommunityManagement;
using Tribe.Data;

namespace Tribe.Controller.CreatorOrga.Services
{
    public interface IRecruitmentService
    {
        Task<RecruitmentPost?> GetPostByIdAsync(string id);
        Task<IEnumerable<RecruitmentPost>> GetPostsByOrganizationAsync(string organizationId);
        Task<RecruitmentPost> CreatePostAsync(RecruitmentPost post);
        Task<bool> UpdatePostAsync(string id, RecruitmentPost post);
        Task<bool> DeletePostAsync(string id);

        Task<MemberApplication> ApplyAsync(MemberApplication application);
        Task<IEnumerable<MemberApplication>> GetApplicationsForPostAsync(string postId);
        Task<bool> ReviewApplicationAsync(string applicationId, string reviewerMemberId, string status, string? notes);
    }

    public class RecruitmentService : IRecruitmentService
    {
        private readonly OrgaDbContext _db;
        public RecruitmentService(OrgaDbContext db) => _db = db;

        public async Task<RecruitmentPost?> GetPostByIdAsync(string id) => await _db.RecruitmentPosts.FindAsync(id);

        public async Task<IEnumerable<RecruitmentPost>> GetPostsByOrganizationAsync(string organizationId) =>
            await _db.RecruitmentPosts.AsNoTracking().Where(p => p.OrganizationId == organizationId).ToListAsync();

        public async Task<RecruitmentPost> CreatePostAsync(RecruitmentPost post)
        {
            _db.RecruitmentPosts.Add(post);
            await _db.SaveChangesAsync();
            return post;
        }

        public async Task<bool> UpdatePostAsync(string id, RecruitmentPost post)
        {
            var existing = await _db.RecruitmentPosts.FindAsync(id);
            if (existing == null) return false;
            existing.Title = post.Title;
            existing.Description = post.Description;
            existing.RequirementsJson = post.RequirementsJson;
            existing.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePostAsync(string id)
        {
            var existing = await _db.RecruitmentPosts.FindAsync(id);
            if (existing == null) return false;
            _db.RecruitmentPosts.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<MemberApplication> ApplyAsync(MemberApplication application)
        {
            _db.MemberApplications.Add(application);
            await _db.SaveChangesAsync();
            return application;
        }

        public async Task<IEnumerable<MemberApplication>> GetApplicationsForPostAsync(string postId) =>
            await _db.MemberApplications.AsNoTracking().Where(a => a.RecruitmentPostId == postId).ToListAsync();

        public async Task<bool> ReviewApplicationAsync(string applicationId, string reviewerMemberId, string status, string? notes)
        {
            var app = await _db.MemberApplications.FindAsync(applicationId);
            if (app == null) return false;
            app.Status = status;
            app.ReviewerMemberId = reviewerMemberId;
            app.ReviewNotes = notes;
            app.ReviewedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
