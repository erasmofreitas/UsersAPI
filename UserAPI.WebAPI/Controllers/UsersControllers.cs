using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UserAPI.Domain;
using UserAPI.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using UserAPI.WebAPI.Dtos;

namespace UserAPI.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        public readonly IUserApiRepository _context;
        private readonly IMapper _mapper;
        public UsersController(IUserApiRepository context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;

        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var users = await _context.GetAllUsersAsync();

                var results = _mapper.Map<IEnumerable<UsersDto>>(users);

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
                var users = await _context.GetAllUsersAsyncById(UserId);

                var results = _mapper.Map<UsersDto>(users);

                return Ok(results);
            }
            catch (System.Exception)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
        }

        [HttpGet("getByNome/{nome}")]
        public async Task<IActionResult> Get(string nome)
        {
            try
            {
                var result = await _context.GetAllUsersAsyncByNome(nome);

                return Ok(result);
            }
            catch (System.Exception)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(UsersDto model)
        {
            try
            {

                var user = _mapper.Map<Users>(model);
                var today = DateTime.Today;
                var age = today.Year - user.BirthDate.Year;
                var ageMin = 18;

                if(age > ageMin)
                {
                    _context.Add(user);
                }
                else
                {
                    return BadRequest("Cadastro não permitido para menores de 18 anos");
                }

                if (await _context.SaveChangesAsync())
                {
                    return Created($"/users/{model.Name}", _mapper.Map<UsersDto>(user));

                }

            }
            catch (System.Exception ex)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Banco de Dados Falhou {ex.Message}");
            }

            return BadRequest();
        }

        [HttpPut("{UserId}")]
        public async Task<IActionResult> Put(int UserId, UsersDto model)
        {
            try
            {
                var user = await _context.GetAllUsersAsyncById(UserId);
                if (user == null) return NotFound();

                _mapper.Map(model, user);

                //model.Id = UserId;

                _context.Update(user);

                if (await _context.SaveChangesAsync())
                {
                    return Created($"/users/{model.Name}", _mapper.Map<UsersDto>(user));

                }

            }
            catch (System.Exception)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }

            return BadRequest();
        }

        [HttpDelete("{UserId}")]
        public async Task<IActionResult> Delete(int UserId)
        {
            try
            {
                var user = await _context.GetAllUsersAsyncById(UserId);
                if (user == null) return NotFound();

                _context.Delete(user);

                if (await _context.SaveChangesAsync())
                {
                    return Ok();

                }

            }
            catch (System.Exception)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }

            return BadRequest();
        }      

    }
}