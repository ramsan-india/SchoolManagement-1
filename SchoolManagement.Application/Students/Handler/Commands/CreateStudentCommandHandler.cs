using MediatR;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Application.Students.Commands;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;
using SchoolManagement.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Students.Handler.Commands
{
    public class MarkAttendanceCommandHandler : IRequestHandler<CreateStudentCommand, CreateStudentResponse>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public MarkAttendanceCommandHandler(
            IStudentRepository studentRepository,
            IUnitOfWork unitOfWork,
            INotificationService notificationService)
        {
            _studentRepository = studentRepository;
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task<CreateStudentResponse> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var address = new Address(
                    request.Address.Street,
                    request.Address.City,
                    request.Address.State,
                    request.Address.Country,
                    request.Address.ZipCode);

                var student = new Student(
                    request.FirstName,
                    request.LastName,
                    request.Email,
                    request.DateOfBirth,
                    (Gender)request.Gender,
                    request.ClassId,
                    request.SectionId);

                student.UpdatePersonalInfo(request.FirstName, request.LastName,
                                         request.MiddleName, request.Phone, address);

                var createdStudent = await _studentRepository.CreateAsync(student);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync();

                // Send welcome notification
                await _notificationService.SendSMSAsync(request.Phone,
                    $"Welcome to our school! Student ID: {createdStudent.StudentId}");

                return new CreateStudentResponse
                {
                    Id = createdStudent.Id,
                    StudentId = createdStudent.StudentId,
                    Message = "Student created successfully",
                    Success = true
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new CreateStudentResponse
                {
                    Message = $"Error creating student: {ex.Message}",
                    Success = false
                };
            }
        }
    }
}
