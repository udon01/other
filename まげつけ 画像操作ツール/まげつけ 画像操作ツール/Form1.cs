using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace まげつけ_画像操作ツール
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
        public static byte[] dainyu_none = new byte[4] { 0x00, 0x00, 0x00, 0x00 };

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
                if (fileextension != ".exe" && fileextension != ".EXE")
                    goto labelfile;
                else
                    goto labelfinish;

                labelfile:;

                // ドラッグ＆ドロップされたファイル
                FileStream fsr = new FileStream(path[b], FileMode.Open, FileAccess.Read);
                byte[] bs = new byte[fsr.Length];
                fsr.Read(bs, 0, bs.Length);
                fsr.Close();

                int writefile_seek = 0;
                byte[] bsbmp = new byte[0];
                if (fileextension == ".bmp" || fileextension == ".BMP")
                {
                    writefile_seek = Getbyteint(bs, 10);
                    Array.Resize(ref bsbmp, writefile_seek);
                    Array.Copy(bs, 0, bsbmp, 0, writefile_seek);
                }

                int writefile_count = bs.Length - writefile_seek;
                byte[] bsw = new byte[0];
                filecount = path.Count() - 1;
                bgWorker.ReportProgress(b - 1);

                for (int i = 0; i < writefile_count; i++)
                {
                    byte[] bsrep = new byte[1];
                    Array.Copy(bs, writefile_seek + writefile_count - i - 1, bsrep, 0, 1);
                    string rep = BitConverter.ToString(bsrep);
                    //string rep1 = rep.Substring(0, 1);
                    //string rep2 = rep.Substring(1, 1);
                    //rep = rep2 + rep1;
                    bsrep[0] = Convert.ToByte(rep, 16);
                    Array.Resize(ref bsw, bsw.Length + 1);
                    Array.Copy(bsrep, 0, bsw, i, 1);
                }
                FileStream fsw = new FileStream(Path.GetDirectoryName(path[b]) + @"\" + Path.GetFileNameWithoutExtension(path[b])
                                                + "_new" + fileextension, FileMode.Create, FileAccess.Write);

                if (fileextension == ".bmp" || fileextension == ".BMP")
                    fsw.Write(bsbmp,0, bsbmp.Length);
                fsw.Write(bsw, 0, bsw.Length);

            labelfinish:;
            }
        }
        
        //byte配列4バイトをintに変換して戻す
        public static int Getbyteint(byte[] bytes, int seek)
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
        public static byte[] Gethex(int hex)
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
