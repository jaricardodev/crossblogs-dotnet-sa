using crossblog.Domain;

namespace crossblog.Repositories
{
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        public CommentRepository(CrossBlogDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}