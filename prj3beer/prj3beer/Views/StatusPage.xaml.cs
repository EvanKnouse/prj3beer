﻿using Microsoft.Data.Sqlite;
using prj3beer.Models;
using prj3beer.Services;
using prj3beer.ViewModels;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace prj3beer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StatusPage : ContentPage
    {
        // Static View Model for the page
        static StatusViewModel svm;
        // Static Beverage to keep track of current Beverage
        static Beverage currentBeverage;
        // Static Preference Object to keep track of
        public static Preference preferredBeverage;
        // Static Brand Object to keep track of current Beverage/Preference
        static Brand currentBrand;
        // Saved ID from the last selected Beverage
        int savedID;
		
        public StatusPage()
        {
            InitializeComponent();

            //The id on the settings page of the app
            // Defaults as -1, seleccting a beverage changes it
            savedID = Settings.BeverageSettings;

            //If a beverage was not selected
            if (savedID == -1)
            {
                //Set default values of a beverage
                beverageName.Text = "No Beverage";
                brandName.Text = "No Brand";
                beverageImage.Source = ImageSource.FromFile("placeholder_can");
                TemperatureStepper.IsEnabled = false;
                TemperatureInput.IsEnabled = false;
            }
            else
            {
                // Instantiate new StatusViewModel
                svm = new StatusViewModel();

                // Setup the current Beverage (find it from the Context) -- This will be passed in from a viewmodel/bundle/etc in the future.
                //currentBeverage = new Beverage { BeverageID = 1, Name = "Great Western Radler", Brand = svm.Context.Brands.Find(2), Type = Models.Type.Radler, Temperature = 2 };
                currentBeverage = svm.Context.Beverage.Find(savedID);
                currentBrand = svm.Context.Brand.Find(currentBeverage.BrandID);
                //svm.Context.Beverage.Find(2);

                // Setup the preference object using the passed in beverage
                SetupPreference();

                // When you first start up the Status Screen, Disable The Inputs (on first launch of application)
                EnablePageElements(false);

                // If the current Beverage is set, (will run if a beverage has been selected)
                if (preferredBeverage != null)
                {   // enable all the elements on the page
                    EnablePageElements(true);
                }

                PopulateStatusScreen();  
            }
            MockTempReadings.StartCounting();
        }

        /// <summary>
        /// This method is run if there is a beverage selected
        /// Sets the images and text elements to represent the selected beverage
        /// </summary>
        private void PopulateStatusScreen()
        {
            // Sets displayed information of the beverage
            beverageName.Text = currentBeverage.Name.ToString();
            brandName.Text = currentBrand.Name.ToString();
            beverageImage.Source = (preferredBeverage.SaveImage(currentBeverage.ImageURL)).Source;
            
            // Size of all our images we currently use, and looks good on screen
            beverageImage.WidthRequest = 200;
            beverageImage.HeightRequest = 200;

            // Ensure elements are enabled if there is a beverage selected
            beverageName.IsEnabled = false;
            brandName.IsEnabled = false;
            beverageImage.IsEnabled = false;


            //This is where to put the check if preffered beverage is already favorited or not and change the sourse accordingly

            FavouriteButton.Source = preferredBeverage.Favourite ? "Favourite" : "NotFavourite";
            //FavouriteButton.Source = "Favourite";
            
        }

        public void UpdateViewModel(object sender, EventArgs args)
        {
            svm.IsCelsius = Settings.TemperatureSettings;
        }

        /// <summary>
        /// This method sets up a Preferred beverage object with the passed in beverage
        /// </summary>
        /// <param name="passedInBeverage">Beverage passed in from other page</param>
        //private void SetupPreference(Beverage passedInBeverage)
        private void SetupPreference()
        {   // Set the page's preferred beverage equal to -> Finding the Beverage in the Database.
            // If the object is found in the database, it will return itself immediately,
            // and attach itself to the context (Database).

            // TODO: Handle Pre-existing Preference Object.
            preferredBeverage = svm.Context.Preference.Find(savedID);
            //preferredBeverage = null; // This is what the previous line SHOULD be doing.

            // If that Preferred beverage did not exist, it will be set to null,
            // So if it is null...
            if(preferredBeverage == null)
            {   // Create a new Preferred Beverage, with copied values from the Passed In Beverage.
                preferredBeverage = new Preference() { BeverageID = currentBeverage.BeverageID, Temperature = currentBeverage.Temperature, Favourite = false };
                // Add the beverage to the Context (Database)
                svm.Context.Preference.Add(preferredBeverage);
                svm.Context.SaveChanges();
            }
        }
       
        /// <summary>
        /// This method will write changes to the Database for any changes that have happened.
        /// </summary>
        /// <param name="context">Database</param>
        public async void UpdatePreference(BeerContext context)
        {
            try
            {   // Set the Temperature of the Preferred beverage to the StatusViewModel's Temperature,
                // Do a calculation if the temperature is currently set to fahrenheit
                preferredBeverage.Temperature = svm.IsCelsius ? svm.Temperature.Value : ((svm.Temperature.Value - 32) / 1.8);
            }
            catch (Exception)
            {
                throw;
            }

            try
            {   // Write Changes to Database when it is not busy.
                svm.Context.Preference.Update(preferredBeverage);
                await context.SaveChangesAsync();
            }
            catch (SqliteException) { throw; }
        }

        /// <summary>
        /// This method will enable or disable all inputs on the screen
        /// </summary>
        /// <param name="enabled">True or False</param>
        private void EnablePageElements(bool enabled)
        {
            // Enable/Disable Steppers
            this.TemperatureStepper.IsEnabled = enabled;

            // Enable / Disable Entry
            this.TemperatureInput.IsEnabled = enabled;
        }

        /// <summary>
        /// This method will clear the input field when the Entry Box gains focus
        /// </summary>
        /// <param name="sender">Entry Field</param>
        /// <param name="args"></param>
        private void SelectEntryText(object sender, EventArgs args)
        {   // Store the sender casted as an entry to an Entry Object (to avoid casting repeatedly)
            Entry text = (Entry)sender;

            // This string will get the text from the StatusViewModel's Preferred Temperature String
            string cursorPosition = ((StatusViewModel)BindingContext).PreferredTemperatureString;
            // Set the value of the entry to an empty string
            text.Text = "";
            // Then set the text to the text retreived from the SVM
            text.Text = cursorPosition;

            // Set the cursor position to 0
            text.CursorPosition = 0;
            // Select all of the Text in the Entry
            text.SelectionLength = text.Text.Length;

            UpdatePreference(svm.Context);
            //this.PrefTemp.Text = preferredBeverage.Temperature.ToString();
        }

        /// <summary>
        /// When the Stepper is changed, update the preference
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TemperatureStepperChanged(object sender, ValueChangedEventArgs e)
        {   // Update the preference object using the Context in the StatusViewModel
            UpdatePreference(svm.Context);
        }

        /// <summary>
        /// This method is called every time the page is opened.
        /// </summary>
        protected override void OnAppearing()
        {   // Instantiate a new StatusViewModel
            svm = new StatusViewModel();

            //currentTemperature.SetBinding(Label.TextProperty, "CurrentTemp", default, new CelsiusFahrenheitConverter());

            if (Settings.BeverageSettings != -1)// So default opening no longer uses a drink that does not exist
            {
                // Set it's Monitored Celsius value to the value from the Settings 
                svm.IsCelsius = Settings.TemperatureSettings;

                // Set the Temperature Stepper to the Max/Minimum possible
                TemperatureStepper.Maximum = 86;
                TemperatureStepper.Minimum = -30;

                // Set the temperature of the StatusViewModel to the current preferred beverage temperature
                svm.Temperature = preferredBeverage.Temperature;

                // is we are currently set to Celsius,
                if (svm.IsCelsius)
                {   // Set the Steppers to Min/Max for Celsius,
                    TemperatureStepper.Minimum = -30;
                    TemperatureStepper.Maximum = 30;
                }
                else
                {   // Otherwise set the Min/Max to Fahrenheit
                    TemperatureStepper.Minimum = -22;
                    TemperatureStepper.Maximum = 86;
                }
                //  Update the binding context to equal the new StatusViewModel
                BindingContext = svm;
            }

            else
            {   // Otherwise set the Min/Max to Fahrenheit
                TemperatureStepper.Minimum = -22;
                TemperatureStepper.Maximum = 86;
            }
            //  Update the binding context to equal the new StatusViewModel
            BindingContext = svm;

            LogInOutButton();
        }

        /// <summary>
        /// This method will replace the Log In / Log Out button 
        /// </summary>
        private void LogInOutButton()
        {
            // Remove the Log in/out button
            ToolbarItems.RemoveAt(1);

            // If there is no current user signed in
            if (Settings.CurrentUserEmail.Length == 0)
            {
                // Create a new ToolBar Button
                ToolbarItem SignInButton = new ToolbarItem
                {   // Assign it the properties below
                    AutomationId = "SignIn",
                    Text = "Sign In",
                    // Set the menu button to the sub-menu
                    Order = ToolbarItemOrder.Secondary
                };
                // Add the button to the menu
                ToolbarItems.Add(SignInButton);
            }
            else // A user is signed in
            {   // Create a new toolbar button caled Sign Out
                ToolbarItem SignOutButton = new ToolbarItem
                {   // Assign it the properties below
                    AutomationId = "SignOut",
                    Text = "Sign Out",
                    // Set the menu button to the sub-menu
                    Order = ToolbarItemOrder.Secondary
                };
                // Add the button to the menu
                ToolbarItems.Add(SignOutButton);
            }
            // Add the click event handler to the button
            ToolbarItems.ElementAt(1).Clicked += SignInOut_Clicked;
        }

        /// <summary>
        /// This method is called when the Settings Menu Button is Clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_Clicked(object sender, EventArgs e)
        {
            // Push a new settings modal
            Navigation.PushModalAsync(new NavigationPage(new SettingsMenu()));
        }

        /// <summary>
        /// This method is called when the Sign In or Out button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SignInOut_Clicked(object sender, EventArgs e)
        {
            // Push a new login page modal
            await Navigation.PushModalAsync(new CredentialSelectPage());
        }

        public void ReAppearing()
        {
            OnAppearing();
        }

		private void FavouriteButtonClicked(object sender, EventArgs e)
        {
            // If the beverage is favourited.
            if(preferredBeverage.Favourite)
            {
                // Remove as a favourite.
                preferredBeverage.Favourite = false;
                // Change the FavouriteButton image source.
                FavouriteButton.Source = "NotFavourite";
            }
            // If the beverage is favourited.
            else
            {
                // If the user has less than 5 favourited beverages.
                if(svm.Context.Preference.Where(p => p.Favourite == true).Count() < 5)
                {
                    // Add as a favourite.
                    preferredBeverage.Favourite = true;
                    // Change the FavouriteButton image source.
                    FavouriteButton.Source = "Favourite";
                }
                // If the user has 5 favourited beverages. Limit on number of favourites can be changed at any time.
                else
                {
                    // Display a toast to the user, works for Android and iOS.
                    DependencyService.Get<IToastHandler>().LongToast("You cannot have any more favourites");
                }
            }

            // Update the preferredBeverage as its favourite boolean may have changed.
            svm.Context.Preference.Update(preferredBeverage);
            // Saving changes is necessary as the EntityFramework tracking doesn't seem dependable.
            svm.Context.SaveChanges();
        }
    }
}