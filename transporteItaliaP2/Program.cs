using System;
using System.Collections.Generic;
using System.IO;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Drawing.Printing;
using Newtonsoft.Json;
using ZXing;
using System.Drawing;
using System.Globalization;
using System.Text;

namespace transporteItaliaP2
{
    class Program
    {

        // Declaracion de fuentes
        public static XFont fontCourier12 = new XFont("Courier New", 12, XFontStyle.Regular);
        public static XFont fontCourier11 = new XFont("Courier New", 11, XFontStyle.Regular);
        public static XFont fontCourier10 = new XFont("Courier New", 10, XFontStyle.Regular);
        public static XFont fontCourier9 = new XFont("Courier New", 9, XFontStyle.Regular);
        public static XFont fontCourier8 = new XFont("Courier New", 8, XFontStyle.Regular);
        public static XFont fontCourier7 = new XFont("Courier New", 7, XFontStyle.Regular);
        public static XFont fontCourier6 = new XFont("Courier New", 6, XFontStyle.Regular);
        public static XFont fontCourierBold35 = new XFont("Courier New", 35, XFontStyle.Bold);
        public static XFont fontCourierBold20 = new XFont("Courier New", 20, XFontStyle.Bold);
        public static XFont fontCourierBold15 = new XFont("Courier New", 15, XFontStyle.Bold);
        public static XFont fontCourierBold14 = new XFont("Courier New", 14, XFontStyle.Bold);
        public static XFont fontCourierBold13 = new XFont("Courier New", 13, XFontStyle.Bold);
        public static XFont fontCourierBold12 = new XFont("Courier New", 12, XFontStyle.Bold);
        public static XFont fontCourierBold11 = new XFont("Courier New", 11, XFontStyle.Bold);
        public static XFont fontCourierBold10 = new XFont("Courier New", 10, XFontStyle.Bold);
        public static XFont fontCourierBold9 = new XFont("Courier New", 9, XFontStyle.Bold);
        public static XFont fontCourierBold7 = new XFont("Courier New", 7, XFontStyle.Bold);
        public static XFont fontHelvetica35 = new XFont("Helvetica", 35, XFontStyle.Bold);
        private static StringBuilder log = new StringBuilder();
        /////////////////////////
        static void Main(string[] args)
        {
            string path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "IN_DEYCR");
            string textToParse;
            try
            {
                textToParse = System.IO.File.ReadAllText(path);
            }
            catch (FileNotFoundException e)
            {
                log.Append(DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " " + "PATH NOT FOUND" + "\n");
                log.Append(DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " " + e.Message + "\n");
                logWrite(log);
                throw e;
            }
            //Creamos un documento unico
            PdfSharp.Pdf.PdfDocument document = new PdfSharp.Pdf.PdfDocument();
            document.Info.Title = "Transporte Italia - Arrecifes";
            document.Options.FlateEncodeMode = PdfFlateEncodeMode.BestCompression;
            try
            {
                String fileName = pdfGeneratorTransItalia(textToParse, document);
                //string filename = tipoComprobante + DateTime.Now.ToString("ddMMMM")  + ".pdf";
                string pathFolder = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "_PDFS");
                if (!Directory.Exists(pathFolder))
                {
                    Directory.CreateDirectory(pathFolder);
                }
                string fullPath = Path.Combine(pathFolder, fileName);
                document.Save(fullPath);
                System.Diagnostics.Process.Start(fullPath);
                
            }
            catch (Exception e2)
            {
                log.Append(DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " "+ e2.Message + "\n");
                logWrite(log);
                throw e2;
            }

           
            //doc.Print();
            //printPDF(pathImpresion, filename);
            //File.Delete(filename);
            //System.Diagnostics.Process.Start(filename);
            //string cRun = "PDFlite.exe";
            //string arguments = " -print-to \"" + cPrinter + "\" " + " -print-settings \"" + "1x" + "\" " + filename;
            //string argument = "-print-to-default " + filename;
            //Process process = new Process();
            //process.StartInfo.FileName = cRun;
            //process.StartInfo.Arguments = argument;
            //process.Start();
            //process.WaitForExit();
            //File.Delete(filename);
        }
   

        static string GetDefaultPrinter()
        {
            PrinterSettings settings = new PrinterSettings();
            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                settings.PrinterName = printer;
                if (settings.IsDefaultPrinter)
                    return printer;
            }
            return string.Empty;
        }

        private static String pdfGeneratorTransItalia(string pagina, PdfSharp.Pdf.PdfDocument document) 
        {
            if (pagina.Length != 455)
            {
                throw new IndexOutOfRangeException("El documento contiene una cantidad de caracteres inválida");
            }
            int pivote = 0;
            PdfPage page = document.AddPage();
            page.Size = PdfSharp.PageSize.A4;

            // Get an XGraphics object for drawing
            XGraphics gfx = XGraphics.FromPdfPage(page);
            if (!File.Exists("ItaliaP2.jpg"))
            {
                throw  new FileNotFoundException("No se encontro el archivo ItaliaP2.jpg");
            }
            
            XImage img = XImage.FromFile("ItaliaP2.jpg");
            gfx.DrawImage(img, 0, 0);

            //Armado de variables
            String tipoComprobante = pagina.Substring(pivote, 3);
            String letra = pagina.Substring(pivote += 3, 1);
            String prefijo = pagina.Substring(pivote += 1, 4);
            String numero = pagina.Substring(pivote += 4, 8);

            String fecha = pagina.Substring(pivote += 8, 2) + "/" + pagina.Substring(pivote += 2, 2) + "/" + pagina.Substring(pivote += 2, 4);
            //
            String ma_cuenta = pagina.Substring(pivote += 4, 8);
            String ma_nombre = pagina.Substring(pivote += 8, 30);
            String ma_domicilio = pagina.Substring(pivote += 30, 30);
            String ma_localidad = pagina.Substring(pivote += 30, 30);

            String ma_cuit = pagina.Substring(pivote += 30, 11);
            String ma_condIva = pagina.Substring(pivote += 11, 15);

            List<string> cuerpos = new List<string>();//Tabla Cuerpo
            cuerpos.Add(pagina.Substring(pivote += 15, 75));
            for (int i = 0; i < 2; i++) cuerpos.Add(pagina.Substring(pivote += 75, 75));

            String subtotal = int.Parse(pagina.Substring(pivote += 75, 12)).ToString();
            subtotal = subtotal.Insert(subtotal.Length - 2, ".");
            String iva = int.Parse(pagina.Substring(pivote += 12, 12)).ToString();
            iva = iva.Insert(iva.Length - 2, ".");
            String ivaNoInscripto = int.Parse(pagina.Substring(pivote += 12, 12)).ToString();
            if (ivaNoInscripto == "0") ivaNoInscripto = "0.00";
            else ivaNoInscripto = ivaNoInscripto.Insert(ivaNoInscripto.Length - 2, ".");
            String exento = int.Parse(pagina.Substring(pivote += 12, 12)).ToString();
            if (exento == "0") exento = "0.00";
            else exento = exento.Insert(exento.Length - 2, ".");
            String total = int.Parse(pagina.Substring(pivote += 12, 12)).ToString();
            total = total.Insert(total.Length - 2, ".");

            String cae = pagina.Substring(pivote += 12, 14);
            String caeVto = pagina.Substring(pivote += 14, 4) + "/" + pagina.Substring(pivote += 4, 2) + "/" + pagina.Substring(pivote += 2, 2);

            gfx.DrawString(letra, fontHelvetica35, XBrushes.Black, 285, 41);
            gfx.DrawString("Código:" + tipoComprobante, fontCourier8, XBrushes.Black, 275, 54);

            gfx.DrawString(letra, fontHelvetica35, XBrushes.Black, 285, 460);
            gfx.DrawString("Código:" + tipoComprobante, fontCourier8, XBrushes.Black, 275, 474);

            int posy = 176;
            int posySegundaHoja = 596;
            int desdoblaCuerpo = 0;
            int posx = -15;
            foreach(string cuerpo in cuerpos)
            {
                if(desdoblaCuerpo == 0)
                {
                    posx = -10;
                }
                if(desdoblaCuerpo == 3)
                {
                    posy += 79;
                    posySegundaHoja += 79;
                    posx = -5;
                }
                if(desdoblaCuerpo == 8)
                {
                    posy = 230;
                    posySegundaHoja = 650;
                    posx = 25;
                }
                gfx.DrawString(cuerpo, fontCourier12, XBrushes.Black, posx, posy+=11);
                gfx.DrawString(cuerpo, fontCourier12, XBrushes.Black, posx, posySegundaHoja += 11);
                desdoblaCuerpo++;
            }

            //gfx.DrawString("FACTURA", fontCourierBold14, XBrushes.Black, 433, 25);
            gfx.DrawString("Factura: ", fontCourierBold12, XBrushes.Black, 386, 36);
            gfx.DrawString(prefijo + "." + numero, fontCourierBold12, XBrushes.Black, 450, 36);
            gfx.DrawString("Fecha: ", fontCourierBold12, XBrushes.Black, 386, 51);
            gfx.DrawString(fecha, fontCourierBold12, XBrushes.Black, 450, 51);

            //gfx.DrawString("FACTURA", fontCourierBold14, XBrushes.Black, 433, 442);
            gfx.DrawString("Factura: ", fontCourierBold12, XBrushes.Black, 386, 453);
            gfx.DrawString(prefijo + "." + numero, fontCourierBold12, XBrushes.Black, 450, 453);
            gfx.DrawString("Fecha: ", fontCourierBold12, XBrushes.Black, 386, 468);
            gfx.DrawString(fecha, fontCourierBold12, XBrushes.Black, 450, 468);

            String cuentaFileName;
           
            posy = 332;
            gfx.DrawString("$", fontCourier11, XBrushes.Black, 485, posy);
            gfx.DrawString("$", fontCourier11, XBrushes.Black, 485, posy -= 11);
            gfx.DrawString("$", fontCourier11, XBrushes.Black, 485, posy -= 11);
            gfx.DrawString("$", fontCourier11, XBrushes.Black, 485, posy -= 11);


            posy = 340;
            gfx.DrawString("SUB-TOTAL: ", fontCourierBold11, XBrushes.Black, 383, posy += 11);
            subtotal = FormateaPrecio(subtotal);
            gfx.DrawString("$  " + subtotal, fontCourier11, XBrushes.Black, 482, posy);
            gfx.DrawString("IVA 21%: ", fontCourier11, XBrushes.Black, 383, posy += 11);
            iva = FormateaPrecio(iva);
            gfx.DrawString("$  " + iva, fontCourier11, XBrushes.Black, 482, posy);
            gfx.DrawString("IVA NO I.: ", fontCourier11, XBrushes.Black, 383, posy += 11);
            //ivaNoInscripto = FormateaPrecio(ivaNoInscripto);
            gfx.DrawString("$  " + ivaNoInscripto, fontCourier11, XBrushes.Black, 482, posy);
            gfx.DrawString("TOTAL $: ", fontCourierBold11, XBrushes.Black, 383, posy += 11);
            total = FormateaPrecio(total);
            gfx.DrawString("$  " + total, fontCourier11, XBrushes.Black, 482, posy);

            posy = 752;
            gfx.DrawString("$", fontCourier11, XBrushes.Black, 485, posy);
            gfx.DrawString("$", fontCourier11, XBrushes.Black, 485, posy -= 11);
            gfx.DrawString("$", fontCourier11, XBrushes.Black, 485, posy -= 11);
            gfx.DrawString("$", fontCourier11, XBrushes.Black, 485, posy -= 11);

            posy = 760;
            gfx.DrawString("SUB-TOTAL: ", fontCourierBold11, XBrushes.Black, 383, posy += 11);
            gfx.DrawString("$  " + subtotal, fontCourier11, XBrushes.Black, 482, posy);
            gfx.DrawString("IVA 21%: ", fontCourier11, XBrushes.Black, 383, posy += 11);
            gfx.DrawString("$  " + iva, fontCourier11, XBrushes.Black, 482, posy);
            gfx.DrawString("IVA NO I.: ", fontCourier11, XBrushes.Black, 383, posy += 11);
            gfx.DrawString("$  " + ivaNoInscripto, fontCourier11, XBrushes.Black, 482, posy);
            gfx.DrawString("TOTAL: ", fontCourierBold11, XBrushes.Black, 383, posy += 11);
            gfx.DrawString("$  " + total, fontCourier11, XBrushes.Black, 482, posy);

            gfx.DrawString("C.A.E.: " + cae + " Vencimiento: " + caeVto, fontCourierBold9, XBrushes.Black, 60, 392);
            gfx.DrawString("C.A.E.: " + cae + " Vencimiento: " + caeVto, fontCourierBold9, XBrushes.Black, 60, 812);

            String fileName = tipoComprobante + "_" + letra + "_" + prefijo + "_" + numero + "_" + ma_cuenta + ".pdf";
            return fileName; 
        }

        private static String FormateaPrecio(String dato)
        {
            if(dato.Length == 6)
            {
                dato = " " + dato;
            }
            if(dato.Length == 5)
            {
                dato = "  " + dato;
            }
            if(dato.Length == 4)
            {
                dato = "   " + dato;
            }
            return dato;
        }

        private static void drawNomDomLoc(XGraphics gfx, String nombre, String domicilio, String localidad)
        {
            int posy = 343;
            gfx.DrawString("DESTINATARIO: ", fontCourierBold9, XBrushes.Black, 26, posy += 9);
            gfx.DrawString(nombre, fontCourier9, XBrushes.Black, 140, posy);
            gfx.DrawString("DOMICILIO: ", fontCourierBold9, XBrushes.Black, 26, posy += 9);
            gfx.DrawString(domicilio, fontCourier9, XBrushes.Black, 140, posy);
            gfx.DrawString("LOCALIDAD: ", fontCourierBold9, XBrushes.Black, 26, posy += 9);
            gfx.DrawString(localidad, fontCourier9, XBrushes.Black, 140, posy);
            posy = 762;
            gfx.DrawString("DESTINATARIO: ", fontCourierBold9, XBrushes.Black, 26, posy += 9);
            gfx.DrawString(nombre, fontCourier9, XBrushes.Black, 140, posy);
            gfx.DrawString("DOMICILIO: ", fontCourierBold9, XBrushes.Black, 26, posy += 9);
            gfx.DrawString(domicilio, fontCourier9, XBrushes.Black, 140, posy);
            gfx.DrawString("LOCALIDAD: ", fontCourierBold9, XBrushes.Black, 26, posy += 9);
            gfx.DrawString(localidad, fontCourier9, XBrushes.Black, 140, posy);
        }

        private static void drawNomDomLocIvaCuit(XGraphics gfx, String nombre, String domicilio, String localidad, String conIva, String cuit, String cuenta)
        {
            int posy = 103;
            gfx.DrawString(nombre, fontCourierBold12, XBrushes.Black, 26, posy+=15);
            gfx.DrawString("COND. IVA: ", fontCourierBold12, XBrushes.Black, 275, posy);
            gfx.DrawString(conIva, fontCourierBold12, XBrushes.Black, 350, posy);
            //gfx.DrawString("DESTINATARIO: ", fontCourier10, XBrushes.Black, 26, posy += 11);
            //gfx.DrawString("DOMICILIO: ", fontCourier10, XBrushes.Black, 26, posy += 11);
            gfx.DrawString(domicilio, fontCourierBold12, XBrushes.Black, 26, posy += 11);
            gfx.DrawString("CUIT: ", fontCourierBold12, XBrushes.Black, 275, posy);
            gfx.DrawString(cuit.Substring(0,2) + "-" + cuit.Substring(2,8) + "-" + cuit.Substring(10,1), fontCourierBold12, XBrushes.Black, 350, posy);
            //gfx.DrawString("LOCALIDAD: ", fontCourier10, XBrushes.Black, 26, posy += 11);
            gfx.DrawString(localidad, fontCourierBold12, XBrushes.Black, 26, posy += 11);
            gfx.DrawString("N. CUENTA: ", fontCourierBold12, XBrushes.Black, 275, posy);
            gfx.DrawString(cuenta, fontCourierBold12, XBrushes.Black, 350, posy);
            posy = 520;
            //gfx.DrawString("DESTINATARIO: ", fontCourier10, XBrushes.Black, 26, posy += 11);
            gfx.DrawString(nombre, fontCourierBold12, XBrushes.Black, 26, posy+=15);
            gfx.DrawString("COND. IVA: ", fontCourierBold12, XBrushes.Black, 275, posy);
            gfx.DrawString(conIva, fontCourierBold12, XBrushes.Black, 350, posy);
            //gfx.DrawString("DOMICILIO: ", fontCourier10, XBrushes.Black, 26, posy += 11);
            gfx.DrawString(domicilio, fontCourierBold12, XBrushes.Black, 26, posy+=11);
            gfx.DrawString("CUIT: ", fontCourierBold12, XBrushes.Black, 275, posy);
            gfx.DrawString(cuit.Substring(0, 2) + "-" + cuit.Substring(2, 8) + "-" + cuit.Substring(10, 1), fontCourierBold12, XBrushes.Black, 350, posy);
            //gfx.DrawString("LOCALIDAD: ", fontCourier10, XBrushes.Black, 26, posy += 11);
            gfx.DrawString(localidad, fontCourierBold12, XBrushes.Black, 26, posy+=11);
            gfx.DrawString("N. CUENTA: ", fontCourierBold12, XBrushes.Black, 275, posy);
            gfx.DrawString(cuenta, fontCourierBold12, XBrushes.Black, 350, posy);
        }

        private static void DrawQR(XGraphics gfx, String fecha, String cuit, int prefijo, int tipoComp, int numero, Double importe, String cae)
        {
            //Preparo la fecha
            DateTime d = DateTime.ParseExact(fecha, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var fechaParseada = d.ToString("yyyy-MM-dd");
            //Inicio el string que va a ir en el QR
            String qr = "https://www.afip.gob.ar/fe/qr/?p=";
            //Creo un objeto del tipo QRAfip con todo lo que me piden los delincuentes
            QRAfip qrAFIP = new QRAfip(
                1,//version
                fechaParseada, //fecha
                cuit,//cuit ma o re
                prefijo, //pto de venta
                tipoComp,
                numero,
                importe,//cambiar coma por punto
                "PES",
                "1",
                "E",
                cae
                );
            //Creo el JSON
            string jsonQRAfip = JsonConvert.SerializeObject(qrAFIP);
            //Lo paso a base64
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(jsonQRAfip);
            var jsonBase64 = System.Convert.ToBase64String(plainTextBytes);
            //Se lo agrego al qr para completarlo
            qr += jsonBase64;

            //Con esta prueba vemos si el JSON se paso a base64 Correctamente.
            /*var base64EncodedBytes = System.Convert.FromBase64String(jsonBase64);
            var pruebadecode = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);*/

            //Generate  & Draw QR
            var bcWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new ZXing.QrCode.QrCodeEncodingOptions
                {
                    ErrorCorrection = ZXing.QrCode.Internal.ErrorCorrectionLevel.Q,
                    Height = 85,
                    Width = 85,
                    Margin = 0,
                },
            };
            Bitmap bm = bcWriter.Write(qr);
            XImage img = XImage.FromGdiPlusImage((Image)bm);
            img.Interpolate = false;
            
            gfx.DrawImage(img, 497, 93);
            gfx.DrawImage(img, 497, 512);


        }
        class QRAfip
        {
            Int32 ver;
            String fecha;
            String cuit;
            Int32 ptoVta;
            Int32 tipoComp;
            Int32 nroCmp;
            Double importe;
            String moneda;
            String ctz;
            //Int32 tipoDocRec;
            //Double nroDocRec;
            String tipoCodAut;
            String codAut;

            //Constructor
            public QRAfip(Int32 ver,
                String fecha,
                String cuit,
                Int32 ptoVta,
                Int32 tipoComp,
                Int32 nroCmp,
                Double importe,
                String moneda,
                String ctz,
                //Int32 tipoDocRec,
                //Double nroDocRec,
                String tipoCodAut,
                String codAut
                )
            {
                this.Ver = ver;
                this.Fecha = fecha;
                this.Cuit = cuit;
                this.PtoVta = ptoVta;
                this.TipoComp = tipoComp;
                this.NroCmp = nroCmp;
                this.Importe = importe;
                this.Moneda = moneda;
                this.Ctz = ctz;
                //this.tipoDocRec=tipoDocRec
                //this.nroDocRec =nroDocRec 
                this.TipoCodAut = tipoCodAut;
                this.CodAut = codAut;
            }

            public int Ver { get => ver; set => ver = value; }
            public string Fecha { get => fecha; set => fecha = value; }
            public string Cuit { get => cuit; set => cuit = value; }
            public int PtoVta { get => ptoVta; set => ptoVta = value; }
            public int TipoComp { get => tipoComp; set => tipoComp = value; }
            public int NroCmp { get => nroCmp; set => nroCmp = value; }
            public double Importe { get => importe; set => importe = value; }
            public string Moneda { get => moneda; set => moneda = value; }
            public string Ctz { get => ctz; set => ctz = value; }
            public string TipoCodAut { get => tipoCodAut; set => tipoCodAut = value; }
            public string CodAut { get => codAut; set => codAut = value; }
        }

        static void logWrite(StringBuilder log)
        {
            string pathFolder = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "logs");
            if (!Directory.Exists(pathFolder))
            {
                Directory.CreateDirectory(pathFolder);
            }
            string logName = "log_" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt";
            File.AppendAllText(Path.Combine(pathFolder,logName) , log.ToString() + Environment.NewLine);
        }
    }
}