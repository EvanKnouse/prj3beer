﻿using NUnit.Framework;
using System.Linq;
using Xamarin.UITest;
using Xamarin.UITest.Queries;
using prj3beer.Models;
using Xamarin.Forms;
using Xamarin.UITest.Configuration;
using prj3beer.Services;

namespace UITests
{
    [TestFixture(Platform.Android)]
    //[TestFixture(Platform.iOS)]
    public class StatusTests
    {
        IApp app;
        Platform platform;

        string apkFile = "D:\\virpc\\prj3beer\\prj3.beer\\prj3beer\\prj3beer.Android\\bin\\Debug\\com.companyname.prj3beer.apk";

        public StatusTests(Platform platform)
        {
            this.platform = platform;
        }

        [SetUp]
        public void BeforeEachTest()
        {
            //Initialize the app, arrive at home page (default for now)
            app = ConfigureApp.Android.ApkFile(apkFile).StartApp();
            //Tap into the screen navigation menu
            app.TapCoordinates(150, 90);
            ////Tap into the screen navigation menu (default for now)
            //app.Tap(c => c.Marked("ScreenSelectButton"));

            ////Sets the Temperature settings to celsius for every test
            //Settings.TemperatureSettings = true;

            ////Sets the master Notification setting to on
            //Settings.NotificationSettings = true;

            ////Sets the In Range Notification setting to on
            //Settings.InRangeSettings = true;

            ////Sets the Too Hot/Cold Notification setting to on
            //Settings.NotificationSettings = true;
        }

        [Test]
        public void TestSettingsMenuIsDisplayedOnStatusScreenWhenSettingsButtonIsPressed()
        {
            //Pick Status screen from the screen selection menu
            app.Tap("Status");

            //Wait for the Settings button to appear on screen
            app.WaitForElement("Settings");

            //Press Settings Menu button
            app.Tap("Settings");

            //Wait for the Temperature switch to appear on screen
            app.WaitForElement("SettingsTable");

            //Look for the toggle button on the Settings Menu
            AppResult[] button = app.Query(("SettingsTable"));

            //Will be greater than 0 if it exists, returns AppResult[]
            Assert.IsTrue(button.Any());
        }

        [Test]
        public void TestSettingsAreAppliedOnSettingsChange()
        {
            Settings.TemperatureSettings = true;

            //Pick Status screen from the screen selection menu
            app.Tap("Status");

            //Wait for the Settings button to appear on screen
            app.WaitForElement("Settings");

            //Check that the label for the current temperature is set to "\u00B0C"
            string tempLabel = app.Query("currentTemperature")[0].Text;

            //If equal, the temperature label has been set to Celsius
            bool isCelsius = tempLabel.Contains("\u00B0C");

            //Press Settings menu button
            app.Tap("Settings");

            //Wait for the Temperature switch to appear on screen
            app.WaitForElement("SettingsTable");

            //Tap on the toggle button to change the temperature setting to fahrenheit
            //app.Tap("switchTemp");
            app.Tap(e => e.Class("SwitchCellView").Class("Switch").Index(0));

            //Go back to the Status screen
            app.Back();

            //Wait for the Current Temperature Label to appear on screen
            app.WaitForElement("currentTemperature");

            //Check that the label for the current temperature is set to "\u00B0F"
            tempLabel = app.Query("currentTemperature")[0].Text;

            bool isFahrenheit = tempLabel.Contains("\u00b0F");

            //If equal, the temperature was initially set to celsius
            Assert.AreEqual(true, isCelsius);
            //If equal, the temperature label has been set to fahrenheit and the settings have been applied
            Assert.AreEqual(true, isFahrenheit);
        }

