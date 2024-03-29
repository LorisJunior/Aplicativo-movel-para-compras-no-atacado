﻿using System;
using TCCApp.Services;
using TCCApp.View;
using TCCApp.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TCCApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        LoginViewModel loginViewModel;
        public MainPage(IOAuth2Service oAuth2Service)
        {
            loginViewModel = new LoginViewModel(this, oAuth2Service);
            InitializeComponent();
            BindingContext = loginViewModel;
            NavigationPage.SetHasNavigationBar(this, false);
        }        

        private async void EsqSenhaButton_Clicked(object sender, EventArgs e)
        {
            
            await Navigation.PushAsync(new EsqSenhaPage());
            
        }

        private async void CadastroButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CadastroPage());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            App.user = new Model.User();
        }
    }
}
