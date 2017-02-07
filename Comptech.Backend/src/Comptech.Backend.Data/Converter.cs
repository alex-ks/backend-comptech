using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comptech.Backend.Data.DbEntities;
using Comptech.Backend.Data.DomainEntities;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Comptech.Backend.Data.Test")]

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
                ExpiresAt = entity.ExpiresAt,
                Status = entity.Status.ToString()
            };
            return session;
        }

        internal static DbResult ToDbEntity(this RecognitionResults entity)
        {
            if (entity.IsValid==true)
            {
                DbResult result = new DbResult
                {
                    IsValid = true,
                    X1 = entity.Coords.TopLeft.X,
                    X2 = entity.Coords.BottomRight.X,
                    Y1 = entity.Coords.TopLeft.Y,
                    Y2 = entity.Coords.BottomRight.Y,
                    PhotoId = entity.PhotoID
                };

                return result;
            }
            else
            {
                DbResult result = new DbResult
                {
                    IsValid = false,
                    X1 = null,
                    X2 = null,
                    Y1 = null,
                    Y2 = null,
                    PhotoId = entity.PhotoID
                };

                return result;
            }
        }

        internal static Photo ToDomainEntity(this DbPhoto entity)
        {
            Photo photo = new Photo
            {
                PhotoID = entity.PhotoId,
                SessionID = entity.SessionId,
                Image = entity.Image,
                TimeStamp = DateTime.SpecifyKind(entity.Timestamp, DateTimeKind.Utc)
            };
            return photo;
        }

        internal static Pulse ToDomainEntity(this DbPulse entity)
        {
            Pulse pulse = new Pulse
            {
                SessionID = entity.SessionId,
                BPM = entity.Bpm,
                TimeStamp = DateTime.SpecifyKind(entity.timestamp, DateTimeKind.Utc)
            };
            return pulse;
        }

        internal static Session ToDomainEntity(this DbSession entity)
        {
            Session session = new Session
            {
                SessionID = entity.SessionId,
                UserID = entity.UserId,
                Start = DateTime.SpecifyKind(entity.Start, DateTimeKind.Utc),
                ExpiresAt = DateTime.SpecifyKind(entity.ExpiresAt, DateTimeKind.Utc),
                Status = (SessionStatus)Enum.Parse(typeof(SessionStatus),entity.Status)
            };
            return session;
        }

        internal static RecognitionResults ToDomainEntity(this DbResult entity)
        {
            if (entity.IsValid == true)
            {
                RecognitionResults recognitionResult = new RecognitionResults
                {
                    IsValid = true,
                    Coords = new Points
                    {
                        TopLeft = new Point { X = entity.X1.Value, Y = entity.Y1.Value },
                        BottomRight = new Point { X = entity.X2.Value, Y = entity.Y2.Value }
                    },
                    PhotoID = entity.PhotoId

                };
                return recognitionResult;
            }
            else
            {

                RecognitionResults recognitionResult = new RecognitionResults
                {
                    IsValid = false,
                    PhotoID = entity.PhotoId
                };
                return recognitionResult;
            }
        }
    }
}
