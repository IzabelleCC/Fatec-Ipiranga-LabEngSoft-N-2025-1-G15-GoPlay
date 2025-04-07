using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using GoPlay_Core.Business.Interfaces;
using GoPlay_Core.Entities;
using GoPlay_Core.Exceptions;
using GoPlay_Core.Services.Interfaces;
using GoPlay_App.Api.Controllers.UserController;
using GoPlay_App.Api.Controllers.UserController.Models;
using GoPlay_App.Api.Controllers.AccessManager.Models;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using NUnit.Framework;
using GoPlay_App.Api.Controllers;

namespace GoPlay_Test.Api
{
    [TestFixture]
    public class UserManagerControllerTests
    {
        private Mock<IUserBusiness<UserEntity>> _mockBusiness;
        private Mock<IEmailService> _mockEmailService;
        private Mock<UserManager<UserEntity>> _mockUserManager;
        private Mock<IConfiguration> _mockConfiguration;
        private UserManagerController _controller;

        [SetUp]
        public void Setup()
        {
            _mockBusiness = new Mock<IUserBusiness<UserEntity>>();
            _mockEmailService = new Mock<IEmailService>();
            _mockUserManager = CreateMockUserManager();
            _mockConfiguration = new Mock<IConfiguration>();

            _controller = new UserManagerController(
                _mockBusiness.Object,
                _mockEmailService.Object,
                _mockUserManager.Object,
                _mockConfiguration.Object
            );
        }

        private Mock<UserManager<UserEntity>> CreateMockUserManager()
        {
            var store = new Mock<IUserStore<UserEntity>>();
            return new Mock<UserManager<UserEntity>>(
                store.Object,
                null,
                new PasswordHasher<UserEntity>(),
                new List<IUserValidator<UserEntity>>(),
                new List<IPasswordValidator<UserEntity>>(),
                null,
                null,
                null,
                null
            );
        }

        #region Add

        [Test]
        public async Task Add_ReturnsOk_WhenUserIsCreated()
        {
            var request = new UserRequestBase<UserCreateRequest>
            {
                Data = new UserCreateRequest("Test Name", "test@example.com", "testuser", "Password123!", 1, null, "12345678901", null, null, null, null)
            };

            _mockBusiness.Setup(b => b.Add(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _mockEmailService.Setup(e => e.SendEmailRegisterAsync(It.IsAny<UserEntity>())).Returns(Task.CompletedTask);

            var result = await _controller.Add(request, CancellationToken.None);

            result.Should().BeOfType<OkObjectResult>()
                  .Which.StatusCode.Should().Be(200);
        }

        [Test]
        public async Task Add_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            var request = new UserRequestBase<UserCreateRequest>
            {
                Data = new UserCreateRequest("Test Name", "test@example.com", "testuser", "Password123!", 1, null, "12345678901", null, null, null, null)
            };

            _mockBusiness.Setup(b => b.Add(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()))
                         .Throws(new Exception("Test Exception"));

            var result = await _controller.Add(request, CancellationToken.None);

            result.Should().BeOfType<ObjectResult>()
                  .Which.StatusCode.Should().Be(500);

            var message = ((ObjectResult)result).Value?.GetType().GetProperty("message")?.GetValue(((ObjectResult)result).Value);
            message.Should().Be("Test Exception");
        }

        #endregion

        #region GetByUserName

        [Test]
        public async Task GetByUserName_ReturnsUser_WhenExists()
        {
            var userName = "testuser";
            var userEntity = new UserEntity { UserName = userName };

            _mockBusiness.Setup(b => b.GetByUserName(userName, It.IsAny<CancellationToken>())).ReturnsAsync(userEntity);

            var result = await _controller.GetByUserName(userName, CancellationToken.None);

            result.Should().BeOfType<OkObjectResult>()
                  .Which.Value.Should().Be(userEntity);
        }

        [Test]
        public async Task GetByUserName_ReturnsNotFound_WhenUserDoesNotExist()
        {
            var userName = "nonexistentuser";

            _mockBusiness.Setup(b => b.GetByUserName(userName, It.IsAny<CancellationToken>())).ReturnsAsync((UserEntity)null);

            var result = await _controller.GetByUserName(userName, CancellationToken.None);

            result.Should().BeOfType<NotFoundObjectResult>()
                  .Which.StatusCode.Should().Be(404);
        }

        [Test]
        public async Task GetByUserName_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            var userName = "testuser";

            _mockBusiness.Setup(b => b.GetByUserName(userName, It.IsAny<CancellationToken>())).Throws(new Exception("Test Exception"));

            var result = await _controller.GetByUserName(userName, CancellationToken.None);

            result.Should().BeOfType<ObjectResult>()
                  .Which.StatusCode.Should().Be(500);
        }

