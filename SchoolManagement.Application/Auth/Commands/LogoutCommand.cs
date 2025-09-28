﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Auth.Commands
{
    public class LogoutCommand : IRequest<bool>
    {
        public string RefreshToken { get; set; }
        public Guid UserId { get; set; }
    }
}
