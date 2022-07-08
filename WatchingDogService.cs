using Microsoft.VisualBasic.FileIO;
using System;
using System.IO;
using System.Net.Mail;
using System.ServiceProcess;


namespace Watching_Dog
{
    public partial class WatchingDogService : ServiceBase
    {

        FileSystemWatcher fileSystemWatcher;
        //DelayFileSystemWatcher delayFileSystemWatcher;  

        public WatchingDogService()
        {
            InitializeComponent();
        }

       static string monitorPath = "D:\\abcd";//要監管的地方 設成全域變數 必須用static不然會錯誤

        protected override void OnStart(string[] args)
        {
            //string monitorPath = "D:\\abcd";//要監管的地方
            fileSystemWatcher = new FileSystemWatcher(monitorPath); 
           

           

            //EventLog.WriteEntry("Watching Dog Service Started");

           fileSystemWatcher = new FileSystemWatcher(monitorPath)
            {
                EnableRaisingEvents = true,
                IncludeSubdirectories = true
            };
            
             fileSystemWatcher.Created += DirectoryChanged;
             fileSystemWatcher.Deleted += DirectoryChanged;
             fileSystemWatcher.Changed += DirectoryChanged;
             fileSystemWatcher.Renamed += DirectoryChanged;
            //換個寫法試試
           /* fileSystemWatcher.Changed += new FileSystemEventHandler(DirectoryChanged);
            fileSystemWatcher.Created += new FileSystemEventHandler(DirectoryChanged);
            fileSystemWatcher.Deleted += new FileSystemEventHandler(DirectoryChanged);
            fileSystemWatcher.Renamed += new RenamedEventHandler(DirectoryChanged);
            fileSystemWatcher.EnableRaisingEvents = true;
            fileSystemWatcher.NotifyFilter =  NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName  | NotifyFilters.Size | NotifyFilters.LastWrite;//會檢查哪些
            fileSystemWatcher.IncludeSubdirectories = true; */

            var serviceLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location); // c:\\Program Files\\Folder To Watch\\Watching_Dog.exe (因為其他人的檔案處會不同,用此得到現在位置)
            File.AppendAllText($"{serviceLocation}\\log.txt", $"service started! {DateTime.Now.ToShortTimeString()}\n"); //本來沒有這個log.txt,這裡生成,並把msg的東西寫入

            string dirName = Path.GetFileName(monitorPath);
            string backupPath = @"D:\ForWatchingDog\backup\" + dirName;//這樣能把root的檔案夾也複製,如果有多個要監控的檔案夾,備份的東西才不會混在一起
            if (!System.IO.Directory.Exists(backupPath))//如果不存在此檔案就創一個
            {
                Directory.CreateDirectory(backupPath);
                File.AppendAllText($"{serviceLocation}\\log.txt", "backup dir created!\n");
                FileSystem.CopyDirectory("D:\\abcd", backupPath);
                File.AppendAllText($"{serviceLocation}\\log.txt", "backup successful!\n");
            }


        }

       
        

      

         private void DirectoryChanged(object sender, FileSystemEventArgs e)
        {
            //避免重複觸發事件

            var watcher = sender as FileSystemWatcher;
            watcher.EnableRaisingEvents = false;
            
            
            var msg = $"{e.ChangeType} - {e.FullPath} {System.Environment.NewLine}";
            var serviceLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location); // c:\\Program Files\\Folder To Watch\\Watching_Dog.exe (因為其他人的檔案處會不同,用此得到現在位置)
            File.AppendAllText($"{serviceLocation}\\log.txt", "--------Start the change func!\n");
            File.AppendAllText($"{serviceLocation}\\log.txt", msg); //本來沒有這個log.txt,這裡生成,並把msg的東西寫入

            //char lastAddr = e.FullPath[-1];//取最後一個字元,會出錯

            //寄給 此信箱,警告檔案變動
            // SendAutomatedEmail("yyy@gmail.com"); //這句還是放在移出還原那裏較好,因為當移動時,也算onChange,會再記一次mail
     



            //如果不存在此檔案就創一個
            if (!System.IO.Directory.Exists(@"D:\ForWatchingDog\changed"))
            {
                Directory.CreateDirectory(@"D:\ForWatchingDog\changed");
                File.AppendAllText($"{serviceLocation}\\log.txt", "dir created!\n");
            }

            string filename = Path.GetFileName(e.FullPath);
            string fileLocation = @"D:\ForWatchingDog\changed\" + filename;//file要移動到的新地點
            string backupPath = @"D:\ForWatchingDog\backup\" + getLastDirName(e.FullPath);

            // File.AppendAllText($"{serviceLocation}\\log.txt", $"backup dir = {backupPath}\n"); //檢查備份路徑用

            string changeType =$"{e.ChangeType}";//這樣轉成string,不能直接e.ChangeType
         
