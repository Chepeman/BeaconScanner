using System;

using Xamarin.Forms;

namespace BeaconScanner
{
	public class App : Application
	{
		public static IBeaconManager BeaconManager;

		public App()
		{
			// The root page of your application
			BeaconManager = DependencyService.Get<IBeaconManager>();
			MainPage = new NavigationPage(new ScanPage()) {
				BarTextColor = Color.White,
				BarBackgroundColor = Color.FromHex("#2196F3")
			};
		}

		protected async override void OnStart()
		{
			// Handle when your app starts
			var result = await BeaconManager.Init();
			var debug = true;
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}

