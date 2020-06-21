using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FlightMobileServer.Models
{
    interface IClient
    {
        void Connect(string ip, int port);
        void Write(string command);
        string Read();
        void Disconnect();
    }

    public class FlightGearClient : IClient
    {
        private static FlightGearClient instance;
        private readonly BlockingCollection<AsyncCommand> _queue;
        private TcpClient _client;
        private NetworkStream stream;
        readonly string _ip = "127.0.0.1";
        readonly int _port = 5402;
        private bool isConnected = false;
        // Lock synchronization object
        private static readonly object syncLock = new object();
        public static FlightGearClient GetFlightGearClient()
        {
            if (instance == null)
            {
                lock (syncLock)
                {
                    if (instance == null)
                    {
                        instance = new FlightGearClient();
                    }
                }
            }
            return instance;
        }
        // Constructor (protected)
        protected FlightGearClient()
        {
            _queue = new BlockingCollection<AsyncCommand>();
            //start a new task 
            Start();
        }
        // Called by the WebApi Controller, it will await on the returned Task<>
        // This is not an async method, since it does not await anything.
        public Task<Result> Execute(Command cmd)
        {
            var asyncCommand = new AsyncCommand(cmd);
            _queue.Add(asyncCommand);
            return asyncCommand.Task;
        }
        public void Start()
        {
            Task.Factory.StartNew(ProcessCommands);
        }
        public void ProcessCommands()
        {
            //connect only once
            Connect(_ip, _port);

            string read;
            double paramValue;
            string[] gets = { "Aileron", "Elevator", "Rudder", "Throttle" };

            foreach (AsyncCommand command in _queue.GetConsumingEnumerable())
            {

                // Aileron
                paramValue = command.Command.Aileron;
                Write("set" + command.Command.ParseAileronToString());
                Write("get /controls/flight/aileron\n");
                read = Read();
                gets[0] = read;


                // Elevator
                paramValue = command.Command.Elevator;
                Write("set" + command.Command.ParseElevatorToString());
                Write("get /controls/flight/elevator\n");
                read = Read();
                gets[1] = read;


                // Rudder
                paramValue = command.Command.Rudder;
                Write("set" + command.Command.ParseRudderToString());
                Write("get /controls/flight/rudder\n");
                read = Read();
                gets[2] = read;

                // Throttle
                paramValue = command.Command.Throttle;
                Write("set" + command.Command.ParseThrottleToString());
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
            if (!isConnected)
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
                isConnected = true;
                _client.ReceiveTimeout = 10000;
                _client.SendTimeout = 10000;
            }

            if (isConnected)
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
            isConnected = false;
            Console.WriteLine("Server is disconnected");
        }
        public void Write(string command)
        {
            //Console.WriteLine(command);
            // Translate the passed message into ASCII and store it as a Byte array.
            byte[] outData = new byte[1024];
            outData = Encoding.ASCII.GetBytes(command);
            // Send the message to the connected TcpServer.
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