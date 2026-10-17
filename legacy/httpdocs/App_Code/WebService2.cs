using System;
using System.Collections.Generic;
//using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
/// <summary>
/// Summary description for WebService2
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[ScriptService]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class WebService2 : System.Web.Services.WebService {

    public WebService2 () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string sonucGetir2(string pSecilenResim)
    {
        if (pSecilenResim != "")
        {
            string[] gelenBilgiler = pSecilenResim.Split('~');

            string kisiID = gelenBilgiler[0].ToString();
            string resimID = gelenBilgiler[1].ToString();
            string adet15x23 = gelenBilgiler[2].ToString();
            string adet20x30 = gelenBilgiler[3].ToString();
            string adet50x70 = gelenBilgiler[4].ToString();



        }

        return pSecilenResim + " isimli resim alındı ve veritabanına kaydedildi!";
    }
    
}
