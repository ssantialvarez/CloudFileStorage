using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudFileStorage.Controllers;
using CloudFileStorage.Models.DTOs;
using CloudFileStorage.Services.Interfaces;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;


namespace CloudFileStorage.Tests
{
    public class AuthControllerTests
    {
        private readonly IAuthService _authServiceMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authServiceMock = A.Fake<IAuthService>();
            _controller = new AuthController(_authServiceMock);
        }

        [Fact]
        public void Register_Returns_Succes_Token()
        {
            //Arrange
            var fakeUser = A.CollectionOfDummy<AuthRequest>(1).First();
            AuthResponse response = new AuthResponse(); 
            response.success = true;
            response.token = "token";
            response.message = "Login successful";
          
            A.CallTo(() => _authServiceMock.RegisterAsync(fakeUser)).Returns(response);
            
            //Act
            var actionResult = _controller.Register(fakeUser);

            //Assert
            var result = actionResult.Result as OkObjectResult;
            var responseValue = result.Value as AuthResponse;
            Assert.Equal(response, responseValue);
        }

        [Fact]
        public void Register_Returns_Bad_Request()
        {
            //Arrange
            var fakeUser = A.CollectionOfDummy<AuthRequest>(1).First();
            AuthResponse response = new AuthResponse();
            response.message = "Username already exists.";
            A.CallTo(() => _authServiceMock.RegisterAsync(fakeUser)).Returns(response);
            
            //Act
            var actionResult = _controller.Register(fakeUser);
            
            //Assert
            var result = actionResult.Result as BadRequestObjectResult;
            result.Value = "Username already exists.";
            Assert.Equal(response.message, result.Value);
        }

        [Fact]
        public void Login_Returns_Succes_Token()
        {
            //Arrange
            var fakeUser = A.CollectionOfDummy<AuthRequest>(1).First();
            AuthResponse response = new AuthResponse();
            response.success = true;
            response.token = "token";
            response.message = "Login successful";
            A.CallTo(() => _authServiceMock.LoginAsync(fakeUser)).Returns(response);
            
            //Act
            var actionResult = _controller.Login(fakeUser);
            
            //Assert
            var result = actionResult.Result as OkObjectResult;
            var responseValue = result.Value as AuthResponse;
            Assert.Equal(response, responseValue);
        }

        [Fact]
        public void Login_Returns_Bad_Request()
        {
            //Arrange
            var fakeUser = A.CollectionOfDummy<AuthRequest>(1).First();
            AuthResponse response = new AuthResponse();
            response.success = false;
            response.token = null;
            response.message = "Invalid username.";
            A.CallTo(() => _authServiceMock.LoginAsync(fakeUser)).Returns(response);
            
            //Act
            var actionResult = _controller.Login(fakeUser);
            
            //Assert
            var result = actionResult.Result as BadRequestObjectResult;
            result.Value = "Invalid username.";
            
            Assert.Equal(response.message, result.Value);
        }
    }
}
