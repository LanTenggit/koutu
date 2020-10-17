using System;
using System.Collections.Generic;
using System.Linq;
using Drawing = System.Drawing;
using Emgu.CV;

namespace AlphaMattingDemo
{
    public class labelPoint
    {
        public int x;
        public int y;
        public int label;
    };

    public class TupleInfo
    {
        public double FR;
        public double FG;
        public double FB;

        public double BR;
        public double BG;
        public double BB;

        public double sigmaf;
        public double sigmab;
        public int flag;
    };

    public class FtupleInfo
    {
        public double FR;
        public double FG;
        public double FB;

        public double BR;
        public double BG;
        public double BB;

        public double alphar;
        public double confidence;
    };

    public class SharedMattingHelper
    {
        private Mat pImg;
        private Mat trimap;

        List<Drawing.Point> uT = new List<Drawing.Point>();
        List<TupleInfo> tuples = new List<TupleInfo>();
        List<FtupleInfo> ftuples = new List<FtupleInfo>();

        int height;
        int width;
        int kI;
        int kG;
        int[,] unknownIndex;
        int[,] tri;
        int[,] alpha;

        double kC;
        int step;
        int channels;
        private byte[] data;

        public SharedMattingHelper()
        {
            kI = 10;
            kC = 5.0;
            kG = 4;
            uT.Clear();
            tuples.Clear();
        }

        public void loadImage(string sFile)
        {
            pImg = CvInvoke.Imread(sFile);
            if (pImg.GetData() == null || pImg.GetData().Length == 0)
            {
                Console.WriteLine("load pImg failed!");
                return;
            }

            height = pImg.Rows;
            width = pImg.Cols;
            step = pImg.Step / (pImg.ElementSize / pImg.NumberOfChannels);

            channels = pImg.NumberOfChannels;
            data = pImg.GetData();
            unknownIndex = new int[height, width];
            tri = new int[height, width];
            alpha = new int[height, width];
        }

        public void loadTrimap(string sFile)
        {
            trimap = CvInvoke.Imread(sFile);
            if (trimap.GetData() == null || trimap.GetData().Length == 0)
                Console.WriteLine("load trimap failed!");
            return;
        }

        private double CalcAlpha(double CB, double CG, double CR, double FB, double FG, double FR, double BB, double BG, double BR)
        {
            double Alpha = (double)((CR - BR) * (FR - BR) +
                                    (CG - BG) * (FG - BG) +
                                    (CB - BB) * (FB - BB)) /
                           ((FR - BR) * (FR - BR) +
                            (FG - BG) * (FG - BG) +
                            (FB - BB) * (FB - BB) + 0.0000001);        // 这里0.0000001换成Eps在LocalSmooth阶段似乎就不对了，有反常的噪点产生
            if (Alpha > 1)
                Alpha = 1;
            else if (Alpha < 0)
                Alpha = 0;
            return Alpha;
        }

        private double CalcColorDistance(double AB, double AG, double AR, double BB, double BG, double BR)
        {
            return (AB - BB) * (AB - BB) + (AG - BG) * (AG - BG) + (AR - BR) * (AR - BR);
        }

