using BL.Contracts;
using BL.Dtos;
using DAL.Context;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Exams.Models
{
    public class UserServices : IUserServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
       private readonly ExamsContext _context;
        public UserServices(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor
             ,ExamsContext Contect) {
            _userManager=userManager;
            _signInManager=signInManager;
            _httpContextAccessor=httpContextAccessor;
            _context = Contect;
        }


        public async Task<UserRegusterDto> RegisterAsync(UserDto registerDto)
        {
            if (registerDto.Password != registerDto.ConfirmPassword)
            {
                return new UserRegusterDto
                {
                    Success = false,
                    Errors = new[] { "password do not match" }

                };
            }
        var user = new ApplicationUser { UserName = registerDto.Email, Email = registerDto.Email ,FirstName=registerDto.FirstName
        ,LastName=registerDto.LastName,PhoneNumber=registerDto.PhoneNumber};

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            var rolerName = string.IsNullOrEmpty(registerDto.Role) ? "User" : registerDto.Role;
            var roleResult = await _userManager.AddToRoleAsync(user, rolerName);
           
         
            if (!roleResult.Succeeded)
            {
                return new UserRegusterDto
                {
                    Success = false,
                    Errors = roleResult.Errors?.Select(e => e.Description)

                };
            }

            return new UserRegusterDto
            {
                Success = result.Succeeded,
                Errors = result.Errors?.Select(e => e.Description)
            };

        }
        public async Task<UserRegusterDto> LoginAsync(LoginDto loginDto)
        {
            var result = await _signInManager.PasswordSignInAsync(loginDto.Email,loginDto.Password, true,false);
            if (!result.Succeeded)
            {
                return new UserRegusterDto
                {
                    Success = false,
                    Errors = new[] { " Invaild Login attempt." }

                };
            }
            return new UserRegusterDto { Success = true, Token = "DummyTokenForNow" };

        }

        public async Task LogoutAsenc()
        {
            await _signInManager.SignOutAsync();
        }


        public async Task<UserDto> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            return new UserDto
            {
                Id=Guid.Parse(user.Id),
                Email=user.Email,
            };

        }



        public Guid GetLoggedInServices()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            // var refrsshToken = _IRefreshTokens.GetByToken(refrsshToken);
            return Guid.Parse(userId);
        }

        public async Task<UserDto> GetUserByEmailAsync(string Eamil)
        {

            // 🔍 يبحث عن اليوزر بالـ Email
            var user = await _userManager.FindByEmailAsync(Eamil);

            if (user == null)
                return null;

            // 🔍 يجيب كل الـ Roles الخاصة باليوزر
            var roles =await  _userManager.GetRolesAsync(user);

            // 🔄 يرجع DTO فيه البيانات اللي محتاجها
            return new UserDto
            {
                Id = Guid.Parse(user.Id),      // لان Id في Identity بيكون string
                Email = user.Email,
                Role = roles.FirstOrDefault()  // بيرجع أول Role (مثلاً "Admin" أو "User")
            };
        }

       
        public async Task<IEnumerable<UserDto>> GetUserByIdAsync()
        {
            var users = _userManager.Users;
            return users.Select(u => new UserDto
            {
                Id = Guid.Parse(u.Id),
                Email = u.Email,
            });
        }




        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            return  _context.Users
                .Select(u => new UserDto
                {
                    Id = Guid.Parse(u.Id),
                    Email = u.Email ?? "❌ No Email",
                    FirstName = u.FirstName ?? string.Empty,
                    LastName = u.LastName ?? string.Empty,
                    PhoneNumber = u.PhoneNumber ?? string.Empty,
                    Role = (from ur in _context.UserRoles
                            join r in _context.Roles on ur.RoleId equals r.Id
                            where ur.UserId == u.Id
                            select r.Name).FirstOrDefault() ?? "User"
                })
                .ToList();
        }



    }
}
