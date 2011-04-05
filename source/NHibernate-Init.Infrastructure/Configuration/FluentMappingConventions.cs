using System;

using FluentNHibernate.Cfg;
using FluentNHibernate.Conventions.Helpers;

using NHibernate_Init.Shared;

namespace NHibernate_Init.Infrastructure.Configuration
{
	public class FluentMappingConventions : IMappingContributor
	{
		public void Apply(MappingConfiguration configuration)
		{
			var conventions = configuration.FluentMappings.Conventions;

			conventions.Add(ConventionBuilder
			                	.Class
			                	.Always(x => x.Table(Inflector.Net.Inflector.Pluralize(x.EntityType.Name).Escape())));

			conventions.Add(ConventionBuilder
			                	.Id
								.Always(x => x.Column(String.Format("{0}ID", x.EntityType.Name).Escape())));

			conventions.Add(ConventionBuilder.Reference.Always(x =>
				{
					x.Column(String.Format("{0}ID", x.Property.Name).Escape());
					x.ForeignKey(String.Format("FK_{0}To{1}", x.EntityType.Name, x.Property.Name));
				}));

			conventions.Add(ConventionBuilder.HasMany.Always(x =>
				{
					x.Key.Column(String.Format("{0}ID", x.Key.EntityType.Name).Escape());
					x.Key.ForeignKey(String.Format("FK_{0}To{1}",
					                               x.Relationship.StringIdentifierForModel,
												   x.Relationship.EntityType.Name).Escape());
				}));

			conventions.Add(ConventionBuilder.Property.Always(x => x.Column(x.Property.Name.Escape())));

			conventions.Add(DefaultLazy.Always());
		}
	}
}