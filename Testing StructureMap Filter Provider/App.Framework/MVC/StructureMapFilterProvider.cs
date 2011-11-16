using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Web.Mvc;

using StructureMap;

namespace App.Framework.MVC
{
    public class StructureMapFilterProvider : FilterAttributeFilterProvider
    {
        /// <summary>
        /// Structuremap instance.
        /// </summary>
        private readonly IContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="StructureMapFilterProvider"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public StructureMapFilterProvider(IContainer container)
        {
            this.container = container;
        }

        /// <summary>
        /// Intercept's GetFilters, then use the "BuildUp" feature of structuremap to avoid using decorators for property injection and this
        /// involves coupling.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="actionDescriptor">The action descriptor.</param>
        /// <returns>A list of filters for the current context.</returns>
        public override IEnumerable<Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var filters = base.GetFilters(controllerContext, actionDescriptor);

            if (filters != null)
            {
                foreach (var filter in filters)
                {
                    this.container.BuildUp(filter.Instance);
                }

                return filters;
            }

            return default(IEnumerable<Filter>);
        }
    }
}
