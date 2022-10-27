using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using LocalRepositoryImpl.UTraveller.LocalRepository;
using UTravellerModel.UTraveller.Model;
using System.Linq;
using System.Diagnostics;
using LocalRepositoryImpl.UTraveller.LocalRepository.Implementation;
using System.Data.Linq;
using System.Windows.Resources;
using System.Windows;
using System.IO;
using UTravellerModel.UTraveller.Entity;

namespace LocalRepositoryTest
{
    [TestClass]
    public class PhotoRepositoryTest
    {
        [TestMethod]
        public void TestInsertEvent()
        {
            using (var localDatabase = new LocalDatabase("Data Source=isostore:/eventsDb.sdf"))
            {
                if (localDatabase.DatabaseExists() == false)
                {
                    localDatabase.CreateDatabase();
                    var event2 = new EventEntity { Name = "Event2" };

                    var repository = new EventRepository(localDatabase);
                    repository.Insert(new EventEntity { Name = "Event1" });
                    repository.Insert(event2);

                    event2.Name = "new name";
                    repository.Update(event2);
                    //localDatabase.SubmitChanges();
                }

                var result = from EventEntity e in localDatabase.Events select e;
                foreach (var e in result)
                {
                    Debug.WriteLine(e.Name);
                }
            }
        }

        [TestMethod]
        public void TestInsertPhoto()
        {
            using (var localDatabase = new LocalDatabase("Data Source=isostore:/eventsDb.sdf"))
            {
                if (localDatabase.DatabaseExists() == false)
                {
                    localDatabase.CreateDatabase();
                }

                byte[] imgBytes = null;
                Uri uri = new Uri("Resource/TempImage/OnePixelImg.bmp", UriKind.Relative);
                StreamResourceInfo resource = Application.GetResourceStream(uri);
                if (resource != null)
                {
                    using (Stream stream = resource.Stream)
                    {
                        imgBytes = GetBytes(stream);
                    }
                }

                PhotoEntity photo = new PhotoEntity();
                photo.Name = "p1";
                photo.Date = DateTime.Now;
                photo.Thumbnail = imgBytes;
                photo.ImageUrl = "imgBytes";

                var photoRepository = new PhotoRepository(localDatabase);
                photoRepository.Insert(photo);

                foreach (var p in photoRepository.GetPhotosOfEvent(1))
                {
                    Assert.AreEqual(photo.Name, p.Name);
                }
            }
        }

        [TestMethod]
        public void TestInsertPhotosOfEvent()
        {
            using (var localDatabase = new LocalDatabase("Data Source=isostore:/eventsDb.sdf"))
            {
                if (localDatabase.DatabaseExists() == false)
                {
                    localDatabase.CreateDatabase();
                }

                var eventRepository = new EventRepository(localDatabase);
                var photoRepository = new PhotoRepository(localDatabase);

                var event1 = new EventEntity { Name = "Event1" };
                var event2 = new EventEntity { Name = "Event2" };
                eventRepository.Insert(event1);
                eventRepository.Insert(event2);

                PhotoEntity photo1 = new PhotoEntity();
                photo1.Name = "p1";
                photo1.Date = DateTime.Now;
                photo1.EventId = event1.Id;

                PhotoEntity photo2 = new PhotoEntity();
                photo2.Name = "p2";
                photo2.Date = DateTime.Now;
                photo2.EventId = event2.Id;

                photoRepository.Insert(photo1);
                photoRepository.Insert(photo2);

                var photosOfEvent1 = photoRepository.GetPhotosOfEvent(event1.Id).Count();
                var photosOfEvent2 = photoRepository.GetPhotosOfEvent(event2.Id).Count();

                Assert.AreEqual(1, photosOfEvent1);
                Assert.AreEqual(1, photosOfEvent2);

                photoRepository.Delete(photo1);
                photoRepository.Delete(photo2);
                Assert.AreEqual(0, photoRepository.GetPhotosOfEvent(event2.Id).Count());
            }
        }

       
        [TestMethod]
        public void TestInsertMoneySpendingInEvent()
        {
            using (var localDatabase = new LocalDatabase("Data Source=isostore:/eventsDb.sdf"))
            {
                if (localDatabase.DatabaseExists() == false)
                {
                    localDatabase.CreateDatabase();
                }

                var eventRepository = new EventRepository(localDatabase);
                var moneySpendingRepository = new MoneySpendingRepository(localDatabase);

                var event1 = new EventEntity { Name = "Event1" };
                eventRepository.Insert(event1);

                MoneySpendingEntity entity = new MoneySpendingEntity();
                entity.EventId = event1.Id;
                entity.Currency = CurrencyType.Euro;
                entity.MoneySpendingCategory = MoneySpendingCategory.Camping;
                entity.Amount = 100;
                entity.Description = "description";
                entity.Date = DateTime.Now;

                moneySpendingRepository.Insert(entity);

                var moneySpendings = moneySpendingRepository.GetMoneySpendingsForEvent(event1.Id).ToList();

                moneySpendingRepository.Delete(entity);
                Assert.AreEqual(0, moneySpendingRepository.GetMoneySpendingsForEvent(event1.Id).Count());
            }

        }

        [TestMethod]
        public void TestInsertRouteInEvent()
        {
            using (var localDatabase = new LocalDatabase("Data Source=isostore:/eventsDb.sdf"))
            {
                if (localDatabase.DatabaseExists() == false)
                {
                    localDatabase.CreateDatabase();
                }

                var eventRepository = new EventRepository(localDatabase);
                var routeRepository = new RouteRepository(localDatabase);

                var event1 = new EventEntity { Name = "Event1" };
                eventRepository.Insert(event1);

                RouteEntity route = new RouteEntity();
                route.Coordinates = "123.42;32.421 3.11;65";
                route.Event = event1;

                routeRepository.Insert(route);

                var photosOfEvent1 = routeRepository.GetRoutesOfEvent(event1.Id).Count();

                Assert.AreEqual(1, photosOfEvent1);

                routeRepository.Delete(route);
                Assert.AreEqual(0, routeRepository.GetRoutesOfEvent(event1.Id).Count());
            }
        }

        [TestMethod]
        public void TestInsertPhotoPost()
        {
            using (var localDatabase = new LocalDatabase("Data Source=isostore:/eventsDb.sdf"))
            {
                if (localDatabase.DatabaseExists() == false)
                {
                    localDatabase.CreateDatabase();
                }

                var eventRepository = new FacebookPhotoPostRepository(localDatabase);
                var entity = new FacebookPhotoPostEntity();
                entity.Id = "124325437654_3539237429602342";
                entity.PhotoId = 1;
                entity.AlbumId = "2455211423416232";
                entity.Date = DateTime.Now;

                eventRepository.Insert(entity);   
            }
        }

        public static byte[] GetBytes(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
