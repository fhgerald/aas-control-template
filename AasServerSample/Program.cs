using BaSyx.AAS.Server.Http;
using BaSyx.API.Components;
using BaSyx.Common.UI;
using BaSyx.Common.UI.Swagger;
using BaSyx.Registry.Client.Http;
using BaSyx.Utils.Settings.Sections;
using BaSyx.Utils.Settings.Types;
using DiceDigitalTwin.Services;
using NLog;
using NLog.Web;
using ILogger = NLog.ILogger;

ILogger logger = LogManager.GetCurrentClassLogger();

logger.Info("Starting AssetAdministrationShell's HTTP server...");

//Loading server configurations settings
ServerSettings aasRepositorySettings = ServerSettings.CreateSettings();
aasRepositorySettings.ServerConfig.Hosting.ContentPath = "Content";
aasRepositorySettings.ServerConfig.Hosting.Urls.Add("http://+:5080");
aasRepositorySettings.ServerConfig.Hosting.Urls.Add("https://+:5443");

//Initialize generic HTTP-REST interface passing previously loaded server configuration
AssetAdministrationShellHttpServer server = new AssetAdministrationShellHttpServer(aasRepositorySettings);

//Configure the entire application to use your own logger library (here: Nlog)
server.WebHostBuilder.UseNLog();

//Instantiate Asset Administration Shell Service
var diceCommunicationService = new CommunicationService();
diceCommunicationService.Connect();
var shellService = new AssetAdministrationShellService(diceCommunicationService);

//Dictate Asset Administration Shell service to use provided endpoints from the server configuration
shellService.UseAutoEndpointRegistration(aasRepositorySettings.ServerConfig);

//Assign Asset Administration Shell Service to the generic HTTP-REST interface
server.SetServiceProvider(shellService);

//Add Swagger documentation and UI
server.AddSwagger(Interface.AssetAdministrationShell);

//Add BaSyx Web UI
server.AddBaSyxUI(PageNames.AssetAdministrationShellServer);

//Action that gets executed when server is fully started
server.ApplicationStarted = () =>
{
    var result = shellService.RegisterAssetAdministrationShell(new RegistryClientSettings
    {
        RegistryConfig =
        {
            RegistryUrl = "http://localhost:4000/registry"
        }
    });
    if (result.IsException == true)
    {
        logger.Error(result.Messages);
        return;
    }
    logger.Info("Successfully connected to registry.");
};


//Run HTTP server
server.Run();           