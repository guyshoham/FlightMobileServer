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
        public AsyncCommand(Command input, TaskCompletionSource<Result> completion)
        {
            Command = input;
            Completion = completion;
        }
    }
}
