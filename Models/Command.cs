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

    }
}
