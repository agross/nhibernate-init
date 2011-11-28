using System.Collections.Generic;

using NHibernate_Init.Shared.NHibernateSupport;

namespace NHibernate_Init.Domain
{
    public class Blog : Entity
    {
    	readonly ICollection<Post> _posts = new List<Post>();

        public virtual IEnumerable<Post> Posts
        {
            get { return _posts; }
        }

		public virtual void AddPost(Post post)
        {
            _posts.Add(post);
        }
    }
}