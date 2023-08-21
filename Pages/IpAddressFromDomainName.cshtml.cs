using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using Whois.NET;

namespace NetworkingApp.Pages
{
    [BindProperties]
    public class IpAddressFromDomainNameModel : PageModel
    {
        public string DomainNameForIpAddressSearch { get; set; }

        public IPHostEntry Host { get; private set; }

        public async Task<PageResult> OnPostAsync()
        {
            Host = await Dns.GetHostEntryAsync(DomainNameForIpAddressSearch);
            return Page();
        }
    }
}