        [Test]
        public void TestThatSettingsMenuDisplaysCurrentAppSettings()
        {
            //Pick status screen from the screen selection menu
            app.Tap("Status");

            //Wait for the Settings button to appear on screen
            app.WaitForElement("Settings");

            //Press Settings menu button
            app.Tap("Settings");

            //Wait for the Temperature switch to appear on screen
            app.WaitForElement("SettingsTable");

            //Grab the switch value to prove it was Celsius before it switched
            bool switchValue = app.Query(e => e.Class("SwitchCellView").Class("Switch").Index(0).Invoke("isChecked").Value<bool>()).First();

            //If it was originally Celsius it would pass
            Assert.AreEqual(true, switchValue);

            //Tap on the toggle button to change the temperature setting to fahrenheit
            app.Tap(e => e.Class("SwitchCellView").Class("Switch").Index(0));

            //Grab the temperature label text to prove it switched
            switchValue = app.Query(e => e.Class("SwitchCellView").Class("Switch").Index(0).Invoke("isChecked").Value<bool>()).First();

            //Check if the enabled value is true
            Assert.AreEqual(false, switchValue);
        }

        #region Story 04 UI Tests
        // Test that the application is currently on the status page
        [Test]
        public void AppIsOnStatusPage()
        {
            //Pick Status screen from the screen selection menu
            //app.Tap("Status");
            //Thread.Sleep(5000);

            app.TapCoordinates(150, 90);

            //Thread.Sleep(5000);

            app.WaitForElement("StatusPage");

            AppResult[] results = app.Query("StatusPage");

            Assert.IsTrue(results.Any());
        }

        // Test that the target temperature entry field is on the status page
        [Test]
        public void TargetTempEntryIsOnPage()
        {
            //Pick Status screen from the screen selection menu
            app.Tap("Status");

            app.WaitForElement("currentTarget");

            AppResult[] results = app.Query("currentTarget");

            Assert.IsTrue(results.Any());
        }

        // Test that the target temperature stepper buttons are on the status page
        [Test]
        public void StepperIsOnPage()
        {
            //Pick Status screen from the screen selection menu
            app.Tap("Status");

            app.WaitForElement("TempStepper");

            AppResult[] results = app.Query("TempStepper");
            Assert.IsTrue(results.Any());
        }

        // Test that a temperature value can be entered in the entry field on the status page
        [Test]
        public void TargetTempEntryFieldCanBeSetManually()
        {
            //Pick Status screen from the screen selection menu
            app.Tap("Status");

            app.WaitForElement("currentTarget");

            int userInput = 2;

            app.Tap("currentTarget");
            app.ClearText("currentTarget");
            app.EnterText("currentTarget", userInput.ToString());
            app.PressEnter();

            string targetTemperature = app.Query("currentTarget")[0].Text;

            Assert.AreEqual(userInput.ToString(), targetTemperature);
        }

        // Test that the target temperature value in the entry field is incremented by 1
        [Test]
        public void TargetTempIsIncrementedByButton()
        {
            //Pick Status screen from the screen selection menu
            app.Tap("Status");

            app.WaitForElement("currentTarget");

            int startTemp = int.Parse(app.Query("currentTarget")[0].Text);

            app.TapCoordinates(860, 1650);

            string targetTemperature = app.Query("currentTarget")[0].Text;

            Assert.AreEqual((startTemp + 1).ToString(), targetTemperature);
        }

        // Test that the target temperature value in the entry field is decremented by 1
        [Test]
        public void TargetTempIsDecrementedByButton()
        {
            //Pick Status screen from the screen selection menu
            app.Tap("Status");

            app.WaitForElement("currentTarget");

            int startTemp = int.Parse(app.Query("currentTarget")[0].Text);

            app.TapCoordinates(560, 1560);

            string targetTemperature = app.Query("currentTarget")[0].Text;

            Assert.AreEqual((startTemp - 1).ToString(), targetTemperature);
        }
        #endregion

