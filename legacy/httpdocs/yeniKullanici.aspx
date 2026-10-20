<%@ Page Language="C#" AutoEventWireup="true" CodeFile="yeniKullanici.aspx.cs" Inherits="yeniKullanici" CompilationMode="Always" MaintainScrollPositionOnPostback="true" %>

<%@ Register assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI" tagprefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    
    <style type="text/css">
        .style1
        {
            text-align: center;
        }
    </style>
    

<script>
  (function(i,s,o,g,r,a,m){i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){
  (i[r].q=i[r].q||[]).push(arguments)},i[r].l=1*new Date();a=s.createElement(o),
  m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m)
  })(window,document,'script','//www.google-analytics.com/analytics.js','ga');

  ga('create', 'UA-70919502-1', 'auto');
  ga('send', 'pageview');

</script>
</head>

<body style="font-family:'Trebuchet MS';">
<form id="form1" runat="server">
<%--        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>--%>
                <div style="width:620px; margin:0 auto;padding-top:50px;">
                    <table border="0" cellpadding="5" cellspacing="0" width="602">
                    <tr>
                        <td class="style1" colspan="3" style="width: 569px">
                            <strong>YENİ KULLANICI</strong></td>
                    </tr>
                    <tr>
                        <td width="177">
                            &nbsp;</td>
                        <td width="6">
                            &nbsp;</td>
                        <td width="409">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td width="177">
                            Kullanıcı Adı&nbsp;<span style="color:Red;">*</span></td>
                        <td width="6">
                            :</td>
                        <td width="409">
                            <asp:TextBox ID="txtKullaniciIsmi" runat="server" Width="250px"></asp:TextBox>
                            &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                                ErrorMessage="Kullanıcı Adı Boş Bırakılamaz!" 
                                ControlToValidate="txtKullaniciIsmi" Display="Dynamic" 
                                ValidationGroup="v1">*</asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Şifre&nbsp;<span style="color:Red;">*</span></td>
                        <td>
                            :</td>
                        <td>
                            <asp:TextBox ID="txtSifre" runat="server" TextMode="Password" Width="250px"></asp:TextBox>
                            &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                                ControlToValidate="txtSifre" ErrorMessage="Şifre kısmı boş bırakılamaz" 
                                ValidationGroup="v1">*</asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Şifre (Tekrar)&nbsp;<span style="color:Red;">*</span></td>
                        <td>
                            :</td>
                        <td>
                            <asp:TextBox ID="txtSifreTekrar" runat="server" TextMode="Password" Width="250px"></asp:TextBox>
                            &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" 
                                ControlToValidate="txtSifreTekrar" ErrorMessage="Tekrar girdiğiniz Şifre kısmı boş bırakılamaz" 
                                ValidationGroup="v1">*</asp:RequiredFieldValidator>
                        &nbsp;<asp:CompareValidator ID="CompareValidator1" runat="server" 
                                ControlToCompare="txtSifre" ControlToValidate="txtSifreTekrar" 
                                ErrorMessage="Şifre ve Şifre Tekrar alanlarına girilen değelerin aynı olması gerekir." 
                                ValidationGroup="v1">*</asp:CompareValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Öğrenci Adı&nbsp;<span style="color:Red;">*</span></td>
                        <td>
                            :</td>
                        <td>
                            <asp:TextBox ID="txtOgrenciAdi" runat="server" Width="250px"></asp:TextBox>
                            &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                                ControlToValidate="txtOgrenciAdi" 
                                ErrorMessage="Öğrenci Adı boş bırakılamaz" ValidationGroup="v1">*</asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Öğrenci Soyadı&nbsp;<span style="color:Red;">*</span></td>
                        <td>
                            :</td>
                        <td>
                            <asp:TextBox ID="txtOgrenciSoyadi" runat="server" Width="250px"></asp:TextBox>
                            &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                                ControlToValidate="txtOgrenciSoyadi" 
                                ErrorMessage="Öğrenci Soyadı boş bırakılamaz" ValidationGroup="v1">*</asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Veli Yakınlık Derecesi&nbsp;<span style="color:Red;">*</span></td>
                        <td>
                            :</td>
                        <td>
                            <asp:DropDownList ID="drpYakinlikDerecesi" runat="server" Width="250px" 
                                DataTextField="tur" DataValueField="yakinlikID">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Veli Adı&nbsp;<span style="color:Red;">*</span></td>
                        <td>
                            :</td>
                        <td>
                            <asp:TextBox ID="txtVeliAdi" runat="server" Width="250px"></asp:TextBox>
                            &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" 
                                ControlToValidate="txtVeliAdi" ErrorMessage="Veli Adı boş bırakılamaz" 
                                ValidationGroup="v1">*</asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Veli Soyadı&nbsp;<span style="color:Red;">*</span></td>
                        <td>
                            :</td>
                        <td>
                            <asp:TextBox ID="txtVeliSoyadi" runat="server" Width="250px"></asp:TextBox>
                            &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" 
                                ControlToValidate="txtVeliSoyadi" 
                                ErrorMessage="Veli Soyadı boş bırakılamaz" ValidationGroup="v1">*</asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            E-mail<span style="color:Red;"> *</span></td>
                        <td>
                            :</td>
                        <td>
                            <asp:TextBox ID="txtEmail" runat="server" Width="250px"></asp:TextBox>
                            &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" 
                                ControlToValidate="txtEmail" 
                                ErrorMessage="E-mail boş bırakılamaz" ValidationGroup="v1">*</asp:RequiredFieldValidator>
                        &nbsp;<asp:RegularExpressionValidator ID="RegularExpressionValidator1" 
                                runat="server" ControlToValidate="txtEmail" 
                                ErrorMessage="Lütfen e-mail adresini geçerli bir formatta yazınız." 
                                ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" 
                                ValidationGroup="v1">*</asp:RegularExpressionValidator>
                        </td>
                    </tr>
                     <tr>
                        <td>
                            E-mail (Tekrar)
                            <span style="color:Red;">*</span></td>
                        <td>
                            :</td>
                        <td>
                            <asp:TextBox ID="txtEmailTekrar" runat="server" Width="250px"></asp:TextBox>
                            &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" 
                                ControlToValidate="txtEmailTekrar" 
                                ErrorMessage="Tekrar amaçlı yazılan e-mail boş bırakılamaz" 
                                ValidationGroup="v1">*</asp:RequiredFieldValidator>
                        &nbsp;<asp:RegularExpressionValidator ID="RegularExpressionValidator2" 
                                runat="server" ControlToValidate="txtEmailTekrar" 
                                ErrorMessage="Lütfen tekrar amaçlı yazdığınız e-mail adresini geçerli bir formatta yazınız." 
                                ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" 
                                ValidationGroup="v1">*</asp:RegularExpressionValidator>
                        &nbsp;<asp:CompareValidator ID="CompareValidator2" runat="server" 
                                ControlToCompare="txtEmail" ControlToValidate="txtEmailTekrar" 
                                ErrorMessage="E-mail ve E-mail Tekrar alanlarına girilen değelerin aynı olması gerekir." 
                                ValidationGroup="v1">*</asp:CompareValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Cep Telefonu
                            <span style="color:Red;">*</span></td>
                        <td>
                            :</td>
                        <td>
                            <asp:TextBox ID="txtCep" runat="server" Width="250px"></asp:TextBox>
                        &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server" 
                                ControlToValidate="txtCep" 
                                ErrorMessage="Cep Telefonu bilgisi boş bırakılamaz." ValidationGroup="v1">*</asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td style="vertical-align:top;">Kargo Adresi</td>
                        <td style="vertical-align:top;">:</td>
                        <td style="vertical-align:top;">
                            <asp:TextBox ID="txtKargoAdresi" runat="server" Width="250px" Height="200px" 
                                TextMode="MultiLine"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;</td>
                        <td>
                            &nbsp;</td>
                        <td>
                            &nbsp;</td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;</td>
                        <td>
                            &nbsp;</td>
                        <td>
                            <asp:Button ID="btnKaydet" runat="server" onclick="btnKaydet_Click" 
                                Text="Kaydet" Width="250px" Height="40px" ValidationGroup="v1" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;</td>
                        <td>
                            &nbsp;</td>
                        <td>
                            &nbsp;</td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;</td>
                        <td>
                            &nbsp;</td>
                        <td>
                            <span style="color:Red;">* şeklinde gösterilen alanları doldurmak zorunludur!</span></td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;</td>
                        <td>
                            &nbsp;</td>
                        <td>
                            <asp:HyperLink ID="HyperLink1" runat="server" Font-Size="10pt" 
                                NavigateUrl="~/giris.aspx">Kaydı iptal et ve ana sayfaya dön</asp:HyperLink>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;</td>
                        <td>
                            &nbsp;</td>
                        <td>
                            <asp:ValidationSummary ID="ValidationSummary1" runat="server" 
                                ShowMessageBox="True" ValidationGroup="v1" ShowSummary="False" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;</td>
                        <td>
                            &nbsp;</td>
                        <td>
                            <asp:Label ID="lblHata" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;</td>
                        <td>
                            &nbsp;</td>
                        <td>
                            &nbsp;</td>
                    </tr>
                </table>
                </div>
<%--            </ContentTemplate>
        </asp:UpdatePanel>--%>
</form>
</body>
</html>
