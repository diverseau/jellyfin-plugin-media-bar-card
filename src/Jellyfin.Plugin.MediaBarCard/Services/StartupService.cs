using System.IO.Pipes;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using Jellyfin.Plugin.MediaBarCard.Helpers;
using Jellyfin.Plugin.MediaBarCard.JellyfinVersionSpecific;
using Jellyfin.Plugin.MediaBarCard.Model;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Playlists;
using MediaBrowser.Model.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Jellyfin.Plugin.MediaBarCard.Services
{
    public class StartupService : IScheduledTask
    {
        public string Name => "Media Bar Card Startup";

        public string Key => "Jellyfin.Plugin.MediaBarCard.Startup";
        
        public string Description => "Registers the Media Bar Card page changes with File Transformation";
        
        public string Category => "Startup Services";
        
        private readonly IServerApplicationHost m_serverApplicationHost;
        private readonly ILogger<MediaBarPlugin> m_logger;
        private readonly IUserManager m_userManager;
        private readonly IPlaylistManager m_playlistManager;

        public StartupService(IServerApplicationHost serverApplicationHost, ILogger<MediaBarPlugin> logger,
            IUserManager userManager, IPlaylistManager playlistManager)
        {
            m_serverApplicationHost = serverApplicationHost;
            m_logger = logger;
            m_userManager = userManager;
            m_playlistManager = playlistManager;
        }

        public Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
        {
            m_logger.LogInformation("Media Bar Card startup: registering file transformations");
            
            List<JObject> payloads = new List<JObject>();

            {
                JObject payload = new JObject();
                payload.Add("id", "10be76e5-d138-4bc8-8e97-248b5a63e3fa");
                payload.Add("fileNamePattern", "index.html");
                payload.Add("callbackAssembly", GetType().Assembly.FullName);
                payload.Add("callbackClass", typeof(TransformationPatches).FullName);
                payload.Add("callbackMethod", nameof(TransformationPatches.IndexHtml));
                
                payloads.Add(payload);
            }
            {
                JObject payload = new JObject();
                payload.Add("id", "069a8fcb-7d8b-4aa5-9f7a-359c5c76a87c");
                payload.Add("fileNamePattern", "avatars/list.txt");
                payload.Add("callbackAssembly", GetType().Assembly.FullName);
                payload.Add("callbackClass", typeof(TransformationPatches).FullName);
                payload.Add("callbackMethod", nameof(TransformationPatches.AvatarsList));
                
                payloads.Add(payload);
            }

            Assembly? fileTransformationAssembly =
                AssemblyLoadContext.All.SelectMany(x => x.Assemblies).FirstOrDefault(x =>
                    x.FullName?.Contains(".FileTransformation") ?? false);

            if (fileTransformationAssembly != null)
            {
                Type? pluginInterfaceType = fileTransformationAssembly.GetType("Jellyfin.Plugin.FileTransformation.PluginInterface");

                if (pluginInterfaceType != null)
                {
                    foreach (JObject payload in payloads)
                    {
                        pluginInterfaceType.GetMethod("RegisterTransformation")?.Invoke(null, new object?[] { payload });
                    }
                }
                else
                {
                    m_logger.LogWarning("Media Bar Card: this File Transformation version has no PluginInterface; the bar will not be added to the web client");
                }
            }
            else
            {
                m_logger.LogWarning("Media Bar Card: the File Transformation plugin is not installed, so the bar will not be added to the web client");
            }

            return Task.CompletedTask;
        }

        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers() => StartupServiceHelper.GetDefaultTriggers();
    }
}