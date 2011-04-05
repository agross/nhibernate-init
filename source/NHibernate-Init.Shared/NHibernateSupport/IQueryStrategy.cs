namespace NHibernate_Init.Shared.NHibernateSupport
{
	public interface IQueryStrategy<T>
	{
		void Apply(T query);
	}
}