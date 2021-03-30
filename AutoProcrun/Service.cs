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
        //Установка сервиса 
        public void InstallService()
        {
            string[] settings = new string[20];
            string allsettings = "";
            
            //Здесь собираются настройки для сервиса, которые выполнятся в cmd
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
            for (int i = 0; i < 20; i++) Console.WriteLine(settings[i]);
            for (int i = 1; i < 20; i++) allsettings += settings[i] + " ";

            // jar сервис сохраняется в главную папку
            File.Copy(pathFile, classpath, true);
            File.Copy(pathFile, latestVersionPath, true);

            allsettings = "cd JavaServices\\procrun & prunsrv update " + serviceID + " " + allsettings;
            CmdCommand.ExecuteCommandSync(settings[0]);
            CmdCommand.ExecuteCommandSync(allsettings);
        }
        //Удалется сам сервис, файлы логов, настройки и jar файлы
        public static void DeleteService(string settingsPath)
        {
            Service service = LoadSettings(settingsPath);
            string command = "cd JavaServices\\procrun & prunsrv delete " + service.serviceID;
            CmdCommand.ExecuteCommandSync(command);
            File.Delete(service.stdOutput);
            File.Delete(service.stdError);
            File.Delete(service.latestVersionPath);
            File.Delete(service.classpath);
            File.Delete(service.settingsPath);

            Console.WriteLine("Deleted");
        }
        //Save сохраняет экземпляр класса Service в XML файл
        public void SaveSettings(Service service)
        {  

            XmlSerializer formatter = new XmlSerializer(typeof(Service));

            service.settingsPath = service.serviceDirectory + "\\settings\\" + service.serviceID + "_settings.xml";

            using (FileStream fs = new FileStream(service.settingsPath, FileMode.OpenOrCreate)) { formatter.Serialize(fs, service); }
        }
        //Load получает экземпляр класса Service из XML файла
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
        //Открывает jar файл в виде архива и находит файл manifest.mf и возвращает весь текст в string переменную
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
        // Из manifest.mf находит Specification-Version
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
        //Из файла manifest.mf находист исполняемый класс
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
            #region Default Settings
            this.pathFile = pathFile;
            this.serviceID = serviceID;
            this.displayName = displayName;
            this.description = description;
            this.autoUpdate = autoUpdate;
            
            serviceDirectory = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "\\JavaServices";
            classpath = System.IO.Path.Combine(serviceDirectory, System.IO.Path.GetFileName(pathFile));
            latestVersionPath = System.IO.Path.Combine(serviceDirectory + "\\latest_version\\", System.IO.Path.GetFileName(pathFile));
            startup = "auto";
            jvmOptions = "\"-XX:+HeapDumpOnOutOfMemoryError\"";
            startMode = "jvm";
            startParams = "start";
            startClass = GetMainClass(pathFile);
            stopMode = "jvm";
            stopParams = "stop";
            stopClass = startClass;
            install = "\"" + serviceDirectory + "\\procrun\\prunsrv.exe\"";
            stdOutput = serviceDirectory + "\\logs\\" + serviceID + "_stdout.log";
            stdError = serviceDirectory + "\\logs\\" + serviceID + "_stderr.log";
            settingsPath = serviceDirectory + "\\settings\\" + serviceID + "_settings.xml";
            #endregion

            //Здесь необходимо указать свой путь к jdk
            javaHome = "\"C:\\Program Files\\Java\\jdk-16\"";
            environment = "\"PATH=C:\\Program Files\\Java\\jdk-16\\bin\"";
            jvm = "\"C:\\Program Files\\Java\\jdk-16\\bin\\server\\jvm.dll\"";
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
        public string latestVersionPath;
        public string settingsPath;

    }
}
