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
            var articleModel = Builder<ArticleModel>.CreateNew().Build();
            var article = Builder<Article>.CreateNew().Build();
            _articleRepositoryMock.Setup(m => m.InsertAsync(article)).Returns(Task.FromResult(Builder<Article>.CreateNew().Build()));

            var expectedUri = "articles/0";

            // Act
            var result = await _articlesController.Post(articleModel);

            // Assert
            Assert.NotNull(result);

            var objectResult = result as CreatedResult;
            Assert.NotNull(objectResult);
            Assert.Equal(expectedUri, objectResult.Location);

            var content = objectResult.Value as Article;
            Assert.NotNull(content);
        }

        [Fact]
        public async Task Put_Returns400BadRequest()
        {

            // Arrange
            _articlesController.ModelState.AddModelError("Content", "Content is required");
            var article = Builder<ArticleModel>.CreateNew().Build();

            //Act
            var result = await _articlesController.Put(0,article);

            // Assert
            Assert.NotNull(result);

            var objectResult = result as BadRequestObjectResult;
            Assert.NotNull(objectResult);
            Assert.Equal(400, objectResult.StatusCode);
        }

        [Fact]
        public async Task Put_Returns404NotFound()
        {
            // Arrange
            var article = Builder<ArticleModel>.CreateNew().Build();
            _articleRepositoryMock.Setup(m => m.GetAsync(0)).Returns(Task.FromResult((Article)null));
            
            //Act
            var result = await _articlesController.Put(0, article);

            // Assert
            Assert.NotNull(result);

            var objectResult = result as NotFoundResult;
            Assert.NotNull(objectResult);
            Assert.Equal(404, objectResult.StatusCode);
        }

        [Fact]
        public async Task Put_Returns200OkResponse()
        {
            // Arrange
            var articleModel = Builder<ArticleModel>.CreateNew().Build();
            var article = Builder<Article>.CreateNew().Build();
            _articleRepositoryMock.Setup(m => m.UpdateAsync(article)).Returns(Task.FromResult(article));
            _articleRepositoryMock.Setup(m => m.GetAsync(0)).Returns(Task.FromResult(article));

            // Act
            var result = await _articlesController.Put(0, articleModel);

            // Assert
            Assert.NotNull(result);

            var objectResult = result as OkObjectResult;
            Assert.NotNull(objectResult);
            Assert.Equal(200, objectResult.StatusCode);

            var content = objectResult.Value as Article;
            Assert.NotNull(content);
        }

        [Fact]
        public async Task Delete_Returns404NotFound()
        {
            // Arrange
            _articleRepositoryMock.Setup(m => m.GetAsync(0)).Returns(Task.FromResult((Article)null));
          
            //Act
            var result = await _articlesController.Delete(0);

            // Assert
            Assert.NotNull(result);

            var objectResult = result as NotFoundResult;
            Assert.NotNull(objectResult);
            Assert.Equal(404, objectResult.StatusCode);           
        }

        [Fact]
        public async Task Delete_Returns200OkResponse()
        {
            // Arrange
            _articleRepositoryMock.Setup(m => m.GetAsync(0)).Returns(Task.FromResult(Builder<Article>.CreateNew().Build()));
            _articleRepositoryMock.Setup(m => m.DeleteAsync(Builder<Article>.CreateNew().Build())).Returns(Task.CompletedTask);


            // Act
            var result = await _articlesController.Delete(0);

            // Assert
            Assert.NotNull(result);

            var objectResult = result as OkResult;
            Assert.NotNull(objectResult);
            Assert.Equal(200, objectResult.StatusCode);
        }
    }
}
