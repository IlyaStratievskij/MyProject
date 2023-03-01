﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using WeatherApp_three.Helper;
using WeatherApp_three.Models;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WeatherApp_three.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WeatherAppPage : ContentPage
    {
        Button getWeatherButton;

        public WeatherAppPage()
        {
            InitializeComponent();
            GetCoordinates();
        }

        
        private string Location { get; set; } = "London";
        public double Latitude { get; set; } // широта
        public double Longitude { get; set; } // долгота

        private async void GetCoordinates()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Best);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    Latitude = location.Latitude;
                    Longitude = location.Longitude;
                    Location = await GetCity(location);

                    GetWeatherInfo();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        private async Task<string> GetCity(Location location)
        {
            var place = await Geocoding.GetPlacemarksAsync(location);
            var currentPlace = place?.FirstOrDefault();

            if (currentPlace != null)
                return $"{currentPlace.Locality}, {currentPlace.CountryName}";

            return null;
        }

        private async void GetWeatherInfo()
        {
            var url = $"http://api.openweathermap.org/data/2.5/weather?q={Location}&appid=9fd64392ba07f9eaa2684ab365706b71&units=metric&lang=ru";

            var result = await ApiCaller.Get(url);

            if (result.Successful)
            {
                try
                {

                    var weatherInfo = JsonConvert.DeserializeObject<WeatherInfo>(result.Response);

                    descriptionTxt.Text = weatherInfo.weather[0].description.ToUpper();
                    iconImg.Source = "Man.fbx"; //= $"w{weatherInfo.weather[0].icon}";
                    cityTxt.Text = weatherInfo.name.ToUpper();
                    temperatureTxt.Text = weatherInfo.main.temp.ToString("0");
                    humidityTxt.Text = $"{weatherInfo.main.humidity}%";
                    pressureTxt.Text = $"{weatherInfo.main.pressure} мм";
                    windTxt.Text = $"{weatherInfo.wind.speed} м/с";
                    cloudinessTxt.Text = $"{weatherInfo.clouds.all}%";

                    //var dt = new DateTime().ToUniversalTime().AddSeconds(weatherInfo.dt);
                    var dt = DateTime.Now;
                    dateTxt.Text = dt.ToString("dddd, MMM dd", CultureInfo.CreateSpecificCulture("ru-RU")).ToUpper();

                    GetForecast();
                } 
                catch (Exception ex)
                {
                    await DisplayAlert("Weather Info", ex.Message, "OK");
                }
            }
            else
            {
                GetCoordinates();
                await DisplayAlert("Weather Info", "No weather information found", "Ok");
            }
        }

        private async void GetForecast()
        {
            var url = $"http://api.openweathermap.org/data/2.5/forecast?q={Location}&appid=9fd64392ba07f9eaa2684ab365706b71&units=metric";
            var result = await ApiCaller.Get(url);

            if (result.Successful)
            {
                try
                {
                    var forcastInfo = JsonConvert.DeserializeObject<ForecastInfo>(result.Response);

                    List<List> allList = new List<List>();

                    foreach (var list in forcastInfo.list)
                    {
                        //var date = DateTime.ParseExact(list.dt_txt, "yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture);
                        var date = DateTime.Parse(list.dt_txt);

                        if (date > DateTime.Now && date.Hour == 0 && date.Minute == 0 && date.Second == 0)
                            allList.Add(list);
                    }

                    dayOneTxt.Text = DateTime.Parse(allList[0].dt_txt).ToString("dddd");
                    dateOneTxt.Text = DateTime.Parse(allList[0].dt_txt).ToString("dd MMM");
                    iconOneImg.Source = $"w{allList[0].weather[0].icon}";
                    tempOneTxt.Text = allList[0].main.temp.ToString("0");

                    dayTwoTxt.Text = DateTime.Parse(allList[1].dt_txt).ToString("dddd");
                    dateTwoTxt.Text = DateTime.Parse(allList[1].dt_txt).ToString("dd MMM");
                    iconTwoImg.Source = $"w{allList[1].weather[0].icon}";
                    tempTwoTxt.Text = allList[1].main.temp.ToString("0");

                    dayThreeTxt.Text = DateTime.Parse(allList[2].dt_txt).ToString("dddd");
                    dateThreeTxt.Text = DateTime.Parse(allList[2].dt_txt).ToString("dd MMM");
                    iconThreeImg.Source = $"w{allList[2].weather[0].icon}";
                    tempThreeTxt.Text = allList[2].main.temp.ToString("0");

                    dayFourTxt.Text = DateTime.Parse(allList[3].dt_txt).ToString("dddd");
                    dateFourTxt.Text = DateTime.Parse(allList[3].dt_txt).ToString("dd MMM");
                    iconFourImg.Source = $"w{allList[3].weather[0].icon}";
                    tempFourTxt.Text = allList[3].main.temp.ToString("0");

                }
                catch (Exception ex)
                {
                    await DisplayAlert("Weather Info", ex.Message, "OK");
                }
            }
            else
            {
                GetCoordinates();
                await DisplayAlert("Weather Info", "No forecast information found", "OK");
            }

        }
        void OnButtonClicked(object sender, EventArgs args)
        {
            try
            {
                Location = cityTxt.Text;
                GetWeatherInfo();
            }
            catch (Exception ex)
            {
                DisplayAlert("Weather Info", ex.Message, "OK");
            }
        }
    }
}