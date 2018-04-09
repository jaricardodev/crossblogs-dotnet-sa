using System.Collections.Generic;

namespace crossblog.Model
{
    public class CommentListModel
    {
        public IEnumerable<CommentModel> Comments { get; set; }
    }
}