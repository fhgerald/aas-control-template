using NLog;
using ILogger = NLog.ILogger;

namespace DiceDigitalTwin.Services;

// Service which holds the connection to 
public class CommunicationService
{
    // TODO: Implement connection to your field bus protocol. 
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    public CommunicationService(/* .... DiceModbusClient diceModbusClient ... */)
    {
        // TODO: Save connector (client) to communicate with your field bus server
    }

    public void Connect()
    {
        try
        {
            // TODO: Initialize connection to your field bus protocol
            // ie. _diceModbusClient.Connect();
        }
        catch (Exception e)
        {
            Logger.Error(e, "Cannot connect to MODBUS server",e);
        }
    }

    // TODO: Counter for sample only;
    //       probably not necessary for your implementation, because
    //       your properties will be retrieved via the field bus protocol
    private int _exampleCounter = 0;


    // TODO: Example for property1 getter
    public int? Property1 => _exampleCounter++; // TODO: replace this with your code

    // TODO: Example for property2 getter and setter
    public string[] Property2 {
        get { 
            /* TODO: insert your code here */ 
            return new string[] { "Sample1", "Sample2" }; 
        }
        set { 
            /* TODO: insert your code here */ 
        }
    }

    // TODO: Example to call a external method
    public void CallExternalMethod1()
    {
        // TODO: insert your code here; ie. start the remote machine via field bus
    }

    // TODO: Example to call a method
    public void CallExternalMethod2()
    {
        // TODO: insert your code here; ie. to stop the remote machine via field bus
    }
}