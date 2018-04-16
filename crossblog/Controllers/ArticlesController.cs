using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using crossblog.Domain;
using crossblog.Model;
using crossblog.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace crossblog.Controllers
{
    [Route("[controller]")]
    public class ArticlesController : Controller
    {
        private readonly IArticleRepository _articleRepository;

        public ArticlesController(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        // GET articles/search
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery]string title)
        {
            ///This is old logic
            //var articles = await _articleRepository.Query()
            //    .Where(a => a.Title.Contains(title) || a.Content.Contains(title))
            //    .Take(20)
            //    .ToListAsync();

            //var result = new ArticleListModel
            //{
            //    Articles = articles.Select(a => new ArticleModel
            //    {
            //        Id = a.Id,
            //        Title = a.Title,
            //        Content = a.Content,
            //        Date = a.Date,
            //        Published = a.Published
            //    })
            //};

            //return Ok(result);

            ///For search improvement, I added indexes (check context class), disabled tracking, changed contains with like and avoided being tooo greedy with colums
            ///Those are the quick improvements I can come up for this query
            ///A further improvement could be translate the whole query into a Store procedure and call the sproc. Didnt do it to avoid spending too much time on it 
            ///The performance improvement is more obvious removing the Take(20) condition. 
            ///Using about 110000 records on articles table, old logic took about 1.5 seconds while new logic was under 800 ms. In both cases not using he Take(20) condition.
            ///I created a dummy data sproc that include in the project files for testing preformance
            var articles = await _articleRepository.Query()
            .AsNoTracking()
            .Where(a => EF.Functions.Like(a.Title, $"%{title}%") || EF.Functions.Like(a.Content, $"%{title}%"))
            .Take(20)
            .Select(a => new ArticleModel
            {
                Id = a.Id,
                Title = a.Title,
                Content = a.Content,
                Date = a.Date,
                Published = a.Published
            })
            .ToListAsync();

            var result = new ArticleListModel
            {
                Articles = articles
            };

            return Ok(result);


            ///This logic uses a call to a sproc. I created it just to show how the same operation can be done using this technique
            ///In some cases this is preferred to using a full linq composed query. For this case, uncommented solution was faster
            ///To try this use up method commented code on 20180412155610_AdSprocForSearch migration file. I commneted it due to pomelo
            ///connector having issue with Delimiter sentece, reference: https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/529
            //var param0 = new MySql.Data.MySqlClient.MySqlParameter()
            //{
            //    ParameterName = "@textSearch",
            //    MySqlDbType = MySql.Data.MySqlClient.MySqlDbType.VarChar,
            //    Direction = System.Data.ParameterDirection.Input,
            //    Size = 50,
            //    Value = title
            //};

            //var articles = await _articleRepository.Query()
            //.AsNoTracking()
            //.FromSql("CALL `crossblog`.`searchSproc`(@textSearch);", param0)
            /////.Take(20)
            //.Select(a => new ArticleModel
            //{
            //    Id = a.Id,
            //    Title = a.Title,
            //    Content = a.Content,
            //    Date = a.Date,
            //    Published = a.Published
            //})
            //.ToListAsync();

            //var result = new ArticleListModel
            //{
            //    Articles = articles
            //};

            //return Ok(result);


        }

        // GET articles/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var article = await _articleRepository.GetAsync(id);

            if (article == null)
            {
                return NotFound();
            }

            var result = new ArticleModel
            {
                Id = article.Id,
                Title = article.Title,
                Content = article.Content,
                Date = article.Date,
                Published = article.Published
            };

            return Ok(result);
        }

        // POST articles
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]ArticleModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var article = new Article
            {
                Title = model.Title,
                Content = model.Content,
                Date = model.Date,
                Published = model.Published
            };

            await _articleRepository.InsertAsync(article);

            return Created($"articles/{article.Id}", article);
        }

        // PUT articles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody]ArticleModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var article = await _articleRepository.GetAsync(id);

            if (article == null)
            {
                return NotFound();
            }

            article.Title = model.Title;
            article.Content = model.Content;
            article.Date = DateTime.UtcNow;
            article.Published = model.Published;

            await _articleRepository.UpdateAsync(article);

            return Ok(article);
        }

        // DELETE articles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var article = await _articleRepository.GetAsync(id);

            if (article == null)
            {
                return NotFound();
            }

            await _articleRepository.DeleteAsync(article);

            return Ok();
        }
    }
}