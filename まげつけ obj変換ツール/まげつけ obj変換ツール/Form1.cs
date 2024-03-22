using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;

namespace まげつけ_obj変換ツール
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static string objpath;
        public static string filepath;
        public static string fpposition;
        public static int spacecount_n = 6;
        public static bool surflag = false;
        public static bool keikoku = false;
        public static int filehexnone = 0x00;
        public static byte[] filehex00 = BitConverter.GetBytes(filehexnone);
        public static List<string> sr_f_read = new List<string>();
        public static List<string> sr_vt_read = new List<string>();
        public static List<string> sr_vn_read = new List<string>();
        public static List<float> pos_x = new List<float>();
        public static List<float> pos_z = new List<float>();
        public static List<float> pos_y = new List<float>();
        public static List<float> pos_x_vt = new List<float>();
        public static List<float> pos_y_vt = new List<float>();
        public static List<float> pos_x_vn = new List<float>();
        public static List<float> pos_z_vn = new List<float>();
        public static List<float> pos_y_vn = new List<float>();
        public static List<float> pos_x_mnumbers = new List<float>();
        public static List<float> pos_z_mnumbers = new List<float>();
        public static List<float> pos_y_mnumbers = new List<float>();
        public static float pos_x_rrnumbers = 0;
        public static float pos_z_rrnumbers = 0;
        public static float pos_y_rrnumbers = 0;
        public static float pos_x_rlnumbers = 0;
        public static float pos_z_rlnumbers = 0;
        public static float pos_y_rlnumbers = 0;
        public static float pos_x_frnumbers = 0;
        public static float pos_z_frnumbers = 0;
        public static float pos_y_frnumbers = 0;
        public static float pos_x_flnumbers = 0;
        public static float pos_z_flnumbers = 0;
        public static float pos_y_flnumbers = 0;
        public static List<string> pos_all = new List<string>();
        public static List<string> men_hex = new List<string>();
        public static int surfacecount = 0;
        public static byte[] dainyu_none = new byte[4] { 0x00, 0x00, 0x00, 0x00 };
        public static byte[] dainyu_surface1 = new byte[1] { 0x00 };
        public static byte[] dainyu_surface = new byte[4] { 0x00, 0x00, 0x00, 0x0C };
        public static List<int> pos_num_list_no = new List<int>();
        public static List<int> sur_num_list_no = new List<int>();
        public static bool daburi_no = false;
        public static int surfacenumber = 0;
        public static float dou;
        public static string hex1 = "";
        public static string hex2 = "";
        public static string dainyu;
        public static byte[] dainyu_byte = new byte[4];
        public static string hex_0 = "0";
        public static string hex_00 = "00";
        public static string dainyu_1 = "0000803F";
        public static string[] path = new string[] { "" };
        public static int surface_all = 0;
        public static bool close = false;
        public static byte[] triStripCount = new byte[13] { 0x74, 0x72, 0x69, 0x53, 0x74, 0x72, 0x69, 0x70, 0x43, 0x6F, 0x75, 0x6E, 0x74 };
        public static byte[] triStripData = new byte[12] { 0x74, 0x72, 0x69, 0x53, 0x74, 0x72, 0x69, 0x70, 0x44, 0x61, 0x74, 0x61 };
        public static byte[] triListCount = new byte[12] { 0x74, 0x72, 0x69, 0x4C, 0x69, 0x73, 0x74, 0x43, 0x6F, 0x75, 0x6E, 0x74 };
        public static byte[] triListData = new byte[11] { 0x74, 0x72, 0x69, 0x4C, 0x69, 0x73, 0x74, 0x44, 0x61, 0x74, 0x61 };
        public static byte[] cullFunc = new byte[9] { 0x08, 0x63, 0x75, 0x6C, 0x6C, 0x46, 0x75, 0x6E, 0x63 };
        public static byte[] vertices = new byte[8] { 0x76, 0x65, 0x72, 0x74, 0x69, 0x63, 0x65, 0x73 };
        public static byte[] xgMaterial = new byte[13] { 0x01, 0x7D, 0x0A, 0x78, 0x67, 0x4D, 0x61, 0x74, 0x65, 0x72, 0x69, 0x61, 0x6C };
        public static byte[] dag = new byte[8] { 0x01, 0x7D, 0x03, 0x64, 0x61, 0x67, 0x01, 0x7B };
        public static byte[] XGBv1 = new byte[8] { 0x58, 0x47, 0x42, 0x76, 0x31, 0x2E, 0x30, 0x30 };
        public static byte[] xgDagTransform = new byte[16] { 0x0E, 0x78, 0x67, 0x44, 0x61, 0x67, 0x54, 0x72, 0x61, 0x6E, 0x73, 0x66, 0x6F, 0x72, 0x6D, 0x02 };
        public static byte[] xgBgMatrix = new byte[14] { 0x01, 0x3B, 0x0A, 0x78, 0x67, 0x42, 0x67, 0x4D, 0x61, 0x74, 0x72, 0x69, 0x78, 0x0B };
        public static byte[] matrix_0 = new byte[23] { 0x24, 0x6D, 0x61, 0x74, 0x72, 0x69, 0x78, 0x5F, 0x30, 0x01, 0x3B, 0x0A, 0x78, 0x67, 0x42, 0x67, 0x4D, 0x61,
                                                       0x74, 0x72, 0x69, 0x78, 0x12 };
        public static byte[] matrix_1 = new byte[18] { 0x5F, 0x56, 0x70, 0x69, 0x76, 0x6F, 0x74, 0x24, 0x6D, 0x61, 0x74, 0x72, 0x69, 0x78, 0x5F, 0x31, 0x01, 0x3B };
        public static byte[] rrposition = new byte[29] { 0x72, 0x72, 0x5F, 0x56, 0x70, 0x69, 0x76, 0x6F, 0x74, 0x24, 0x6D, 0x61, 0x74, 0x72, 0x69,
                                                         0x78, 0x5F, 0x31, 0x01, 0x7B, 0x08, 0x70, 0x6F, 0x73, 0x69, 0x74, 0x69, 0x6F, 0x6E };
        public static byte[] rlposition = new byte[29] { 0x72, 0x6C, 0x5F, 0x56, 0x70, 0x69, 0x76, 0x6F, 0x74, 0x24, 0x6D, 0x61, 0x74, 0x72, 0x69,
                                                         0x78, 0x5F, 0x31, 0x01, 0x7B, 0x08, 0x70, 0x6F, 0x73, 0x69, 0x74, 0x69, 0x6F, 0x6E };
        public static byte[] frposition = new byte[29] { 0x66, 0x72, 0x5F, 0x56, 0x70, 0x69, 0x76, 0x6F, 0x74, 0x24, 0x6D, 0x61, 0x74, 0x72, 0x69,
                                                         0x78, 0x5F, 0x31, 0x01, 0x7B, 0x08, 0x70, 0x6F, 0x73, 0x69, 0x74, 0x69, 0x6F, 0x6E };
        public static byte[] flposition = new byte[29] { 0x66, 0x6C, 0x5F, 0x56, 0x70, 0x69, 0x76, 0x6F, 0x74, 0x24, 0x6D, 0x61, 0x74, 0x72, 0x69,
                                                         0x78, 0x5F, 0x31, 0x01, 0x7B, 0x08, 0x70, 0x6F, 0x73, 0x69, 0x74, 0x69, 0x6F, 0x6E };
        public static byte[] imputMatrix = new byte[15] { 0x01, 0x7B, 0x0B, 0x69, 0x6E, 0x70, 0x75, 0x74, 0x4D, 0x61, 0x74, 0x72, 0x69, 0x78, 0x0B };
        public static byte[] outputMatrix = new byte[36] { 0x24, 0x6D, 0x61, 0x74, 0x72, 0x69, 0x78, 0x5F, 0x30, 0x0C, 0x6F, 0x75, 0x74, 0x70, 0x75, 0x74, 0x4D, 
                                                           0x61, 0x74, 0x72, 0x69, 0x78, 0x01, 0x7D, 0x0A, 0x78, 0x67, 0x42, 0x67, 0x4D, 0x61, 0x74, 0x72, 0x69, 
                                                           0x78, 0x0B };
        public static byte[] imputParentMatrix = new byte[94] { 0x24, 0x6D, 0x61, 0x74, 0x72, 0x69, 0x78, 0x5F, 0x30, 0x01, 0x7B, 0x08, 0x70, 0x6F, 0x73, 0x69, 
                                                                0x74, 0x69, 0x6F, 0x6E, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00, 0x80, 
                                                                0x08, 0x72, 0x6F, 0x74, 0x61, 0x74, 0x69, 0x6F, 0x6E, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00, 
                                                                0x80, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x80, 0x3F, 0x05, 0x73, 0x63, 0x61, 0x6C, 0x65, 0x00, 
                                                                0x00, 0x80, 0x3F, 0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x80, 0x3F, 0x11, 0x69, 0x6E, 0x70, 0x75, 
                                                                0x74, 0x50, 0x61, 0x72, 0x65, 0x6E, 0x74, 0x4D, 0x61, 0x74, 0x72, 0x69, 0x78, 0x12 };
        public static byte[] VpibotMatrix = new byte[43] { 0x5F, 0x56, 0x70, 0x69, 0x76, 0x6F, 0x74, 0x24, 0x6D, 0x61, 0x74, 0x72, 0x69, 0x78, 0x5F, 0x31, 0x0C, 
                                                           0x6F, 0x75, 0x74, 0x70, 0x75, 0x74, 0x4D, 0x61, 0x74, 0x72, 0x69, 0x78, 0x01, 0x7D, 0x0A, 0x78, 0x67, 
                                                           0x42, 0x67, 0x4D, 0x61, 0x74, 0x72, 0x69, 0x78, 0x12 };
        public static byte[] position = new byte[27] { 0x5F, 0x56, 0x70, 0x69, 0x76, 0x6F, 0x74, 0x24, 0x6D, 0x61, 0x74, 0x72, 0x69, 0x78, 0x5F, 0x31, 0x01, 0x7B, 
                                                       0x08, 0x70, 0x6F, 0x73, 0x69, 0x74, 0x69, 0x6F, 0x6E };
        public static byte[] rotation = new byte[45] { 0x08, 0x72, 0x6F, 0x74, 0x61, 0x74, 0x69, 0x6F, 0x6E, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00, 0x80, 0x00, 
                                                       0x00, 0x00, 0x80, 0x00, 0x00, 0x80, 0x3F, 0x05, 0x73, 0x63, 0x61, 0x6C, 0x65, 0x00, 0x00, 0x80, 0x3F, 0x00, 
                                                       0x00, 0x80, 0x3F, 0x00, 0x00, 0x80, 0x3F, 0x01, 0x7D };
        public static byte[] dag_node = new byte[7] { 0x03, 0x64, 0x61, 0x67, 0x01, 0x7B, 0x02 };
        public static byte[] dagspace = new byte[5] { 0x01, 0x5B, 0x01, 0x5D, 0x02 };
        public static byte[] dagend = new byte[6] { 0x01, 0x5B, 0x01, 0x5D, 0x01, 0x7D };

        private void Form1_Shown(object sender, EventArgs e)
        {
            ProgressBar1.Minimum = 0;
            ProgressBar1.Value = 0;
            BackgroundWorker1.WorkerReportsProgress = true;
            BackgroundWorker1.RunWorkerAsync();
        }

        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBar1.Value = e.ProgressPercentage;
            ProgressBar1.Maximum = surface_all;
            Label1.Text = e.ProgressPercentage.ToString() + "/" + surface_all.ToString();
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
                string filedirectory = Path.GetDirectoryName(path[b]);
                string apppath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string appdirectory = Path.GetDirectoryName(apppath);
                string xgdirectory = appdirectory + @"\CARALL\";

                // obj以外であればイベントハンドラを抜ける
                if (fileextension != ".obj" && fileextension != ".OBJ")
                    goto labelfinish;

                // ドラッグ＆ドロップされたファイル
                StreamReader fsr = new StreamReader(path[b]);

                string v_vt_vn_f;
                string[] vnumbers = new string[0];
                string[] vtnumbers = new string[0];
                string[] vnnumbers = new string[0];
                string[] fnumbers = new string[0];
                int vcount = 0;
                int vtcount = 0;
                int vncount = 0;
                int fcount = 0;
                List<string> mnumbers = new List<string>();
                string rrnumbers = "";
                string rlnumbers = "";
                string frnumbers = "";
                string flnumbers = "";
                int f_minus = 0;

            label1:;

                v_vt_vn_f = fsr.ReadLine();
                
            labelo:;
                if (v_vt_vn_f == null)
                {
                    if (vcount == 0)
                    {
                        //MessageBox.Show("頂点座標(v)が1つもありません");
                        //goto labelfinish2;
                    }

                    else if (fcount == 0)
                    {
                        //MessageBox.Show("面(f)が1つもありません");
                        //goto labelfinish2;
                    }

                    //else
                        //goto label2;
                }

                else if (0 <= v_vt_vn_f.IndexOf("o "))
                {
                    if (v_vt_vn_f == "o m1" || v_vt_vn_f == "o m2" || v_vt_vn_f == "o m3" || 
                        0 <= v_vt_vn_f.IndexOf("o m1_s") || 0 <= v_vt_vn_f.IndexOf("o m2_s") || 0 <= v_vt_vn_f.IndexOf("o m3_s"))
                    {
                        v_vt_vn_f = fsr.ReadLine();
                        if (0 <= v_vt_vn_f.IndexOf("v "))
                        {
                            mnumbers.Add(v_vt_vn_f);
                            f_minus += 1;
                        }
                        while (true)
                        {
                            v_vt_vn_f = fsr.ReadLine();
                            if (0 <= v_vt_vn_f.IndexOf("o ") || v_vt_vn_f == null)
                                goto labelo;
                            else if (0 <= v_vt_vn_f.IndexOf("v "))
                                f_minus += 1;
                        }
                    }

                    else if (v_vt_vn_f == "o rr" || 0 <= v_vt_vn_f.IndexOf("o rr_s"))
                    {
                        v_vt_vn_f = fsr.ReadLine();
                        if (0 <= v_vt_vn_f.IndexOf("v "))
                        { 
                            rrnumbers = v_vt_vn_f;
                            f_minus += 1;
                        }
                        while (true)
                        {
                            v_vt_vn_f = fsr.ReadLine();
                            if (0 <= v_vt_vn_f.IndexOf("o ") || v_vt_vn_f == null)
                                goto labelo;
                            else if (0 <= v_vt_vn_f.IndexOf("v "))
                                f_minus += 1;
                        }
                    }

                    else if (v_vt_vn_f == "o rl" || 0 <= v_vt_vn_f.IndexOf("o rl_s"))
                    {
                        v_vt_vn_f = fsr.ReadLine();
                        if (0 <= v_vt_vn_f.IndexOf("v "))
                        {
                            rlnumbers = v_vt_vn_f;
                            f_minus += 1;
                        }
                        while (true)
                        {
                            v_vt_vn_f = fsr.ReadLine();
                            if (0 <= v_vt_vn_f.IndexOf("o ") || v_vt_vn_f == null)
                                goto labelo;
                            else if (0 <= v_vt_vn_f.IndexOf("v "))
                                f_minus += 1;
                        }
                    }

                    else if (v_vt_vn_f == "o fr" || 0 <= v_vt_vn_f.IndexOf("o fr_s"))
                    {
                        v_vt_vn_f = fsr.ReadLine();
                        if (0 <= v_vt_vn_f.IndexOf("v "))
                        {
                            frnumbers = v_vt_vn_f;
                            f_minus += 1;
                        }
                        while (true)
                        {
                            v_vt_vn_f = fsr.ReadLine();
                            if (0 <= v_vt_vn_f.IndexOf("o ") || v_vt_vn_f == null)
                                goto labelo;
                            else if (0 <= v_vt_vn_f.IndexOf("v "))
                                f_minus += 1;
                        }
                    }

                    else if (v_vt_vn_f == "o fl" || 0 <= v_vt_vn_f.IndexOf("o fl_s"))
                    {
                        v_vt_vn_f = fsr.ReadLine();
                        if (0 <= v_vt_vn_f.IndexOf("v "))
                        {
                            flnumbers = v_vt_vn_f;
                            f_minus += 1;
                        }
                        while (true)
                        {
                            v_vt_vn_f = fsr.ReadLine();
                            if (0 <= v_vt_vn_f.IndexOf("o ") || v_vt_vn_f == null)
                                goto labelo;
                            else if (0 <= v_vt_vn_f.IndexOf("v "))
                                f_minus += 1;
                        }
                    }

                    else
                        goto label1;
                }

                else if (0 <= v_vt_vn_f.IndexOf("#") || 0 <= v_vt_vn_f.IndexOf("mtllib") || 0 <= v_vt_vn_f.IndexOf("usemtl") 
                    || 0 <= v_vt_vn_f.IndexOf("s") || 0 <= v_vt_vn_f.IndexOf("l"))
                    goto label1;

                else if (0 <= v_vt_vn_f.IndexOf("v "))
                {
                    vcount = vcount + 1;
                    Array.Resize(ref vnumbers, vnumbers.Length + 1);
                    vnumbers[vnumbers.Length - 1] = v_vt_vn_f;
                    goto label1;
                }

                else if (0 <= v_vt_vn_f.IndexOf("vt "))
                {
                    vtcount = vtcount + 1;
                    Array.Resize(ref vtnumbers, vtnumbers.Length + 1);
                    vtnumbers[vtnumbers.Length - 1] = v_vt_vn_f;
                    goto label1;
                }

                else if (0 <= v_vt_vn_f.IndexOf("vn "))
                {
                    vncount = vncount + 1;
                    Array.Resize(ref vnnumbers, vnnumbers.Length + 1);
                    vnnumbers[vnnumbers.Length - 1] = v_vt_vn_f;
                    goto label1;
                }


                else if (0 <= v_vt_vn_f.IndexOf("f "))
                {
                    fcount = fcount + 1;
                    Array.Resize(ref fnumbers, fnumbers.Length + 1);
                    fnumbers[fnumbers.Length - 1] = v_vt_vn_f;
                    goto label1;
                }

            label2:;
                fsr.Close();

                //頂点座標の変換処理
                string minus;
                int p_read;
                string p_st_number;
                float p_number;
                int removedec;

                for (int m = 0; m < vnumbers.Length; m++)
                {
                    for (int three = 0; three < 3; three++)
                    {
                        //「v 」を消す
                        if (three == 0)
                            vnumbers[m] = vnumbers[m].Remove(0, 2);

                        //小数点の位置を取得
                        removedec = vnumbers[m].IndexOf(".");

                        //数値がマイナスかどうかを判定する
                        minus = vnumbers[m].Substring(0, 1);
                        if (minus == "-")
                        {
                            p_read = 9;
                            removedec -= 2;
                            p_read += removedec;
                        }

                        else
                        {
                            p_read = 8;
                            removedec -= 1;
                            p_read += removedec;
                        }

                        //座標を浮動小数点に変換する
                        p_st_number = vnumbers[m].Substring(0, p_read);
                        p_number = float.Parse(p_st_number);

                        //リストに代入する
                        if (three == 0)
                            pos_x.Add(p_number);

                        else if (three == 1)
                            pos_z.Add(p_number);

                        else if (three == 2)
                            pos_y.Add(p_number);

                        //座標情報を削除
                        if (three == 2)
                            vnumbers[m] = vnumbers[m].Remove(0, p_read);

                        else
                            vnumbers[m] = vnumbers[m].Remove(0, p_read + 1);
                    }

                    //3回繰り返し
                }

                for (int m = 0; m < vtnumbers.Length; m++)
                {
                    for (int three = 0; three < 2; three++)
                    {
                        //「vt 」を消す
                        if (three == 0)
                            vtnumbers[m] = vtnumbers[m].Remove(0, 3);

                        //小数点の位置を取得
                        removedec = vtnumbers[m].IndexOf(".");

                        //数値がマイナスかどうかを判定する
                        minus = vtnumbers[m].Substring(0, 1);
                        if (minus == "-")
                        {
                            p_read = 9;
                            removedec -= 2;
                            p_read += removedec;
                        }

                        else
                        {
                            p_read = 8;
                            removedec -= 1;
                            p_read += removedec;
                        }

                        //座標を浮動小数点に変換する
                        p_st_number = vtnumbers[m].Substring(0, p_read);
                        p_number = float.Parse(p_st_number);

                        //リストに代入する
                        if (three == 0)
                            pos_x_vt.Add(p_number);

                        else if (three == 1)
                            pos_y_vt.Add(p_number);

                        //座標情報を削除
                        if (three == 1)
                            vtnumbers[m] = vtnumbers[m].Remove(0, p_read);

                        else
                            vtnumbers[m] = vtnumbers[m].Remove(0, p_read + 1);
                    }

                    //2回繰り返し
                }

                for (int m = 0; m < vnnumbers.Length; m++)
                {
                    for (int three = 0; three < 3; three++)
                    {
                        //「vn 」を消す
                        if (three == 0)
                            vnnumbers[m] = vnnumbers[m].Remove(0, 3);

                        //小数点の位置を取得
                        removedec = vnnumbers[m].IndexOf(".");

                        //数値がマイナスかどうかを判定する
                        minus = vnnumbers[m].Substring(0, 1);
                        if (minus == "-")
                        {
                            p_read = 7;
                            removedec -= 2;
                            p_read += removedec;
                        }

                        else
                        {
                            p_read = 6;
                            removedec -= 1;
                            p_read += removedec;
                        }

                        //座標を浮動小数点に変換する
                        p_st_number = vnnumbers[m].Substring(0, p_read);
                        p_number = float.Parse(p_st_number);

                        //リストに代入する
                        if (three == 0)
                            pos_x_vn.Add(p_number);

                        else if (three == 1)
                            pos_z_vn.Add(p_number);

                        else if (three == 2)
                            pos_y_vn.Add(p_number);

                        //座標情報を削除
                        if (three == 2)
                            vnnumbers[m] = vnnumbers[m].Remove(0, p_read);

                        else
                            vnnumbers[m] = vnnumbers[m].Remove(0, p_read + 1);
                    }

                    //3回繰り返し
                }

                if (mnumbers.Count() > 0)
                {
                    string mnumber = "";
                    for (int i = 0; i < mnumbers.Count(); i++)
                    {
                        for (int three = 0; three < 3; three++)
                        {
                            //「v 」を消す
                            if (three == 0)
                                mnumber = mnumbers.ElementAt(i).Remove(0, 2);

                            //小数点の位置を取得
                            removedec = mnumber.IndexOf(".");

                            //数値がマイナスかどうかを判定する
                            minus = mnumber.Substring(0, 1);
                            if (minus == "-")
                            {
                                p_read = 9;
                                removedec -= 2;
                                p_read += removedec;
                            }

                            else
                            {
                                p_read = 8;
                                removedec -= 1;
                                p_read += removedec;
                            }

                            //座標を浮動小数点に変換する
                            p_st_number = mnumber.Substring(0, p_read);
                            p_number = float.Parse(p_st_number);

                            //リストに代入する
                            if (three == 0)
                                pos_x_mnumbers.Add(p_number);

                            else if (three == 1)
                                pos_z_mnumbers.Add(p_number);

                            else if (three == 2)
                                pos_y_mnumbers.Add(p_number);

                            //座標情報を削除
                            if (three == 2)
                                mnumber = mnumber.Remove(0, p_read);

                            else
                                mnumber = mnumber.Remove(0, p_read + 1);
                        }
                    }
                }

                if (rrnumbers != "")
                {
                    for (int three = 0; three < 3; three++)
                    {
                        //「v 」を消す
                        if (three == 0)
                            rrnumbers = rrnumbers.Remove(0, 2);

                        //小数点の位置を取得
                        removedec = rrnumbers.IndexOf(".");

                        //数値がマイナスかどうかを判定する
                        minus = rrnumbers.Substring(0, 1);
                        if (minus == "-")
                        {
                            p_read = 9;
                            removedec -= 2;
                            p_read += removedec;
                        }

                        else
                        {
                            p_read = 8;
                            removedec -= 1;
                            p_read += removedec;
                        }

                        //座標を浮動小数点に変換する
                        p_st_number = rrnumbers.Substring(0, p_read);
                        p_number = float.Parse(p_st_number);

                        //リストに代入する
                        if (three == 0)
                            pos_x_rrnumbers = p_number;

                        else if (three == 1)
                            pos_z_rrnumbers = p_number;

                        else if (three == 2)
                            pos_y_rrnumbers = p_number;

                        //座標情報を削除
                        if (three == 2)
                            rrnumbers = "true";

                        else
                            rrnumbers = rrnumbers.Remove(0, p_read + 1);
                    }
                }

                if (rlnumbers != "")
                {
                    for (int three = 0; three < 3; three++)
                    {
                        //「v 」を消す
                        if (three == 0)
                            rlnumbers = rlnumbers.Remove(0, 2);

                        //小数点の位置を取得
                        removedec = rlnumbers.IndexOf(".");

                        //数値がマイナスかどうかを判定する
                        minus = rlnumbers.Substring(0, 1);
                        if (minus == "-")
                        {
                            p_read = 9;
                            removedec -= 2;
                            p_read += removedec;
                        }

                        else
                        {
                            p_read = 8;
                            removedec -= 1;
                            p_read += removedec;
                        }

                        //座標を浮動小数点に変換する
                        p_st_number = rlnumbers.Substring(0, p_read);
                        p_number = float.Parse(p_st_number);

                        //リストに代入する
                        if (three == 0)
                            pos_x_rlnumbers = p_number;

                        else if (three == 1)
                            pos_z_rlnumbers = p_number;

                        else if (three == 2)
                            pos_y_rlnumbers = p_number;

                        //座標情報を削除
                        if (three == 2)
                            rlnumbers = "true";

                        else
                            rlnumbers = rlnumbers.Remove(0, p_read + 1);
                    }
                }

                if (frnumbers != "")
                {
                    for (int three = 0; three < 3; three++)
                    {
                        //「v 」を消す
                        if (three == 0)
                            frnumbers = frnumbers.Remove(0, 2);

                        //小数点の位置を取得
                        removedec = frnumbers.IndexOf(".");

                        //数値がマイナスかどうかを判定する
                        minus = frnumbers.Substring(0, 1);
                        if (minus == "-")
                        {
                            p_read = 9;
                            removedec -= 2;
                            p_read += removedec;
                        }

                        else
                        {
                            p_read = 8;
                            removedec -= 1;
                            p_read += removedec;
                        }

                        //座標を浮動小数点に変換する
                        p_st_number = frnumbers.Substring(0, p_read);
                        p_number = float.Parse(p_st_number);

                        //リストに代入する
                        if (three == 0)
                            pos_x_frnumbers = p_number;

                        else if (three == 1)
                            pos_z_frnumbers = p_number;

                        else if (three == 2)
                            pos_y_frnumbers = p_number;

                        //座標情報を削除
                        if (three == 2)
                            frnumbers = "true";

                        else
                            frnumbers = frnumbers.Remove(0, p_read + 1);
                    }
                }

                if (flnumbers != "")
                {
                    for (int three = 0; three < 3; three++)
                    {
                        //「v 」を消す
                        if (three == 0)
                            flnumbers = flnumbers.Remove(0, 2);

                        //小数点の位置を取得
                        removedec = flnumbers.IndexOf(".");

                        //数値がマイナスかどうかを判定する
                        minus = flnumbers.Substring(0, 1);
                        if (minus == "-")
                        {
                            p_read = 9;
                            removedec -= 2;
                            p_read += removedec;
                        }

                        else
                        {
                            p_read = 8;
                            removedec -= 1;
                            p_read += removedec;
                        }

                        //座標を浮動小数点に変換する
                        p_st_number = flnumbers.Substring(0, p_read);
                        p_number = float.Parse(p_st_number);

                        //リストに代入する
                        if (three == 0)
                            pos_x_flnumbers = p_number;

                        else if (three == 1)
                            pos_z_flnumbers = p_number;

                        else if (three == 2)
                            pos_y_flnumbers = p_number;

                        //座標情報を削除
                        if (three == 2)
                            flnumbers = "true";

                        else
                            flnumbers = flnumbers.Remove(0, p_read + 1);
                    }
                }

                var sur_one = new List<int>();
                var sur_poli = new List<int>();
                int sur_poli_count = 0;
                int digit = 0;
                string slash = "";
                string s_st_number;
                int f_number;
                int vt_number = 0;
                int vn_number;
                int s_numbersindex;
                StreamWriter swvt = new StreamWriter(filedirectory + @"\テクスチャ座標.txt", false);
                StreamWriter swvn = new StreamWriter(filedirectory + @"\法線.txt", false);
                StreamWriter swf = new StreamWriter(filedirectory + @"\面.txt", false);

                //面情報の処理
                for (int m = 0; m < fnumbers.Length; m++)
                {
                    if (fnumbers[m].Contains("f ") == true)
                        fnumbers[m] = fnumbers[m].Remove(0, 2);

                    while (true)
                    {
                        //面の桁数を取得する
                        digit = 0;
                        while (true)
                        {
                            slash = fnumbers[m].Substring(digit, 1);

                            if (slash == "/")
                                break;

                            digit = digit + 1;
                        }

                        //面の番号を取得する
                        s_st_number = fnumbers[m].Substring(0, digit);
                        f_number = int.Parse(s_st_number);
                        f_number -= f_minus;

                        //面情報を削除
                        s_numbersindex = fnumbers[m].IndexOf("/");
                        s_numbersindex = s_numbersindex + 1;
                        fnumbers[m] = fnumbers[m].Remove(0, s_numbersindex);
                        surfacecount += 1;

                            //テクスチャ座標の桁数を取得する
                            digit = 0;
                            while (true)
                            {
                                slash = fnumbers[m].Substring(digit, 1);

                                if (slash == "/")
                                    break;

                                digit = digit + 1;
                            }

                            //テクスチャ座標の番号を取得する
                            s_st_number = fnumbers[m].Substring(0, digit);
                            vt_number = int.Parse(s_st_number);

                            //テクスチャ座標情報を削除
                            s_numbersindex = fnumbers[m].IndexOf("/");
                            s_numbersindex = s_numbersindex + 1;
                            fnumbers[m] = fnumbers[m].Remove(0, s_numbersindex);

                        //法線の桁数を取得する
                        digit = 0;
                        while (true)
                        {
                            if (fnumbers[m].IndexOf(" ") >= 0)
                            {
                                slash = fnumbers[m].Substring(digit, 1);
                                if (slash == " ")
                                    break;

                                digit += 1;
                            }

                            else
                            {
                                digit = fnumbers[m].Length;
                                break;
                            }
                        }

                        //法線の番号を取得する
                        s_st_number = fnumbers[m].Substring(0, digit);
                        vn_number = int.Parse(s_st_number);
                        //法線情報を削除
                        if (fnumbers[m].IndexOf(" ") >= 0)
                        {
                            s_numbersindex = fnumbers[m].IndexOf(" ");
                            s_numbersindex = s_numbersindex + 1;
                            fnumbers[m] = fnumbers[m].Remove(0, s_numbersindex);
                        }

                        else
                            s_numbersindex = 0;
                        
                        sur_poli_count = sur_poli_count + 1;

                        if (s_numbersindex == 0)
                        {
                            //面の頂点数を代入する
                            sur_poli.Add(sur_poli_count);
                            sur_poli_count = 0;
                            swf.WriteLine(" " + f_number + " ");
                            swvt.WriteLine(" " + vt_number + " ");
                            swvn.WriteLine(" " + vn_number + " ");
                            break;
                        }

                        else
                        {
                            swf.Write(" " + f_number + " ");
                            swvt.Write(" " + vt_number + " ");
                            swvn.Write(" " + vn_number + " ");
                        }
                    }
                }

                swf.Close();
                swvt.Close();
                swvn.Close();
                int pos_y_count = pos_y.Count();
                int y_count = pos_y.Count(x => x == pos_y.Min());
                StreamReader sr_f = new StreamReader(filedirectory + @"\面.txt");
                StreamReader sr_vt = new StreamReader(filedirectory + @"\テクスチャ座標.txt");
                StreamReader sr_vn = new StreamReader(filedirectory + @"\法線.txt");
                sr_f_read = new List<string>();
                sr_vt_read = new List<string>();
                sr_vn_read = new List<string>();

                //全ての面情報をリストに代入
                int j = 0;
                string lines2 = "";
                while (sr_f.Peek() > -1)
                {
                    string lines = sr_f.ReadLine();
                    sr_f_read.Add(lines);
                    j = j + 1;
                    lines2 = lines;
                }

                sr_f.Close();

                j = 0;
                lines2 = "";
                while (sr_vt.Peek() > -1)
                {
                    string lines = sr_vt.ReadLine();
                    sr_vt_read.Add(lines);
                    j = j + 1;
                    lines2 = lines;
                }

                sr_vt.Close();

                j = 0;
                lines2 = "";
                while (sr_vn.Peek() > -1)
                {
                    string lines = sr_vn.ReadLine();
                    sr_vn_read.Add(lines);
                    j = j + 1;
                    lines2 = lines;
                }

                sr_vn.Close();
                
                File.Delete(filedirectory + @"\面.txt");
                File.Delete(filedirectory + @"\テクスチャ座標.txt");
                File.Delete(filedirectory + @"\法線.txt");

                //面の数を代入
                surface_all = sr_f_read.Count;
                int sr_s_gyousuu = sr_f_read.Count - 2;
                int sr_s_mensuu = sr_f_read.Count - 1;
                int sr_s_mensuu_nokori = sr_f_read.Count - 1;

                //nゴンが存在する場合、エラーを表示する
                for (int i = 0; i < sr_s_mensuu; i++)
                {
                    spacecount_n = sr_f_read.ElementAt(i).CountOf(" ");
                    if (spacecount_n > 8)
                    {
                        keikoku = true;
                        goto labelfinish2;
                    }
                }

                FileStream fsp = File.Create(filedirectory + @"\頂点データ" + ".bin");
                FileStream fsf = File.Create(filedirectory + @"\面データ" + ".bin");

                int seek_p = 0;
                int seek_f = 0; 
                int surfacebangou = 0;

                //変換できる面がなくなるまで繰り返す
                while (true)
                {
                    bgWorker.ReportProgress(surfacebangou);
                    if (sr_s_mensuu_nokori >= 0)
                    {
                        string read_dainyu = sr_f_read.ElementAt(surfacebangou);
                        int k = read_dainyu.CountOf(" ");
                        read_dainyu = read_dainyu.Remove(0, 1);
                        //面の番号を取得する
                        int space = read_dainyu.IndexOf(" ");

                        string move1 = read_dainyu.Substring(0, space);
                        read_dainyu = read_dainyu.Remove(0, move1.Length + 2);
                        space = read_dainyu.IndexOf(" ");
                        string move2 = read_dainyu.Substring(0, space);
                        read_dainyu = read_dainyu.Remove(0, move2.Length + 2);
                        space = read_dainyu.IndexOf(" ");
                        string move3 = read_dainyu.Substring(0, space);
                        read_dainyu = read_dainyu.Remove(0, move3.Length + 1);
                        string move4 = "";
                        
                        move1 = move1.Replace(" ", "");
                        move2 = move2.Replace(" ", "");
                        move3 = move3.Replace(" ", "");

                        if (k == 8)
                        {
                            read_dainyu = read_dainyu.Remove(0, 1);
                            space = read_dainyu.IndexOf(" ");
                            move4 = read_dainyu.Substring(0, space);
                            move4 = move4.Replace(" ", "");
                        }

                        List<int> sur_num_dainyu = new List<int>();
                        sur_num_dainyu.Add(int.Parse(move1));
                        sur_num_dainyu.Add(int.Parse(move2));
                        sur_num_dainyu.Add(int.Parse(move3));

                        if (k == 8)
                            sur_num_dainyu.Add(int.Parse(move4));

                        //最も多く張れるように面を張っていく
                        List<int> v_num_list = new List<int>();
                        List<int> vt_num_list = new List<int>();
                        List<int> vn_num_list = new List<int>();

                        string num1 = " " + sur_num_dainyu.ElementAt(0).ToString() + " ";
                        string num2 = " " + sur_num_dainyu.ElementAt(1).ToString() + " ";
                        string num3 = " " + sur_num_dainyu.ElementAt(2).ToString() + " ";
                        string num4 = "";
                        if (k == 8)
                            num4 = " " + sur_num_dainyu.ElementAt(3).ToString() + " ";

                        //頂点の番号を代入する
                        num1 = num1.Replace(" ", "");
                        num2 = num2.Replace(" ", "");
                        num3 = num3.Replace(" ", "");
                        if (k == 8)
                            num4 = num4.Replace(" ", "");

                        v_num_list.Add(int.Parse(num1));
                        v_num_list.Add(int.Parse(num2));
                        v_num_list.Add(int.Parse(num3));
                        if (k == 8)
                            v_num_list.Add(int.Parse(num4));

                        //法線の番号を代入する
                        read_dainyu = sr_vn_read.ElementAt(surfacebangou);
                        read_dainyu = read_dainyu.Remove(0, 1);

                        space = read_dainyu.IndexOf(" ");

                        string vn1 = read_dainyu.Substring(0, space);
                        read_dainyu = read_dainyu.Remove(0, vn1.Length + 2);
                        space = read_dainyu.IndexOf(" ");
                        string vn2 = read_dainyu.Substring(0, space);
                        read_dainyu = read_dainyu.Remove(0, vn2.Length + 2);
                        space = read_dainyu.IndexOf(" ");
                        string vn3 = read_dainyu.Substring(0, space);
                        read_dainyu = read_dainyu.Remove(0, vn3.Length + 1);
                        string vn4 = "";

                        if (k == 8)
                        {
                            read_dainyu = read_dainyu.Remove(0, 1);
                            space = read_dainyu.IndexOf(" ");
                            vn4 = read_dainyu.Substring(0, space);
                            vn4 = vn4.Replace(" ", "");
                        }

                        vn1 = vn1.Replace(" ", "");
                        vn2 = vn2.Replace(" ", "");
                        vn3 = vn3.Replace(" ", "");

                        vn_num_list.Add(int.Parse(vn1));
                        vn_num_list.Add(int.Parse(vn2));
                        vn_num_list.Add(int.Parse(vn3));
                        if (k == 8)
                            vn_num_list.Add(int.Parse(vn4));

                        //テクスチャ座標の番号を代入する
                        read_dainyu = sr_vt_read.ElementAt(surfacebangou);
                        read_dainyu = read_dainyu.Remove(0, 1);

                        space = read_dainyu.IndexOf(" ");

                        string vt1 = read_dainyu.Substring(0, space);
                        read_dainyu = read_dainyu.Remove(0, vt1.Length + 2);
                        space = read_dainyu.IndexOf(" ");
                        string vt2 = read_dainyu.Substring(0, space);
                        read_dainyu = read_dainyu.Remove(0, vt2.Length + 2);
                        space = read_dainyu.IndexOf(" ");
                        string vt3 = read_dainyu.Substring(0, space);
                        read_dainyu = read_dainyu.Remove(0, vt3.Length + 1);
                        string vt4 = "";

                        if (k == 8)
                        {
                            read_dainyu = read_dainyu.Remove(0, 1);
                            space = read_dainyu.IndexOf(" ");
                            vt4 = read_dainyu.Substring(0, space);
                            vt4 = vt4.Replace(" ", "");
                        }

                        vt1 = vt1.Replace(" ", "");
                        vt2 = vt2.Replace(" ", "");
                        vt3 = vt3.Replace(" ", "");

                        vt_num_list.Add(int.Parse(vt1));
                        vt_num_list.Add(int.Parse(vt2));
                        vt_num_list.Add(int.Parse(vt3));
                        if (k == 8)
                            vt_num_list.Add(int.Parse(vt4));

                        int kurikaeshi = k / 2;
                        for (int i = 0; i < kurikaeshi; i++)
                        {
                            int n = i;

                            //面の変換処理
                            //X座標
                            //浮動小数点を16進数に変換する
                            dou = pos_x.ElementAt(v_num_list.ElementAt(n) - 1);
                            dou = -dou;
                            dainyu = FloatToString(dou);
                            pos_all.Add(dainyu);

                            //Z座標
                            //浮動小数点を16進数に変換する
                            dou = pos_z.ElementAt(v_num_list.ElementAt(n) - 1);
                            dainyu = FloatToString(dou);
                            pos_all.Add(dainyu);

                            //Y座標
                            //浮動小数点を16進数に変換する
                            dou = pos_y.ElementAt(v_num_list.ElementAt(n) - 1);
                            dainyu = FloatToString(dou);
                            pos_all.Add(dainyu);

                            pos_all.Add(dainyu_1);

                            //法線の変換処理
                            //X座標
                            //浮動小数点を16進数に変換する
                            dou = pos_x_vn.ElementAt(vn_num_list.ElementAt(n) - 1);
                            dou = -dou;
                            dainyu = FloatToString(dou);
                            pos_all.Add(dainyu);

                            //Z座標
                            //浮動小数点を16進数に変換する
                            dou = pos_z_vn.ElementAt(vn_num_list.ElementAt(n) - 1);
                            dainyu = FloatToString(dou);
                            pos_all.Add(dainyu);

                            //Y座標
                            //浮動小数点を16進数に変換する
                            dou = pos_y_vn.ElementAt(vn_num_list.ElementAt(n) - 1);
                            dainyu = FloatToString(dou);
                            pos_all.Add(dainyu);

                            //テクスチャ座標の変換処理
                            //X座標
                            //浮動小数点を16進数に変換する
                            dou = pos_x_vt.ElementAt(vt_num_list.ElementAt(n) - 1);
                            dainyu = FloatToString(dou);
                            pos_all.Add(dainyu);

                            //Y座標
                            //浮動小数点を16進数に変換する
                            dou = pos_y_vt.ElementAt(vt_num_list.ElementAt(n) - 1);
                            dou = -dou;
                            dainyu = FloatToString(dou);
                            pos_all.Add(dainyu);

                            for (int m = 0; m < pos_all.Count; m++)
                            {
                                //頂点データ.binに代入する
                                dainyu_byte = StringToBytes(pos_all.ElementAt(m));
                                fsp.Write(dainyu_byte, 0, 4);
                                seek_p += 4;
                                fsp.Seek(seek_p, SeekOrigin.Begin);
                            }

                            pos_all = new List<string>();
                        }

                        int mensuu = 0;
                        
                        mensuu = kurikaeshi;

                        sr_s_mensuu_nokori -= 1;
                        surfacebangou += 1;

                        //面データ.binに「00 00 00 (面数)」を書き込み
                        //面数を16進数に変換する
                        string hex = mensuu.ToString("X");
                        if (hex.Length == 1)
                        {
                            hex1 = "0" + hex;
                            dainyu = hex_00 + hex_00 + hex_00 + hex1;
                        }
                        else if (hex.Length == 2)
                            dainyu = hex_00 + hex_00 + hex_00 + hex;
                        else if (hex.Length == 3)
                        {
                            hex1 = "0" + hex.Remove(1, 2);
                            hex2 = hex.Remove(0, 1);
                            dainyu = hex_00 + hex_00 + hex1 + hex2;
                        }
                        else if (hex.Length == 4)
                        {
                            hex1 = hex.Remove(2, 2);
                            hex2 = hex.Remove(0, 2);
                            dainyu = hex_00 + hex_00 + hex1 + hex2;
                        }
                        byte[] f_multi_byte = StringToBytes(dainyu);
                        fsf.Write(f_multi_byte, 0, 4);
                        seek_f = seek_f + 4;
                        fsf.Seek(seek_f, SeekOrigin.Begin);
                        mensuu = 0;
                    }

                    else
                        break;
                }

                fsp.Close();
                fsf.Close();

                FileStream fspread2 = new FileStream(filedirectory + @"\頂点データ.bin", FileMode.Open, FileAccess.Read);
                FileStream fsfread2 = new FileStream(filedirectory + @"\面データ.bin", FileMode.Open, FileAccess.Read);

                byte[] fsp2byte = new byte[fspread2.Length];
                fspread2.Read(fsp2byte, 0, fsp2byte.Length);
                fspread2.Close();

                byte[] fsf2byte = new byte[fsfread2.Length];
                fsfread2.Read(fsf2byte, 0, fsf2byte.Length);
                fsfread2.Close();

                FileStream fsp2 = new FileStream(filedirectory + @"\頂点データ.bin", FileMode.Create, FileAccess.Write);
                FileStream fsf2 = new FileStream(filedirectory + @"\面データ.bin", FileMode.Create, FileAccess.Write);

                //頂点数を代入する
                int read_hex1 = fsp2byte.Length;
                read_hex1 /= 36;
                string dou_16 = BitConverter.ToString(BitConverter.GetBytes(read_hex1), 0);
                dou_16 = dou_16.Replace("-", "");
                dainyu_byte = StringToBytes(dou_16);
                read_hex1 = dainyu_byte[0];
                int read_hex2 = dainyu_byte[1];
                hex1 = Convert.ToString(read_hex1, 16);
                int len_hex = hex1.Length;
                if (len_hex == 1)
                    hex1 = hex_0 + hex1;
                hex2 = Convert.ToString(read_hex2, 16);
                len_hex = hex2.Length;
                if (len_hex == 1)
                    hex2 = hex_0 + hex2;
                dainyu = hex1 + hex2;
                dainyu_byte = StringToBytes(dainyu);
                fsp2.Write(dainyu_byte, 0, dainyu_byte.Length);
                if (fsp2byte.Length == 0)
                fsp2.Write(dainyu_byte, 0, dainyu_byte.Length);
                fsp2.Seek(4, SeekOrigin.Begin);
                fsp2.Write(fsp2byte, 0, fsp2byte.Length);
                seek_p = fsp2byte.Length + 4;

                //面数を代入する
                read_hex2 = fsf2byte.Length;
                read_hex2 /= 4;
                hex2 = Convert.ToString(read_hex2);
                int hex2int = int.Parse(hex2);
                hex2int += 1;
                dainyu_byte = Gethex(hex2int);
                fsf2.Write(dainyu_byte, 0, 4);
                fsf2.Seek(4, SeekOrigin.Begin);
                fsf2.Write(dainyu_surface1, 0, 1);
                fsf2.Seek(5, SeekOrigin.Begin);
                fsf2.Write(fsf2byte, 0, fsf2byte.Length);
                fsf2.Seek(fsf2byte.Length + 5, SeekOrigin.Begin);
                fsf2.Write(dainyu_surface, 0, 4);

                fsp2.Close();
                fsf2.Close();

                string[] files = Directory.GetFiles(xgdirectory, "*", SearchOption.AllDirectories);
                for (int i = 0; i < files.Count(); i++)
                {
                    FileStream fsrxg = new FileStream(files[i], FileMode.Open, FileAccess.Read);
                    string filename = Path.GetFileName(files[i]);
                    byte[] bsxg = new byte[fsrxg.Length];
                    fsrxg.Read(bsxg, 0, bsxg.Length);
                    fsrxg.Close();
                    byte[] newbsxg = new byte[0];
                    int seek_xg = 0;

                    if (0 <= bsxg.IndexOf(triStripData))
                    {
                        FileStream fsp3 = new FileStream(filedirectory + @"\頂点データ.bin", FileMode.Open, FileAccess.Read);
                        FileStream fsf3 = new FileStream(filedirectory + @"\面データ.bin", FileMode.Open, FileAccess.Read); 
                        
                        byte[] fsp3byte = new byte[fsp3.Length];
                        fsp3.Read(fsp3byte, 0, fsp3byte.Length);
                        fsp3.Close();

                        byte[] fsf3byte = new byte[fsf3.Length];
                        fsf3.Read(fsf3byte, 0, fsf3byte.Length);
                        fsf3.Close();

                        FileStream fswxg = new FileStream(filedirectory + @"\" + filename, FileMode.Create, FileAccess.Write);
                        int triStripCountlength = bsxg.IndexOf(triStripCount);
                        triStripCountlength += 13;
                        int triStripDatalength = bsxg.IndexOf(triStripData);
                        triStripDatalength += 12;
                        int triListCountlength = bsxg.IndexOf(triListCount);
                        triListCountlength -= 4;
                        int triListDatalength = bsxg.IndexOf(triListData);
                        triListDatalength -= 1;
                        int cullFunclength = bsxg.IndexOf(cullFunc);
                        int verticeslength = bsxg.IndexOf(vertices);
                        verticeslength += 12;
                        int xgMateriallength = 0;
                        if (0 <= bsxg.IndexOf(xgMaterial))
                            xgMateriallength = bsxg.IndexOf(xgMaterial);
                        int daglength = bsxg.IndexOf(dag);

                        fswxg.Write(bsxg, 0, triStripCountlength);
                        fswxg.Seek(triStripCountlength, SeekOrigin.Begin);
                        hex2int -= 1;
                        dainyu_byte = Gethex(hex2int);
                        fswxg.Write(dainyu_byte, 0, 4);
                        seek_xg = triStripCountlength + 4;
                        fswxg.Seek(seek_xg, SeekOrigin.Begin);

                        fswxg.Write(bsxg, triStripCountlength + 4, 13);
                        seek_xg += 13;
                        fswxg.Seek(seek_xg, SeekOrigin.Begin);
                        fswxg.Write(fsf3byte, 0, fsf3byte.Length);
                        seek_xg += fsf3byte.Length;
                        fswxg.Seek(seek_xg, SeekOrigin.Begin);

                        fswxg.Write(triListCount, 0, triListCount.Length);
                        seek_xg += triListCount.Length;
                        fswxg.Seek(seek_xg, SeekOrigin.Begin);
                        fswxg.Write(dainyu_none, 0, 4);
                        seek_xg += 4;
                        fswxg.Seek(seek_xg, SeekOrigin.Begin);
                        fswxg.Write(bsxg, triListDatalength, 12);
                        seek_xg += 12;
                        fswxg.Seek(seek_xg, SeekOrigin.Begin);
                        fswxg.Write(dainyu_none, 0, 4);
                        seek_xg += 4;
                        fswxg.Seek(seek_xg, SeekOrigin.Begin);
                        fswxg.Write(bsxg, cullFunclength, verticeslength - cullFunclength);
                        seek_xg += (verticeslength - cullFunclength);
                        fswxg.Seek(seek_xg, SeekOrigin.Begin);
                        fswxg.Write(fsp3byte, 0, fsp3byte.Length);
                        seek_xg += fsp3byte.Length;
                        fswxg.Seek(seek_xg, SeekOrigin.Begin);

                        if (xgMateriallength > verticeslength)
                            fswxg.Write(bsxg, xgMateriallength, bsxg.Length - xgMateriallength);
                        else if (xgMateriallength < verticeslength)
                            fswxg.Write(bsxg, daglength, bsxg.Length - daglength);

                        fswxg.Close();

                        File.Delete(filedirectory + @"\頂点データ.bin");
                        File.Delete(filedirectory + @"\面データ.bin");
                    }

                    else if (0 <= bsxg.IndexOf(rrposition))
                    {
                        FileStream fswxg = new FileStream(filedirectory + @"\" + filename, FileMode.Create, FileAccess.Write);
                        fswxg.Write(XGBv1, 0, XGBv1.Length);
                        seek_xg = 8;
                        string write = "";

                        for (int k = 0; k < mnumbers.Count(); k++)
                        {
                            write = "m" + (mnumbers.Count() - k).ToString();
                            byte[] writebyte = System.Text.Encoding.ASCII.GetBytes(write);

                            fswxg.Write(xgDagTransform, 0, xgDagTransform.Length);
                            seek_xg += xgDagTransform.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(writebyte, 0, writebyte.Length);
                            seek_xg += writebyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(xgBgMatrix, 0, xgBgMatrix.Length);
                            seek_xg += xgBgMatrix.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(writebyte, 0, writebyte.Length);
                            seek_xg += writebyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(matrix_0, 0, matrix_0.Length);
                            seek_xg += matrix_0.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(writebyte, 0, writebyte.Length);
                            seek_xg += writebyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(matrix_1, 0, matrix_1.Length);
                            seek_xg += matrix_1.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                        }

                        if (rrnumbers != "")
                        {
                            write = "rr";
                            byte[] writebyte = System.Text.Encoding.ASCII.GetBytes(write);

                            fswxg.Write(xgDagTransform, 0, xgDagTransform.Length);
                            seek_xg += xgDagTransform.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(writebyte, 0, writebyte.Length);
                            seek_xg += writebyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(xgBgMatrix, 0, xgBgMatrix.Length);
                            seek_xg += xgBgMatrix.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(writebyte, 0, writebyte.Length);
                            seek_xg += writebyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(matrix_0, 0, matrix_0.Length);
                            seek_xg += matrix_0.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(writebyte, 0, writebyte.Length);
                            seek_xg += writebyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(matrix_1, 0, matrix_1.Length);
                            seek_xg += matrix_1.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                        }

                        if (rlnumbers != "")
                        {
                            write = "rl";
                            byte[] writebyte = System.Text.Encoding.ASCII.GetBytes(write);

                            fswxg.Write(xgDagTransform, 0, xgDagTransform.Length);
                            seek_xg += xgDagTransform.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(writebyte, 0, writebyte.Length);
                            seek_xg += writebyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(xgBgMatrix, 0, xgBgMatrix.Length);
                            seek_xg += xgBgMatrix.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(writebyte, 0, writebyte.Length);
                            seek_xg += writebyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(matrix_0, 0, matrix_0.Length);
                            seek_xg += matrix_0.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(writebyte, 0, writebyte.Length);
                            seek_xg += writebyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(matrix_1, 0, matrix_1.Length);
                            seek_xg += matrix_1.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                        }

                        if (frnumbers != "")
                        {
                            write = "fr";
                            byte[] writebyte = System.Text.Encoding.ASCII.GetBytes(write);

                            fswxg.Write(xgDagTransform, 0, xgDagTransform.Length);
                            seek_xg += xgDagTransform.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(writebyte, 0, writebyte.Length);
                            seek_xg += writebyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(xgBgMatrix, 0, xgBgMatrix.Length);
                            seek_xg += xgBgMatrix.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(writebyte, 0, writebyte.Length);
                            seek_xg += writebyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(matrix_0, 0, matrix_0.Length);
                            seek_xg += matrix_0.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(writebyte, 0, writebyte.Length);
                            seek_xg += writebyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(matrix_1, 0, matrix_1.Length);
                            seek_xg += matrix_1.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                        }

                        if (flnumbers != "")
                        {
                            write = "fl";
                            byte[] writebyte = System.Text.Encoding.ASCII.GetBytes(write);

                            fswxg.Write(xgDagTransform, 0, xgDagTransform.Length);
                            seek_xg += xgDagTransform.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(writebyte, 0, writebyte.Length);
                            seek_xg += writebyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(xgBgMatrix, 0, xgBgMatrix.Length);
                            seek_xg += xgBgMatrix.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(writebyte, 0, writebyte.Length);
                            seek_xg += writebyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(matrix_0, 0, matrix_0.Length);
                            seek_xg += matrix_0.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(writebyte, 0, writebyte.Length);
                            seek_xg += writebyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(matrix_1, 0, matrix_1.Length);
                            seek_xg += matrix_1.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                        }

                        for (int k = 0; k < mnumbers.Count(); k++)
                        {
                            write = "m" + (mnumbers.Count() - k).ToString();
                            byte[] writebyte = System.Text.Encoding.ASCII.GetBytes(write);

                            fswxg.Write(xgDagTransform, 0, xgDagTransform.Length);
                            seek_xg += xgDagTransform.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(writebyte, 0, writebyte.Length);
                            seek_xg += writebyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(imputMatrix, 0, imputMatrix.Length);
                            seek_xg += imputMatrix.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(writebyte, 0, writebyte.Length);
                            seek_xg += writebyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(outputMatrix, 0, outputMatrix.Length);
                            seek_xg += outputMatrix.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(writebyte, 0, writebyte.Length);
                            seek_xg += writebyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(imputParentMatrix, 0, imputParentMatrix.Length);
                            seek_xg += imputParentMatrix.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(writebyte, 0, writebyte.Length);
                            seek_xg += writebyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(VpibotMatrix, 0, VpibotMatrix.Length);
                            seek_xg += VpibotMatrix.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(writebyte, 0, writebyte.Length);
                            seek_xg += writebyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(position, 0, position.Length);
                            seek_xg += position.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            float writex = pos_x_mnumbers.ElementAt(mnumbers.Count() - k - 1);
                            writex = -writex;
                            dainyu = FloatToString(writex);
                            dainyu_byte = StringToBytes(dainyu);
                            fswxg.Write(dainyu_byte, 0, 4);
                            seek_xg += 4;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            dainyu = FloatToString(pos_z_mnumbers.ElementAt(mnumbers.Count() - k - 1));
                            dainyu_byte = StringToBytes(dainyu);
                            fswxg.Write(dainyu_byte, 0, 4);
                            seek_xg += 4;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            dainyu = FloatToString(pos_y_mnumbers.ElementAt(mnumbers.Count() - k - 1));
                            dainyu_byte = StringToBytes(dainyu);
                            fswxg.Write(dainyu_byte, 0, 4);
                            seek_xg += 4;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(rotation, 0, rotation.Length);
                            seek_xg += rotation.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                        }

                        if (rrnumbers != "")
                        {
                            write = "rr";
                            byte[] writebyte = System.Text.Encoding.ASCII.GetBytes(write);

                            fswxg.Write(xgDagTransform, 0, xgDagTransform.Length);
                            seek_xg += xgDagTransform.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(writebyte, 0, writebyte.Length);
                            seek_xg += writebyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(imputMatrix, 0, imputMatrix.Length);
                            seek_xg += imputMatrix.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(writebyte, 0, writebyte.Length);
                            seek_xg += writebyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(outputMatrix, 0, outputMatrix.Length);
                            seek_xg += outputMatrix.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(writebyte, 0, writebyte.Length);
                            seek_xg += writebyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(imputParentMatrix, 0, imputParentMatrix.Length);
                            seek_xg += imputParentMatrix.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(writebyte, 0, writebyte.Length);
                            seek_xg += writebyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(VpibotMatrix, 0, VpibotMatrix.Length);
                            seek_xg += VpibotMatrix.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(writebyte, 0, writebyte.Length);
                            seek_xg += writebyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(position, 0, position.Length);
                            seek_xg += position.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            pos_x_rrnumbers = -pos_x_rrnumbers;
                            dainyu = FloatToString(pos_x_rrnumbers);
                            dainyu_byte = StringToBytes(dainyu);
                            fswxg.Write(dainyu_byte, 0, 4);
                            seek_xg += 4;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            dainyu = FloatToString(pos_z_rrnumbers);
                            dainyu_byte = StringToBytes(dainyu);
                            fswxg.Write(dainyu_byte, 0, 4);
                            seek_xg += 4;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            dainyu = FloatToString(pos_y_rrnumbers);
                            dainyu_byte = StringToBytes(dainyu);
                            fswxg.Write(dainyu_byte, 0, 4);
                            seek_xg += 4;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(rotation, 0, rotation.Length);
                            seek_xg += rotation.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                        }

                        if (rlnumbers != "")
                        {
                            write = "rl";
                            byte[] writebyte = System.Text.Encoding.ASCII.GetBytes(write);

                            fswxg.Write(xgDagTransform, 0, xgDagTransform.Length);
                            seek_xg += xgDagTransform.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(writebyte, 0, writebyte.Length);
                            seek_xg += writebyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(imputMatrix, 0, imputMatrix.Length);
                            seek_xg += imputMatrix.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(writebyte, 0, writebyte.Length);
                            seek_xg += writebyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(outputMatrix, 0, outputMatrix.Length);
                            seek_xg += outputMatrix.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(writebyte, 0, writebyte.Length);
                            seek_xg += writebyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(imputParentMatrix, 0, imputParentMatrix.Length);
                            seek_xg += imputParentMatrix.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(writebyte, 0, writebyte.Length);
                            seek_xg += writebyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(VpibotMatrix, 0, VpibotMatrix.Length);
                            seek_xg += VpibotMatrix.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(writebyte, 0, writebyte.Length);
                            seek_xg += writebyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(position, 0, position.Length);
                            seek_xg += position.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            pos_x_rlnumbers = -pos_x_rlnumbers;
                            dainyu = FloatToString(pos_x_rlnumbers);
                            dainyu_byte = StringToBytes(dainyu);
                            fswxg.Write(dainyu_byte, 0, 4);
                            seek_xg += 4;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            dainyu = FloatToString(pos_z_rlnumbers);
                            dainyu_byte = StringToBytes(dainyu);
                            fswxg.Write(dainyu_byte, 0, 4);
                            seek_xg += 4;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            dainyu = FloatToString(pos_y_rlnumbers);
                            dainyu_byte = StringToBytes(dainyu);
                            fswxg.Write(dainyu_byte, 0, 4);
                            seek_xg += 4;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(rotation, 0, rotation.Length);
                            seek_xg += rotation.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                        }

                        if (frnumbers != "")
                        {
                            write = "fr";
                            byte[] writebyte = System.Text.Encoding.ASCII.GetBytes(write);

                            fswxg.Write(xgDagTransform, 0, xgDagTransform.Length);
                            seek_xg += xgDagTransform.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(writebyte, 0, writebyte.Length);
                            seek_xg += writebyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(imputMatrix, 0, imputMatrix.Length);
                            seek_xg += imputMatrix.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(writebyte, 0, writebyte.Length);
                            seek_xg += writebyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(outputMatrix, 0, outputMatrix.Length);
                            seek_xg += outputMatrix.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(writebyte, 0, writebyte.Length);
                            seek_xg += writebyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(imputParentMatrix, 0, imputParentMatrix.Length);
                            seek_xg += imputParentMatrix.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(writebyte, 0, writebyte.Length);
                            seek_xg += writebyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(VpibotMatrix, 0, VpibotMatrix.Length);
                            seek_xg += VpibotMatrix.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(writebyte, 0, writebyte.Length);
                            seek_xg += writebyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(position, 0, position.Length);
                            seek_xg += position.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            pos_x_frnumbers = -pos_x_frnumbers;
                            dainyu = FloatToString(pos_x_frnumbers);
                            dainyu_byte = StringToBytes(dainyu);
                            fswxg.Write(dainyu_byte, 0, 4);
                            seek_xg += 4;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            dainyu = FloatToString(pos_z_frnumbers);
                            dainyu_byte = StringToBytes(dainyu);
                            fswxg.Write(dainyu_byte, 0, 4);
                            seek_xg += 4;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            dainyu = FloatToString(pos_y_frnumbers);
                            dainyu_byte = StringToBytes(dainyu);
                            fswxg.Write(dainyu_byte, 0, 4);
                            seek_xg += 4;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(rotation, 0, rotation.Length);
                            seek_xg += rotation.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                        }

                        if (flnumbers != "")
                        {
                            write = "fl";
                            byte[] writebyte = System.Text.Encoding.ASCII.GetBytes(write);

                            fswxg.Write(xgDagTransform, 0, xgDagTransform.Length);
                            seek_xg += xgDagTransform.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(writebyte, 0, writebyte.Length);
                            seek_xg += writebyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(imputMatrix, 0, imputMatrix.Length);
                            seek_xg += imputMatrix.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(writebyte, 0, writebyte.Length);
                            seek_xg += writebyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(outputMatrix, 0, outputMatrix.Length);
                            seek_xg += outputMatrix.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(writebyte, 0, writebyte.Length);
                            seek_xg += writebyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(imputParentMatrix, 0, imputParentMatrix.Length);
                            seek_xg += imputParentMatrix.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(writebyte, 0, writebyte.Length);
                            seek_xg += writebyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(VpibotMatrix, 0, VpibotMatrix.Length);
                            seek_xg += VpibotMatrix.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(writebyte, 0, writebyte.Length);
                            seek_xg += writebyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(position, 0, position.Length);
                            seek_xg += position.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            pos_x_flnumbers = -pos_x_flnumbers;
                            dainyu = FloatToString(pos_x_flnumbers);
                            dainyu_byte = StringToBytes(dainyu);
                            fswxg.Write(dainyu_byte, 0, 4);
                            seek_xg += 4;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            dainyu = FloatToString(pos_z_flnumbers);
                            dainyu_byte = StringToBytes(dainyu);
                            fswxg.Write(dainyu_byte, 0, 4);
                            seek_xg += 4;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            dainyu = FloatToString(pos_y_flnumbers);
                            dainyu_byte = StringToBytes(dainyu);
                            fswxg.Write(dainyu_byte, 0, 4);
                            seek_xg += 4;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            fswxg.Write(rotation, 0, rotation.Length);
                            seek_xg += rotation.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                        }

                        fswxg.Write(dag_node, 0, dag_node.Length);
                        seek_xg += dag_node.Length;
                        fswxg.Seek(seek_xg, SeekOrigin.Begin);

                        for (int k = 0; k < mnumbers.Count(); k++)
                        {
                            write = "m" + (mnumbers.Count() - k).ToString();
                            byte[] dagbyte = System.Text.Encoding.ASCII.GetBytes(write);

                            fswxg.Write(dagbyte, 0, dagbyte.Length);
                            seek_xg += dagbyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);

                            if (k + 1 < mnumbers.Count())
                            {
                                fswxg.Write(dagspace, 0, dagspace.Length);
                                seek_xg += dagspace.Length;
                                fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            }
                        }

                        if (rrnumbers != "")
                        {
                            write = "rr";
                            byte[] dagbyte = System.Text.Encoding.ASCII.GetBytes(write);

                            fswxg.Write(dagspace, 0, dagspace.Length);
                            seek_xg += dagspace.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(dagbyte, 0, dagbyte.Length);
                            seek_xg += dagbyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                        }

                        if (rlnumbers != "")
                        {
                            write = "rl";
                            byte[] dagbyte = System.Text.Encoding.ASCII.GetBytes(write);

                            fswxg.Write(dagspace, 0, dagspace.Length);
                            seek_xg += dagspace.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(dagbyte, 0, dagbyte.Length);
                            seek_xg += dagbyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                        }

                        if (frnumbers != "")
                        {
                            write = "fr";
                            byte[] dagbyte = System.Text.Encoding.ASCII.GetBytes(write);

                            fswxg.Write(dagspace, 0, dagspace.Length);
                            seek_xg += dagspace.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(dagbyte, 0, dagbyte.Length);
                            seek_xg += dagbyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                        }

                        if (flnumbers != "")
                        {
                            write = "fl";
                            byte[] dagbyte = System.Text.Encoding.ASCII.GetBytes(write);

                            fswxg.Write(dagspace, 0, dagspace.Length);
                            seek_xg += dagspace.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                            fswxg.Write(dagbyte, 0, dagbyte.Length);
                            seek_xg += dagbyte.Length;
                            fswxg.Seek(seek_xg, SeekOrigin.Begin);
                        }

                        fswxg.Write(dagend, 0, dagend.Length);

                        fswxg.Close();
                    }
                }

                goto labelfinish;

            labelfinish2:;

                fsr.Close();

                if (keikoku == true)
                    MessageBox.Show("エラー:Nゴンが含まれているため変換できません");

            labelfinish:;
            }
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

        //floatを16進数stringに変換
        public static string FloatToString(float fl)
        {
            string fl_16 = BitConverter.ToString(BitConverter.GetBytes(fl), 0);
            fl_16 = fl_16.Replace("-", "");
            if (fl == 0)
                fl_16 = "00000000";
            byte[] dainyu = new byte[4];
            dainyu = StringToBytes(fl_16);
            int read_hex1 = dainyu[0];
            int read_hex2 = dainyu[1];
            int read_hex3 = dainyu[2];
            int read_hex4 = dainyu[3];
            string hex1 = Convert.ToString(read_hex1, 16);
            int len_hex = hex1.Length;
            if (len_hex == 1)
                hex1 = hex_0 + hex1;
            string hex2 = Convert.ToString(read_hex2, 16);
            len_hex = hex2.Length;
            if (len_hex == 1)
                hex2 = hex_0 + hex2;
            string hex3 = Convert.ToString(read_hex3, 16);
            len_hex = hex3.Length;
            if (len_hex == 1)
                hex3 = hex_0 + hex3;
            string hex4 = Convert.ToString(read_hex4, 16);
            len_hex = hex4.Length;
            if (len_hex == 1)
                hex4 = hex_0 + hex4;
            string st_dainyu = hex1 + hex2 + hex3 + hex4;
            return st_dainyu;
        }

        // 16進数文字列 => Byte配列
        public static byte[] StringToBytes(string str)
        {
            var bs = new List<byte>();
            for (int i = 0; i < str.Length / 2; i++)
            {
                bs.Add(Convert.ToByte(str.Substring(i * 2, 2), 16));
            }
            // "01-AB-EF" こういう"-"区切りを想定する場合は以下のようにする
            // var bs = str.Split('-').Select(hex => Convert.ToByte(hex, 16));
            return bs.ToArray();
        }
    }
}

public static partial class StringExtensions
{
    /// <summary>
    /// 指定した文字列がいくつあるか
    /// </summary>
    public static int CountOf(this string self, params string[] strArray)
    {
        int count = 0;

        foreach (string str in strArray)
        {
            int index = self.IndexOf(str, 0);
            while (index != -1)
            {
                count++;
                index = self.IndexOf(str, index + str.Length);
            }
        }

        return count;
    }
}

public static class IEnumerableExtensions
{
    public static IEnumerable<int> IndicesOf<T>(this IEnumerable<T> source, IList<T> target)
    {
        if (target == null || target.Count == 0)
            return Enumerable.Empty<int>();
        return source
            .Select((x, i) => new { index = i, value = source.Skip(i).Take(target.Count) })
            .Where(x => x.value.SequenceEqual(target))
            .Select(x => x.index);
    }

    public static int IndexOf<T>(this IEnumerable<T> source, IList<T> target)
    {
        return source.IndicesOf(target).DefaultIfEmpty(-1).First();
    }
}
