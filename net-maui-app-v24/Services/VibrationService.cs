namespace net_maui_app_v24.Services
{
    public class VibrationService
    {
        public void SetVibration(int time)
        {
            int secondsToVibrate = Random.Shared.Next(1, time);
            TimeSpan vibrationLength = TimeSpan.FromSeconds(secondsToVibrate);

            Vibration.Default.Vibrate(vibrationLength);
        }
    }
}
