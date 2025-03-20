using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.Extensions.Logging;
#if WINDOWS
using Microsoft.UI.Windowing;
using Windows.Graphics;
using WinRT.Interop;
#endif

namespace Chatroom_MAUI;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if WINDOWS
        builder.ConfigureLifecycleEvents(events =>
        {
            events.AddWindows(windows =>
            {
                windows.OnWindowCreated(window =>
                {
                    var windowHandle = WindowNative.GetWindowHandle(window);
                    var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
                    var appWindow = AppWindow.GetFromWindowId(windowId);
                    if (appWindow is not null)
                    {
                        appWindow.Resize(new SizeInt32(600, 600));

                        Shell.Current.Navigated += (sender, args) =>
                        {
                            if (args.Current.Location.OriginalString.Contains(nameof(MainPage)))
                            {
                                appWindow.Resize(new SizeInt32(1000, 800));
                            }
                            else if (args.Current.Location.OriginalString.Contains(nameof(Login)))
                            {
                                appWindow.Resize(new SizeInt32(600, 600));
                            }
                        };
                    }
                });
            });
        });
#endif

#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
