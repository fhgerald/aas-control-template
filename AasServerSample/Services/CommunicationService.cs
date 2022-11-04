using NLog;
using ILogger = NLog.ILogger;

namespace DiceDigitalTwin.Services;

public class CommunicationService
{
    // TODO: Implement connection to your field bus protocol. 
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    public CommunicationService(/*DiceModbusClient diceModbusClient*/)
    {
    }

    public void Connect()
    {
        try
        {
            //_diceModbusClient.Connect();
        }
        catch (Exception e)
        {
            Logger.Error(e, "Cannot connect to MODBUS server",e);
        }
    }

    private int _producedCountTest = 0;

    public int? ProductsProducedCount => _producedCountTest++; //_diceModbusClient.GetInputRegisterValue(1);

    public string[] CurrentProductTypes { get; set; }


    public void Start()
    {
    }

    public void Stop()
    {

    }
}