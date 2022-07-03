using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml;

namespace VT.main;

class VTmain
{
    public static string GetAddressIP()
    {
        string AddressIP = string.Empty;
        foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
        {
            if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
            {
                AddressIP = _IPAddress.ToString();
            }
        }
        return AddressIP;
    }

    static void SendMsg()
    {
        string message = null;
        byte[] messageBytes;
        try
        {
            message = "Server name is " + Servername;
            messageBytes = Encoding.UTF8.GetBytes(message);
            stream.Write(messageBytes, 0, messageBytes.Length);
            while (message != "exit")
            {
                message = Console.ReadLine();
                messageBytes = Encoding.UTF8.GetBytes(message);
                stream.Write(messageBytes, 0, messageBytes.Length);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.ReadKey();
        }
    }

    static void ReceiveMsg()
    {
        byte[] buffer = new byte[1024]; 
        int count = 0;
        try
        {
            while ((count = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                Console.WriteLine($"Client:{Encoding.UTF8.GetString(buffer, 0, count)}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.ReadKey();
        }

    }

    static void Readserverxml()
    {
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load("data/server.xml");
        XmlElement root = xmlDocument.DocumentElement;
        foreach (XmlNode node in root.ChildNodes)
        {
            if (node.Name == "ip_address")
            {
                ip = node.InnerText;
            }
            else if (node.Name == "server_port")
            {
                port = Convert.ToInt32(node.InnerText);
            }
            else if (node.Name == "server_name")
            {
                Servername = node.InnerText;
            }
        }
    }
    public static string ip = VTmain.GetAddressIP();
    public static int port = 2233;
    static TcpClient tcpClient;
    static NetworkStream stream;
    static string Servername;
    public static  void Main(string[] args)
    {
        Readserverxml();
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        TcpListener tcpListener = new TcpListener(endPoint);
        tcpListener.Start();
        Console.WriteLine($"Server has been started on {ip}:{port}");
        TcpClient tcpClient = tcpListener.AcceptTcpClient();
        Console.WriteLine("Client has been connected...");
        stream = tcpClient.GetStream();
        Thread Receive = new Thread(ReceiveMsg);
        Receive.Start();
        Thread Send = new Thread(SendMsg);
        Send.Start();
    }
}