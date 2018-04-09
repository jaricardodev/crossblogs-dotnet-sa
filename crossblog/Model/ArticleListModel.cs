using System.Collections.Generic;

namespace crossblog.Model
{
    public class ArticleListModel
    {
        public IEnumerable<ArticleModel> Articles { get; set; }
    }
}