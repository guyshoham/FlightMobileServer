using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Text;

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
        private readonly BlockingCollection<AsyncCommand> _queue;
        static TcpClient _client;
        static NetworkStream stream;
        readonly string _ip = "127.0.0.1";
        readonly int _port = 5402;

        public FlightGearClient()
        {
            _queue = new BlockingCollection<AsyncCommand>();
            if (_client == null)
            {
                _client = new TcpClient();
            }
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
            if (!_client.Connected)
            {
                Connect(_ip, _port);
            }
            Result res;
            string read;
            double paramValue;

            foreach (AsyncCommand command in _queue.GetConsumingEnumerable())
            {

                // Aileron
                paramValue = command.Command.Aileron;
                Write("set" + command.Command.ParseAileronToString());
                Write("get /controls/flight/aileron\n");
                read = Read();
                res = CheckData(paramValue, read);


                // Elevator
                paramValue = command.Command.Elevator;
                Write("set" + command.Command.ParseElevatorToString());
                Write("get /controls/flight/elevator\n");
                read = Read();
                res = CheckData(paramValue, read);


                // Rudder
                paramValue = command.Command.Rudder;
                Write("set" + command.Command.ParseRudderToString());
                Write("get /controls/flight/rudder\n");
                read = Read();
                res = CheckData(paramValue, read);

                // Throttle
                paramValue = command.Command.Throttle;
                Write("set" + command.Command.ParseThrottleToString());
                Write("get /controls/engines/current-engine/throttle\n");
                read = Read();
                res = CheckData(paramValue, read);


                command.Completion.SetResult(res);
            }
        }
        public Result CheckData(double sent, string recieve)
        {
            if (sent == Convert.ToDouble(recieve))
            {
                return Result.Ok;
            }
            return Result.NotOk;
        }
        public void Connect(string ip, int port)
        {
            //_client = new TcpClient();
            _client.Connect(ip, port);

            // Set timeout.
            //tcp_client.ReceiveTimeout = 10000;
            //tcp_client.SendTimeout = 10000;

            Console.WriteLine("Establishing Connection");
            Console.WriteLine("Server Connected");
            stream = _client.GetStream();

            // first command to change PROMPT
            Write("data\n");
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
            int bytes = stream.Read(inData, 0, inData.Length);
            responseData = Encoding.ASCII.GetString(inData, 0, bytes);
            //Console.WriteLine("Received: {0}", responseData);
            return responseData;
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
            Console.WriteLine("Server is disconnected");
        }
    }
}
