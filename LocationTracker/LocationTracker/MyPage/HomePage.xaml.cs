using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocationTracker.Model;
using LocationTracker.SQLite;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LocationTracker.MyPage
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HomePage : ContentPage
	{
	    private readonly SQLiteConnection _sqLiteConnection;
	    public List<GroupClass> listGroup { get; set; } = new List<GroupClass>();
        public HomePage ()
		{
			InitializeComponent ();
		    _sqLiteConnection = DependencyService.Get<ISQLite>().GetConnection();
		    _sqLiteConnection.CreateTable<GroupClass>();
		    refeshListView();
        }

	    private void refeshListView()
	    {
	        Device.BeginInvokeOnMainThread(() =>
	        {
	            listGroup = _sqLiteConnection.Table<GroupClass>().ToList();
	            listViewMain.ItemsSource = listGroup;
	            if (listGroup.Count != 0)
	            {
	                buttonGoTracking.Text = "Go Tracking";
	            }
	            if (listGroup.Count == 0)
	            {
	                buttonGoTracking.Text = "Add a Group";
	            }
	        });
	    }
        private async void ButtonGoTracking_OnClicked(object sender, EventArgs e)
	    {
	        if (listGroup.Count != 0)
	        {
	            var selectedGroup = listViewMain.SelectedItem as GroupClass;
	            if (selectedGroup != null)
	            {
	                var trackingPage = new TrackingPage();

	                trackingPage.BindingContext = selectedGroup;
	                await Navigation.PushAsync(trackingPage);
	            }
	            else
	            {
	                await DisplayAlert("Hi!", "Please select a group to start tracking!", "Ok");
	            }
	        }
	        else
	        {
	            await Navigation.PushAsync(new GroupManagerPage());
	        }
        }
	}
}