namespace System.Net
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>http://www.cnblogs.com/SkyD/archive/2008/11/05/1326800.html</remarks>
    public static class IpAddressExtensions
    {
        public static long ToInt64Value(this IPAddress ip)
        {
            var x = 3;

            long o = 0;

            foreach (var f in ip.GetAddressBytes())
            {
                o += (long)f << 8 * x--;
            }

            return o;
        }

        public static IPAddress ToIpAddress(this long l)
        {
            var b = new byte[4];

            for (var i = 0; i < 4; i++)
            {
                b[3 - i] = (byte)(l >> 8 * i & 255);
            }

            return new IPAddress(b);
        }
    }
}