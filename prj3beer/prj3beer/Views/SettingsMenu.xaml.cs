﻿using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using prj3beer.ViewModels;
using System;

namespace prj3beer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsMenu : ContentPage
    {
        
        public SettingsMenu()
        {
            InitializeComponent();
            
            //Check the Settings class to see if set to celsius or fahrenheit
            switchTemp.On = Models.Settings.TemperatureSettings;

            //Set the notification master switch to on by default
            switchNotifications.On = Models.Settings.NotificationSettings;

            
        }

        public void UpdateViewModel(object sender, EventArgs args)
        {

        }

        //changes temperature display settings in response to the switch 
        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            switchTemp.Text = e.Value ? "Celsius" : "Fahrenheit";

            Models.Settings.TemperatureSettings = e.Value;
        }

        //Closes the settings modal
        async private void Close_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}