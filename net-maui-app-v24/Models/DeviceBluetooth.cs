using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;

namespace net_maui_app_v24.Models
{
    public class DeviceBluetooth
    {
        public Guid DeviceId { get; set; }
        public string Name { get; set; }
        public int Rssi { get; set; }
        public bool IsConnectable { get; set; }
        public IReadOnlyList<AdvertisementRecord> AdvertisementRecords { get; set; }
        public string Adverts
        {
            get => String.Join('\n', AdvertisementRecords.Select(advert => $"{advert.Type}: 0x{Convert.ToHexString(advert.Data)}"));
        }
        public DeviceState State { get; set; }

        public IDevice Device { get; set; }

        public string NativeDevice { get; set; }

    }
}
