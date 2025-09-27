using MediatR;
using SchoolManagement.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Students.Commands
{
    public class CreateStudentCommand : IRequest<CreateStudentResponse>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Gender { get; set; }
        public Guid ClassId { get; set; }
        public Guid SectionId { get; set; }
        public AddressDto Address { get; set; }
    }
}
