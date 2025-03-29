using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;
using MAUSIC.Managers;
using MAUSIC.Services;
using MAUSIC.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Storage;

namespace MAUSIC;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.UseMauiCommunityToolkitMediaElement()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif
		// Registering toolkit items
		builder.Services
			.AddSingleton<IFolderPicker>(FolderPicker.Default)
			.AddSingleton<IFilePicker>(FilePicker.Default);

		// Registering Services
		builder.Services
			.AddSingleton<StorageService>();

		// Registering Managers
		builder.Services
			.AddSingleton<StorageManager>();

		// Registering ViewModels
		builder.Services
			.AddSingleton<PlayerViewModel>();

		return builder.Build();
	}
}