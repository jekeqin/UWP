using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintServer.print
{
    public class PaperSet
    {

        private int _width;
        private int _height;
        private int _left;
        private int _right;
        private int _top;
        private int _bottom;

        public string print { get; set; }
        public string file { get; set; }
        public int width
        {
            get
            {
                return _width < 20 ? 20 : _width;
            }
            set
            {
                _width = value;
            }
        }
        public int height
        {
            get
            {
                return _height < 0 ? 0 : _height;
            }
            set
            {
                _height = value;
            }
        }

        public int left
        {
            get
            {
                return _left < 0 ? 0 : _left;
            }
            set
            {
                _left = value;
            }
        }
        public int top
        {
            get
            {
                return _top < 0 ? 0 : _top;
            }
            set
            {
                _top = value;
            }
        }
        public int right
        {
            get
            {
                return _right < 0 ? 0 : _right;
            }
            set
            {
                _right = value;
            }
        }
        public int bottom
        {
            get
            {
                return _bottom < 0 ? 0 : _bottom;
            }
            set
            {
                _bottom = value;
            }
        }

        public string fontName { get; set; }
        public int fontSize { get; set; }

        private Font _font = null;
        public Font font
        {
            get
            {
                if ( _font!=null )
                {
                    return _font;
                }
                var name = fontName == null ? "宋体" : fontName;
                var size = fontSize <= 0 ? 1 : fontSize;
                _font = new Font(name, size);
                return _font;
            }
        }

        public string color { get; set; }

        public Brush brush
        {
            get
            {
                return getBrush(color);
            }
        }

        public static Brush getBrush(string key)
        {
            if ( key!=null )
            {
                if (key.ToLower().CompareTo("red") == 0)
                {
                    return Brushes.Red;
                }
            }
            return Brushes.Black;
        }
    }
}
