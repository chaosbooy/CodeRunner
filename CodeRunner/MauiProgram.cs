using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;

namespace CodeRunner
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            //builder.ConfigureMauiHandlers(handlers =>
            //{
            //    handlers.AddHandler<Image, Microsoft.Maui.Handlers.ImageHandler>();
            //    ImageHandler.Mapper.AppendToMapping("PixelArt", (handler, view) =>
            //    {
            //        handler.PlatformView.SetScaleType(Android.Widget.ImageView.ScaleType.Center);
            //        handler.PlatformView.SetLayerType(Android.Views.LayerType.Software, null);

            //        var paint = new Paint();
            //        paint.FilterBitmap = false;
            //    });
            //});

            return builder.Build();
        }
    }
}
