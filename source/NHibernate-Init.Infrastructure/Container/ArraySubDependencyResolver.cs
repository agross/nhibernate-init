using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;

namespace NHibernate_Init.Infrastructure.Container
{
	internal class ArraySubDependencyResolver : ISubDependencyResolver
	{
		readonly IKernel _kernel;

		public ArraySubDependencyResolver(IKernel kernel)
		{
			_kernel = kernel;
		}

		public object Resolve(CreationContext context,
		                      ISubDependencyResolver parentResolver,
		                      ComponentModel model,
		                      DependencyModel dependency)
		{
			var targetType = dependency.TargetType.GetElementType();
			var resolved = _kernel.ResolveAll(targetType, null);
			return resolved;
		}

		public bool CanResolve(CreationContext context,
		                       ISubDependencyResolver parentResolver,
		                       ComponentModel model,
		                       DependencyModel dependency)
		{
			return dependency.TargetType != null &&
			       dependency.TargetType.IsArray &&
			       dependency.TargetType.GetElementType().IsInterface &&
				   !model.Parameters.Contains(dependency.DependencyKey);
		}
	}
}