using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using net_maui_app_v24.Interfaces;
using net_maui_app_v24.ViewModels;
using net_maui_app_v24.Services;


#if ANDROID
using net_maui_app_v24.Platforms.Android.Services;
#endif


namespace net_maui_app_v24
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseMauiCommunityToolkitMediaElement()
                .UseMauiCommunityToolkitCamera()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if ANDROID
            builder.Services.AddTransient<IAudioPlayer, AudioPlayer>();
            builder.Services.AddTransient<IRecordAudio, RecordAudio>();
            builder.Services.AddTransient<ITextSpeak, TextSpeak>();
#endif
            builder.Services.AddTransient<MainPageViewModel>();
            builder.Services.AddTransient<AudioService>();
            builder.Services.AddTransient<DownloadService>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<AppShell>();
            builder.Services.AddTransient<ModalViewModel>();
            builder.Services.AddTransient<ModalView>();
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
