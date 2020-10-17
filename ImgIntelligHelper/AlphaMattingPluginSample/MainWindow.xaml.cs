using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AlphaMattingPluginSample
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnCall_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string sIndex = btn.Tag.ToString();
            this.DoConvert(sIndex);
        }

        private void DoConvert(string sIndex)
        {
            string sBasePath = AppDomain.CurrentDomain.BaseDirectory;
            // Todo：AlphaMattingPlugin项目重新生成后，要将生成的exe拷贝至本程序生成目录下，不然调用的还是原来的
            string sExeFile = sBasePath + @"\AlphaMattingPlugin.exe";
            string sInput = sBasePath + @"\Datas\input" + sIndex+ ".jpg";
            if (File.Exists(sInput))
                this.ShowImage(this.ImgOrigin, sInput);
            string sTrimap = sBasePath + @"\Datas\trimap" + sIndex + ".jpg";
            string sOutput = sBasePath + @"\Datas\AlphaMattingPluginSample" + sIndex + ".png";

            DateTime timeStart = DateTime.Now;

            // 已进程方式调用，传入相应参数
            Process process = new Process();
            process.StartInfo.FileName = sExeFile;
            process.StartInfo.Arguments = " " + sInput + " " + sTrimap + " " + sOutput;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            process.WaitForExit();

            DateTime timeEnd = DateTime.Now;
            TimeSpan timeInterval = timeEnd - timeStart;
            this.TbkTime.Text = "Use Time (s) :" + timeInterval.TotalSeconds.ToString();

            if (File.Exists(sOutput))
                this.ShowImage(this.ImgResult, sOutput);

          
        }

        private void ShowImage(Image img, string sFile)
        {
            byte[] bytes = System.IO.File.ReadAllBytes(sFile);
            BitmapImage bitImg = new BitmapImage();
            bitImg.BeginInit();
            bitImg.StreamSource = new System.IO.MemoryStream(bytes);
            bitImg.EndInit();
            bitImg.Freeze();
            img.Source = bitImg;
        }
    }
}
