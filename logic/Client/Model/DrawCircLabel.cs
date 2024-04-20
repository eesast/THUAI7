using Client.Util;
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
                if (value == x) return;
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
                if (value == y) return;
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
                thick.Left = (y - 0.5) * UtilInfo.unitWidth >= 0 ? (y - 0.5) * UtilInfo.unitWidth : 0;
                thick.Top = (x - 0.5) * UtilInfo.unitHeight >= 0 ? (x - 0.5) * UtilInfo.unitHeight : 0;
                return thick;
            }
            set
            {
                if (value == thick) return;
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
                if (value == radius) return;
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
                if (value == color) return;
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
                if (value == text) return;
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
                if (value == fontSize) return;
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
                if (value == textColor) return;
                textColor = value;
                OnPropertyChanged();
            }
        }

    }
}
