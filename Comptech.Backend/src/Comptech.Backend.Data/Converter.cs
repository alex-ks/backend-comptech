using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comptech.Backend.Data.DbEntities;
using Comptech.Backend.Data.DomainEntities;

namespace Comptech.Backend.Data
{
    static class Converter
    {
        internal static DbPhoto ToDbEntity(this Photo entity)
        {
            DbPhoto photo = new DbPhoto
            {
                PhotoId = entity.PhotoID,
                SessionId=entity.SessionID,
                Image=entity.Image,
                Timestamp=entity.TimeStamp
            };
            return photo;
        }

        internal static DbPulse ToDbEntity(this Pulse entity)
        {
            DbPulse pulse = new DbPulse
            {
                SessionId=entity.SessionID,
                Bpm=entity.BPM,
                timestamp=entity.TimeStamp
            };
            return pulse;
        }

        internal static DbSession ToDbEntity(this Session entity)
        {
            DbSession session = new DbSession
            {
                SessionId = entity.SessionID,
                UserId = entity.UserID,
                Start = entity.Start,
                Status = entity.Status.ToString()
            };
            return session;
        }
        internal static DbResult ToDbEntity(this RecognitionResult entity)
        {
            DbResult result = new DbResult
            {
                IsValid=entity.IsValid,
                X1=entity.Coords.TopLeft.X,
                X2=entity.Coords.BottomRight.X,
                Y1=entity.Coords.TopLeft.Y,
                Y2=entity.Coords.BottomRight.Y,
                PhotoId=entity.PhotoID
            };
            return result;
        }
        internal static Photo ToDomainEntity(this DbPhoto entity)
        {
            Photo photo = new Photo
            {
                PhotoID=entity.PhotoId,
                SessionID=entity.SessionId,
                Image=entity.Image,
                TimeStamp=entity.Timestamp
            };
            return photo;
        }
        internal static Pulse ToDomainEntity(this DbPulse entity)
        {
            Pulse pulse = new Pulse
            {
                SessionID=entity.SessionId,
                BPM=entity.Bpm,
                TimeStamp=entity.timestamp
            };
            return pulse;
        }
        internal static Session ToDomainEntity(this DbSession entity)
        {
            Session session = new Session
            {
                SessionID = entity.SessionId,
                UserID = entity.UserId,
                Start = entity.Start,
                Status = Enum.Parse(,entity.Status)
            };
        }
    internal static RecognitionResult ToDomainEntity(this DbResult entity)
        {
            throw new NotImplementedException();
        }
    }
}
