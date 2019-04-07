using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LocationTracker
{
    public partial class MainPage : MasterDetailPage
    {
        public MainPage()
        {
            InitializeComponent();
            Detail = new NavigationPage(new MyPage.HomePage());
        }

        private void HomeButton_OnClicked(object sender, EventArgs e)
        {
            Detail = new NavigationPage(new MyPage.HomePage());
            IsPresented = false;
        }

        private void GroupManagerButton_OnClicked(object sender, EventArgs e)
        {
            Detail = new NavigationPage(new MyPage.GroupManagerPage());
            IsPresented = false;
        }

        private void InputManualtButton_OnClicked(object sender, EventArgs e)
        {
            
        }

        private void ReportButton_OnClicked(object sender, EventArgs e)
        {
            Detail = new NavigationPage(new MyPage.ReportPage());
            IsPresented = false;
        }
    }
}
