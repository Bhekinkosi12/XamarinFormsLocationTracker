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
using Xamarin.Forms.Xaml;

namespace LocationTracker.MyPage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TrackingPage : ContentPage
    {
        private readonly SQLiteConnection _sqLiteConnection;

        bool tracking = false;

        double firstlong = 0;
        double firstla = 0;

        double distance = 0;

        int lastId = 0;

        List<SeriesPoint> listSeriesPoint = new List<SeriesPoint>();

        public TrackingPage()
        {
            InitializeComponent();
            _sqLiteConnection = DependencyService.Get<ISQLite>().GetConnection();
            _sqLiteConnection.CreateTable<KilometManager>();
            _sqLiteConnection.CreateTable<SeriesPoint>();
            buttonSave.IsVisible = false;
            getRadiusPicker();
        }

        private void getRadiusPicker()
        {
            List<string> listRadius = new List<string>();
            listRadius.Add("100");
            listRadius.Add("90");
            listRadius.Add("80");
            listRadius.Add("70");
            listRadius.Add("60");
            listRadius.Add("50");
            listRadius.Add("40");
            listRadius.Add("30");
            listRadius.Add("20");
            foreach (var item in listRadius)
            {
                pickerRadius.Items.Add(item);
            }
            pickerRadius.SelectedItem = Helpers.Settings.RadiusSettings;
        }

        private async void ButtonStart_OnClicked(object sender, EventArgs e)
        {
            if (buttonStart.Text == "Start")
            {
                var start = await DisplayAlert("Hi!", "Start tracking?", "Yes", "No");
                if (start == true)
                {
                    distance = 0;
                    getLastID();
                    txtDistance.Text = "";
                    buttonStart.Text = "Stop";
                    buttonSave.IsVisible = false;
                    txtStatus.Text = "Getting your location";
                    try
                    {
                        await getCurrentLocationAsync();
                        CrossGeolocator.Current.PositionChanged -= Current_PositionChanged1;
                        CrossGeolocator.Current.PositionChanged += Current_PositionChanged1;
                        await CrossGeolocator.Current.StartListeningAsync(TimeSpan.FromSeconds(1), 1, true);

                    }
                    catch (Exception ex)
                    {
                        var str = ex.Message.ToString();
                    }
                }
            }
            else
            {
                var stop = await DisplayAlert("Hi!", "Stop tracking?", "Yes", "No");
                if (stop == true)
                {
                    buttonStart.Text = "Start";
                    await CrossGeolocator.Current.StopListeningAsync();
                    CrossGeolocator.Current.PositionChanged -= Current_PositionChanged1;
                    buttonSave.IsVisible = true;
                    buttonSave.IsEnabled = true;
                    txtStatus.Text = "";
                    progressRing.IsRunning = false;
                    progressRing.IsVisible = false;
                }
            }
        }
        private void getLastID()
        {
            List<KilometManager> listKM = _sqLiteConnection.Table<KilometManager>().ToList();
            if (listKM.Count != 0)
            {
                var final = listKM.LastOrDefault();
                lastId = final.Id + 1;
            }
            else
            {
                lastId = 1;
            }

        }
        private void Current_PositionChanged1(object sender, global::Plugin.Geolocator.Abstractions.PositionEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var position = e.Position;
                var accuracy = position.Accuracy;
                txtRadius.Text = "Current GPS Radius: " + Math.Round(position.Accuracy, 2) + " meters";
                if (txtDistance.Text != null && txtDistance.Text != "")
                {
                    txtStatus.Text = "Now you can minimize this app";
                }
                if (position.Accuracy <= double.Parse(Helpers.Settings.RadiusSettings))
                {
                    progressRing.IsRunning = false;
                    progressRing.IsVisible = false;
                    double newlong = position.Longitude;
                    double newla = position.Latitude;

                    if (Double.IsNaN(newlong) == false || Double.IsNaN(newla) == false)
                    {
                        distance += DistanceTo(firstla, firstlong, newla, newlong, 'K');
                        firstla = newla;
                        firstlong = newlong;
                        distance = Math.Round(distance, 2);
                        txtDistance.Text = distance.ToString();
                        listSeriesPoint.Add(new SeriesPoint { Group = txtGroup.Text, Longtitude = firstlong, Latitude = firstla, Ngay = DateTime.Now.ToString("yyyy-MM-dd"), Id = lastId });

                    }
                }
            });
        }
        static public double DistanceTo(double lat1, double lon1, double lat2, double lon2, char unit = 'K')
        {
            double rlat1 = Math.PI * lat1 / 180;
            double rlat2 = Math.PI * lat2 / 180;
            double theta = lon1 - lon2;
            double rtheta = Math.PI * theta / 180;
            double dist = Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) * Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;
            if (dist.ToString() != "NaN")
            {
                switch (unit)
                {
                    case 'K': //Kilometers -> default
                        return dist * 1.609344;
                    case 'N': //Nautical Miles 
                        return dist * 0.8684;
                    case 'M': //Miles
                        return dist;
                }
            }
            else
            {
                Debug.WriteLine("Calculate distance: " + dist);
            }
            return 0;
        }
        private async void ButtonSave_OnClicked(object sender, EventArgs e)
        {
            if (distance != 0)
            {
                //DisplayAlert("Hi!", "You cliked on Save button", "Yeah");
                txtStatus.Text = "Please wait until saving complete!";
                buttonSave.IsVisible = false;
                buttonStart.IsVisible = false;
                progressRing.IsRunning = true;
                progressRing.IsVisible = true;
                await SaveDatabaseToSQLite();
                progressRing.IsRunning = false;
                progressRing.IsVisible = false;
                txtDistance.Text = "";
                buttonStart.IsVisible = true;
                distance = 0;
                txtStatus.Text = "Save completed!";

            }
            else
            {
                buttonSave.IsVisible = false;
                txtDistance.Text = "";
            }
        }
        private Task SaveDatabaseToSQLite()
        {
            var distance = double.Parse(txtDistance.Text);
            var group = txtGroup.Text;
            var today = DateTime.Now.ToString("yyyy-MM-dd");
            return Task.Factory.StartNew(() =>
            {
                _sqLiteConnection.Insert(new KilometManager { Ngay = today, SoKmDiDuoc = distance, Group = group });
                foreach (var item in listSeriesPoint)
                {
                    _sqLiteConnection.Insert(new SeriesPoint { Id = item.Id, Longtitude = item.Longtitude, Latitude = item.Latitude, Group = item.Group, Ngay = item.Ngay });
                }
            });
        }
        private async Task getCurrentLocationAsync()
        {
            try
            {
                progressRing.IsRunning = true;
                progressRing.IsVisible = true;
                buttonStart.IsVisible = false;
                var position = await CrossGeolocator.Current.GetPositionAsync(TimeSpan.FromSeconds(20));
                buttonStart.IsVisible = true;
                //progressRing.IsRunning = false;
                //progressRing.IsVisible = false;
                firstlong = position.Longitude;
                firstla = position.Latitude;
            }
            catch
            {
                await DisplayAlert("Hi!", "Please check location permission if turned on try restart your device!", "Ok");
                return;
            }
        }


        private void PickerRadius_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            Helpers.Settings.RadiusSettings = pickerRadius.SelectedItem.ToString();
        }
    }
}