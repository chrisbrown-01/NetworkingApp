using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkingApp.Pages
{
    //TODO: When pushing to production, ensure that Google Maps API key has restrictions set to only allow usage for this web page

    
    [BindProperties]
    public class IpAddressGeoLocationModel : PageModel
    {
        private readonly IConfiguration _config;

        public IpAddressGeoLocationModel(IConfiguration config)
        {
            _config = config;
        }

        public class IpApiGetResponse
        {
            public string query { get; set; }
            public string status { get; set; }
            public string country { get; set; }
            public string countryCode { get; set; }
            public string region { get; set; }
            public string regionName { get; set; }
            public string city { get; set; }
            public string zip { get; set; }
            public float lat { get; set; }
            public float lon { get; set; }
            public string timezone { get; set; }
            public string isp { get; set; }
            public string org { get; set; }
            public string message { get; set; }
            public string GoogleMapsApiQuery { get; set; }
        }

        public string IpAddress { get; set; }
        public IpApiGetResponse ApiResults { get; set; }

        public async Task<PageResult> OnGetAsync()
        {
            await QueryIpApi("");

            return Page();
        }

        public async Task<PageResult> OnPostAsync()
        {
            await QueryIpApi(IpAddress);

            return Page();
        }

        private async Task QueryIpApi(string ipAddress)
        {
            var client = new RestClient("http://ip-api.com/json");
            ApiResults = await client.GetJsonAsync<IpApiGetResponse>(ipAddress);

            if (ApiResults.status.ToLower() == "success")
            {
                string googleUrl = "https://www.google.com/maps/embed/v1/place?key=";
                string googleApiKey = _config["GoogleMapsApiKey"];
                string googleQuery = "&q=" + ApiResults.lat + "," + ApiResults.lon;
                ApiResults.GoogleMapsApiQuery = string.Concat(googleUrl, googleApiKey, googleQuery);
            }
        }
    }
}
