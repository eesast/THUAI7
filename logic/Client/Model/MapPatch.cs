using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Model
{
    public enum MapPatchType
    {
        Ground = 0,
        RedHome = 1,
        BlueHome = 2,
        Ruin = 3,
        Grass = 4,
        River = 5,
        Garbage = 6,
        RecycleBank = 7,
        ChargeStation = 8,
        SignalTower = 9,
        WormHole = 10,
        Null = 11
    };

    public class MapPatch : BindableObject
    {
        private Color patchColor;
        private Color textColor;
        private int x;
        private int y;
        private int unitWidth;
        private int unitHeight;
        private string text;
        private MapPatchType type;
        public Color PatchColor
        {
            get => patchColor;
            set
            {
                patchColor = value;
                OnPropertyChanged();
            }
        }

        public Color TextColor
        {
            get => textColor;
            set
            {
                textColor = value;
                OnPropertyChanged();
            }
        }
        public int X
        {
            get => x;
            set
            {
                x = value;
                OnPropertyChanged();
            }
        }
        public int Y
        {
            get => y;
            set
            {
                y = value;
                OnPropertyChanged();
            }
        }
        public int UnitWidth
        {
            get => unitWidth;
            set
            {
                unitWidth = value;
                OnPropertyChanged();
            }
        }
        public int UnitHeight
        {
            get => unitHeight;
            set
            {
                unitHeight = value;
                OnPropertyChanged();
            }
        }
        public string Text
        {
            get => text;
            set
            {
                text = value;
                OnPropertyChanged();
            }
        }
        public MapPatchType Type
        {
            get => type;
            set
            {
                type = value;
                OnPropertyChanged();
            }
        }
    }
    //{
    //    private Color patchColor;
    //    private int x;
    //    private int y;
    //    private int unitWidth;
    //    private int unitHeight;
    //    private string text;
    //    private MapPatchType type;
    //    public Color PatchColor
    //    {
    //        get => patchColor;
    //        set => patchColor = value;
    //    }
    //    public int X
    //    {
    //        get => x;
    //        set => x = value;
    //    }
    //    public int Y
    //    {
    //        get => y;
    //        set => y = value;
    //    }
    //    public int UnitWidth
    //    {
    //        get => unitWidth;
    //        set => unitWidth = value;
    //    }
    //    public int UnitHeight
    //    {
    //        get => unitHeight;
    //        set => unitHeight = value;
    //    }
    //    public string Text
    //    {
    //        get => text;
    //        set => text = value;
    //    }
    //    public MapPatchType Type
    //    {
    //        get => type;
    //        set => type = value;
    //    }
    //}

    public class TestClass : BindableObject
    {
        private String name;
        public String Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }
        private int age;
        public int Age
        {
            get => age;
            set
            {
                age = value;
                OnPropertyChanged();
            }
        }
    }
}
