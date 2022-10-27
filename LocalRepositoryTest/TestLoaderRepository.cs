using LocalRepositoryImpl.UTraveller.LocalRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Entity;

namespace LocalRepositoryTest.Test
{
    public class TestLoaderRepository : BaseRepository<PhotoEntity>
    {
        public TestLoaderRepository(LocalDatabase database) : base(database) { }

        public List<EventEntity> GetEvents()
        {
            var all = database.Events.Select(x => x);
            var events = new List<EventEntity>();
            foreach (var row in all)
            {
                events.Add(row);
            }

            return events;
        }

        public List<PhotoEntity> GetPhotos()
        {
            var all = database.Photos.Select(x => x);
            var photos = new List<PhotoEntity>();
            foreach (var row in all)
            {
                photos.Add(row);
            }
         
            return photos;
        }

      
        public override void Insert(PhotoEntity entity)
        {
            throw new NotImplementedException();
        }

        public override void Delete(PhotoEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
