using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WCFApp
{
    [ServiceContract]
    public interface IMyObject
    {
        [OperationContract]
        string GetCommandString(int i);
    }

    public class MyObject : IMyObject
    {
        public string GetCommandString(int i)
        {
            switch (i)
            {
                case 1:
                    return "Start of - from server";

                case 0:
                    return "End of - from server";

                default:
                    return "I have a " + i;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(MyObject), new Uri("http://localhost:1050/TestService"));
            host.AddServiceEndpoint(typeof(IMyObject), new BasicHttpBinding(), "");
            host.Open();
            Console.WriteLine("Server started");
            Console.ReadLine();

            host.Close();

            Uri tcpUri = new Uri("http://localhost:1050/TestService");
            EndpointAddress address = new EndpointAddress(tcpUri);
            BasicHttpBinding binding = new BasicHttpBinding();
            ChannelFactory<IMyObject> factory = new ChannelFactory<IMyObject>(binding, address);
            IMyObject service = factory.CreateChannel();

            Console.WriteLine("Show must go on? - from client");
            Console.WriteLine(service.GetCommandString(1));
            Console.WriteLine(service.GetCommandString(2));
            Console.WriteLine(service.GetCommandString(20));
            Console.WriteLine(service.GetCommandString(1562492));
            Console.WriteLine(service.GetCommandString(0));
            Console.ReadLine();
        }
    }
}
