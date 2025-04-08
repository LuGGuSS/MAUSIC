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
		// Registering nuget items
		builder.Services
			.AddSingleton(FolderPicker.Default)
			.AddSingleton(FilePicker.Default);

		// Registering Services
		builder.Services
			.AddSingleton<StorageService>()
			.AddSingleton<DatabaseService>()
			.AddSingleton<QueueService>();

		// Registering Managers
		builder.Services
			.AddSingleton<StorageManager>()
			.AddSingleton<DatabaseManager>()
			.AddSingleton<QueueManager>();

		// Registering ViewModels
		builder.Services
			.AddSingleton<PlayerViewModel>();

		return builder.Build();
	}
}