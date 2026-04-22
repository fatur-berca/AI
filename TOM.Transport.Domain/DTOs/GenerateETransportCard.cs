using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace TOM.Transport.Domain.DTOs
{
    public class GenerateETransportCard
    {
        const double TO_PIXEL_MULTIPLIER = 37.795275590551;
        public string getPreviewBarcode(string codes)
        {
            string ret = "";
            System.Drawing.Color bgColor = System.Drawing.ColorTranslator.FromHtml("#F1F4F7");
            int canvas_width = CentimeterToPixel(7);
            int canvas_height = CentimeterToPixel(4.2);
            System.Drawing.Bitmap canvas = new System.Drawing.Bitmap(canvas_width, canvas_height);
            System.Drawing.Graphics gr = System.Drawing.Graphics.FromImage(canvas);
            gr.Clear(bgColor);

            int multiplyX = 0, multiplyY = 0, spaceRow = 0;
                string code = codes;
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
                }                
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                im.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                byte[] byteImage = ms.ToArray();
                ret = Convert.ToBase64String(byteImage);
            
            return ret;
        }
        
        private int CentimeterToPixel(double Centimeter)
        {
            return (int)(Centimeter * TO_PIXEL_MULTIPLIER);
        }
    }
}
