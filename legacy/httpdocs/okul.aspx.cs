using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using System.Web.UI.HtmlControls;



public partial class okul : System.Web.UI.Page
{
    ImageButton imgTemp = null;
    Button btn = null;

    protected void Page_Load(object sender, EventArgs e)
    {
        string gelenRandomSifre = "";
        string kisiID = "";

        if (!IsPostBack)
        {
            StringBuilder sb = new StringBuilder();

            if (Request.QueryString["x"] != null && Session["kisiID"] != null)
            {
                gelenRandomSifre = Request.QueryString["x"].ToString();

                kisiID = Session["kisiID"].ToString();

                gelenRandomSifre = EncryptionHelper.Decrypt(gelenRandomSifre.Replace("okul.aspx?x=", ""), true);

                if (gelenRandomSifre != "")
                {

                    DataSet dsOkulIsmi = new DataSet();

                    //gelenRandomSifre = "18968";

                    try
                    {
                        dsOkulIsmi = SqlHelper.ExecuteDataset(conn.connection, CommandType.Text, @"select birimAdi from anaTablo where randomSifre = " + gelenRandomSifre + " and turID = 1");
                    }
                    catch (Exception ex)
                    {
                    }

                    if (dsOkulIsmi != null && dsOkulIsmi.Tables.Count > 0 && dsOkulIsmi.Tables[0].Rows.Count > 0)
                    {
                        lblOkulIsmi.Text = dsOkulIsmi.Tables[0].Rows[0]["birimAdi"].ToString();
                    }

                    DataSet ds = new DataSet();

                    try
                    {
                        ds = SqlHelper.ExecuteDataset(conn.connection, CommandType.Text, @"SELECT
                    a.ID,a.ustID,a.turID,a.resimID,a.randomSifre,r.aciklama,r.resimPath,r.orjinalResimPath
                    FROM dbo.anaTablo a, dbo.resimler r
                    where a.resimID = r.resimID 
                    and a.randomSifre = r.randomSifre
                    and a.turID = 2
                    and a.aktif = 1
                    and a.uploaded = 1
                    and a.randomSifre = " + gelenRandomSifre + @"
                    ");
                    }
                    catch (Exception ex)
                    {
                    }

                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        //dlstResimler.DataSource = ds.Tables[0];
                        //dlstResimler.DataBind();
                        sb.AppendLine("<div id=\"kisiID\" kisiIDBilgisi=\"" + kisiID + "\" style=\"visibility:hidden;display:none;\"></div>");


                       

                        sb.AppendLine("<div class=\"portfolio_row\">");

                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            string resimPath = ds.Tables[0].Rows[i]["resimPath"].ToString();

                            switch ((i + 1) % 3)
                            {
                                case 1:
                                    {
                                        sb.AppendLine("<div id=\"div1\" runat=\"server\" class=\"grid_4 alpha\">");
                                        break;
                                    }

                                case 2:
                                    {
                                        sb.AppendLine("<div id=\"div2\" runat=\"server\" class=\"grid_4\">");
                                        break;
                                    }

                                case 0:
                                    {
                                        sb.AppendLine("<div id=\"div3\" runat=\"server\" class=\"grid_4 omega\">");
                                        break;
                                    }
                            }

                            #region yedek
                            //imgTemp = new ImageButton();
                            //imgTemp.ImageUrl = resimPath;
                            //imgTemp.Attributes.Add("href", resimPath);
                            //imgTemp.Attributes.Add("data-gal", "prettyPhoto[1]");
                            //imgTemp.Attributes.Add("class", "img_wrap2 lightbox-image");
                            //imgTemp.ToolTip = resimPath;

                            //imgTemp.Click += new ImageClickEventHandler(imgTemp_Click);

                            //btn = new Button();
                            //btn.Text = "Sepete Ekle " + ds.Tables[0].Rows[i]["resimID"].ToString();
                            //btn.Click += new EventHandler(btn_Click);



                            //div1.Controls.Add(imgTemp);
                            //div1.Controls.Add(btn);

                            //sb.AppendLine("<input type=\"image\" name=\"ImageButton1\" id=\"ImageButton1\" src=\"" + resimPath + "\" style=\"width:120px;border-width:0px;\" data-gal=\"prettyPhoto[1]\" href=\"" + resimPath + "\" class=\"img_wrap2 lightbox-image\" />"); 
                            #endregion

                            sb.AppendLine("<a data-gal=\"prettyPhoto[1]\" href=\"" + resimPath + "\" class=\"img_wrap2 lightbox-image\" resimDegeri=\"" + ds.Tables[0].Rows[i]["orjinalResimPath"].ToString() + "\">");
                            sb.AppendLine("<img src=\"" + resimPath + "\" alt=\"\" style=\"width:240px;\" resimNo=\"" + ds.Tables[0].Rows[i]["orjinalResimPath"].ToString() + "\" />");
                            sb.AppendLine("</a>");
                            sb.AppendLine("<br/>");
                            sb.AppendLine("Resim No : <h4>" + ds.Tables[0].Rows[i]["orjinalResimPath"].ToString() + "</h4>");
                            sb.AppendLine("<div class=\"sepeteEkle\" style=\"width:230px;padding:10px;background-color:Orange;border:solid White 2px;color:Black;text-align:center;cursor:pointer;font-weight:bold;\" deger=\"" + ds.Tables[0].Rows[i]["orjinalResimPath"].ToString() + "\">SEPETE EKLE</div>");
                            sb.AppendLine("</div>");

                            if ((i + 1) % 3 == 0)
                            {
                                sb.AppendLine("</div>");
                                sb.AppendLine("<div style\"padding-top:5px;\">&nbsp;</div>");
                                sb.AppendLine("<div class=\"portfolio_row\">");
                            }
                        }

                        if (ds.Tables[0].Rows.Count % 3 != 0)
                        {
                            sb.AppendLine("</div>");
                        }

                        lblIcerik.Text = sb.ToString();
                    }
                }
            }
        }
    }

    protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
    {

    }


    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        foreach (Control ctrl in ((LinkButton)sender).Controls)
        {
            if (ctrl.ID == "tempImage")
            { 
            
            }
        }
    }

    
}