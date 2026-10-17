using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;

public partial class yeniKullanici : System.Web.UI.Page
{
    DataSet dsYakinlikDerecesi = new DataSet();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["randomSifre"] == null)
        {
            Response.Redirect("~/giris.aspx");
        }
        else
        {
            if (!IsPostBack)
            {
                dsYakinlikDerecesi = SqlHelper.ExecuteDataset(conn.connection, CommandType.Text, "select * from yakinlik where aktif=1");

                if (dsYakinlikDerecesi != null && dsYakinlikDerecesi.Tables.Count > 0 && dsYakinlikDerecesi.Tables[0].Rows.Count > 0)
                {
                    drpYakinlikDerecesi.DataSource = dsYakinlikDerecesi.Tables[0];
                    drpYakinlikDerecesi.DataBind();

                    drpYakinlikDerecesi.Items[0].Selected = true;
                }
            }
        }
    }

    void alanlarıTemizle()
    {
        txtKullaniciIsmi.Text = "";
        txtSifre.Text = "";
        txtOgrenciAdi.Text = "";
        txtOgrenciSoyadi.Text = "";
        
        drpYakinlikDerecesi.Items[0].Selected = true;
        
        txtVeliAdi.Text = "";
        txtVeliSoyadi.Text = "";       
        txtKargoAdresi.Text = "";
        txtCep.Text = "";
    }

    protected void btnKaydet_Click(object sender, EventArgs e)
    {
        try
        {
            //Zorunlu alanlar...
            if (
                    (
                        txtKullaniciIsmi.Text != ""
                        && txtSifre.Text != ""
                        && txtSifreTekrar.Text != ""
                        && txtOgrenciAdi.Text != ""
                        && txtOgrenciSoyadi.Text != ""
                        && txtVeliAdi.Text != ""
                        && txtVeliSoyadi.Text != ""
                    )
                    &&
                    (txtSifre.Text.Trim() == txtSifreTekrar.Text.Trim())
               )
            {
                string insertSorgu = @" insert dbo.kisiler
                                    (	
	                                    [kullaniciAdi], 
	                                    [sifre], 
	                                    [ogrenciAdi], 
	                                    [ogrenciSoyadi], 
	                                    [yakinlikID], 
	                                    [veliAdi], 
	                                    [veliSoyadi], 
                                        [cep],
                                        [email],	
                                        [kargoAdresi]
                                    )
                                    values
                                    (
	                                    @kullaniciAdi, 
	                                    @sifre, 
	                                    @ogrenciAdi, 
	                                    @ogrenciSoyadi, 
	                                    @yakinlikID, 
	                                    @veliAdi, 
	                                    @veliSoyadi, 
                                        @cep,
                                        @email,
                                        @kargoAdresi
                                    )";

                SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@kullaniciAdi",SqlDbType.NVarChar),
                new SqlParameter("@sifre",SqlDbType.NVarChar),
                new SqlParameter("@ogrenciAdi",SqlDbType.NVarChar),
                new SqlParameter("@ogrenciSoyadi",SqlDbType.NVarChar),
                new SqlParameter("@yakinlikID",SqlDbType.BigInt),
                new SqlParameter("@veliAdi",SqlDbType.NVarChar),
                new SqlParameter("@veliSoyadi",SqlDbType.NVarChar),    
                new SqlParameter("@cep",SqlDbType.NVarChar),                 
                new SqlParameter("@email",SqlDbType.NVarChar),
                new SqlParameter("@kargoAdresi",SqlDbType.NVarChar)
            };

                parameters[0].Value = txtKullaniciIsmi.Text.Trim();
                parameters[1].Value = txtSifre.Text.Trim();
                parameters[2].Value = txtOgrenciAdi.Text.Trim();
                parameters[3].Value = txtOgrenciSoyadi.Text.Trim();

                if (drpYakinlikDerecesi.SelectedValue != "")
                {
                    parameters[4].Value = Convert.ToInt64(drpYakinlikDerecesi.SelectedValue);
                }

                parameters[5].Value = txtVeliAdi.Text.Trim();
                parameters[6].Value = txtVeliSoyadi.Text.Trim();
                parameters[9].Value = txtCep.Text.Trim();
                parameters[7].Value = txtEmail.Text.Trim();
                parameters[8].Value = txtKargoAdresi.Text.Trim();

                int insertIslemiSonucu = SqlHelper.ExecuteNonQuery(conn.connection, CommandType.Text, insertSorgu, parameters);

                Response.Redirect("~/kullaniciGirisi.aspx?uid=" + txtKullaniciIsmi.Text.Trim());
            }
            else
            { 
                //EKSİK VERİ GİRİŞİ                
            }
        }
        catch (Exception ex)
        {
        }
    }



    bool isInteger(string pDeger)
    {
        bool retVal = false;

        try
        {
            Int64 t = Convert.ToInt64(pDeger);
            retVal = true;
        }
        catch (Exception ex)
        {
        }

        return retVal;
    }
}