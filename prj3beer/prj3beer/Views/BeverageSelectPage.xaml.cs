﻿using System;
using System.Collections.Generic;
using prj3beer.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using prj3beer.Services;
using System.Linq;
using System.Text.RegularExpressions;

namespace prj3beer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    //This page will contain a search bar and a potential list of beverages. Defaults to
    //No beverages displayed if search bar is left blank
    public partial class BeverageSelectPage : ContentPage
    {
        //A list that will contain all valid beverages that meet the search criteria
        List<String> listViewBeverages = new List<String>();

        /// <summary>
        /// This will initialize the page and bring in the beverage objects from local storage
        /// and places in a context
        /// </summary>
        /// <param></param>
        public BeverageSelectPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //  Setup The Menu Button
            LogInOutButton();
        }

        /// <summary>
        /// This method will replace the Log In / Log Out button 
        /// </summary>
        private void LogInOutButton()
        {
            // Remove the Log in/out button
            ToolbarItems.RemoveAt(1);

            //bool loggedOut = (Settings.CurrentUserEmail.Length == 0) ? true : false;

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
        /// This method will take an a search input change event and attempt to
        /// find any beverages that list the criteria
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchChanged(object sender, TextChangedEventArgs e)
        {
            //Display the loading spinner
            loadingSpinner.IsRunning = true;

            //Make a new listview of beverages - essentially resetting it
            listViewBeverages = new List<string>();

            //Grab the text of the search string, trim it, and make it all lower case
            string searchString = searchBeverage.Text.ToString().Trim();

            searchString = Regex.Replace(searchString, @"\s+", " ");
            // grab the text of the search string without modifying it, for use in the error message label
            string potentialInvalidSearch = searchString;
            searchString = searchString.ToLower();

            //Uses the entity framework to find beverages which might the criteria
            //Checks on 3 different fields (brandName, Name of beverage, and the type) and stores it

            //Check to see if the search string finds any brands with a mtaching name
            var brands = App.Context.Brand.Where(b => b.Name.ToLower().Contains(searchString)).Distinct();

            //Initialize a nullable integer to store the brand ID
            int? brand = null;

            try
            {
                //Try to save the result from the previous search
                brand = brands.First().BrandID;
            }
            catch(Exception)
            {
                //If there is an exception, reset the brand ID to null
                brand = null;
            }

            // Search the Beverages Database for search string and brand ID that matches
            var beverages = App.Context.Beverage.Where(b => b.BrandID.Value.Equals(brand) || b.Name.ToLower().Contains(searchString) || b.Type.ToString().ToLower().Contains(searchString)).Distinct();
            
            //If the search string is not empty
            if (!searchString.Equals(""))
            {
                //hide the error message
                errorLabel.IsVisible = false;

                //for each beverage add it to the list
                foreach (var beverage in beverages)
                {
                    listViewBeverages.Add(beverage.Name);
                }

                //Sort beverage list alphabetically for display
                listViewBeverages.Sort();

                //If there are no beverages
                if (listViewBeverages.Count() == 0)
                {
                    //set the item source to null (make it empty)
                    beverageListView.ItemsSource = null;
                    // display the error label
                    errorLabel.IsVisible = true;
                    //append the current search into the error message
                    errorLabel.Text = "\"" + potentialInvalidSearch + "\" could not be found/does not exist";
                }
                else //there are beverages
                {
                    //Set spinner hidden to true
                    beverageListView.ItemsSource = listViewBeverages;
                }
            }
            else //search string is empty
            {
                //hide the error message
                errorLabel.IsVisible = false;
                //reset the list
                beverageListView.ItemsSource = null;
            }
            //hide the load spinner
            loadingSpinner.IsRunning = false;
        }

        /// <summary>
        /// Selecting a beverage from the list
        /// Sets the settings page prefferd ID to be used on the stauts page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BeverageTapped(object sender, ItemTappedEventArgs e)
        {
            //Get the beverage tapped
            Beverage tappedBeverage = (App.Context.Beverage.Where(b => b.Name.Contains(e.Item.ToString()))).First();
            
            //Get that beverage's ID
            Settings.BeverageSettings = tappedBeverage.BeverageID;
            //Application.Current.MainPage = new NavigationPage(new StatusPage());
            // await Navigation.PushModalAsync(new NavigationPage(new StatusPage()));
            
            //Set the ID to the setting page
            var id = (int)MenuItemType.Status;

            //Go to the settings page, done like this to keep the menu - May need to be changed later
            //await RootPage.NavigateFromMenu(id);
            await Navigation.PushAsync(new StatusPage());
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
    }
}