using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

public partial class giris : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btnGiris_Click(object sender, EventArgs e)
    {
        if (txtRandomSifre.Text != "")
        {
            DataSet dsOkulIsmi = new DataSet();

            //gelenRandomSifre = "18968";

            try
            {
                //dsOkulIsmi = SqlHelper.ExecuteDataset(conn.connection, CommandType.Text, @"select birimAdi from anaTablo where randomSifre = " + txtRandomSifre.Text.Trim() + " and turID = 1");
                dsOkulIsmi.ReadXml(Server.MapPath("referanslar.xml"));


                if (dsOkulIsmi != null && dsOkulIsmi.Tables.Count > 0 && dsOkulIsmi.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsOkulIsmi.Tables[0].Rows)
                    {
                        if (txtRandomSifre.Text == dr[0].ToString())
                        {
                            Response.Redirect(dr[1].ToString());
                        }
                    }

                    //Session["randomSifre"] = txtRandomSifre.Text.Trim();
                    //Response.Redirect("kullaniciGirisi.aspx", true);
                }
                else
                {
                    lblHataMesaji.Text = "<script type=\"text/javascript\">alert('Girmiş olduğunuz " + txtRandomSifre.Text.Trim() + " geçerli bir Referans No değildir! Giriş yapabilmek için lütfen 0 532 416 33 43 nolu telefonla irtibat kurarak geçerli bir Referans No edinip tekrar deneyiniz.')</script>";
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}