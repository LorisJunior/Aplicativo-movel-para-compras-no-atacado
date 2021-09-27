﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCCApp.Model;
using TCCApp.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TCCApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddItemPage : ContentPage
    {
        IItemService itemDataReference;

        public AddItemPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            itemDataReference = DependencyService.Get<IItemService>();

        }

        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                var media = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Pega uma foto"
                });
                var stream = await media.OpenReadAsync();
                itemImage.Source = ImageSource.FromStream(() => stream);
                itemImage.Margin = 0;
            }
            catch (NullReferenceException)
            {
            }
        }

        private void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Icone icone = (Icone)icones.SelectedItem;

                if (icone != null)
                {
                    itemImage.Source = icone.IconImage;
                    itemImage.Margin = 15;
                }

            }
            catch (Exception)
            {
            }
        }

        private async void criarItem_Clicked(object sender, EventArgs e)
        {
            Item item = new Item
            {
                Nome = nome.Text,
                Quantidade = quantidade.Text,
                Descricao = descricao.Text,
                Cor = produtoCor.BackgroundColor,
                ImageUrl = itemImage.Source
            };

            itemDataReference.AddItem(item);

            await DisplayAlert("Sucesso!", "O item foi criado com sucesso", "ok");

            await Navigation.PopAsync();

        }
    }
}