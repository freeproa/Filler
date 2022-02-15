using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Xml.Serialization;

namespace Fill.Model
{
    public class Filler
    {
        public List<Item> ItemList = new List<Item>();
        public int Width;
        public int Height;
        internal bool IsStopped;
        Rectangle bound;
        internal int limit = 1000;
        internal int foundCounter;
        List<List<Item>> foundList = new List<List<Item>>();

        internal Action<string> FillerEvent { get; set; }
        internal Action<List<Item>> ResultEvent { get; set; }

        public Filler() { }

        internal void Run()
        {
            IsStopped = false;
            foundCounter = 0;
            foundList.Clear();

            bound = new Rectangle() { Width = Width, Height = Height };

            // обнуление позиций и установка индекса из DataGrid
            for (int i = 0; i < ItemList.Count; i++)
                ItemList[i].Reset( i + 1 );

            // сортировка - большие в начало
            ItemList.Sort( new Item() );

            // устновка индекса в порядке добавления на полотно
            for (int i = 0; i < ItemList.Count; i++)
                ItemList[i].PlaceIndex = i + 1;

            Task.Run( () =>
            {
                go( 0 );

                // выход
                if (foundCounter > 0)
                    FillerEvent?.Invoke( $"The solution is found." );
                else
                    FillerEvent?.Invoke( "Error: you can not fill the area with shapes" );

                // сообщение о выходе == null
                ResultEvent?.Invoke( null );
            } );
        }

        void go( int idx )
        {
            if (IsStopped)
                return;

            if (idx == ItemList.Count)
            {
                //OK!!!
                List<Item> result = ItemList.Select( i => i.Clone() ).ToList();
                // проверка на дубли
                if (equalFound( result ))
                    return;
                // нет дубля - запомнить и сообщить наверх
                foundList.Add( result );
                ResultEvent?.Invoke( result );
                foundCounter++;
                if (foundCounter >= limit)
                    IsStopped = true;
                return;
            }
            Item item = ItemList[idx];

            // перечисление возможных позиций
            foreach (Pos pos in generator( idx, item ))
            {
                item.Y = pos.Y;
                item.X = pos.X;
                // проверка на размещение
                if (tryplace( idx, item ))
                    go( idx + 1 );
            }
        }

        // возможно есть избыточность - это долго все надо тестировать
        IEnumerable<Pos> generator( int idx, Item item )
        {
            yield return new Pos();

            List<Item> slice = ItemList.GetRange( 0, idx ).OrderBy( i => i.Y ).ToList();

            foreach (Item prev in slice)
                yield return new Pos( prev.Bound.Right, prev.Y );

            slice = ItemList.OrderBy( i => i.Bound.Bottom ).ToList();

            foreach (Item prev in slice)
                yield return new Pos( prev.X, prev.Bound.Bottom );

            yield return new Pos( 0, Height - item.Height );

            yield return new Pos( Width - item.Width, Height - item.Height );

            slice = ItemList.OrderByDescending( i => i.Y ).ToList();

            foreach (Item prev in slice)
                yield return new Pos( prev.X, prev.Bound.Bottom );

            slice = ItemList.OrderByDescending( i => i.Bound.Bottom ).ToList();

            foreach (Item prev in slice)
                yield return new Pos( prev.X, prev.Bound.Bottom );
        }

        // проверка на пересечение с другими уже установленными
        bool tryplace( int idx, Item item )
        {
            if (bound.Contains( item.Bound ))
            {
                for (int i = 0; i < idx; i++)
                    if (ItemList[i].Bound.IntersectsWith( item.Bound ))
                        return false;
                return true;
            }
            return false;
        }

        bool equalFound( List<Item> list )
        {
            foreach (List<Item> found in foundList)
            {
                bool equal = true;
                for (int i = 0; i < list.Count; i++)
                {
                    if (found[i].X != list[i].X || found[i].Y != list[i].Y)
                    {
                        equal = false;
                        break;
                    }
                }
                if (equal)
                    return true;
            }
            return false;
        }
    }
    //-------------------------------------------
    struct Pos
    {
        public int Y, X;
        public Pos( int x, int y )
        {
            Y = y;
            X = x;
        }
        public override string ToString()
        {
            return $"{X} {Y}";
        }
    }
}
