using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using StructureMap;

namespace App.Framework.MVC
{
    public class StructureMapDependencyResolver : IDependencyResolver
    {
        /// <summary>
        /// An injected instance of the IContainer
        /// </summary>
        private readonly IContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="StructureMapDependencyResolver"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public StructureMapDependencyResolver(IContainer container)
        {
            this.container = container;
        }

        /// <summary>
        /// Returns the type from the container (called by MVC).
        /// </summary>
        /// <param name="serviceType">The type (interface normally) to locate.</param>
        /// <remarks>If the type is not found within the container then null is returned. This then tells MVC to surface this as an error to the user.</remarks>
        /// <returns>The service object</returns>
        public object GetService(Type serviceType)
        {
            try
            {
                return this.container.GetInstance(serviceType);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Returns multiple types registered with the container for the passed type.
        /// </summary>
        /// <param name="serviceType">The type of the requested services.</param>
        /// <returns>
        /// The requested services.
        /// </returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return this.container.GetAllInstances(serviceType).Cast<object>();
            }
            catch
            {
                return new List<object>();
            }
        }
    }
}
