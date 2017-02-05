using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comptech.Backend.Data.DomainEntities;

namespace Comptech.Backend.Data.Repositories.Psql
{
    public class PsqlRecognitionResultsRepository : IRecognitionResultsRepository
    {
        public bool Add(RecognitionResult entity)
        {
            throw new NotImplementedException();
        }

        public RecognitionResult GetRecognitionResultsByPhotoId(int photoId)
        {
            throw new NotImplementedException();
        }

        public bool Update(RecognitionResult entity)
        {
            throw new NotImplementedException();
        }
    }
}
