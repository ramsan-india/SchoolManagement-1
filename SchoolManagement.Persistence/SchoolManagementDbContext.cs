using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Persistence
{
    public class SchoolManagementDbContext : DbContext
    {
        public SchoolManagementDbContext(DbContextOptions<SchoolManagementDbContext> options)
            : base(options)
        { }

        // Master Data
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RoleMenuPermission> RoleMenuPermissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<User> Users { get; set; }

        // Academic
        public DbSet<Student> Students { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<ExamResult> ExamResults { get; set; }

        // Employee & Payroll
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeAttendance> EmployeeAttendances { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Designation> Designations { get; set; }
        public DbSet<LeaveApplication> LeaveApplications { get; set; }
        public DbSet<PayrollRecord> PayrollRecords { get; set; }
        public DbSet<PerformanceReview> PerformanceReviews { get; set; }
        public DbSet<Allowance> Allowances { get; set; }
        public DbSet<Deduction> Deductions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Menu Configuration
            modelBuilder.Entity<Menu>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.DisplayName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Icon).HasMaxLength(50);
                entity.Property(e => e.Route).HasMaxLength(200);
                entity.Property(e => e.Component).HasMaxLength(100);

                entity.HasOne(e => e.ParentMenu)
                      .WithMany(e => e.SubMenus)
                      .HasForeignKey(e => e.ParentMenuId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasIndex(e => new { e.ParentMenuId, e.SortOrder });
            });
            #endregion

            #region Role Configuration
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.DisplayName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);

                entity.HasIndex(e => e.Name).IsUnique();
            });
            #endregion

            #region Permission Configuration
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.DisplayName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Module).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Action).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Resource).IsRequired().HasMaxLength(50);

                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasIndex(e => new { e.Module, e.Action, e.Resource }).IsUnique();
            });
            #endregion

            #region RoleMenuPermission Configuration
            modelBuilder.Entity<RoleMenuPermission>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Role)
                      .WithMany(r => r.RoleMenuPermissions)
                      .HasForeignKey(e => e.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Menu)
                      .WithMany(m => m.RoleMenuPermissions)
                      .HasForeignKey(e => e.MenuId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.RoleId, e.MenuId }).IsUnique();
            });
            #endregion

            #region RolePermission Configuration
            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Role)
                      .WithMany(r => r.RolePermissions)
                      .HasForeignKey(e => e.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Permission)
                      .WithMany(p => p.RolePermissions)
                      .HasForeignKey(e => e.PermissionId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.RoleId, e.PermissionId }).IsUnique();
            });
            #endregion

            #region UserRole Configuration
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.User)
                      .WithMany(u => u.UserRoles)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Role)
                      .WithMany(r => r.UserRoles)
                      .HasForeignKey(e => e.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.UserId, e.RoleId }).IsUnique();
            });
            #endregion

            #region User Configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(500);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);

                entity.HasOne(e => e.Student)
                      .WithOne()
                      .HasForeignKey<User>(e => e.StudentId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Employee)
                      .WithOne()
                      .HasForeignKey<User>(e => e.EmployeeId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });
            #endregion

            #region Student Configuration
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.StudentId).IsRequired().HasMaxLength(20);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);

                entity.OwnsOne(e => e.Address, address =>
                {
                    address.Property(a => a.Street).HasMaxLength(200);
                    address.Property(a => a.City).HasMaxLength(50);
                    address.Property(a => a.State).HasMaxLength(50);
                    address.Property(a => a.Country).HasMaxLength(50);
                    address.Property(a => a.ZipCode).HasMaxLength(10);
                });

                entity.OwnsOne(e => e.BiometricInfo, bio =>
                {
                    bio.Property(b => b.TemplateHash).HasMaxLength(500);
                    bio.Property(b => b.DeviceId).HasMaxLength(50);
                });

                entity.HasIndex(e => e.Email).IsUnique();
            });
            #endregion

            #region Attendance Configuration
            modelBuilder.Entity<Attendance>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Student)
                      .WithMany(s => s.Attendances)
                      .HasForeignKey(e => e.StudentId);

                entity.Property(e => e.CheckInTime).IsRequired();
                entity.Property(e => e.CheckOutTime);
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.Mode).IsRequired();
                entity.Property(e => e.DeviceId).HasMaxLength(50);
                entity.Property(e => e.Remarks).HasMaxLength(500);

                entity.HasIndex(e => new { e.StudentId, e.Date }).IsUnique();
            });
            #endregion

            #region EmployeeAttendance Configuration
            modelBuilder.Entity<EmployeeAttendance>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Employee)
                      .WithMany(emp => emp.Attendances)
                      .HasForeignKey(e => e.EmployeeId);

                entity.Property(e => e.RegularHours).HasColumnType("decimal(5,2)");
                entity.Property(e => e.OvertimeHours).HasColumnType("decimal(5,2)");

                entity.HasIndex(e => new { e.EmployeeId, e.Date }).IsUnique();
            });
            #endregion

            base.OnModelCreating(modelBuilder);
        }
    }
}
