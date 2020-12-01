using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Maincotech.Net
{
    public static class NetworkUtils
    {
        public static PingResult Ping(string nameOrAddress)
        {
            var reuslt = new PingResult
            {
                RoundtripTime = null,
                Succeeded = false
            };

            Ping pinger = null;

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(nameOrAddress);
                reuslt.Succeeded = reply.Status == IPStatus.Success;
                return reuslt;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            return reuslt;
        }

        public static PingResult TcpPing(string nameOrAddress, int port)
        {
            try
            {
                var client = new TcpClient();
                var stopwatch = new Stopwatch();
                // Measure the Connect call only
                stopwatch.Start();
                client.Connect(nameOrAddress, port);
                stopwatch.Stop();

                return new PingResult
                {
                    Succeeded = true,
                    RoundtripTime = stopwatch.Elapsed.TotalMilliseconds
                };
            }
            catch
            {
                return PingResult.Failed;
            }
        }
    }
}