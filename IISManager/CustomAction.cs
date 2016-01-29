using System;
using Microsoft.Deployment.WindowsInstaller;
using System.IO;
using Newtonsoft.Json.Linq;

namespace IISManager
{
    public static class CustomActions
    {
        [CustomAction]
        public static ActionResult GetWebSites(Session session)
        {
            try
            {
                if (session == null) { throw new ArgumentNullException("session"); }

                var comboBoxView = session.Database.OpenView("select * from ComboBox");

                int order = 1;
                string first = null;

                foreach (var site in IisManager.GetIisWebSites())
                {
                    var newComboRecord = new Record("WEBSITEVALUE", order++, site.ID, site.Name);
                    comboBoxView.Modify(ViewModifyMode.InsertTemporary, newComboRecord);
                    if (string.IsNullOrWhiteSpace(first))
                        first = site.ID;
                }

                if (first != null)
                    session["WEBSITEVALUE"] = first;

                return ActionResult.Success;
            }
            catch (Exception ex)
            {
                if (session != null)
                    session.Log("Custom Action Exception: " + ex);

                return ActionResult.Failure;
            }
        }

        [CustomAction]
        public static ActionResult GetAppPools(Session session)
        {
            try
            {
                if (session == null) { throw new ArgumentNullException("session"); }

                var comboBoxView = session.Database.OpenView("select * from ComboBox");

                int order = 1;
                string first = null;

                foreach (var appPool in IisManager.GetIisAppPools())
                {
                    var newComboRecord = new Record("APPPOOLVALUE", order++, appPool);
                    comboBoxView.Modify(ViewModifyMode.InsertTemporary, newComboRecord);
                    if (string.IsNullOrWhiteSpace(first))
                        first = appPool;
                }

                if (first != null)
                    session["APPPOOLVALUE"] = first;
               
                return ActionResult.Success;
            }
            catch (Exception e)
            {
                if (session != null)
                    session.Log("Custom Action Exception " + e);
            }

            return ActionResult.Failure;
        }

        [CustomAction]
        public static ActionResult JsonFileAppsettings(Session session)
        {
            try
            {
                if (session == null) { throw new ArgumentNullException("session"); }

                CustomActionData data = null;

                if(session["INSTALLATIONMODE"] == "virtualdirectory")
                    data = new CustomActionData(session["JsonFileAppsettingsVirtualDirectory"]);
                else
                    data = new CustomActionData(session["JsonFileAppsettingsWebsite"]);

                string file = data.ContainsKey("File") ? data["File"] : "";
                string rootKey = data.ContainsKey("RootKey") ? data["RootKey"] : "";
                string key = data.ContainsKey("Key") ? data["Key"] : "";
                string value = data.ContainsKey("Value") ? data["Value"] : "";
                if (value.IndexOf("[") > -1 && value.IndexOf("]") > -1)
                    value = session.Format(data["Value"]);

                var dir = session["INSTALLDIR"];
                if (dir == null)
                    return ActionResult.Success;

                file = Path.Combine(dir, file);
                if ( !File.Exists(file))
                    return ActionResult.Success;

                ReplaceInJsonFile(file, rootKey, key, value);

                return ActionResult.Success;
            }
            catch (Exception e)
            {
                if (session != null)
                    session.Log("Custom Action Exception " + e);
            }

            return ActionResult.Failure;
        }

        private static void ReplaceInJsonFile(string file, string parent, string key, string value)
        {
            var json = File.ReadAllText(file);

            var jo = JObject.Parse(json);
            var token = jo.SelectToken("$." + parent);

            if (token == null)
                return;

            token[key] = value;

            json = jo.ToString();

            File.WriteAllText(file,json);
        }
    }
}
