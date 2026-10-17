using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;

public partial class kullaniciGirisi : System.Web.UI.Page
{
    DataSet dsKullanici = new DataSet();
    string randomSifre = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        //if (Request.QueryString["randomSifre"] == null)
        //{
        //    Response.Redirect("~/giris.aspx");
        //}
        //else
        //{

            if (Session["randomSifre"] != null && Session["randomSifre"].ToString() != "")
            {
                randomSifre = Session["randomSifre"].ToString();
            }

            if (Request.QueryString["uid"] != null && Request.QueryString["uid"].ToString() != "")
            {
                txtKullaniciIsmi.Text = Request.QueryString["uid"].ToString();
            }
           

        //}
    }

    protected void btnGiriş_Click(object sender, EventArgs e)
    {
        if (txtKullaniciIsmi.Text != "" && txtSifre.Text != "")
        {
            dsKullanici = SqlHelper.ExecuteDataset(conn.connection, CommandType.Text, "select * from kisiler where kullaniciAdi = '" + txtKullaniciIsmi.Text.Trim() + "' and sifre = '" + txtSifre.Text.Trim() + "'");

            if (dsKullanici != null && dsKullanici.Tables.Count > 0 && dsKullanici.Tables[0].Rows.Count > 0)
            {
                if (randomSifre != "")
                {
                    Session["randomSifre"] = randomSifre;

                    if (conn.connection.State != ConnectionState.Closed)
                    {
                        conn.connection.Close();
                    }

                    DataSet ds = new DataSet();

                    ds = SqlHelper.ExecuteDataset(conn.connection, CommandType.Text, "select count(*) as toplam from anaTablo where randomSifre = '" + randomSifre + "' and turID = 2");

                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0]["toplam"].ToString() != "0")
                    {
                        lblHata.Text = "";

                        if (Convert.ToInt32(ds.Tables[0].Rows[0]["toplam"]) > 0)
                        {
                            string url = "okul.aspx?x=" + EncryptionHelper.Encrypt(randomSifre, true);

                            string decryptUrl = EncryptionHelper.Decrypt(url.Replace("okul.aspx?x=", ""), true);

                            Session["kisiID"] = dsKullanici.Tables[0].Rows[0]["kisiID"].ToString();

                            Response.Redirect(url);
                        }
                    }
                    else
                    {
                        randomSifre = "";
                        //lblHata.Text = "<span style=\"color:Red;font-size:14pt;padding-top:10px;padding-bottom:10px;\">Geçersiz bir referans kodu girdiniz!</span>";
                    }

                }
            }
            else
            {
                lblHata.Text = "<script type=\"text/javascript\">alert('Lütfen kullanıcı isminizi ve şifrenizi kontrol ederek tekrar deneyiniz. Belirttiğiniz kullanıcı ismi ve şifreye ait bir kullanıcı bulunamamıştır!');</script>";
            }
        }
        else
        {
            lblHata.Text = "<script type=\"text/javascript\">alert('boş geçilemez!');</script>";
        }
    }

    protected void lnkBtnYeniUye_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/yeniKullanici.aspx");
    }
}