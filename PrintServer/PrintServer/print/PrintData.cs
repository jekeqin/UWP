using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintServer.print
{
    public class PrintData
    {
        public PaperSet set { get; set; }
        public List<PrintText> body { get; set; }
    }
}
