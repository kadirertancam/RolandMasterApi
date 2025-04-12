using Microsoft.AspNetCore.Mvc;

namespace RolandMasterApi.Controllers
{
    /// <summary>
    /// Ana sayfa controller
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : Controller
    {
        /// <summary>
        /// Ana sayfaya yönlendirir
        /// </summary>
        [Route("/")]
        [Route("/home")]
        [Route("/index")]
        public IActionResult Index()
        {
            return Redirect("/docs/index.html");
        }

        /// <summary>
        /// API dokümantasyonu sayfasına yönlendirir
        /// </summary>
        [Route("/docs")]
        public IActionResult Docs()
        {
            return Redirect("/docs/index.html");
        }

        /// <summary>
        /// API Swagger sayfasına yönlendirir
        /// </summary>
        [Route("/api")]
        [Route("/swagger")]
        public IActionResult Swagger()
        {
            return Redirect("/swagger/index.html");
        }
    }
}
