<%@ Page Language="C#" AutoEventWireup="true" CodeFile="kullaniciGirisi.aspx.cs" Inherits="kullaniciGirisi" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    

<script>
  (function(i,s,o,g,r,a,m){i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){
  (i[r].q=i[r].q||[]).push(arguments)},i[r].l=1*new Date();a=s.createElement(o),
  m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m)
  })(window,document,'script','//www.google-analytics.com/analytics.js','ga');

  ga('create', 'UA-70919502-1', 'auto');
  ga('send', 'pageview');

</script>
</head>

<body>
    <form id="form1" runat="server">
        <div style="width:420px; margin:0 auto;padding-top:100px;">
            	<table width="402" border="0" cellspacing="0" cellpadding="5">
  <tr>
    <td width="100" class="style1">Kullanıcı Adı</td>
    <td width="4">:</td>
    <td width="296">
        <asp:TextBox ID="txtKullaniciIsmi" runat="server" Width="250px" 
            ValidationGroup="v1"></asp:TextBox>
      &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator13" runat="server" 
                                ControlToValidate="txtKullaniciIsmi" 
                                ErrorMessage="Kullanıcı Ismi boş bırakılamaz." 
                                ValidationGroup="v1">*</asp:RequiredFieldValidator>
      </td>
  </tr>
  <tr>
    <td>Şifre</td>
    <td>:</td>
    <td>
        <asp:TextBox ID="txtSifre" runat="server" TextMode="Password" Width="250px" 
            ValidationGroup="v1"></asp:TextBox>
      &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator14" runat="server" 
                                ControlToValidate="txtSifre" 
                                ErrorMessage="Şifre boş bırakılamaz." 
                                ValidationGroup="v1">*</asp:RequiredFieldValidator>
      </td>
  </tr>
  <tr>
    <td>&nbsp;</td>
    <td>&nbsp;</td>
    <td>
        <asp:Button ID="btnGiriş" runat="server" Text="Giriş" Width="200px" 
            onclick="btnGiriş_Click" Height="40px" ValidationGroup="v1" />
      </td>
  </tr>
  <tr>
    <td>&nbsp;</td>
    <td>&nbsp;</td>
    <td>&nbsp;</td>
  </tr>
  <tr>
    <td>&nbsp;</td>
    <td>&nbsp;</td>
    <td>
        <asp:LinkButton ID="lnkBtnYeniUye" runat="server" onclick="lnkBtnYeniUye_Click">* Üye değilseniz olmak için lüften tıklayınız.</asp:LinkButton>
      </td>
  </tr>
  <tr>
    <td>&nbsp;</td>
    <td>&nbsp;</td>
    <td>
                            <asp:ValidationSummary ID="ValidationSummary1" runat="server" 
                                ShowMessageBox="True" ValidationGroup="v1" ShowSummary="False" />
                        <br />
        <asp:Label ID="lblHata" runat="server"></asp:Label>
      </td>
  </tr>
</table>

        </div>
    </form>
</body>
</html>