        #endregion

        #region Update

        [Test]
        public async Task Update_ReturnsOk_WhenUserIsUpdated()
        {
            var request = new UserRequestBase<UserUpDateRequest>
            {
                Data = new UserUpDateRequest("1", "Updated Name", "updateduser", null, null, null, null, null)
            };

            _mockBusiness.Setup(b => b.Update(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var result = await _controller.Update(request, CancellationToken.None);

            result.Should().BeOfType<OkObjectResult>()
                  .Which.StatusCode.Should().Be(200);
        }

        [Test]
        public async Task Update_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            var request = new UserRequestBase<UserUpDateRequest>
            {
                Data = new UserUpDateRequest("1", "Updated Name", "updateduser", null, null, null, null, null)
            };

            _mockBusiness.Setup(b => b.Update(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()))
                         .Throws(new Exception("Test Exception"));

            var result = await _controller.Update(request, CancellationToken.None);

            result.Should().BeOfType<ObjectResult>()
                  .Which.StatusCode.Should().Be(500);
        }

        #endregion

        #region Delete

        [Test]
        public async Task Delete_ReturnsOk_WhenUserIsDeleted()
        {
            var userName = "testuser";

            _mockBusiness.Setup(b => b.Delete(userName, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var result = await _controller.Delete(userName, CancellationToken.None);

            result.Should().BeOfType<OkObjectResult>()
                  .Which.StatusCode.Should().Be(200);
        }

        [Test]
        public async Task Delete_ReturnsNotFound_WhenUserDoesNotExist()
        {
            var userName = "nonexistentuser";

            _mockBusiness.Setup(b => b.Delete(userName, It.IsAny<CancellationToken>()))
                         .Throws(new NotFoundException("Usuário não encontrado."));

            var result = await _controller.Delete(userName, CancellationToken.None);

            result.Should().BeOfType<NotFoundObjectResult>()
                  .Which.StatusCode.Should().Be(404);

            var message = ((NotFoundObjectResult)result).Value?.GetType().GetProperty("message")?.GetValue(((NotFoundObjectResult)result).Value);
            message.Should().Be("Usuário não encontrado.");
        }

        [Test]
        public async Task Delete_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            var userName = "testuser";

            _mockBusiness.Setup(b => b.Delete(userName, It.IsAny<CancellationToken>()))
                         .Throws(new Exception("Test Exception"));

            var result = await _controller.Delete(userName, CancellationToken.None);

            result.Should().BeOfType<ObjectResult>()
                  .Which.StatusCode.Should().Be(500);
        }

        #endregion

        #region EmailConfirmation

        [Test]
        public async Task EmailConfirmation_ReturnsOk_WhenEmailIsConfirmed()
        {
            var email = "test@example.com";
            var token = "token";
            var user = new UserEntity { Email = email };

            _mockUserManager.Setup(u => u.FindByEmailAsync(email)).ReturnsAsync(user);
            _mockUserManager.Setup(u => u.ConfirmEmailAsync(user, token)).ReturnsAsync(IdentityResult.Success);

            var result = await _controller.EmailConfirmation(email, token, CancellationToken.None);

            result.Should().BeOfType<OkObjectResult>()
                  .Which.StatusCode.Should().Be(200);
        }

        [Test]
        public async Task EmailConfirmation_ReturnsNotFound_WhenUserDoesNotExist()
        {
            var email = "nonexistent@example.com";
            var token = "token";

            _mockUserManager.Setup(u => u.FindByEmailAsync(email)).ReturnsAsync((UserEntity)null);

            var result = await _controller.EmailConfirmation(email, token, CancellationToken.None);

            result.Should().BeOfType<NotFoundObjectResult>()
                  .Which.StatusCode.Should().Be(404);
        }

        [Test]
        public async Task EmailConfirmation_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            var email = "test@example.com";
            var token = "token";

            _mockUserManager.Setup(u => u.FindByEmailAsync(email))
                            .Throws(new Exception("Test Exception"));

            var result = await _controller.EmailConfirmation(email, token, CancellationToken.None);

            result.Should().BeOfType<ObjectResult>()
                  .Which.StatusCode.Should().Be(500);
        }

        #endregion

        #region SendPasswordResetLink

        [Test]
        public async Task SendPasswordResetLink_ReturnsOk_WhenLinkIsSent()
        {
            var request = new UserRequestBase<PasswordResetLinkRequest>
            {
                Data = new PasswordResetLinkRequest { Email = "test@example.com" }
            };
            var user = new UserEntity { Email = request.Data.Email };

            _mockUserManager.Setup(u => u.FindByEmailAsync(request.Data.Email)).ReturnsAsync(user);
            _mockUserManager.Setup(u => u.GeneratePasswordResetTokenAsync(user)).ReturnsAsync("token");
            _mockConfiguration.Setup(c => c["Frontend:AppUrl"]).Returns("http://localhost");
            _mockEmailService.Setup(e => e.SendPasswordResetLinkAsync(user, user.Email, It.IsAny<string>())).Returns(Task.CompletedTask);

            var result = await _controller.SendPasswordResetLink(request, CancellationToken.None);

            result.Should().BeOfType<OkObjectResult>()
                  .Which.StatusCode.Should().Be(200);
        }

        [Test]
        public async Task SendPasswordResetLink_ReturnsNotFound_WhenUserDoesNotExist()
        {
            var request = new UserRequestBase<PasswordResetLinkRequest>
            {
                Data = new PasswordResetLinkRequest { Email = "nonexistent@example.com" }
            };

            _mockUserManager.Setup(u => u.FindByEmailAsync(request.Data.Email)).ReturnsAsync((UserEntity)null);

            var result = await _controller.SendPasswordResetLink(request, CancellationToken.None);

            result.Should().BeOfType<NotFoundObjectResult>()
                  .Which.StatusCode.Should().Be(404);
        }

        [Test]
        public async Task SendPasswordResetLink_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            var request = new UserRequestBase<PasswordResetLinkRequest>
            {
                Data = new PasswordResetLinkRequest { Email = "test@example.com" }
            };

            _mockUserManager.Setup(u => u.FindByEmailAsync(request.Data.Email))
                            .Throws(new Exception("Test Exception"));

            var result = await _controller.SendPasswordResetLink(request, CancellationToken.None);

            result.Should().BeOfType<ObjectResult>()
                  .Which.StatusCode.Should().Be(500);
        }

