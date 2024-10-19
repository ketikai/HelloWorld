namespace HelloWorld.Models.Connection
{
    public class ConnectionInfo
    {
        private readonly Dictionary<string, string> _devices = new();

        public Dictionary<string, string> Devices
        {
            get => new(_devices);
        }

    }
}
