﻿using prj3beer.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Windows;
using Xamarin.Forms;
using System.Drawing;

namespace prj3beer.Models
{
    public class Preference
    {
        #region Attributes
        BeerContext context;


        #region Attributes
        [Key]
        [Required(ErrorMessage = "ID is required")]
        public int BeverageID { get; set; }

        [Required(ErrorMessage = "Favourite temperature is required")]
        [Range(-30, 30, ErrorMessage = "Target Temperature cannot be below -30C or above 30C")]
        public double? Temperature { get; set; }

        //[ForeignKey("bevId")]
        //[Required(ErrorMessage = "Beverage object is required")]
        //Beverage prefBev;
        #endregion


        //public Preference()
        //{

        //}

        //public Preference(int prefId, Beverage prefBev, double faveTemp)
        //{
        //    this.prefId = prefId;
        //    this.prefBev = prefBev;
        //    this.faveTemp = faveTemp;
        //}
        
      
        
             
        public double? Temperature { get; set; }

        //[ForeignKey("bevId")]
        //[Required(ErrorMessage = "Beverage object is required")]
        //Beverage prefBev;

        [DefaultValue("placeholder_can")]
        //[DefaultValue("")]
        public string ImagePath { get; set; }

        //public Image savedImage { get; set; }

        #endregion


    

        public Preference()
        {
            this.BeverageID = BeverageID;
            context = new BeerContext();
            Beverage SelectedBeverage = (context.Beverage.Find(BeverageID));

            SaveImage(SelectedBeverage.ImageURL);
        }

        public Preference(int bevID)
        {
            this.BeverageID = bevID;
            context = new BeerContext();
            Beverage SelectedBeverage = (context.Beverage.Find(BeverageID));

            //SaveImage(SelectedBeverage.ImageURL);
        }

        //public Preference(int prefId, Beverage prefBev, double faveTemp)
        //{
        //    this.prefId = prefId;
        //    this.prefBev = prefBev;
        //    this.faveTemp = faveTemp;
        //}


        /// <summary>
        /// This method will set a bool value to true when the image gets saved locally
        /// </summary>
        /// <returns></returns>
        public bool ImageSaved()
        {
            //throw new NotImplementedException();
            //if (ImagePath == null)
            if (ImagePath == "" || ImagePath == "placeholder_can" || ImagePath==null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Image SaveImage(String imageURL)
        {
            Image image = new Image();
            image.Source = "placeholder_can";
            if (ImageSaved() == false)
            {
                if (imageURL.Length > 0)
                {


                    Uri uriImage = new Uri(imageURL);
                    //Uri uriImage = new Uri("https://uca5cfff121c99373e472113118c.previews.dropboxusercontent.com/p/thumb/AAtr0X92A9m_ux4gfWXZ1prCfeYReWu9b5x36LsTq57AXO0vO-f9yz4tSWC2vSFLep7JB7a0HzZBG6iFPtgcB7Lw1V2hmwx4vpdWCXNAibIpHKHLYnL1gT6VN1MydjACBB0lpCY9HjpNgNXm2X9QWj6PzNfhv8oKd4MYeDqqd8YR8ReWsc-y5WZVIegS3MX3CYKRs9jsFRKEojX3EEF54kWUa6WrYJSUKBjEU6pEGLMqnUDEC6tOR22O3c8PoeOz5pC5PPZxaWaxU2X6IqJgqzfB8Io1pTV8ujh5A71PQg8bOqanZiNWhcjqg7vBFsbvfVj8w4g_bmP8N47fwy7FlEMhTaIrCpI9r_q44NrVUB8oo4rxfZMBtFysQ6b7e3jchAxVuK9uvY1pKUqR1slm3iET/p.png?fv_content=true&size_mode=5");

                    image.Source = new UriImageSource
                    {
                        Uri = uriImage,
                        CachingEnabled = true,
                        CacheValidity = new TimeSpan(7, 0, 0, 0)
                    };
                }

            }
            return image;

        }
    }
}
 