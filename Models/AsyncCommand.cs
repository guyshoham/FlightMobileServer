using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightMobileServer.Models
{
    public enum Result { Ok, NotOk }
    public class AsyncCommand
    {
        public Command Command { get; private set; }
        public Task<Result> Task { get => Completion.Task; }
        public TaskCompletionSource<Result> Completion { get; private set; }
        public AsyncCommand(Command input)
        {
            Command = input;
            // Watch out! Run Continuations Async is important!
            Completion = new TaskCompletionSource<Result>(
            TaskCreationOptions.RunContinuationsAsynchronously);
        }
    }

}
