using System.IO;
using System.ServiceProcess;


namespace Watching_Dog
{
    public partial class WatchingDogService : ServiceBase
    {

        FileSystemWatcher fileSystemWatcher;

        public WatchingDogService()
        {
            InitializeComponent();
        }



        protected override void OnStart(string[] args)
        {
            //EventLog.WriteEntry("Watching Dog Service Started");
            fileSystemWatcher = new FileSystemWatcher("D:\\abcd")
            {
                EnableRaisingEvents = true,
                IncludeSubdirectories = true
            };

            fileSystemWatcher.Created += DirectoryChanged;
            fileSystemWatcher.Deleted += DirectoryChanged;
            fileSystemWatcher.Changed += DirectoryChanged;
            fileSystemWatcher.Renamed += DirectoryChanged;

            var serviceLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location); // c:\\Program Files\\Folder To Watch\\Watching_Dog.exe (因為其他人的檔案處會不同,用此得到現在位置)
            File.AppendAllText($"{serviceLocation}\\log.txt", "service started!\n"); //本來沒有這個log.txt,這裡生成,並把msg的東西寫入

        }

        private void DirectoryChanged(object sender, FileSystemEventArgs e)
        {
            var msg = $"{e.ChangeType} - {e.FullPath} {System.Environment.NewLine}";
            var serviceLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location); // c:\\Program Files\\Folder To Watch\\Watching_Dog.exe (因為其他人的檔案處會不同,用此得到現在位置)
            File.AppendAllText($"{serviceLocation}\\log.txt", msg); //本來沒有這個log.txt,這裡生成,並把msg的東西寫入

            //char lastAddr = e.FullPath[-1];//取最後一個字元,會出錯
          

            //Directory.CreateDirectory(@"D:\ForWatchingDog\changed");
            // File.AppendAllText($"{serviceLocation}\\log.txt", "dir created!\n");
            if (!System.IO.Directory.Exists(@"D:\ForWatchingDog\changed"))//如果不存在此檔案就創一個
            {
                Directory.CreateDirectory(@"D:\ForWatchingDog\changed");
                File.AppendAllText($"{serviceLocation}\\log.txt", "dir created!\n");
            }

            string filename = Path.GetFileName(e.FullPath);
            string fileLocation = @"D:\ForWatchingDog\changed\" + filename;//file要移動到的新地點
           // File.Copy(e.FullPath, fileLocation);//複製檔案 System.IO的功能
           string changeType =$"{e.ChangeType}";//這樣轉成string,不能直接e.ChangeType
            if (Directory.Exists(e.FullPath) && changeType == "Renamed")
            {
                Directory.Move(e.FullPath, fileLocation);//移動目錄到指定地點
               

            }
            else if(File.Exists(e.FullPath))
            {

                 File.Move(e.FullPath, fileLocation);//移動檔案 System.IO的功能
                //File.Copy(e.FullPath, fileLocation);//複製檔案 System.IO的功能
            }



            // Console.WriteLine("Change!"); 這個沒法用
        }

        protected override void OnStop()
        {
            var serviceLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //var msg = $"random - stopped {System.Environment.NewLine}";
            File.AppendAllText($"{serviceLocation}\\log.txt", "service stopped!\n");
            //EventLog.WriteEntry("Watching Dog Service Stopped");
        }




    }
}
