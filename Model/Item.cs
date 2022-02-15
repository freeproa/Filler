using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Xml.Serialization;

namespace Fill.Model
{
    public class Item : IComparer<Item>
    {  
        public Rectangle Bound = new Rectangle();
        [XmlIgnore]
        public int Width { get => Bound.Width; set { Bound.Width = value; } }
        [XmlIgnore]
        public int Height { get => Bound.Height; set { Bound.Height = value; } }
        [XmlIgnore]
        public int X { get => Bound.X; set { Bound.X = value; } }
        [XmlIgnore]
        public int Y { get => (int)Bound.Y; set { Bound.Y = value; } }
        [XmlIgnore]
        public int Index { get; set; }
        public int PlaceIndex { get; set; }

        public Item() { }

        internal void Reset( int idx )
        {
            X = Y = 0;
            Index = idx;
        }

        public int Compare( Item x, Item y )
        {
            int max1 = Math.Max( x.Width, x.Height );
            int max2 = Math.Max( y.Width, y.Height );
            return max2 - max1;
        }
        public Item Clone() { return this.MemberwiseClone() as Item; }
        public override string ToString()
        {
            return $"{Index}) {X}:{Y} {Bound.Right}:{Bound.Bottom}";
        }
    }
}
