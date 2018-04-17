using crossblog.Controllers;
using crossblog.Domain;
using crossblog.Model;
using crossblog.Repositories;
using FizzWare.NBuilder;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace crossblog.tests.Controllers
{
    public class CommentsControllerTests
    {

        private CommentsController _commentsController;

        private Mock<ICommentRepository> _commentRepositoryMock = new Mock<ICommentRepository>();

        private Mock<IArticleRepository> _articleRepositoryMock = new Mock<IArticleRepository>();


        public CommentsControllerTests()
        {
            _commentsController = new CommentsController(_articleRepositoryMock.Object, _commentRepositoryMock.Object);
        }

        [Fact]
        public async Task Get_ByArticle_NotFound()
        {
            // Arrange
            _articleRepositoryMock.Setup(m => m.GetAsync(1)).Returns(Task.FromResult<Article>(null));

            // Act
            var result = await _commentsController.Get(1);

            // Assert
            Assert.NotNull(result);

            var objectResult = result as NotFoundResult;
            Assert.NotNull(objectResult);
        }

        [Fact]
        public async Task Get_ByArticle_ReturnsItem()
        {
            // Arrange
            _articleRepositoryMock.Setup(m => m.GetAsync(1)).Returns(Task.FromResult(Builder<Article>.CreateNew().Build()));
            var articleDbSetMock = Builder<Comment>.CreateListOfSize(3).Build().ToAsyncDbSetMock();
            _commentRepositoryMock.Setup(m => m.Query()).Returns(articleDbSetMock.Object);

            // Act
            var result = await _commentsController.Get(1);

            // Assert
            Assert.NotNull(result);

            var objectResult = result as OkObjectResult;
            Assert.NotNull(objectResult);

            Assert.Equal(200, objectResult.StatusCode);

            var content = objectResult.Value as CommentListModel;
            Assert.NotNull(content);

            Assert.Equal(3, content.Comments.Count());
        }

        [Fact]
        public async Task Get_ByArticleAndComment_NotFound()
        {
            // Arrange
            _articleRepositoryMock.Setup(m => m.GetAsync(1)).Returns(Task.FromResult<Article>(null));

            // Act
            var result = await _commentsController.Get(1);

            // Assert
            Assert.NotNull(result);

            var objectResult = result as NotFoundResult;
            Assert.NotNull(objectResult);
        }

        [Fact]
        public async Task Get_ByArticleAndComment_ReturnsItem()
        {
            // Arrange
            _articleRepositoryMock.Setup(m => m.GetAsync(1)).Returns(Task.FromResult(Builder<Article>.CreateNew().Build()));
            var articleDbSetMock = Builder<Comment>.CreateListOfSize(3).Build().ToAsyncDbSetMock();
            _commentRepositoryMock.Setup(m => m.Query()).Returns(articleDbSetMock.Object);

            // Act
            var result = await _commentsController.Get(1);

            // Assert
            Assert.NotNull(result);

            var objectResult = result as OkObjectResult;
            Assert.NotNull(objectResult);

            Assert.Equal(200, objectResult.StatusCode);

            var content = objectResult.Value as CommentListModel;
            Assert.NotNull(content);

            Assert.Equal(3, content.Comments.Count());
        }
    }
}
