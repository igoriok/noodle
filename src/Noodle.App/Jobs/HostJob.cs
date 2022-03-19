using System.Net;
using Noodle.App.Logic;

namespace Noodle.App.Jobs;

public abstract class HostJob
{
    private IPAddress[] _addresses;

    protected abstract string Host { get; }

    protected abstract string[] IpAddresses { get; }

    protected virtual async Task<IPAddress> GetAddressAsync(CancellationToken cancellationToken)
    {
        if (_addresses == null)
        {
            if (IpAddresses?.Length > 0)
            {
                _addresses = IpAddresses
                    .Select(IPAddress.Parse)
                    .ToArray();
            }
            else if (IPAddress.TryParse(Host, out var address))
            {
                _addresses = new[] { address };
            }
            else
            {
                _addresses = await Dns.GetHostAddressesAsync(Host!, cancellationToken);
            }
        }

        return _addresses.Random();
    }
}