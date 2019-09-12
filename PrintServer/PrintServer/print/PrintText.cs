using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintServer.print
{
    public class PrintText
    {
        private int _left;
        private int _top;
        private int _width;
        private int _height;
        public string text { get; set; }
        public int left {
            get {
                return _left < 0 ? 0 : _left;
            } set {
                this._left = value;
            }
        }
        public int top {
            get
            {
                return _top < 0 ? 0 : _top;
            }
            set
            {
                this._top = value;
            }
        }
        public int width {
            get
            {
                return _width < 0 ? 0 : _width;
            }
            set
            {
                this._width = value;
            }
        }
        public int height {
            get
            {
                return _height < 0 ? 0 : _height;
            }
            set
            {
                this._height = value;
            }
        }
        public string fontName{ get; set; }
        public int fontSize { get; set; }

        public Font font
        {
            get
            {
                if (fontSize <= 0)
                {
                    return null;
                }
                if (fontName==null || fontName.Length==0)
                {
                    fontName = "黑体";
                }
                return new Font(fontName,fontSize);
            }
        }

        public string color { get; set; }

        public Brush brush
        {
            get
            {
                if ( color==null )
                {
                    return Brushes.Transparent;
                }
                return PaperSet.getBrush(color);
            }
        }

        /// <summary>
        /// 类型，0文本，1横线
        /// </summary>
        public int type { get; set; }
    }
}
