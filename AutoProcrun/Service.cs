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
    [Serializable]
    public class Service
    {
        public void InstallService(string install, string serviceID, string displayName, string description, 
            string startup, string jvm, string classpath, string environment, string javaHome, 
            string serviceDirectory, string jvmOptions, string startMode, string startClass, string stdOutput, 
            string stdError, string pathFile, string latestPath, string startParams, string stopParams, string stopMode, string stopClass)
        {
            string[] settings = new string[20];
            string allsettings = "";
            
            #region Settings
            settings[0] = install + " //IS//" + serviceID;
            settings[1] = "--DisplayName \"" + displayName + "\"";
            settings[2] = "--Description \"" + description + "\"";
            settings[3] = "--Startup=" + startup;
            settings[4] = "--Install=" + install;
            settings[5] = "--Jvm=" + jvm;
            settings[6] = "--Classpath \"" + classpath + "\"";
            settings[7] = "--Environment=" + environment;
            settings[8] = "--JavaHome=" + javaHome;
            settings[9] = "--StartPath=" + serviceDirectory;
            settings[10] = "--JvmOptions=" + jvmOptions;
            settings[11] = "--StartMode=" + startMode;
            settings[12] = "--StartClass=" + startClass;
            settings[13] = "--StartParams=" + startParams;
            settings[14] = "--StopMode=" + stopMode;
            settings[15] = "--StopClass=" + stopClass;
            settings[16] = "--StopParams=" + stopParams;
            settings[17] = "--StdOutput=" + stdOutput;
            settings[18] = "--StdError=" + stdError;
            #endregion
            for (int i = 1; i < 20; i++) Console.WriteLine(settings[i]);
            for (int i = 1; i < 20; i++) allsettings += settings[i] + " ";

            allsettings = "cd JavaServices\\procrun & prunsrv update " + serviceID + " " + allsettings;

            CmdCommand.ExecuteCommandSync(settings[0]);
            CmdCommand.ExecuteCommandSync(allsettings);


        }
        public static void DeleteService(string serviceID)
        {
            Service service = LoadSettings(serviceID);
            string command = "cd JavaServices\\procrun & prunsrv delete " + service.serviceID;
            CmdCommand.ExecuteCommandSync(command);
            File.Delete(service.stdOutput);
            File.Delete(service.stdError);
            File.Delete(service.latestPath);
            File.Delete(service.classpath);
            File.Delete(service.settingsPath);

            Console.WriteLine("Deleted");
        }
        public static void SaveSettings(Service service)
        {  

            XmlSerializer formatter = new XmlSerializer(typeof(Service));

            service.settingsPath = service.serviceDirectory + "\\settings\\" + service.serviceID + "_settings.xml";

            using (FileStream fs = new FileStream(service.settingsPath, FileMode.OpenOrCreate)) { formatter.Serialize(fs, service); }
        }

        public static Service LoadSettings(string settingsPath)
        {
            Service service = new Service();

            XmlSerializer formatter = new XmlSerializer(typeof(Service));

            using (FileStream fs = new FileStream(settingsPath, FileMode.OpenOrCreate))
            {
                service = (Service)formatter.Deserialize(fs);
            }
            return service;
        }
        public string GetManifest(string classpath)
        {
            string text = "";
            using (ZipArchive archive = ZipFile.OpenRead(classpath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.Name == "MANIFEST.MF")
                    {
                        using (var reader = new StreamReader(entry.Open()))
                        {
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                text += line + " ";
                            }
                        }
                    }
                    break;
                }
            }
            return text;
        }
        public string GetVersion(string classpath)
        {
            string manifest = GetManifest(classpath);
            string version = "";
            string textfind = "Specification-Version";
            int index = manifest.IndexOf(textfind) + textfind.Length;
            for (int i = index + 2; i < manifest.Length; i++)
            {
                if (Convert.ToString(manifest[i]) == " ") break;
                version += manifest[i];
            }
            return version;
        }
        public string GetMainClass(string classpath)
        {
            string manifest = GetManifest(classpath);
            string mainClass = "";
            string textfind = "Main-Class";
            int index = manifest.IndexOf(textfind) + textfind.Length;
            for (int i = index + 2; i < manifest.Length; i++)
            {
                if (Convert.ToString(manifest[i]) == " ") break;
                mainClass += manifest[i];
            }
            return mainClass;
        }
        public Service(string pathFile, string serviceID, string displayName, string description, bool autoUpdate)
        {
            this.pathFile = pathFile;
            this.serviceID = serviceID;
            this.displayName = displayName;
            this.description = description;
            this.autoUpdate = autoUpdate;
            

            serviceDirectory = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "\\JavaServices";
            classpath = System.IO.Path.Combine(serviceDirectory, System.IO.Path.GetFileName(pathFile));
            latestPath = System.IO.Path.Combine(serviceDirectory + "\\latest_version\\", System.IO.Path.GetFileName(pathFile));

            File.Copy(pathFile, classpath, true);
            File.Copy(pathFile, latestPath, true);

            startup = "auto";
            jvmOptions = "\"-XX:+HeapDumpOnOutOfMemoryError\"";
            startMode = "jvm";
            startParams = "start";
            startClass = GetMainClass(classpath);
            stopMode = "jvm";
            stopParams = "stop";
            stopClass = startClass;
            install = "\"" + serviceDirectory + "\\procrun\\prunsrv.exe\"";
            stdOutput = serviceDirectory + "\\logs\\" + serviceID + "_stdout.log";
            stdError = serviceDirectory + "\\logs\\" + serviceID + "_stderr.log";
            settingsPath = serviceDirectory + "\\settings\\" + serviceID + "_settings.xml";


            jvm = "\"C:\\Program Files\\Java\\jdk-16\\bin\\server\\jvm.dll\"";
            javaHome = "\"C:\\Program Files\\Java\\jdk-16\"";
            environment = "\"PATH=C:\\Program Files\\Java\\jdk-16\\bin\"";


            InstallService(install, serviceID, displayName, description, startup, jvm, classpath, environment, javaHome, serviceDirectory, jvmOptions, startMode, startClass, stdOutput, stdError, pathFile, latestPath, startParams, stopParams, stopMode, stopClass);

        }

        public Service() { }

        public string 
            serviceID, 
            displayName, 
            description, 
            startup, 
            install, 
            jvm, 
            classpath, 
            environment, 
            javaHome, 
            startPath, 
            jvmOptions, 
            startMode, 
            stopMode,
            startClass, 
            stopClass,
            startParams,
            stopParams,
            stdOutput, 
            stdError;

        public string serviceDirectory;
        public string pathFile;
        public bool autoUpdate;
        public string latestPath;
        public string settingsPath;

    }
}
