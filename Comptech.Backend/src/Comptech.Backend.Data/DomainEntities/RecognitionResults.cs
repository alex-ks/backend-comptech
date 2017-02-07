using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comptech.Backend.Data.DomainEntities
{
    public class RecognitionResults
    {
        public bool IsValid { get; set; }
        public Points Coords { get; set; }
        public int PhotoID { get; set; }

        public RecognitionResults() { }

        public RecognitionResults(bool isValid, Points coords, int photoID)
        {
            IsValid = isValid;
            Coords = coords;
            PhotoID = photoID;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            RecognitionResults recognitionResultsObj = obj as RecognitionResults;
            return IsValid.Equals(recognitionResultsObj.IsValid) && Coords.Equals(recognitionResultsObj.Coords)
                && PhotoID.Equals(PhotoID);
        }

        public override int GetHashCode()
        {
            return IsValid.GetHashCode() ^ Coords.GetHashCode() ^ PhotoID;
        }
    }

}
