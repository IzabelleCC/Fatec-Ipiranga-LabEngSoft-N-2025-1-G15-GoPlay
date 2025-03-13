﻿using GoPlay_UserManagementService_App.Api.Controllers.Models;
using GoPlay_UserManagementService_Core.Business.Interfaces;
using GoPlay_UserManagementService_Core.Repository.Interfaces;
using GoPlay_UserManagementService_Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoPlay_UserManagementService_App.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserBusiness _business;
        private readonly IUserRepository _repository;
        private readonly UserService _service;

        public UserController(IUserBusiness business, IUserRepository repository, UserService service)
        {
            _business = business ?? throw new ArgumentNullException(nameof(business));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>
        /// Adiciona Usuário
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Add([FromBody] UserRequestBase<UserCreateRequest> request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = request.Data.ToUserEntity();
                await _business.Add(entity, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Realiza Login do Usuário
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Login(UserRequestBase<UserLoginRequest> request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = request.Data.ToLoginEntity();
                var token = await _service.Login(entity);

                return Ok(token);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Busca um usuário pelo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _repository.GetById(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Atualiza um usuário
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(int id, [FromBody] UserRequestBase<UserUpDateRequest> request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = request.Data.ToUserEntity();
                await _business.Update(entity, id, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _repository.GetById(id);
                await _business.Delete(entity, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
