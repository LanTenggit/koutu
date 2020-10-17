using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace ImgIntelligSample
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public object NativeMethod { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        [DllImport("ImgIntelligHelper.dll", EntryPoint = "Add")]
        static extern double Add(double a, double b);

        [DllImport("ImgIntelligHelper.dll", CharSet = CharSet.Unicode)]
        public static extern double Sub(double a, double b);

        [DllImport("ImgIntelligHelper.dll", CharSet = CharSet.Unicode)]
        public static extern double Mul(double a, double b);

        [DllImport("ImgIntelligHelper.dll", CharSet = CharSet.Unicode)]
        public static extern double Div(double a, double b);

        [StructLayout(LayoutKind.Sequential)]
        public struct AlphaData
        {
            [MarshalAs(UnmanagedType.ByValArray)]
            int[,] Map;
        }

        [DllImport("ImgIntelligHelper.dll", CharSet = CharSet.Unicode)]
        public extern static void GetMatteData([MarshalAs(UnmanagedType.LPStr)] string sInput,
            [MarshalAs(UnmanagedType.LPStr)] string sOutput, [MarshalAs(UnmanagedType.LPStruct)]ref AlphaData alphaDatas);

        [DllImport("ImgIntelligHelper.dll", CharSet = CharSet.Unicode)]
        public extern static void GetAlphaData([MarshalAs(UnmanagedType.LPStr)] string sInput,
            [MarshalAs(UnmanagedType.LPStr)] string sOutput, [MarshalAs(UnmanagedType.LPArray)]ref byte[] alphaDatas);

        [DllImport("ImgIntelligHelper.dll", CharSet = CharSet.Unicode)]
        public extern static IntPtr GetAlphaMap([MarshalAs(UnmanagedType.LPStr)] string sInput,
            [MarshalAs(UnmanagedType.LPStr)] string sOutput, [MarshalAs(UnmanagedType.LPArray)]ref int[] alphaDatas);

        private void BtnAdd_OnClick(object sender, RoutedEventArgs e)
        {
            double dResult = Add(3d, 4d);
            this.TbkResult.Text = dResult.ToString();
        }

        private void BtnSub_OnClick(object sender, RoutedEventArgs e)
        {
            double dResult = Sub(3d, 4d);
            this.TbkResult.Text = dResult.ToString();
        }

        private void BtnMul_OnClick(object sender, RoutedEventArgs e)
        {
            double dResult = Mul(3d, 4d);
            this.TbkResult.Text = dResult.ToString();
        }

        [DllImport("ImgIntelligHelper.dll", CharSet = CharSet.Unicode)]
        public extern static IntPtr GetMatteMap([MarshalAs(UnmanagedType.LPStr)] string sInput,
            [MarshalAs(UnmanagedType.LPStr)] string sOutput);

        private void BtnMatting_OnClick(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int nTag = Convert.ToInt32(btn.Tag);
            this.DoConvert(nTag);
        }

        // 调用c++ dll进行抠图
        private void DoConvert(int nIndex)
        {
            string sInput = AppDomain.CurrentDomain.BaseDirectory + "Datas/input"+ nIndex +".jpg";

            byte[] bytes = System.IO.File.ReadAllBytes(sInput);
            BitmapImage bitImg = new BitmapImage();
            bitImg.BeginInit();
            bitImg.StreamSource = new System.IO.MemoryStream(bytes);
            bitImg.EndInit();
            bitImg.Freeze();
            this.ImgMain.Source = bitImg;

            string sTrimap = AppDomain.CurrentDomain.BaseDirectory + "Datas/trimap" + nIndex +".jpg";
            System.Drawing.Bitmap oBitmap = new Bitmap(sInput);

            DateTime timeStart = DateTime.Now;
            int nlength = oBitmap.Width * oBitmap.Height;
            IntPtr intptr = GetMatteMap(sInput, sTrimap);
            int[] arrAlpha = new int[nlength];
            Marshal.Copy(intptr, arrAlpha, 0, nlength);

            System.Drawing.Bitmap newBitmap = new System.Drawing.Bitmap(oBitmap.Width, oBitmap.Height,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(newBitmap))
            {
                g.DrawImage(oBitmap, 0, 0);
                for (int i = 0; i < oBitmap.Height; i++)
                {
                    for (int j = 0; j < oBitmap.Width; j++)
                    {
                        int nAlpha = arrAlpha[i * oBitmap.Width + j];
                        if (nAlpha > 0)
                        {
                            System.Drawing.Color pixelColor = oBitmap.GetPixel(j, i);
                            System.Drawing.Color newColor = System.Drawing.Color.FromArgb(nAlpha, pixelColor.R, pixelColor.G, pixelColor.B);
                            newBitmap.SetPixel(j, i, newColor);
                        }
                        else
                            newBitmap.SetPixel(j, i, System.Drawing.Color.Transparent);
                    }
                }
            }

            IntPtr ptr = newBitmap.GetHbitmap();
            BitmapSource bitmapSrc = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                ptr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            DeleteObject(ptr);

            this.ImgResult.Source = bitmapSrc;

            DateTime timeEnd = DateTime.Now;
            TimeSpan timeInterval = timeEnd - timeStart;
            this.TbkResult.Text ="Use Time (s) :" + timeInterval.TotalSeconds.ToString();
        }

        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);
    }
}
