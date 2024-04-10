using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Model;
using Microsoft.Maui.Controls.Internals;
using Protobuf;

namespace Client.Util
{
    public class UtilFunctions
    {
        public static void SlideLengthSet(BoxView slide, ref bool haveSetSlideLength, ref double lengthOfSlide, double parentGridWidth)
        {
            if (parentGridWidth < 0)
            {
                return;
            }
            if (haveSetSlideLength == false)
            {
                lengthOfSlide = parentGridWidth;
                slide.WidthRequest = lengthOfSlide;
            }
            else
            {
                slide.WidthRequest = slide.WidthRequest / lengthOfSlide * parentGridWidth;
                lengthOfSlide = parentGridWidth;
            }
        }
        public static int getCellIndex(int i, int j)
        {
            return 50 * i + j;
        }
        public static int getGridIndex(float i, float j)
        {
            return 50 * (int)i / 1000 + (int)j / 1000;
        }

        public static PointF Grid2CellPoint(float i, float j)
        {
            return new PointF(i / 1000, j / 1000);
        }

        public static bool IsShipEqual(Ship a, Ship b)
        {
            System.Diagnostics.Debug.WriteLine($"{Convert.ToString(a.TeamID)}, {Convert.ToString(b.TeamID)},{Convert.ToString(a.Type)}, {Convert.ToString(b.Type)},{Convert.ToString(a.State)}, {Convert.ToString(b.State)},{Convert.ToString(a.HP)}, {Convert.ToString(b.HP)},{Convert.ToString(a.ProducerModule)}, {Convert.ToString(b.ProducerModule)},{Convert.ToString(a.ConstuctorModule)}, {Convert.ToString(b.ConstuctorModule)},{Convert.ToString(a.ShieldModule)}, {Convert.ToString(b.ShieldModule)},{Convert.ToString(a.WeaponModule)}, {Convert.ToString(b.WeaponModule)}");
            if (a == null || b == null) return false;
            if (
                a.TeamID == b.TeamID &&
                a.Type == b.Type &&
                a.State == b.State &&
                a.HP == b.HP &&
                a.ProducerModule == b.ProducerModule &&
                a.ConstuctorModule == b.ConstuctorModule &&
                a.ArmorModule == b.ArmorModule &&
                a.ShieldModule == b.ShieldModule &&
                a.WeaponModule == b.WeaponModule
                )
            {
                System.Diagnostics.Debug.WriteLine("Equal!");
                return true;
            }
            else
                return false;
        }
    }
}