            //移出變動的地方並復原回去
            if (Directory.Exists(e.FullPath) && changeType == "Renamed" )
            {
                
                Directory.Move(e.FullPath, fileLocation);//移動目錄到指定地點
                
                string upDir = Path.GetDirectoryName(backupPath); //取得其父檔案
                string[] dirs = Directory.GetDirectories(upDir); 
                string nowDir = Path.GetDirectoryName(e.FullPath);
                foreach (var dir in dirs)
                {
                    
                    string DirName = Path.GetFileName(dir);
                    string newDirLocation = nowDir +'\\'+ DirName;//file要移動到的新地點

                    File.AppendAllText($"{serviceLocation}\\log.txt", $"check this dir = {dir} vs new path {newDirLocation}\n");
                    if (!Directory.Exists(newDirLocation))
                    {
                        FileSystem.CopyDirectory(dir, newDirLocation);
                        File.AppendAllText($"{serviceLocation}\\log.txt", $"recovery\n");
                    }
                }

            }
            else if(Directory.Exists(e.FullPath) && changeType == "Created")//當被多新增檔案夾就直接移開就好
            {
                Directory.Move(e.FullPath, fileLocation);   
            }
            else if(File.Exists(e.FullPath) && changeType != "Renamed")
            {
                
                
                
                File.Move(e.FullPath, fileLocation);//移動檔案 System.IO的功能

                //以下是復原方式
                
                if ( File.Exists(backupPath) )//因為可能是新增檔案,那麼backup就沒有可以用的
                {
                    
                    //File.Create(e.FullPath);//只有修改or刪除檔案時,才要重建一個 copy似乎本身就有這功能(當指定路徑沒東西時)
                    File.Copy(backupPath, e.FullPath,true);//複製檔案 System.IO的功能 //增加true 表示能夠覆蓋同名檔案
                }
                
                File.AppendAllText($"{serviceLocation}\\log.txt", "recovery successfully!\n");


                
            }
            else if(changeType == "Deleted")//如果被刪除的話
            {
                if (File.Exists(backupPath))
                {               
                    File.Copy(backupPath, e.FullPath, true);
                }
                else if (Directory.Exists(backupPath))
                {
                    FileSystem.CopyDirectory(backupPath, e.FullPath, true);
                }


            }
            else if(changeType == "Renamed")//這裡是指file部分
            {
                File.Move(e.FullPath, fileLocation);//移動目錄到指定地點

                string upDir = Path.GetDirectoryName(backupPath); //取得其父檔案
                string[] files = Directory.GetFiles(upDir);
                string nowDir = Path.GetDirectoryName(e.FullPath);
                foreach (var file in files)
                {

                    string nowFileName = Path.GetFileName(file);
                    string newFileLocation = nowDir + '\\' + nowFileName;//file要移動到的新地點

                    File.AppendAllText($"{serviceLocation}\\log.txt", $"check this file = {file} vs new path {newFileLocation}\n");
                    if (!File.Exists(newFileLocation))
                    {
                        File.Copy(file, newFileLocation);
                        File.AppendAllText($"{serviceLocation}\\log.txt", $"recovery\n");
                    }
                }

            }

            File.AppendAllText($"{serviceLocation}\\log.txt", "--------End the change func!\n");
            watcher.EnableRaisingEvents = true; //復原
            // Console.WriteLine("Change!"); 這個沒法用
        }

        protected override void OnStop()
        {
            var serviceLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            
            File.AppendAllText($"{serviceLocation}\\log.txt", $"service stopped! {DateTime.Now.ToShortTimeString()}\n");
             fileSystemWatcher.Dispose();//釋放資源
            //EventLog.WriteEntry("Watching Dog Service Stopped");
        }

        public static void SendAutomatedEmail(string ReceiveMail)
        {
            //var serviceLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location); // c:\\Program Files\\Folder To Watch\\Watching_Dog.exe (因為其他人的檔案處會不同,用此得到現在位置)
            //File.AppendAllText($"{serviceLocation}\\log.txt", "enter send func!\n"); //確認進入此func

            try
            {
                

                MailMessage msg = new System.Net.Mail.MailMessage();
                msg.To.Add(ReceiveMail);
                //msg.To.Add("b@b.com");可以發送給多人
                //msg.CC.Add("c@c.com");
                //msg.CC.Add("c@c.com");可以抄送副本給多人 
                //這裡可以隨便填，不是很重要
                msg.From = new MailAddress("xxx@gmail.com", "Watching Dog系統", System.Text.Encoding.UTF8);
                /* 上面3個參數分別是發件人地址（可以隨便寫），發件人姓名，編碼*/
                msg.Subject = "警告";//郵件標題
                msg.SubjectEncoding = System.Text.Encoding.UTF8;//郵件標題編碼
                msg.Body = $"您所監控的檔案已經在{DateTime.Now.ToShortTimeString()}的時候變動,變動的放在 D:\\ForWatchingDog\\changed\\ 裡面,原檔案也已經幫您復原完畢"; //郵件內容
                msg.BodyEncoding = System.Text.Encoding.UTF8;//郵件內容編碼 
                //msg.Attachments.Add(new Attachment(@"D:\test2.docx"));  //附件
                msg.IsBodyHtml = true;//是否是HTML郵件 
                //msg.Priority = MailPriority.High;//郵件優先級 

                SmtpClient client = new SmtpClient();
                client.Credentials = new System.Net.NetworkCredential("xxx@gmail.com", "wwwwww"); //這裡要填正確的帳號跟(應用程式)密碼
                client.Host = "smtp.gmail.com"; //設定smtp Server
                client.Port = 25; //設定Port
                client.EnableSsl = true; //gmail預設開啟驗證
                client.Send(msg); //寄出信件
                client.Dispose();
                msg.Dispose();
                

                //File.AppendAllText($"{serviceLocation}\\log.txt", "EMAIL send!\n"); //確認寄發成功
            }
            catch (Exception ex)
            {
                //File.AppendAllText($"{serviceLocation}\\log.txt", $"{ex.ToString()}\n"); //錯誤訊息寫入log
                
            }
        }

        //用backup復原檔案時所需
        public static string getLastDirName(string path) //如監控檔案是 D:\abcd\ddk     path是 D:\abcd\ddk\hello\text.txt  回傳 ddk\hello\text.txt
        {
            string monitorDir = Path.GetDirectoryName( monitorPath );//取得監視檔案的dir
            int len = monitorDir.Length;
            int i = len-1;
            while (monitorDir[i]!= '\\')
            {
                i--;
            }
            int len2 = path.Length;

            return path.Substring(i+1,len2-i-1); //[i+1,len - 1]
        }

        

    }
}
