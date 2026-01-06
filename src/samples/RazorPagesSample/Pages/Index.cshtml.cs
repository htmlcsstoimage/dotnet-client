using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesSample.Pages;

public class IndexModel : PageModel
{
    public void OnGet() { }
    public OGModel OGModel { get; } = new OGModel("Test Title", "Test Description");

}
