using MediatR;
using SchoolManagement.Application.Auth.Commands;
using SchoolManagement.Application.DTOs;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Auth.Handler
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;

        public LoginCommandHandler(
            IUnitOfWork unitOfWork,
            IPasswordService passwordService,
            ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _passwordService = passwordService;
            _tokenService = tokenService;
        }

        public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.AuthRepository.GetByEmailAsync(request.Email);
            if (user == null)
                throw new AuthenticationException("Invalid email or password");

            if (!user.IsActive)
                throw new AuthenticationException("Account is deactivated");

            if (!_passwordService.VerifyPassword(request.Password, user.PasswordHash))
                throw new AuthenticationException("Invalid email or password");

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshTokenValue = _tokenService.GenerateRefreshToken();

            var refreshToken = new RefreshToken(
                refreshTokenValue,
                DateTime.UtcNow.AddDays(7), // 7 days expiry
                user.Id);

            user.AddRefreshToken(refreshToken);
            user.RecordLogin();

            await _unitOfWork.AuthRepository.SaveRefreshTokenAsync(refreshToken);
            await _unitOfWork.AuthRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue,
                User = new UserDto
                {
                    Id = user.Id.ToString(),
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.UserRoles.ToString()
                }
            };
        }
    }
}
