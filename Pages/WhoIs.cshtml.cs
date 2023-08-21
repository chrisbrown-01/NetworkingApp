using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Whois.NET;

namespace NetworkingApp.Pages
{
    [BindProperties]
    public class WhoIsModel : PageModel
    {
        public string DomainNameForQuery { get; set; }

        public WhoisResponse QueryResult { get; private set; }

        public PageResult OnGet()
        {
            return Page();
        }

        public async Task<PageResult> OnPostAsync()
        {
            QueryResult = await WhoisClient.QueryAsync(DomainNameForQuery);
            return Page();
        }
    }
}
