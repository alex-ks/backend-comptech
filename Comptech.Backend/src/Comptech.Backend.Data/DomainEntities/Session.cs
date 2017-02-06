using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comptech.Backend.Data.DomainEntities
{
    public class Session
    {
        public int SessionID { get; set; }
        public int UserID { get; set; }
        public DateTime Start { get; set; }
        public DateTime ExpiresAt { get; set; }
        public SessionStatus Status { get; set; }

        public Session(int userID, DateTime start, DateTime expiresAt, SessionStatus status)
        {
            UserID = userID;
            Start = start;
            ExpiresAt = expiresAt;
            Status = status;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            Session recognitionResultsObj = obj as Session;
            return UserID.Equals(recognitionResultsObj.UserID) && Start.Equals(recognitionResultsObj.Start)
                && ExpiresAt.Equals(ExpiresAt) && Status.Equals(Status);
        }

        public override int GetHashCode()
        {
            return UserID ^ Start.GetHashCode() ^ ExpiresAt.GetHashCode() ^ Status.GetHashCode();
        }
    }

}
