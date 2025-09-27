using SchoolManagement.Domain.Entities;
using SchoolManagement.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Interfaces
{
    public interface IAttendanceRepository
    {
        Task<Attendance> CreateAsync(Attendance attendance);
        Task<IEnumerable<Attendance>> GetStudentAttendanceAsync(Guid studentId, DateTime fromDate, DateTime toDate);
        Task<IEnumerable<Attendance>> GetClassAttendanceAsync(Guid classId, DateTime date);
        Task<Attendance> GetTodayAttendanceAsync(Guid studentId, DateTime date);
        Task<AttendanceStatistics> GetAttendanceStatisticsAsync(Guid studentId, DateTime fromDate, DateTime toDate);
    }
}
