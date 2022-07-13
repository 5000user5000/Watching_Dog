using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
          var serviceLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string resource = MainDir(serviceLocation)+ "\\data.txt";
            if(File.Exists(resource))
            {
                File.Delete(resource);
            }
          
           

        }

        private void btnSend_Click(object sender, EventArgs e)
        {

            var serviceLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string resource = MainDir(serviceLocation);
            //File.Create($"{resource}\\data.txt");
            File.AppendAllText($"{resource}\\data.txt", $"{MonitorPath.Text}\n"); //本來沒有這個data.txt,這裡生成,並把msg的東西寫入
            File.AppendAllText($"{resource}\\data.txt", $"{EmailPath.Text}\n");
            MessageBox.Show("send");

        }
        public string MainDir(string path)
        {
            //dir一共比path上去3層.把檔案存在那(Watching_Dog),或是多加個resource檔放入
            string dir = Path.GetDirectoryName(path);
            for(int i=0;i < 2;i++)
            {
                dir = Path.GetDirectoryName(dir);
            }
            return dir;
        }

       
    }
}
