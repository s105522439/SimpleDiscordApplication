using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ChatClient.Generators
{
    public class RandomColorGenerator
    {
        private Random random = new Random();

        public string GenerateColorCode()
        {
            byte r = (byte)random.Next(256);
            byte g = (byte)random.Next(256);
            byte b = (byte)random.Next(256);
            return Color.FromRgb(r,g,b).ToString();
        }
    }
}
