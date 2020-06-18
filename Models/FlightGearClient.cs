using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Text;

namespace FlightMobileServer.Models
{
    public class FlightGearClient
    {
        private readonly BlockingCollection<AsyncCommand> _queue;
        private readonly TcpClient _client;
        public FlightGearClient()
        {
            _queue = new BlockingCollection<AsyncCommand>();
            _client = new TcpClient();
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
            _client.Connect("127.0.0.1", 5402);
            NetworkStream stream = _client.GetStream();
            foreach (AsyncCommand command in _queue.GetConsumingEnumerable())
            {   //this is the way to send requests to the stream
                byte[] sendBuffer = Encoding.ASCII.GetBytes("get /instrumentation/airspeed-indicator/indicated-speed-kt\n");
                byte[] recvBuffer = new byte[1024];
                stream.Write(sendBuffer, 0, sendBuffer.Length);
                int nRead = stream.Read(recvBuffer, 0, 1024);
                //after command has complited, we get the status in result
                Result res = command.Task.Result;
                             // TaskCompletionSource allows an external thread to set
                             // the result (or the exceptino) on the associated task object
                command.Completion.SetResult(res);
            }
        }
    }
}
