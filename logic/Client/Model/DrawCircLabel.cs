using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Model
{
    public class DrawCircLabel : BindableObject
    {
        private float x;
        public float X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Thick));
            }
        }
        private float y;
        public float Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Thick));
            }
        }
        private Thickness thick;
        public Thickness Thick
        {
            get 
            { 
                thick.Left = x; 
                thick.Top = y;
                return thick;
            }
            set
            {
                thick = value;
                OnPropertyChanged();
            }
        }
        private float radius;
        public float Radius
        {
            get
            {
                return radius;
            }
            set
            {
                radius = value;
                OnPropertyChanged();
            }
        }
        private Color color;
        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
                OnPropertyChanged();
            }
        }
        private string text;
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                OnPropertyChanged();
            }
        }

        private float fontSize;
        public float FontSize
        {
            get
            {
                return fontSize;
            }
            set
            {
                fontSize = value;
                OnPropertyChanged();
            }
        }

        private Color textColor;
        public Color TextColor
        {
            get
            {
                return textColor;
            }
            set
            {
                textColor = value;
                OnPropertyChanged();
            }
        }

    }
}
