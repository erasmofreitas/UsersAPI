using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UserAPI.Domain.Identity;
using UserAPI.Repository;
using UserAPI.WebAPI.Dtos;

namespace UserAPI.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        public readonly IUserApiRepository _context;
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public SignInManager<User> _signInManager;

        public UsersController(IConfiguration config,
                                UserManager<User> userManager,
                                SignInManager<User> signInManager,
                                IMapper mapper)
        {
            _signInManager = signInManager;
            _mapper = mapper;
            _config = config;
            _userManager = userManager;
        }


        [HttpGet("GetUser")]        
        public async Task<IActionResult> GetUser()
        {
            try
            {
                //var users = await _context.GetAllUsersAsync();
                var users = await _userManager.Users.ToListAsync();

                var results = _mapper.Map<IEnumerable<UserDto>>(users);

                return Ok(results);
            }
             catch (System.Exception ex)
             {
                 return this.StatusCode(StatusCodes.Status500InternalServerError, $"Banco de Dados Falhou {ex.Message}");
             }
        }

        [HttpGet("{UserId}")]
        public async Task<IActionResult> Get(int UserId)
        {
            try
            {
                var users = await _userManager.FindByIdAsync(UserId.ToString());

                var results = _mapper.Map<UserDto>(users);
                if (users != null)
                {
                    return Ok(results);
                }

                return Unauthorized();
            }
            catch (System.Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Banco de Dados Falhou {ex.Message}");
            }
        }

        [HttpGet("getByNome/{nome}")]
        public async Task<IActionResult> Get(string nome)
        {
            try
            {
                var users = await _userManager.FindByNameAsync(nome);

                var results = _mapper.Map<UserDto>(users);
                if (users != null)
                {
                    return Ok(results);
                }

                return Unauthorized();
            }
            catch (System.Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Banco de Dados Falhou {ex.Message}");
            }
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserDto userIdentityDto)
        {
            try
            {
                var user = _mapper.Map<User>(userIdentityDto);
                var result = await _userManager.CreateAsync(user, userIdentityDto.Password);

                var today = DateTime.Today;
                var age = today.Year - user.BirthDate.Year;

                if(age < 18)
                {
                    return BadRequest(new {message = "Registro permitido apenas para maiores de 18 anos."});
                }

                var userToReturn = _mapper.Map<UserDto>(user);

                if (result.Succeeded)
                {
                    return Created("GetUser", userToReturn);
                }

                return BadRequest(result.Errors);
            }
            catch (System.Exception ex)
            {
                
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Banco de Dados Falhou {ex.Message}");
            }
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginDto userIdentityDto)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userIdentityDto.UserName);

                var result = await _signInManager.CheckPasswordSignInAsync(user, userIdentityDto.Password, false);

                if (result.Succeeded)
                {
                    var appUser = await _userManager.Users
                        .FirstOrDefaultAsync(u => u.NormalizedUserName == userIdentityDto.UserName.ToUpper());
                    
                    var userToReturn = _mapper.Map<UserLoginDto>(appUser);

                    return Ok(new {
                        token = GenereteJWToken(appUser).Result,
                        user = userToReturn
                    });
                }

                return Unauthorized();
            }
            catch (System.Exception ex)
            {
                
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Banco de Dados Falhou {ex.Message}");
            }
        }

        [HttpPut("{UserId}")]
        public async Task<IActionResult> Put(int UserId, UserDto model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(UserId.ToString());
                if (user == null) return NotFound();
                user.UserName = model.UserName;
                user.Name = model.Name;
                user.PasswordHash = _userManager.PasswordHasher.HashPassword(user,model.Password);
                user.FullName = model.Fullname;
                user.PhoneNumber = model.Phone.ToString();

                var result = await _userManager.UpdateAsync(user);                

                if (result.Succeeded)
                {
                    return Created($"/users/{model.Name}", _mapper.Map<UserDto>(user));

                }

            }
            catch (System.Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Banco de Dados Falhou {ex.Message}");
            }

            return BadRequest();
        }

        [HttpDelete("{UserId}")]
        public async Task<IActionResult> Delete(int UserId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(UserId.ToString());
                if (user == null) return NotFound();

               var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return Ok();
                }

            }
            catch (System.Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Banco de Dados Falhou {ex.Message}");                
            }

            return BadRequest();
        }      
       private async Task<string> GenereteJWToken(User userIdentity)
        {

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userIdentity.Id.ToString()),
                new Claim(ClaimTypes.Name, userIdentity.UserName)

            };

            var roles = await _userManager.GetRolesAsync(userIdentity);

            foreach(var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.ASCII
                .GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}