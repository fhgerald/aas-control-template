using BaSyx.API.Components;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Core.Common;
using BaSyx.Models.Export;
using BaSyx.Utils.ResultHandling;
using NLog;
using ILogger = NLog.ILogger;

// Load AAS from file and connect methods

namespace DiceDigitalTwin.Services
{
    public sealed class AssetAdministrationShellService : AssetAdministrationShellServiceProvider
    {
        private readonly CommunicationService _communicationService;
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public AssetAdministrationShellService(CommunicationService communicationService)
        {
            _communicationService = communicationService;
            
            // TODO: This is only a sample to bind to operations of sub model "Operations"
            var operationsServiceProvider = new SubmodelServiceProvider();
            operationsServiceProvider.BindTo(AssetAdministrationShell.Submodels["Operations"]);
            operationsServiceProvider.RegisterMethodCalledHandler("Start", StartOperationHandler);
            operationsServiceProvider.RegisterMethodCalledHandler("Stop", StopOperationHandler);
            operationsServiceProvider.RegisterSubmodelElementHandler("ProducedCount",
                 new SubmodelElementHandler(ProducedCountGetHandler, null));
            RegisterSubmodelServiceProvider("Operations", operationsServiceProvider);
            
            var generalInformationServiceProvider = new SubmodelServiceProvider();
            generalInformationServiceProvider.BindTo(AssetAdministrationShell.Submodels["GeneralInformation"]);
            generalInformationServiceProvider.UseInMemorySubmodelElementHandler();
            RegisterSubmodelServiceProvider("GeneralInformation", generalInformationServiceProvider);
        }

        private IValue ProducedCountGetHandler(ISubmodelElement submodelElement)
        {
            return new ElementValue<int?>(_communicationService.ProductsProducedCount);
        }

        private Task<OperationResult> StopOperationHandler(IOperation operation, IOperationVariableSet inputarguments, IOperationVariableSet inoutputarguments, IOperationVariableSet outputarguments, CancellationToken cancellationtoken)
        {
            _communicationService.Stop();
            return Task.FromResult(new OperationResult(true));
        }

        private Task<OperationResult> StartOperationHandler(IOperation operation, IOperationVariableSet inputarguments, IOperationVariableSet inoutputarguments, IOperationVariableSet outputarguments, CancellationToken cancellationtoken)
        {
            _communicationService.Start();
            return Task.FromResult(new OperationResult(true));
        }

        public override IAssetAdministrationShell? BuildAssetAdministrationShell()
        {
            // TODO: Add YOUR AASX file here.
            string aasxFilePath = "Content/Template.aasx";
            
            
            using (AASX aasx = new AASX(aasxFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                AssetAdministrationShellEnvironment_V2_0 environment = aasx.GetEnvironment_V2_0();
                if (environment == null)
                {
                    Logger.Error("Asset Administration Shell Environment cannot be obtained from AASX-Package " + aasxFilePath);
                    return null;
                }

                Logger.Info("AASX-Package successfully loaded");

                if (environment.AssetAdministrationShells.Count != 0)
                {
                    var aas = environment.AssetAdministrationShells.FirstOrDefault();
                    return aas;
                }

                Logger.Error("No Asset Administration Shells found AASX-Package " + aasxFilePath);
                return null;
            }
        }
    }
}