using System.Security.Claims;
using CloudFileStorage.Controllers;
using CloudFileStorage.Models;
using CloudFileStorage.Models.DTOs;
using CloudFileStorage.Services.Interfaces;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Moq;

namespace CloudFileStorage.Tests
{
    public class UsersControllerTests
    {
        private readonly IUserService _userServiceMock;
        private readonly UsersController _controller;
        public UsersControllerTests() {
            _userServiceMock = A.Fake<IUserService>();
            _controller = new UsersController(_userServiceMock);
        }

        [Fact]
        public void GetMe_Returns_Me()
        {
            // Arrange
            var fakeUser = A.CollectionOfDummy<UserResponse>(1).First();
            A.CallTo(() => _userServiceMock.GetMeAsync()).Returns(fakeUser);

            // Act
            var actionResult = _controller.GetMe();

            // Assert
            var result = actionResult.Result as OkObjectResult;
            var responseValue = result.Value as UserResponse;
            Assert.Equal(fakeUser, responseValue);
        }

        [Fact]
        public void GetUsers_Returns_Users()
        {
            // Arrange
            var fakeUsers = A.CollectionOfDummy<UserResponse>(5);
            A.CallTo(() => _userServiceMock.GetUsersAsync()).Returns(fakeUsers as List<UserResponse>);

            // Act
            var actionResult = _controller.GetUsers();

            // Assert
            var result = actionResult.Result as OkObjectResult;
            var responseValue = result.Value as List<UserResponse>;
            Assert.Equal(fakeUsers, responseValue);
        }

        [Fact]
        public void GetUserById_Returns_User()
        {
            // Arrange
            var fakeUser = A.CollectionOfDummy<UserResponse>(1).First();
            string userId = "123";
            A.CallTo(() => _userServiceMock.GetUserByIdAsync(userId)).Returns(fakeUser);

            // Act
            var actionResult = _controller.GetUserById(userId);

            // Assert
            var result = actionResult.Result as OkObjectResult;
            var responseValue = result.Value as UserResponse;
            Assert.Equal(fakeUser, responseValue);
        }

        [Fact]
        public void UpdateUser_Returns_Updated_User()
        {
            // Arrange
            var fakeUser = A.CollectionOfDummy<UserResponse>(1).First();
            var updateRequest = new UpdateUserRequest();
            updateRequest.username = "username";
            updateRequest.password = "password";
            A.CallTo(() => _userServiceMock.UpdateUserAsync(updateRequest)).Returns(fakeUser);

            // Act
            var actionResult = _controller.PutUser(updateRequest);

            // Assert
            var result = actionResult.Result as OkObjectResult;
            var responseValue = result.Value as UserResponse;
            Assert.Equal(fakeUser, responseValue);
        }

        [Fact]
        public void DeleteUser_Returns_Deleted_User()
        {
            // Arrange
            var fakeUser = A.CollectionOfDummy<UserResponse>(1).First();
            string userId = "123";
            A.CallTo(() => _userServiceMock.DeleteUserAsync(userId)).Returns(fakeUser);

            // Act
            var actionResult = _controller.DeleteUser(userId);

            // Assert
            var result = actionResult.Result as OkObjectResult;
            var responseValue = result.Value as UserResponse;
            Assert.Equal(fakeUser, responseValue);
        }
    }
    
}