using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace document.lib.web.v2.Components.Shared;

public partial class CultureSelector : ComponentBase
{
    protected override void OnInitialized()
    {
        Culture = CultureInfo.CurrentCulture;
    }

    private CultureInfo Culture
    {
        get
        {
            return CultureInfo.CurrentCulture;
        }
        set
        {
            if (CultureInfo.CurrentCulture != value)
            {
                var uri = new Uri(Navigation.Uri)
                    .GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);
                var cultureEscaped = Uri.EscapeDataString(value.Name);
                var uriEscaped = Uri.EscapeDataString(uri);

                var fullUri = $"api/culture?culture={cultureEscaped}&redirectUri={uriEscaped}";
                Navigation.NavigateTo(fullUri, forceLoad: true);
            }
        }
    }
}