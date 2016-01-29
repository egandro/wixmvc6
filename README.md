Sample WIX based installer for ASP.Net / MVC 6 Applications

In their great wisdom Microsoft decided to do almost everything different as in previous
ASP.Net versions. That - of course - includes the handling of the Setup and the
deployment to IIS. (Sorry I don't care about Azure. It's impossible for my clients
to share their data with the USA / CIA / NSA / any A...).

We get a simple instruction set on how to handle IIS based setups. 

http://docs.asp.net/en/latest/publishing/iis.html

This doesn't cover all scenarios. The MS tutorial teaches you how to install the MVC6
on a new Website e.g. an IP / HostName / Port (or any combination of this). HowevHower
there is no way to install it in a Virtual Directory to the DefaultWebSite as previous
Setup Tools (e.g. from old VS2010 times).

How it works:

We have three components:

A) IIS Manager
B) MSI Project
C) Bundle Project
+ The MVC Application you want to deploy.

IIS Manager

- detects the IIS Applications Pools (note this only works for IIS >7.5 - but MVC6 needs IIS8 anyway)
- some dirty json manipulation code

MSI Project

- we have a custom dialog here for the IIS Settings (virdual directory or website installation mode)
- we unlock the SystemWebServer Handlers in IIS (required according to the ASP.Net IIS Installation document from Microsoft)

Bundle Project

- chains the httpPlatformHandler and the MSI project
- the bundle project is required to elevate the permissions for the msi project to query IIS settings
with the IIS manager

Required preparations to your MVC application:

1) Add a filesystem based publishing profile. In the sample application it is published
to ..\publish\WebApplication1. Make sure that the application works, has every DLL included
and references all needed DLLs (in the Nuget format that MVC6 requries).

2) Publish the Project and run harvest.cmd in the MSI Project. Harvesting will happen to the
published dir. We have to use some XSL Transformations to fix the target directories,
as $var.ProjectName.TargetDir doesn't exist in MVC6 (no bin\Debug, bin\Release). Yeah!

Please note: you might want to add some automatisation here via build events. This is left as
homework for the reader.

3) In order to get VirtualDirectory support, you have to tweak your Startup.cs

3.1) Rename the "Configure" method to "ConfigureWrapper", make it private or protected
3.2) Add this new Configure method:
-- snipp --
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // add virtual directory handling based on the configuration

            var virtualDirectory = Configuration.Get<string>("Hosting:VirtualDirectory");

            if (virtualDirectory == null)
                virtualDirectory = "";

            virtualDirectory = virtualDirectory.Trim();

            if (virtualDirectory.Length > 0)
            {
                if (virtualDirectory.EndsWith("/"))
                    virtualDirectory.Substring(0, virtualDirectory.Length - 1);
                virtualDirectory = "/" + virtualDirectory;

                app.Map(virtualDirectory, (app1) => ConfigureWrapped(app1, env, loggerFactory));
            }
            else
            {
                ConfigureWrapped(app, env, loggerFactory);
            }
        }
-- snapp --

3.3) Add a section to your appsettings.json File:

-- snipp --
  "Hosting": {
    "VirtualDirectory": ""
  },  
-- snapp --

The WiX Setup will add the VirtualDirectory string of the IIS Setup Dialog to this key when doing
a VirtualDirectory based installation.

If you want to change / add other stuff - e.g. a DB Connection string,feel free to use/enhance the 
JsonFileAppsettings CustomAction. Note to WIX team: We need a "util:JsonFile" method - my version is a mess.


