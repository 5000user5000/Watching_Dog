using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

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

        }

        private void DirectoryChanged(object sender, FileSystemEventArgs e)
        {  
            var msg = $"{e.ChangeType} - {e.FullPath} {System.Environment.NewLine}";
            var serviceLocation = Path.GetDirectoryName(  System.Reflection.Assembly.GetExecutingAssembly().Location ); // c:\\Program Files\\Folder To Watch\\Watching_Dog.exe (因為其他人的檔案處會不同,用此得到現在位置)
            File.AppendAllText($"{serviceLocation}\\log.txt",msg);//本來沒有這個log.txt,這裡生成,並把msg的東西寫入
            
            char lastAddr = e.FullPath[-1];//取最後一個字元

            //Directory.CreateDirectory(@"D:\ForWatchingDog\changed");
            File.AppendAllText($"{serviceLocation}\\log.txt", "dir created!\n");
            /*if(!System.IO.Directory.Exists(@"D:\ForWatchingDog\changed"))//如果不存在此檔案就創一個
            {
                Directory.CreateDirectory(@"D:\ForWatchingDog\changed");
            }*/
            /*if (lastAddr == '\\')
            {
                System.IO.File.Move(e.FullPath, @"D:\ForWatchingDog\changed\");//移動檔案

            }
            else
            {
                System.IO.Directory.Move(e.FullPath, @"D:\ForWatchingDog\changed\");//移動目錄到指定地點
            }*/
            
             

            // Console.WriteLine("Change!"); 這個沒法用
        }

        protected override void OnStop()

        {
            var serviceLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var msg = $"random - stopped {System.Environment.NewLine}";
            File.AppendAllText($"{serviceLocation}\\log.txt", "service stopped!\n");
            //EventLog.WriteEntry("Watching Dog Service Stopped");
        }


        
    }
}
