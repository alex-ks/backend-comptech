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
    }

}
