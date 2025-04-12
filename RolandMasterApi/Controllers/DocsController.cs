using Microsoft.AspNetCore.Mvc;

namespace RolandMasterApi.Controllers
{
    /// <summary>
    /// API dokümantasyonu için controller
    /// </summary>
    [ApiController]
    [Route("api/docs")]
    public class DocsController : ControllerBase
    {
        /// <summary>
        /// API dokümantasyonuna yönlendirir
        /// </summary>
        /// <returns>Dokümantasyon sayfasına yönlendirme</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status302Found)]
        public IActionResult GetDocs()
        {
            return Redirect("/docs/index.html");
        }

        /// <summary>
        /// API dokümanını JSON formatında getirir
        /// </summary>
        /// <returns>API dokümanı (OpenAPI)</returns>
        [HttpGet("openapi")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetOpenApiDocument()
        {
            return Redirect("/swagger/v1/swagger.json");
        }
    }
}
