using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
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
        public static int getIndex(int i, int j)
        {
            return 50 * i + j;
        }

        public static PointF getMapCenter(float i, float j)
        {
            return new PointF(10 * i + 5, 10 * j + 5);
        }
    }


}
