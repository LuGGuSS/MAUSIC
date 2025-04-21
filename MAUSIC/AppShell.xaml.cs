using Microsoft.Maui.Controls;

namespace MAUSIC;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		TabBar.CurrentItem = TabBar.Items.FirstOrDefault(item => item.Items[0].Route == "PlayerPage");
	}
}