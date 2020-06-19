using Newtonsoft.Json;

namespace FlightMobileServer.Models
{
    public class Command
    {
        [JsonProperty(PropertyName = "aileron")]
        public double Aileron { get; set; }

        [JsonProperty(PropertyName = "rudder")]
        public double Rudder { get; set; }

        [JsonProperty(PropertyName = "elevator")]
        public double Elevator { get; set; }

        [JsonProperty(PropertyName = "throttle")]
        public double Throttle { get; set; }

        public string ParseElevatorToString() { return " /controls/flight/elevator " + Elevator + "\n"; }
        public string ParseRudderToString() { return " /controls/flight/rudder " + Rudder + "\n"; }
        public string ParseThrottleToString() { return " /controls/engines/current-engine/throttle " + Throttle + "\n"; }
        public string ParseAileronToString() { return " /controls/flight/aileron " + Aileron + "\n"; }
    }
}
