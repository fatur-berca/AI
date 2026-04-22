using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace hms_tom_dev.Models.Transport
{
    public class GenerateETransportCard
    {
        const double TO_PIXEL_MULTIPLIER = 37.795275590551;
        public List<string> getPreviewBarcode(List<string> codes)
        {
            List<string> ret = new List<string>();
            System.Drawing.Color bgColor = System.Drawing.ColorTranslator.FromHtml("#F1F4F7");
            int canvas_width = CentimeterToPixel(7);
            int canvas_height = CentimeterToPixel(4.2);
            System.Drawing.Bitmap canvas = new System.Drawing.Bitmap(canvas_width, canvas_height);
            System.Drawing.Graphics gr = System.Drawing.Graphics.FromImage(canvas);
            gr.Clear(bgColor);

            int multiplyX = 0, multiplyY = 0, spaceRow = 0;
            for (int i = 1; i <= codes.Count; i++)
            {
                string code = codes[i - 1];
                if (code.Trim() == "")
                {
                    ret.Add("");
                    continue;
                }
                Barcode128 code128 = new Barcode128();
                code128.CodeType = Barcode.CODE128;
                code128.ChecksumText = true;
                code128.GenerateChecksum = true;
                code128.StartStopText = true;
                code128.Code = code;
                System.Drawing.Image im = code128.CreateDrawingImage(System.Drawing.Color.Black, System.Drawing.Color.White);

                int width = CentimeterToPixel(2.6);
                int height = CentimeterToPixel(1.5);
                System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(width, height);

                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp))
                {
                    g.Clear(bgColor);
                    g.DrawImage(im, 0, 0, im.Width, im.Height);
                    /*
                    System.Drawing.Font font = new System.Drawing.Font("Helvetica-Bold", 8);
                    float stringWidth = g.MeasureString(code, font).Width;
                    
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    g.DrawString(code,
                                font, new System.Drawing.SolidBrush(System.Drawing.Color.Black),
                                (bmp.Width / 2) - (stringWidth / 2), bmp.Height - 25);
                    */
                }
                /*
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(canvas))
                {
                    int yPos = bmp.Height * multiplyY;
                    multiplyX = (i % 2) - 1;
                    multiplyX = multiplyX < 0 ? 2 : multiplyX;
                    g.DrawImage(bmp, new System.Drawing.PointF(((75) * multiplyX), yPos + spaceRow));
                    multiplyY += (i % 2 == 0 ? 1 : 0);
                    spaceRow = multiplyY > 0 ? 1 : 0;
                }
                */
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                im.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                byte[] byteImage = ms.ToArray();
                ret.Add(Convert.ToBase64String(byteImage));
            }

            return ret;
        }

        public byte[] generateETransportCardPdf(List<string> codes)
        {
            Document doc = new Document(PageSize.A4, 0f, 0f, 0f, 0f);
            using (var ms = new MemoryStream())
            {
                PdfWriter writer = PdfWriter.GetInstance(doc, ms);
                string imagesDirectory = System.Web.HttpContext.Current.Server.MapPath("../assets/images/");

                MemoryStream mStreamImage = new MemoryStream(System.IO.File.ReadAllBytes(imagesDirectory + "truck_barcode.png"));
                System.Drawing.Image imgTruck = System.Drawing.Image.FromStream(mStreamImage);
                mStreamImage.Close();

                int canvas_width = CentimeterToPixel(21);
                int canvas_height = CentimeterToPixel(29.7);
                System.Drawing.Bitmap canvas = new System.Drawing.Bitmap(canvas_width, canvas_height);
                System.Drawing.Graphics gr = System.Drawing.Graphics.FromImage(canvas);
                gr.Clear(System.Drawing.Color.White);

                int multiplyX = 0, multiplyY = 0, spaceRow = 0, spaceCol = 0, idx = 0;
                for (int i = 1; i <= codes.Count; i++)
                {
                    string code = codes[i - 1];
                    if (code.Trim() == "") continue;

                    code = code.Replace(" ", "-").ToUpper();
                    idx++; //untuk acuan posisi barcode berikutnya
                    Barcode128 code128 = new Barcode128();
                    code128.CodeType = Barcode.CODE128;
                    code128.ChecksumText = true;
                    code128.GenerateChecksum = true;
                    code128.StartStopText = true;
                    code128.Code = code;
                    System.Drawing.Image im = code128.CreateDrawingImage(System.Drawing.Color.Black, System.Drawing.Color.White);

                    int width = CentimeterToPixel(5.2);
                    int height = CentimeterToPixel(7.2);
                    System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(width, height);

                    using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp))
                    {
                        var title = "e-Transport Card";
                        g.Clear(System.Drawing.Color.White);
                        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                        System.Drawing.Font font = new System.Drawing.Font("Helvetica-Bold", 16, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
                        float stringWidth = g.MeasureString(title, font).Width;
                        g.DrawString(title,
                                   font, new System.Drawing.SolidBrush(System.Drawing.Color.Black),
                                   (bmp.Width / 2) - (stringWidth / 2), 20);

                        g.DrawImage(imgTruck, new System.Drawing.Rectangle(((bmp.Width / 2) - (imgTruck.Width / 2)), 40, imgTruck.Width, imgTruck.Height - 15));

                        g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Black, 1),
                                        new System.Drawing.Rectangle(0, 0, bmp.Width - 1, bmp.Height - 1));

                        g.DrawImage(im, (bmp.Width / 2) - (im.Width / 2), bmp.Height - ((im.Height * 2) + 30), im.Width, (im.Height * 2));

                        font = new System.Drawing.Font("Helvetica-Bold", 10, System.Drawing.FontStyle.Bold);

                        title = code;
                        stringWidth = g.MeasureString(title, font).Width;
                        g.DrawString(title,
                                    font, new System.Drawing.SolidBrush(System.Drawing.Color.Black),
                                    (bmp.Width / 2) - (stringWidth / 2), bmp.Height - 25);
                    }

                    using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(canvas))
                    {
                        int yPos = bmp.Height * multiplyY;
                        multiplyX = (idx % 6) - 1;
                        multiplyX = multiplyX < 0 ? 2 : multiplyX;

                        //jika kol > 1 maka inc + 1, untuk beri space antar col
                        spaceCol += idx % 3 == 1 ? 0 : 1;
                        //jika kol 1 maka set spaceCol = 0
                        spaceCol = idx % 3 == 1 ? 0 : spaceCol;

                        g.DrawImage(bmp, new System.Drawing.PointF((bmp.Width * multiplyX) + spaceCol, yPos + spaceRow));
                        multiplyY += (idx % 3 == 0 ? 1 : 0);
                        spaceRow = multiplyY;
                    }
                }
                doc.Open();
                doc.Add(iTextSharp.text.Image.GetInstance(canvas, System.Drawing.Imaging.ImageFormat.Bmp));
                doc.Close();
                
                return ms.ToArray();
            }
            
            //return(ms, "application/pdf", "GenerateETransportCard" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf");
        }

        private int CentimeterToPixel(double Centimeter)
        {
            return (int)(Centimeter * TO_PIXEL_MULTIPLIER);
        }
    }
}