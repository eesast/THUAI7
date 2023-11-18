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
    }
}
