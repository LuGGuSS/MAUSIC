using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;
using MAUSIC.Managers;
using MAUSIC.PageModels;
using MAUSIC.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Storage;
using PlayerPageModel = MAUSIC.PageModels.PlayerPageModel;

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
			.AddSingleton<DatabaseService>()
			.AddSingleton<HistoryService>()
			.AddSingleton<PlaylistService>()
			.AddSingleton<QueueService>()
			.AddSingleton<RecommendationService>()
			.AddSingleton<SongsService>()
			.AddSingleton<StorageService>();

		// Registering Managers
		builder.Services
			.AddSingleton<DatabaseManager>()
			.AddSingleton<HistoryManager>()
			.AddSingleton<PlaylistManager>()
			.AddSingleton<QueueManager>()
			.AddSingleton<RecommendationManager>()
			.AddSingleton<SongsManager>()
			.AddSingleton<StorageManager>();

		// Registering PageModels
		builder.Services
			.AddSingleton<FoldersPageModel>()
			.AddSingleton<PlayerPageModel>()
			.AddSingleton<PlaylistsPageModel>();

		return builder.Build();
	}
}