using FluentAssertions;
using GoPlay_App.Api.Controllers;
using GoPlay_App.Api.Controllers.AccessManager;
using GoPlay_App.Api.Controllers.AccessManager.Models;
using GoPlay_Core.Entities;
using GoPlay_Core.Exceptions;
using GoPlay_Core.Repository.Interfaces;
using GoPlay_Core.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace GoPlay_Test.Api
{
    [TestFixture]
    public class AccessManagerControllerTests
    {
        private Mock<IUserService> _userServiceMock;
        private Mock<ITokenService> _tokenServiceMock;
        private AccessManagerController _controller;
        private Mock<IEmailService> _mockEmailService;
        private Mock<IUserRepository> _repository;
        private Mock <UserManager<UserEntity>> _user;

        [SetUp]
        public void Setup()
        {
            _userServiceMock = new Mock<IUserService>();
            _tokenServiceMock = new Mock<ITokenService>();
            _mockEmailService = new Mock<IEmailService>();
            _repository = new Mock<IUserRepository>();
            _user = new Mock<UserManager<UserEntity>>(
                new Mock<IUserStore<UserEntity>>().Object,
                null, null, null, null, null, null, null, null);
            _controller = new AccessManagerController(
                _userServiceMock.Object,
                _tokenServiceMock.Object,
                _mockEmailService.Object,
                _user.Object,
                _repository.Object
            );
        }

        #region ValidateUser

        [Test]
        public async Task ValidateUser_ShouldReturnOk_WhenTokenIsValid()
        {
            // Arrange
            var token = "valid_token";
            _tokenServiceMock.Setup(ts => ts.ValidateToken(It.IsAny<string>())).Returns("valid_token"); 
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
                {
                    Request = { Headers = { ["Authorization"] = "Bearer " + token } }
                }
            };

            // Act
            var result = await _controller.ValidateUser();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(new { message = "Acesso permitido." });
        }

        #endregion

        #region Login

        [Test]
        public async Task Login_ShouldReturnOk_WhenLoginIsSuccessful()
        {
            var userRequest = new UserRequestBase<UserLoginRequest>
            {
                Data = new UserLoginRequest("username", "password")
            };
            var token = "valid_token";
            _userServiceMock.Setup(us => us.Login(It.IsAny<LoginEntity>())).ReturnsAsync(token);

            // Act
            var result = await _controller.Login(userRequest, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(new { message = "Login realizado com sucesso.", token });
        }

        [Test]
        public async Task Login_ShouldReturnNotFound_WhenUserNotFound()
        {
            // Arrange
            var userRequest = new UserRequestBase<UserLoginRequest> { Data = new UserLoginRequest("username", "password") };
            _userServiceMock.Setup(us => us.Login(It.IsAny<LoginEntity>())).ThrowsAsync(new NotFoundException("Usuário não encontrado."));

            // Act
            var result = await _controller.Login(userRequest, CancellationToken.None);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult!.Value.Should().BeEquivalentTo(new { message = "Usuário não encontrado.", error = "Usuário não encontrado." });
        }

        [Test]
        public async Task Login_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var userRequest = new UserRequestBase<UserLoginRequest> { Data = new UserLoginRequest("username", "password") };
            _userServiceMock.Setup(us => us.Login(It.IsAny<LoginEntity>())).ThrowsAsync(new Exception("Login failed"));

            // Act
            var result = await _controller.Login(userRequest, CancellationToken.None);

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            objectResult.Value.Should().BeEquivalentTo(new { message = "Erro ao realizar login.", error = "Login failed" });
        }

        #endregion

        #region Logout

        [Test]
        public async Task Logout_ShouldReturnOk_WhenLogoutIsSuccessful()
        {
            // Arrange
            _userServiceMock.Setup(us => us.Logout()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Logout();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(new { message = "Logout realizado com sucesso." });
        }

        [Test]
        public async Task Logout_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            _userServiceMock.Setup(us => us.Logout()).ThrowsAsync(new Exception("Logout failed"));

            // Act
            var result = await _controller.Logout();

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            objectResult.Value.Should().BeEquivalentTo(new { message = "Erro ao realizar logout.", error = "Logout failed" });
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

            _user.Setup(u => u.FindByEmailAsync(request.Data.Email)).ReturnsAsync(user);
            _mockEmailService.Setup(e => e.SendPasswordResetLinkAsync(user)).Returns(Task.CompletedTask);

            var result = await _controller.SendPasswordResetLink(request, CancellationToken.None);

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(new { message = "Link de redefinição enviado com sucesso." });
        }

        [Test]
        public async Task SendPasswordResetLink_ReturnsNotFound_WhenUserNotFound()
        {
            var request = new UserRequestBase<PasswordResetLinkRequest>
            {
                Data = new PasswordResetLinkRequest { Email = "notfound@example.com" }
            };

            _user.Setup(u => u.FindByEmailAsync(request.Data.Email)).ReturnsAsync((UserEntity)null!);

            var result = await _controller.SendPasswordResetLink(request, CancellationToken.None);

            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult!.Value.Should().BeEquivalentTo(new { message = "Email não cadastrado." });
        }

        [Test]
        public async Task SendPasswordResetLink_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            var request = new UserRequestBase<PasswordResetLinkRequest>
            {
                Data = new PasswordResetLinkRequest { Email = "error@example.com" }
            };

            _user.Setup(u => u.FindByEmailAsync(request.Data.Email)).ThrowsAsync(new Exception("Erro interno"));

            var result = await _controller.SendPasswordResetLink(request, CancellationToken.None);

            result.Should().BeOfType<ObjectResult>();
            var errorResult = result as ObjectResult;
            errorResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            errorResult.Value.Should().BeEquivalentTo(new { message = "Erro interno" });
        }

        #endregion

        #region ResetPassword

        [Test]
        public async Task ResetPassword_ReturnsOk_WhenPasswordIsReset()
        {
            var token = "valid_token";
            var password = "NewPassword123";
            var userId = "user-id";

            var request = new UserRequestBase<PasswordResetRequest>
            {
                Data = new PasswordResetRequest { Password = password }
            };

            _tokenServiceMock.Setup(ts => ts.ValidateToken(token)).Returns(userId);
            _repository.Setup(r => r.UpDatePassword(userId, password)).ReturnsAsync(true);

            var result = await _controller.ResetPassword(token, request, CancellationToken.None);

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(new { message = "Senha redefinida com sucesso." });
        }

        [Test]
        public async Task ResetPassword_ReturnsBadRequest_WhenTokenIsInvalid()
        {
            var request = new UserRequestBase<PasswordResetRequest>
            {
                Data = new PasswordResetRequest { Password = "senha" }
            };

            _tokenServiceMock.Setup(ts => ts.ValidateToken("invalid_token")).Returns((string)null!);

            var result = await _controller.ResetPassword("invalid_token", request, CancellationToken.None);

            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequest = result as BadRequestObjectResult;
            badRequest!.Value.Should().BeEquivalentTo(new { message = "Token inválido." });
        }

        [Test]
        public async Task ResetPassword_ReturnsBadRequest_WhenPasswordIsMissing()
        {
            var token = "valid_token";
            var userId = "user-id";
            var request = new UserRequestBase<PasswordResetRequest>
            {
                Data = new PasswordResetRequest { Password = "" }
            };

            _tokenServiceMock.Setup(ts => ts.ValidateToken(token)).Returns(userId);

            var result = await _controller.ResetPassword(token, request, CancellationToken.None);

            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequest = result as BadRequestObjectResult;
            badRequest!.Value.Should().BeEquivalentTo(new { message = "Senha Obrigatória." });
        }

        [Test]
        public async Task ResetPassword_ReturnsInternalServerError_WhenUpdateFails()
        {
            var token = "valid_token";
            var password = "Senha123";
            var userId = "user-id";

            var request = new UserRequestBase<PasswordResetRequest>
            {
                Data = new PasswordResetRequest {Password = password }
            };

            _tokenServiceMock.Setup(ts => ts.ValidateToken(token)).Returns(userId);
            _repository.Setup(r => r.UpDatePassword(userId, password)).ReturnsAsync(false);

            var result = await _controller.ResetPassword(token, request, CancellationToken.None);

            result.Should().BeOfType<ObjectResult>();
            var errorResult = result as ObjectResult;
            errorResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            errorResult.Value.Should().BeEquivalentTo(new { message = "Erro ao redefinir a senha." });
        }

        #endregion


    }
}



