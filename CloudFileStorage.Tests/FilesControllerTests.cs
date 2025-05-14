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
    public class FilesControllerTests
    {
        private readonly FilesController _controller;
        private readonly IFileService _fileServiceMock;

        public FilesControllerTests()
        {
            _fileServiceMock = A.Fake<IFileService>();
            _controller = new FilesController(_fileServiceMock);
        }

        [Fact]
        public void GetFiles_Returns_Files()
        {
            // Arrange
            var fakeFiles = A.CollectionOfDummy<FileResponse>(5);
            A.CallTo(() => _fileServiceMock.GetAllFilesAsync()).Returns(fakeFiles as List<FileResponse>);

            // Act
            var actionResult = _controller.GetFiles();

            // Assert
            var result = actionResult.Result as OkObjectResult;
            var responseValue = result.Value as List<FileResponse>;
            Assert.Equal(fakeFiles, responseValue);
        }
        [Fact]
        public void GetFile_Returns_File()
        {
            // Arrange
            var fakeFile = A.CollectionOfDummy<FileResponse>(1).First();
            Guid fileId = Guid.NewGuid();
            fakeFile.id = fileId; 
            fakeFile.fileName = "TestFile";
    
            A.CallTo(() => _fileServiceMock.GetFileByIdAsync(fileId)).Returns(fakeFile);

            // Act
            var actionResult = _controller.GetFile(fileId);

            // Assert
            var result = actionResult.Result as OkObjectResult;
            var responseValue = result.Value as FileResponse;
            Assert.Equal(fakeFile.id, responseValue.id);
            Assert.Equal(fakeFile.fileName, responseValue.fileName);
        }





        [Fact]
        public void GetFilesByUserId_Returns_Files()
        {
            // Arrange
            var fakeFiles = A.CollectionOfDummy<FileResponse>(5);
            string userId = "123";
            A.CallTo(() => _fileServiceMock.GetFilesByUserIdAsync(userId)).Returns(fakeFiles as List<FileResponse>);
            // Act
            var actionResult = _controller.GetFilesByUserId(userId);
            // Assert
            var result = actionResult.Result as OkObjectResult;
            var responseValue = result.Value as List<FileResponse>;
            Assert.Equal(fakeFiles, responseValue);
        }

        /*
        [Fact]
        public void GetStats_Returns_Stats()
        {
            // Arrange
            var fakeStats = A.Dummy<StatsResponse>();
            A.CallTo(() => _fileServiceMock.GetStatsAsync()).Returns(fakeStats);
            // Act
            var actionResult = _controller.GetStats();
            // Assert
            var result = actionResult.Result as OkObjectResult;
            var responseValue = result.Value as StatsResponse;
            Assert.Equal(fakeStats, responseValue);
        }
        */

    }
}
