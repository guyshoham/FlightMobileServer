using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

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
            {
                //byte[] sendBuffer = // command to buffer…
                byte[] recvBuffer = new byte[1024];
                //stream.Write(sendBuffer, 0, sendBuffer.Length);
                int nRead = stream.Read(recvBuffer, 0, 1024);
                //Result res = // recvBuffer to Result
                             // TaskCompletionSource allows an external thread to set
                             // the result (or the exceptino) on the associated task object
                //command.Completion.SetResult(res);
            }
        }
    }
}
