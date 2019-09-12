using Newtonsoft.Json;
using PrintServer.print;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PrintServer.server
{
    public class HttpServer
    {

        private HttpListener ws = null;

        public bool Start(string port)
        {
            CmdUtil.registPort(port);
            var url = "http://+:"+port+"/";
            ws = new HttpListener();
            try
            {
                ws.Prefixes.Add(url);
                ws.Start();
                ws.BeginGetContext(wsMessageHandle, null);   // 监听消息

                LogUtil.Info(String.Format("HttpListener.Start: {0}", url));
                return true;
            }
            catch(Exception e)
            {
                ws = null;
                LogUtil.Error(String.Format("HttpListener.Start.Error: {0}", e.Message));
                MessageBox.Show(e.Message, "服务启动失败", MessageBoxButton.OK);
                return false;
            }
        }

        public bool Stop()
        {
            if ( ws!=null )
            {
                if (ws.Prefixes.Count > 0)
                {
                    ws.Prefixes.Clear();
                }
                ws = null;
                LogUtil.Info("HttpListener.Stop");
            }
            return true;
        }

        private void wsMessageHandle(IAsyncResult msg)
        {
            ws.BeginGetContext(wsMessageHandle, null);   // 已监听到消息，再次启动监听

            var resultJson = "{\"code\":200,\"msg\":\"success\"}"; ;

            var client = ws.EndGetContext(msg);
            var req = client.Request;
            var resp = client.Response;
            resp.AppendHeader("Access-Control-Allow-Headers", "Content-Type");
            resp.AppendHeader("Access-Control-Allow-Origin", req.Headers.Get("Origin"));
            //resp.AppendHeader("Access-Control-Allow-Credentials", "true");
            resp.ContentType = "application/json;charset=utf-8";
            if (req.InputStream != null && req.HttpMethod == "POST")
            {
                StreamReader reader = new StreamReader(req.InputStream, Encoding.UTF8);
                string json = reader.ReadToEnd();
                LogUtil.Info(String.Format("Request:{0}:{1}", req.Url, json));
                resultJson = this.doPrint(json);
            }

            resp.StatusCode = 200;
            resp.StatusDescription = "OK";
            using (var stream = resp.OutputStream)
            {
                byte[] exec = Encoding.UTF8.GetBytes(resultJson);
                stream.Write(exec, 0, exec.Length);
                stream.Flush();
            }
        }

        private string doPrint(string json)
        {
            if (json == null || json.Length==0)
            {
                return "{\"code\":0,\"msg\":\"no params\"}";
            }
            try
            {
                PrintData data = JsonConvert.DeserializeObject<PrintData>(json);
                if (data.set != null && data.body != null)
                {
                    Console.WriteLine("print.name:{0}", data.set.print);
                    new PrintUtil(data.set, data.body).doPrint();
                    return "{\"code\":200,\"msg\":\"success\"}";
                }
                return "{\"code\":0,\"msg\":\"no params\"}";
            }
            catch(Exception e)
            {
                Console.WriteLine("json error:{0}",e.Message);
                return "{\"code\":500,\"msg\":\""+e.Message+"\"}";
            }
        }
    }
}
