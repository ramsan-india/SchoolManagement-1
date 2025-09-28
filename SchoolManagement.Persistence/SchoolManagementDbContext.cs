using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Persistence
{
    public class SchoolManagementDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SchoolManagementDbContext(DbContextOptions<SchoolManagementDbContext> options,
                                     IHttpContextAccessor httpContextAccessor = null)
        : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        //public SchoolManagementDbContext(DbContextOptions<SchoolManagementDbContext> options)
        //    : base(options)
        //{ }

        #region DbSets

        // Master Data
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RoleMenuPermission> RoleMenuPermissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        // Academic
        public DbSet<Student> Students { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<ExamResult> ExamResults { get; set; }
        public DbSet<StudentParent> StudentParents { get; set; }
        public DbSet<FeePayment> FeePayments { get; set; }

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

        #endregion

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
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Menu)
                      .WithMany(m => m.RoleMenuPermissions)
                      .HasForeignKey(e => e.MenuId)
                      .OnDelete(DeleteBehavior.Restrict);
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
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Permission)
                      .WithMany(p => p.RolePermissions)
                      .HasForeignKey(e => e.PermissionId)
                      .OnDelete(DeleteBehavior.Restrict);
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
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Role)
                      .WithMany(r => r.UserRoles)
                      .HasForeignKey(e => e.RoleId)
                      .OnDelete(DeleteBehavior.Restrict);
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

                entity.OwnsOne(
                    e => e.Address,
                    a =>
                    {
                        a.ToTable("StudentAddresses");
                        a.Property(p => p.Street).HasMaxLength(200);
                        a.Property(p => p.City).HasMaxLength(50);
                        a.Property(p => p.State).HasMaxLength(50);
                        a.Property(p => p.Country).HasMaxLength(50);
                        a.Property(p => p.ZipCode).HasMaxLength(10);
                    });

                entity.OwnsOne(
                    e => e.BiometricInfo,
                    b =>
                    {
                        b.ToTable("StudentBiometricInfos");
                        b.Property(p => p.DeviceId).HasMaxLength(50);
                        b.Property(p => p.TemplateHash).HasMaxLength(500);
                    });

                entity.HasOne(s => s.Class)
                      .WithMany(c => c.Students)
                      .HasForeignKey(s => s.ClassId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.Section)
                      .WithMany(sec => sec.Students)
                      .HasForeignKey(s => s.SectionId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.Email).IsUnique();
            });
            #endregion

            #region StudentParent Configuration
            modelBuilder.Entity<StudentParent>(entity =>
            {
                entity.HasKey(sp => sp.Id);
                entity.Property(sp => sp.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(sp => sp.LastName).IsRequired().HasMaxLength(50);
                entity.Property(sp => sp.Email).HasMaxLength(100);
                entity.Property(sp => sp.Phone).HasMaxLength(20);
                entity.Property(sp => sp.Occupation).HasMaxLength(100);

                entity.HasOne(sp => sp.Student)
                      .WithMany(s => s.StudentParents)
                      .HasForeignKey(sp => sp.StudentId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.OwnsOne(
                    sp => sp.Address,
                    a =>
                    {
                        a.ToTable("StudentParentAddresses");
                        a.Property(p => p.Street).HasMaxLength(200);
                        a.Property(p => p.City).HasMaxLength(50);
                        a.Property(p => p.State).HasMaxLength(50);
                        a.Property(p => p.Country).HasMaxLength(50);
                        a.Property(p => p.ZipCode).HasMaxLength(10);
                    });
            });
            #endregion

            #region Attendance
            modelBuilder.Entity<Attendance>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Student)
                      .WithMany(s => s.Attendances)
                      .HasForeignKey(e => e.StudentId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.Property(e => e.CheckInTime).IsRequired();
                entity.Property(e => e.CheckOutTime);
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.Mode).IsRequired();
                entity.Property(e => e.DeviceId).HasMaxLength(50);
                entity.Property(e => e.Remarks).HasMaxLength(500);
                entity.HasIndex(e => new { e.StudentId, e.Date }).IsUnique();
            });
            #endregion

            #region FeePayment
            modelBuilder.Entity<FeePayment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(fp => fp.Student)
                      .WithMany(s => s.FeePayments)
                      .HasForeignKey(fp => fp.StudentId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            #endregion

            #region ExamResult
            modelBuilder.Entity<ExamResult>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(er => er.Student)
                      .WithMany(s => s.ExamResults)
                      .HasForeignKey(er => er.StudentId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            #endregion

            #region Section
            modelBuilder.Entity<Section>(entity =>
            {
                entity.HasOne(sec => sec.Class)
                      .WithMany(cls => cls.Sections)
                      .HasForeignKey(sec => sec.ClassId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            #endregion

            #region Employee Configuration
            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(d => d.Id);
                entity.Property(d => d.Name).IsRequired().HasMaxLength(100);
                entity.Property(d => d.Code).IsRequired().HasMaxLength(50);
                entity.HasMany(d => d.Employees)
                      .WithOne(e => e.Department)
                      .HasForeignKey(e => e.DepartmentId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(d => d.HeadOfDepartment)
                      .WithMany()
                      .HasForeignKey(d => d.HeadOfDepartmentId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);

                entity.OwnsOne(
                    e => e.Address,
                    a =>
                    {
                        a.ToTable("EmployeeAddresses");
                        a.Property(p => p.Street).HasMaxLength(200);
                        a.Property(p => p.City).HasMaxLength(50);
                        a.Property(p => p.State).HasMaxLength(50);
                        a.Property(p => p.Country).HasMaxLength(50);
                        a.Property(p => p.ZipCode).HasMaxLength(10);
                    });

                entity.OwnsOne(
                    e => e.SalaryInfo,
                    s =>
                    {
                        s.ToTable("EmployeeSalaryInfos");
                        s.Property(p => p.BasicSalary).HasColumnType("decimal(18,2)");
                        s.Property(p => p.HRA).HasColumnType("decimal(18,2)");
                        s.Property(p => p.SpecialAllowance).HasColumnType("decimal(18,2)");
                    });

                entity.OwnsOne(
                    e => e.BiometricInfo,
                    b =>
                    {
                        b.ToTable("EmployeeBiometricInfos");
                        b.Property(p => p.DeviceId).HasMaxLength(50);
                        b.Property(p => p.TemplateHash).HasMaxLength(500);
                    });
            });
            #endregion

            #region EmployeeAttendance
            modelBuilder.Entity<EmployeeAttendance>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Employee)
                      .WithMany(emp => emp.Attendances)
                      .HasForeignKey(e => e.EmployeeId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.Property(e => e.RegularHours).HasColumnType("decimal(5,2)");
                entity.Property(e => e.OvertimeHours).HasColumnType("decimal(5,2)");
                entity.HasIndex(e => new { e.EmployeeId, e.Date }).IsUnique();
            });
            #endregion

            #region LeaveApplication
            modelBuilder.Entity<LeaveApplication>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Employee)
                      .WithMany(emp => emp.LeaveApplications)
                      .HasForeignKey(e => e.EmployeeId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.ApprovedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.ApprovedBy)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.Property(e => e.Reason).HasMaxLength(500);
                entity.Property(e => e.ApprovalRemarks).HasMaxLength(500);
                entity.Property(e => e.DocumentPath).HasMaxLength(250);
            });
            #endregion

            #region PerformanceReview
            modelBuilder.Entity<PerformanceReview>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Employee)
                      .WithMany(emp => emp.PerformanceReviews)
                      .HasForeignKey(e => e.EmployeeId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Reviewer)
                      .WithMany()
                      .HasForeignKey(e => e.ReviewerId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.Property(e => e.Goals).HasMaxLength(1000);
                entity.Property(e => e.Achievements).HasMaxLength(1000);
                entity.Property(e => e.AreasOfImprovement).HasMaxLength(1000);
                entity.Property(e => e.ReviewerComments).HasMaxLength(1000);
                entity.Property(e => e.EmployeeComments).HasMaxLength(1000);
            });
            #endregion

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            ApplyAuditInfo();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditInfo();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyAuditInfo()
        {
            var entries = ChangeTracker
                .Entries<BaseEntity>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            var currentUser = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "System";
            var currentIp = _httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.SetCreated(currentUser, currentIp);
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.SetUpdated(currentUser, currentIp);
                }
            }
        }
    }
}
