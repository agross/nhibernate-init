using System;

namespace NHibernate_Init.Shared
{
	public interface IReflection
	{
		Action GetInvoker(object instance, string methodName, params object[] parameters);
		Func<TResult> GetInvoker<TResult>(object instance, string methodName, params object[] parameters);
	}
}