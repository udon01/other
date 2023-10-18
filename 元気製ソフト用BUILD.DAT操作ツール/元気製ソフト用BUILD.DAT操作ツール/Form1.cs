using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace 元気製ソフト用BUILD.DAT操作ツール
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void radioNormal_MouseClick(object sender, MouseEventArgs e)
        {
            radioNormal.Checked = true;
            radioTouge.Checked = false;
        }

        private void radioTouge_MouseClick(object sender, MouseEventArgs e)
        {
            radioNormal.Checked = false;
            radioTouge.Checked = true;
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

                string filedir = Path.GetDirectoryName(path[b]);
                string fileextension = Path.GetExtension(path[b]);
                if (fileextension == ".DAT" || fileextension == ".dat")
                {
                    if (File.Exists(filedir + @"\BUILD.TOC") == false)
                        goto labelerror;
                    goto labelfile;
                }
                else if (Directory.Exists(path[b]))
                {
                    if (File.Exists(filedir + @"\BUILD.TOC") == false)
                        goto labelerror2;
                    goto labelfolder;
                }
                goto labelerror;

            labelfile:;

                // ドラッグ＆ドロップされたファイル
                FileStream fsr = new FileStream(path[b], FileMode.Open, FileAccess.Read);
                
                FileStream fsr_toc = new FileStream(filedir + @"\BUILD.TOC", FileMode.Open, FileAccess.Read);
                byte[] bs_toc = new byte[fsr_toc.Length];
                fsr_toc.Read(bs_toc, 0, bs_toc.Length);
                fsr_toc.Close();

                filecount = Getbyteint(bs_toc, 0);

                int writefile_start_seek = 16;
                int writefile_length_seek = 24;

                DirectoryInfo di = Directory.CreateDirectory(Path.GetDirectoryName(path[b]) + @"\" + Path.GetFileNameWithoutExtension(path[b]));

                for (a = 0; a < filecount; a++)
                {
                    bgWorker.ReportProgress(a);
                    // ファイルの開始位置
                    int writefile_startlength = Getbyteint(bs_toc, writefile_start_seek) * 2048;

                    // ファイルの長さ
                    int writefile_length = Getbyteint(bs_toc, writefile_length_seek) * 2048;

                    // ファイルを書き出す
                    byte[] writefile_file = new byte[writefile_length];
                    fsr.Seek(writefile_startlength, SeekOrigin.Begin);
                    fsr.Read(writefile_file, 0, writefile_length);
                    FileStream fsw = new FileStream(di + @"\" + string.Format("{0:00000000}", a), FileMode.Create, FileAccess.Write);
                    fsw.Write(writefile_file, 0, writefile_file.Length);
                    fsw.Close();

                    writefile_start_seek += 20;
                    writefile_length_seek += 20;
                }
                fsr.Close();

            labelfolder:;
                string[] files = Directory.GetFiles(path[b], "*", SearchOption.AllDirectories);
                FileStream fsr_toc_lf = new FileStream(filedir + @"\BUILD.TOC", FileMode.Open, FileAccess.Read);
                byte[] bs_toc_lf = new byte[fsr_toc_lf.Length];
                fsr_toc_lf.Read(bs_toc_lf, 0, bs_toc_lf.Length);
                fsr_toc_lf.Close();

                filecount = files.Count();
                DirectoryInfo di_lf = Directory.CreateDirectory(Path.GetDirectoryName(path[b]) + @"\" + "new");
                int writefile_totallength = 0;

                byte[] bs_toc_new_lf = new byte[0];
                byte[] bs_toc_4 = new byte[4];
                bs_toc_4 = Gethex(filecount);
                Array.Resize(ref bs_toc_new_lf, 4);
                Array.Copy(bs_toc_4, 0, bs_toc_new_lf, 0, 4);

                Array.Resize(ref bs_toc_new_lf, bs_toc_new_lf.Length + 12);
                Array.Copy(bs_toc_lf, 4, bs_toc_new_lf, 4, 12);

                byte[] bs_dat_lf = new byte[0];

                FileStream fsw_new = new FileStream(di_lf + @"\BUILD.DAT", FileMode.Create, FileAccess.Write);

                for (a = 0; a < filecount; a++)
                {
                    bgWorker.ReportProgress(a);

                    FileStream fsr_lf = new FileStream(files[a], FileMode.Open, FileAccess.Read);

                    // ファイルの長さ
                    int writefile_length = (int)fsr_lf.Length;

                    byte[] writefile_file = new byte[writefile_length];
                    fsr_lf.Read(writefile_file, 0, writefile_file.Length);
                    fsr_lf.Close();
                    writefile_length /= 2048;

                    // ファイルを書き出す
                    fsw_new.Seek(writefile_totallength * 2048, SeekOrigin.Begin);
                    fsw_new.Write(writefile_file, 0, writefile_file.Length);

                    bs_toc_4 = Gethexint(writefile_totallength);
                    Array.Resize(ref bs_toc_new_lf, bs_toc_new_lf.Length + 4);
                    Array.Copy(bs_toc_4, 0, bs_toc_new_lf, 16 + (20 * a), 4);
                    Array.Resize(ref bs_toc_new_lf, bs_toc_new_lf.Length + 4);
                    Array.Copy(bs_toc_lf, 20 + (20 * a), bs_toc_new_lf, 20 + (20 * a), 4);
                    bs_toc_4 = Gethexint(writefile_length);
                    Array.Resize(ref bs_toc_new_lf, bs_toc_new_lf.Length + 4);
                    Array.Copy(bs_toc_4, 0, bs_toc_new_lf, 24 + (20 * a), 4);
                    Array.Resize(ref bs_toc_new_lf, bs_toc_new_lf.Length + 8);
                    Array.Copy(bs_toc_lf, 28 + (20 * a), bs_toc_new_lf, 28 + (20 * a), 8);

                    writefile_totallength += writefile_length;
                }

                fsw_new.Close();
                FileStream fsw_toc_new = new FileStream(di_lf + @"\BUILD.TOC", FileMode.Create, FileAccess.Write);
                fsw_toc_new.Write(bs_toc_new_lf, 0, bs_toc_new_lf.Length);
                fsw_toc_new.Close();
                goto labelfinish;

            labelerror:;
                if (File.Exists(filedir + @"\BUILD.TOC") == false)
                    MessageBox.Show("BUILD.DATと同じディレクトリにBUILD.TOCを置いてください");
                string fileextensionf = Path.GetExtension(path[b]);
                if (fileextensionf != ".DAT" && fileextensionf != ".dat")
                    MessageBox.Show("このファイルはBUILD.DATではありません");

            labelerror2:;
                if (File.Exists(filedir + @"\BUILD.TOC") == false)
                    MessageBox.Show("フォルダと同じディレクトリに生成元のBUILD.TOCを置いてください");

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

        //intをbyte配列4バイト(リトルエンディアン)に変換して戻す
        public static byte[] Gethexint(int hex)
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
