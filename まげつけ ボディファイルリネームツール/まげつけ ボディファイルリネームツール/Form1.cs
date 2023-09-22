using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace まげつけ_ボディファイルリネームツール
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static int a = 0;
        List<string> fileslist = new List<string>();
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
            ProgressBar1.Maximum = fileslist.Count;
            Label1.Text = a.ToString() + "/" + fileslist.Count.ToString();
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
                if (Directory.Exists(path[b]))
                    goto labelfolder;
                else
                    goto labelfinish;

                labelfolder:;

                string foldername = path[b].Substring(path[b].LastIndexOf(@"\") + 1);
                if (foldername.Length != 6 || Regex.IsMatch(foldername, @"^[0-9a-zA-Z]+$") == false)
                    goto labelfinish;

                string[] files = Directory.GetFiles(path[b], "*");

                for (int i = 0; i < files.Length; i++)
                {
                    if (Path.GetExtension(files[i]) == ".xg" || Path.GetExtension(files[i]) == ".XG" ||
                        Path.GetExtension(files[i]) == ".imx" || Path.GetExtension(files[i]) == ".IMX")
                        fileslist.Add(files[i]);
                }

                string newpath = path[b].Substring(0, path[b].LastIndexOf(@"\") + 1);
                newpath = newpath + @"\new\";
                if (Directory.Exists(newpath) == false)
                    Directory.CreateDirectory(newpath);

                string oldfilename = Path.GetFileNameWithoutExtension(fileslist[0]);
                oldfilename = oldfilename.Substring(0, 6);
                byte[] finb = System.Text.Encoding.ASCII.GetBytes(oldfilename);
                byte[] fonb = System.Text.Encoding.ASCII.GetBytes(foldername);

                for (a = 0; a < fileslist.Count; a++)
                {
                    bgWorker.ReportProgress(a);
                    FileStream fs = new FileStream(fileslist[a], FileMode.Open, FileAccess.Read);
                    int fs_nokori = (int)fs.Length;
                    int fs_seek = 0;

                    string filename = Path.GetFileName(fileslist[a]);
                    filename = filename.Substring(6, filename.Length - 6);

                    FileStream fsw = new FileStream(newpath + foldername + filename, FileMode.Create, FileAccess.Write);
                    int fsw_seek = 0;

                    while (true)
                    {
                        byte[] bs = new byte[1];
                        fs.Read(bs, 0, 1);
                        if (bs[0] == finb[3])
                            goto label2;
                        if (bs[0] != finb[0])
                        {
                            fsw.Write(bs, 0, 1);
                            fs_seek += 1;
                            fsw_seek += 1;
                            fs.Seek(fs_seek, SeekOrigin.Begin);
                            fsw.Seek(fsw_seek, SeekOrigin.Begin);
                        }

                        else
                        {
                            fs_seek += 1;
                            fs.Seek(fs_seek, SeekOrigin.Begin);
                            byte[] bs2 = new byte[1];
                            fs.Read(bs2, 0, 1);
                            if (bs2[0] != finb[1])
                            {
                                fsw.Write(bs, 0, 1);
                                fsw.Write(bs2, 0, 1);
                                fs_seek += 1;
                                fsw_seek += 2;
                                fs.Seek(fs_seek, SeekOrigin.Begin);
                                fsw.Seek(fsw_seek, SeekOrigin.Begin);
                            }

                            else
                            {
                                fs_seek += 1;
                                fs.Seek(fs_seek, SeekOrigin.Begin);
                                byte[] bs3 = new byte[1];
                                fs.Read(bs3, 0, 1);
                                if (bs3[0] != finb[2])
                                {
                                    fsw.Write(bs, 0, 1);
                                    fsw.Write(bs2, 0, 1);
                                    fsw.Write(bs3, 0, 1);
                                    fs_seek += 1;
                                    fsw_seek += 3;
                                    fs.Seek(fs_seek, SeekOrigin.Begin);
                                    fsw.Seek(fsw_seek, SeekOrigin.Begin);
                                }

                                else
                                {
                                    fs_seek += 1;
                                    fs.Seek(fs_seek, SeekOrigin.Begin);
                                    byte[] bs4 = new byte[1];
                                    fs.Read(bs4, 0, 1);
                                    if (bs4[0] != finb[3])
                                    {
                                        fsw.Write(bs, 0, 1);
                                        fsw.Write(bs2, 0, 1);
                                        fsw.Write(bs3, 0, 1);
                                        fsw.Write(bs4, 0, 1);
                                        fs_seek += 1;
                                        fsw_seek += 4;
                                        fs.Seek(fs_seek, SeekOrigin.Begin);
                                        fsw.Seek(fsw_seek, SeekOrigin.Begin);
                                    }

                                    else
                                    {
                                        fs_seek -= 3;
                                        fs.Seek(fs_seek, SeekOrigin.Begin);
                                        byte[] bs_oldname6 = new byte[6];
                                        fs.Read(bs_oldname6, 0, 6);
                                        bool bsequal = bs_oldname6.SequenceEqual(finb);
                                        if (bsequal == true)
                                        {
                                            fsw.Write(fonb, 0, 6);
                                            fs_seek+= 6;
                                            fsw_seek += 6;
                                            fs.Seek(fs_seek, SeekOrigin.Begin);
                                            fsw.Seek(fsw_seek, SeekOrigin.Begin);
                                        }

                                        else
                                        {
                                            fs.Seek(fs_seek, SeekOrigin.Begin);
                                            byte[] bs_oldname8 = new byte[8];
                                            fs.Read(bs_oldname8, 0, 8);
                                            bs_oldname6 = new byte[6];
                                            Array.Copy(bs_oldname8, 0, bs_oldname6, 0, 5);
                                            Array.Copy(bs_oldname8, 7, bs_oldname6, 5, 1);
                                            bsequal = bs_oldname6.SequenceEqual(finb);
                                            if (bsequal == true)
                                            {
                                                fsw.Write(fonb, 0, 5);
                                                fsw_seek += 5;
                                                fsw.Seek(fsw_seek, SeekOrigin.Begin);
                                                byte[] bs_oldname2 = new byte[2];
                                                Array.Copy(bs_oldname8, 5, bs_oldname2, 0, 2);
                                                fsw.Write(bs_oldname2, 0, 2);
                                                fsw_seek += 2;
                                                fsw.Seek(fsw_seek, SeekOrigin.Begin);
                                                fsw.Write(fonb, 5, 1);
                                                fsw_seek += 1;
                                                fsw.Seek(fsw_seek, SeekOrigin.Begin);
                                                fs_seek += 8;
                                                fs.Seek(fs_seek, SeekOrigin.Begin);
                                            }

                                            else
                                            {
                                                fs.Seek(fs_seek, SeekOrigin.Begin);
                                                byte[] bs_oldname10 = new byte[10];
                                                fs.Read(bs_oldname10, 0, 10);
                                                bs_oldname6 = new byte[6];
                                                Array.Copy(bs_oldname10, 0, bs_oldname6, 0, 4);
                                                Array.Copy(bs_oldname10, 8, bs_oldname6, 4, 2);
                                                bsequal = bs_oldname6.SequenceEqual(finb);
                                                if (bsequal == true)
                                                {
                                                    fsw.Write(fonb, 0, 4);
                                                    fsw_seek += 4;
                                                    fsw.Seek(fsw_seek, SeekOrigin.Begin);
                                                    byte[] bs_oldname4 = new byte[4];
                                                    Array.Copy(bs_oldname10, 4, bs_oldname4, 0, 4);
                                                    fsw.Write(bs_oldname4, 0, 4);
                                                    fsw_seek += 4;
                                                    fsw.Seek(fsw_seek, SeekOrigin.Begin);
                                                    fsw.Write(fonb, 4, 2);
                                                    fsw_seek += 2;
                                                    fsw.Seek(fsw_seek, SeekOrigin.Begin);
                                                    fs_seek += 10;
                                                    fs.Seek(fs_seek, SeekOrigin.Begin);
                                                }

                                                else
                                                {
                                                    fs.Seek(fs_seek, SeekOrigin.Begin);
                                                    byte[] bs_oldname12 = new byte[12];
                                                    fs.Read(bs_oldname12, 0, 12);
                                                    bs_oldname6 = new byte[6];
                                                    Array.Copy(bs_oldname12, 0, bs_oldname6, 0, 4);
                                                    Array.Copy(bs_oldname12, 10, bs_oldname6, 4, 2);
                                                    bsequal = bs_oldname6.SequenceEqual(finb);
                                                    if (bsequal == true)
                                                    {
                                                        fsw.Write(fonb, 0, 4);
                                                        fsw_seek += 4;
                                                        fsw.Seek(fsw_seek, SeekOrigin.Begin);
                                                        byte[] bs_oldname4_2 = new byte[4];
                                                        Array.Copy(bs_oldname12, 4, bs_oldname4_2, 0, 4);
                                                        fsw.Write(bs_oldname4_2, 0, 4);
                                                        fsw_seek += 4;
                                                        fsw.Seek(fsw_seek, SeekOrigin.Begin);
                                                        fsw.Write(fonb, 4, 1);
                                                        fsw_seek += 2;
                                                        fsw.Seek(fsw_seek, SeekOrigin.Begin);
                                                        fs_seek += 10;
                                                        fs.Seek(fs_seek, SeekOrigin.Begin);
                                                    }

                                                    else
                                                    {
                                                        MessageBox.Show("例外エラー ↓を教えてくれるとありがたいです！\n" +
                                                                        fileslist[a] + "の" + fs_seek.ToString() + "バイト目以降");
                                                        break;
                                                    }
                                                }
                                            }

                                        }
                                    }
                                }
                            }
                        }

                    label2:;
                        if (bs[0] == finb[3])
                        {
                            fs_seek += 1;
                            fs.Seek(fs_seek, SeekOrigin.Begin);
                            byte[] bs2 = new byte[1];
                            fs.Read(bs2, 0, 1);
                            if (bs2[0] != finb[4])
                            {
                                fsw.Write(bs, 0, 1);
                                fsw.Write(bs2, 0, 1);
                                fs_seek += 1;
                                fsw_seek += 2;
                                fs.Seek(fs_seek, SeekOrigin.Begin);
                                fsw.Seek(fsw_seek, SeekOrigin.Begin);
                            }

                            else
                            {
                                fs_seek += 1;
                                fs.Seek(fs_seek, SeekOrigin.Begin);
                                byte[] bs3 = new byte[1];
                                fs.Read(bs3, 0, 1);
                                if (bs3[0] != finb[5])
                                {
                                    fsw.Write(bs, 0, 1);
                                    fsw.Write(bs2, 0, 1);
                                    fsw.Write(bs3, 0, 1);
                                    fs_seek += 1;
                                    fsw_seek += 3;
                                    fs.Seek(fs_seek, SeekOrigin.Begin);
                                    fsw.Seek(fsw_seek, SeekOrigin.Begin);
                                }

                                else
                                {
                                    fs_seek += 1;
                                    fs.Seek(fs_seek, SeekOrigin.Begin);
                                    byte[] bs4 = new byte[1];
                                    fs.Read(bs4, 0, 1);
                                    if (bs4[0] != finb[6])
                                    {
                                        fsw.Write(bs, 0, 1);
                                        fsw.Write(bs2, 0, 1);
                                        fsw.Write(bs3, 0, 1);
                                        fsw.Write(bs4, 0, 1);
                                        fs_seek += 1;
                                        fsw_seek += 4;
                                        fs.Seek(fs_seek, SeekOrigin.Begin);
                                        fsw.Seek(fsw_seek, SeekOrigin.Begin);
                                    }

                                    else
                                    {
                                        fs_seek += 1;
                                        fsw.Write(fonb, 2, 4);
                                        fsw_seek += 4;
                                        fs.Seek(fs_seek, SeekOrigin.Begin);
                                        fsw.Seek(fsw_seek, SeekOrigin.Begin);
                                    }
                                }
                            }
                        }

                        if (fs_seek == fs.Length)
                        break;
                    }
                    
                    fs.Close();
                    fsw.Close();
                }

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
