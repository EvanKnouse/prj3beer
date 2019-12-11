﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Windows;

namespace prj3beer.Models
{
    public class Preference
    {
        #region Attributes
        [Key]
        [Required(ErrorMessage = "ID is required")]
        public int BeverageID { get; set; }

        [Required(ErrorMessage = "Favourite temperature is required")]
        [Range(-30, 30, ErrorMessage = "Target Temperature cannot be below -30C or above 30C")]
        public double Temperature { get; set; }

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
        
      
        
    }
}