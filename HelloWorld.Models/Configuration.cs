using HelloWorld.Models.Util;

namespace HelloWorld.Models
{
    public class Configuration : NotifyObject
    {
        private string _locale = "";

        public string Locale
        {
            get { return _locale; }
            set { _locale = value; OnPropertyChanged(); }
        }

        private Tools _tools = new();

        public Tools Tools
        {
            get { return _tools; }
            set { _tools = value; OnPropertyChanged(); }
        }

        private ServerList _serverList = new();

        public ServerList ServerList
        {
            get { return _serverList; }
            set { _serverList = value; OnPropertyChanged(); }
        }

        private Hostname _hostname = new();

        public Hostname Hostname
        {
            get { return _hostname; }
            set { _hostname = value; OnPropertyChanged(); }
        }

        private VirtualIPAddress _virtualIPAddress = new();

        public VirtualIPAddress VirtualIPAddress
        {
            get { return _virtualIPAddress; }
            set { _virtualIPAddress = value; OnPropertyChanged(); }
        }
    }

    public class Tools : NotifyObject
    {
        private bool _ipBroadcast = false;

        public bool IPBroadcast
        {
            get { return _ipBroadcast; }
            set { _ipBroadcast = value; OnPropertyChanged(); }
        }
    }

    public class Server : NotifyObject, IComparable
    {
        private string _driver = "";

        public string Driver
        {
            get { return _driver; }
            set { _driver = value; OnPropertyChanged(); }
        }

        private string _adapter = "";

        public string Adapter
        {
            get { return _adapter; }
            set { _adapter = value; OnPropertyChanged(); }
        }

        private string _hostname = "";

        public string Hostname
        {
            get { return _hostname; }
            set { _hostname = value; OnPropertyChanged(); }
        }

        private ushort _port = 0;

        public ushort Port
        {
            get { return _port; }
            set { _port = value; OnPropertyChanged(); }
        }

        private string _username = "";

        public string Username
        {
            get { return _username; }
            set { _username = value; OnPropertyChanged(); }
        }

        private string _password = "";

        public string Password
        {
            get { return _password; }
            set { _password = value; OnPropertyChanged(); }
        }

        private string _extraOptions = "";

        public string ExtraOptions
        {
            get { return _extraOptions; }
            set { _extraOptions = value; OnPropertyChanged(); }
        }

        public virtual int CompareTo(object? obj)
        {
            return 1;
        }
    }

    public class NamedServer: Server, INamed, IComparable
    {
        private string _name = "";

        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }

        public NamedServer()
        {

        }

        public NamedServer(Server server): base()
        {
            Driver = server.Driver;
            Adapter = server.Adapter;
            Hostname = server.Hostname;
            Port = server.Port;
            Username = server.Username;
            Password = server.Password;
            ExtraOptions = server.ExtraOptions;
        }

        public Server ToServer()
        {
            return new Server()
            {
                Driver = Driver,
                Adapter = Adapter,
                Hostname = Hostname,
                Port = Port,
                Username = Username,
                Password = Password,
                ExtraOptions = ExtraOptions
            };
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (typeof(NamedServer) != obj?.GetType()) return false;
            return Name == ((NamedServer)obj).Name;
        }

        public bool ValuesEquals(NamedServer? other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name
                && Driver == other.Driver
                && Adapter == other.Adapter
                && Hostname == other.Hostname
                && Port == other.Port
                && Username == other.Username
                && Password == other.Password
                && ExtraOptions == other.ExtraOptions;
        }
    }

    public class ServerList : NotifyObject
    {
        private string _selected = "";

        public string Selected
        {
            get { return _selected; }
            set { _selected = value; OnPropertyChanged(); }
        }

        private SortedDictionary<string, Server> _list = new();

        public SortedDictionary<string, Server> List
        {
            get { return _list; }
            set { _list = value; OnPropertyChanged(); }
        }
    }

    public class Hostname : NotifyObject
    {
        private bool _autoGen = true;

        public bool AutoGen
        {
            get { return _autoGen; }
            set { _autoGen = value; OnPropertyChanged(); }
        }

        private string _name = "";

        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }
    }

    public class VirtualIPAddress : NotifyObject
    {
        private bool _autoAssign = true;

        public bool AutoAssign
        {
            get { return _autoAssign; }
            set { _autoAssign = value; OnPropertyChanged(); }
        }


        private string _address = "";

        public string Address
        {
            get { return _address; }
            set { _address = value; OnPropertyChanged(); }
        }
    }
}

