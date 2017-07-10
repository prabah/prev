using System.Threading.Tasks;
using System.Web.Http;

namespace SqlInjection.Prevention.API1.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/FileManager")]
    public class FileManagerController : ApiController
    {
        [HttpGet]
        [Route("ReadFile/{filePath}")]
        public async Task<IHttpActionResult> ReadFile(string filePath)
        {
            try
            {
                await ReadFilePath();
                return Ok();
            }
            catch (HttpResponseException)
            {
                return InternalServerError();
            }
        }

        private async Task ReadFilePath()
        {
            await Task.Delay(1000);
        }
    }
}
