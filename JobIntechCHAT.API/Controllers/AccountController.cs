using JobIntechCHAT.API.Data;
using JobIntechCHAT.API.DTOs;
using JobIntechCHAT.API.Interfaces;
using JobIntechCHAT.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace JobIntechCHAT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly AppDbContext _appDbContext;
        private readonly ITokenService _tokenService;

        public AccountController(AppDbContext appDb, ITokenService tokenService)
        {
            _appDbContext = appDb;
            _tokenService = tokenService;

        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Inscription(RegisterDto registerDto)
        {
            if (await CheckExist(registerDto.Username)) return BadRequest("username taken");

            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };
             _appDbContext.Users.Add(user);
             await _appDbContext.SaveChangesAsync();

            return new UserDto
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        private async Task<bool> CheckExist(string username)
        {
            return await _appDbContext.Users.AnyAsync(e => e.UserName == username.ToLower());
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.UserName == loginDto.Username);
            if (user == null)
            {
                return Unauthorized("Invalid username");
            }
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            if (!computedHash.SequenceEqual(user.PasswordHash))
            {
                return Unauthorized("Invalid password");
            }
            return new UserDto
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user)
            };

        }

        //[HttpPost("loginAncienne")]
        //public async Task<ActionResult<AppUser>> Login(LoginDto loginDto)
        //{
        //    var user = await _appDbContext.Users.SingleOrDefaultAsync(u => u.UserName == loginDto.Username);

        //    if(user == null) return Unauthorized("invalid username");
            
        //    using var hmac = new HMACSHA512(user.PasswordSalt);

        //    var computehash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        //    for(int i = 0; i < computehash.Length; i++)
        //    {
        //        if (computehash[i] != user.PasswordHash[i])
        //        {
        //            return Unauthorized("invalid password");
        //        }
        //    }
        //    return user;
            
        //}

    }
}
