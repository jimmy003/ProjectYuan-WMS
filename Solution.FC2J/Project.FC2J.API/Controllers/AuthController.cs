using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Project.FC2J.DataStore.Interfaces;
using Project.FC2J.Models.Dtos;
using Project.FC2J.Models.Token;
using Project.FC2J.Models.User;

namespace Project.FC2J.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;

        public AuthController(IAuthRepository repository, IConfiguration config)
        {
            _repo = repository;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            if(await _repo.UserExists(userForRegisterDto.Username))
            {
                return BadRequest("Username already exists");
            }

            var userToCreate = new User
            {
                UserName = userForRegisterDto.Username
            };

            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);

            return StatusCode(201);
        }

        [AllowAnonymous]
        [HttpPost("hash")]
        public async Task<IActionResult> Hash(UserForLoginDto userForLoginDto)
        {
            var value = await _repo.GetHash(userForLoginDto);

            return Ok(value);

        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var userFromRepo = await _repo.Login(userForLoginDto);

            if (userFromRepo == null) return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.UserName),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var setting = new Setting
            {
                NearDue = _config.GetSection("AppSettings:NearDue").Value
            };

            return Ok(new { Token = tokenHandler.WriteToken(token), User = userFromRepo , Setting = setting } );

        }

        [AllowAnonymous]
        [HttpPost("token")]
        public async Task<IActionResult> Token(UserForLoginDto userForLoginDto)
        {
            var userFromRepo = await _repo.ValidateUser(userForLoginDto);
            if (userFromRepo == null) return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.UserName),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new { Token = tokenHandler.WriteToken(token) });

        }

        [HttpGet("getuser")]
        public async Task<IActionResult> GetUserByUserName(string userName)
        {
            var user = await _repo.GetUserByUserNameAsync(userName);
            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var list = await _repo.GetList();
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Save(User user)
        {

            user.PasswordX = _config.GetSection("AppSettings:DefaultPassword").Value;
            var result = await _repo.Save(user);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update(User user)
        {
            await _repo.Update(user);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Remove(long id)
        {
            await _repo.Remove(id);
            return Ok();
        }

    }
}
