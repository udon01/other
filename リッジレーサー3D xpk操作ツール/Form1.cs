using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace リッジレーサー3D_xpk操作ツール
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
        public static byte[] dainyu0 = new byte[1] { 0x00 };
        public static byte[] dainyu_none = new byte[4] { 0x00, 0x00, 0x00, 0x00 };
        public static byte[] dainyu_2800 = new byte[2] { 0x00, 0x28 };

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
                if (b == 0)
                    goto labelfinish;
                string filedir = Path.GetDirectoryName(path[b]);
                string filename = Path.GetFileNameWithoutExtension(path[b]);
                string fileextension = Path.GetExtension(path[b]);
                if (fileextension == ".xpk" || fileextension == ".XPK")
                    goto labelfile;
                
                else if (Directory.Exists(path[b]))
                    goto labelfolder;
                
                else
                    goto labelerror;

                labelfile:;
                
                // ドラッグ＆ドロップされたファイル
                FileStream fsr = new FileStream(path[b], FileMode.Open, FileAccess.Read);
                DirectoryInfo di = Directory.CreateDirectory(Path.GetDirectoryName(path[b]));
                
                byte[] bs = new byte[fsr.Length];
                fsr.Read(bs, 0, bs.Length);
                filecount = Getbyteint2(bs, 4);
                int writefile_header_length = Getbyteint4(bs, 8);
                int writefile_name = 40;
                int writefile_path = 84;
                int writefile_length_seek = 260;
                int writefile_start_seek = 264;
                int writefile_next_seek = 268;

                for (a = 0; a < filecount; a++)
                {
                    bgWorker.ReportProgress(a);
                    // ファイル名の長さ
                    int writefile_name_length = 0;
                    int writefile_name_0 = 0;

                    while (true)
                    {
                        writefile_name_0 = Getbyteint1(bs, writefile_name + writefile_name_length);
                        if (writefile_name_0 == 0)
                            break;
                        writefile_name_length += 1;
                    }

                    byte[] bs_filename = new byte[writefile_name_length];
                    Array.Copy(bs, writefile_name, bs_filename, 0, writefile_name_length);
                    string xpkfilename = Encoding.ASCII.GetString(bs_filename);

                    int writefile_path_length = 0;
                    int writefile_path_0 = 0;

                    while (true)
                    {
                        writefile_path_0 = Getbyteint1(bs, writefile_path + writefile_path_length);
                        if (writefile_path_0 == 0)
                            break;
                        writefile_path_length += 1;
                    }

                    //ない場合ディレクトリを作成
                    byte[] bs_filepath = new byte[writefile_path_length];
                    Array.Copy(bs, writefile_path, bs_filepath, 0, writefile_path_length);
                    string dirpath_def = Encoding.ASCII.GetString(bs_filepath);
                    dirpath_def = dirpath_def.Replace(@"\\", @"\");
                    dirpath_def = dirpath_def.Replace("data_tmp", filename);
                    string dirpath = filedir + @"\" + dirpath_def;
                    if (Directory.Exists(dirpath) == false)
                    {
                        DirectoryInfo difn = Directory.CreateDirectory(dirpath);
                    }
                    string xpkfilepath = dirpath + xpkfilename;

                    int writefile_length = Getbyteint4(bs, writefile_length_seek);
                    int writefile_start = Getbyteint4(bs, writefile_start_seek);

                    // ファイルを書き出す
                    FileStream fsw = new FileStream(xpkfilepath, FileMode.Create, FileAccess.Write);
                    byte[] bs_file = new byte[writefile_length];
                    Array.Copy(bs, writefile_start, bs_file, 0, writefile_length);
                    fsw.Write(bs_file, 0, writefile_length);
                    fsw.Close();

                    int writefile_next = Getbyteint4(bs, writefile_next_seek);
                    writefile_name += writefile_next;
                    writefile_path += writefile_next;
                    writefile_length_seek += writefile_next;
                    writefile_start_seek += writefile_next;
                    writefile_next_seek += writefile_next;

                    if (a + 1 == filecount)
                    {
                        int dirpath_def_indexof = dirpath_def.IndexOf(@"\");
                        dirpath_def = dirpath_def.Substring(0, dirpath_def_indexof);
                        string repackpath = filedir + @"\" + dirpath_def + @"\";

                        FileStream fsw_hed = new FileStream(repackpath + filename + "_header.bin", FileMode.Create, FileAccess.Write);
                        byte[] bs_header = new byte[writefile_header_length];
                        Array.Copy(bs, 0, bs_header, 0, writefile_header_length);
                        fsw_hed.Write(bs_header, 0, writefile_header_length);
                        fsw_hed.Close();

                        /*
                        if (Directory.Exists(repackpath + "xpk"))
                        {
                            string[] xpkfiles = Directory.GetFiles(repackpath + "xpk", "*", SearchOption.AllDirectories);
                            MessageBox.Show(xpkfiles.Count().ToString());
                            for (int a = 0; a < xpkfiles.Count(); a++)
                            {
                                FileStream fsr_xpk = new FileStream(xpkfiles[a], FileMode.Open, FileAccess.Read);

                                bs = new byte[fsr_xpk.Length];
                                fsr_xpk.Read(bs, 0, bs.Length);
                                filecount = xpkfiles.Count();
                                int filecount_xpk = Getbyteint2(bs, 4);
                                writefile_header_length = Getbyteint4(bs, 8);
                                writefile_name = 40;
                                writefile_path = 84;
                                writefile_length_seek = 260;
                                writefile_start_seek = 264;
                                writefile_next_seek = 268;

                                for (a = 0; a < filecount_xpk; a++)
                                {
                                    // ファイル名の長さ
                                    writefile_name_length = 0;
                                    writefile_name_0 = 0;

                                    while (true)
                                    {
                                        writefile_name_0 = Getbyteint1(bs, writefile_name + writefile_name_length);
                                        if (writefile_name_0 == 0)
                                            break;
                                        writefile_name_length += 1;
                                    }

                                    bs_filename = new byte[writefile_name_length];
                                    Array.Copy(bs, writefile_name, bs_filename, 0, writefile_name_length);
                                    xpkfilename = Encoding.ASCII.GetString(bs_filename);

                                    writefile_path_length = 0;
                                    writefile_path_0 = 0;

                                    while (true)
                                    {
                                        writefile_path_0 = Getbyteint1(bs, writefile_path + writefile_path_length);
                                        if (writefile_path_0 == 0)
                                            break;
                                        writefile_path_length += 1;
                                    }

                                    //ない場合ディレクトリを作成
                                    bs_filepath = new byte[writefile_path_length];
                                    Array.Copy(bs, writefile_path, bs_filepath, 0, writefile_path_length);
                                    dirpath_def = Encoding.ASCII.GetString(bs_filepath);
                                    dirpath_def = dirpath_def.Replace(@"\\", @"\");
                                    dirpath = filedir + @"\" + dirpath_def;
                                    if (Directory.Exists(dirpath) == false)
                                    {
                                        DirectoryInfo difn = Directory.CreateDirectory(dirpath);
                                    }
                                    xpkfilepath = dirpath + xpkfilename;

                                    writefile_length = Getbyteint4(bs, writefile_length_seek);
                                    writefile_start = Getbyteint4(bs, writefile_start_seek);

                                    // ファイルを書き出す
                                    fsw = new FileStream(xpkfilepath, FileMode.Create, FileAccess.Write);
                                    bs_file = new byte[writefile_length];
                                    Array.Copy(bs, writefile_start, bs_file, 0, writefile_length);
                                    fsw.Write(bs_file, 0, writefile_length);
                                    fsw.Close();

                                    writefile_next = Getbyteint4(bs, writefile_next_seek);
                                    writefile_name += writefile_next;
                                    writefile_path += writefile_next;
                                    writefile_length_seek += writefile_next;
                                    writefile_start_seek += writefile_next;
                                    writefile_next_seek += writefile_next;

                                    if (a + 1 == filecount_xpk)
                                    {
                                        dirpath_def_indexof = dirpath_def.IndexOf(@"\");
                                        dirpath_def = dirpath_def.Substring(0, dirpath_def_indexof);
                                        string inxpkfilename = Path.GetFileNameWithoutExtension(xpkfiles[a]);
                                        repackpath = filedir + @"\" + dirpath_def + @"\xpk\" + inxpkfilename + "_header.bin";

                                        fsw_hed = new FileStream(repackpath, FileMode.Create, FileAccess.Write);
                                        bs_header = new byte[writefile_header_length];
                                        Array.Copy(bs, 0, bs_header, 0, writefile_header_length);
                                        fsw_hed.Write(bs_header, 0, writefile_header_length);
                                        fsw_hed.Close();
                                    }
                                }
                            }
                        }
                        */
                    }
                }
                goto labelfinish;


            labelfolder:;
                string filesearch = filedir + @"\" + filename + @"\";
                DirectoryInfo di_search = new DirectoryInfo(filesearch);
                FileInfo[] fipatterns = di_search.GetFiles("*_header*");
                string filepatterns = fipatterns[0].FullName;
                string newfilename = Path.GetFileNameWithoutExtension(filepatterns);
                newfilename = newfilename.Replace("_header", "");
                FileStream fsrh = new FileStream(filepatterns, FileMode.Open, FileAccess.Read);
                byte[] bsh = new byte[fsrh.Length];
                fsrh.Read(bsh, 0, bsh.Length);
                filecount = Getbyteint2(bsh, 4);
                int readfile_header_length = Getbyteint4(bsh, 8);
                int readfile_name = 40;
                int readfile_path = 84;
                byte[] newh = new byte[0];
                Array.Resize(ref newh, 20);
                Array.Copy(bsh, 16, newh, 0, 20);
                int bsh_seek = 36;
                byte[] newb = new byte[0];
                int newfilecount = 0;

                for (a = 0; a < filecount; a++)
                {
                    bgWorker.ReportProgress(a);
                    // ファイル名の長さ
                    int readfile_name_length = 0;
                    int readfile_name_0 = 0;

                    while (true)
                    {
                        readfile_name_0 = Getbyteint1(bsh, readfile_name + readfile_name_length);
                        if (readfile_name_0 == 0)
                            break;
                        readfile_name_length += 1;
                    }

                    byte[] bsh_filename = new byte[readfile_name_length];
                    Array.Copy(bsh, readfile_name, bsh_filename, 0, readfile_name_length);
                    string xpkfilename = Encoding.ASCII.GetString(bsh_filename);

                    int readfile_path_length = 0;
                    int readfile_path_0 = 0;

                    while (true)
                    {
                        readfile_path_0 = Getbyteint1(bsh, readfile_path + readfile_path_length);
                        if (readfile_path_0 == 0)
                            break;
                        readfile_path_length += 1;
                    }

                    byte[] bsh_filepath = new byte[readfile_path_length];
                    Array.Copy(bsh, readfile_path, bsh_filepath, 0, readfile_path_length);
                    string dirpath_def = Encoding.ASCII.GetString(bsh_filepath);
                    dirpath_def = dirpath_def.Replace(@"\\", @"\");
                    string readpath = filedir + @"\" + dirpath_def + xpkfilename;
                    dirpath_def = dirpath_def.Replace("data_tmp", newfilename);
                    string readpath2 = filedir + @"\" + dirpath_def + xpkfilename;

                    //ファイルがある場合xpkに追加する
                    if (File.Exists(readpath) || File.Exists(readpath2))
                    {
                        if (File.Exists(readpath2))
                            readpath = readpath2;
                        bsh_seek += 4;
                        Array.Resize(ref newh, bsh_seek - 16);
                        Array.Copy(dainyu_none, 0, newh, bsh_seek - 20, 4);

                        int readfile_start = readfile_header_length + newb.Length;

                        FileStream fsrb = new FileStream(readpath, FileMode.Open, FileAccess.Read);
                        byte[] bsb = new byte[fsrb.Length];
                        fsrb.Read(bsb, 0, bsb.Length);
                        Array.Resize(ref newb, newb.Length + bsb.Length);
                        Array.Copy(bsb, 0, newb, newb.Length - bsb.Length, bsb.Length);
                        int bsblength = bsb.Length;
                        string bsblength16 = Convert.ToString(bsb.Length, 16);
                        string bsblength16_0 = bsblength16.Substring(bsblength16.Length - 1, 1);
                        if (bsblength16_0 != "0")
                        {
                            while (true)
                            {
                                Array.Resize(ref newb, newb.Length + 1);
                                Array.Copy(dainyu0, 0, newb, newb.Length - 1, 1);

                                bsblength += 1;
                                bsblength16 = Convert.ToString(bsblength, 16);
                                bsblength16_0 = bsblength16.Substring(bsblength16.Length - 1, 1);

                                if (bsblength16_0 == "0")
                                    break;
                            }
                        }

                        bsh_seek += 220;
                        Array.Resize(ref newh, bsh_seek - 16);
                        Array.Copy(bsh, bsh_seek - 220, newh, bsh_seek - 236, 220);

                        int readfile_length = bsb.Length;
                        byte[] lengthbyte = new byte[4];
                        lengthbyte = Gethex(readfile_length);
                        bsh_seek += 4;
                        Array.Resize(ref newh, bsh_seek - 16);
                        Array.Copy(lengthbyte, 0, newh, bsh_seek - 20, 4);
                        byte[] startbyte = new byte[4];
                        startbyte = Gethex(readfile_start);
                        bsh_seek += 4;
                        Array.Resize(ref newh, bsh_seek - 16);
                        Array.Copy(startbyte, 0, newh, bsh_seek - 20, 4);

                        byte[] hlengthbyte = new byte[4] { 0xF0, 0x00, 0x00, 0x00 };
                        bsh_seek += 4;
                        Array.Resize(ref newh, bsh_seek - 16);
                        Array.Copy(hlengthbyte, 0, newh, bsh_seek - 20, 4);

                        byte[] unknownbyte = new byte[4];
                        Array.Copy(bsh, bsh_seek, unknownbyte, 0, 4);
                        bsh_seek += 4;
                        Array.Resize(ref newh, bsh_seek - 16);
                        Array.Copy(unknownbyte, 0, newh, bsh_seek - 20, 4);

                        int readfile_next = 240;
                        readfile_name += readfile_next;
                        readfile_path += readfile_next;
                        newfilecount += 1;
                    }

                    if (a + 1 == filecount)
                    {
                        bsh_seek += 4;
                        Array.Resize(ref newh, bsh_seek - 16);
                        Array.Copy(dainyu_none, 0, newh, bsh_seek - 20, 4);

                        byte[] newxpkh = new byte[4] { 0x58, 0x50, 0x4B, 0x00 };
                        byte[] newfilecountbyte = new byte[2];
                        newfilecountbyte = Gethex2(newfilecount);
                        Array.Resize(ref newxpkh, newxpkh.Length + 2);
                        Array.Copy(newfilecountbyte, 0, newxpkh, 4, 2);
                        byte[] newxpk2800 = new byte[2] { 0x28, 0x00 };
                        Array.Resize(ref newxpkh, newxpkh.Length + 2);
                        Array.Copy(newxpk2800, 0, newxpkh, 6, 2);
                        int newheaderlength = newh.Length + 16;
                        byte[] newheaderlengthbyte = new byte[4];
                        newheaderlengthbyte = Gethex(newheaderlength);
                        Array.Resize(ref newxpkh, newxpkh.Length + 4);
                        Array.Copy(newheaderlengthbyte, 0, newxpkh, 8, 4);
                        int newxpklength = newh.Length + newb.Length + 16;
                        byte[] newxpklengthbyte = new byte[4];
                        newxpklengthbyte = Gethex(newxpklength);
                        Array.Resize(ref newxpkh, newxpkh.Length + 4);
                        Array.Copy(newxpklengthbyte, 0, newxpkh, 12, 4);

                        string newfiledir = Path.GetDirectoryName(filepatterns);
                        int newfiledir_search = newfiledir.LastIndexOf(@"\");
                        newfiledir = newfiledir.Substring(0, newfiledir_search + 1);
                        FileStream fsw_new = new FileStream(newfiledir + newfilename + ".xpk", FileMode.Create, FileAccess.Write);
                        byte[] bs_new = new byte[0];
                        Array.Resize(ref bs_new, 16);
                        Array.Copy(newxpkh, 0, bs_new, 0, 16);
                        Array.Resize(ref bs_new, bs_new.Length + newh.Length);
                        Array.Copy(newh, 0, bs_new, 16, newh.Length);
                        Array.Resize(ref bs_new, bs_new.Length + newb.Length);
                        Array.Copy(newb, 0, bs_new, bs_new.Length - newb.Length, newb.Length);
                        fsw_new.Write(bs_new, 0, bs_new.Length);
                        fsw_new.Close();
                        goto labelfinish;
                    }
                }

                labelerror:;
                string fileextensionf = Path.GetExtension(path[b]);
                if (fileextensionf != ".xpk" && fileextensionf != ".XPK")
                    MessageBox.Show("このファイルはxpkではありません");

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
                hexstr = hexstr + "00";

            else if (hexstr.Length == 4)
            {
                string hexstr1 = hexstr.Substring(0, 2);
                string hexstr2 = hexstr.Substring(2, 2);
                hexstr = hexstr2 + hexstr1;
            }
            byte[] hexbyte = new byte[2];
            hexbyte = StringToBytes(hexstr);
            return hexbyte;
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
