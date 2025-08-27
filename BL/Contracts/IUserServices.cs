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
       
            //Task<UserRegusterDto> RegisterAsync(UserDto model);
            //Task<UserRegusterDto> LoginAsync(LoginDto model);
            //Task LogoutAsync(); // ✅ تسجيل الخروج
            //Task<List<UserDto>> GetAllUsersAsync();
            //Task<UserDto?> GetUserByIdAsync(Guid id);
            //Task<bool> DeleteUserAsync(Guid id);
            //Task<IdentityResult> UpdateUserAsync(Guid id, UpdateUserDto model);
            //Task<IdentityResult> AssignRoleAsync(Guid userId, string role);
            //Task<IList<string>> GetUserRolesAsync(Guid userId);

        Task<UserRegusterDto> RegisterAsync(UserDto registerDto);
        Task<UserRegusterDto> LoginAsync(LoginDto loginDto);
        Task LogoutAsenc();
        Task<UserDto> GetUserByIdAsync(string userId);
        Task<UserDto> GetUserByEmailAsync(string Eamil);
        Task<IEnumerable<UserDto>> GetUserByIdAsync();
        Guid GetLoggedInServices();

    }

    }





