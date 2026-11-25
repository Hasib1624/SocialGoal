using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SocialGoal.Web.Services
{
    public interface IImageFetcher
    {
        /// <summary>Downloads a user-supplied URL; null if it is not allowed, too big or unreachable.</summary>
        Task<byte[]> FetchAsync(string url);
    }

    /// <summary>
    /// SSRF guard: http/https only, public addresses only (checked on every connection,
    /// so redirects and DNS rebinding are covered), size and time limits.
    /// </summary>
    public class ImageFetcher : IImageFetcher
    {
        public const int MaxBytes = 4 * 1024 * 1024;

        private readonly HttpClient client = new HttpClient(new SocketsHttpHandler
        {
            UseProxy = false,
            ConnectCallback = async (context, token) =>
            {
                var addresses = await Dns.GetHostAddressesAsync(context.DnsEndPoint.Host, token);
                if (addresses.Length == 0 || !addresses.All(IsPublic))
                    throw new HttpRequestException("Address not allowed");
                var socket = new Socket(SocketType.Stream, ProtocolType.Tcp) { NoDelay = true };
                try
                {
                    await socket.ConnectAsync(addresses, context.DnsEndPoint.Port, token);
                    return new NetworkStream(socket, true);
                }
                catch
                {
                    socket.Dispose();
                    throw;
                }
            }
        })
        {
            Timeout = TimeSpan.FromSeconds(10),
            MaxResponseContentBufferSize = MaxBytes
        };

        public async Task<byte[]> FetchAsync(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                return null;
            try
            {
                return await client.GetByteArrayAsync(uri);
            }
            catch (Exception e) when (e is HttpRequestException || e is TaskCanceledException || e is SocketException || e is IOException)
            {
                return null;
            }
        }

        public static bool IsPublic(IPAddress address)
        {
            if (address.IsIPv4MappedToIPv6)
                address = address.MapToIPv4();
            var b = address.GetAddressBytes();
            if (address.AddressFamily == AddressFamily.InterNetworkV6)
                // only global unicast (2000::/3), minus 6to4 (2002::/16) which can embed private IPv4
                return (b[0] & 0xE0) == 0x20 && !(b[0] == 0x20 && b[1] == 0x02);
            return !(b[0] == 0 || b[0] == 10 || b[0] == 127 || b[0] >= 224
                || (b[0] == 100 && b[1] >= 64 && b[1] <= 127)
                || (b[0] == 169 && b[1] == 254)
                || (b[0] == 172 && b[1] >= 16 && b[1] <= 31)
                || (b[0] == 192 && b[1] == 168)
                || (b[0] == 192 && b[1] == 0)
                || (b[0] == 198 && (b[1] == 18 || b[1] == 19)));
        }
    }
}
