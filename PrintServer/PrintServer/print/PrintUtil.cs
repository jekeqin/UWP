using PrintServer.server;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintServer.print
{
    public class PrintUtil
    {
        private PaperSet set = null;
        private List<PrintText> textlist = null;

        public PrintUtil()
        {

        }

        public PrintUtil(PaperSet set, List<PrintText> textlist)
        {
            this.set = set;
            this.textlist = textlist;
        }


        /// <summary>
        /// 获取所有打印机名称
        /// </summary>
        /// <returns></returns>
        public List<string> Prints()
        {
            List<string> list = new List<string>();
            foreach (string name in PrinterSettings.InstalledPrinters)
            {
                list.Add(name);
            }
            return list;
        }

        public void doPrint()
        {
            if ( set==null || textlist==null || textlist.Count==0 )
            {
                return;
            }
            LogUtil.Info(String.Format("Document.Create.{0}",set.file));

            PrintDocument document = new PrintDocument();

            PrinterSettings print = document.PrinterSettings;
            print.PrinterName = set.print;
            print.PrintRange = PrintRange.CurrentPage;
            if( set.file!=null)
            {
                print.PrintFileName = set.file;
                document.DocumentName = set.file;
            }

            PageSettings setting = document.DefaultPageSettings;
            setting.Landscape = false;  // 横向，纵向
            setting.PaperSize = new PaperSize("customer", mmToInch(set.width), mmToInch(set.height));
            setting.Margins = new Margins(mmToInch(set.left), mmToInch(set.right), mmToInch(set.top), mmToInch(set.bottom));

            document.PrintPage += new PrintPageEventHandler(this.onStartPage);
            document.Print();

            LogUtil.Info(String.Format("Document.Print.{0}", set.file));
        }

        private void onStartPage(object sender,PrintPageEventArgs e)
        {
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Near;
            format.LineAlignment = StringAlignment.Near;
            int lastTop = 0;
            foreach (PrintText text in textlist)
            {
                if (text.type == 1)
                {
                    e.Graphics.DrawLine(Pens.Black, text.left, text.top, text.left + text.width, text.top + text.height);
                    //LogUtil.Info(String.Format("Document.{0}.Add.Line",set.file));
                    lastTop = text.top;
                }
                else
                {
                    Font font = text.font == null ? set.font : text.font;
                    SizeF realSize = e.Graphics.MeasureString(text.text, font, text.width,format);  // 测量
                    lastTop = text.top + (int)realSize.Height;
                    RectangleF f = new RectangleF(text.left, text.top, text.width, text.height);

                    e.Graphics.DrawString(text.text
                        , font
                        , text.brush == Brushes.Transparent ? set.brush : text.brush
                        , f
                        , format);

                    //LogUtil.Info(String.Format("Document.{0}.Add.Text:{1}", set.file, text.text));
                }
            }
            if (set.height ==0 )
            {   // 不限制高度时增加底部横线
                e.Graphics.DrawLine(Pens.Black, 0, (int)(lastTop + set.bottom), 1, (int)(lastTop + set.bottom));
            }
            e.HasMorePages = false;
        }

        /// <summary>
        /// 毫米转化为 100英寸，打印机是以 1/100英寸为单位
        /// </summary>
        /// <param name="mm"></param>
        /// <returns></returns>
        public int mmToInch(int mm)
        {
            return (int)(mm * 100.0f / 25.4f);
        }

    }

}
