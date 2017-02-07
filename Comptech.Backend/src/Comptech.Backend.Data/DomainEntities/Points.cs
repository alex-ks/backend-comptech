using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Comptech.Backend.Data.DomainEntities
{
    public class Points
    {
        public Point TopLeft { get; set; }
        public Point BottomRight { get; set; }

        public Points() { }

        public Points(Point topLeft, Point bottomRight)
        {
            TopLeft = topLeft;
            BottomRight = bottomRight;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            Points pointsObj = obj as Points;
            return TopLeft.Equals(pointsObj.TopLeft) && BottomRight.Equals(pointsObj.BottomRight);
        }

        public override int GetHashCode()
        {
            return TopLeft.GetHashCode() ^ BottomRight.GetHashCode();
        }
    }

}
