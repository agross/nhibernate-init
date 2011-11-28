using NHibernate_Init.Shared.NHibernateSupport;

namespace NHibernate_Init.Domain
{
	public class Post : Entity
	{
		public virtual string Title
		{
			get;
			set;
		}

		public virtual TagInfo Tags
		{
			get;
			set;
		}
	}
}