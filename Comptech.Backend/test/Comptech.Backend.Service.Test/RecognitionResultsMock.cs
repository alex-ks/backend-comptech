using Comptech.Backend.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comptech.Backend.Data.DomainEntities;

namespace Comptech.Backend.Service.Test
{
    public class RecognitionResultsMock : IRecognitionResultsRepository
    {
        private List<RecognitionResults> rep = new List<RecognitionResults>();
        public bool Add(RecognitionResults entity)
        {
            rep.Add(entity);
            return true;
        }

        public RecognitionResults GetRecognitionResultsByPhotoId(int photoId)
        {
            return rep.Last();
        }

        public bool Update(RecognitionResults entity)
        {
            throw new NotImplementedException();
        }
    }
}
