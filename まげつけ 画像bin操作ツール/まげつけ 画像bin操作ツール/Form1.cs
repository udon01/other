using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace まげつけ_画像bin操作ツール
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static int a = 0;
        public static int filecount = 0;
        public static bool close = false;
        public static byte[] dainyu_none2 = new byte[2] { 0x00, 0x00 };
        public static byte[] dainyu_none4 = new byte[4] { 0x00, 0x00, 0x00, 0x00 };
        public static byte[] bmp_bm = new byte[2] { 0x42, 0x4D };
        public static byte[] bmp_palettestart = new byte[4] { 0x00, 0x00, 0x7C, 0x00 };
        public static byte[] bmp_unk1 = new byte[2] { 0x01, 0x00 };
        public static byte[] bmp_p16_1 = new byte[2] { 0x04, 0x00 };
        public static byte[] bmp_p256_1 = new byte[2] { 0x08, 0x00 };
        public static byte[] bmp_unk2 = new byte[4] { 0x00, 0x00, 0x23, 0x2E };
        public static byte[] bmp_p16_2 = new byte[4] { 0x00, 0x00, 0x10, 0x00 };
        public static byte[] bmp_p256_2 = new byte[4] { 0x00, 0x00, 0x00, 0x01 };
        public static byte[] bmp_unk3 = new byte[8] { 0xFF, 0x00, 0x00, 0xFF, 0x00, 0x00, 0xFF, 0x00 };
        public static byte[] bmp_bgrs = new byte[4] { 0x42, 0x47, 0x52, 0x73 };
        public static byte[] bmp_unk4 = new byte[4] { 0x02, 0x00, 0x00, 0x00 };
        public static byte[] file_palette256 = new byte[2] { 0x01, 0x00 };
        public static byte[] file_bmpratio = new byte[4] { 0x00, 0x00, 0x80, 0x3F };
        public static byte[] file_head_unk = new byte[4] { 0x01, 0x00, 0x00, 0x00 };
        public static byte[] bsint2 = new byte[2];
        public static byte[] bsint4 = new byte[4];

        private void Form1_Shown(object sender, EventArgs e)
        {
            ProgressBar1.Minimum = 0;
            ProgressBar1.Value = 0;
            BackgroundWorker1.WorkerReportsProgress = true;
            BackgroundWorker1.RunWorkerAsync();
        }

        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBar1.Value = a;
            ProgressBar1.Maximum = filecount;
            Label1.Text = a.ToString() + "/" + filecount.ToString();
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (close == true)
                Close();
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bgWorker = (BackgroundWorker)sender;
            string[] path = Environment.GetCommandLineArgs();
            if (path.Count() != 1)
            {
                Label1.Text = "実行中...";
                close = true;
            }

            for (int b = 0; b < path.Count(); b++)
            {
                string fileextension = Path.GetExtension(path[b]);
                if (fileextension == ".bin" || fileextension == ".BIN")
                    goto labelfile;
                else if (Directory.Exists(path[b]))
                    goto labelfolder;
                else
                    goto labelfinish;

                labelfile:;

                //ドラッグ＆ドロップされたファイル
                FileStream fsr = new FileStream(path[b], FileMode.Open, FileAccess.Read);
                byte[] bs = new byte[fsr.Length];
                fsr.Read(bs, 0, bs.Length);
                fsr.Close();

                filecount = Getbyteint2(bs, 2);
                int palettestart = Getbyteint4(bs, 4);
                int bmpsize_base = 32;
                int palettesize = 0;
                int bmpstart = 64;
                int bmpnext = Getbyteint4(bs, 32);
                int bmpnext_start = 32;

                DirectoryInfo di = Directory.CreateDirectory(Path.GetDirectoryName(path[b]) + @"\" + Path.GetFileNameWithoutExtension(path[b]));

                for (a = 0; a < filecount; a++)
                {
                    byte[] bswi = new byte[bmpnext - bmpstart];
                    byte[] bswi_rep = new byte[0];
                    Array.Copy(bs, bmpstart, bswi, 0, bmpnext - bmpstart);

                    int bmpsize_yoko_1 = Getbyteint2(bs, bmpsize_base + 8);
                    int bmpsize_tate_1 = Getbyteint2(bs, bmpsize_base + 10);
                    int bmpsize_yoko_2 = Getbyteint2(bs, bmpsize_base + 12);
                    int bmpsize_tate_2 = Getbyteint2(bs, bmpsize_base + 14);
                    if (bmpsize_yoko_1 == bmpsize_yoko_2)
                        palettesize = 1024;
                    else
                        palettesize = 64;
                    byte[] bswp = new byte[palettesize];
                    Array.Copy(bs, palettestart, bswp, 0, palettesize);

                    int bmp_yoko_half = bmpsize_yoko_1 / 2;
                    for (int j = 0; j < bswi.Length / bmp_yoko_half; j++)
                    {
                        for (int k = 0; k < bmp_yoko_half; k++)
                        {
                            byte[] bsrep = new byte[1];
                            Array.Copy(bswi, bswi.Length - (bmp_yoko_half * j) - bmp_yoko_half + k, bsrep, 0, 1);
                            if (palettesize == 64)
                            {
                                string rep = BitConverter.ToString(bsrep);
                                string rep1 = rep.Substring(0, 1);
                                string rep2 = rep.Substring(1, 1);
                                rep = rep2 + rep1;
                                bsrep[0] = Convert.ToByte(rep, 16);
                            }
                            Array.Resize(ref bswi_rep, bswi_rep.Length + 1);
                            Array.Copy(bsrep, 0, bswi_rep, bswi_rep.Length - 1, 1);
                        }
                    }

                    byte[] bswbmp = new byte[0];

                    //bmpのヘッダーを作る
                    Array.Resize(ref bswbmp, 2);
                    Array.Copy(bmp_bm, 0, bswbmp, 0, 2);

                    int bmp_filesize = bswi.Length + bswp.Length + 138;
                    bsint2 = Gethex2(bmp_filesize);
                    Array.Resize(ref bswbmp, bswbmp.Length + 2);
                    Array.Copy(bsint2, 0, bswbmp, bswbmp.Length - 2, 2);

                    Array.Resize(ref bswbmp, bswbmp.Length + 4);
                    Array.Copy(dainyu_none4, 0, bswbmp, bswbmp.Length - 4, 4);
                    Array.Resize(ref bswbmp, bswbmp.Length + 2);
                    Array.Copy(dainyu_none2, 0, bswbmp, bswbmp.Length - 2, 2);

                    int bmp_pixelstart = bswp.Length + 138;
                    bsint2 = Gethex2(bmp_pixelstart);
                    Array.Resize(ref bswbmp, bswbmp.Length + 2);
                    Array.Copy(bsint2, 0, bswbmp, bswbmp.Length - 2, 2);

                    Array.Resize(ref bswbmp, bswbmp.Length + 4);
                    Array.Copy(bmp_palettestart, 0, bswbmp, bswbmp.Length - 4, 4);
                    Array.Resize(ref bswbmp, bswbmp.Length + 2);
                    Array.Copy(dainyu_none2, 0, bswbmp, bswbmp.Length - 2, 2);

                    bsint2 = Gethex2(bmpsize_yoko_1);
                    Array.Resize(ref bswbmp, bswbmp.Length + 2);
                    Array.Copy(bsint2, 0, bswbmp, bswbmp.Length - 2, 2);
                    Array.Resize(ref bswbmp, bswbmp.Length + 2);
                    Array.Copy(dainyu_none2, 0, bswbmp, bswbmp.Length - 2, 2);

                    bsint2 = Gethex2(bmpsize_tate_1);
                    Array.Resize(ref bswbmp, bswbmp.Length + 2);
                    Array.Copy(bsint2, 0, bswbmp, bswbmp.Length - 2, 2);
                    Array.Resize(ref bswbmp, bswbmp.Length + 2);
                    Array.Copy(dainyu_none2, 0, bswbmp, bswbmp.Length - 2, 2);

                    Array.Resize(ref bswbmp, bswbmp.Length + 2);
                    Array.Copy(bmp_unk1, 0, bswbmp, bswbmp.Length - 2, 2);

                    if (palettesize == 64)
                    {
                        Array.Resize(ref bswbmp, bswbmp.Length + 2);
                        Array.Copy(bmp_p16_1, 0, bswbmp, bswbmp.Length - 2, 2);
                    }
                    else
                    {
                        Array.Resize(ref bswbmp, bswbmp.Length + 2);
                        Array.Copy(bmp_p256_1, 0, bswbmp, bswbmp.Length - 2, 2);
                    }

                    Array.Resize(ref bswbmp, bswbmp.Length + 4);
                    Array.Copy(dainyu_none4, 0, bswbmp, bswbmp.Length - 4, 4);

                    int bmp_pixelsize = bswi.Length;
                    bsint2 = Gethex2(bmp_pixelsize);
                    Array.Resize(ref bswbmp, bswbmp.Length + 2);
                    Array.Copy(bsint2, 0, bswbmp, bswbmp.Length - 2, 2);

                    for (int j = 0; j < 2; j++)
                    {
                        Array.Resize(ref bswbmp, bswbmp.Length + 4);
                        Array.Copy(bmp_unk2, 0, bswbmp, bswbmp.Length - 4, 4);
                    }

                    for (int j = 0; j < 2; j++)
                    {
                        if (palettesize == 64)
                        {
                            Array.Resize(ref bswbmp, bswbmp.Length + 4);
                            Array.Copy(bmp_p16_2, 0, bswbmp, bswbmp.Length - 4, 4);
                        }
                        else
                        {
                            Array.Resize(ref bswbmp, bswbmp.Length + 4);
                            Array.Copy(bmp_p256_2, 0, bswbmp, bswbmp.Length - 4, 4);
                        }
                    }

                    Array.Resize(ref bswbmp, bswbmp.Length + 4);
                    Array.Copy(dainyu_none4, 0, bswbmp, bswbmp.Length - 4, 4);
                    Array.Resize(ref bswbmp, bswbmp.Length + 8);
                    Array.Copy(bmp_unk3, 0, bswbmp, bswbmp.Length - 8, 8);
                    Array.Resize(ref bswbmp, bswbmp.Length + 4);
                    Array.Copy(dainyu_none4, 0, bswbmp, bswbmp.Length - 4, 4);
                    Array.Resize(ref bswbmp, bswbmp.Length + 2);
                    Array.Copy(dainyu_none2, 0, bswbmp, bswbmp.Length - 2, 2);

                    Array.Resize(ref bswbmp, bswbmp.Length + 4);
                    Array.Copy(bmp_bgrs, 0, bswbmp, bswbmp.Length - 4, 4);

                    for (int j = 0; j < 12; j++)
                    {
                        Array.Resize(ref bswbmp, bswbmp.Length + 4);
                        Array.Copy(dainyu_none4, 0, bswbmp, bswbmp.Length - 4, 4);
                    }

                    Array.Resize(ref bswbmp, bswbmp.Length + 4);
                    Array.Copy(bmp_unk4, 0, bswbmp, bswbmp.Length - 4, 4);

                    for (int j = 0; j < 3; j++)
                    {
                        Array.Resize(ref bswbmp, bswbmp.Length + 4);
                        Array.Copy(dainyu_none4, 0, bswbmp, bswbmp.Length - 4, 4);
                    }

                    Array.Resize(ref bswbmp, bswbmp.Length + bswp.Length);
                    Array.Copy(bswp, 0, bswbmp, bswbmp.Length - bswp.Length, bswp.Length);
                    Array.Resize(ref bswbmp, bswbmp.Length + bswi_rep.Length);
                    Array.Copy(bswi_rep, 0, bswbmp, bswbmp.Length - bswi_rep.Length, bswi_rep.Length);

                    FileStream fsw = new FileStream(di + @"\" + string.Format("{0:0000}", a) + ".bmp", FileMode.Create, FileAccess.Write);
                    fsw.Write(bswbmp, 0, bswbmp.Length);
                    fsw.Close();

                    //透過画像の場合ヘッダーを出力
                    if (Getbyteint4(bs, bmpsize_base + 16) != 0 || Getbyteint4(bs, bmpsize_base + 28) != 0)
                    {
                        FileStream fswbin = new FileStream(di + @"\" + string.Format("{0:0000}", a) + ".bin", FileMode.Create, FileAccess.Write);
                        byte[] bs_head = new byte[32];
                        Array.Copy(bs, bmpsize_base, bs_head, 0, 32);
                        fswbin.Write(bs_head, 0, bs_head.Length);
                        fswbin.Close();
                    }

                    palettestart += palettesize;
                    bmpsize_base = bmpnext;
                    bmpstart = bmpnext + 32;
                    bmpnext_start = bmpnext;
                    bmpnext = Getbyteint4(bs, bmpnext_start);

                    bgWorker.ReportProgress(a);
                }

                //パレットの下になんか入ってたら出力する
                if (Getbyteint4(bs, 24) != 0)
                {
                    FileStream fswunk = new FileStream(di + @"\" + "unknown" + ".bin", FileMode.Create, FileAccess.Write);
                    int unk_length = bs.Length - Getbyteint4(bs, 12);
                    byte[] bs_unk = new byte[unk_length];
                    Array.Copy(bs, Getbyteint4(bs, 12), bs_unk, 0, unk_length);
                    fswunk.Write(bs_unk, 0, bs_unk.Length);
                    fswunk.Close();
                }

                goto labelfinish;

            labelfolder:;

                string[] files = Directory.GetFiles(path[b], "*.bmp");
                filecount = files.Count();
                byte[] bsf = new byte[0];
                byte[] bspf = new byte[0];
                int filenext_tortal = 0;
                int palette16count = 0;
                int palette256count = 0;

                for (a = 0; a < filecount; a++)
                {
                    FileStream fsrf = new FileStream(files[a], FileMode.Open, FileAccess.Read);
                    byte[] bsrf = new byte[fsrf.Length];
                    fsrf.Read(bsrf, 0, bsrf.Length);
                    fsrf.Close();

                    byte[] bsf_bmp_head = new byte[0];

                    //パレット数ごとにファイルをカウントする
                    int palettesize_f = Getbyteint2(bsrf, 10) - Getbyteint2(bsrf, 14) - 14;
                    if (palettesize_f == 64)
                    {
                        Array.Resize(ref bsf_bmp_head, bsf_bmp_head.Length + 2);
                        Array.Copy(dainyu_none2, 0, bsf_bmp_head, bsf_bmp_head.Length - 2, 2);
                        bsint2 = Gethex2(palette16count);
                        Array.Resize(ref bsf_bmp_head, bsf_bmp_head.Length + 2);
                        Array.Copy(bsint2, 0, bsf_bmp_head, bsf_bmp_head.Length - 2, 2);
                        palette16count += 1;
                    }
                    else
                    {
                        Array.Resize(ref bsf_bmp_head, bsf_bmp_head.Length + 2);
                        Array.Copy(file_palette256, 0, bsf_bmp_head, bsf_bmp_head.Length - 2, 2);
                        bsint2 = Gethex2(palette256count);
                        Array.Resize(ref bsf_bmp_head, bsf_bmp_head.Length + 2);
                        Array.Copy(bsint2, 0, bsf_bmp_head, bsf_bmp_head.Length - 2, 2);
                        palette256count += 1;
                    }

                    //横の長さ
                    Array.Resize(ref bsf_bmp_head, bsf_bmp_head.Length + 2);
                    Array.Copy(bsrf, 18, bsf_bmp_head, bsf_bmp_head.Length - 2, 2);

                    //縦の長さ
                    Array.Resize(ref bsf_bmp_head, bsf_bmp_head.Length + 2);
                    Array.Copy(bsrf, 22, bsf_bmp_head, bsf_bmp_head.Length - 2, 2);

                    //パレットサイズを確認する
                    int bmpsize_yoko_2_f = Getbyteint2(bsrf, 18);
                    bmpsize_yoko_2_f /= 2;
                    if (palettesize_f == 64)
                    {
                        bsint2 = Gethex2(bmpsize_yoko_2_f);
                        Array.Resize(ref bsf_bmp_head, bsf_bmp_head.Length + 2);
                        Array.Copy(bsint2, 0, bsf_bmp_head, bsf_bmp_head.Length - 2, 2);
                        Array.Resize(ref bsf_bmp_head, bsf_bmp_head.Length + 2);
                        Array.Copy(bsrf, 22, bsf_bmp_head, bsf_bmp_head.Length - 2, 2);
                    }
                    else
                    {
                        Array.Resize(ref bsf_bmp_head, bsf_bmp_head.Length + 2);
                        Array.Copy(bsrf, 18, bsf_bmp_head, bsf_bmp_head.Length - 2, 2);
                        Array.Resize(ref bsf_bmp_head, bsf_bmp_head.Length + 2);
                        Array.Copy(bsrf, 22, bsf_bmp_head, bsf_bmp_head.Length - 2, 2);
                    }

                    //不明(0以外のファイルもある)
                    Array.Resize(ref bsf_bmp_head, bsf_bmp_head.Length + 4);
                    Array.Copy(dainyu_none4, 0, bsf_bmp_head, bsf_bmp_head.Length - 4, 4);

                    //画像の比率
                    for (int i = 0; i < 2; i++)
                    {
                        Array.Resize(ref bsf_bmp_head, bsf_bmp_head.Length + 4);
                        Array.Copy(file_bmpratio, 0, bsf_bmp_head, bsf_bmp_head.Length - 4, 4);
                    }
                    Array.Resize(ref bsf_bmp_head, bsf_bmp_head.Length + 4);
                    Array.Copy(dainyu_none4, 0, bsf_bmp_head, bsf_bmp_head.Length - 4, 4);

                    //ピクセル部分を並び替える
                    int bmp_head_size = Getbyteint2(bsrf, 10);
                    byte[] bswi_f = new byte[bsrf.Length - bmp_head_size];
                    Array.Copy(bsrf, bmp_head_size, bswi_f, 0, bswi_f.Length);
                    byte[] bswi_rep_f = new byte[0];
                    for (int j = 0; j < bswi_f.Length / bmpsize_yoko_2_f; j++)
                    {
                        for (int k = 0; k < bmpsize_yoko_2_f; k++)
                        {
                            byte[] bsrep = new byte[1];
                            Array.Copy(bswi_f, bswi_f.Length - (bmpsize_yoko_2_f * j) - bmpsize_yoko_2_f + k, bsrep, 0, 1);
                            if (palettesize_f == 64)
                            {
                                string rep = BitConverter.ToString(bsrep);
                                string rep1 = rep.Substring(0, 1);
                                string rep2 = rep.Substring(1, 1);
                                rep = rep2 + rep1;
                                bsrep[0] = Convert.ToByte(rep, 16);
                            }
                            Array.Resize(ref bswi_rep_f, bswi_rep_f.Length + 1);
                            Array.Copy(bsrep, 0, bswi_rep_f, bswi_rep_f.Length - 1, 1);
                        }
                    }

                    //次の画像のアドレス位置
                    int filenext = filenext_tortal + 36 + bsf_bmp_head.Length + bswi_rep_f.Length;
                    bsint4 = Gethex4(filenext);
                    Array.Resize(ref bsf, bsf.Length + 4);
                    Array.Copy(bsint4, 0, bsf, bsf.Length - 4, 4);

                    //画像のヘッダーが存在する場合コピーする
                    string bmpheadpath = path[b] + @"\" + string.Format("{0:0000}", a) + ".bin";
                    if (File.Exists(bmpheadpath))
                    {
                        FileStream fsr_head_f = new FileStream(bmpheadpath, FileMode.Open, FileAccess.Read);
                        byte[] bsr_head_f = new byte[fsr_head_f.Length];
                        fsr_head_f.Read(bsr_head_f, 0, bsr_head_f.Length);
                        fsr_head_f.Close();
                        Array.Resize(ref bsf, bsf.Length + 12);
                        Array.Copy(bsf_bmp_head, 0, bsf, bsf.Length - 12, 12);
                        Array.Resize(ref bsf, bsf.Length + 4);
                        Array.Copy(bsr_head_f, 16, bsf, bsf.Length - 4, 4);
                        Array.Resize(ref bsf, bsf.Length + 8);
                        Array.Copy(bsf_bmp_head, 16, bsf, bsf.Length - 8, 8);
                        Array.Resize(ref bsf, bsf.Length + 4);
                        Array.Copy(bsr_head_f, 28, bsf, bsf.Length - 4, 4);
                        Array.Resize(ref bsf, bsf.Length + bswi_rep_f.Length);
                        Array.Copy(bswi_rep_f, 0, bsf, bsf.Length - bswi_rep_f.Length, bswi_rep_f.Length);
                    }
                    else
                    {
                        Array.Resize(ref bsf, bsf.Length + bsf_bmp_head.Length);
                        Array.Copy(bsf_bmp_head, 0, bsf, bsf.Length - bsf_bmp_head.Length, bsf_bmp_head.Length);
                        Array.Resize(ref bsf, bsf.Length + bswi_rep_f.Length);
                        Array.Copy(bswi_rep_f, 0, bsf, bsf.Length - bswi_rep_f.Length, bswi_rep_f.Length);
                    }

                    //パレットをコピー
                    Array.Resize(ref bspf, bspf.Length + palettesize_f);
                    Array.Copy(bsrf, Getbyteint2(bsrf, 14) + 14, bspf, bspf.Length - palettesize_f, palettesize_f);

                    filenext_tortal += bswi_rep_f.Length + 32;
                    bgWorker.ReportProgress(a);
                }

                //ファイル全体のヘッダーを作る
                byte[] bsf_head = new byte[2];
                Array.Copy(dainyu_none2, 0, bsf_head, 0, 2);
                bsint2 = Gethex2(filecount);
                Array.Resize(ref bsf_head, bsf_head.Length + 2);
                Array.Copy(bsint2, 0, bsf_head, bsf_head.Length - 2, 2);

                bsint4 = Gethex4(bsf.Length + 64);
                Array.Resize(ref bsf_head, bsf_head.Length + 4);
                Array.Copy(bsint4, 0, bsf_head, bsf_head.Length - 4, 4);

                bsint4 = Gethex4(bsf.Length + 64 + (palette16count * 64));
                Array.Resize(ref bsf_head, bsf_head.Length + 4);
                Array.Copy(bsint4, 0, bsf_head, bsf_head.Length - 4, 4);

                bsint4 = Gethex4(bsf.Length + 64 + bspf.Length);
                Array.Resize(ref bsf_head, bsf_head.Length + 4);
                Array.Copy(bsint4, 0, bsf_head, bsf_head.Length - 4, 4);

                bsint4 = Gethex4(palette16count);
                Array.Resize(ref bsf_head, bsf_head.Length + 4);
                Array.Copy(bsint4, 0, bsf_head, bsf_head.Length - 4, 4);

                bsint4 = Gethex4(palette256count);
                Array.Resize(ref bsf_head, bsf_head.Length + 4);
                Array.Copy(bsint4, 0, bsf_head, bsf_head.Length - 4, 4);

                string unk_path = path[b] + @"\" + "unknown" + ".bin";
                if (File.Exists(unk_path))
                {
                    Array.Resize(ref bsf_head, bsf_head.Length + 4);
                    Array.Copy(file_head_unk, 0, bsf_head, bsf_head.Length - 4, 4);
                }
                else
                {
                    Array.Resize(ref bsf_head, bsf_head.Length + 4);
                    Array.Copy(dainyu_none4, 0, bsf_head, bsf_head.Length - 4, 4);
                }
                Array.Resize(ref bsf_head, bsf_head.Length + 4);
                Array.Copy(dainyu_none4, 0, bsf_head, bsf_head.Length - 4, 4);

                FileStream fsw_f = new FileStream(path[b] + ".BIN", FileMode.Create, FileAccess.Write);
                fsw_f.Write(bsf_head, 0, bsf_head.Length);
                fsw_f.Write(bsf, 0, bsf.Length);

                for (int i = 0; i < 8; i++)
                {
                    fsw_f.Write(dainyu_none4, 0, 4);
                }
                fsw_f.Write(bspf, 0, bspf.Length);

                if (File.Exists(unk_path))
                {
                    FileStream fsw_unk_f = new FileStream(path[b] + @"\unknown.bin", FileMode.Open, FileAccess.Read);
                    byte[] unk_r = new byte[fsw_unk_f.Length];
                    fsw_unk_f.Read(unk_r, 0, unk_r.Length);
                    fsw_unk_f.Close();
                    fsw_f.Write(unk_r, 0, unk_r.Length);
                }

                fsw_f.Close();


            labelfinish:;
            }
        }

        //byte配列1バイトをintに変換して戻す
        public static int Getbyteint1(byte[] bytes, int seek)
        {
            byte[] byte1 = new byte[1];
            Array.Copy(bytes, seek, byte1, 0, 1);

            string str1 = BitConverter.ToString(byte1);
            int returnint = Convert.ToInt32(str1, 16);

            return returnint;
        }

        //byte配列2バイトをintに変換して戻す
        public static int Getbyteint2(byte[] bytes, int seek)
        {
            byte[] byte1 = new byte[1];
            Array.Copy(bytes, seek, byte1, 0, 1);
            byte[] byte2 = new byte[1];
            Array.Copy(bytes, seek + 1, byte2, 0, 1);

            string str1 = BitConverter.ToString(byte1);
            string str2 = BitConverter.ToString(byte2);
            int bytelength = 0;

            if (str2 != "00")
            {
                bytelength = 2;
                goto label_byteget;
            }
            else if (str1 != "00")
            {
                bytelength = 1;
                goto label_byteget;
            }

            else
                return 0;

            label_byteget:;

            string str16 = "";
            if (bytelength == 1)
                str16 = str1;
            else if (bytelength == 2)
                str16 = str2 + str1;

            int returnint = Convert.ToInt32(str16, 16);

            return returnint;
        }

        //byte配列4バイトをintに変換して戻す
        public static int Getbyteint4(byte[] bytes, int seek)
        {
            byte[] byte1 = new byte[1];
            Array.Copy(bytes, seek, byte1, 0, 1);
            byte[] byte2 = new byte[1];
            Array.Copy(bytes, seek + 1, byte2, 0, 1);
            byte[] byte3 = new byte[1];
            Array.Copy(bytes, seek + 2, byte3, 0, 1);
            byte[] byte4 = new byte[1];
            Array.Copy(bytes, seek + 3, byte4, 0, 1);

            string str1 = BitConverter.ToString(byte1);
            string str2 = BitConverter.ToString(byte2);
            string str3 = BitConverter.ToString(byte3);
            string str4 = BitConverter.ToString(byte4);
            int bytelength = 0;

            if (str4 != "00")
            {
                bytelength = 4;
                goto label_byteget;
            }
            else if (str3 != "00")
            {
                bytelength = 3;
                goto label_byteget;
            }
            else if (str2 != "00")
            {
                bytelength = 2;
                goto label_byteget;
            }
            else if (str1 != "00")
            {
                bytelength = 1;
                goto label_byteget;
            }

            else
                return 0;

            label_byteget:;

            string str16 = "";
            if (bytelength == 1)
                str16 = str1;
            else if (bytelength == 2)
                str16 = str2 + str1;
            else if (bytelength == 3)
                str16 = str3 + str2 + str1;
            else if (bytelength == 4)
                str16 = str4 + str3 + str2 + str1;

            int returnint = Convert.ToInt32(str16, 16);

            return returnint;
        }

        //intをbyte配列2バイト(リトルエンディアン)に変換して戻す
        public static byte[] Gethex2(int hex)
        {
            string hexstr = hex.ToString("X");
            if (hexstr.Length == 1 || hexstr.Length == 3)
                hexstr = "0" + hexstr;

            if (hexstr.Length == 2)
                hexstr = "00" + hexstr;

            string hexstr1 = hexstr.Substring(0, 2);
            string hexstr2 = hexstr.Substring(2, 2);
            hexstr = hexstr2 + hexstr1;

            byte[] hexbyte = new byte[2];
            hexbyte = StringToBytes(hexstr);
            return hexbyte;
        }

        //intをbyte配列4バイト(リトルエンディアン)に変換して戻す
        public static byte[] Gethex4(int hex)
        {
            string hexstr = hex.ToString("X");
            if (hexstr.Length == 1 || hexstr.Length == 3 || hexstr.Length == 5 || hexstr.Length == 7)
                hexstr = "0" + hexstr;

            if (hexstr.Length == 2)
                hexstr = hexstr + "000000";

            else if (hexstr.Length == 4)
            {
                string hexstr1 = hexstr.Substring(0, 2);
                string hexstr2 = hexstr.Substring(2, 2);
                hexstr = hexstr2 + hexstr1 + "0000";
            }

            else if (hexstr.Length == 6)
            {
                string hexstr1 = hexstr.Substring(0, 2);
                string hexstr2 = hexstr.Substring(2, 2);
                string hexstr3 = hexstr.Substring(4, 2);
                hexstr = hexstr3 + hexstr2 + hexstr1 + "00";
            }

            else if (hexstr.Length == 8)
            {
                string hexstr1 = hexstr.Substring(0, 2);
                string hexstr2 = hexstr.Substring(2, 2);
                string hexstr3 = hexstr.Substring(4, 2);
                string hexstr4 = hexstr.Substring(6, 2);
                hexstr = hexstr4 + hexstr3 + hexstr2 + hexstr1;
            }
            byte[] hexbyte = new byte[4];
            hexbyte = StringToBytes(hexstr);
            return hexbyte;
        }

        // 16進数文字列 => Byte配列
        public static byte[] StringToBytes(string str)
        {
            var bs = new List<byte>();
            for (int i = 0; i < str.Length / 2; i++)
            {
                bs.Add(Convert.ToByte(str.Substring(i * 2, 2), 16));
            }
            return bs.ToArray();
        }
    }
}