        #endregion

        #region ResetPassword

        [Test]
        public async Task ResetPassword_ReturnsOk_WhenPasswordIsReset()
        {
            var token = "token";
            var request = new UserRequestBase<PasswordResetRequest>
            {
                Data = new PasswordResetRequest { Email = "test@example.com", Password = "NewPassword123" }
            };
            var user = new UserEntity { Email = request.Data.Email };

            _mockUserManager.Setup(u => u.FindByEmailAsync(request.Data.Email)).ReturnsAsync(user);
            _mockUserManager.Setup(u => u.ResetPasswordAsync(user, token, request.Data.Password)).ReturnsAsync(IdentityResult.Success);

            var result = await _controller.ResetPassword(token, request, CancellationToken.None);

            result.Should().BeOfType<OkObjectResult>()
                  .Which.StatusCode.Should().Be(200);
        }

        [Test]
        public async Task ResetPassword_ReturnsNotFound_WhenUserDoesNotExist()
        {
            var token = "token";
            var request = new UserRequestBase<PasswordResetRequest>
            {
                Data = new PasswordResetRequest { Email = "nonexistent@example.com", Password = "NewPassword123" }
            };

            _mockUserManager.Setup(u => u.FindByEmailAsync(request.Data.Email)).ReturnsAsync((UserEntity)null);

            var result = await _controller.ResetPassword(token, request, CancellationToken.None);

            result.Should().BeOfType<NotFoundObjectResult>()
                  .Which.StatusCode.Should().Be(404);
        }

        [Test]
        public async Task ResetPassword_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            var token = "token";
            var request = new UserRequestBase<PasswordResetRequest>
            {
                Data = new PasswordResetRequest { Email = "test@example.com", Password = "NewPassword123" }
            };

            _mockUserManager.Setup(u => u.FindByEmailAsync(request.Data.Email))
                            .Throws(new Exception("Test Exception"));

            var result = await _controller.ResetPassword(token, request, CancellationToken.None);

            result.Should().BeOfType<ObjectResult>()
                  .Which.StatusCode.Should().Be(500);
        }

        #endregion
    }
}
