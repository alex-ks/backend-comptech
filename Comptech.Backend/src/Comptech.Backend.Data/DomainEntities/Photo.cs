using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comptech.Backend.Data.DomainEntities
{
    public class Photo
    {
        public int PhotoID { get; set; }
        public int SessionID { get; set; }
        public byte[] Image { get; set; }
        public DateTime TimeStamp { get; set; }

        public Photo(int sessionID, byte[] image, DateTime timeStamp)
        {
            SessionID = sessionID;
            Image = image;
            TimeStamp = timeStamp;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            else
            {
                Photo photoObj = obj as Photo;
                return PhotoID.Equals(PhotoID) && SessionID.Equals(SessionID) && Image.Equals(Image) 
                    && TimeStamp.Equals(TimeStamp);
            }
        }

        public override int GetHashCode()
        {
            return PhotoID ^ SessionID  ^ TimeStamp.GetHashCode();
        }
    }
}
