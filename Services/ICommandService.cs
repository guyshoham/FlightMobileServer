using FlightMobileServer.Models;

namespace FlightMobileServer.Services
{
    public interface ICommandService
    {
        void SendCommand(Command command);
    }
}
