using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesSample.Pages;

public class IndexModel : PageModel
{
    public void OnGet() { }

    [JsonPropertyName("title")]
    public string? HCTI_Title { get; set; }


}