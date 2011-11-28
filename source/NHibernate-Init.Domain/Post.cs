using NHibernate_Init.Shared.NHibernateSupport;

namespace NHibernate_Init.Domain
{
	public class Post : Entity
	{
		public string Title
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

	public class TagInfo : Entity
	{
		public string Tags
		{
			get;
			set;
		}
	}
}