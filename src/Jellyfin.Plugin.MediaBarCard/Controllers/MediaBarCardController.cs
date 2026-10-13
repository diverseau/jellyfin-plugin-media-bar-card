using System.Reflection;
using Jellyfin.Extensions;
using Jellyfin.Plugin.MediaBarCard.Configuration;
using Jellyfin.Plugin.MediaBarCard.Helpers;
using Jellyfin.Plugin.MediaBarCard.Model;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Playlists;
using Microsoft.AspNetCore.Mvc;
using MediaBrowser.Controller.Entities.TV;

namespace Jellyfin.Plugin.MediaBarCard.Controllers
{
    [Route("[controller]")]
    public class MediaBarCardController : ControllerBase
    {
        [HttpGet("{file}")]
        public ActionResult GetFile([FromRoute] string file)
        {
            Stream? fileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Jellyfin.Plugin.MediaBarCard.Inject." + file);

            if (fileStream == null)
            {
                return NotFound();
            }

            using StreamReader streamReader = new StreamReader(fileStream);
            string fileContents = streamReader.ReadToEnd();

            string contentType = "text/plain";

            if (Path.GetExtension(file) == ".js")
            {
                contentType = "text/javascript";
            }
            else if (Path.GetExtension(file) == ".css")
            {
                contentType = "text/css";
            }
            else if (Path.GetExtension(file) == ".html")
            {
                contentType = "text/html";
            }
        
            return Content(fileContents, contentType);
        }

        [HttpGet("WebConfig")]
        public ActionResult<WebConfig> GetWebConfig()
        {
            return Ok(MediaBarPlugin.Instance.Configuration.WebConfig);
        }
    }
}