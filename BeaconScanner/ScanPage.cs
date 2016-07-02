using System;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Globalization;

namespace BeaconScanner
{
	public class MtsConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value + " mts.";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class ColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var v = (Proximity)value;
			switch (v)
			{
				case Proximity.Far:
					return Color.FromHex("#9E9E9E");
				case Proximity.Immediate:
					return Color.FromHex("#CDDC39");
				case Proximity.Near:
					return Color.FromHex("#FDD835");
				default:
					return Color.White;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class ScanPage : ContentPage
	{
		ListView _beaconsList;
		public ScanPage()
		{
			this.Title = "Beacon Scanner";

			var beaconDataTemplate = new DataTemplate(() =>
			{
				var identifierLabel = new Label { HorizontalTextAlignment = TextAlignment.Start };
				var proximityLabel = new Label { HorizontalTextAlignment = TextAlignment.Start };
				var distanceLabel = new Label { HorizontalTextAlignment = TextAlignment.Start };
				var mtsLabel = new Label { HorizontalTextAlignment = TextAlignment.Start };

				identifierLabel.Text = "Kontakt";
				identifierLabel.FontSize = 20;
				//identifierLabel.SetBinding(Label.TextProperty, "Distance");
				proximityLabel.FontSize = 15;
				proximityLabel.SetBinding(Label.TextProperty, "Distance");

				distanceLabel.FontSize = 12;
				distanceLabel.SetBinding(Label.TextProperty, "Proximity");
				distanceLabel.SetBinding(Label.TextColorProperty, "Proximity", BindingMode.Default, new ColorConverter());

				mtsLabel.FontSize = 12;
				mtsLabel.SetBinding(Label.TextProperty, "Meters", BindingMode.Default, new MtsConverter());

				var beaconInfo = new StackLayout();
				beaconInfo.Padding = new Thickness(10, 0, 0, 0);
				//beaconInfo.Orientation = StackOrientation.Horizontal;
				beaconInfo.Children.Add(identifierLabel);
				beaconInfo.Children.Add(proximityLabel);
				beaconInfo.Children.Add(
					new StackLayout{
					Orientation = StackOrientation.Horizontal,
					Children = { distanceLabel, mtsLabel }
				});

				return new ViewCell { View = beaconInfo };
			});

			_beaconsList = new ListView();
			_beaconsList.RowHeight = 80;
			_beaconsList.VerticalOptions = LayoutOptions.FillAndExpand;
			_beaconsList.ItemTemplate = beaconDataTemplate;


			var container = new StackLayout();
			container.HorizontalOptions = LayoutOptions.FillAndExpand;
			container.VerticalOptions = LayoutOptions.FillAndExpand;
			container.Children.Add(_beaconsList);

			this.Content = container;
		}

		protected async override void OnAppearing()
		{
			base.OnAppearing();

			await Task.Delay(1000);
			//DEMO
			//App.BeaconManager.AddRegion("5739684d584f", "f7826da6-4fa2-4e98-8024-bc5b71e0893e");
			//App.BeaconManager.AddRegion("4f7234416c31", "f7826da6-4fa2-4e98-8024-bc5b71e0893e");

			//Reto
			App.BeaconManager.AddRegion("Kontakt1", "f7826da6-4fa2-4e98-8024-bc5b71e0893e");
			App.BeaconManager.AddRegion("Kontakt2", "f7826da6-4fa2-4e98-8024-bc5b71e0263c");
			App.BeaconManager.AddRegion("Kontakt3", "f7826da6-4fa2-4e98-8024-bc5b71e08911");

			App.BeaconManager.SetRangedAction(OnRanged);
			App.BeaconManager.StartScan();
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			App.BeaconManager.StopScan();
		}

		void OnRanged(IEnumerable<IBeacon> beacons)
		{
			_beaconsList.ItemsSource = beacons;
		}
	}
}

