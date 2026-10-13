using MediaBrowser.Controller.Library;

namespace Jellyfin.Plugin.MediaBarCard.JellyfinVersionSpecific
{
    public static class UserHelper
    {
        public static IEnumerable<Guid> GetAllUserIds(this IUserManager userManager) => userManager.GetUsersIds();
    }
}