using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Whois.NET;

namespace NetworkingApp.Pages
{
    [BindProperties]
    public class PingModel : PageModel
    {
        public string PingReplyStatus { get; private set; } = "";
        public string PingReplyAddress { get; private set; } = "";
        public string PingReplyRoundTripTime { get; private set; } = "";
        public string HostNameOrAddress { get; set; }
        public bool PostRequestReceived { get; private set; }

        public async Task<PageResult> OnPostAsync()
        {
            PostRequestReceived = true;

            await GetPingReplyAsync(HostNameOrAddress);

            return Page();
        }

        public async Task GetPingReplyAsync(string hostNameOrAddress)
        {
            using var ping = new Ping();

            try
            {
                var reply = await ping.SendPingAsync(hostNameOrAddress, 100);

                PingReplyStatus = reply.Status.ToString();
                PingReplyAddress = reply.Address.ToString();
                PingReplyRoundTripTime = reply.RoundtripTime.ToString();
            }
            catch(Exception ex)
            {
                PingReplyStatus = $"Failed. Reason for failure: {ex.Message}";
            }
        }
    }
}
