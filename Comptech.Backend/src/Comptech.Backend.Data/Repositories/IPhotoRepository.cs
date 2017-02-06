using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comptech.Backend.Data.DomainEntities;

namespace Comptech.Backend.Data.Repositories
{
    public interface IPhotoRepository : IRepository<Photo>
    {
        Photo GetLastPhotoInSession(int sessionId);
        int GetLastPhotoIdInSession(int sessionId);
        Photo GetPhotoById(int photoId);
    }

}
