using System;

using Machine.Specifications;

namespace NHibernate_Init.Shared.NHibernateSupport
{
	public class Person : Entity
	{
		public Person(string name)
		{
			Name = name;
		}

		public Person(int id, string name) : base(id)
		{
			Name = name;
		}

		public string Name
		{
			get;
			private set;
		}
	}

	[Behaviors]
	internal class EntityInequality
	{
#pragma warning disable 649
		protected static Person Person1;
		protected static Person Person2;
#pragma warning restore 649

		It should_be_considered_as_unequal = () => 
            ((Object) Person1).Equals(Person2).ShouldBeFalse();
		
        It should_be_considered_as_unequal_with_equality_operator = 
            () => (Person1 == Person2).ShouldBeFalse();
		
        It should_be_considered_as_unequal_with_inequality_operator =
            () => (Person1 != Person2).ShouldBeTrue();

		It should_have_symmetric_inequality =
            () => ((Object) Person2).Equals(Person1).ShouldBeFalse();

		It should_compute_a_different_hash = 
            () => Person1.GetHashCode().ShouldNotEqual(Person2.GetHashCode());
	}
	
	[Behaviors]
	internal class EntityEquality
	{
#pragma warning disable 649
		protected static Person Person1;
		protected static Person Person2;
#pragma warning restore 649

		It should_be_considered_as_equal =
            () => ((Object) Person1).Equals(Person2).ShouldBeTrue();

		It should_be_considered_as_equal_with_equality_operator = 
            () => (Person1 == Person2).ShouldBeTrue();

		It should_be_considered_as_equal_with_inequality_operator = 
            () => (Person1 != Person2).ShouldBeFalse();

		It should_have_symmetric_inequality = 
            () => ((Object) Person2).Equals(Person1).ShouldBeTrue();

		It should_compute_the_same_hash = 
            () => Person1.GetHashCode().ShouldEqual(Person2.GetHashCode());
	}

	[Subject(typeof(Entity))]
	public class When_comparing_unsaved_entities_with_identical_values
	{
		protected static Person Person1;
		protected static Person Person2;

		Establish context = () =>
			{
				Person1 = new Person("Darth Vader");
				Person2 = new Person("Darth Vader");
			};

		Behaves_like<EntityInequality> inequal_entities;
	}

	[Subject(typeof(Entity))]
	public class When_comparing_unsaved_entities_with_different_values
	{
		protected static Person Person1;
		protected static Person Person2;

		Establish context = () =>
			{
				Person1 = new Person("Darth Vader");
				Person2 = new Person("Senator Palpatine");
			};

		Behaves_like<EntityInequality> inequal_entities;
	}

	[Subject(typeof(Entity))]
	public class When_comparing_saved_entities_with_different_IDs
	{
		protected static Person Person1;
		protected static Person Person2;

		Establish context = () =>
			{
				Person1 = new Person(42, "Darth Vader");
				Person2 = new Person(43, "Senator Palpatine");
			};

		Behaves_like<EntityInequality> inequal_entities;
	}

	[Subject(typeof(Entity))]
	public class When_comparing_saved_entities_with_identical_IDs
	{
		protected static Person Person1;
		protected static Person Person2;

		Establish context = () =>
			{
				Person1 = new Person(42, "Darth Vader");
				Person2 = new Person(42, "Senator Palpatine");
			};

		Behaves_like<EntityEquality> equal_entities;
	}

	[Subject(typeof(Entity))]
	public class When_comparing_the_same_entity_with_itself
	{
		static Person Person;

		Establish context = () => { Person = new Person("Darth Vader"); };

        It should_have_reflexive_equality = 
            () => Person.Equals(Person).ShouldBeTrue();
	}
}