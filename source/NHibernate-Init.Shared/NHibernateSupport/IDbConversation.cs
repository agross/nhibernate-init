namespace NHibernate_Init.Shared.NHibernateSupport
{
	public interface IDbConversation
	{
		void Insert(object entity);

		TResult Query<TResult>(IDomainQuery<TResult> query);
	}
}