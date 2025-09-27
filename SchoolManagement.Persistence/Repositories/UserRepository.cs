using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagement.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SchoolManagementDbContext _context;

        public UserRepository(SchoolManagementDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == username && !u.IsDeleted);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
        }

        public async Task<User> CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task AssignRoleAsync(Guid userId, Guid roleId, DateTime? expiresAt = null)
        {
            var existingUserRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId && !ur.IsDeleted);

            if (existingUserRole != null)
            {
                if (expiresAt.HasValue)
                    existingUserRole.ExtendRole(expiresAt.Value);
                return;
            }

            var userRole = new UserRole(userId, roleId, expiresAt);
            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();
        }

        public async Task RevokeRoleAsync(Guid userId, Guid roleId)
        {
            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId && !ur.IsDeleted);

            if (userRole != null)
            {
                userRole.DeactivateRole();
                _context.UserRoles.Update(userRole);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Role>> GetUserRolesAsync(Guid userId)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId && ur.IsActive && !ur.IsExpired() && !ur.IsDeleted)
                .Include(ur => ur.Role)
                .Select(ur => ur.Role)
                .Where(r => r.IsActive)
                .ToListAsync();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var user = await GetByIdAsync(id);
            if (user == null) return false;

            // Use the actual soft delete method from your User entity
            //user.Delete(); // Replace with your actual method name (SoftDelete, MarkAsDeleted, etc.)
            await UpdateAsync(user);
            return true;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users
                .Where(u => !u.IsDeleted)
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetByUserTypeAsync(UserType userType)
        {
            return await _context.Users
                .Where(u => u.UserType == userType && !u.IsDeleted)
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ToListAsync();
        }
    }
}