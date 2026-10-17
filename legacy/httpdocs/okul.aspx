<%@ Page Language="C#" AutoEventWireup="true" CodeFile="okul.aspx.cs" Inherits="okul" %>

<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>www.trcekim.com</title>
    <meta charset="utf-8">
    <link rel="icon" href="images/favicon.ico" type="image/x-icon">
    <link rel="shortcut icon" href="images/favicon.ico" type="image/x-icon" />
    <meta name="description" content="Your description">
    <meta name="keywords" content="Your keywords">
    <meta name="author" content="Your name">
    <link rel="stylesheet" href="css/style.css">
    <script src="js/jquery-1.7.2.min.js"></script>
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

<script type="text/javascript">
<!--

    // Disable Right Click Script

    function IE(e) {
        if (navigator.appName == "Microsoft Internet Explorer" && (event.button == "2" || event.button == "3")) {
            return false;
        }
    }
    function NS(e) {
        if (document.layers || (document.getElementById && !document.all)) {
            if (e.which == "2" || e.which == "3") {
                return false;
            }
        }
    }
    document.onmousedown = IE; document.onmouseup = NS; document.oncontextmenu = new Function("return false");
 
//-->
</script>

    <!--HIGHSLIDE-->
    <script type="text/javascript" src="highslide/highslide-with-html.js"></script>
    <link rel="stylesheet" type="text/css" href="highslide/highslide.css" />
    <script type="text/javascript">
        hs.graphicsDir = 'highslide/graphics/';
        hs.outlineType = 'rounded-white';
        hs.wrapperClassName = 'draggable-header';
    </script>
    <!--HIGHSLIDE-SON-->
    <style type="text/css">
        .popup
        {
            width: 600px;
            background-color: #fff;
            position: fixed;
            z-index:5500;
            margin: 0 auto;
            border:solid 4px Yellow;
            display: none;
            padding: 10px;
            -moz-border-radius: 10px;
            border-radius: 10px;
            -o-border-radius: 10px;
            -ms-border-radius: 10px;
            -webkit-border-radius: 10px;
            font-family: "Trebuchet MS" , Arial, Helvetica, sans-serif;
        }
        
        .ortala
        {
            text-align: center;
        }
        
        .sagaDayali
        {
            text-align: right;
        }
        
        .hediyecik
        {
            text-align: left;
            color: Red;
            font-size: 9pt;
        }
    </style>
    <script type="text/javascript">

        function sepetiKonumunaAl() {
            var winH = $(window).height();
            var winW = $(window).width();

            var sepet = document.getElementById("sepet");
            sepet.style.left = (winW - parseInt(sepet.style.width) - 4) + 'px';
        };

        $(document).ready(function () {

            var ResimID = "";
            var kisiID = "";
            var secilmisResimBilgisi = "";

            $(".sepeteEkle").click(function () {

                

                ResimID = $(this).attr("deger");
                kisiID = $("#kisiID").attr("kisiIDBilgisi");
                secilmisResimBilgisi = $("#secilmisResimBilgisi").attr("secilmisResimBilgisiDeger"); 

                //alert('secilmisResimBilgisi  : ' + secilmisResimBilgisi);

                //                if (secilmisResim != "") 
                //                {
                //                    alert('secilmisResim boş değil : ' + secilmisResim);
                //                    ResimID = secilmisResim;
                //                }
                //                else 
                //                {
                //                    ResimID = $(this).attr("deger");
                //                    alert('secilmisResim boş : ' + secilmisResim);
                //                }

                //                $("#sepetIcinBoyutSecilenEkrandaResimIDGoruntuleme").html(ResimID);

                //Ekran yüksekliği
                var winH = $(window).height();
                // Ekran Genişliği
                var winW = $(window).width();
                $("#mask").css({ "width": winW, "height": winH });
                $("#mask").fadeTo("slow", 0.3);
                $(".popup").css({ "left": (winW - $(".popup").width()) / 2, "top": 200 });
                $("#mask").show();
                $(".popup").show();
            });


            function WsDenemeFuntionByParametreli_onComplete(result) {
                if (result.d.length > 0) {
                    ResimID = "";
                    secilmisResim = "";

                    $("#mask").hide();
                    $(".popup").hide();

                    //alert(result.d);

                    temizle();
                }
            }

            function temizle() {
                var txt15x23 = document.getElementById('txt15x23'); txt15x23.value = '';
                var txt20x30 = document.getElementById('txt20x30'); txt20x30.value = '';
                var txt50x70 = document.getElementById('txt50x70'); txt50x70.value = '';

                var araToplam15x23 = document.getElementById('araToplam15x23'); araToplam15x23.innerHTML = '';
                var araToplam20x30 = document.getElementById('araToplam20x30'); araToplam20x30.innerHTML = '';
                var araToplam50x70 = document.getElementById('araToplam50x70'); araToplam50x70.innerHTML = '';

                var toplam = document.getElementById('toplam'); toplam.innerHTML = '';

                var hediye = document.getElementById('hediye'); hediye.innerHTML = '';
            }

            //arkadaki gri arkafona tıklayınca popup'ın kapanması
            $("#mask").click(function () {
                $(this).hide();
                $(".popup").hide();
            });

            $("#secimEkraniniKapat").click(function () {
                $("#mask").hide();
                $(".popup").hide();
            });

            $("#btnSecim").click(function () {

                alert('Sepete ekle şu an aktif değildir. Lütfen seçtiğiniz resim için resmin altında yer alan resim kodunu not alarak bildiriniz.');

                var adet15x23 = $("#txt15x23").val(); /* 5 TL */
                var adet20x30 = $("#txt20x30").val(); /* 15 TL */
                var adet50x70 = $("#txt50x70").val(); /* 70 TL */
                var devam = "";
                var gndr = "";

                if (adet15x23.length == 0) { adet15x23 = 0; } else { if (isInteger(adet15x23)) { devam = 1; } }
                if (adet20x30.length == 0) { adet20x30 = 0; } else { if (isInteger(adet20x30)) { devam = 1; } }
                if (adet50x70.length == 0) { adet50x70 = 0; } else { if (isInteger(adet50x70)) { devam = 1; } }

                if (devam == 1) {
                    gndr = adet15x23 + "~" + adet20x30 + "~" + adet50x70;
                }

                if (gndr.length > 0) {
                    $.ajax(
                     {
                         type: "POST",
                         url: "WebService2.asmx/sonucGetir2",
                         data: "{'pSecilenResim':\"" + kisiID + "~" + ResimID.toString() + "~" + gndr.toString() + "\"}",
                         dataType: "json",
                         contentType: "application/json; charset=utf-8",
                         error: function (xhr, ajaxOptions, thrownError) {
                             alert("Hata : " + xhr.status + " - " + thrownError);
                         },
                         success: WsDenemeFuntionByParametreli_onComplete
                     });

                    var sepet = document.getElementById("sepet");
                    sepet.style.visibility = 'visible';
                    sepet.style.display = 'block';


                    //Web servisine tek parametre ile git.	
                }
            });


        });


        var toplamHediye = 0;

        function hesapla15x23() {

            /*

            var txt15x23 = document.getElementById('txt15x23');
            var araToplam15x23 = document.getElementById('araToplam15x23');

            if (txt15x23.value.length > 0) {
            if (isInteger(txt15x23.value)) {
            araToplam15x23.innerHTML = (parseInt(txt15x23.value) * 5) + ' TL';
            }
            else {
            araToplam15x23.innerHTML = '';
            }
            }

            genelToplam();

            */
        }

        function hesapla20x30() {

            /*

            var txt20x30 = document.getElementById('txt20x30');
            var araToplam20x30 = document.getElementById('araToplam20x30');

            if (txt20x30.value.length > 0) {
                if (isInteger(txt20x30.value)) {
                    araToplam20x30.innerHTML = (parseInt(txt20x30.value) * 15) + ' TL';
                }
                else {
                    araToplam20x30.innerHTML = '';
                }
            }

            genelToplam();

            */
        }

        function hesapla50x70() {
            
            /*

            var txt50x70 = document.getElementById('txt50x70');
            var araToplam50x70 = document.getElementById('araToplam50x70');

            if (txt50x70.value.length > 0) {
                if (isInteger(txt50x70.value)) {
                    araToplam50x70.innerHTML = (parseInt(txt50x70.value) * 70) + ' TL';
                }
                else {
                    araToplam50x70.innerHTML = '';
                }
            }

            genelToplam();

            */
        }

        function genelToplam() {

            /*

            var araToplam15x23 = document.getElementById('araToplam15x23');
            var araToplam20x30 = document.getElementById('araToplam20x30');
            var araToplam50x70 = document.getElementById('araToplam50x70');
            var toplam = document.getElementById('toplam');
            var GenelToplam = 0;

            if (araToplam15x23.innerHTML.length > 0 && isInteger(araToplam15x23.innerHTML)) {
                GenelToplam = parseInt(GenelToplam) + parseInt(araToplam15x23.innerHTML.replace(" TL", ""));
            }

            if (araToplam20x30.innerHTML.length > 0 && isInteger(araToplam20x30.innerHTML)) {
                GenelToplam = parseInt(GenelToplam) + parseInt(araToplam20x30.innerHTML.replace(" TL", ""));
            }

            if (araToplam50x70.innerHTML.length > 0 && isInteger(araToplam50x70.innerHTML)) {
                GenelToplam = parseInt(GenelToplam) + parseInt(araToplam50x70.innerHTML.replace(" TL", ""));
            }

            toplam.innerHTML = GenelToplam + ' TL';

            var txt5x23 = document.getElementById('txt15x23');
            var txt20x30 = document.getElementById('txt20x30');
            var txt50x70 = document.getElementById('txt50x70');
            var hediye = document.getElementById('hediye');
            var adetToplam = 0;

            if (txt15x23.value.length > 0 && isInteger(txt15x23.value)) {
                adetToplam = parseInt(adetToplam) + parseInt(txt15x23.value);
            }

            if (txt20x30.value.length > 0 && isInteger(txt20x30.value)) {
                adetToplam = parseInt(adetToplam) + parseInt(txt20x30.value);
            }

            if (txt50x70.value.length > 0 && isInteger(txt50x70.value)) {
                adetToplam = parseInt(adetToplam) + parseInt(txt50x70.value);
            }

            if (Math.floor(parseInt(adetToplam) / 10) > 0) {
                var hediyeAdedi = Math.floor(parseInt(adetToplam) / 10);

                var hediyeMetni = 'Sipariş etmekte olduğunuz ' + adetToplam + ' adet resim karşılığında <b style=\"font-size:12pt;\">' + hediyeAdedi + ' adet</b> 15x23 hediye resim kazandınız!';

                hediye.innerHTML = hediyeMetni;
            }
            else {
                hediye.innerHTML = '';
            }

            */
        }

        function isInteger(deger) {
            var retVal = false;

            try {
                var temp = parseInt(deger);

                if (isNaN(temp)) {
                }
                else {
                    retVal = true;
                }
            }
            catch (ex) {
            }

            return retVal;
        }
		

    </script>

