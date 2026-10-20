<%@ Page Language="C#" AutoEventWireup="true" CodeFile="giris.aspx.cs" Inherits="giris" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
  	<title>Home</title>
  	<meta charset="utf-8">
	<link rel="icon" href="images/favicon.ico" type="image/x-icon">
    <link rel="shortcut icon" href="images/favicon.ico" type="image/x-icon" />
    <meta name="description" content="Your description">
    <meta name="keywords" content="Your keywords">
    <meta name="author" content="Your name">
    <link rel="stylesheet" href="css/style.css">
    <script src="js/jquery-1.7.1.min.js"></script>
    <script src="js/script.js"></script>
<!--[if lt IE 8]>
   <div style=' clear: both; text-align:center; position: relative;'>
     <a href="http://windows.microsoft.com/en-US/internet-explorer/products/ie/home?ocid=ie6_countdown_bannercode">
       <img src="http://storage.ie6countdown.com/assets/100/images/banners/warning_bar_0000_us.jpg" border="0" height="42" width="820" alt="You are using an outdated browser. For a faster, safer browsing experience, upgrade for free today." />
    </a>
  </div>
<![endif]-->
<!--[if lt IE 9]>
	<script src="js/html5.js"></script>
	<link rel="stylesheet" href="css/ie.css"> 
<![endif]-->
<style type="text/css">
        .style1
        {
            width: 180px;
        }
    </style>
</head>   
<body>
<form id="form2" runat="server">
<!--==============================header=================================-->
  <header>
	    <div class="container_12">
			<div class="grid_12">
				<h1><a class="logo" href="default.aspx"></a></h1>
				<nav>
					<ul class="sf-menu">
							<li class="current"><a href="default.aspx"><span>ANA SAYFA</span></a></li>
							<li class="sf-with-ul"><a href="more.html"><span>HAKKIMIZDA</span></a>
								<ul>
									<li><a href="more.html">VİZYON</a></li>
									<li><a href="more.html">HABERLER</a>
										<ul>
											<li><a href="more.html">SON YAPILAN ETKİNLİKLER</a></li>
											<li><a href="more.html">ARŞİV</a></li>
										</ul>
									</li>
									<li><a href="more.html">OKUL AİLE BİRLİĞİ</a></li>
									<li><a href="more.html">DESTEKÇİLERİMİZ</a></li>
								</ul>
							</li>
							<li><a href="giris.aspx"><span>GALERİ</span></a></li>
							<li><a href="#"><span>SERVİSLER</span></a></li>
							<li><a href="#"><span>EN İYİLER</span></a></li>
							<li><a href="index-5.html"><span>İLETİŞİM</span></a></li>
						</ul>
				  </nav>
				<div class="clear"></div>
			</div>
			<div class="clear"></div>
		 </div>
  </header>
<!--==============================content================================-->
    <section id="content">
    	<div class="container_12">
				<div class="wrapper">
						<article class="grid_12">
							<table width="800" border="0" cellspacing="5" cellpadding="5">
                              <tr>
                                <td class="style1">&nbsp;</td>
                                <td>&nbsp;</td>
                              </tr>
                              <tr>
                                <td class="style1"><h4>Referans No : </h4></td>
                                <td>
                                    <asp:TextBox ID="txtRandomSifre" runat="server" Font-Bold="True" Font-Size="24pt" 
                                        Width="400px" MaxLength="6"></asp:TextBox>
                                  </td>
                              </tr>
                              <tr>
                                <td class="style1"></td>
                                <td>
                                   <div style="padding-top:10px;">
                                    <asp:Button ID="btnGiris" runat="server" Font-Bold="False" Font-Size="18pt" 
                                        Text="Giriş" Width="405px" onclick="btnGiris_Click" />
                                   </div>
                                   
                                </td>
                              </tr>
                           
                              <tr>
                                <td class="style1">&nbsp;</td>
                                <td>
                                    <asp:Label ID="lblHata" runat="server"></asp:Label>
                                    <br />
                                   
                                </td>
                              </tr>
                           
                            </table>
						</article>
				</div>
			</div>
    </section>
<!--==============================footer=================================-->
    <footer>
		<div class="container_12">
			<div class="wrapper">
				<div class="grid_12">
					<div class="privacy">
						<a href="http://www.dizi.com.tr">TRÇEKİM bir Dizi Production iştirakidir.</a> © 2013 | <a href="http://www.dizi.com.tr">Gizlilik Politikası</a><br>
						<!-- {%FOOTER_LINK} -->
					</div>
					<div class="social">
						<a href="#" title="Twitter"><img src="images/soc1.png" width="28" height="56" alt=""></a>
						<a href="#" title="Facebook"><img src="images/soc2.png" width="28" height="56" alt=""></a>
						<a href="#" title="Flickr"><img src="images/soc3.png" width="28" height="56" alt=""></a>
						<a href="#" title="RSS"><img src="images/soc4.png" width="28" height="56" alt=""></a>
						<a href="#" title="Google +"><img src="images/soc5.png" width="28" height="56" alt=""></a>
					</div>
					<div class="clear"></div>
				</div>
			</div>
		</div>
    </footer>
    <asp:Label ID="lblHataMesaji" runat="server"></asp:Label>
    </form>
</body>
</html>