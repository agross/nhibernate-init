using System.Collections.Generic;

using NHibernate_Init.Shared.NHibernateSupport;

namespace NHibernate_Init.Domain
{
    public class Blog : Entity
    {
        ICollection<Post> _posts = new List<Post>();

        public IEnumerable<Post> Posts
        {
            get { return _posts; }
        }

        public void AddPost(Post post)
        {
            _posts.Add(post);
        }
    }
}