        private void expandKnown()
        {
            List<labelPoint> vp = new List<labelPoint>();
            int kc2 = Convert.ToInt32(kC * kC);
            vp.Clear();
            int s = trimap.Step / (trimap.ElementSize / trimap.NumberOfChannels);
            int c = trimap.NumberOfChannels;
            byte[] d = trimap.GetData();
            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    tri[i, j] = d[i * step + j * channels];
                }
            }

            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                {

                    if (tri[i, j] != 0 && tri[i, j] != 255)
                    {

                        int label = -1;
                        double dmin = 10000.0;
                        bool flag = false;
                        int pb = data[i * step + j * channels];
                        int pg = data[i * step + j * channels + 1];
                        int pr = data[i * step + j * channels + 2];

                        for (int k = 0; (k <= kI) && !flag; ++k)
                        {
                            int k1 = Math.Max(0, i - k);
                            int k2 = Math.Min(i + k, height - 1);
                            int l1 = Math.Max(0, j - k);
                            int l2 = Math.Min(j + k, width - 1);

                            for (int l = k1; (l <= k2) && !flag; ++l)
                            {
                                double dis;
                                double gray;


                                gray = tri[l, l1];
                                if (gray == 0 || gray == 255)
                                {
                                    dis = dP(new Drawing.Point(i, j), new Drawing.Point(l, l1));
                                    if (dis > kI)
                                        continue;

                                    int qb = data[l * step + l1 * channels];
                                    int qg = data[l * step + l1 * channels + 1];
                                    int qr = data[l * step + l1 * channels + 2];
                                    double distanceColor = this.CalcColorDistance(pb, pg, pr, qb, qg, qr);
                                    if (distanceColor <= kc2)
                                    {
                                        flag = true;
                                        label = (int)(gray);
                                    }
                                }

                                if (flag)
                                    break;

                                gray = tri[l, l2];
                                if (gray == 0 || gray == 255)
                                {
                                    dis = dP(new Drawing.Point(i, j), new Drawing.Point(l, l2));
                                    if (dis > kI)
                                        continue;

                                    int qb = data[l * step + l2 * channels];
                                    int qg = data[l * step + l2 * channels + 1];
                                    int qr = data[l * step + l2 * channels + 2];
                                    double distanceColor = this.CalcColorDistance(pb, pg, pr, qb, qg, qr);
                                    if (distanceColor <= kc2)
                                    {
                                        flag = true;
                                        label = (int)(gray);
                                    }
                                }
                            }

                            for (int l = l1; (l <= l2) && !flag; ++l)
                            {
                                double dis;
                                double gray;

                                gray = tri[k1, l];
                                if (gray == 0 || gray == 255)
                                {
                                    dis = dP(new Drawing.Point(i, j), new Drawing.Point(k1, l));
                                    if (dis > kI)
                                    {
                                        continue;
                                    }

                                    int qb = data[k1 * step + l * channels];
                                    int qg = data[k1 * step + l * channels + 1];
                                    int qr = data[k1 * step + l * channels + 2];
                                    double distanceColor = this.CalcColorDistance(pb, pg, pr, qb, qg, qr);
                                    if (distanceColor <= kc2)
                                    {
                                        flag = true;
                                        label = (int)(gray);
                                    }
                                }

                                gray = tri[k2, l];
                                if (gray == 0 || gray == 255)
                                {
                                    dis = dP(new Drawing.Point(i, j), new Drawing.Point(k2, l));
                                    if (dis > kI)
                                    {
                                        continue;
                                    }

                                    int qb = data[k2 * step + l * channels];
                                    int qg = data[k2 * step + l * channels + 1];
                                    int qr = data[k2 * step + l * channels + 2];
                                    double distanceColor = this.CalcColorDistance(pb, pg, pr, qb, qg, qr);
                                    if (distanceColor <= kc2)
                                    {
                                        flag = true;
                                        label = (int)(gray);
                                    }
                                }
                            }
                        }

                        if (label != -1)
                        {
                            labelPoint lp = new labelPoint();
                            lp.x = i;
                            lp.y = j;
                            lp.label = label;
                            vp.Add(lp);
                        }
                        else
                        {
                            Drawing.Point lp = new Drawing.Point(i, j);
                            uT.Add(lp);
                        }
                    }
                }
            }

            for (int i = 0; i < vp.Count; i++)
            {
                labelPoint item = vp[i];
                tri[item.x, item.y] = item.label;
            }

            vp.Clear();
        }

        private double mP(int i, int j, double FB, double FG, double FR, double BB, double BG, double BR)
        {
            double CB = data[i * step + j * channels];
            double CG = data[i * step + j * channels + 1];
            double CR = data[i * step + j * channels + 2];
            double alpha = this.CalcAlpha(CB, CG, CR, FB, FG, FR, BB, BG, BR);
            double result = Math.Sqrt((CB - alpha * FB - (1 - alpha) * BB) * (CB - alpha * FB - (1 - alpha) * BB) +
                                      (CG - alpha * FG - (1 - alpha) * BG) * (CG - alpha * FG - (1 - alpha) * BG) +
                                      (CR - alpha * FR - (1 - alpha) * BR) * (CR - alpha * FR - (1 - alpha) * BR));
            return result / 255.0;
        }

        private double nP(int i, int j, double FB, double FG, double FR, double BB, double BG, double BR)
        {
            int i1 = Math.Max(0, i - 1);
            int i2 = Math.Min(i + 1, height - 1);
            int j1 = Math.Max(0, j - 1);
            int j2 = Math.Min(j + 1, width - 1);

            double result = 0;
            for (int k = i1; k <= i2; ++k)
            {
                for (int l = j1; l <= j2; ++l)
                {
                    double m = mP(k, l, FB, FG, FR, BB, BG, BR);
                    result += m * m;
                }
            }
            return result;
        }

        private double eP(int i1, int j1, int i2, int j2)
        {
            double ci = i2 - i1;
            double cj = j2 - j1;
            double z = Math.Sqrt(ci * ci + cj * cj);
            double ei = ci / (z + 0.0000001);
            double ej = cj / (z + 0.0000001);
            double stepinc = Math.Min(1 / (Math.Abs(ei) + 1e-10), 1 / (Math.Abs(ej) + 1e-10));
            double result = 0;
            int PB = data[i1 * step + j1 * channels];
            int PG = data[i1 * step + j1 * channels + 1];
            int PR = data[i1 * step + j1 * channels + 2];
            int ti = i1;
            int tj = j1;

            for (double t = 1; ; t += stepinc)
            {
                double inci = ei * t;
                double incj = ej * t;
                int i = (int)(i1 + inci + 0.5);
                int j = (int)(j1 + incj + 0.5);

                z = 1;

                int CB = data[i * step + j * channels];
                int CG = data[i * step + j * channels + 1];
                int CR = data[i * step + j * channels + 2];

                if (ti - i > 0 && tj - j == 0)
                    z = ej;
                else if (ti - i == 0 && tj - j > 0)
                    z = ei;

                result += ((CB - PB) * (CB - PB) + (CG - PG) * (CG - PG) + (CR - PR) * (CR - PR)) * z;
                PB = CB;
                PG = CG;
                PR = CR;

                ti = i;
                tj = j;

                if (Math.Abs(ci) >= Math.Abs(inci) || Math.Abs(cj) >= Math.Abs(incj))
                    break;
            }

            return result;
        }

        private double pfP(Drawing.Point p, List<Drawing.Point> f, List<Drawing.Point> b)
        {
            double fmin = 1e10;

            for (int i = 0; i < f.Count; i++)
            {
                double fp = eP(p.X, p.Y, f[i].X, f[i].Y);
                if (fp < fmin)
                {
                    fmin = fp;
                }
            }

            double bmin = 1e10;
            for (int i = 0; i < b.Count; i++)
            {
                double bp = eP(p.X, p.Y, b[i].X, b[i].Y);
                if (bp < bmin)
                {
                    bmin = bp;
                }
            }
            return bmin / (fmin + bmin + 1e-10);
        }

        private double aP(int i, int j, double pf, double FB, double FG, double FR, double BB, double BG, double BR)
        {
            int CB = data[i * step + j * channels];
            int CG = data[i * step + j * channels + 1];
            int CR = data[i * step + j * channels + 2];
            double alpha = this.CalcAlpha(CB, CG, CR, FB, FG, FR, BB, BG, BR);
            return pf + (1 - 2 * pf) * alpha;
        }

        private double dP(Drawing.Point s, Drawing.Point d)
        {
            return Math.Sqrt((double)((s.X - d.X) * (s.X - d.X) + (s.Y - d.Y) * (s.Y - d.Y)));
        }

        private double gP(Drawing.Point p, Drawing.Point fp, Drawing.Point bp, double pf)
        {
            int FB = data[fp.X * step + fp.Y * channels];
            int FG = data[fp.X * step + fp.Y * channels + 1];
            int FR = data[fp.X * step + fp.Y * channels + 2];

            int BB = data[bp.X * step + bp.Y * channels];
            int BG = data[bp.X * step + bp.Y * channels + 1];
            int BR = data[bp.X * step + bp.Y * channels + 2];

            double tn = Math.Pow(nP(p.X, p.Y, FB, FG, FR, BB, BG, BR), 3);
            double ta = Math.Pow(aP(p.X, p.Y, pf, FB, FG, FR, BB, BG, BR), 2);
            double tf = dP(p, fp);
            double tb = Math.Pow(dP(p, bp), 4);

            return tn * ta * tf * tb;
        }

        private double gP(Drawing.Point p, Drawing.Point fp, Drawing.Point bp, double dpf, double pf)
        {
            int FB = data[fp.X * step + fp.Y * channels];
            int FG = data[fp.X * step + fp.Y * channels + 1];
            int FR = data[fp.X * step + fp.Y * channels + 2];

            int BB = data[bp.X * step + bp.Y * channels];
            int BG = data[bp.X * step + bp.Y * channels + 1];
            int BR = data[bp.X * step + bp.Y * channels + 2];

            double tn = Math.Pow(nP(p.X, p.Y, FB, FG, FR, BB, BG, BR), 3);
            double ta = Math.Pow(aP(p.X, p.Y, pf, FB, FG, FR, BB, BG, BR), 2);
            double tf = dpf;
            double tb = Math.Pow(dP(p, bp), 4);

            return tn * ta * tf * tb;
        }

        private double sigma2(Drawing.Point p)
        {
            int CB = data[p.X * step + p.Y * channels];
            int CG = data[p.X * step + p.Y * channels + 1];
            int CR = data[p.X * step + p.Y * channels + 2];

            int i1 = Math.Max(0, (int)p.X - 2);
            int i2 = Math.Min((int)p.X + 2, height - 1);
            int j1 = Math.Max(0, (int)p.Y - 2);
            int j2 = Math.Min((int)p.Y + 2, width - 1);

            double result = 0;
            int num = 0;

            int TempB, TempG, TempR;
            for (int i = i1; i <= i2; ++i)
            {
                for (int j = j1; j <= j2; ++j)
                {

                    TempB = data[i * step + j * channels];
                    TempG = data[i * step + j * channels + 1];
                    TempR = data[i * step + j * channels + 2];
                    result += this.CalcColorDistance(CB, CG, CR, TempB, TempG, TempR);
                    ++num;
                }
            }
            return result / (num + 1e-10);
        }

        private void sample(Drawing.Point p, List<Drawing.Point> f, List<Drawing.Point> b)
        {
            int i = (int)p.X;
            int j = (int)p.Y;

            double inc = 360.0 / kG;
            double ca = inc / 9;
            double angle = (i % 3 * 3 + j % 9) * ca;
            for (int k = 0; k < kG; ++k)
            {
                bool flagf = false;
                bool flagb = false;

                double z = (angle + k * inc) / 180 * 3.1415926;
                double ei = Math.Sin(z);
                double ej = Math.Cos(z);

                double step = Math.Min(1.0 / (Math.Abs(ei) + 1e-10), 1.0 / (Math.Abs(ej) + 1e-10));

                for (double t = 1; ; t += step)
                {
                    int ti = (int)(i + ei * t + 0.5);
                    int tj = (int)(j + ej * t + 0.5);

                    if (ti >= height || ti < 0 || tj >= width || tj < 0)
                        break;
                    int gray = tri[ti, tj];

                    if (!flagf && gray == 255)
                    {
                        Drawing.Point tp = new Drawing.Point(ti, tj);
                        f.Add(tp);
                        flagf = true;
                    }
                    else if (!flagb && gray == 0)
                    {
                        Drawing.Point tp = new Drawing.Point(ti, tj);
                        b.Add(tp);
                        flagb = true;
                    }
                    if (flagf && flagb)
                        break;
                }
            }
        }

        private void Sample(List<List<Drawing.Point>> F, List<List<Drawing.Point>> B)
        {
            int a, b, i;
            int x, y, p, q;
            int w, h, gray;
            int angle;
            double z, ex, ey, t, step;

            a = 360 / kG;
            b = (int)(1.7f * a / 9);
            F.Clear();
            B.Clear();
            w = pImg.Cols;
            h = pImg.Rows;

            for (int j = 0; j < uT.Count; j++)
            {
                List<Drawing.Point> fPts = new List<Drawing.Point>();
                List<Drawing.Point> bPts = new List<Drawing.Point>();

                x = uT[j].X;
                y = uT[j].Y;
                angle = (x + y) * b % a;
                for (i = 0; i < kG; ++i)
                {
                    bool f1 = false;
                    bool f2 = false;

                    z = (angle + i * a) / 180.0f * 3.1415926f;
                    ex = Math.Sin(z);
                    ey = Math.Cos(z);
                    step = Math.Min(1.0f / (Math.Abs(ex) + 1e-10f),
                        1.0f / (Math.Abs(ey) + 1e-10f));

                    for (t = 0; ; t += step)
                    {
                        p = (int)(x + ex * t + 0.5f);
                        q = (int)(y + ey * t + 0.5f);
                        if (p < 0 || p >= h || q < 0 || q >= w)
                            break;

                        gray = tri[p, q];
                        if (!f1 && gray < 50)
                        {
                            Drawing.Point pt = new Drawing.Point(p, q);
                            bPts.Add(pt);
                            f1 = true;
                        }
                        else
                        if (!f2 && gray > 200)
                        {
                            Drawing.Point pt = new Drawing.Point(p, q);
                            fPts.Add(pt);
                            f2 = true;
                        }
                        else
                        if (f1 && f2)
                            break;
                    }
                }

                F.Add(fPts);
                B.Add(bPts);
            }
        }

        private void gathering()
        {
            List<Drawing.Point> f = new List<Drawing.Point>();
            List<Drawing.Point> b = new List<Drawing.Point>();
            List<List<Drawing.Point>> F = new List<List<Drawing.Point>>();
            List<List<Drawing.Point>> B = new List<List<Drawing.Point>>();

            Sample(F, B);

            int index = 0;
            double a;
            int size = uT.Count();

            for (int m = 0; m < size; ++m)
            {
                int i = uT[m].X;
                int j = uT[m].Y;

                double pfp = pfP(new Drawing.Point(i, j), F[m], B[m]);
                double gmin = 1.0e10;

                Drawing.Point tf = new Drawing.Point();
                Drawing.Point tb = new Drawing.Point();

                bool flag = false;
                bool first = true;

                for (int idx = 0; idx < F[m].Count; idx++)
                {
                    double dpf = dP(new Drawing.Point(i, j), F[m][idx]);
                    for (int idx2 = 0; idx2 < B[m].Count; idx2++)
                    {
                        double gp = gP(new Drawing.Point(i, j), F[m][idx], B[m][idx2], dpf, pfp);
                        if (gp < gmin)
                        {
                            gmin = gp;
                            tf = new Drawing.Point(F[m][idx].X, F[m][idx].Y);
                            tb = new Drawing.Point(B[m][idx2].X, B[m][idx2].Y);
                            flag = true;
                        }
                    }
                }

                TupleInfo tupleInfo = new TupleInfo();
                tupleInfo.flag = -1;
                if (flag)
                {
                    tupleInfo.FB = data[tf.X * step + tf.Y * channels];
                    tupleInfo.FG = data[tf.X * step + tf.Y * channels + 1];
                    tupleInfo.FR = data[tf.X * step + tf.Y * channels + 2];
                    tupleInfo.flag = 1;

                    tupleInfo.BB = data[tb.X * step + tb.Y * channels];
                    tupleInfo.BG = data[tb.X * step + tb.Y * channels + 1];
                    tupleInfo.BR = data[tb.X * step + tb.Y * channels + 2];

                    tupleInfo.sigmaf = sigma2(tf);
                    tupleInfo.sigmab = sigma2(tb);
                }

                tuples.Add(tupleInfo);
                unknownIndex[i, j] = index;
                ++index;
            }
            f.Clear();
            b.Clear();
        }

        private void refineSample()
        {
            if (ftuples.Count < width * height + 1)
            {
                for (int i = ftuples.Count; i < width * height + 1; i++)
                    ftuples.Add((new FtupleInfo()));
            }

            while (ftuples.Count > width * height + 1)
            {
                ftuples.RemoveAt(ftuples.Count - 1);
            }

            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    double CB = data[i * step + j * channels];
                    double CG = data[i * step + j * channels + 1];
                    double CR = data[i * step + j * channels + 2];
                    int indexf = i * width + j;
                    int gray = tri[i, j];
                    if (gray == 0)
                    {
                        ftuples[indexf].FB = CB;
                        ftuples[indexf].FG = CG;
                        ftuples[indexf].FR = CR;

                        ftuples[indexf].BB = CB;
                        ftuples[indexf].BG = CG;
                        ftuples[indexf].BR = CR;
                        ftuples[indexf].alphar = 0;
                        ftuples[indexf].confidence = 1;
                        alpha[i, j] = 0;
                    }
                    else if (gray == 255)
                    {
                        ftuples[indexf].FB = CB;
                        ftuples[indexf].FG = CG;
                        ftuples[indexf].FR = CR;

                        ftuples[indexf].BB = CB;
                        ftuples[indexf].BG = CG;
                        ftuples[indexf].BR = CR;
                        ftuples[indexf].alphar = 1;
                        ftuples[indexf].confidence = 1;
                        alpha[i, j] = 255;
                    }
                }
            }

            for (int i = 0; i < uT.Count; i++)
            {
                int xi = uT[i].X;
                int yj = uT[i].Y;
                int i1 = Math.Max(0, xi - 5);
                int i2 = Math.Min(xi + 5, height - 1);
                int j1 = Math.Max(0, yj - 5);
                int j2 = Math.Min(yj + 5, width - 1);

                double[] minvalue = { 1e10, 1e10, 1e10 };
                Drawing.Point[] p = new Drawing.Point[3];
                int num = 0;
                for (int k = i1; k <= i2; ++k)
                {
                    for (int l = j1; l <= j2; ++l)
                    {
                        int temp = tri[k, l];

                        if (temp == 0 || temp == 255)
                            continue;

                        TupleInfo t = tuples[unknownIndex[k, l]];
                        if (t.flag == -1)
                            continue;

                        double m = mP(xi, yj, t.FB, t.FG, t.FR, t.BB, t.BG, t.BR);
                        if (m > minvalue[2])
                            continue;

                        if (m < minvalue[0])
                        {
                            minvalue[2] = minvalue[1];
                            p[2] = p[1];

                            minvalue[1] = minvalue[0];
                            p[1] = p[0];

                            minvalue[0] = m;
                            p[0].X = k;
                            p[0].Y = l;

                            ++num;
                        }
                        else if (m < minvalue[1])
                        {
                            minvalue[2] = minvalue[1];
                            p[2] = p[1];

                            minvalue[1] = m;
                            p[1].X = k;
                            p[1].Y = l;

                            ++num;
                        }
                        else if (m < minvalue[2])
                        {
                            minvalue[2] = m;
                            p[2].X = k;
                            p[2].Y = l;

                            ++num;
                        }
                    }
                }

                num = Math.Min(num, 3);

                double FB = 0;
                double FG = 0;
                double FR = 0;

                double BB = 0;
                double BG = 0;
                double BR = 0;

                double sf = 0;
                double sb = 0;

                for (int k = 0; k < num; ++k)
                {
                    int idx = unknownIndex[p[k].X, p[k].Y];
                    FB += tuples[idx].FB;
                    FG += tuples[idx].FG;
                    FR += tuples[idx].FR;

                    BB += tuples[idx].BB;
                    BG += tuples[idx].BG;
                    BR += tuples[idx].BR;

                    sf += tuples[idx].sigmaf;
                    sb += tuples[idx].sigmab;
                }

                FB /= (num + 1e-10);
                FG /= (num + 1e-10);
                FR /= (num + 1e-10);

                BB /= (num + 1e-10);
                BG /= (num + 1e-10);
                BR /= (num + 1e-10);

                sf /= (num + 1e-10);
                sb /= (num + 1e-10);

                double PB = data[xi * step + yj * channels];
                double PG = data[xi * step + yj * channels + 1];
                double PR = data[xi * step + yj * channels + 2];

                double df = this.CalcColorDistance(PB, PG, PR, FB, FG, FR);
                double db = this.CalcColorDistance(PB, PG, PR, BB, BG, BR);

                double tFB = FB;
                double tFG = FG;
                double tFR = FR;

                double tBB = BB;
                double tBG = BG;
                double tBR = BR;

                int index = xi * width + yj;
                if (df < sf)
                {
                    FB = PB;
                    FG = PG;
                    FR = PR;
                }
                if (db < sb)
                {
                    BB = PB;
                    BG = PG;
                    BR = PR;
                }
                if (FB == BB && FG == BG && FR == BR)
                    ftuples[index].confidence = 0.00000001;
                else
                    ftuples[index].confidence = Math.Exp(-10 * mP(xi, yj, tFB, tFG, tFR, tBB, tBG, tBR));


                ftuples[index].FB = FB;
                ftuples[index].FG = FG;
                ftuples[index].FR = FR;
                ftuples[index].BB = BB;
                ftuples[index].BG = BG;
                ftuples[index].BR = BR;

                ftuples[index].alphar = Math.Max(0.0, Math.Min(1.0, this.CalcAlpha(PB, PG, PR, FB, FG, FR, BB, BG, BR)));
            }
            tuples.Clear();
        }

        private void localSmooth()
        {
            double sig2 = 100.0 / (9 * 3.1415926);
            double r = 3 * Math.Sqrt(sig2);

            for (int it = 0; it != uT.Count; ++it)
            {
                int xi = uT[it].X;
                int yj = uT[it].Y;

                int i1 = Math.Max(0, (int)(xi - r));
                int i2 = Math.Min((int)(xi + r), height - 1);
                int j1 = Math.Max(0, (int)(yj - r));
                int j2 = Math.Min((int)(yj + r), width - 1);

                int indexp = xi * width + yj;
                FtupleInfo ptuple = ftuples[indexp];

                double wcfsumdown = 0;
                double wcbsumdown = 0;
                double wfbsumup = 0;
                double wfbsundown = 0;
                double wasumup = 0;
                double wasumdown = 0;

                double wcfsumupB = 0;
                double wcfsumupG = 0;
                double wcfsumupR = 0;

                double wcbsumupB = 0;
                double wcbsumupG = 0;
                double wcbsumupR = 0;


                for (int k = i1; k <= i2; ++k)
                {
                    for (int l = j1; l <= j2; ++l)
                    {
                        int indexq = k * width + l;
                        FtupleInfo qtuple = ftuples[indexq];

                        double d = dP(new Drawing.Point(xi, yj), new Drawing.Point(k, l));

                        if (d > r)
                            continue;

                        double wc;
                        if (d == 0)
                            wc = Math.Exp(-(d * d) / sig2) * qtuple.confidence;
                        else
                            wc = Math.Exp(-(d * d) / sig2) * qtuple.confidence * Math.Abs(qtuple.alphar - ptuple.alphar);

                        wcfsumdown += wc * qtuple.alphar;
                        wcbsumdown += wc * (1 - qtuple.alphar);

                        wcfsumupB += wc * qtuple.alphar * qtuple.FB;
                        wcfsumupG += wc * qtuple.alphar * qtuple.FG;
                        wcfsumupR += wc * qtuple.alphar * qtuple.FR;

                        wcbsumupB += wc * (1 - qtuple.alphar) * qtuple.BB;
                        wcbsumupG += wc * (1 - qtuple.alphar) * qtuple.BG;
                        wcbsumupR += wc * (1 - qtuple.alphar) * qtuple.BR;

                        double wfb = qtuple.confidence * qtuple.alphar * (1 - qtuple.alphar);
                        wfbsundown += wfb;
                        wfbsumup += wfb * Math.Sqrt(this.CalcColorDistance(qtuple.FB, qtuple.FG, qtuple.FR, qtuple.BB, qtuple.BG, qtuple.BR));

                        double delta = 0;
                        if (tri[k, l] == 0 || tri[k, l] == 255)
                            delta = 1;
                        double wa = qtuple.confidence * Math.Exp(-(d * d) / sig2) + delta;
                        wasumdown += wa;
                        wasumup += wa * qtuple.alphar;
                    }
                }

                double CB = data[xi * step + yj * channels];
                double CG = data[xi * step + yj * channels + 1];
                double CR = data[xi * step + yj * channels + 2];

                double BB = Math.Min(255.0, Math.Max(0.0, wcbsumupB / (wcbsumdown + 1e-200)));
                double BG = Math.Min(255.0, Math.Max(0.0, wcbsumupG / (wcbsumdown + 1e-200)));
                double BR = Math.Min(255.0, Math.Max(0.0, wcbsumupR / (wcbsumdown + 1e-200)));

                double FB = Math.Min(255.0, Math.Max(0.0, wcfsumupB / (wcfsumdown + 1e-200)));
                double FG = Math.Min(255.0, Math.Max(0.0, wcfsumupG / (wcfsumdown + 1e-200)));
                double FR = Math.Min(255.0, Math.Max(0.0, wcfsumupR / (wcfsumdown + 1e-200)));

                double dfb = wfbsumup / (wfbsundown + 1e-200);
                double distanceColor = this.CalcColorDistance(FB, FG, FR, BB, BG, BR);
                double conp = Math.Min(1.0, Math.Sqrt(distanceColor) / dfb) * Math.Exp(-10 * mP(xi, yj, FB, FG, FR, BB, BG, BR));
                double alp = wasumup / (wasumdown + 1e-200);
                double dAlpha = this.CalcAlpha(CB, CG, CR, FB, FG, FR, BB, BG, BR);
                double alpha_t = conp * dAlpha + (1 - conp) * Math.Max(0.0, Math.Min(alp, 1.0));

                alpha[xi, yj] = (int)(alpha_t * 255);
            }
            ftuples.Clear();
        }

        public void solveAlpha()
        {
            expandKnown();
            gathering();
            refineSample();
            localSmooth();
        }

        public Drawing.Bitmap GetMattingImage()
        {
            Drawing.Bitmap oBitmap = pImg.Bitmap;
            Drawing.Bitmap newBitmap = new Drawing.Bitmap(oBitmap.Width, oBitmap.Height,
                Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Drawing.Graphics g = Drawing.Graphics.FromImage(newBitmap))
            {
                g.DrawImage(oBitmap, 0, 0);
                for (int i = 0; i < oBitmap.Height; i++)
                {
                    for (int j = 0; j < oBitmap.Width; j++)
                    {
                        double dAlpha = alpha[i, j] / 255.0;
                        if (dAlpha > 0)
                        {
                            Drawing.Color pixelColor = oBitmap.GetPixel(j, i);
                            Drawing.Color newColor = Drawing.Color.FromArgb(alpha[i, j], pixelColor.R, pixelColor.G, pixelColor.B);
                            newBitmap.SetPixel(j, i, newColor);
                        }
                        else
                            newBitmap.SetPixel(j, i, Drawing.Color.Transparent);
                    }
                }
            }
            return newBitmap;
        }
    }
}
