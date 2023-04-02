using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace リッジレーサーV用R5.ALL操作ツール
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
        public static string JP = "SLPS_200.01";
        public static string US = "SLUS_200.02";
        public static string PAL = "SCES_500.00";
        public static string JP_f = @"\SLPS_200.01";
        public static string US_f = @"\SLUS_200.02";
        public static string PAL_f = @"\SCES_500.00";

        private void Form1_Shown(object sender, EventArgs e)
        {
            ProgressBar1.Minimum = 0;
            ProgressBar1.Value = 0;
            BackgroundWorker1.WorkerReportsProgress = true;
            BackgroundWorker1.RunWorkerAsync();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void radioJP_MouseClick(object sender, MouseEventArgs e)
        {
            radioJP.Checked = true;
            radioUS.Checked = false;
            radioPAL.Checked = false;
        }

        private void radioUS_MouseClick(object sender, MouseEventArgs e)
        {
            radioJP.Checked = false;
            radioUS.Checked = true;
            radioPAL.Checked = false;
        }

        private void radioPAL_MouseClick(object sender, MouseEventArgs e)
        {
            radioJP.Checked = false;
            radioUS.Checked = false;
            radioPAL.Checked = true;
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
                if (b == 0)
                    goto labelfinish;
                radioJP.Enabled = false;
                radioUS.Enabled = false;
                radioPAL.Enabled = false;
                string region = "";
                string region_f = "";
                int writefile_count = 0;
                if (radioJP.Checked == true)
                {
                    region = JP;
                    region_f = JP_f;
                    writefile_count = 1136;
                }
                else if (radioUS.Checked == true)
                {
                    region = US;
                    region_f = US_f;
                    writefile_count = 1136;
                }
                else
                {
                    region = PAL;
                    region_f = PAL_f;
                    writefile_count = 1208;
                }
                string filedir = Path.GetDirectoryName(path[b]);
                if (File.Exists(filedir + region_f) == false)
                    goto labelerror;
                string fileextension = Path.GetExtension(path[b]);
                if (fileextension == ".all" || fileextension == ".ALL")
                    goto labelfile;
                /*
                else if (Directory.Exists(path[b]))
                    goto labelfolder;
                */
                else
                    goto labelerror;

                labelfile:;
                
                // ドラッグ＆ドロップされたファイル
                FileStream fsr = new FileStream(path[b], FileMode.Open, FileAccess.Read);
                DirectoryInfo di = Directory.CreateDirectory(Path.GetDirectoryName(path[b]) + @"\" + Path.GetFileNameWithoutExtension(path[b]));
                
                FileStream fsr_toc = new FileStream(filedir + region_f, FileMode.Open, FileAccess.Read);
                byte[] bs_toc = null;
                if (region == JP)
                {
                    bs_toc = new byte[18176];
                    fsr_toc.Seek(1097700, SeekOrigin.Begin);
                    fsr_toc.Read(bs_toc, 0, bs_toc.Length);
                }
                if (region == US)
                {
                    bs_toc = new byte[18176];
                    fsr_toc.Seek(1102420, SeekOrigin.Begin);
                    fsr_toc.Read(bs_toc, 0, bs_toc.Length);
                }
                if (region == PAL)
                {
                    bs_toc = new byte[19328];
                    fsr_toc.Seek(1115060, SeekOrigin.Begin);
                    fsr_toc.Read(bs_toc, 0, bs_toc.Length);
                }
                fsr_toc.Close();

                int writefile_start_seek = 4;
                int writefile_length_seek = 12;
                
                for (a = 0; a < writefile_count; a++)
                {
                    // ファイルの開始位置
                    int writefile_startlength = Getbyteint(bs_toc, writefile_start_seek) * 2048;

                    // ファイルの長さ
                    int writefile_length = Getbyteint(bs_toc, writefile_length_seek);

                    // ファイルを書き出す
                    byte[] writefile_file = new byte[writefile_length];
                    fsr.Seek(writefile_startlength, SeekOrigin.Begin);
                    fsr.Read(writefile_file, 0, writefile_length);
                    FileStream fsw = new FileStream(di + @"\" + string.Format("{0:00000000}", a), FileMode.Create, FileAccess.Write);
                    fsw.Write(writefile_file, 0, writefile_file.Length);
                    fsw.Close();

                    writefile_start_seek += 16;
                    writefile_length_seek += 16;
                }
                FileStream fsw_toc = new FileStream(Path.GetDirectoryName(path[b]) + @"\" + "R5.TOC", FileMode.Create, FileAccess.Write);
                fsw_toc.Write(bs_toc, 0, bs_toc.Length);
                fsw_toc.Close();
                fsr.Close();
                goto labelfinish;


            //labelfolder:;

            labelerror:;
                if (File.Exists(filedir + region_f) == false)
                    MessageBox.Show("R5.ALLと同じディレクトリに" + region + "を置いてください" + "\r\n" +
                                    "Put" + region + "in the same directory as R5.ALL");
                string fileextensionf = Path.GetExtension(path[b]);
                if (fileextensionf != ".all" && fileextensionf != ".ALL")
                    MessageBox.Show("このファイルはR5.ALLではありません" + "\r\n" + "This file is not R5.ALL");

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
