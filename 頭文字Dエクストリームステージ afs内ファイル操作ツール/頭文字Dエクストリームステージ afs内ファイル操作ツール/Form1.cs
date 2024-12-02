using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 頭文字Dエクストリームステージ_afs内ファイル操作ツール
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
        public static byte[] zero4 = new byte[4] { 0x00, 0x00, 0x00, 0x00 };
        public static bool not_afs_no_naka_file = false;
        public static byte[] byte2 = new byte[2];
        public static byte[] byte4 = new byte[4];

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
                if (Path.GetExtension(path[b]).ToLower() == ".exe")
                    goto labelfinish;

                var isDirectory = File.GetAttributes(path[b]).HasFlag(FileAttributes.Directory);
                if (isDirectory == true)
                    goto label_to_pack;

                filecount = path.Count() - 1;

                //まとめファイル(NO_NAME)を展開する
                FileStream fsr_up = new FileStream(path[b], FileMode.Open, FileAccess.Read);
                byte[] bs_pack = new byte[fsr_up.Length];
                fsr_up.Read(bs_pack, 0, bs_pack.Length);
                fsr_up.Close();

                string unpackfile_path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\" + Path.GetFileName(path[b]) + @"_extracted\";
                if (!Directory.Exists(unpackfile_path))
                    Directory.CreateDirectory(unpackfile_path);
                int file_pointer_address = 0;
                Array.Copy(bs_pack, 0, byte4, 0, 4);
                int file_pointer_1 = BitConverter.ToInt32(byte4, 0);
                int filecount_up = 1;

                while (true)
                {
                    Array.Copy(bs_pack, file_pointer_address, byte4, 0, 4);
                    int file_pointer = BitConverter.ToInt32(byte4, 0);
                    Array.Copy(bs_pack, file_pointer_address + 4, byte4, 0, 4);
                    int file_length = BitConverter.ToInt32(byte4, 0);
                    string file_name = "";
                    int file_name_length = 0;

                    while (true)
                    {
                        byte[] name_1byte = new byte[1];
                        Array.Copy(bs_pack, file_pointer_address + 8 + file_name_length, name_1byte, 0, 1);
                        file_name_length += 1;

                        if (name_1byte[0] == 0x00)
                            break;
                        file_name = file_name + Encoding.ASCII.GetString(name_1byte, 0, 1);
                    }

                    byte[] bs_unpackfile = new byte[file_length];
                    Array.Copy(bs_pack, file_pointer, bs_unpackfile, 0, file_length);
                    FileStream fsw_up = new FileStream(unpackfile_path + filecount_up + "_" + file_name, FileMode.Create, FileAccess.Write);
                    fsw_up.Write(bs_unpackfile, 0, file_length);
                    fsw_up.Close();

                    filecount_up += 1;
                    file_pointer_address += 8 + file_name_length;

                    if (file_pointer_address >= file_pointer_1)
                        break;
                }

                a += 1;
                bgWorker.ReportProgress(a);
                goto labelfinish;


            label_to_pack:;
                //展開されたファイルをまとめファイル(NO_NAME)に戻す
                string[] pack_files = Directory.GetFiles(path[b], "*");
                Array.Sort(pack_files, new LogicalStringComparer());
                string repackfile_name = path[b].Substring(path[b].LastIndexOf(@"\") + 1, path[b].Length - path[b].LastIndexOf(@"\") - 1);
                repackfile_name = repackfile_name.Replace("_extracted", "");
                string repackfile_path = path[b].Substring(0, path[b].LastIndexOf(@"\")) + @"\new\";

                int file_pointer_1_rp = 0;
                string[] pack_files_names = new string[pack_files.Count()];

                filecount = pack_files.Count();
                for (int i = 0; i < pack_files.Count(); i++)
                {
                    string pack_files_name = Path.GetFileName(pack_files[i]);
                    pack_files_name = pack_files_name.Substring(pack_files_name.IndexOf("_") + 1, pack_files_name.Length - pack_files_name.IndexOf("_") - 1);
                    file_pointer_1_rp += 9 + pack_files_name.Length;
                    pack_files_names[i] = pack_files_name;
                }

                //元に戻す部分
                byte[] bs_repack_header = new byte[0];
                byte[] bs_repack_files = new byte[0];
                int file_pointer_rp = file_pointer_1_rp;
                for (a = 0; a < pack_files.Count(); a++)
                {
                    byte4 = BitConverter.GetBytes(file_pointer_rp);
                    Array.Resize(ref bs_repack_header, bs_repack_header.Length + 4);
                    Array.Copy(byte4, 0, bs_repack_header, bs_repack_header.Length - 4, 4);

                    FileStream fsr_rp = new FileStream(pack_files[a], FileMode.Open, FileAccess.Read);
                    byte[] bs_repack = new byte[fsr_rp.Length];
                    fsr_rp.Read(bs_repack, 0, bs_repack.Length);
                    fsr_rp.Close();
                    Array.Resize(ref bs_repack_files, bs_repack_files.Length + bs_repack.Length);
                    Array.Copy(bs_repack, 0, bs_repack_files, bs_repack_files.Length - bs_repack.Length, bs_repack.Length);

                    byte4 = BitConverter.GetBytes(bs_repack.Length);
                    Array.Resize(ref bs_repack_header, bs_repack_header.Length + 4);
                    Array.Copy(byte4, 0, bs_repack_header, bs_repack_header.Length - 4, 4);

                    byte[] pack_files_name_byte = Encoding.ASCII.GetBytes(pack_files_names[a]);
                    Array.Resize(ref pack_files_name_byte, pack_files_name_byte.Length + 1);
                    Array.Copy(zero1, 0, pack_files_name_byte, pack_files_name_byte.Length - 1, 1);
                    Array.Resize(ref bs_repack_header, bs_repack_header.Length + pack_files_name_byte.Length);
                    Array.Copy(pack_files_name_byte, 0, bs_repack_header, bs_repack_header.Length - pack_files_name_byte.Length, pack_files_name_byte.Length);

                    file_pointer_rp += bs_repack.Length;
                    bgWorker.ReportProgress(a);
                }

                if (!Directory.Exists(repackfile_path))
                    Directory.CreateDirectory(repackfile_path);
                FileStream fsw_rp = new FileStream(repackfile_path + repackfile_name, FileMode.Create, FileAccess.Write);
                fsw_rp.Write(bs_repack_header, 0, bs_repack_header.Length);
                fsw_rp.Write(bs_repack_files, 0, bs_repack_files.Length);
                fsw_rp.Close();


            labelfinish:;
                if (not_afs_no_naka_file == true)
                    MessageBox.Show("このファイルはafsの中にあるまとめファイルではありません！");
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

        //byte配列2バイト(ビッグエンディアン)をintに変換して戻す
        public static int Getbyteint2(byte[] bytes, int seek)
        {
            byte[] byte1 = new byte[1];
            Array.Copy(bytes, seek, byte1, 0, 1);
            byte[] byte2 = new byte[1];
            Array.Copy(bytes, seek + 1, byte2, 0, 1);

            string str1 = BitConverter.ToString(byte1);
            string str2 = BitConverter.ToString(byte2);
            int bytelength = 0;

            if (str1 != "00")
            {
                bytelength = 2;
                goto label_byteget;
            }
            else if (str2 != "00")
            {
                bytelength = 1;
                goto label_byteget;
            }

            else
                return 0;

            label_byteget:;

            string str16 = "";
            if (bytelength == 1)
                str16 = str2;
            else if (bytelength == 2)
                str16 = str1 + str2;

            int returnint = Convert.ToInt32(str16, 16);

            return returnint;
        }

        //byte配列4バイト(ビッグエンディアン)をintに変換して戻す
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

            if (str1 != "00")
            {
                bytelength = 4;
                goto label_byteget;
            }
            else if (str2 != "00")
            {
                bytelength = 3;
                goto label_byteget;
            }
            else if (str3 != "00")
            {
                bytelength = 2;
                goto label_byteget;
            }
            else if (str4 != "00")
            {
                bytelength = 1;
                goto label_byteget;
            }

            else
                return 0;

            label_byteget:;

            string str16 = "";
            if (bytelength == 1)
                str16 = str4;
            else if (bytelength == 2)
                str16 = str3 + str4;
            else if (bytelength == 3)
                str16 = str2 + str3 + str4;
            else if (bytelength == 4)
                str16 = str1 + str2 + str3 + str4;

            int returnint = Convert.ToInt32(str16, 16);

            return returnint;
        }

        //byte配列1バイトをstringに変換して戻す
        public static string Getbytestr1(byte[] bytes, int seek)
        {
            byte[] byte1 = new byte[1];
            Array.Copy(bytes, seek, byte1, 0, 1);

            string str1 = BitConverter.ToString(byte1);
            return str1;
        }

        //byte配列4バイトをstringに変換して戻す
        public static string Getbytestr4(byte[] bytes, int seek)
        {
            byte[] byte1 = new byte[4];
            Array.Copy(bytes, seek, byte1, 0, 4);

            string str = BitConverter.ToString(byte1).Replace("-", string.Empty);
            return str;
        }

        //intをbyte配列2バイト(ビッグエンディアン)に変換して戻す
        public static byte[] Gethex2(int hex)
        {
            string hexstr = hex.ToString("X");
            if (hexstr.Length == 1 || hexstr.Length == 3)
                hexstr = "0" + hexstr;

            if (hexstr.Length == 2)
                hexstr = "00" + hexstr;

            byte[] hexbyte = StringToBytes(hexstr);
            return hexbyte;
        }

        //intをbyte配列4バイト(ビッグエンディアン)に変換して戻す
        public static byte[] Gethex4(int hex)
        {
            string hexstr = hex.ToString("X");
            if (hexstr.Length == 1 || hexstr.Length == 3 || hexstr.Length == 5 || hexstr.Length == 7)
                hexstr = "0" + hexstr;

            if (hexstr.Length == 2)
                hexstr = "000000" + hexstr;

            else if (hexstr.Length == 4)
                hexstr = "0000" + hexstr;

            else if (hexstr.Length == 6)
                hexstr = "00" + hexstr;
            
            byte[] hexbyte = StringToBytes(hexstr);
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

        /// <summary>
        /// 大文字小文字を区別せずに、
        /// 文字列内に含まれている数字を考慮して文字列を比較します。
        /// </summary>
        public class LogicalStringComparer :
            System.Collections.IComparer,
            System.Collections.Generic.IComparer<string>
        {
            [System.Runtime.InteropServices.DllImport("shlwapi.dll",
                CharSet = System.Runtime.InteropServices.CharSet.Unicode,
                ExactSpelling = true)]
            private static extern int StrCmpLogicalW(string x, string y);

            public int Compare(string x, string y)
            {
                return StrCmpLogicalW(x, y);
            }

            public int Compare(object x, object y)
            {
                return this.Compare(x.ToString(), y.ToString());
            }
        }
    }
}
