using NHibernate;

namespace NHibernate_Init.Shared.NHibernateSupport
{
	public interface IDomainQuery<TResult>
	{
		TResult Execute(ISession session);
	}
}