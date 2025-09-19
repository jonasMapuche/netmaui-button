namespace net_maui_app_v24.Services
{
    public class BatteryService
    {
        public double GetCharge() 
        {
            return (Battery.ChargeLevel * 100);
        }

        public string GetMode()
        {
            return Battery.Default.EnergySaverStatus == EnergySaverStatus.On? "On": Battery.Default.EnergySaverStatus == EnergySaverStatus.Off ? "Off": "Unknown";
        }

        public BatteryState GetState()
        {
            return Battery.Default.State;
        }

        public BatteryPowerSource GetSource()
        {
            return Battery.Default.PowerSource;
        }
    }
}
