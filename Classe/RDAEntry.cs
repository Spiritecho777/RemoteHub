using Avalonia.Controls;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteHub.Classe
{
    public class RDAEntry
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Software { get; set; }
        public string Features { get; set; } // => JSON
        public string Icon { get; set; }

        public Bitmap IconBitmap
        {
            get
            {
                var bytes = Convert.FromBase64String(Icon);
                using var ms = new System.IO.MemoryStream(bytes);
                var bmp = new Bitmap(ms);
                return bmp;            
            }
        }
    }
}
