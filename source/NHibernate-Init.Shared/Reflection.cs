using System;
using System.Reflection;

namespace NHibernate_Init.Shared
{
	public class Reflection : IReflection
	{
		public Action GetInvoker(object instance, string methodName, params object[] parameters)
		{
			return () => GetMethod(instance, methodName).Invoke(instance, parameters);
		}

		public Func<TResult> GetInvoker<TResult>(object instance, string methodName, params object[] parameters)
		{
			return () => (TResult) GetMethod(instance, methodName).Invoke(instance, parameters);
		}

		static MethodBase GetMethod(object instance, string methodName)
		{
			var type = instance.GetType();
			return type.GetMethod(methodName);
		}
	}
}