        #region Story 15 UI Tests
        [Test]
        public void TestThatTurningOffMasterNotificationSwitchHidesNotificationsSubSettings()
        {
            //Pick status screen from the screen selection menu
            app.Tap("Status");

            //Wait for the Settings button to appear on screen
            app.WaitForElement("Settings");

            //Press Settings menu button
            app.Tap("Settings");

            app.WaitForElement("SettingsTable");

            //Look for InRange switch
            AppResult[] results = app.Query(e => e.Class("SwitchCellView").Class("Switch").Index(2));
     
            //Does the InRange switch exist before turning off master notifications
            Assert.IsTrue(results.Any());

            //Tap on the master notification switch, turning notifications off
            app.Tap(e => e.Class("SwitchCellView").Class("Switch").Index(1));

            //Get the result of querying for the notification sub-setting switch
            results = app.Query(e => e.Class("SwitchCellView").Class("Switch").Index(2));

            //Results should not contain the notification in range switch
            Assert.IsFalse(results.Any());

        }

        [Test]
        public void TestThatTurningOnMasterNotificationSwitchShowsNotificationsSubSettings()
        {
            //Pick status screen from the screen selection menu
            app.Tap("Status");

            //Wait for the Settings button to appear on screen
            app.WaitForElement("Settings");

            //Press Settings menu button
            app.Tap("Settings");

            app.WaitForElement("SettingsTable");

            //Turn off master notifications
            app.Tap(e => e.Class("SwitchCellView").Class("Switch").Index(1));

            //Get the result of querying for the inRange notification switch
            AppResult[] results = app.Query(e => e.Class("SwitchCellView").Class("Switch").Index(2));

            //Results should not contain the inRange switch
            Assert.IsFalse(results.Any());

            //app.WaitForElement("notificationSubSettings");

            //Turn on notifications by tapping master notification switch
            app.Tap(e => e.Class("SwitchCellView").Class("Switch").Index(1));

            //Get the result of querying for the inRange notification switch
            results = app.Query(e => e.Class("SwitchCellView").Class("Switch").Index(2));

            //Results should contain the notification in range switch
            Assert.IsTrue(results.Any());
        }

        [Test]
        public void TestThatNotificationSettingsPersistOnAppReload()
        {
            //Pick Status screen from the screen selection menu
            app.Tap("Status");

            app.WaitForElement("Settings");

            //Press Settings menu button
            app.Tap("Settings");

            app.WaitForElement("SettingsTable");

            //Tap the master notifications switch
            app.Tap(e => e.Class("SwitchCellView").Class("Switch").Index(1));

            bool firstSwitchValue = app.Query(e => e.Class("SwitchCellView").Class("Switch").Index(1).Invoke("isChecked").Value<bool>()).First();

            app = ConfigureApp.Android.ApkFile(apkFile).StartApp(AppDataMode.DoNotClear);

            app.TapCoordinates(150, 90);

            //Pick Status screen from the screen selection menu
            app.Tap("Status");

            app.WaitForElement("Settings");

            //Press Settings menu button
            app.Tap("Settings");

            app.WaitForElement("SettingsTable");

            bool secondSwitchValue = app.Query(e => e.Class("SwitchCellView").Class("Switch").Index(1).Invoke("isChecked").Value<bool>()).First();

            Assert.AreEqual(true, firstSwitchValue == secondSwitchValue);
        }

        [Test]
        public void TestThatOnlyPerfectNotificationsAreSent()
        {
            //Set master notifications setting is on
            Settings.NotificationSettings = true;

            //Set in range notifications setting to off
            Settings.InRangeSettings = false;

            //Set too hot/cold notifications setting to off
            Settings.TooHotColdSettings = false;

            //Pick Status screen from the screen selection menu
            //app.Tap("Status");

            //app.WaitForElement("Settings");

            ////Press Settings menu button
            //app.Tap("Settings");

            //app.WaitForElement("SettingsTable");

            //app.Tap(e => e.Class("SwitchCellView").Class("Switch").Index(2));

            //app.Tap(e => e.Class("SwitchCellView").Class("Switch").Index(3));

            //app.Back();

            Assert.AreEqual(0, Notifications.TryNotification(6, 5, NotificationType.NO_MESSAGE));

            Assert.AreEqual(3, Notifications.TryNotification(5, 5, NotificationType.NO_MESSAGE));

            Assert.AreEqual(0, Notifications.TryNotification(0, 5, NotificationType.PERFECT));
        }
        #endregion
    }
}
