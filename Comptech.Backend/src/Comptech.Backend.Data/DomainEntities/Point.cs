using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comptech.Backend.Data.DomainEntities
{
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            else
            {
                Point pointObj = obj as Point;
                return X.Equals(pointObj.X) && Y.Equals(pointObj.Y);
            }
        }

        public override int GetHashCode()
        {
            return X ^ Y;
        }
    }
}
