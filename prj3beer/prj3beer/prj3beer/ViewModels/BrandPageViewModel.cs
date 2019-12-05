﻿using prj3beer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace prj3beer.ViewModels
{
    public class BrandPageViewModel : BasePageViewModel
    {
        public ICommand GetBrandsCommand { get; set; }
        public ObservableCollection<Brand> Brands { get; set; }
        public BrandPageViewModel()
        {
            GetBrandsCommand = new Command(async () => await GetBrands());
            GetBrandsCommand.Execute(null);
        }

        async Task GetBrands()
        {
            bool IsBusy = true;
            var brands = await Api.GetBrands();
            if (brands != null)
            {
                Brands = new ObservableCollection<Brand>(brands);
            }
            IsBusy = false;
        }

    }
}