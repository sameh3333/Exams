using BL.Dtos;
using Exams.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Contracts
{
    public  interface IUserServices
    {

        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserRegusterDto> RegisterAsync(UserDto registerDto);
        Task<UserRegusterDto> LoginAsync(LoginDto loginDto);
        Task LogoutAsenc();
        Task<UserDto> GetUserByIdAsync(string userId);
        Task<UserDto> GetUserByEmailAsync(string Eamil);
        Task<IEnumerable<UserDto>> GetUserByIdAsync();
        Guid GetLoggedInServices();

    }

    }





