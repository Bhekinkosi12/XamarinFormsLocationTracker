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
	public partial class GroupManagerPage : ContentPage
	{
	    private readonly SQLiteConnection _sqLiteConnection;
        public GroupManagerPage ()
		{
			InitializeComponent ();
            //Get connetion
		    _sqLiteConnection = DependencyService.Get<ISQLite>().GetConnection();
            //Create Table GroupClass if not exsits
		    _sqLiteConnection.CreateTable<GroupClass>();
		    refeshListView();
		    hideButton();
        }

	    private void ButtonNew_OnClicked(object sender, EventArgs e)
	    {
	        if (txtGroupName.Text != null & txtGroupName.Text != "")
	        {
                //Check exsits
	            var exsit = checkExist(txtGroupName.Text);

	            if (exsit == false)
	            {
	                _sqLiteConnection.Insert(new GroupClass { Group = txtGroupName.Text });
	                txtGroupName.Text = "";
	                refeshListView();
	                hideButton();
	            }
	            else
	            {
	                DisplayAlert("Hi!", txtGroupName.Text + " already exists in the database, please try another name!", "Ok");
	            }
	        }
	        else
	        {
	            DisplayAlert("Hi!", "Please enter name of group!", "Ok");
	        }
        }
	    public List<GroupClass> listGroup { get; set; } = new List<GroupClass>();
        private void refeshListView()
        {
            listGroup = _sqLiteConnection.Table<GroupClass>().ToList();
            listViewMain.ItemsSource = listGroup;
        }

        private bool checkExist(string groupName)
        {
            bool exist = false;

            //Get list of "groupName" in Database
            List<GroupClass> itemInDBList = _sqLiteConnection.Table<GroupClass>().Where(i => i.Group == groupName).ToList();

            if (itemInDBList.Count > 0)
            {
                return exist = true;
            }
            else
            {
                return exist = false;
            }
        }

        private void ButtonUpdate_OnClicked(object sender, EventArgs e)
	    {
	        if (txtGroupName.Text != null && txtGroupName.Text != "")
	        {
	            var seletedItem = listViewMain.SelectedItem as GroupClass;
	            Update(seletedItem.Id, txtGroupName.Text, seletedItem.Group);
	        }
	        else
	        {
	            DisplayAlert("Hi!", "Please select a group and change name to update!", "Ok");
	        }
        }

	    private void Update(int id, string Value, string oldValue)
	    {
	        if (txtGroupName.Text != null & txtGroupName.Text != "")
	        {
	            var selectedItem = listViewMain.SelectedItem as GroupClass;
	            var exsits = checkExist(selectedItem.Group);
	            if (exsits == true)
	            {
	                _sqLiteConnection.Update(new GroupClass { Id = selectedItem.Id, Group = txtGroupName.Text });
                    //Updat GroupName in KilometManager Table
                    string updateString2 = "UPDATE KilometManager SET [Group] = '" + Value + "' WHERE [Group] = '" + oldValue + "'";
                    //Updat GroupName in  SeriesPoint
                    string updateString1 = "UPDATE SeriesPoint SET [Group] = '" + Value + "' WHERE [Group] = '" + oldValue + "'";
	                _sqLiteConnection.Execute(updateString2);
	                _sqLiteConnection.Execute(updateString1);
	                txtGroupName.Text = "";
	                refeshListView();
	                hideButton();
	            }
	            else
	            {
	                DisplayAlert("Hi!", txtGroupName.Text + " not exists in database!", "Ok");
	            }
	        }
	        else
	        {
	            DisplayAlert("Hi!", "Please select a group and change name to update!", "Ok");
	        }
	    }

        private async void ButtonDelete_OnClicked(object sender, EventArgs e)
	    {
	        if (txtGroupName.Text != null & txtGroupName.Text != "")
	        {
	            var delete = await DisplayAlert("Hi!", "Are you sure want to delete " + txtGroupName.Text, "Ok", "Cancel");
	            if (delete == true)
	            {
	                var seletedItem = listViewMain.SelectedItem as GroupClass;
	                DeleteItem(seletedItem.Id, seletedItem.Group);
	            }
	        }
	        else
	        {
	            await DisplayAlert("Hi!", "Please select a group to delete!", "Ok");
	        }
        }
	    private void DeleteItem(int id, string delGroup)
	    {
	        var exist = checkExist(txtGroupName.Text);
	        if (exist == true)
	        {
	            var selectItem = listViewMain.SelectedItem as GroupClass;
	            string queryString = "Delete From GroupClass Where Id = " + id;
	            _sqLiteConnection.Delete<GroupClass>(id);
	            string queryString2 = "Delete From KilometManager Where [Group] = '" + delGroup + "'";
	            _sqLiteConnection.Query<KilometManager>(queryString2);
	            string queryString3 = "Delete From SeriesPoint Where [Group] = '" + delGroup + "'";
	            _sqLiteConnection.Query<SeriesPoint>(queryString3);
	            //_sqLiteConnection.Delete<GroupClass>(selectItem.Id);
	            txtGroupName.Text = "";
	            refeshListView();
	            hideButton();
	        }
	        else
	        {
	            DisplayAlert("Hi!", txtGroupName.Text + "not exists in database!", "Ok");
	        }
	    }

        private void ListViewMain_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
	    {
	        showButton();
	        var selectedItem = listViewMain.SelectedItem as GroupClass;
	        txtGroupName.Text = selectedItem.Group.ToString();
        }

	    private void showButton()
	    {
	        buttonUpdate.IsVisible = true;
	        buttonDelete.IsVisible = true;
	    }

	    private void hideButton()
	    {
	        buttonUpdate.IsVisible = false;
	        buttonDelete.IsVisible = false;
	    }
    }
}