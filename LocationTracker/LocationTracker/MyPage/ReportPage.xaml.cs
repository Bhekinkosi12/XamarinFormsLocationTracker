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
	public partial class ReportPage : ContentPage
	{
	    private readonly SQLiteConnection _sqLiteConnection;
	    List<GroupClass> listOfGroup = new List<GroupClass>();
        public ReportPage ()
		{
			InitializeComponent ();
		    _sqLiteConnection = DependencyService.Get<ISQLite>().GetConnection();
		    _sqLiteConnection.CreateTable<GroupClass>();
		    buttonView.IsVisible = false;
		    loadListOfGroup();
        }

        private void loadListOfGroup()
        {
            listOfGroup = _sqLiteConnection.Table<GroupClass>().ToList();
            foreach (var item in listOfGroup)
            {
                pickerGroup.Items.Add(item.Group);
            }
        }

	    private void PickerGroup_OnSelectedIndexChanged(object sender, EventArgs e)
	    {
	        buttonView.IsVisible = true;
	        buttonViewOnMap.IsVisible = false;
        }

	    private async void ButtonViewOnMap_OnClicked(object sender, EventArgs e)
	    {
	        var Tu = TuNgay.Date.ToString("yyyy-MM-dd"); //From Date
	        var Den = DenNgay.Date.ToString("yyyy-MM-dd"); //To Date

	        var group = pickerGroup.SelectedItem.ToString();
	        var paramater = Tu + " " + Den + " " + group;

	        await Navigation.PushAsync(new MapPageTest(paramater));
        }
	    List<KilometManager> list1 = new List<KilometManager>();
	    List<KilometManager> list2 = new List<KilometManager>();
        private void ButtonView_OnClicked(object sender, EventArgs e)
	    {
	        double tong1Group = 0;
	        double tongAllGroup = 0;
	        list1 = null;
	        list2 = null;
	        string queryString = "Select * From KilometManager Where Ngay >= '" + TuNgay.Date.ToString("yyyy-MM-dd") + "' And Ngay <= '" + DenNgay.Date.ToString("yyyy-MM-dd") + "' And [Group] = '" + pickerGroup.SelectedItem.ToString() + "'";

	        list1 = _sqLiteConnection.Query<KilometManager>(queryString).ToList();
	        foreach (var item in list1)
	        {
	            tong1Group += item.SoKmDiDuoc;
	        }
	        string queryString2 = "Select * From KilometManager Where Ngay >= '" + TuNgay.Date.ToString("yyyy-MM-dd") + "' And Ngay <= '" + DenNgay.Date.ToString("yyyy-MM-dd") + "'";
	        list2 = _sqLiteConnection.Query<KilometManager>(queryString2).ToList();
	        foreach (var item in list2)
	        {
	            tongAllGroup += item.SoKmDiDuoc;
	        }
	        txtKetQua.Text = tong1Group.ToString() + " Km";
	        txtTotalAllgroup.Text = "All group: " + tongAllGroup + " Km";
	        if (tongAllGroup != 0)
	        {
	            buttonViewOnMap.IsVisible = true;
	        }

        }
    }
}