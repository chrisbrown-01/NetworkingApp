using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.WiFi;

namespace NetworkingApp.Pages
{
    public class WifiScanModel : PageModel
    {
        public IReadOnlyList<WiFiAdapter> WifiAdapters { get; private set; }

        public async Task<PageResult> OnGetAsync()
        {
            // source code: https://stackoverflow.com/questions/496568/how-do-i-get-the-available-wifi-aps-and-their-signal-strength-in-net
            WifiAdapters = await WiFiAdapter.FindAllAdaptersAsync();

            return Page();
        }

        public async Task<PageResult> OnPostAsync()
        {
            // source code: https://stackoverflow.com/questions/496568/how-do-i-get-the-available-wifi-aps-and-their-signal-strength-in-net
            WifiAdapters = await WiFiAdapter.FindAllAdaptersAsync();

            return Page();
        }
    }
}
