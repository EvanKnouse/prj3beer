﻿using System;
using System.Collections.Generic;
using System.Text;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace prj3beer.Models
{
    /// <summary>
    /// This Class holds all of the persistant settings in the beer app
    /// </summary>
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        //Constant values to store the Key and default values for the Settings Dictionary
        #region Setting Constants

        private const string TemperatureKey = "temperature_key";
        private static readonly bool TemperatureDefault = true;

        #endregion

        /// <summary>
        /// This attribute gets or sets the temperature unit settings, 
        /// </summary>
        public static bool TemperatureSettings
        {
            get
            {
                //Returns true for Celcius and false for Fahtrenheit, if Temperature was never set its default is true 
                return AppSettings.GetValueOrDefault(TemperatureKey, TemperatureDefault);
            }
            set
            {
                //Sets the TemperatureSettings KeyValue pair and sets its value to the passed in value
                AppSettings.AddOrUpdateValue(TemperatureKey, value);
            }
        }
    }

}
