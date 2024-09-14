using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PS3版湾岸ミッドナイト用DAT操作ツール
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
        public static byte[] zero1 = new byte[1] { 0x00 };

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
                if (Path.GetExtension(path[b]).ToLower() == ".exe")
                    goto labelfinish;

                string filedir = Path.GetDirectoryName(path[b]);
                string filename = Path.GetFileNameWithoutExtension(path[b]);
                if (File.Exists(path[b]))
                {
                    if (File.Exists(filedir + @"\" + filename + ".TOC") == false)
                        goto labelerror;
                    goto labelfile;
                }
                else if (Directory.Exists(path[b]))
                {
                    if (File.Exists(filedir + @"\" + filename + @"\" + filename + ".TOC") == false)
                        goto labelerror2;
                    goto labelfolder;
                }
                goto labelerror;

            labelfile:;

                // ドラッグ＆ドロップされたDATとTOC
                FileStream fsr_file = new FileStream(filedir + @"\" + filename + ".DAT", FileMode.Open, FileAccess.Read);
                byte[] bs_file = new byte[fsr_file.Length];
                fsr_file.Read(bs_file, 0, bs_file.Length);
                fsr_file.Close();

                FileStream fsr_toc_file = new FileStream(filedir + @"\" + filename + ".TOC", FileMode.Open, FileAccess.Read);
                byte[] bs_toc_file = new byte[fsr_toc_file.Length];
                fsr_toc_file.Read(bs_toc_file, 0, bs_toc_file.Length);
                fsr_toc_file.Close();

                filecount = Getbyteint(bs_toc_file, 0x14);

                int writefile_name_seek = Getbyteint(bs_toc_file, 0x2C) + 0x38;
                int writefile_start_seek = 0x30;
                int writefile_length_seek = 0x34;

                if (!Directory.Exists(filedir + @"\" + filename))
                    Directory.CreateDirectory(filedir + @"\" + filename);

                for (a = 0; a < filecount; a++)
                {
                    bgWorker.ReportProgress(a);
                    // ファイルの開始位置
                    int writefile_startlength = Getbyteint(bs_toc_file, writefile_start_seek) * 0x800;

                    // ファイルの長さ
                    int writefile_length = Getbyteint(bs_toc_file, writefile_length_seek);

                    // ファイルのパス(出力先)
                    string writefile_path = "";
                    while(true)
                    {
                        byte[] name_string1 = new byte[1];
                        Array.Copy(bs_toc_file, writefile_name_seek, name_string1, 0, 1);
                        writefile_name_seek += 1;
                        if (name_string1[0] == 0x00)
                            break;
                        writefile_path = writefile_path + System.Text.Encoding.ASCII.GetString(name_string1);
                    }
                    writefile_path = filedir + @"\" + filename + writefile_path;
                    if (!Directory.Exists(Path.GetDirectoryName(writefile_path)))
                        Directory.CreateDirectory(Path.GetDirectoryName(writefile_path));

                    // ファイルを書き出す
                    byte[] writefile_file = new byte[writefile_length];
                    Array.Copy(bs_file, writefile_startlength, writefile_file, 0, writefile_length);
                    FileStream fsw = new FileStream(writefile_path, FileMode.Create, FileAccess.Write);
                    fsw.Write(writefile_file, 0, writefile_file.Length);
                    fsw.Close();

                    writefile_start_seek += 0x28;
                    writefile_length_seek += 0x28;
                }

                FileStream fsw_toc = new FileStream(filedir + @"\" + filename + @"\" + filename + ".TOC", FileMode.Create, FileAccess.Write);
                fsw_toc.Write(bs_toc_file, 0, bs_toc_file.Length);
                fsw_toc.Close();

                goto labelfinish;

            labelfolder:;
                FileStream fsr_toc_folder = new FileStream(filedir + @"\" + filename + @"\" + filename + ".TOC", FileMode.Open, FileAccess.Read);
                byte[] bs_toc_folder = new byte[fsr_toc_folder.Length];
                fsr_toc_folder.Read(bs_toc_folder, 0, bs_toc_folder.Length);
                fsr_toc_folder.Close();

                if (!Directory.Exists(filedir + @"\" + "new"))
                    Directory.CreateDirectory(filedir + @"\" + "new");

                filecount = Getbyteint(bs_toc_folder, 0x14);
                byte[] bs_toc_4 = new byte[4];
                byte[] bs_toc_new_tocdata = new byte[0];
                int readfile_name_seek = Getbyteint(bs_toc_folder, 0x2C) + 0x38;
                int writefile_totallength = 0;
                int writefile_totallength_800 = 0;
                int writefile_totallength_w800 = 0;
                int writefilecount = 0;

                byte[] bs_toc_new_filepathdata = new byte[8];
                Array.Copy(bs_toc_folder, readfile_name_seek - 8, bs_toc_new_filepathdata, 0, 8);

                FileStream fsw_new = new FileStream(filedir + @"\new\" + filename + ".DAT", FileMode.Create, FileAccess.Write);

                for (a = 0; a < filecount; a++)
                {
                    bgWorker.ReportProgress(a);

                    // ファイルのパス(出力先)
                    string readfile_path = "";
                    int filepath_length = 0;
                    while (true)
                    {
                        byte[] name_string1 = new byte[1];
                        Array.Copy(bs_toc_folder, readfile_name_seek, name_string1, 0, 1);
                        readfile_name_seek += 1;
                        filepath_length += 1;
                        if (name_string1[0] == 0x00)
                            break;
                        readfile_path = readfile_path + System.Text.Encoding.ASCII.GetString(name_string1);
                    }
                    readfile_path = filedir + @"\" + filename + readfile_path;

                    if (!File.Exists(readfile_path))
                        goto labelfolder_loopfinish;

                    Array.Resize(ref bs_toc_new_filepathdata, bs_toc_new_filepathdata.Length + filepath_length);
                    Array.Copy(bs_toc_folder, readfile_name_seek - filepath_length, bs_toc_new_filepathdata, bs_toc_new_filepathdata.Length - filepath_length, filepath_length);

                    writefilecount += 1;

                    FileStream fsr_folder = new FileStream(readfile_path, FileMode.Open, FileAccess.Read);

                    // ファイルの長さ
                    byte[] writefile_file = new byte[fsr_folder.Length];
                    fsr_folder.Read(writefile_file, 0, writefile_file.Length);
                    fsr_folder.Close();

                    // ファイルを書き出す
                    fsw_new.Seek(writefile_totallength_800, SeekOrigin.Begin);
                    fsw_new.Write(writefile_file, 0, writefile_file.Length);

                    //ファイル位置(DAT側、800で割る)
                    bs_toc_4 = Gethexint(writefile_totallength_w800);
                    Array.Resize(ref bs_toc_new_tocdata, bs_toc_new_tocdata.Length + 4);
                    Array.Copy(bs_toc_4, 0, bs_toc_new_tocdata, 0x28 * a, 4);
                    //ファイルの長さ
                    bs_toc_4 = Gethexint(writefile_file.Length);
                    Array.Resize(ref bs_toc_new_tocdata, bs_toc_new_tocdata.Length + 4);
                    Array.Copy(bs_toc_4, 0, bs_toc_new_tocdata, (0x28 * a) + 0x04, 4);
                    Array.Resize(ref bs_toc_new_tocdata, bs_toc_new_tocdata.Length + 0x20);
                    Array.Copy(bs_toc_folder, (0x28 * a) + 0x38, bs_toc_new_tocdata, (0x28 * a) + 0x08, 0x20);

                    writefile_totallength = writefile_totallength_800;
                    writefile_totallength += writefile_file.Length;
                    writefile_totallength_w800 = writefile_totallength / 0x800;
                    if (writefile_totallength % 0x800 != 0)
                        writefile_totallength_w800 += 1;
                    writefile_totallength_800 = writefile_totallength_w800 * 0x800;
                    writefile_totallength_w800 = writefile_totallength_800 / 0x800;

                labelfolder_loopfinish:;
                }

                for (int i = 0; i < writefile_totallength_800 - writefile_totallength; i++)
                {
                    fsw_new.Seek(writefile_totallength + i, SeekOrigin.Begin);
                    fsw_new.Write(zero1, 0, 1);
                }
                fsw_new.Close();

                byte[] bs_toc_new_folder = new byte[0];

                Array.Resize(ref bs_toc_new_folder, bs_toc_new_folder.Length + 0x14);
                Array.Copy(bs_toc_folder, 0, bs_toc_new_folder, 0, 0x14);

                //ファイル数
                bs_toc_4 = Gethexint(writefilecount);
                Array.Resize(ref bs_toc_new_folder, bs_toc_new_folder.Length + 4);
                Array.Copy(bs_toc_4, 0, bs_toc_new_folder, bs_toc_new_folder.Length - 4, 4);

                Array.Resize(ref bs_toc_new_folder, bs_toc_new_folder.Length + 0x14);
                Array.Copy(bs_toc_folder, 0x18, bs_toc_new_folder, bs_toc_new_folder.Length - 0x14, 0x14);

                //ファイルパスまでの長さ
                bs_toc_4 = Gethexint(writefilecount * 0x28);
                Array.Resize(ref bs_toc_new_folder, bs_toc_new_folder.Length + 4);
                Array.Copy(bs_toc_4, 0, bs_toc_new_folder, bs_toc_new_folder.Length - 4, 4);

                //TOCデータ(DAT側のファイル位置)
                Array.Resize(ref bs_toc_new_folder, bs_toc_new_folder.Length + bs_toc_new_tocdata.Length);
                Array.Copy(bs_toc_new_tocdata, 0, bs_toc_new_folder, bs_toc_new_folder.Length - bs_toc_new_tocdata.Length, bs_toc_new_tocdata.Length);

                //TOCファイルパス
                Array.Resize(ref bs_toc_new_folder, bs_toc_new_folder.Length + bs_toc_new_filepathdata.Length);
                Array.Copy(bs_toc_new_filepathdata, 0, bs_toc_new_folder, bs_toc_new_folder.Length - bs_toc_new_filepathdata.Length, bs_toc_new_filepathdata.Length);

                int toc_unknown = 16;
                if (readfile_name_seek % 4 != 0)
                    toc_unknown -= readfile_name_seek % 4;
                else
                    toc_unknown -= 4;

                Array.Resize(ref bs_toc_new_folder, bs_toc_new_folder.Length + toc_unknown);
                Array.Copy(bs_toc_folder, readfile_name_seek, bs_toc_new_folder, bs_toc_new_folder.Length - toc_unknown, toc_unknown);

                //ファイル数
                bs_toc_4 = Gethexint(writefilecount);
                Array.Resize(ref bs_toc_new_folder, bs_toc_new_folder.Length + 4);
                Array.Copy(bs_toc_4, 0, bs_toc_new_folder, bs_toc_new_folder.Length - 4, 4);

                byte[] bs_toc_unknown2 = new byte[0];
                int bs_toc_unknown2_length = bs_toc_folder.Length - (readfile_name_seek + toc_unknown + 4);
                Array.Resize(ref bs_toc_unknown2, bs_toc_unknown2_length);
                Array.Copy(bs_toc_folder, readfile_name_seek + toc_unknown + 4, bs_toc_unknown2, 0, bs_toc_unknown2_length);
                Array.Resize(ref bs_toc_new_folder, bs_toc_new_folder.Length + bs_toc_unknown2_length);
                Array.Copy(bs_toc_unknown2, 0, bs_toc_new_folder, bs_toc_new_folder.Length - bs_toc_unknown2_length, bs_toc_unknown2_length);

                FileStream fsw_toc_new = new FileStream(filedir + @"\new\" + filename + ".TOC", FileMode.Create, FileAccess.Write);
                fsw_toc_new.Write(bs_toc_new_folder, 0, bs_toc_new_folder.Length);
                fsw_toc_new.Close();
                goto labelfinish;

            labelerror:;
                if (!File.Exists(filedir + @"\" + filename + ".TOC"))
                    MessageBox.Show("ドラッグ&ドロップしたファイルのディレクトリに同じ名前のTOCを置いてください");
                goto labelfinish;

            labelerror2:;
                if (!File.Exists(filedir + @"\" + filename + ".TOC"))
                    MessageBox.Show("フォルダ内に圧縮に使用するTOC(フォルダと同じ名前)を置いてください");

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
