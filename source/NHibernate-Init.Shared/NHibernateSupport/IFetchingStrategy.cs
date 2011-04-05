namespace NHibernate_Init.Shared.NHibernateSupport
{
	public interface IFetchingStrategy<TResult>
	{
		TResult Fetch();
	}
}