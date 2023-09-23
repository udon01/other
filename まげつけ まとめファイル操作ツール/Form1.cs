using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace まげつけ_まとめファイル操作ツール
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
                if (fileextension == ".bin" || fileextension == ".BIN")
                    goto labelfile;
                else if (Directory.Exists(path[b]))
                    goto labelfolder;
                else
                    goto labelfinish;

                labelfile:;

                // ドラッグ＆ドロップされたファイル
                FileStream fsr = new FileStream(path[b], FileMode.Open, FileAccess.Read);
                byte[] bs = new byte[fsr.Length];
                fsr.Read(bs, 0, bs.Length);

                // ヘッダーの長さ
                int headerlength = Getbyteint(bs, 8);

                byte[] fsrheader = new byte[headerlength];
                fsr.Seek(0, SeekOrigin.Begin);
                fsr.Read(fsrheader, 0, headerlength);

                DirectoryInfo di = Directory.CreateDirectory(Path.GetDirectoryName(path[b]) + @"\" + Path.GetFileNameWithoutExtension(path[b]));

                //if (Path.GetFileNameWithoutExtension(path[b]) != "CARALL" && Path.GetFileNameWithoutExtension(path[b]) != "CARDEF")
                {
                    FileStream fsh = new FileStream(Path.GetDirectoryName(path[b]) + @"\" + Path.GetFileNameWithoutExtension(path[b])
                                                                          + @"\" + "header.bin", FileMode.Create, FileAccess.Write);
                    fsh.Write(fsrheader, 0, headerlength);
                    fsh.Close();
                }

                byte[] writefile = new byte[16];
                int writefile_seek = 32;
                int writefile_count = 0;

                while (true)
                {
                    fsr.Seek(writefile_seek, SeekOrigin.Begin);
                    fsr.Read(writefile, 0, 16);
                    string writefile_str = BitConverter.ToString(writefile).Replace("-", "");
                    if (writefile_str.IndexOf("2E696D78") >= 0 || writefile_str.IndexOf("2E494D58") >= 0
                        || writefile_str.IndexOf("2E7867") >= 0 || writefile_str.IndexOf("2E5847") >= 0
                        || writefile_str.IndexOf("2E62696E") >= 0 || writefile_str.IndexOf("2E42494E") >= 0
                        || writefile_str.IndexOf("2E646566") >= 0)
                        writefile_count += 1;
                    else
                        break;
                    writefile_seek += 48;
                }

                filecount = writefile_count;
                int writefile_start_seek = 16;
                int writefile_length_seek = 20;
                byte[] writefile_name_byte = new byte[16];
                int writefile_name_seek = 32;

                for (a = 0; a < writefile_count; a++)
                {
                    // ファイルの開始位置
                    int writefile_startlength = Getbyteint(bs, writefile_start_seek);

                    // ファイルの長さ
                    int writefile_length = Getbyteint(bs, writefile_length_seek);

                    // ファイル名を取得
                    fsr.Seek(writefile_name_seek, SeekOrigin.Begin);
                    fsr.Read(writefile_name_byte, 0, 16);

                    string bytestr = BitConverter.ToString(writefile_name_byte).Replace("-", "");
                    bytestr = bytestr.Replace("00", "");
                    byte[] byte16 = StringToBytes(bytestr);
                    string writefile_name = Encoding.GetEncoding("shift_jis").GetString(byte16);

                    // ファイルを書き出す
                    byte[] writefile_file = new byte[writefile_length];
                    fsr.Seek(writefile_startlength, SeekOrigin.Begin);
                    fsr.Read(writefile_file, 0, writefile_length);
                    FileStream fsw = new FileStream(Path.GetDirectoryName(path[b]) + @"\" + Path.GetFileNameWithoutExtension(path[b])
                                                                      + @"\" + writefile_name, FileMode.Create, FileAccess.Write);
                    fsw.Write(writefile_file, 0, writefile_file.Length);
                    fsw.Close();

                    writefile_start_seek += 48;
                    writefile_length_seek += 48;
                    writefile_name_seek += 48;
                    bgWorker.ReportProgress(a);
                }
                fsr.Close();


            labelfolder:;

                string foldername = path[b].Substring(path[b].LastIndexOf(@"\") + 1);
                int filestart_seek = 16;
                int filelength_seek = 20;
                int filenameend_seek = 48;

                string[] filessort = Directory.GetFiles(path[b], "*", SearchOption.AllDirectories);
                List<string> filessortlist = new List<string>();
                for (int i = 0; i < filessort.Length; i++)
                {
                    if (Path.GetFileNameWithoutExtension(filessort[i]) != "header")
                        filessortlist.Add(filessort[i]);

                    else
                        filessort[i] = "";
                }
                List<string> files = new List<string>();
                List<string> headerfilename = new List<string>();
                
                if (foldername == "CARALL")
                {
                    int file1 = 0;
                    int file2 = 4;
                    int file3 = 1;
                    int file4 = 2;
                    int file5 = 6;
                    int file6 = 3;
                    int file7 = 7;
                    int file8 = 8;
                    int file9 = 9;
                    int file10 = 10;
                    int file11 = 12;
                    int file12 = 14;
                    int file13 = 5;
                    int file14 = 11;
                    int file15 = 13;
                    int file16 = 15;

                    for (int i = 0; i < filessort.Count() / 16; i++)
                    {
                        files.Add(filessortlist.ElementAt(file1));
                        files.Add(filessortlist.ElementAt(file2));
                        files.Add(filessortlist.ElementAt(file3));
                        files.Add(filessortlist.ElementAt(file4));
                        files.Add(filessortlist.ElementAt(file5));
                        files.Add(filessortlist.ElementAt(file6));
                        files.Add(filessortlist.ElementAt(file7));
                        files.Add(filessortlist.ElementAt(file8));
                        files.Add(filessortlist.ElementAt(file9));
                        files.Add(filessortlist.ElementAt(file10));
                        files.Add(filessortlist.ElementAt(file11));
                        files.Add(filessortlist.ElementAt(file12));
                        files.Add(filessortlist.ElementAt(file13));
                        files.Add(filessortlist.ElementAt(file14));
                        files.Add(filessortlist.ElementAt(file15));
                        files.Add(filessortlist.ElementAt(file16));
                        file1 += 16;
                        file2 += 16;
                        file3 += 16;
                        file4 += 16;
                        file5 += 16;
                        file6 += 16;
                        file7 += 16;
                        file8 += 16;
                        file9 += 16;
                        file10 += 16;
                        file11 += 16;
                        file12 += 16;
                        file13 += 16;
                        file14 += 16;
                        file15 += 16;
                        file16 += 16;
                    }
                }

                else
                {
                
                    int filecountcopy = 0;
                    byte[] filename_byte = new byte[16];
                    int filename_seek = 32;
                    FileStream fshr = new FileStream(Path.GetDirectoryName(path[b]) + @"\" + Path.GetFileNameWithoutExtension(path[b])
                                                    + @"\" + "header.bin", FileMode.Open, FileAccess.Read);

                    if (foldername != "CARDEF" && foldername != "DOME")
                    //if (true)
                    {
                        for (int i = 0; i < filessortlist.Count(); i++)
                        {
                            // ファイル名を取得
                            fshr.Seek(filename_seek, SeekOrigin.Begin);
                            fshr.Read(filename_byte, 0, 16);

                            string bytestr = BitConverter.ToString(filename_byte).Replace("-", "");
                            bytestr = bytestr.Replace("00", "");
                            byte[] byte16 = StringToBytes(bytestr);
                            string filename = Encoding.GetEncoding("shift_jis").GetString(byte16);
                            headerfilename.Add(filename);

                            filename_seek += 48;
                        }
                        fshr.Close();

                        while (true)
                        {
                            for (int i = 0; i < filessortlist.Count(); i++)
                            {
                                if (Path.GetFileName(filessortlist.ElementAt(i)) == headerfilename.ElementAt(filecountcopy))
                                {
                                    files.Add(filessortlist.ElementAt(i));
                                    filecountcopy += 1;
                                    break;
                                }
                            }

                            if (filecountcopy == filessortlist.Count())
                                break;
                        }
                    }

                    else
                    {
                        for (int i = 0; i < filessortlist.Count(); i++)
                        {
                            files.Add(filessortlist.ElementAt(i));
                            filecountcopy += 1;
                        }
                    }
                }
                
                filecount = files.Count();
                byte[] dainyu4 = new byte[4];
                int fileheaderlength = files.Count() * 48 + 16;
                int filestartlength = files.Count() * 48 + 16;

                FileStream newfsw = File.Create(Path.GetDirectoryName(path[b]) + @"\" + Path.GetFileName(path[b]) + ".BIN");

                dainyu4 = Gethex(files.Count());
                newfsw.Write(dainyu4, 0, 4);
                newfsw.Seek(4, SeekOrigin.Begin);
                newfsw.Write(dainyu_none, 0, 4);
                newfsw.Seek(8, SeekOrigin.Begin);
                dainyu4 = Gethex(fileheaderlength);
                newfsw.Write(dainyu4, 0, 4);
                newfsw.Seek(12, SeekOrigin.Begin);
                newfsw.Write(dainyu_none, 0, 4);
                newfsw.Seek(16, SeekOrigin.Begin);

                while (true)
                {
                    string fileheaderlength_hex = filestartlength.ToString("X");
                    string hex00 = fileheaderlength_hex.Substring(fileheaderlength_hex.Length - 3, 2);
                    if (hex00 == "00" || hex00 == "80")
                        break;
                    else
                        filestartlength += 16;
                }

                List<int> filestartlengthlist = new List<int>();
                List<int> filelengthlist = new List<int>();
                for (int i = 0; i < files.Count(); i++)
                {
                    filestartlengthlist.Add(filestartlength);

                    newfsw.Seek(filestart_seek, SeekOrigin.Begin);
                    dainyu4 = Gethex(filestartlength);
                    newfsw.Write(dainyu4, 0, 4);
                    newfsw.Seek(filelength_seek, SeekOrigin.Begin);
                    FileInfo fileinfo = new FileInfo(files.ElementAt(i));
                    filelengthlist.Add((int)fileinfo.Length);
                    dainyu4 = Gethex((int)fileinfo.Length);
                    newfsw.Write(dainyu4, 0, 4);
                    newfsw.Seek(filelength_seek + 4, SeekOrigin.Begin);
                    newfsw.Write(dainyu_none, 0, 4);
                    newfsw.Seek(filelength_seek + 8, SeekOrigin.Begin);
                    newfsw.Write(dainyu_none, 0, 4);
                    newfsw.Seek(filelength_seek + 12, SeekOrigin.Begin);

                    string filenamestr = Path.GetFileName(files.ElementAt(i));
                    byte[] filename = Encoding.ASCII.GetBytes(filenamestr);
                    newfsw.Write(filename, 0, filename.Length);
                    newfsw.Seek(filenameend_seek, SeekOrigin.Begin);

                    for (int j = 0; j < 4; j++)
                    {
                        newfsw.Write(dainyu_none, 0, 4);
                        newfsw.Seek(filenameend_seek + j * 4, SeekOrigin.Begin);
                    }

                    filestartlength += (int)fileinfo.Length;
                    filestart_seek += 48;
                    filelength_seek += 48;
                    filenameend_seek += 48;

                    while (true)
                    {
                        string fileheaderlength_hex = filestartlength.ToString("X");
                        string hex0 = fileheaderlength_hex.Substring(fileheaderlength_hex.Length - 1, 1);
                        if (hex0 == "0")
                            break;
                        else
                            filestartlength += 1;
                    }

                    while (true)
                    {
                        string fileheaderlength_hex = filestartlength.ToString("X");
                        string hex00 = fileheaderlength_hex.Substring(fileheaderlength_hex.Length - 3, 2);
                        if (hex00 == "00" || hex00 == "80")
                            break;
                        else
                            filestartlength += 16;
                    }
                }

                // データをコピー
                for (a = 0; a < files.Count(); a++)
                {
                    FileStream fs = new FileStream(files.ElementAt(a), FileMode.Open, FileAccess.Read);
                    byte[] bsfile = new byte[fs.Length];
                    fs.Read(bsfile, 0, bsfile.Length);
                    fs.Close();
                    newfsw.Seek(filestartlengthlist.ElementAt(a), SeekOrigin.Begin);
                    newfsw.Write(bsfile, 0, filelengthlist.ElementAt(a));
                    bgWorker.ReportProgress(a);
                }
                newfsw.Seek(filestartlength - 4, SeekOrigin.Begin);
                newfsw.Write(dainyu_none, 0, 4);
                newfsw.Close();

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
