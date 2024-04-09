using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ガチャレーシング用tga変換ツール
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
                if (fileextension.ToLower() != ".exe")
                    goto labelfile;
                else
                    goto labelfinish;

                labelfile:;

                // ドラッグ＆ドロップされたファイル
                FileStream fsr = new FileStream(path[b], FileMode.Open, FileAccess.Read);
                byte[] bs = new byte[fsr.Length];
                fsr.Read(bs, 0, bs.Length);
                fsr.Close();

                if (fileextension.ToLower() != ".tga")
                    goto labelfinish;

                int pixelcount_yoko = Getbyte2(bs, 12);
                int pixelcount_tate = Getbyte2(bs, 14);
                byte[] bs_new_gacha = new byte[0];
                byte[] bs_header = new byte[0];
                Array.Resize(ref bs_header, bs_header.Length + 18);
                Array.Copy(bs, 0, bs_header, 0, 18);
                byte[] bsp = new byte[0];
                Array.Resize(ref bsp, pixelcount_yoko * pixelcount_tate * 4);
                Array.Copy(bs, 18, bsp, 0, bsp.Length);
                filecount = pixelcount_tate;
                a = 0;

                string filename_matsubi = Path.GetFileNameWithoutExtension(path[b]);
                if (filename_matsubi.Length >= 7)
                    filename_matsubi = filename_matsubi.Substring(filename_matsubi.Length - 7, 7);
                if (filename_matsubi == "_normal")
                    goto labelgacha;

                //元のtgaに戻す
                byte[] bsp_new = new byte[0];
                int writefile_seek = 0;
                for (int q = 0; q < pixelcount_tate / 64; q++)
                {
                    for (int p = 0; p < 8; p++)
                    {
                        byte[] bsp_yoko_all_1 = new byte[0];
                        byte[] bsp_yoko_all_2 = new byte[0];
                        byte[] bsp_yoko_all_3 = new byte[0];
                        byte[] bsp_yoko_all_4 = new byte[0];
                        byte[] bsp_yoko_all_5 = new byte[0];
                        byte[] bsp_yoko_all_6 = new byte[0];
                        byte[] bsp_yoko_all_7 = new byte[0];
                        byte[] bsp_yoko_all_8 = new byte[0];
                        for (int o = 0; o < 8; o++)
                        {
                            a += 1;
                            bgWorker.ReportProgress(a);
                            byte[] bsp_yoko_all = new byte[0];
                            for (int n = 0; n < 2; n++)
                            {
                                for (int m = 0; m < 2; m++)
                                {
                                    for (int k = 0; k < 2; k++)
                                    {
                                        byte[] bsp_yoko_8 = new byte[0];
                                        int bsp_seek = 0;
                                        for (int j = 0; j < 16; j++)
                                        {
                                            for (int i = 0; i < 2; i++)
                                            {
                                                Array.Resize(ref bsp_yoko_8, bsp_yoko_8.Length + 8);
                                                Array.Copy(bsp, writefile_seek + bsp_seek, bsp_yoko_8, bsp_yoko_8.Length - 8, 8);
                                                Array.Resize(ref bsp_yoko_8, bsp_yoko_8.Length + 8);
                                                Array.Copy(bsp, writefile_seek + bsp_seek + 16, bsp_yoko_8, bsp_yoko_8.Length - 8, 8);
                                                bsp_seek += 64;
                                            }
                                            bsp_seek += 128;
                                        }
                                        Array.Resize(ref bsp_yoko_all, bsp_yoko_all.Length + 512);
                                        Array.Copy(bsp_yoko_8, 0, bsp_yoko_all, bsp_yoko_all.Length - 512, 512);
                                        writefile_seek += 8;
                                    }
                                    writefile_seek += 16;
                                }
                                writefile_seek += 64;
                            }
                            Array.Resize(ref bsp_yoko_all_1, bsp_yoko_all_1.Length + 512);
                            Array.Copy(bsp_yoko_all, 0, bsp_yoko_all_1, bsp_yoko_all_1.Length - 512, 512);
                            Array.Resize(ref bsp_yoko_all_2, bsp_yoko_all_2.Length + 512);
                            Array.Copy(bsp_yoko_all, 512, bsp_yoko_all_2, bsp_yoko_all_2.Length - 512, 512);
                            Array.Resize(ref bsp_yoko_all_3, bsp_yoko_all_3.Length + 512);
                            Array.Copy(bsp_yoko_all, 1024, bsp_yoko_all_3, bsp_yoko_all_3.Length - 512, 512);
                            Array.Resize(ref bsp_yoko_all_4, bsp_yoko_all_4.Length + 512);
                            Array.Copy(bsp_yoko_all, 1536, bsp_yoko_all_4, bsp_yoko_all_4.Length - 512, 512);
                            Array.Resize(ref bsp_yoko_all_5, bsp_yoko_all_5.Length + 512);
                            Array.Copy(bsp_yoko_all, 2048, bsp_yoko_all_5, bsp_yoko_all_5.Length - 512, 512);
                            Array.Resize(ref bsp_yoko_all_6, bsp_yoko_all_6.Length + 512);
                            Array.Copy(bsp_yoko_all, 2560, bsp_yoko_all_6, bsp_yoko_all_6.Length - 512, 512);
                            Array.Resize(ref bsp_yoko_all_7, bsp_yoko_all_7.Length + 512);
                            Array.Copy(bsp_yoko_all, 3072, bsp_yoko_all_7, bsp_yoko_all_7.Length - 512, 512);
                            Array.Resize(ref bsp_yoko_all_8, bsp_yoko_all_8.Length + 512);
                            Array.Copy(bsp_yoko_all, 3584, bsp_yoko_all_8, bsp_yoko_all_8.Length - 512, 512);

                            writefile_seek += pixelcount_yoko * 4 - 256;
                        }
                        Array.Resize(ref bsp_new, bsp_new.Length + bsp_yoko_all_1.Length);
                        Array.Copy(bsp_yoko_all_1, 0, bsp_new, bsp_new.Length - bsp_yoko_all_1.Length, bsp_yoko_all_1.Length);
                        Array.Resize(ref bsp_new, bsp_new.Length + bsp_yoko_all_2.Length);
                        Array.Copy(bsp_yoko_all_2, 0, bsp_new, bsp_new.Length - bsp_yoko_all_2.Length, bsp_yoko_all_2.Length);
                        Array.Resize(ref bsp_new, bsp_new.Length + bsp_yoko_all_3.Length);
                        Array.Copy(bsp_yoko_all_3, 0, bsp_new, bsp_new.Length - bsp_yoko_all_3.Length, bsp_yoko_all_3.Length);
                        Array.Resize(ref bsp_new, bsp_new.Length + bsp_yoko_all_4.Length);
                        Array.Copy(bsp_yoko_all_4, 0, bsp_new, bsp_new.Length - bsp_yoko_all_4.Length, bsp_yoko_all_4.Length);
                        Array.Resize(ref bsp_new, bsp_new.Length + bsp_yoko_all_5.Length);
                        Array.Copy(bsp_yoko_all_5, 0, bsp_new, bsp_new.Length - bsp_yoko_all_5.Length, bsp_yoko_all_5.Length);
                        Array.Resize(ref bsp_new, bsp_new.Length + bsp_yoko_all_6.Length);
                        Array.Copy(bsp_yoko_all_6, 0, bsp_new, bsp_new.Length - bsp_yoko_all_6.Length, bsp_yoko_all_6.Length);
                        Array.Resize(ref bsp_new, bsp_new.Length + bsp_yoko_all_7.Length);
                        Array.Copy(bsp_yoko_all_7, 0, bsp_new, bsp_new.Length - bsp_yoko_all_7.Length, bsp_yoko_all_7.Length);
                        Array.Resize(ref bsp_new, bsp_new.Length + bsp_yoko_all_8.Length);
                        Array.Copy(bsp_yoko_all_8, 0, bsp_new, bsp_new.Length - bsp_yoko_all_8.Length, bsp_yoko_all_8.Length);
                    }
                }
                Array.Resize(ref bs_new_gacha, bs_new_gacha.Length + 18);
                Array.Copy(bs_header, 0, bs_new_gacha, 0, 18);
                Array.Resize(ref bs_new_gacha, bs_new_gacha.Length + bsp_new.Length);
                Array.Copy(bsp_new, 0, bs_new_gacha, bs_new_gacha.Length - bsp_new.Length, bsp_new.Length);
                FileStream fsw = new FileStream(Path.GetDirectoryName(path[b]) + @"\" + Path.GetFileNameWithoutExtension(path[b])
                                                + "_normal" + fileextension, FileMode.Create, FileAccess.Write);
                fsw.Write(bs_new_gacha, 0, bs_new_gacha.Length);
                fsw.Close();
                goto labelfinish;

            labelgacha:;
                //ガチャレーシング形式のtgaに変換する
                writefile_seek = 0;
                bs_new_gacha = new byte[0];
                bsp_new = new byte[0];
                for (int q = 0; q < pixelcount_tate / 8; q++)
                {
                    for (int o = 0; o < pixelcount_yoko / 128; o++)
                    {
                        a += 1;
                        bgWorker.ReportProgress(a + 1);
                        byte[] bsp_yoko_all_gacha = new byte[0];
                        for (int n = 0; n < 16; n++)
                        {
                            for (int m = 0; m < 2; m++)
                            {
                                for (int k = 0; k < 2; k++)
                                {
                                    int bsp_seek = 0;
                                    for (int j = 0; j < 2; j++)
                                    {
                                        for (int i = 0; i < 2; i++)
                                        {
                                            int test = writefile_seek + bsp_seek + pixelcount_yoko * 4;
                                            Array.Resize(ref bsp_yoko_all_gacha, bsp_yoko_all_gacha.Length + 8);
                                            Array.Copy(bsp, writefile_seek + bsp_seek, bsp_yoko_all_gacha, bsp_yoko_all_gacha.Length - 8, 8);
                                            Array.Resize(ref bsp_yoko_all_gacha, bsp_yoko_all_gacha.Length + 8);
                                            Array.Copy(bsp, writefile_seek + bsp_seek + pixelcount_yoko * 4, bsp_yoko_all_gacha, bsp_yoko_all_gacha.Length - 8, 8);
                                            bsp_seek += 8;
                                        }
                                        bsp_seek += pixelcount_yoko * 4 * 2 - 16;
                                    }
                                    writefile_seek += 16;
                                }
                                writefile_seek += pixelcount_yoko * 4 * 4 - 32;
                            }
                            writefile_seek -= pixelcount_yoko * 4 * 8 - 32;
                        }
                        Array.Resize(ref bsp_new, bsp_new.Length + bsp_yoko_all_gacha.Length);
                        Array.Copy(bsp_yoko_all_gacha, 0, bsp_new, bsp_new.Length - bsp_yoko_all_gacha.Length, bsp_yoko_all_gacha.Length);
                    }
                    writefile_seek += pixelcount_yoko * 4 * 7;
                }
                Array.Resize(ref bs_new_gacha, bs_new_gacha.Length + 18);
                Array.Copy(bs_header, 0, bs_new_gacha, 0, 18);
                Array.Resize(ref bs_new_gacha, bs_new_gacha.Length + bsp_new.Length);
                Array.Copy(bsp_new, 0, bs_new_gacha, bs_new_gacha.Length - bsp_new.Length, bsp_new.Length);

                string filename_minus_normal = Path.GetFileNameWithoutExtension(path[b]);
                int _normal_last = filename_minus_normal.LastIndexOf("_normal");
                filename_minus_normal = filename_minus_normal.Remove(_normal_last, 7);

                FileStream fsw_gacha = new FileStream(Path.GetDirectoryName(path[b]) + @"\" + filename_minus_normal + fileextension,
                                                      FileMode.Create, FileAccess.Write);
                fsw_gacha.Write(bs_new_gacha, 0, bs_new_gacha.Length);
                fsw_gacha.Close();

            labelfinish:;
            }
        }

        //byte配列2バイトをintに変換して戻す
        public static int Getbyte2(byte[] bytes, int seek)
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
