using System.Linq;

using Castle.Windsor;

using NHibernate_Init.Shared;
using NHibernate_Init.Shared.NHibernateSupport;

namespace NHibernate_Init.Infrastructure.Persistence
{
	public class NHibernateDbConversation : IDbConversation
	{
		readonly IUnitOfWork _unitOfWork;

		public NHibernateDbConversation(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IWindsorContainer Container
		{
			get;
			set;
		}

		public IReflection Reflection
		{
			get;
			set;
		}

		public void Insert(object entity)
		{
			_unitOfWork.CurrentSession.Save(entity);
		}

		public TResult Query<TResult>(IDomainQuery<TResult> query)
		{
			ApplyQueryStrategies(query);

			return query.Execute(_unitOfWork.CurrentSession);
		}

		void ApplyQueryStrategies<TResult>(IDomainQuery<TResult> query)
		{
			if (Container == null || Reflection == null)
			{
				return;
			}

			var strategiesToApply = query
				.GetType()
				.GetInterfaces()
				.Select(x => typeof(IQueryStrategy<>).MakeGenericType(x))
				.Select(strategyType => Container.ResolveAll(strategyType))
				.Where(strategies => strategies != null);

			foreach (var strategySet in strategiesToApply)
			{
				foreach (var strategy in strategySet)
				{
					try
					{
						var invoker = Reflection.GetInvoker(strategy, "Apply", query);
						invoker();
					}
					finally
					{
						Container.Release(strategy);
					}
				}
			}
		}
	}
}