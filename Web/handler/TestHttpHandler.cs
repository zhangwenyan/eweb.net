using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Web;

namespace Web.handler
{
    public class TestHttpHandler : IHttpAsyncHandler
    {
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            Thread.Sleep(10000);
            context.Response.Write("bb");
            return new MyAsyncResult(context,cb,extraData);
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            //throw new NotImplementedException();
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Write("ProcessRequest");
//            throw new NotImplementedException();
        }
    }

    public class MyAsyncResult : IAsyncResult
    {
        public HttpContext contex;
        public AsyncCallback cb;
        public object extraData;

        public bool isCompleted = false;

        public MyAsyncResult(HttpContext contex, AsyncCallback cb, object extraData)
        {
            this.contex = contex;
            this.cb = cb;
            this.extraData = extraData;
        }

        public void send(string resultStr)
        {
            this.contex.Response.Output.Write(resultStr);
        }

        public object AsyncState
        {
            get { return null; }
        }

        public System.Threading.WaitHandle AsyncWaitHandle
        {
            get { return null; }
        }

        public bool CompletedSynchronously
        {
            //在网络连接或者流读取中，这里需要设置为True，否则前台是不能显示接收数据的。
            get { return true; }
        }

        public bool IsCompleted
        {
            get { return isCompleted; }
        }
    }

    #region 异步执行对象
    public static class myAsyncFunction
    {
        private static string resultResponse = string.Empty;  //获取客户端数据
        private static TcpListener tcpListener;

        private static MyAsyncResult asyncResult;  //通知回传对象

        // 把一个异步的请求对象传入以便于供操作
        public static void Init(MyAsyncResult result, int port)
        {
            asyncResult = result;
            if (tcpListener == null)
            {
                try
                {
                    IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("192.168.0.100"), port);
                    tcpListener = new TcpListener(endPoint);
                    tcpListener.Start();
                }
                catch (System.Net.Sockets.SocketException ex)
                {
                    tcpListener = null;
                    throw new Exception(ex.Message);
                }
            }
        }

        //接收客户端数据并将数据打印到前台
        public static void AcceptClients()
        {
            TcpClient client = tcpListener.AcceptTcpClient();
            byte[] bufferResult = new byte[1024];
            NetworkStream readStream = client.GetStream();
            readStream.Read(bufferResult, 0, bufferResult.Length);
            resultResponse = System.Text.Encoding.Default.GetString(bufferResult);

        //    Logger.WriteLog("Receiving TCP Client Message: " + resultResponse);
            asyncResult.send(resultResponse);
        }
    }
    #endregion

    #region 异步执行请求
    public class AsyncSocketHandler : IHttpAsyncHandler
    {
        private static object obj = new object();

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            lock (obj)
            {
                int port = Int32.Parse(context.Request.QueryString["port"].ToString());
                MyAsyncResult asyncResult = new MyAsyncResult(context, cb, extraData);  //实例
                myAsyncFunction.Init(asyncResult, port); //接收所有传入的异步对象
                myAsyncFunction.AcceptClients();   //处理所有传入的异步对象
                asyncResult.isCompleted = true;
                return asyncResult;
            }
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            MyAsyncResult asyncResult = result as MyAsyncResult;
            if (asyncResult != null)
            {
                asyncResult.send(string.Empty);
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        { }
    }
    #endregion
}