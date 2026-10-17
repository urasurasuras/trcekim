<%@ Page Language="C#" AutoEventWireup="true" CodeFile="sil.aspx.cs" Inherits="sil" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .style1
        {
            width: 130px;
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

<body>
    <form id="form1" runat="server">
    <div>
    <table width="800" border="0" cellspacing="5" cellpadding="5">
                              <tr>
                                <td class="style1">&nbsp;</td>
                                <td>&nbsp;</td>
                              </tr>
                              <tr>
                                <td class="style1">Referans No : </td>
                                <td>
                                    <asp:TextBox ID="TextBox1" runat="server" Font-Bold="True" Font-Size="24pt" 
                                        Width="400px"></asp:TextBox>
                                  </td>
                              </tr>
                              <tr>
                                <td class="style1"></td>
                                <td>
                                   
                                    <asp:Button ID="btnGiris" runat="server" Font-Bold="False" Font-Size="18pt" 
                                        Text="Giriş" Width="400px" />
                                   
                                </td>
                              </tr>
                           
                            </table>
    </div>
    </form>
</body>
</html>
