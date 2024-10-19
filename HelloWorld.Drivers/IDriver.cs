using HelloWorld.Models;
using HelloWorld.Models.Connection;

namespace HelloWorld.Drivers
{
    public interface IDriver : IDisposable
    {
        /// <summary>
        /// Information about the driver.
        /// </summary>
        DriverInfo Info { get; }
        /// <summary>
        /// Whether the driver is connected to the server.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Connects to the server.
        /// </summary>
        /// <param name="server"></param>
        /// <exception cref="DriverException">
        /// When the server cannot be connected.
        /// </exception>
        void Connect(Server server);

        /// <summary>
        /// Disconnects from the server.
        /// </summary>
        /// <exception cref="DriverException">
        /// When the driver cannot be disconnected.
        /// </exception>
        void Disconnect();

        /// <summary>
        /// Gets information about the connection.
        /// </summary>
        /// <returns>
        /// Connection information or null if the driver is not connected.
        /// </returns>
        ConnectionInfo? GetConnectionInfo();

        void IDisposable.Dispose()
        {
            try
            {
                if (IsConnected)
                {
                    Disconnect();
                }
            }
            finally
            {
                GC.SuppressFinalize(this);
            }
        }
    }

    public class DriverInfo
    {
        /// <summary>
        /// The unique name of the driver.
        /// </summary>
        public readonly string Name;

        public DriverInfo(string name)
        {
            Name = name;
        }
    }

    public class DriverException : Exception
    {
        public DriverException() { }

        public DriverException(string message) : base(message) { }
    }
}
