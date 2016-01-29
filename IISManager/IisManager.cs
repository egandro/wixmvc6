using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Web.Administration;
using Microsoft.Win32;

namespace IISManager
{
    public static class IisManager
    {
        public static int IisVersion
        {
            get
            {
                int iisVersion;

                using (var iisKey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\InetStp"))
                {
                    if (iisKey == null)
                        throw new Exception("IIS is not installed.");

                    iisVersion = (int)iisKey.GetValue("MajorVersion");
                }

                return iisVersion;
            }
        }

        #region WebSites

        /// <summary>
        /// gets IIS WebSites (only works this way for IIS > 7)
        /// </summary>
        /// <returns></returns>
        public static IList<IisWebSite> GetIisWebSites()
        {
            if (IisVersion < 7)
                throw new Exception("Your IIS is to old. You need a Windows with 8.0+");

            var sites = new List<IisWebSite>();

            using (var iisManager = new ServerManager())
            {
                sites.AddRange(iisManager.Sites.Select
                    (s => new IisWebSite(s.Id.ToString(CultureInfo.InvariantCulture), s.Name)));
            }

            return sites;
        }

        #endregion

        #region App Pools

        /// <summary>
        /// gets IIS Application Pools (only works this way for IIS > 7)
        /// </summary>
        /// <returns></returns>
        public static IList<string> GetIisAppPools()
        {
            if (IisVersion < 7)
                throw new Exception("Your IIS is to old. You need a Windows with IIS 8.0+");

            var pools = new List<string>();

            using (var iisManager = new ServerManager())
            {
                pools.AddRange(iisManager.ApplicationPools.Select(p => p.Name));
            }

            return pools;
        }

        #endregion
    }
}
