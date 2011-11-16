using System.Web.Mvc;

namespace App.Framework.Tests.MVC
{
    public class FilterProviderControllerMock : Controller
    {
        /// <summary>
        /// Mock action, place holder for testing the StructureMapFilterProvider. 
        /// </summary>
        [CustomErrorHandler]
        public void MockAction()
        {
        }
    }
}
