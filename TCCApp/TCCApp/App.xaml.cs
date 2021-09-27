﻿using System;
using System.Collections.Generic;
using TCCApp.Helpers;
using TCCApp.Serviços;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TCCApp
{
    public partial class App : Application
    {
        public App(IOAuth2Service oAuth2Service)
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage(oAuth2Service));

            var statusBarColor = DependencyService.Get<ISetStatusBarColor>();

            statusBarColor.SetStatusBarColor(Color.White, false);
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
