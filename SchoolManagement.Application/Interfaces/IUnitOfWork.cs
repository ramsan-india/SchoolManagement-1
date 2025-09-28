using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IAuthRepository AuthRepository { get; }
        IEmployeeRepository EmployeeRepository { get; }
        IUserRepository UserRepository { get; }
        IRoleRepository RoleRepository { get; }
        //ICourseRepository CourseRepository { get; }
        IStudentRepository StudentRepository { get; }
        IAttendanceRepository AttendanceRepository { get; }
        //ILeaveRepository LeaveRepository { get; }
        //IPerformanceRepository PerformanceRepository { get; }
        //IPayrollRepository PayrollRepository { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
