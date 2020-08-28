namespace Maincotech.Net
{
    public class PingResult
    {
        public static PingResult Failed = new PingResult { Succeeded = false };

        public bool Succeeded { get; set; }
        public double? RoundtripTime { get; set; }
    }
}