﻿using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagement.Persistence.Repositories
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly SchoolManagementDbContext _context;

        public AttendanceRepository(SchoolManagementDbContext context)
        {
            _context = context;
        }

        public async Task<Attendance> CreateAsync(Attendance attendance)
        {
            _context.Attendances.Add(attendance);
            return attendance;
        }

        public async Task<IEnumerable<Attendance>> GetStudentAttendanceAsync(Guid studentId, DateTime fromDate, DateTime toDate)
        {
            return await _context.Attendances
                .Where(a => a.StudentId == studentId &&
                           a.Date >= fromDate.Date &&
                           a.Date <= toDate.Date &&
                           !a.IsDeleted)
                .Include(a => a.Student)
                .OrderBy(a => a.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Attendance>> GetClassAttendanceAsync(Guid classId, DateTime date)
        {
            return await _context.Attendances
                .Include(a => a.Student)
                .Where(a => a.Student.ClassId == classId &&
                           a.Date.Date == date.Date &&
                           !a.IsDeleted)
                .ToListAsync();
        }

        public async Task<Attendance> GetTodayAttendanceAsync(Guid studentId, DateTime date)
        {
            return await _context.Attendances
                .FirstOrDefaultAsync(a => a.StudentId == studentId &&
                                        a.Date.Date == date.Date &&
                                        !a.IsDeleted);
        }

        public async Task<AttendanceStatistics> GetAttendanceStatisticsAsync(Guid studentId, DateTime fromDate, DateTime toDate)
        {
            var attendances = await GetStudentAttendanceAsync(studentId, fromDate, toDate);
            var totalDays = attendances.Count();
            var presentDays = attendances.Count(a => a.Status == AttendanceStatus.Present || a.Status == AttendanceStatus.Late);
            var absentDays = attendances.Count(a => a.Status == AttendanceStatus.Absent);
            var lateDays = attendances.Count(a => a.Status == AttendanceStatus.Late);

            return new AttendanceStatistics
            {
                TotalDays = totalDays,
                PresentDays = presentDays,
                AbsentDays = absentDays,
                LateDays = lateDays,
                AttendancePercentage = totalDays > 0 ? (decimal)presentDays / totalDays * 100 : 0
            };
        }
    }
}
