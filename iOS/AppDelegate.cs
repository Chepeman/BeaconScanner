using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using Xamarin.Forms;

namespace BeaconScanner.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init();

			DependencyService.Register<IBeaconManager, iOS_IBeaconManager>();

			LoadApplication(new App());

			return base.FinishedLaunching(app, options);
		}
	}
}

