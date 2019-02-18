using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using mchomerEANProductFinder.Core.Abstract.Services;
using mchomerEANProductFinder.Core.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace mchomerEANProductFinder.Core.Services
{
    public class OpenEANDatabaseService : IOpenEANDatabaseService
    {
        public string EANQueryBaseAddress { get; set; }
        public string FormCMD { get; set; }
        public string FormSID { get; set; }
        public string LastErrorOccured { get; set; }

        public OpenEANDatabaseService()
        {
            this.EANQueryBaseAddress = "https://opengtindb.org/index.php";
            this.FormCMD = "ean1";
            this.FormSID = "";
        }

        public OpenEANDatabaseService(string EANQueryBaseAddress, string FormCMD, string FormSID)
        {
            this.EANQueryBaseAddress = EANQueryBaseAddress;
            this.FormCMD = FormCMD;
            this.FormSID = FormSID;
        }

        public virtual async Task<EANItem> FindItemByEANBarCode(string eanbarcode)
        {
            var eanitemhtml = await this.DoHttpPostRequestForEANProduct(eanbarcode);
            var eanitem = new EANItem();
            if (!string.IsNullOrEmpty(eanitemhtml))
            {
                eanitem = this.ConvertHTMLResponseStringToEANItem(eanitemhtml, eanbarcode); 
            }
            return eanitem;
        }

        public virtual async Task<string> DoHttpPostRequestForEANProduct(string eanbarcode)
        {
            var htmlresponse = string.Empty;
            this.LastErrorOccured = string.Empty;
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
                    httpclient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
                    httpclient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", "ISO-8859-1");
                    var postdata = new StringContent($"cmd={this.FormCMD}&SID={this.FormSID}&ean={eanbarcode}");
                    postdata.Headers.ContentType.MediaType = "application/x-www-form-urlencoded";
                    var httpresponse = await httpclient.PostAsync(this.EANQueryBaseAddress, postdata);
                    var bytes = await httpresponse.Content.ReadAsByteArrayAsync();
                    htmlresponse = Encoding.GetEncoding("iso-8859-1").GetString(bytes);
                    if (WebUtility.HtmlDecode(htmlresponse).Contains("Fehler: Ungültige Nummer eingegeben!"))
                    {
                        this.LastErrorOccured = "Ungültige Nummer eingegeben!";
                    }
                    else if (WebUtility.HtmlDecode(htmlresponse).Contains("Fehler: Ungültige EAN/GTIN eingegeben (7)!"))
                    {
                        this.LastErrorOccured = "Ungültige EAN/GTIN eingegeben (7)!";
                    }
                    else if (WebUtility.HtmlDecode(htmlresponse).Contains("Fehler: Diese EAN/GTIN ist derzeit noch nicht bekannt."))
                    {
                        this.LastErrorOccured = "Diese EAN/GTIN ist derzeit noch nicht bekannt.";
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message, ex.Source);                
            }
            if (!string.IsNullOrEmpty(this.LastErrorOccured))
            {
                htmlresponse = string.Empty;
            }
            return htmlresponse;
        }

        public virtual EANItem ConvertHTMLResponseStringToEANItem(string htmlresponsestring, string eanbarcode)
        {
            var htmldocument = new HtmlDocument();
            var eanitem = new EANItem();
            try
            {
                htmldocument.LoadHtml(htmlresponsestring);
                var document = htmldocument.DocumentNode;
                var tables = document.QuerySelectorAll("table").ToArray();
                var itemtable = tables[4];
                var categories = itemtable.QuerySelectorAll("td>font").ToArray();
                var attributes = itemtable.QuerySelectorAll("td").ToArray();
                eanitem.MainCategory = WebUtility.HtmlDecode(categories[0].InnerText.Trim());
                eanitem.SubCategory = WebUtility.HtmlDecode(categories[1].InnerText.Trim());
                eanitem.Name = WebUtility.HtmlDecode(attributes[5].InnerText.Trim());
                eanitem.NameInDetail = WebUtility.HtmlDecode(attributes[7].InnerText.Trim());
                eanitem.Description = WebUtility.HtmlDecode(attributes[9].InnerText.Trim());
                eanitem.Producer = WebUtility.HtmlDecode(attributes[11].InnerText.Trim());
                eanitem.Origin = WebUtility.HtmlDecode(attributes[17].InnerText.Trim());
                eanitem.Validation = WebUtility.HtmlDecode(attributes[19].InnerText.Trim());
                eanitem.Ingredient = WebUtility.HtmlDecode(attributes[21].InnerText.Trim());
                eanitem.Packing = WebUtility.HtmlDecode(attributes[23].InnerText.Trim());
                eanitem.EANBarCode = eanbarcode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message, ex.Source);
            }
            return eanitem;
        }
    }
}
