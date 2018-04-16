using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using crossblog.Controllers;
using crossblog.Domain;
using crossblog.Model;
using crossblog.Repositories;
using FizzWare.NBuilder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Xunit.Sdk;

namespace crossblog.tests.Controllers
{
    public class ArticlesControllerTests
    {
        private ArticlesController _articlesController;

        private Mock<IArticleRepository> _articleRepositoryMock = new Mock<IArticleRepository>();

        public ArticlesControllerTests()
        {
            _articlesController = new ArticlesController(_articleRepositoryMock.Object);
        }

        [Fact]
        public async Task Search_ReturnsEmptyList()
        {
            // Arrange
            var articleDbSetMock = Builder<Article>.CreateListOfSize(3).Build().ToAsyncDbSetMock();
            _articleRepositoryMock.Setup(m => m.Query()).Returns(articleDbSetMock.Object);

            // Act
            var result = await _articlesController.Search("Invalid");

            // Assert
            Assert.NotNull(result);

            var objectResult = result as OkObjectResult;
            Assert.NotNull(objectResult);

            var content = objectResult.Value as ArticleListModel;
            Assert.NotNull(content);

            Assert.Empty(content.Articles);
        }

        [Fact]
        public async Task Search_ReturnsList()
        {
            // Arrange
            var articleDbSetMock = Builder<Article>.CreateListOfSize(3).Build().ToAsyncDbSetMock();
            _articleRepositoryMock.Setup(m => m.Query()).Returns(articleDbSetMock.Object);

            // Act
            var result = await _articlesController.Search("Title");

            // Assert
            Assert.NotNull(result);

            var objectResult = result as OkObjectResult;
            Assert.NotNull(objectResult);

            var content = objectResult.Value as ArticleListModel;
            Assert.NotNull(content);

            Assert.Equal(3, content.Articles.Count());
        }

        [Fact]
        public async Task Get_NotFound()
        {
            // Arrange
            _articleRepositoryMock.Setup(m => m.GetAsync(1)).Returns(Task.FromResult<Article>(null));

            // Act
            var result = await _articlesController.Get(1);

            // Assert
            Assert.NotNull(result);

            var objectResult = result as NotFoundResult;
            Assert.NotNull(objectResult);
        }

        [Fact]
        public async Task Get_ReturnsItem()
        {
            // Arrange
            _articleRepositoryMock.Setup(m => m.GetAsync(1)).Returns(Task.FromResult<Article>(Builder<Article>.CreateNew().Build()));

            // Act
            var result = await _articlesController.Get(1);

            // Assert
            Assert.NotNull(result);

            var objectResult = result as OkObjectResult;
            Assert.NotNull(objectResult);

            var content = objectResult.Value as ArticleModel;
            Assert.NotNull(content);

            Assert.Equal("Title1", content.Title);
        }

        [Fact]
        public async Task Post_Returns400BadRequest()
        {

            // Arrange
            _articlesController.ModelState.AddModelError("Content", "Content is required");
            var article = Builder<ArticleModel>.CreateNew().Build();

            //Act
            var result = await _articlesController.Post(article);

            // Assert
            Assert.NotNull(result);

            var objectResult = result as BadRequestObjectResult;
            Assert.NotNull(objectResult);
            Assert.Equal(400, objectResult.StatusCode);
        }

        [Fact]
        public async Task Post_Returns201CreatedResponse()
        {
            // Arrange
            var article = Builder<ArticleModel>
            .CreateNew()
                .With(x => x.Title = "Special title")
                .And(x => x.Content = "Special description")
                .And(x => x.Date = DateTime.Now)
                .And(x => x.Published = true)
            .Build();

            var expectedUri = "articles/0";

            // Act
            var result = await _articlesController.Post(article);

            // Assert
            Assert.NotNull(result);

            var objectResult = result as CreatedResult;
            Assert.NotNull(objectResult);
            Assert.Equal(expectedUri, objectResult.Location);

            var content = objectResult.Value as Article;
            Assert.NotNull(content);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(10)]
        public async Task Delete(int id)
        {
            // Arrange for success
            _articleRepositoryMock.Setup(m => m.GetAsync(id)).Returns(Task.FromResult(Builder<Article>.CreateNew().Build()));


            //Act
            var result = await _articlesController.Delete(id);

            // Assert
            Assert.NotNull(result);

            var objectResult = result as BadRequestObjectResult;
            Assert.NotNull(objectResult);
            Assert.Equal(400, objectResult.StatusCode);
        }
    }
}
