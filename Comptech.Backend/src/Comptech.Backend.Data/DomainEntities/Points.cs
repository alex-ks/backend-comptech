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

        public Points(Point topLeft, Point bottomRight)
        {
            TopLeft = topLeft;
            BottomRight = bottomRight;
        }
    }
    
}
