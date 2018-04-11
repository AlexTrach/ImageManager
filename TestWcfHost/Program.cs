using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using ImagesWcfService;

namespace TestWcfHost
{
    class Program
    {
        static void Main()
        {
            using (ServiceHost host = new ServiceHost(typeof(ImagesService)))
            {
                host.Open();
                Console.WriteLine("*****Host started!*****");
                Console.WriteLine("Press ENTER to terminate the host");
                Console.ReadLine();
            }
        }
    }
}
