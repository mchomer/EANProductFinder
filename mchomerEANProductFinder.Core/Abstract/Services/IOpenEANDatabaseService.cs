using mchomerEANProductFinder.Core.Models;
using System.Threading.Tasks;

namespace mchomerEANProductFinder.Core.Abstract.Services
{
    public interface IOpenEANDatabaseService
    {
        Task<EANItem> FindItemByEANBarCode(string eanbarcode);
        Task<string> DoHttpPostRequestForEANProduct(string eanbarcode);
        EANItem ConvertHTMLResponseStringToEANItem(string htmlresponsestring, string eanbarcode);
    }
}
