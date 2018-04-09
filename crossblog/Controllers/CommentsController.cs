using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using crossblog.Domain;
using crossblog.Model;
using crossblog.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace crossblog.Controllers
{
    [Route("articles")]
    public class CommentsController : Controller
    {
        private readonly ICommentRepository _commentRepository;

        private readonly IArticleRepository _articleRepository;

        public CommentsController(IArticleRepository articleRepository, ICommentRepository commentRepository)
        {
            _articleRepository = articleRepository;
            _commentRepository = commentRepository;
        }

        // GET articles/5/comments
        [HttpGet("{articleId}/[controller]")]
        public async Task<IActionResult> Get([FromRoute]int articleId)
        {
            var article = await _articleRepository.GetAsync(articleId);

            if (article == null)
            {
                return NotFound();
            }

            var comments = await _commentRepository.Query().ToListAsync();

            var result = new CommentListModel
            {
                Comments = comments.Select(c => new CommentModel
                {
                    Id = c.Id,
                    Email = c.Email,
                    Title = c.Title,
                    Content = c.Content,
                    Date = c.Date,
                    Published = c.Published
                })
            };

            return Ok(result);
        }

        // GET articles/{articleId}/comments/5
        [HttpGet("{articleId}/[controller]/{id}")]
        public async Task<IActionResult> Get([FromRoute]int articleId, [FromRoute]int id)
        {
            var article = await _articleRepository.GetAsync(articleId);

            if (article == null)
            {
                return NotFound();
            }

            var comment = await _commentRepository.Query().FirstOrDefaultAsync(c => c.ArticleId == articleId && c.Id == id);

            if (comment == null)
            {
                return NotFound();
            }

            var result = new CommentModel
            {
                Id = comment.Id,
                Email = comment.Email,
                Title = comment.Title,
                Content = comment.Content,
                Date = comment.Date,
                Published = comment.Published
            };

            return Ok(result);
        }

        // POST articles/5/comments
        [HttpPost("{articleId}/[controller]")]
        public async Task<IActionResult> Post([FromRoute]int articleId, [FromBody]CommentModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var article = await _articleRepository.GetAsync(articleId);

            if (article == null)
            {
                return NotFound();
            }

            var comment = new Comment
            {
                ArticleId = articleId,
                Email = model.Email,
                Title = model.Title,
                Content = model.Content,
                Date = DateTime.UtcNow,
                Published = model.Published
            };

            await _commentRepository.InsertAsync(comment);

            var result = new CommentModel
            {
                Id = comment.Id,
                Email = comment.Email,
                Title = comment.Title,
                Content = comment.Content,
                Date = comment.Date,
                Published = comment.Published
            };

            return Created($"articles/{articleId}/comments/{comment.Id}", result);
        }
    }
}