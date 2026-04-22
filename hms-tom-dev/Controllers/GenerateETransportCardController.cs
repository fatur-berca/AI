using DFIS.Universal.Domain.Outputs;
using DFIS.Utils;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using iTextSharp.text;
using iTextSharp.text.pdf;
using hms_tom_dev.Models.Transport;

namespace hms_tom_dev.Controllers
{
    public class GenerateETransportCardController : BaseController
    {
        public GenerateETransportCardController()
        {
            SetPage(EnumHelper.GetDescription(Enums.PageName.TransportGenerateETransportCard));
        }
        // GET: GenerateETransportCard
        public ActionResult Index()
        {
            var session = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            ViewBag.PageAccess = session.Page.Select(x => x.FunctionName).ToList();
            return View();
        }

        public FileResult downloadETransportCardPdf()
        {
            GenerateETransportCard tGen = new GenerateETransportCard();
            List<string> codes = Convert.ToString(Request.QueryString["codes"]).Split('|').ToList();
            byte[] byteInfo = tGen.generateETransportCardPdf(codes);
            MemoryStream ms = new MemoryStream(byteInfo);
            ms.Write(byteInfo, 0, byteInfo.Length);
            ms.Position = 0;

            return File(ms, "application/pdf", "GenerateETransportCard" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf");
        }

        public FileResult pdf()
        {
            string[] codes = Convert.ToString(Request.QueryString["codes"]).Split('|');
            MemoryStream workStream = new MemoryStream();
            //Document document = new Document();
            Document doc = new Document(PageSize.A4, 88f, 88f, 10f, 10f);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            /*
            document.Open();
            document.Add(new Paragraph("Hello World"));
            document.Add(new Paragraph(DateTime.Now.ToString()));
            document.Close();
            */
           
            Font NormalFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, 12, Font.NORMAL, BaseColor.BLACK);
            doc.Open();
            PdfPTable table = new PdfPTable(2);
            foreach (string code in codes)
            {
                BarcodePDF417 pdf417 = new BarcodePDF417();                
                Barcode128 code128 = new Barcode128();
                code128.CodeType = Barcode.CODE128;
                code128.ChecksumText = true;
                code128.GenerateChecksum = true;
                code128.StartStopText = true;
                code128.Code = code;
               
                //Image img = code128.CreateDrawingImage(System.Drawing.Color.Black, System.Drawing.Color.White);
                //Image img = new Bitmap(code128.CreateDrawingImage(Color.Black, Color.White);
                //String text = code;
                //barcode.set .SetText(text);
                //Image img = pdf417.GetImage();
                //img.Alignment = Element.ALIGN_RIGHT;
                //img.ScalePercent(80f);
                //table.AddCell(code128);
            }
            doc.Add(table);
            doc.Close();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;

            // return File(workStream, "application/pdf");
            return File(workStream, "application/pdf", "GenerateETransportCard" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf");
        }

        public ActionResult GenerateBarcode(List<string> codes)
        {
            GenerateETransportCard tGen = new GenerateETransportCard();           
            return Json(tGen.getPreviewBarcode(codes), JsonRequestBehavior.AllowGet);            
        }
    }
}