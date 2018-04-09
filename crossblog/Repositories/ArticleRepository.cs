using crossblog.Domain;

namespace crossblog.Repositories
{
    public class ArticleRepository : GenericRepository<Article>, IArticleRepository
    {
        public ArticleRepository(CrossBlogDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}