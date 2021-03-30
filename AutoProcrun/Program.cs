using java.util.jar;
using sun.misc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;


namespace AutoProcrun
{
    class Program
    {
        static void Main(string[] args)
        {
            string pathFile, serviceID, displayName, description;

            bool autoUpdate;

            pathFile = @"D:\test\javatest.jar";
            serviceID = "Atechie";
            displayName = "Atechie";
            description = "default";
            autoUpdate = false;

            //Service service = new Service(pathFile, serviceID, displayName, description, autoUpdate);

            //Service.SaveSettings(service);

            //Service.DeleteService(serviceID);

            //if (service.GetVersion(service.latestPath) != service.GetVersion(service.classpath))
            //{
            //    System.IO.File.Copy(service.latestPath, service.classpath, true);
            //    string command = "cd JavaServices\\procrun & prunsrv stop " + serviceID;
            //    CmdCommand.ExecuteCommandSync(command);
            //    command = "cd JavaServices\\procrun & prunsrv start " + serviceID;
            //    CmdCommand.ExecuteCommandSync(command);
            //    Console.WriteLine("zamena");
            //}



            Console.ReadKey();

        }
    }
}
