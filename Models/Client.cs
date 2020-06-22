using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FlightMobileServer.Models
{
    public class Client : IClient
    {
        private static Client instance;
        private readonly BlockingCollection<AsyncCommand> _queue;
        private TcpClient _client;
        private NetworkStream stream;
        private string _ip;
        private int _port;
        private bool connected = false;
        private static readonly object locker = new object();

        //singleton
        public static Client getClient()
        {
            if (instance == null)
            {
                lock (locker)
                {
                    if (instance == null)
                    {
                        instance = new Client();
                    }
                }
            }
            return instance;
        }
        // Constructor
        protected Client()
        {
            _queue = new BlockingCollection<AsyncCommand>();
            _ip = "127.0.0.1";
            _port = 5402;
            run();
        }

        //API initiate
        public Task<Result> Execute(Command command)
        {
            var asyncCommand = new AsyncCommand(command);
            _queue.Add(asyncCommand);
            return asyncCommand.Task;
        }

        public void run()
        {
            Task.Factory.StartNew(ProcessCommands);
        }
        public void ProcessCommands()
        {
            string read;
            double paramValue;
            string[] gets = { "Aileron", "Elevator", "Rudder", "Throttle" };

            Connect(_ip, _port);

            foreach (AsyncCommand command in _queue.GetConsumingEnumerable())
            {
                // Aileron
                paramValue = command.Command.Aileron;
                Write("set" + command.Command.AileronString());
                Write("get /controls/flight/aileron\n");
                read = Read();
                gets[0] = read;

                // Elevator
                paramValue = command.Command.Elevator;
                Write("set" + command.Command.ElevatorString());
                Write("get /controls/flight/elevator\n");
                read = Read();
                gets[1] = read;

                // Rudder
                paramValue = command.Command.Rudder;
                Write("set" + command.Command.RudderString());
                Write("get /controls/flight/rudder\n");
                read = Read();
                gets[2] = read;

                // Throttle
                paramValue = command.Command.Throttle;
                Write("set" + command.Command.ThrottleString());
                Write("get /controls/engines/current-engine/throttle\n");
                read = Read();
                gets[3] = read;

                command.Completion.SetResult(CheckData(command.Command, gets));
            }
        }
        public Result CheckData(Command sent, string[] recieve)
        {
            if ((sent.Aileron != Convert.ToDouble(recieve[0])) || (sent.Elevator != Convert.ToDouble(recieve[1])) ||
                (sent.Rudder != Convert.ToDouble(recieve[2])) || (sent.Throttle != Convert.ToDouble(recieve[3])))
            {
                return Result.NotOk;
            }
            return Result.Ok;
        }
        public void Connect(string ip, int port)
        {
            if (!connected)
            {
                _client = new TcpClient(ip, port);
                //_client.Connect(ip, port);
                Console.WriteLine("Establishing Connection");
                Console.WriteLine("Server Connected");

                stream = _client.GetStream();
                if (stream == null)
                {
                    throw new Exception("Can't get NetworkStream from TcpClient");
                }
                // first command to change PROMPT
                Write("data\n");
                connected = true;
                _client.ReceiveTimeout = 10000;
                _client.SendTimeout = 10000;
            }

            if (connected)
            {
                Console.WriteLine("Server Connected");
            }
        }
        public void Disconnect()
        {
            if (stream != null)
            {
                stream.Close();
            }
            if (_client != null)
            {
                _client.Close();
            }
            connected = false;
            Console.WriteLine("Server is disconnected");
        }
        public void Write(string command)
        {

            // Translate the passed message into ASCII and store it as a Byte array.
            byte[] outData = new byte[1024];
            outData = Encoding.ASCII.GetBytes(command);
            // if (stream != null)
            // {
            stream.Write(outData, 0, outData.Length);
            // }
            Console.WriteLine("Sent: {0}", command);
        }
        public string Read()
        {
            // Buffer to store the response bytes.
            byte[] inData = new byte[256];
            // String to store the response ASCII representation.
            String responseData;
            // Read the first batch of the TcpServer response bytes.
            try
            {
                int bytes = stream.Read(inData, 0, inData.Length);
                responseData = Encoding.ASCII.GetString(inData, 0, bytes);
                //Console.WriteLine("Received: {0}", responseData);
                return responseData;
            }//*******************handle exception
            catch (IOException e)
            {
                throw new IOException("Error in read: " + e.Message);

            }

        }
    }
}
