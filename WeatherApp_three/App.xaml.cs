using System;
using WeatherApp_three.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace WeatherApp_three
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new WeatherAppPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
