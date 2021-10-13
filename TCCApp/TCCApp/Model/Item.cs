﻿using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace TCCApp.Model
{
    public class Item
    {
        public String Nome { get; set; }
        public double Quantidade { get; set; }
        public String Descricao { get; set; }
        public Color Cor { get; set; }
        public ImageSource ImageUrl { get; set; }
        public byte[] ByteImage { get; set; }
        public string UserKey { get; set; }
    }
}
