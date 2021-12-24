using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace dns
{
    class Program
    {
        static readonly byte[] answer = new byte[] { 0xc0, 0x0c, 0x00, 0x01, 0x00, 0x01, 0x00, 0x20, 0xf5, 0x80, 0x00, 0x04, 0x7f, 0x00, 0x00, 0x01 };

        static void Main(string[] args)
        {
            try
            {
                var client = new UdpClient(53);
                IPEndPoint RemoteIpEndPoint = null;

                while (true)
                {
                    byte[] bytes = client.Receive(ref RemoteIpEndPoint);
                    if (bytes[5] != 1) continue;
                    bytes[2] = 0x81; // response
                    bytes[7] = 0x01; // answer RRs 1

                    var name = GetName(bytes);
                    if (name != "www.ros-bot.com" && name != "ros-bot.com")
                        continue;

                    byte[] response = new byte[bytes.Length + answer.Length];
                    Array.Copy(bytes, response, bytes.Length);
                    Array.Copy(answer, 0, response, bytes.Length, answer.Length);
                    client.Send(response, response.Length, RemoteIpEndPoint);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString() + "\n" + ex.Message);
            }
            Console.ReadKey();
        }

        static string GetName(byte[] bytes)
        {
            string result = string.Empty;
            int offset = 12;
            int length = bytes[offset];
            while (length != 0)
            {
                result += Encoding.UTF8.GetString(bytes, ++offset, length);
                length = bytes[offset += length];
                if (length != 0)
                    result += ".";
            }
            return result;
        }
    }
}
