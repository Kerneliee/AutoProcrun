using System;


namespace AutoProcrun
{
    class Program
    {
        static void Main(string[] args)
        {
            string pathFile, serviceID, displayName, description;

            bool autoUpdate;

            pathFile = @"D:\test\javatest.jar";
            serviceID = "AJavaServiceTest";
            displayName = "AJavaServiceTest";
            description = "default";
            autoUpdate = true;

            Service service = new Service(pathFile, serviceID, displayName, description, autoUpdate);

            //service.SaveSettings(service);

            //service.InstallService();

            Service.DeleteService(service.settingsPath);

            Console.ReadKey();

        }
    }
}
