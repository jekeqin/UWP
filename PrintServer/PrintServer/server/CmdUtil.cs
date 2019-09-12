using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintServer.server
{
    public class CmdUtil
    {
        public static void registPort(string port)
        {
            List<string> list = new List<string>();
            list.Add("netsh http delete urlacl url=http://+:" + port + "/");
            list.Add("netsh http add urlacl url=http://+:"+port+"/ user=Everyone");
            //list.Add();
            exec(list);
        }
        public static void exec(List<string> commandlist)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "cmd";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.Verb = "runas";  // 提升权限
            try
            {
                proc.Start();
                StreamWriter sw = proc.StandardInput;
                foreach (string command in commandlist)
                {
                    sw.WriteLine(@command);
                }
                sw.WriteLine("exit");
                string result = proc.StandardOutput.ReadToEnd();
                proc.WaitForExit();
                proc.Close();
                LogUtil.Info(String.Format("Cmd.Result:{0}", result));
            }
            catch(Exception e)
            {
                LogUtil.Error(String.Format("Cmd.Error:{0}", e.Message));
            }

        }
    }
}
