﻿using GoPlay_App.Api.Controllers.UserController.Models;
using GoPlay_UserManagementService_Core.Business.Interfaces;
using GoPlay_UserManagementService_Core.Entities;
using GoPlay_UserManagementService_Core.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoPlay_App.Api.Controllers.UserController
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserManagerController : ControllerBase
    {
        private readonly IUserBusiness<UserEntity> _business;
        private readonly IUserRepository _repository;

        public UserManagerController(IUserBusiness<UserEntity> business, IUserRepository repository)
        {
            _business = business ?? throw new ArgumentNullException(nameof(business));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <summary>
        /// Adiciona um novo usuário
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
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
        /// Busca um usuário pelo UserName
        /// </summary>
        [HttpGet("GetByUserName/{userName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByUserName(string userName, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _business.GetByUserName(userName, cancellationToken);
                return Ok(entity);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Atualiza um usuário
        /// </summary>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] UserRequestBase<UserUpDateRequest> request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = request.Data.ToUserEntity();
                await _business.Update(entity, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Deleta um usuário
        /// </summary>
        [HttpDelete("{userName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(string userName, CancellationToken cancellationToken)
        {
            try
            {
                await _business.Delete(userName, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
