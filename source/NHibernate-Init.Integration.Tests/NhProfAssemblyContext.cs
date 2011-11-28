using HibernatingRhinos.Profiler.Appender.NHibernate;

using Machine.Specifications;

namespace NHibernate_Init.Integration.Tests
{
	public class NhProfAssemblyContext : IAssemblyContext
	{
		public void OnAssemblyStart()
		{
			NHibernateProfiler.Initialize();
		}

		public void OnAssemblyComplete()
		{
			NHibernateProfiler.Stop();
		}
	}
}