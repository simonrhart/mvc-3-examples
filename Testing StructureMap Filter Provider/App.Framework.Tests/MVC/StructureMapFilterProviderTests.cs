using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using App.Framework.MVC;
using Moq;
using NUnit.Framework;
using StructureMap;

namespace App.Framework.Tests.MVC
{
    /// <summary>
    /// Tests for the custom structure map filter provider.
    /// </summary>
    [TestFixture]
    public class StructureMapFilterProviderTests
    {
        /// <summary>
        /// The Container instance used for testing.
        /// </summary>
        private Mock<IContainer> containerMock;

        /// <summary>
        /// Our filter provider test instance.
        /// </summary>
        private StructureMapFilterProvider filterProvider;

        /// <summary>
        /// The filter to test.
        /// </summary>
        private Filter customFilter;

        /// <summary>
        /// Collection to use for out filter provider stub.
        /// </summary>
        private List<Filter> customFilterCollection;

        /// <summary>
        /// The custom action filter to test.
        /// </summary>
        private CustomErrorHandlerAttribute customActionFilter;

        /// <summary>
        /// The HTTP context base mock.
        /// </summary>
        private Mock<HttpContextBase> httpContextMock;

        /// <summary>
        /// Sets up this instance.
        /// </summary>
        [TestFixtureSetUp]
        public void Setup()
        {
            this.customActionFilter = new CustomErrorHandlerAttribute();
            this.customFilter = new Filter(this.customActionFilter, FilterScope.Action, 1);
            this.customFilterCollection = new List<Filter>();
            this.customFilterCollection.Add(this.customFilter);
            this.containerMock = new Mock<IContainer>();
            this.filterProvider = new StructureMapFilterProvider(this.containerMock.Object);
            this.httpContextMock = new Mock<HttpContextBase>();
        }

        /// <summary>
        /// This test ensures that the action filter returned from MVC can be intercepted using our custom filter provider
        /// and when it does, it injects any dependencies via property injection (ctor injection is not possible via attributes)
        /// </summary>
        [Test]
        public void CanInterceptCreationOfFilters()
        {
            // Arrange
            this.containerMock.Setup(x => x.BuildUp(this.customActionFilter));

            var controllerMock = new FilterProviderControllerMock();
            var routeData = new RouteData();

            // as the FilterProvider derives from FilterAttributeFilterProvider and this class has a GetFilters method on it, we have to setup
            // the ControllerContext and the ActionDescriptor - there is no other way of testing filter providers even using Moq.
            var controllerContext = new ControllerContext(this.httpContextMock.Object, routeData, controllerMock);

            ControllerDescriptor controllerDescriptor = new ReflectedControllerDescriptor(typeof(FilterProviderControllerMock));

            var mockActionMethodInfo = controllerMock.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(x => x.Name.Equals("MockAction")).FirstOrDefault();
            ActionDescriptor actionDescriptor = new ReflectedActionDescriptor(mockActionMethodInfo, "MockAction", controllerDescriptor);

            // Act
            var filters = this.filterProvider.GetFilters(controllerContext, actionDescriptor);

            // Assert
            // ensure the custom filter provider "buildup" the filter. This injects any dependencies into the filter using structuremap without having to 
            // bind directly to structuremap within the filter using the [Inject] decorator. That would work, but this is nicer.
            this.containerMock.Verify(x => x.BuildUp(this.customActionFilter), Times.Once());
        }

        /// <summary>
        /// Checks that when the filter provider is called, that an empty filter list is returned.
        /// </summary>
        [Test]
        public void ShouldReturnNoResultsOnFilterNotFound()
        {
            var controllerMock = new FilterProviderControllerMock();
            var controllerContext = new ControllerContext(this.httpContextMock.Object, new RouteData(), controllerMock);

            ControllerDescriptor controllerDescriptor = new ReflectedControllerDescriptor(typeof(FilterProviderControllerMock));

            var mockActionMethodInfo = controllerMock.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(x => x.Name.Equals("Dispose")).FirstOrDefault();
            ActionDescriptor actionDescriptor = new ReflectedActionDescriptor(mockActionMethodInfo, "Dispose", controllerDescriptor);

            var filters = this.filterProvider.GetFilters(controllerContext, actionDescriptor);

            Assert.IsTrue(filters.Count() == 0);
        }
    }
}
