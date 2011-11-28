using FluentNHibernate.Mapping;

namespace NHibernate_Init.Domain.Mapping
{
	class BlogMap :ClassMap<Blog>
	{
		public BlogMap()
		{
			Id(x => x.Id).GeneratedBy.HiLo("3");

			HasMany(x => x.Posts).Cascade.All();
		}
	}
}