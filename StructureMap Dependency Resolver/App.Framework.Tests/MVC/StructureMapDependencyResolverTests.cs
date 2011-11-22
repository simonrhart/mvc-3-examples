using System.Collections.Generic;
using System.Linq;
using App.Framework.MVC;
using NUnit.Framework;
using StructureMap;

namespace App.Framework.Tests.MVC
{
    [TestFixture]
    public class StructureMapDependencyResolverTest
    {
        /// <summary>
        /// The StructureMapDependencyResolver instance used for testing.
        /// </summary>
        private StructureMapDependencyResolver structureMapDependencyResolver;

        /// <summary>
        /// The Container instance used for testing.
        /// </summary>
        private Container container;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [TestFixtureSetUp]
        public void Setup()
        {
            this.container = new Container();
            this.container.Configure(x => x.For<IFoobar>().Use<Foobar>());
            this.structureMapDependencyResolver = new StructureMapDependencyResolver(this.container);
        }

        /// <summary>
        /// Shoulds the type of the get single.
        /// </summary>
        [Test]
        public void ShouldGetSingleType()
        {
            var foobar = this.structureMapDependencyResolver.GetService(typeof(IFoobar)) as Foobar;

            Assert.AreEqual(typeof(Foobar), foobar.GetType());
        }

        /// <summary>
        /// Shoulds the return null for missing single lookup.
        /// </summary>
        [Test]
        public void ShouldReturnNullForMissingSingleLookup()
        {
            var foobar2 = this.structureMapDependencyResolver.GetService(typeof(IFoobar2));

            Assert.IsNull(foobar2);
        }

        /// <summary>
        /// Shoulds the return multiple types.
        /// </summary>
        [Test]
        public void ShouldReturnMultipleTypes()
        {
            IEnumerable<object> foobars = this.structureMapDependencyResolver.GetServices(typeof(IFoobar));

            IList<object> list = foobars.ToList();
            Assert.IsTrue(list.Count == 1 && list[0].GetType() == typeof(Foobar));
        }

        /// <summary>
        /// Shoulds the return empty collection for missing multiple lookup.
        /// </summary>
        [Test]
        public void ShouldReturnEmptyCollectionForMissingMultipleLookup()
        {
            IEnumerable<object> foobars = this.structureMapDependencyResolver.GetServices(typeof(IFoobar2));

            IList<object> list = foobars.ToList();
            Assert.IsTrue(list.Count == 0);
        }
    }

}
