using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Drawing = System.Drawing;

namespace AlphaMattingDemo
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

        private void ShowOriginRes(string sInput, string sTrimap)
        {
            if (File.Exists(sInput))
            {
                BitmapImage bitmapImg = new BitmapImage(new Uri(sInput));
                string sInfo = "当前测试图片 像素分辨率：" + bitmapImg.PixelWidth + " * " + bitmapImg.PixelHeight;
                this.ImgOrigin.Source = new BitmapImage(new Uri(sInput));
                this.TbkPixelInfo.Text = sInfo;
            }

            if (File.Exists(sTrimap))
                this.ImgMatting.Source = new BitmapImage(new Uri(sTrimap));

            this.ImgResult.Source = null;
            this.TbkSeconds.Text = "正在抠图...";
        }

        private void BtnSample1_OnClick(object sender, RoutedEventArgs e)
        {
            string sBasePath = AppDomain.CurrentDomain.BaseDirectory + "Samples/Sample1/";
            string sInput = sBasePath + "input.png";
            string sTrimap = sBasePath + "trimap.png";
            this.ShowOriginRes(sInput, sTrimap);

            this.ExecuteClip(sBasePath, sInput, sTrimap);
        }

        private void BtnSample2_OnClick(object sender, RoutedEventArgs e)
        {
            string sBasePath = AppDomain.CurrentDomain.BaseDirectory + "Samples/Sample2/";
            string sInput = sBasePath + "input.jpg";
            string sTrimap = sBasePath + "trimap.bmp";
            this.ShowOriginRes(sInput, sTrimap);

            this.ExecuteClip(sBasePath, sInput, sTrimap);
        }

        private void BtnSample3_OnClick(object sender, RoutedEventArgs e)
        {
            string sBasePath = AppDomain.CurrentDomain.BaseDirectory + "Samples/Sample3/";
            string sInput = sBasePath + "input.jpg";
            string sTrimap = sBasePath + "trimap.jpg";
            this.ShowOriginRes(sInput, sTrimap);

            this.ExecuteClip(sBasePath, sInput, sTrimap);
        }

        private void BtnSample4_OnClick(object sender, RoutedEventArgs e)
        {
            string sBasePath = AppDomain.CurrentDomain.BaseDirectory + "Samples/Sample4/";
            string sInput = sBasePath + "input.jpg";
            string sTrimap = sBasePath + "trimap.jpg";
            this.ShowOriginRes(sInput, sTrimap);

            this.ExecuteClip(sBasePath, sInput, sTrimap);
        }

        private void BtnSample5_OnClick(object sender, RoutedEventArgs e)
        {
            string sBasePath = AppDomain.CurrentDomain.BaseDirectory + "Samples/Sample5/";
            string sInput = sBasePath + "input.jpg";
            string sTrimap = sBasePath + "trimap.jpg";
            this.ShowOriginRes(sInput, sTrimap);

            this.ExecuteClip(sBasePath, sInput, sTrimap);
        }

        private void ExecuteClip(string sBasePath, string sInput, string sTrimap)
        {
            // 延迟0.3s开始执行算法， 让原始图片有时间在界面显示
            DispatcherTimer oTimer = new DispatcherTimer();
            oTimer.Interval = TimeSpan.FromSeconds(0.3);
            oTimer.Tick += (s, e) =>
            {
                oTimer.Stop();
                oTimer.Tick -= (s1, e1) => { };
                oTimer = null;


                DateTime timeStart = DateTime.Now;
                this.TbkStartTime.Text = "Start:" + timeStart.ToString("HH:mm:ss fff");

                SharedMattingHelper sm = new SharedMattingHelper();
                sm.loadImage(sInput);
                sm.loadTrimap(sTrimap);
                sm.solveAlpha();
                Drawing.Bitmap matteImg = sm.GetMattingImage();

                this.ShowBitmapImage(this.ImgResult, matteImg);

                DateTime timeEnd = DateTime.Now;
                this.TbkEndTime.Text = "End:" + timeEnd.ToString("HH:mm:ss fff");

                TimeSpan timeInterval = timeEnd - timeStart;
                this.TbkSeconds.Text = "耗时:" + timeInterval.TotalSeconds + "s";
            };
            oTimer.Start();
        }

        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        private void ShowBitmapImage(Image img, Drawing.Bitmap oBitmap)
        {
            IntPtr ptr = oBitmap.GetHbitmap();
            BitmapSource bitmapSrc = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                ptr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            DeleteObject(ptr);
            img.Source = bitmapSrc;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string sBasePath = AppDomain.CurrentDomain.BaseDirectory + "Samples/Sample6/";
            string sInput = sBasePath + "1.jpg";
            string sTrimap = sBasePath + "2.jpg";
            this.ShowOriginRes(sInput, sTrimap);

            this.ExecuteClip(sBasePath, sInput, sTrimap);





        }
    }
}
