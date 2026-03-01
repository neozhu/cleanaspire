using MudBlazor;

namespace CleanAspire.ClientApp;

/// <summary>
/// Represents the theme configuration for the application.
/// </summary>
public class Theme
{
    /// <summary>
    /// Gets the application theme.
    /// </summary>
    /// <returns>The application theme.</returns>
    public static MudTheme ApplicationTheme()
    {
        var theme = new MudTheme()
        {
            PaletteLight = new PaletteLight
            {
                Primary = "#0f172a", // Modern blue, professional and trustworthy
                PrimaryContrastText = "#ffffff",
                PrimaryDarken = "#020617",
                PrimaryLighten = "#1e293b",
                Secondary = "#71717a",
                SecondaryContrastText = "#ffffff",
                SecondaryLighten = "#52525b",
                SecondaryDarken = "#a1a1aa",
                Success = "#10b981", // Fresh green, success
                Info = "#0ea5e9", // Info blue, clear
                Tertiary = "#8b5cf6",              // Purple 500
                TertiaryContrastText = "#ffffff",
                TertiaryDarken = "#7c3aed",        // Purple 600
                TertiaryLighten = "#a78bfa",       // Purple 400

                Warning = "#f59e0b",               // Amber 500
                WarningContrastText = "#92400e",   // Amber 800
                WarningDarken = "#d97706",         // Amber 600
                WarningLighten = "#fbbf24",        // Amber 400

                Error = "#dc2626", // Clear red, error
                ErrorContrastText = "#ffffff",
                ErrorDarken = "#b91c1c",
                ErrorLighten = "#ef4444",

                Black = "#020617", // Deep blue-black, more texture
                White = "#ffffff",
                AppbarBackground = "#f8fafc", // Very light blue-gray, modern
                AppbarText = "#0a0a0a",
                Background = "#f8fafc", // Very light blue-gray, modern
                Surface = "#ffffff",
                DrawerBackground = "#ffffff",
                TextPrimary = "#0f172a", // Deep blue-gray, modern professional
                TextSecondary = "#64748b", // Neutral gray, hierarchy

                DrawerIcon = "#71717a",

                TextDisabled = "#94a3b8", // Soft gray
                ActionDefault = "#262626",
                ActionDisabled = "rgba(100, 116, 139, 0.4)",
                ActionDisabledBackground = "rgba(100, 116, 139, 0.1)",
                Divider = "#e2e8f0", // Elegant divider
                DividerLight = "#f1f5f9",
                TableLines = "#e2e8f0", // Table lines, elegant
                LinesDefault = "#e2e8f0",
                LinesInputs = "#cbd5e1",
            },
            PaletteDark = new PaletteDark
            {
                Primary = "#fafafa", // shadcn/ui white primary
                PrimaryContrastText = "#020817",
                PrimaryDarken = "#e4e4e7",
                PrimaryLighten = "#ffffff",
                Secondary = "#78716c", // Neutral gray
                Success = "#22c55e", // Green for success
                Info = "#0ea5e9", // Sky blue for info (shadcn sky-500)
                InfoDarken = "#0284c7", // Darker sky blue (shadcn sky-600)
                InfoLighten = "#38bdf8", // Lighter sky blue (shadcn sky-400)

                Tertiary = "#6366f1",
                TertiaryContrastText = "#fafafa",
                TertiaryDarken = "#4f46e5",
                TertiaryLighten = "#818cf8",

                Warning = "#f59e0b", // Orange for warning
                WarningContrastText = "#fafafa",
                WarningDarken = "#d97706",
                WarningLighten = "#fbbf24",

                Error = "#ef4444", // Red for error
                ErrorContrastText = "#fafafa",
                ErrorDarken = "#dc2626",
                ErrorLighten = "#f87171",

                Black = "#020817",
                White = "#fafafa",
                Background = "#0c0a09", // shadcn/ui dark background
                Surface = "#171717", // Deeper surface color
                AppbarBackground = "#0c0a09",
                AppbarText = "#fafafa",
                DrawerText = "#fafafa",
                DrawerIcon = "#a1a1aa",

                DrawerBackground = "#0c0a09",
                TextPrimary = "#fafafa", // shadcn/ui white text
                TextSecondary = "#a1a1aa", // Neutral gray secondary text
                TextDisabled = "rgba(161, 161, 170, 0.5)",
                ActionDefault = "#e5e5e5",
                ActionDisabled = "rgba(161, 161, 170, 0.3)",
                ActionDisabledBackground = "rgba(161, 161, 170, 0.1)",
                Divider = "rgba(255, 255, 255, 0.1)", // shadcn/ui divider color
                DividerLight = "rgba(161, 161, 170, 0.1)",
                TableLines = "rgba(255, 255, 255, 0.1)",
                LinesDefault = "rgba(255, 255, 255, 0.1)",
                LinesInputs = "rgba(161, 161, 170, 0.2)",
                DarkContrastText = "#020817",
                SecondaryContrastText = "#fafafa",
                SecondaryDarken = "#57534e",
                SecondaryLighten = "#a8a29e",
                OverlayLight = "rgba(250, 250, 250, 0.1)",
                OverlayDark = "rgba(0, 0, 0, 0.8)",


            },
            Shadows = new()
            {
                Elevation = new string[]
        {
            "none",
            "0 2px 4px -1px rgba(6, 24, 44, 0.2)",
            "0px 3px 1px -2px rgba(0,0,0,0.2),0px 2px 2px 0px rgba(0,0,0,0.14),0px 1px 5px 0px rgba(0,0,0,0.12)",
            "0 30px 60px rgba(0,0,0,0.12)",
            "0 6px 12px -2px rgba(50,50,93,0.25),0 3px 7px -3px rgba(0,0,0,0.3)",
            "0 50px 100px -20px rgba(50,50,93,0.25),0 30px 60px -30px rgba(0,0,0,0.3)",
            "0px 3px 5px -1px rgba(0,0,0,0.2),0px 6px 10px 0px rgba(0,0,0,0.14),0px 1px 18px 0px rgba(0,0,0,0.12)",
            "0px 4px 5px -2px rgba(0,0,0,0.2),0px 7px 10px 1px rgba(0,0,0,0.14),0px 2px 16px 1px rgba(0,0,0,0.12)",
            "0px 5px 5px -3px rgba(0,0,0,0.2),0px 8px 10px 1px rgba(0,0,0,0.14),0px 3px 14px 2px rgba(0,0,0,0.12)",
            "0px 5px 6px -3px rgba(0,0,0,0.2),0px 9px 12px 1px rgba(0,0,0,0.14),0px 3px 16px 2px rgba(0,0,0,0.12)",
            "0px 6px 6px -3px rgba(0,0,0,0.2),0px 10px 14px 1px rgba(0,0,0,0.14),0px 4px 18px 3px rgba(0,0,0,0.12)",
            "0px 6px 7px -4px rgba(0,0,0,0.2),0px 11px 15px 1px rgba(0,0,0,0.14),0px 4px 20px 3px rgba(0,0,0,0.12)",
            "0px 7px 8px -4px rgba(0,0,0,0.2),0px 12px 17px 2px rgba(0,0,0,0.14),0px 5px 22px 4px rgba(0,0,0,0.12)",
            "0px 7px 8px -4px rgba(0,0,0,0.2),0px 13px 19px 2px rgba(0,0,0,0.14),0px 5px 24px 4px rgba(0,0,0,0.12)",
            "0px 7px 9px -4px rgba(0,0,0,0.2),0px 14px 21px 2px rgba(0,0,0,0.14),0px 5px 26px 4px rgba(0,0,0,0.12)",
            "0px 8px 9px -5px rgba(0,0,0,0.2),0px 15px 22px 2px rgba(0,0,0,0.14),0px 6px 28px 5px rgba(0,0,0,0.12)",
            "0px 8px 10px -5px rgba(0,0,0,0.2),0px 16px 24px 2px rgba(0,0,0,0.14),0px 6px 30px 5px rgba(0,0,0,0.12)",
            "0px 8px 11px -5px rgba(0,0,0,0.2),0px 17px 26px 2px rgba(0,0,0,0.14),0px 6px 32px 5px rgba(0,0,0,0.12)",
            "0px 9px 11px -5px rgba(0,0,0,0.2),0px 18px 28px 2px rgba(0,0,0,0.14),0px 7px 34px 6px rgba(0,0,0,0.12)",
            "0px 9px 12px -6px rgba(0,0,0,0.2),0px 19px 29px 2px rgba(0,0,0,0.14),0px 7px 36px 6px rgba(0,0,0,0.12)",
            "0px 10px 13px -6px rgba(0,0,0,0.2),0px 20px 31px 3px rgba(0,0,0,0.14),0px 8px 38px 7px rgba(0,0,0,0.12)",
            "0px 10px 13px -6px rgba(0,0,0,0.2),0px 21px 33px 3px rgba(0,0,0,0.14),0px 8px 40px 7px rgba(0,0,0,0.12)",
            "0px 10px 14px -6px rgba(0,0,0,0.2),0px 22px 35px 3px rgba(0,0,0,0.14),0px 8px 42px 7px rgba(0,0,0,0.12)",
            "0 50px 100px -20px rgba(50, 50, 93, 0.25), 0 30px 60px -30px rgba(0, 0, 0, 0.30)",
            "2.8px 2.8px 2.2px rgba(0, 0, 0, 0.02),6.7px 6.7px 5.3px rgba(0, 0, 0, 0.028),12.5px 12.5px 10px rgba(0, 0, 0, 0.035),22.3px 22.3px 17.9px rgba(0, 0, 0, 0.042),41.8px 41.8px 33.4px rgba(0, 0, 0, 0.05),100px 100px 80px rgba(0, 0, 0, 0.07)",
            "0px 0px 20px 0px rgba(0, 0, 0, 0.05)"
        }
            },
            LayoutProperties = new()
            {
                DefaultBorderRadius = "4px",
                AppbarHeight = "68px",
            },
            ZIndex = new ZIndex(),
            Typography = new()
            {

                Default = new DefaultTypography
                {
                    FontSize = ".8125rem",
                    FontWeight = "400",
                    LineHeight = "1.4",
                    LetterSpacing = "normal",
                    FontFamily = ["-apple-system", "BlinkMacSystemFont", "Segoe UI", "Noto Sans", "Helvetica", "Arial", "sans-serif", "Apple Color Emoji", "Segoe UI Emoji"]
                },
                H1 = new H1Typography
                {
                    FontSize = "2.2rem",
                    FontWeight = "700",
                    LineHeight = "2.5",
                    LetterSpacing = "-.01562em"
                },
                H2 = new H2Typography
                {
                    FontSize = "2rem",
                    FontWeight = "600",
                    LineHeight = "2.3",
                    LetterSpacing = "-.00833em"
                },
                H3 = new H3Typography
                {
                    FontSize = "1.75rem",
                    FontWeight = "600",
                    LineHeight = "2.2",
                    LetterSpacing = "0"
                },
                H4 = new H4Typography
                {
                    FontSize = "1.5rem",
                    FontWeight = "500",
                    LineHeight = "2.2",
                    LetterSpacing = ".00735em"
                },
                H5 = new H5Typography
                {
                    FontSize = "1.25rem",
                    FontWeight = "500",
                    LineHeight = "1.8",
                    LetterSpacing = "0"
                },
                H6 = new H6Typography
                {
                    FontSize = "1rem",
                    FontWeight = "500",
                    LineHeight = "1.6",
                    LetterSpacing = ".0075em"
                },
                Button = new ButtonTypography
                {
                    FontSize = ".8125rem",
                    FontWeight = "500",
                    LineHeight = "1.75",
                    LetterSpacing = ".02857em",
                    TextTransform = "uppercase"
                },
                Subtitle1 = new Subtitle1Typography
                {
                    FontSize = ".875rem",
                    FontWeight = "400",
                    LineHeight = "1.5",
                    LetterSpacing = "normal",
                    FontFamily = ["Public Sans", "Roboto", "Arial", "sans-serif"]
                },
                Subtitle2 = new Subtitle2Typography
                {
                    FontSize = ".8125rem",
                    FontWeight = "500",
                    LineHeight = "1.57",
                    LetterSpacing = ".00714em"
                },
                Body1 = new Body1Typography
                {
                    FontSize = "0.8125rem",
                    FontWeight = "400",
                    LineHeight = "1.5",
                    LetterSpacing = ".00938em"
                },
                Body2 = new Body2Typography
                {
                    FontSize = ".75rem",
                    FontWeight = "300",
                    LineHeight = "1.43",
                    LetterSpacing = ".01071em"
                },
                Caption = new CaptionTypography
                {
                    FontSize = "0.625rem",
                    FontWeight = "400",
                    LineHeight = "1.5",
                    LetterSpacing = ".03333em"
                },
                Overline = new OverlineTypography
                {
                    FontSize = "0.625rem",
                    FontWeight = "300",
                    LineHeight = "2",
                    LetterSpacing = ".08333em"
                }
            }
        };
        return theme;
    }

}
