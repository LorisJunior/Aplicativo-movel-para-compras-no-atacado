﻿using TCCApp.Helpers;
using TCCApp.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TCCApp.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemsPage : ContentPage
    {

        ItemViewModel itemViewModel;
        public ItemsPage()
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);

            itemViewModel = DependencyService.Get<ItemViewModel>();
           
            BindingContext = itemViewModel;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            itemViewModel.InitSubscription();
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            itemViewModel.Items.SafeClear();
            itemViewModel.Subscription.Dispose();
            itemViewModel.DeleteButtonOpacity = 0.3;
            itemViewModel.ItemSelectionMode = SelectionMode.None;
            itemViewModel.IsDeleting = false;
        }
    }
}