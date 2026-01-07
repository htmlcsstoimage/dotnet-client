using HtmlCssToImage.Models;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

[OptionsValidator]
internal sealed partial class HtmlCssToImageOptionsValidator : IValidateOptions<HtmlCssToImageOptions>
{
}