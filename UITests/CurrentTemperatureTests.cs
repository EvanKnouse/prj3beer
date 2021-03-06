﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.UITest;
using Xamarin.UITest.Queries;
using prj3beer.Services;
using System.Threading;

namespace UITests
{
    [TestFixture(Platform.Android)]
    class CurrentTemperatureTests
    {

        IApp app;
        Platform platform;

        string apkPath = "D:\\virpc\\prj3beer\\prj3.beer\\prj3beer\\prj3beer.Android\\bin\\Debug\\com.companyname.prj3beer.apk";

        public CurrentTemperatureTests(Platform platform)
        {
            this.platform = platform;
        }

        public void selectABeverage(String searchBeverage)
        {
            // tap to navigate to the beverage select screen
            //app.Tap("Beverage Select");
            app.EnterText("searchBeverage", searchBeverage.ToString());

            //app.TapCoordinates(200, 710);
            app.TapCoordinates(200, 550);
        }

        [SetUp]
        public void BeforeEachTest()
        {
            //Initialize the app, arrive at home page (default for now)
            app = app = ConfigureApp.Android.ApkFile(apkPath).StartApp();
            //Tap into the screen navigation menu
            //app.TapCoordinates(150, 90);

            selectABeverage("Great Western Radler");

            //app.TapCoordinates(1350, 175);
            //app.TapCoordinates(175, 1350);

            ////Tap into the screen navigation menu (default for now)
            //app.Tap(c => c.Marked("ScreenSelectButton"));
        }

        [Test]
        public void TestTemperatureBelowRangeError()
        {
            //Tap on the status text to navigate the status screen
            //app.Tap("Status");

            //Wait for the current Temperature label to appear on screen
            app.WaitForElement("currentTemperature");

            //Wait for the temperature to dip below -30C
            Thread.Sleep(54000);

            //Store the labels text 
            String result = app.Query("currentTemperature")[0].Text;

            //Passes if string equals 'Temperature Out Of Range' "
            Assert.AreEqual(result, "Temperature Out Of Range");
        }

        [Test]
        public void TestTemperatureAboveRangeError()
        {
            //Tap on the status text to navigate the status screen
            //app.Tap("Status");

            //Wait for the current Temperature label to appear on screen
            app.WaitForElement("currentTemperature");

            //Wait for the temperature to go above 30C
            Thread.Sleep(123000);

            //Store the labels text 
            String result = app.Query("currentTemperature")[0].Text;

            //Passes if string equals 'Temperature Out Of Range' "
            Assert.AreEqual(result, "Temperature Out Of Range");
        }

        [Test]
        public void TestCurrentTemperatureLabelUpdatesWithTemperatureReadings()
        {
            //app.Tap("Status");


            app.WaitForElement("currentTemperature");

            string firstReading = app.Query("currentTemperature")[0].Text;

            Thread.Sleep(1500);

            string secondReading = app.Query("currentTemperature")[0].Text;

            Assert.IsFalse(firstReading.Equals(secondReading));

        }
    }
}
