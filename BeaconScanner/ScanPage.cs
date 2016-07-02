using System;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BeaconScanner
{
	public class ScanPage : ContentPage
	{
		Button _scanButton;
		ListView _beaconsList;
		public ScanPage()
		{
			this.Title = "Beacon Scanner";

			var beaconDataTemplate = new DataTemplate(() =>
			{
				var identifierLabel = new Label { HorizontalTextAlignment = TextAlignment.Start };
				var proximityLabel = new Label { HorizontalTextAlignment = TextAlignment.End };

				identifierLabel.SetBinding(Label.TextProperty, "Distance");
				proximityLabel.SetBinding(Label.TextProperty, "Proximity");

				var beaconInfo = new StackLayout();
				beaconInfo.Orientation = StackOrientation.Horizontal;
				beaconInfo.Children.Add(identifierLabel);
				beaconInfo.Children.Add(proximityLabel);

				return new ViewCell { View = beaconInfo };
			});

			_scanButton = new Button();
			_scanButton.IsEnabled = true;
			_scanButton.Text = "Start Scan";
			_scanButton.Clicked += _scanButton_Clicked;
			_scanButton.HeightRequest = 80;

			_beaconsList = new ListView();
			_beaconsList.VerticalOptions = LayoutOptions.FillAndExpand;
			_beaconsList.ItemTemplate = beaconDataTemplate;


			var container = new StackLayout();
			container.HorizontalOptions = LayoutOptions.FillAndExpand;
			container.VerticalOptions = LayoutOptions.FillAndExpand;
			container.Children.Add(_scanButton);
			container.Children.Add(_beaconsList);


			this.Content = container;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

		}

		void OnRanged(IEnumerable<IBeacon> beacons)
		{
			_beaconsList.ItemsSource = beacons;
		}

		/*private async Task InitializeManager()
		{
			var status = await EstimoteManager.Instance.Initialize(); // optionally pass false to authorize foreground ranging only
			if (status != BeaconInitStatus.Success)
			{
				await this.DisplayAlert("Error!", "Authorize the permissions!", "Ok");
				_scanButton.IsEnabled = false;
				return;
			}
			_scanButton.IsEnabled = true;
			EstimoteManager.Instance.Ranged += Instance_Ranged;
			EstimoteManager.Instance.RegionStatusChanged += Instance_RegionStatusChanged;

		}

		void Instance_Ranged(object sender, System.Collections.Generic.IEnumerable<IBeacon> e)
		{
			_beaconsList.ItemsSource = e; 
		}

		void Instance_RegionStatusChanged(object sender, BeaconRegionStatusChangedEventArgs e)
		{
			var debug = true;
		}*/

		void _scanButton_Clicked(object sender, EventArgs e)
		{
			App.BeaconManager.AddRegion("Estimote1", "B9407F30-F5F8-466E-AFF9-25556B57FE6D", 25304, 1457);
			App.BeaconManager.AddRegion("Estimote2", "B9407F30-F5F8-466E-AFF9-25556B57FE6D", 25216, 1281);
			App.BeaconManager.AddRegion("5739684d584f", "f7826da6-4fa2-4e98-8024-bc5b71e0893e");
			App.BeaconManager.AddRegion("4f7234416c31", "f7826da6-4fa2-4e98-8024-bc5b71e0893e");
			App.BeaconManager.SetRangedAction(OnRanged);
			App.BeaconManager.StartScan();
		}
	}
}

