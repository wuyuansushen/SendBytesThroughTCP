using System.Net.Sockets;
using System.Reflection;
using System.Text.Json;

namespace SendBytesThroughTCP
{
    public class Program
    {
        public class JsonSendBehaviour
        {
            public string? LoopCycle { get; set; }
            public string? IntervalUnitSecond { get; set; }
        }
        public class jsonConfig
        {
            public string? IP { get; set; }
            public string? Port { get; set; }
            public string? PackageByte { get; set; }
            public JsonSendBehaviour? SendBehaviour { get; set; }
        }
        public static void Main(string[] args)
        {
            try
            {
                string fileName = @"appsettings.json";
                //string jsonString = File.ReadAllText(fileName);
                //string assemblyDirPath =Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string? assemblyDirPath =Path.GetDirectoryName(typeof(jsonConfig).Assembly.Location);
                if (assemblyDirPath == null)
                {
                    return;
                }
                else { }
                string fullFileName = assemblyDirPath + Path.DirectorySeparatorChar + fileName;
                string jsonString = File.ReadAllText(fullFileName);
                jsonConfig? jsonFileConfig = JsonSerializer.Deserialize<jsonConfig>(jsonString);
                if (jsonFileConfig != null)
                {
                    string? HesStringInfo = jsonFileConfig.PackageByte;
                    if(HesStringInfo == null) { return; }
                    else { }
                    byte[] bytesList = new byte[HesStringInfo.Length / 2];
                    for (int i = 0; i < HesStringInfo.Length; i += 2)
                    {
                        bytesList[i / 2] = Convert.ToByte(HesStringInfo.Substring(i, 2), 16);
                    }
                    TcpClient tcpClient = new TcpClient(jsonFileConfig.IP, Convert.ToInt32(jsonFileConfig.Port));
                    NetworkStream networkStream = tcpClient.GetStream();
                    if (jsonFileConfig.SendBehaviour.LoopCycle == @"1")
                    {
                        do
                        {
                            networkStream.Write(bytesList, 0, bytesList.Length);
                            Thread.Sleep(Convert.ToInt32(jsonFileConfig.SendBehaviour.IntervalUnitSecond) * 1000);
                            Console.WriteLine($"{DateTime.Now} Sent Message.");
                        }
                        while (true);
                    }
                    else
                    {
                        networkStream.Write(bytesList, 0, bytesList.Length);
                        Console.WriteLine($"{DateTime.Now} Sent Message (Only one time).");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error:{ex.Message}\r\n{ex.StackTrace}");
            }
        }
    }

}