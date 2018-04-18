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
        public async Task Get_ByArticleAndComment_NotFound_Article()
        {
            // Arrange
            _articleRepositoryMock.Setup(m => m.GetAsync(1)).Returns(Task.FromResult<Article>(null));

            // Act
            var result = await _commentsController.Get(1,1);

            // Assert
            Assert.NotNull(result);

            var objectResult = result as NotFoundResult;
            Assert.NotNull(objectResult);
        }

        [Fact]
        public async Task Get_ByArticleAndComment_NotFound_Comment()
        {
            // Arrange
            _articleRepositoryMock.Setup(m => m.GetAsync(1)).Returns(Task.FromResult(Builder<Article>.CreateNew().Build()));
            var articleDbSetMock = Builder<Comment>.CreateListOfSize(3).Build().ToAsyncDbSetMock();
            _commentRepositoryMock.Setup(m => m.Query()).Returns(articleDbSetMock.Object);

            // Act
            var result = await _commentsController.Get(1, 1);

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
            var articleDbSetMock = Builder<Comment>.CreateListOfSize(1)
                .All()
                .CreateTitles()
                .Build().ToAsyncDbSetMock();
            _commentRepositoryMock.Setup(m => m.Query()).Returns(articleDbSetMock.Object);

            // Act
            var result = await _commentsController.Get(1,1);

            // Assert
            Assert.NotNull(result);

            var objectResult = result as OkObjectResult;
            Assert.NotNull(objectResult);

            Assert.Equal(200, objectResult.StatusCode);

            var content = objectResult.Value as CommentModel;
            Assert.NotNull(content);

            Assert.Equal(Constants.SampleTitle, content.Title);
        }

        [Fact]
        public async Task Post_Returns400BadRequest()
        {
            // Arrange
            _commentsController.ModelState.AddModelError("Content", "Content is required");
            var comment = Builder<CommentModel>.CreateNew().Build();

            //Act
            var result = await _commentsController.Post(1, comment);

            // Assert
            Assert.NotNull(result);

            var objectResult = result as BadRequestObjectResult;
            Assert.NotNull(objectResult);
            Assert.Equal(400, objectResult.StatusCode);
        }

        [Fact]
        public async Task Post_Returns404NotFound()
        {
            // Arrange
            var comment = Builder<CommentModel>.CreateNew().Build();
            _articleRepositoryMock.Setup(m => m.GetAsync(1)).Returns(Task.FromResult<Article>(null));

            //Act
            var result = await _commentsController.Post(1, comment);

            // Assert
            Assert.NotNull(result);

            var objectResult = result as NotFoundResult;
            Assert.NotNull(objectResult);
            Assert.Equal(404, objectResult.StatusCode);
        }

        [Fact]
        public async Task Post_Returns201CreatedResponse()
        {
            // Arrange
            var article = Builder<Article>.CreateNew().Build();
            var comment = Builder<Comment>.CreateNew().Build();
            var commentModel = Builder<CommentModel>.CreateNew().Build();
            _articleRepositoryMock.Setup(m => m.GetAsync(0)).Returns(Task.FromResult(article));
            _commentRepositoryMock.Setup(m => m.InsertAsync(comment)).Returns(Task.FromResult(comment));

            var expectedUri = "articles/0/comments/0";

            // Act
            var result = await _commentsController.Post(0,commentModel);

            // Assert
            Assert.NotNull(result);

            var objectResult = result as CreatedResult;
            Assert.NotNull(objectResult);
            Assert.Equal(expectedUri, objectResult.Location);

            var content = objectResult.Value as CommentModel;
            Assert.NotNull(content);
        }
    }
}
