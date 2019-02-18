using Microsoft.VisualStudio.TestTools.UnitTesting;
using mchomerEANProductFinder.Core.Services;
using System.Threading.Tasks;

namespace mchomerEANProductFinder.Test
{
    [TestClass]
    public class OpenEANDatabaseServiceTest
    {
        private OpenEANDatabaseService _openeandatabaseservice;

        public OpenEANDatabaseServiceTest()
        {
            this._openeandatabaseservice = new OpenEANDatabaseService();
        }

        [TestMethod]
        public async Task DoHttpPostRequestForEANProduct()
        {
            var result = await this._openeandatabaseservice.DoHttpPostRequestForEANProduct("4013595644210");
            Assert.AreNotEqual("", result);
        }

        [TestMethod]
        public async Task ConvertHTMLResponseStringToEANItem()
        {
            var htmlresponsestring = await this._openeandatabaseservice.DoHttpPostRequestForEANProduct("4013595644210");
            var eanitem = this._openeandatabaseservice.ConvertHTMLResponseStringToEANItem(htmlresponsestring, "4013595644210");
            Assert.AreNotEqual("", eanitem.EANBarCode);
            Assert.AreNotEqual("", eanitem.Name);
        }

        [TestMethod]
        public async Task FindItemByEANBarCode()
        {
            var eanitem = await this._openeandatabaseservice.FindItemByEANBarCode("5538838");
            Assert.AreNotEqual("", this._openeandatabaseservice.LastErrorOccured);
            eanitem = await this._openeandatabaseservice.FindItemByEANBarCode("4013595644210");
            Assert.AreEqual("", this._openeandatabaseservice.LastErrorOccured);
            Assert.AreNotEqual("", eanitem.EANBarCode);
            Assert.AreNotEqual("", eanitem.Name);
        }
    }
}
