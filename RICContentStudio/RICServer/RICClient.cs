using System;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Net.NetworkInformation;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace RICContentStudio
{
    public static class RICClient
    {
        static TcpClient Client { get; set; }
        static NetworkStream Stream { get { var stream = Client.GetStream(); stream.ReadTimeout = stream.WriteTimeout = 5000; return stream; } }
        static StreamReader Reader { get { return new StreamReader(Stream); } }

        static StreamWriter Writer { get { return new StreamWriter(Stream) { AutoFlush = true }; } }

        static Thread Manager { get; set; }

        public static bool Connected { get { return Client != null && Client.Connected; } }

        public delegate void InitCompletedDelegate(bool isAdmin);

        public static event InitCompletedDelegate InitCompleted;
        public static bool IsAdmin { get; set; }
        public static void Init(string key)
        {
            if (Manager == null || Manager.ThreadState == ThreadState.Stopped)
                // Analysis disable once FunctionNeverReturns
                (Manager = new Thread(() =>
               {
                   while (true)
                       try
                       {
                           if (!Connected)
                           {
                               Client = new TcpClient(EnvironmentVariables.RICServerHost, EnvironmentVariables.RICServerPort);
                               string hash = (from nic in NetworkInterface.GetAllNetworkInterfaces()
                                              where nic.OperationalStatus == OperationalStatus.Up
                                              select nic.GetPhysicalAddress().ToString()).FirstOrDefault();
                               Writer.WriteWithEndBlock(string.Format("hash={0}", hash));
                               Reader.ReadToEndBlock();
                               Writer.WriteWithEndBlock(string.Format("key={0}", EncryptString(key, hash)));
                               IsAdmin = Reader.ReadToEndBlock().EndsWith("admin");
                               InitCompleted?.Invoke(IsAdmin);
                           }
                           else Thread.Sleep(100);
                       }
                       catch (Exception e)
                       {
                           if (!(e is SocketException || e is IOException)) throw;
                       }
               })).Start();
        }
        public static string EncryptString(string data, string hash)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(StringEncryptor.PackData(StringEncryptor.Encrypt(data, StringEncryptor.GetKey(hash)))));
        }

        public static RICResponse RequestData(RICRequest request)
        {
            if (Connected)
                try
                {
                    Writer.WriteWithEndBlock(request.ToString());
                    return RICResponse.Parse(Reader.ReadToEndBlock().Replace(((char)0).ToString(), ""));
                }
                catch (Exception e)
                {
                    if (e is SocketException || e is IOException) return RICResponse.Text("CONNECTION_ERROR");
                    else throw;
                }
            else return RICResponse.Text("CONNECTION_ERROR");
        }

        public static void Stop()
        {
            if (Manager != null)
            {
                Manager.Abort();
                Manager = null;
            }
            if (Client != null)
            {
                Client.Close();
                Client = null;
            }
        }

        public static string ReadToEndBlock(this StreamReader reader)
        {
            string result = "";
            string tmp;
            while ((tmp = reader.ReadLine()) != "---END---")
                result += tmp;
            return result.Replace(@"\n", "\n");
        }

        public static void WriteWithEndBlock(this StreamWriter writer, string text)
        {
            writer.WriteLine("{0}\n---END---\n", text.Replace("\n", @"\n"));
        }

    }
}

