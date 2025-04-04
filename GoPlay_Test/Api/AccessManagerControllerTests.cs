using FluentAssertions;
using GoPlay_App.Api.Controllers.AccessManager;
using GoPlay_App.Api.Controllers.AccessManager.Models;
using GoPlay_App.Api.Controllers.UserController.Models;
using GoPlay_Core.Entities;
using GoPlay_Core.Exceptions;
using GoPlay_Core.Services.Interfaces;
using Microsoft.AspNetCore.Http;
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

        [SetUp]
        public void Setup()
        {
            _userServiceMock = new Mock<IUserService>();
            _tokenServiceMock = new Mock<ITokenService>();
            _controller = new AccessManagerController(_userServiceMock.Object, _tokenServiceMock.Object);
        }

        #region ValidateUser

        [Test]
        public async Task ValidateUser_ShouldReturnOk_WhenTokenIsValid()
        {
            // Arrange
            var token = "valid_token";
            _tokenServiceMock.Setup(ts => ts.ValidateToken(It.IsAny<string>())).Returns(true);
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

        [Test]
        public async Task ValidateUser_ShouldReturnUnauthorized_WhenTokenIsInvalid()
        {
            // Arrange
            var token = "invalid_token";
            _tokenServiceMock.Setup(ts => ts.ValidateToken(It.IsAny<string>())).Returns(false);
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
            result.Should().BeOfType<UnauthorizedObjectResult>();
            var unauthorizedResult = result as UnauthorizedObjectResult;
            unauthorizedResult.Should().NotBeNull();
            unauthorizedResult!.Value.Should().BeEquivalentTo(new { message = "Token inválido." });
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
    }
}



