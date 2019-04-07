using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocationTracker.Model;
using LocationTracker.SQLite;
using Plugin.Geolocator;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace LocationTracker.MyPage
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MapPageTest : ContentPage
	{
	    string fromDate, toDate, groupName, paramater;
	    private readonly SQLiteConnection _sqLiteConnection;
	    List<KilometManager> listKM;
	    List<SeriesPoint> listPoint;
	    ListView listView = new ListView();
	    ActivityIndicator progress = new ActivityIndicator { IsVisible = false };
	    SeriesPoint centerPoint = new SeriesPoint();
	    CustomMap customMap = new CustomMap
	    {
	        MapType = MapType.Street,
	        WidthRequest = App.ScreenWidth,
	        HeightRequest = App.ScreenHeight,
	        IsShowingUser = true
	    };
        public MapPageTest (string paramaterPass)
		{
            //InitializeComponent();           
            _sqLiteConnection = DependencyService.Get<ISQLite>().GetConnection();
            _sqLiteConnection.CreateTable<KilometManager>();

            getCurrentLocationAsync();
            //Get paramater
            paramater = paramaterPass;
            fromDate = paramater.Substring(0, 10);
            toDate = paramater.Substring(11, 10);
            groupName = paramater.Substring(22);

            var kilometDataTemplate = new DataTemplate(() =>
            {
                var stackData = new StackLayout { HeightRequest = AbsoluteLayout.AutoSize, Padding = 5f };
                stackData.Orientation = StackOrientation.Horizontal;
                var dateLabel = new Label { HorizontalOptions = LayoutOptions.Start };
                var groupLabel = new Label { HorizontalOptions = LayoutOptions.CenterAndExpand };
                var soKMLabel = new Label { HorizontalOptions = LayoutOptions.End };
                var KMLabel = new Label { HorizontalOptions = LayoutOptions.End };

                dateLabel.SetBinding(Label.TextProperty, "Ngay");
                groupLabel.SetBinding(Label.TextProperty, "Group");
                soKMLabel.SetBinding(Label.TextProperty, "SoKmDiDuoc");
                KMLabel.Text = " Km";

                stackData.Children.Add(dateLabel);
                stackData.Children.Add(groupLabel);
                stackData.Children.Add(soKMLabel);
                stackData.Children.Add(KMLabel);

                return new ViewCell { View = stackData };
            });

            //Get list
            string selectStr = "Select * From KilometManager Where Ngay >= '" + fromDate + "' And Ngay <= '" + toDate + "' And [Group] = '" + groupName + "' And SoKmDiDuoc > 0";
            listKM = _sqLiteConnection.Query<KilometManager>(selectStr).ToList();

            listView.ItemsSource = listKM;
            listView.ItemTemplate = kilometDataTemplate;
            listView.ItemSelected += ListView_ItemSelected;

            createCustomMap();
        }

	    private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
	    {
	        try
	        {
	            var seletedItem = listView.SelectedItem as KilometManager;
	            //string selectStr = "Select * From SeriesPoint Where [Group] = '"+ seletedItem.Group + "'";
	            List<SeriesPoint> seriList = _sqLiteConnection.Table<SeriesPoint>().ToList();
	            //listPoint = _sqLiteConnection.Query<SeriesPoint>(selectStr).ToList();
	            listPoint = _sqLiteConnection.Table<SeriesPoint>().Where(id => id.Id == seletedItem.Id).ToList();

	            //Get Start/End point of route
	            var startPoint = listPoint.FirstOrDefault();
	            var endPoint = listPoint.LastOrDefault();
	            //Create Pin 
	            var startPin = new CustomPin
	            {
	                Pin = new Pin
	                {
	                    Type = PinType.Place,
	                    Position = new Position(startPoint.Latitude, startPoint.Longtitude),
	                    Label = "You start from here",
	                    Address = ""
	                },
	                Id = "Start point",
	                Url = ""
	            };

	            //Clear map
	            customMap.RouteCoordinates.Clear();
	            foreach (var item in listPoint)
	            {
	                customMap.RouteCoordinates.Add(new Position(item.Latitude, item.Longtitude));
	                centerPoint = item;
	            }

	            //Add Pin to map
	            customMap.CustomPins = new List<CustomPin> { startPin };
	            createCustomMap();
	        }
	        catch (Exception exception)
	        {
	            Debug.WriteLine("Load route error: " + exception.Message);
	        }

	    }

        private void createCustomMap()
        {
            //This UI of this Map Page
            var gridLayout = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(1,GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(100,GridUnitType.Absolute)},
                    new RowDefinition { Height = new GridLength(50,GridUnitType.Absolute)}
                }
            };

            customMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(centerPoint.Latitude, centerPoint.Longtitude), Distance.FromKilometers(0.3)));


            gridLayout.Children.Add(listView, 0, 1);
            gridLayout.Children.Add(progress);
           
            gridLayout.Children.Add(customMap);
            Content = gridLayout;
        }

        private async void getCurrentLocationAsync()
        {
            try
            {

                progress.IsVisible = true;
                progress.IsRunning = true;
                var position = await CrossGeolocator.Current.GetPositionAsync(TimeSpan.FromSeconds(10));
                progress.IsVisible = false;
                progress.IsRunning = false;
                customMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(position.Latitude, position.Longitude), Distance.FromKilometers(1)));
            }
            catch
            {
                await DisplayAlert("Hi!", "Please check location permission if turned on try restart your device!", "Ok");
            }
        }
    }
}