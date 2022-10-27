using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using ServiceImpl.UTraveller.Service.Implementation;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Service.Implementation;

namespace LocalRepositoryTest
{
    [TestClass]
    public class GeoCoordinateServiceTest
    {
        [TestMethod]
        public void TestIsCoordinateCloseToEachOther()
        {
            var service = new GeoCoordinateService();
            var coordinate1 = new GeoCoordinate(0.001, 0.03);
            var coordinate2 = new GeoCoordinate(0, 0.29);

            Assert.IsTrue(service.IsNeighbours(coordinate1, coordinate2));
        }

        [TestMethod]
        public void TestIsCoordinateNotCloseToEachOther()
        {
            var service = new GeoCoordinateService();
            var coordinate1 = new GeoCoordinate(0.001, 0.03);
            var coordinate2 = new GeoCoordinate(0, 0.59);

            Assert.IsFalse(service.IsNeighbours(coordinate1, coordinate2));
        }
    }
}
