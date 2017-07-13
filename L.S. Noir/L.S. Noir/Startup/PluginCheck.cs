using System;
using System.Linq;
using LSPD_First_Response.Mod.API;

namespace LSNoir.Startup
{
    class PluginCheck
    {
        public static bool IsLspdfrPluginRunning(string plugin, Version minversion = null) => Functions.GetAllUserPlugins().Select(assembly => assembly.GetName()).Where(an => an.Name.ToLower() == plugin.ToLower()).Any(an => minversion == null || an.Version.CompareTo(minversion) >= 0);
    }
}
