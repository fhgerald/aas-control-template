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
            // TODO: This is only a sample to register two methods to sub model "Operations";
            //       in your case these methods (ansd handlers) should be replaced with
            //       your own method names and handlers
            operationsServiceProvider.RegisterMethodCalledHandler("Method1", Method1OperationHandler);
            operationsServiceProvider.RegisterMethodCalledHandler("Method2", Method2OperationHandler);
            // TODO: This is only a sample to register properties of sub model "Operations"
            operationsServiceProvider.RegisterSubmodelElementHandler("Property1",
                                                  new SubmodelElementHandler(Property1GetHandler, null));
            operationsServiceProvider.RegisterSubmodelElementHandler("Property2",
                                                  new SubmodelElementHandler(Property2GetHandler, Property2SetHandler));
            // now register the sub model "Operations" to the AAS
            RegisterSubmodelServiceProvider("Operations", operationsServiceProvider);

            // TODO: This is only a sample to bind to operations of sub model "GeneralInformation"
            var generalInformationServiceProvider = new SubmodelServiceProvider();
            generalInformationServiceProvider.BindTo(AssetAdministrationShell.Submodels["GeneralInformation"]);
            generalInformationServiceProvider.UseInMemorySubmodelElementHandler();
            // now register the sub model "GeneralInformation" to the AAS
            RegisterSubmodelServiceProvider("GeneralInformation", generalInformationServiceProvider);
        }

        private IValue Property1GetHandler(ISubmodelElement submodelElement)
        {
            // TODO: This is a demo code only; implement your own getter here
            //       and return the value of the property retrieved from the
            //       communcation service property1
            return new ElementValue<int?>(_communicationService.Property1);
        }

        private IValue Property2GetHandler(ISubmodelElement submodelElement)
        {
            // TODO: This is a demo code only; implement your own getter here
            var strarr = _communicationService.Property2;
            var str = string.Join("|", strarr);
            return new ElementValue<string?>(str);
        }

        private void Property2SetHandler(ISubmodelElement submodelElement, IValue value)
        {
            // TODO: This is a demo code only; implement your own setter here
            var strarr = value.ToString().Split("|");
            _communicationService.Property2 = strarr;
        }

        private Task<OperationResult> Method1OperationHandler(IOperation operation, IOperationVariableSet inputarguments, IOperationVariableSet inoutputarguments, IOperationVariableSet outputarguments, CancellationToken cancellationtoken)
        {
            // TODO: This is a demo code only; call your own method here
            _communicationService.CallExternalMethod1();
            return Task.FromResult(new OperationResult(true));
        }

        private Task<OperationResult> Method2OperationHandler(IOperation operation, IOperationVariableSet inputarguments, IOperationVariableSet inoutputarguments, IOperationVariableSet outputarguments, CancellationToken cancellationtoken)
        {
            // TODO: This is a demo code only; call your own method here
            _communicationService.CallExternalMethod2();
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