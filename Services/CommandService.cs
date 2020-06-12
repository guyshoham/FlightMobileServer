using FlightMobileServer.Models;
using System;

namespace FlightMobileServer.Services
{
    public class CommandService : ICommandService
    {
        private bool isFIrstTime = true; // send data\n command on first connection with simulator

        public string GetScreenshot()
        {
            return "screenshot";
        }

        public void SendCommand(Command command)
        {
            return;
        }
    }
}
