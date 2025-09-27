using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Services
{
    public class UserService : IUserService
    {
        //private readonly SchoolManagementDbContext _context;
        //private readonly IPasswordHasher<User> _passwordHasher;

        //public UserService(SchoolManagementDbContext context, IPasswordHasher<User> passwordHasher)
        //{
        //    _context = context;
        //    _passwordHasher = passwordHasher;
        //}

        //public async Task<User> GetByIdAsync(Guid id)
        //{
        //    return await _context.Users
        //        .Include(u => u.UserRoles)
        //        .ThenInclude(ur => ur.Role)
        //        .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        //}

        //public async Task<User> GetByUsernameAsync(string username)
        //{
        //    return await _context.Users
        //        .Include(u => u.UserRoles)
        //        .ThenInclude(ur => ur.Role)
        //        .FirstOrDefaultAsync(u => u.Username == username && !u.IsDeleted);
        //}

        //public async Task<User> GetByEmailAsync(string email)
        //{
        //    return await _context.Users
        //        .Include(u => u.UserRoles)
        //        .ThenInclude(ur => ur.Role)
        //        .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
        //}

        //public async Task<User> CreateAsync(User user, string password)
        //{
        //    // Use the domain method instead of directly setting PasswordHash
        //    var hashedPassword = _passwordHasher.HashPassword(user, password);
        //    user.UpdatePassword(hashedPassword);

        //    _context.Users.Add(user);
        //    await _context.SaveChangesAsync();
        //    return user;
        //}

        //public async Task<User> CreateAsync(User user)
        //{
        //    // For users created without password (e.g., external auth)
        //    _context.Users.Add(user);
        //    await _context.SaveChangesAsync();
        //    return user;
        //}

        //public async Task<User> UpdateAsync(User user)
        //{
        //    _context.Users.Update(user);
        //    await _context.SaveChangesAsync();
        //    return user;
        //}

        //public async Task<bool> ValidatePasswordAsync(Guid userId, string password)
        //{
        //    var user = await GetByIdAsync(userId);
        //    if (user == null) return false;

        //    var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        //    return result == PasswordVerificationResult.Success;
        //}

        //public async Task ChangePasswordAsync(Guid userId, string newPassword)
        //{
        //    var user = await GetByIdAsync(userId);
        //    if (user == null)
        //        throw new ArgumentException("User not found");

        //    // Use the domain method instead of directly setting PasswordHash
        //    var hashedPassword = _passwordHasher.HashPassword(user, newPassword);
        //    user.UpdatePassword(hashedPassword);

        //    await UpdateAsync(user);
        //}

        //public async Task AssignRoleAsync(Guid userId, Guid roleId, DateTime? expiresAt = null)
        //{
        //    var existingUserRole = await _context.UserRoles
        //        .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId && !ur.IsDeleted);

        //    if (existingUserRole != null)
        //    {
        //        if (expiresAt.HasValue)
        //            existingUserRole.ExtendRole(expiresAt.Value);
        //        return;
        //    }

        //    var userRole = new UserRole(userId, roleId, expiresAt);
        //    _context.UserRoles.Add(userRole);
        //    await _context.SaveChangesAsync();
        //}

        //public async Task RevokeRoleAsync(Guid userId, Guid roleId)
        //{
        //    var userRole = await _context.UserRoles
        //        .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId && !ur.IsDeleted);

        //    if (userRole != null)
        //    {
        //        userRole.DeactivateRole();
        //        _context.UserRoles.Update(userRole);
        //        await _context.SaveChangesAsync();
        //    }
        //}

        //public async Task<IEnumerable<Role>> GetUserRolesAsync(Guid userId)
        //{
        //    return await _context.UserRoles
        //        .Where(ur => ur.UserId == userId && ur.IsActive && !ur.IsExpired() && !ur.IsDeleted)
        //        .Include(ur => ur.Role)
        //        .Select(ur => ur.Role)
        //        .Where(r => r.IsActive)
        //        .ToListAsync();
        //}

        //public async Task<bool> DeleteAsync(Guid id)
        //{
        //    var user = await GetByIdAsync(id);
        //    if (user == null) return false;

        //    // Replace this with the correct method name based on your User entity implementation
        //    //user.Delete(); // or user.MarkAsDeleted(); or similar method
        //    await UpdateAsync(user);
        //    return true;
        //}

        //public async Task<User> ActivateUserAsync(Guid userId)
        //{
        //    var user = await GetByIdAsync(userId);
        //    if (user == null)
        //        throw new ArgumentException("User not found");

        //    user.SetActive(true);
        //    await UpdateAsync(user);
        //    return user;
        //}

        //public async Task<User> DeactivateUserAsync(Guid userId)
        //{
        //    var user = await GetByIdAsync(userId);
        //    if (user == null)
        //        throw new ArgumentException("User not found");

        //    user.SetActive(false);
        //    await UpdateAsync(user);
        //    return user;
        //}

        //public async Task<User> VerifyEmailAsync(Guid userId)
        //{
        //    var user = await GetByIdAsync(userId);
        //    if (user == null)
        //        throw new ArgumentException("User not found");

        //    user.VerifyEmail();
        //    await UpdateAsync(user);
        //    return user;
        //}

        //public async Task<User> VerifyPhoneAsync(Guid userId)
        //{
        //    var user = await GetByIdAsync(userId);
        //    if (user == null)
        //        throw new ArgumentException("User not found");

        //    user.VerifyPhone();
        //    await UpdateAsync(user);
        //    return user;
        //}

        //public async Task<User> UnlockUserAsync(Guid userId)
        //{
        //    var user = await GetByIdAsync(userId);
        //    if (user == null)
        //        throw new ArgumentException("User not found");

        //    user.UnlockAccount();
        //    await UpdateAsync(user);
        //    return user;
        //}

        //public async Task<IEnumerable<User>> GetAllAsync()
        //{
        //    return await _context.Users
        //        .Where(u => !u.IsDeleted)
        //        .Include(u => u.UserRoles)
        //        .ThenInclude(ur => ur.Role)
        //        .ToListAsync();
        //}

        //public async Task<IEnumerable<User>> GetByUserTypeAsync(UserType userType)
        //{
        //    return await _context.Users
        //        .Where(u => u.UserType == userType && !u.IsDeleted)
        //        .Include(u => u.UserRoles)
        //        .ThenInclude(ur => ur.Role)
        //        .ToListAsync();
        //}
        public Task<User> ActivateUserAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task AssignRoleAsync(Guid userId, Guid roleId, DateTime? expiresAt = null)
        {
            throw new NotImplementedException();
        }

        public Task ChangePasswordAsync(Guid userId, string newPassword)
        {
            throw new NotImplementedException();
        }

        public Task<User> CreateAsync(User user, string password)
        {
            throw new NotImplementedException();
        }

        public Task<User> CreateAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task<User> DeactivateUserAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<User> GetByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetByUsernameAsync(string username)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetByUserTypeAsync(UserType userType)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Role>> GetUserRolesAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task RevokeRoleAsync(Guid userId, Guid roleId)
        {
            throw new NotImplementedException();
        }

        public Task<User> UnlockUserAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<User> UpdateAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ValidatePasswordAsync(Guid userId, string password)
        {
            throw new NotImplementedException();
        }

        public Task<User> VerifyEmailAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<User> VerifyPhoneAsync(Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}