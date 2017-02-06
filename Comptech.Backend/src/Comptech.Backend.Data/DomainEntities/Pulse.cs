using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comptech.Backend.Data.DomainEntities
{
    public class Pulse
    {
        public int SessionID { get; set; }
        public int BPM { get; set; }
        public DateTime TimeStamp { get; set; }

        public Pulse(int sessionID, int bpm, DateTime timeStamp)
        {
            SessionID = sessionID;
            BPM = bpm;
            TimeStamp = timeStamp;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            Pulse pulseObj = obj as Pulse;
            return SessionID.Equals(pulseObj.SessionID) && BPM.Equals(pulseObj.BPM)
                && TimeStamp.Equals(pulseObj.TimeStamp);
        }

        public override int GetHashCode()
        {
            return SessionID.GetHashCode() ^ BPM.GetHashCode() ^ TimeStamp.GetHashCode();
        }
    }

}
