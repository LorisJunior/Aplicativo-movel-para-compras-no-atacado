﻿using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using TCCApp.Model;
using TCCApp.Services;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.Xaml;

namespace TCCApp.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        //Permite que apenas uma thread execute por vez
        //Isso evita a exceção Unmanaged Descriptor
        SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        Circle circle = new Circle();
        Plugin.Geolocator.Abstractions.IGeolocator locator = null;
        Pin userPin;

        private double raio = 3000;
        public double Raio
        {
            get => raio;
            set
            {
                if (raio != value)
                {
                    raio = value;
                    OnPropertyChanged();
                    UpdateCircleRadius();
                }
            }
        }

        public MapPage()
        {
            InitializeComponent();
            BindingContext = this;

            map.UiSettings.MyLocationButtonEnabled = true;
            map.PinClicked += Map_PinClicked;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                locator = CrossGeolocator.Current;
                await locator.StartListeningAsync(new TimeSpan(0, 0, 0), 100);
            }
            catch (Exception)
            {
            }
            
            var position = await locator.GetPositionAsync();
            var center = new Position(position.Latitude, position.Longitude);

            CreateCircleShapeAt(center);
            map.MoveToRegion(MapSpan.FromCenterAndRadius(center, Distance.FromMeters(5000)), true);

            App.user.Latitude = center.Latitude;
            App.user.Longitude = center.Longitude;
            CreatePin(App.user, true);
            await DatabaseService.UpdateUserAsync(App.user.Key, App.user);

            locator.PositionChanged += Locator_PositionChanged;
        }
        protected async override void OnDisappearing()
        {
            base.OnDisappearing();

            CleanMap(map.Pins);

            //Para de receber as posições
            await locator.StopListeningAsync();
        }
        private async void Locator_PositionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
        {
            var center = new Position(e.Position.Latitude, e.Position.Longitude);
            
            try
            {
                App.user.Latitude = center.Latitude;
                App.user.Longitude = center.Longitude;
                await DatabaseService.UpdateUserAsync(App.user.Key, App.user);
                userPin.Position = center;
            }
            catch (Exception)
            {
            }
            CreateCircleShapeAt(center);
            map.MoveToRegion(MapSpan.FromCenterAndRadius(center, Distance.FromMeters(5000)), true);
        }
        private void Map_PinClicked(object sender, PinClickedEventArgs e)
        {
            //TODO
            Pin pin = e.Pin;
            int userId = (int)pin.Tag;
            //var user = DatabaseService.GetUser(userId);

            //Envio uma mensagem para a TabPage ir para a ChatPage
            //TODO CHAT PAGE
            //MessagingCenter.Send<object, int>(user, "click", 3);
        }
        public async void CreatePin(User user, bool isMyPin)
        {
            await semaphoreSlim.WaitAsync();

            BitmapDescriptor icon = null;

            if (user.Buffer != null)
            {
                icon = BitmapDescriptorFactory.FromView(ImageService.GetIcon(user));
            }
            else
            {
                icon = BitmapDescriptorFactory.DefaultMarker(Color.Red);
            }

            Pin pin = new Pin()
            {
                Icon = icon,
                Type = PinType.Place,
                Label = "Olá, vms comprar juntos!",
                ZIndex = 5,
                Tag = user.Id
            };

            try
            {
                if (user.Latitude != 0 || user.Longitude != 0)
                {
                    pin.Position = new Position(user.Latitude, user.Longitude);
                }
            }
            catch (Exception)
            {
            }

            if (isMyPin == true)
            {
                userPin = pin;
                map.Pins.Add(userPin);
            }
            else
            {
                map.Pins.Add(pin);
            }

            semaphoreSlim.Release();
        }
        public void CreateCircleShapeAt(Position position)
        {
            try
            {
                CleanMap(map.Circles);

                circle.Center = position;
                circle.Radius = Distance.FromMeters(Raio);
                circle.StrokeColor = Color.DodgerBlue;
                circle.StrokeWidth = 6f;
                circle.FillColor = Color.FromRgba(0, 0, 255, 32);
                circle.Tag = "CIRCLE";

                map.Circles.Add(circle);
            }
            catch (Exception)
            {
            }
            
        }
        public void UpdateCircleRadius()
        {
            circle.Radius = Distance.FromMeters(Raio);
        }
        public void CleanMap(IEnumerable<object> o)
        {
            if (o is IList<Circle> && o != null)
            {
                try
                {
                    map.Circles.Clear();
                }
                catch (Exception)
                {
                }
            }
            else if (o is IList<Pin> && o != null)
            {
                try
                {
                    map.Pins.Clear();
                }
                catch (Exception)
                {
                }
            }
        }
        private async void Search_Clicked(object sender, EventArgs e)
        {
            await semaphoreSlim.WaitAsync();

            try
            {
                CleanMap(map.Pins);
                CreatePin(App.user, true);
                AddNearUsers();
            }
            catch (Exception)
            {
            }
            
            semaphoreSlim.Release();
        }
        public async void AddNearUsers()
        {
            //Permite que apenas uma thread execute por vez
            //Isso evita a exceção Unmanaged Descriptor
            await semaphoreSlim.WaitAsync();

            var allUsers = await DatabaseService.GetNearUsers();
            var nearUsers = allUsers.Where(u => u.Key != App.user.Key &&
                    DistanceService
                    .CompareDistance(App.user.Latitude, App.user.Longitude, u.Latitude, u.Longitude) <= (raio / 1000)
                    && u.DisplayUserInMap);

            foreach (var user in nearUsers)
            {
                CreatePin(user, false);
            }

            semaphoreSlim.Release();
        }
    }
}