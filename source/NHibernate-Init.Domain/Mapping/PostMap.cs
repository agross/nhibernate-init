using FluentNHibernate.Mapping;

namespace NHibernate_Init.Domain.Mapping
{
	class PostMap : ClassMap<Post>
	{
		public PostMap()
		{
			Id(x => x.Id).GeneratedBy.HiLo("3");

			Map(x => x.Title);
			HasOne(x => x.Tags);
		}
	}
}