using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comptech.Backend.Data.DomainEntities;

namespace Comptech.Backend.Data.Repositories
{
    public interface IRecognitionResultsRepository : IRepository<RecognitionResults>
    {
        RecognitionResults GetRecognitionResultsByPhotoId(int photoId);
    }
}
