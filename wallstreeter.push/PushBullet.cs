using PushbulletSharp;
using PushbulletSharp.Models.Requests;
using PushbulletSharp.Models.Responses;
using System.Linq;

namespace wallstreeter.push
{
    public class PushBullet
    {
        private static PushbulletClient _client;

        public PushBullet(IPushToken token)
        {
             _client = new PushbulletClient(token.Get());
        }

        public void Push(string title, string message)
        {
            var device = GetDevice();
            if (device != null)
            {
                var request = new PushNoteRequest
                {
                    DeviceIden = device.Iden,
                    Title = title,
                    Body = message
                };

                var response = _client.PushNote(request);
            }
        }

        public void Push(string title, string message, string url)
        {
            var device = GetDevice();
            if(device != null)
            {
                var request = new PushLinkRequest
                {
                    DeviceIden = device.Iden,
                    Title = title,
                    Body = message,
                    Url = url
                };

                var response = _client.PushLink(request);
            }
        }

        private Device GetDevice()
        {
            var devices = _client.CurrentUsersDevices();
            return devices.Devices.Where(o => o.Manufacturer == "Apple").FirstOrDefault();
        }
    }
}
