using System;
using System.Reflection;

using FluentNHibernate.Cfg;

namespace NHibernate_Init.Infrastructure.Configuration
{
	public class FluentMappingsFromAssembly : IMappingContributor
	{
		readonly Assembly _assembly;

		public FluentMappingsFromAssembly(Type assemblyContaining)
		{
			_assembly = assemblyContaining.Assembly;
		}

		public void Apply(MappingConfiguration configuration)
		{
			configuration.FluentMappings.AddFromAssembly(_assembly);
		}
	}
}