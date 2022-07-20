using System.Text;

namespace まげつけ_xg_imx分割ツール
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] path = Environment.GetCommandLineArgs();
            for (int i = 0; i < path.Count(); i++)
            {
                string fileextension = Path.GetExtension(path[i].ToString());
                // BIN以外であればイベントハンドラを抜ける
                if (fileextension != ".bin" && fileextension != ".BIN")
                    goto labelfinish;

                // ドラッグ＆ドロップされたファイル
                FileStream fsr = new FileStream(path[i].ToString(), FileMode.Open, FileAccess.Read);
                
                // ヘッダーの長さを計算
                byte[] headerhantei1 = new byte[1];
                fsr.Seek(8, SeekOrigin.Begin);
                fsr.Read(headerhantei1, 0, 1);
                byte[] headerhantei2 = new byte[1];
                fsr.Seek(9, SeekOrigin.Begin);
                fsr.Read(headerhantei2, 0, 1);
                byte[] headerhantei3 = new byte[1];
                fsr.Seek(10, SeekOrigin.Begin);
                fsr.Read(headerhantei3, 0, 1);
                byte[] headerhantei4 = new byte[1];
                fsr.Seek(11, SeekOrigin.Begin);
                fsr.Read(headerhantei4, 0, 1);

                string headerstr1 = BitConverter.ToString(headerhantei1);
                string headerstr2 = BitConverter.ToString(headerhantei2);
                string headerstr3 = BitConverter.ToString(headerhantei3);
                string headerstr4 = BitConverter.ToString(headerhantei4);
                int headerlengthbyte = 0;

                if (headerstr4 != "00")
                {
                    headerlengthbyte = 4;
                    goto label1;
                }
                else if (headerstr3 != "00")
                {
                    headerlengthbyte = 3;
                    goto label1;
                }
                else if (headerstr2 != "00")
                {
                    headerlengthbyte = 2;
                    goto label1;
                }
                else if (headerstr1 != "00")
                {
                    headerlengthbyte = 1;
                    goto label1;
                }

            label1:;

                string headerlength16 = "";
                if (headerlengthbyte == 1)
                    headerlength16 = headerstr1;
                else if (headerlengthbyte == 2)
                    headerlength16 = headerstr2 + headerstr1;
                else if (headerlengthbyte == 3)
                    headerlength16 = headerstr3 + headerstr2 + headerstr1;

                int headerlength = Convert.ToInt32(headerlength16, 16);
                byte[] fsrheader = new byte[headerlength];
                fsr.Read(fsrheader, 0, headerlength);
                byte[] fsrbyte = new byte[fsr.Length];
                fsr.Read(fsrbyte, 0, (int)fsr.Length);

                byte[] xg_or_imx = new byte[16];
                int xg_or_imx_seek = 32;
                int xg_or_imx_count = 0;

                while (true)
                {
                    fsr.Seek(xg_or_imx_seek, SeekOrigin.Begin);
                    fsr.Read(xg_or_imx, 0, 16);
                    string xg_or_imx_str = BitConverter.ToString(xg_or_imx).Replace("-", "");
                    if (xg_or_imx_str.IndexOf("2E696D78") >= 0 || xg_or_imx_str.IndexOf("2E494D58") >= 0
                        || xg_or_imx_str.IndexOf("2E7867") >= 0 || xg_or_imx_str.IndexOf("2E5847") >= 0
                        || xg_or_imx_str.IndexOf("2E62696E") >= 0 || xg_or_imx_str.IndexOf("2E42494E") >= 0)
                        xg_or_imx_count += 1;
                    else
                        break;
                    xg_or_imx_seek += 48;
                }

                int xg_or_imx_start_seek_1 = 16;
                int xg_or_imx_start_seek_2 = 17;
                int xg_or_imx_start_seek_3 = 18;
                int xg_or_imx_start_seek_4 = 19;
                int xg_or_imx_length_seek_1 = 20;
                int xg_or_imx_length_seek_2 = 21;
                int xg_or_imx_length_seek_3 = 22;
                int xg_or_imx_length_seek_4 = 23;
                byte[] xg_or_imx_name_byte = new byte[16];
                int xg_or_imx_name_seek = 32;

                DirectoryInfo di = Directory.CreateDirectory(Path.GetDirectoryName(path[i]) + @"\" +
                                                             Path.GetFileNameWithoutExtension(path[i]) + "extract");

                for (int j = 0; j < xg_or_imx_count; j++)
                {
                    // ファイルの開始位置を計算
                    byte[] xg_or_imx_start_1 = new byte[1];
                    fsr.Seek(xg_or_imx_start_seek_1, SeekOrigin.Begin);
                    fsr.Read(xg_or_imx_start_1, 0, 1);
                    byte[] xg_or_imx_start_2 = new byte[1];
                    fsr.Seek(xg_or_imx_start_seek_2, SeekOrigin.Begin);
                    fsr.Read(xg_or_imx_start_2, 0, 1);
                    byte[] xg_or_imx_start_3 = new byte[1];
                    fsr.Seek(xg_or_imx_start_seek_3, SeekOrigin.Begin);
                    fsr.Read(xg_or_imx_start_3, 0, 1);
                    byte[] xg_or_imx_start_4 = new byte[1];
                    fsr.Seek(xg_or_imx_start_seek_4, SeekOrigin.Begin);
                    fsr.Read(xg_or_imx_start_4, 0, 1);

                    string xg_or_imx_start_str1 = BitConverter.ToString(xg_or_imx_start_1);
                    string xg_or_imx_start_str2 = BitConverter.ToString(xg_or_imx_start_2);
                    string xg_or_imx_start_str3 = BitConverter.ToString(xg_or_imx_start_3);
                    string xg_or_imx_start_str4 = BitConverter.ToString(xg_or_imx_start_4);
                    int xg_or_imx_start_lengthbyte = 0;

                    if (xg_or_imx_start_str4 != "00")
                    {
                        xg_or_imx_start_lengthbyte = 4;
                        goto label2;
                    }
                    else if (xg_or_imx_start_str3 != "00")
                    {
                        xg_or_imx_start_lengthbyte = 3;
                        goto label2;
                    }
                    else if (xg_or_imx_start_str2 != "00")
                    {
                        xg_or_imx_start_lengthbyte = 2;
                        goto label2;
                    }
                    else if (xg_or_imx_start_str1 != "00")
                    {
                        xg_or_imx_start_lengthbyte = 1;
                        goto label2;
                    }

                label2:;

                    string xg_or_imx_startlength16 = "";
                    if (xg_or_imx_start_lengthbyte == 1)
                        xg_or_imx_startlength16 = xg_or_imx_start_str1;
                    else if (xg_or_imx_start_lengthbyte == 2)
                        xg_or_imx_startlength16 = xg_or_imx_start_str2 + xg_or_imx_start_str1;
                    else if (xg_or_imx_start_lengthbyte == 3)
                        xg_or_imx_startlength16 = xg_or_imx_start_str3 + xg_or_imx_start_str2 + xg_or_imx_start_str1;
                    else if (xg_or_imx_start_lengthbyte == 4)
                        xg_or_imx_startlength16 = xg_or_imx_start_str4 + xg_or_imx_start_str3 + xg_or_imx_start_str2 + xg_or_imx_start_str1;

                    int xg_or_imx_startlength = Convert.ToInt32(xg_or_imx_startlength16, 16);

                    // ファイルの長さを計算
                    byte[] xg_or_imx_length_1 = new byte[1];
                    fsr.Seek(xg_or_imx_length_seek_1, SeekOrigin.Begin);
                    fsr.Read(xg_or_imx_length_1, 0, 1);
                    byte[] xg_or_imx_length_2 = new byte[1];
                    fsr.Seek(xg_or_imx_length_seek_2, SeekOrigin.Begin);
                    fsr.Read(xg_or_imx_length_2, 0, 1);
                    byte[] xg_or_imx_length_3 = new byte[1];
                    fsr.Seek(xg_or_imx_length_seek_3, SeekOrigin.Begin);
                    fsr.Read(xg_or_imx_length_3, 0, 1);
                    byte[] xg_or_imx_length_4 = new byte[1];
                    fsr.Seek(xg_or_imx_length_seek_4, SeekOrigin.Begin);
                    fsr.Read(xg_or_imx_length_4, 0, 1);

                    string xg_or_imx_length_str1 = BitConverter.ToString(xg_or_imx_length_1);
                    string xg_or_imx_length_str2 = BitConverter.ToString(xg_or_imx_length_2);
                    string xg_or_imx_length_str3 = BitConverter.ToString(xg_or_imx_length_3);
                    string xg_or_imx_length_str4 = BitConverter.ToString(xg_or_imx_length_4);
                    int xg_or_imx_lengthbyte = 0;

                    if (xg_or_imx_length_str4 != "00")
                    {
                        xg_or_imx_lengthbyte = 4;
                        goto label3;
                    }
                    else if (xg_or_imx_length_str3 != "00")
                    {
                        xg_or_imx_lengthbyte = 3;
                        goto label3;
                    }
                    else if (xg_or_imx_length_str2 != "00")
                    {
                        xg_or_imx_lengthbyte = 2;
                        goto label3;
                    }
                    else if (xg_or_imx_length_str1 != "00")
                    {
                        xg_or_imx_lengthbyte = 1;
                        goto label3;
                    }

                label3:;

                    string xg_or_imx_length16 = "";
                    if (xg_or_imx_lengthbyte == 1)
                        xg_or_imx_length16 = xg_or_imx_length_str1;
                    else if (xg_or_imx_lengthbyte == 2)
                        xg_or_imx_length16 = xg_or_imx_length_str2 + xg_or_imx_length_str1;
                    else if (xg_or_imx_lengthbyte == 3)
                        xg_or_imx_length16 = xg_or_imx_length_str3 + xg_or_imx_length_str2 + xg_or_imx_length_str1;
                    else if (xg_or_imx_lengthbyte == 4)
                        xg_or_imx_length16 = xg_or_imx_length_str4 + xg_or_imx_length_str3 + xg_or_imx_length_str2 + xg_or_imx_length_str1;

                    int xg_or_imx_length = Convert.ToInt32(xg_or_imx_length16, 16);

                    // ファイル名を取得
                    fsr.Seek(xg_or_imx_name_seek, SeekOrigin.Begin);
                    fsr.Read(xg_or_imx_name_byte, 0, 16);

                    string bytestr = BitConverter.ToString(xg_or_imx_name_byte).Replace("-", "");
                    bytestr = bytestr.Replace("00", "");
                    byte[] byte16 = StringToBytes(bytestr);
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    string xg_or_imx_name = Encoding.GetEncoding("shift_jis").GetString(byte16);

                    // ファイルを書き出す
                    byte[] xg_or_imx_file = new byte[xg_or_imx_length];
                    fsr.Seek(xg_or_imx_startlength, SeekOrigin.Begin);
                    fsr.Read(xg_or_imx_file, 0, xg_or_imx_length);
                    FileStream fsw = new FileStream(Path.GetDirectoryName(path[i]) + @"\" + Path.GetFileNameWithoutExtension(path[i])
                                                                      + "extract" + @"\" + xg_or_imx_name, 
                                                                      FileMode.Create, FileAccess.Write);
                    fsw.Write(xg_or_imx_file, 0, xg_or_imx_file.Length);
                    fsw.Close();

                    xg_or_imx_start_seek_1 += 48;
                    xg_or_imx_start_seek_2 += 48;
                    xg_or_imx_start_seek_3 += 48;
                    xg_or_imx_start_seek_4 += 48;
                    xg_or_imx_length_seek_1 += 48;
                    xg_or_imx_length_seek_2 += 48;
                    xg_or_imx_length_seek_3 += 48;
                    xg_or_imx_length_seek_4 += 48;
                    xg_or_imx_name_seek += 48;
                }
                fsr.Close();
            labelfinish:;
                Close();
            }
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