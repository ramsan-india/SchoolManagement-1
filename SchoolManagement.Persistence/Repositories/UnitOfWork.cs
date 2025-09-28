﻿using Microsoft.EntityFrameworkCore.Storage;
using SchoolManagement.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagement.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SchoolManagementDbContext _context;
        private IDbContextTransaction _transaction;
        private IAuthRepository _authRepository;
        //private IEmployeeRepository _employeeRepository;
        //private IUserRepository _userRepository;
        //private IRoleRepository _roleRepository;
        //private ICourseRepository _courseRepository;
        //private IStudentRepository _studentRepository;
        //private IAttendanceRepository _attendanceRepository;


        public IAuthRepository AuthRepository =>
            _authRepository ??= new AuthRepository(_context);

        public IEmployeeRepository EmployeeRepository => throw new NotImplementedException();

        public IUserRepository UserRepository => throw new NotImplementedException();

        public IRoleRepository RoleRepository => throw new NotImplementedException();

        public IStudentRepository StudentRepository => throw new NotImplementedException();

        public IAttendanceRepository AttendanceRepository => throw new NotImplementedException();

        public UnitOfWork(SchoolManagementDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }
    }
}