<script>
  (function(i,s,o,g,r,a,m){i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){
  (i[r].q=i[r].q||[]).push(arguments)},i[r].l=1*new Date();a=s.createElement(o),
  m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m)
  })(window,document,'script','//www.google-analytics.com/analytics.js','ga');

  ga('create', 'UA-70919502-1', 'auto');
  ga('send', 'pageview');

</script>
</head>

<body onload="alert('Sepete ekle şu an aktif değildir. Lütfen seçtiğiniz resim için resmin altında yer alan resim kodunu not alarak bildiriniz.');sepetiKonumunaAl();" onresize="sepetiKonumunaAl();">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    &nbsp;<!--==============================header=================================--><header><div class="container_12">
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
				<div id="portfolio">
						<article class="grid_12">
								<h2 class="h2 ind2"><asp:Label ID="lblOkulIsmi" runat="server"></asp:Label></h2>
<%--<div id="sepet" style="position:fixed;top:2px;width:466px;border:solid 1px Red;background-color:LightYellow;z-index:600;display:none;visibility:hidden;"></div>--%>

<div id="secilmisResimBilgisi" secilmisResimBilgisiDeger=""  style="visibility:hidden;display:none;"></div>

<div class="popup" style="width:370px;margin:0px;margin:0 auto;padding-top:40px;">

    <%--<div id="sepetIcinBoyutSecilenEkrandaResimIDGoruntuleme" style="color:Red;font-size:24pt;text-align:center;"></div>
    <br />
    <br />    --%>      
                                                                                                                                  <table id="tablocuk" width="370" border="1" cellspacing="0" cellpadding="4" style="border-color:#FFF;">
          <tr class="ortala">
            <td width="89">Boyut</td>
            <td width="83">Birim Fiyat</td>
            <td width="51">Adet</td>
            <td width="96">Ara Toplam</td>
          </tr>
          <tr>
            <td class="ortala" >15 x 23</td>
            <td class="sagaDayali">0 TL</td>
            <td class="ortala"><input name="txt15x23" type="text" id="txt15x23" style="width:50px;" onkeyup="hesapla15x23();" /></td>
            <td class="ortala" id="araToplam15x23"></td>
          </tr>
          <tr>
            <td class="ortala">20 x 30</td>
            <td class="sagaDayali">0 TL</td>
            <td class="ortala"><input type="text" name="txt20x30" id="txt20x30" style="width:50px;" onkeyup="hesapla20x30();"  /></td>
            <td class="ortala" id="araToplam20x30">&nbsp;</td>
          </tr>
          <tr>
            <td class="ortala">50 x 70</td>
            <td class="sagaDayali">0 TL</td>
            <td class="ortala"><input type="text" name="txt50x70" id="txt50x70" style="width:50px;" onkeyup="hesapla50x70();"  /></td>
            <td class="ortala" id="araToplam50x70">&nbsp;</td>
          </tr>
          <tr>
            <td colspan="3" class="sagaDayali">Toplam :&nbsp;</td>
            <td class="ortala" id="toplam"></td>
          </tr>
          </table>
    <br/>
    <input id="btnSecim" type="button" value="Seçimi Onayla ve Sepete At" style="width:370px;height:40px;"/>
    <br/>
    <br/>
    <table width="370" border="0" cellspacing="0" cellpadding="4" style="border-color:#FFF;">
  <tr class="ortala">
    <td id="hediye" class="hediyecik"></td>   
  </tr>
</table>
    <div id="secimEkraniniKapat" style="color:Navy;font-size:10pt;text-align:center;cursor:pointer;cursor:hand">Bu pencereyi kapatmak için lütfen tıklayınız.</div>
    <br />
</div>
    
<div id="mask" style="position:fixed; left: 0; top: 0; z-index:4500; background-color: #000;display: none;"></div>

<asp:Label ID="lblIcerik" runat="server"></asp:Label>


                               

						
						</article>
						<div class="clear"></div>
				</div>
      </div>
    </section>
    <!--==============================footer=================================-->
    <footer>
		<div class="container_12">
			<div class="wrapper">
				<div class="grid_12">
					<div class="privacy">
						<a href="index.html">TRÇEKİM bir Dizi Production iştirakidir.</a> © 2012 | <a href="index-6.html">Privacy policy</a>
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
    </form>
</body>
</html>
