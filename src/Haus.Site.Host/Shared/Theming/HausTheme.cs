using MudBlazor;

namespace Haus.Site.Host.Shared.Theming;

public class HausTheme : MudTheme
{
    private static string PrimaryColor => Colors.Orange.Default;
    private static string SecondaryColor => Colors.Yellow.Default;
    private static string SuccessColor => Colors.Green.Default;
    private static string ErrorColor => Colors.Red.Default;
    private static string WarningColor => Colors.DeepOrange.Default;
    
    public HausTheme()
    {
        PaletteLight = new PaletteLight
        {
            Primary = PrimaryColor,
            Secondary = SecondaryColor,
            Success = SuccessColor,
            Error = ErrorColor,
            Warning = WarningColor,
        };
        PaletteDark = new PaletteDark
        {
            Primary = PrimaryColor,
            Secondary = SecondaryColor,
            Success = SuccessColor,
            Error = ErrorColor,
            Warning = WarningColor,
        };
    }
}