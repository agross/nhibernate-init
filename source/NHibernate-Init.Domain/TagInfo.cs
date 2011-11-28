using NHibernate_Init.Shared.NHibernateSupport;

namespace NHibernate_Init.Domain
{
	public class TagInfo : Entity
	{
		public virtual string Tags
		{
			get;
			set;
		}
	}
}