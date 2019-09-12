using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PrintServer.server
{
    public class UACUtil
    {
        public bool isRunsAdmin()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public void requestAdmin()
        {
            if ( isRunsAdmin() )
            {
                return;
            }
            ProcessStartInfo process = new ProcessStartInfo();

        }
    }
}
