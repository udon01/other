using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;

namespace まげつけ_hitdata変換ツール
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static string objpath;
        public static string filepath;
        public static bool keikoku = false;
        public static byte[] zero4 = new byte[4] { 0x00, 0x00, 0x00, 0x00 };
        public static byte[] kokomate = new byte[4] { 0xBA, 0xBA, 0xCF, 0xC3 };
        public static int seek_p = 0;
        public static byte[] dainyu_byte = new byte[4];
        public static string hex_0 = "0";
        public static string hex_00 = "00";
        public static float dou;
        public static string dainyu;
        public static List<string> pos_all = new List<string>();
        public static bool close = false;

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
            ProgressBar1.Maximum = 12;
            Label1.Text = e.ProgressPercentage.ToString() + "/" + "12".ToString();
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
                string fileextension = Path.GetExtension(path[b]);
                string filedirectory = Path.GetDirectoryName(path[b]);

                // obj以外ならhitdataを元に戻す処理に移る
                if (fileextension != ".obj" && fileextension != ".OBJ")
                    goto label2;

                // ドラッグ＆ドロップされたobjファイル
                StreamReader fsr = new StreamReader(path[b]);

                string v_vt_vn_f;
                string[] vnumbers = new string[0];
                int vcount = 0;

            label1:;

                v_vt_vn_f = fsr.ReadLine();

                if (v_vt_vn_f == null)
                {
                    if (vcount == 0)
                    {
                        //MessageBox.Show("頂点座標(v)が1つもありません");
                        //goto labelfinish2;
                    }

                    //else
                    //goto label2;
                }

                else if (0 <= v_vt_vn_f.IndexOf("o "))
                    goto label1;

                else if (0 <= v_vt_vn_f.IndexOf("#") || 0 <= v_vt_vn_f.IndexOf("mtllib") || 0 <= v_vt_vn_f.IndexOf("usemtl")
                      || 0 <= v_vt_vn_f.IndexOf("s") || 0 <= v_vt_vn_f.IndexOf("l") || 0 <= v_vt_vn_f.IndexOf("vt")
                      || 0 <= v_vt_vn_f.IndexOf("vn") || 0 <= v_vt_vn_f.IndexOf("f"))
                    goto label1;

                else if (0 <= v_vt_vn_f.IndexOf("v "))
                {
                    vcount = vcount + 1;
                    Array.Resize(ref vnumbers, vnumbers.Length + 1);
                    vnumbers[vnumbers.Length - 1] = v_vt_vn_f;
                    goto label1;
                }

                fsr.Close();

                if (vnumbers.Count() != 12)
                {
                    keikoku = true;
                    goto labelfinish;
                }

                //頂点座標の変換処理
                string minus;
                int p_read;
                string p_st_number;
                float p_number;
                float p_number_x = 0;
                float p_number_z = 0;
                float p_number_y = 0;
                int removedec;
                List<Vertex> vertexlist = new List<Vertex>();

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

                        //代入する
                        if (three == 0)
                            p_number_x = p_number;

                        else if (three == 1)
                            p_number_z = p_number;

                        else if (three == 2)
                            p_number_y = p_number;

                        //座標情報を削除
                        if (three == 2)
                            vnumbers[m] = vnumbers[m].Remove(0, p_read);

                        else
                            vnumbers[m] = vnumbers[m].Remove(0, p_read + 1);
                        //3回繰り返す
                    }
                    vertexlist.Add(new Vertex(m, p_number_x, p_number_z, p_number_y));
                }

                FileStream fsp = File.Create(filedirectory + @"\hitdata_new" + ".bin");

                //左フロント_低点
                bgWorker.ReportProgress(0);
                List<int> sortlistf1_num = vertexlist.Select(x => x.num).ToList();
                List<float> sortlistf1_x = vertexlist.Select(x => x.vertex_x).ToList();
                List<float> sortlistf1_z = vertexlist.Select(x => x.vertex_z).ToList();
                List<float> sortlistf1_y = vertexlist.Select(x => x.vertex_y).ToList();
                List<Vertex> sortlist1 = new List<Vertex>();
                for (int m = 0; m < 6; m++)
                {
                    float sort6_x = sortlistf1_x.OrderBy(n => n).Skip(m).FirstOrDefault();
                    List<int> sortlisti1 = vertexlist.Where(x => x.vertex_x == sort6_x).Select(x => x.num).ToList();
                    foreach (int item in sortlisti1)
                    {
                        int sort6_num = sortlistf1_num.Skip(item).FirstOrDefault();
                        float sort6_z = sortlistf1_z.Skip(item).FirstOrDefault();
                        float sort6_y = sortlistf1_y.Skip(item).FirstOrDefault();
                        sortlist1.Add(new Vertex(sort6_num, sort6_x, sort6_z, sort6_y));
                    }
                    if (sortlist1.Count() >= 6)
                        break;
                }
                var sort2 = sortlist1.Max(x => x.vertex_y);
                var sortlist2 = sortlist1.Where(x => x.vertex_y == sort2).ToList();
                var sort3 = sortlist2.Min(x => x.vertex_z);
                var sortlist3 = sortlist2.Where(x => x.vertex_z == sort3).ToList();

                //頂点座標の変換処理
                //X座標
                //浮動小数点を16進数に変換する
                var ext = sortlist3.Select(x => x.vertex_x);
                foreach (var floatvertex in ext)
                {
                    dou = floatvertex;
                }
                dainyu = FloatToString(dou);
                pos_all.Add(dainyu);

                //Z座標
                //浮動小数点を16進数に変換する
                ext = sortlist3.Select(x => x.vertex_z);
                foreach (var floatvertex in ext)
                {
                    dou = floatvertex;
                }
                dainyu = FloatToString(dou);
                pos_all.Add(dainyu);

                //Y座標
                //浮動小数点を16進数に変換する
                ext = sortlist3.Select(x => x.vertex_y);
                foreach (var floatvertex in ext)
                {
                    dou = floatvertex;
                }
                dainyu = FloatToString(dou);
                pos_all.Add(dainyu);

                for (int m = 0; m < pos_all.Count; m++)
                {
                    //hitdata_new.binに代入する
                    dainyu_byte = StringToBytes(pos_all.ElementAt(m));
                    fsp.Write(dainyu_byte, 0, 4);
                    seek_p += 4;
                    fsp.Seek(seek_p, SeekOrigin.Begin);
                }
                fsp.Write(zero4, 0, 4);
                seek_p += 4;
                fsp.Seek(seek_p, SeekOrigin.Begin);
                pos_all = new List<string>();

                //左リア_低点
                bgWorker.ReportProgress(1);
                sort2 = sortlist1.Min(x => x.vertex_y);
                sortlist2 = sortlist1.Where(x => x.vertex_y == sort2).ToList();
                sort3 = sortlist2.Min(x => x.vertex_z);
                sortlist3 = sortlist2.Where(x => x.vertex_z == sort3).ToList();

                //頂点座標の変換処理
                //X座標
                //浮動小数点を16進数に変換する
                ext = sortlist3.Select(x => x.vertex_x);
                foreach (var floatvertex in ext)
                {
                    dou = floatvertex;
                }
                dainyu = FloatToString(dou);
                pos_all.Add(dainyu);

                //Z座標
                //浮動小数点を16進数に変換する
                ext = sortlist3.Select(x => x.vertex_z);
                foreach (var floatvertex in ext)
                {
                    dou = floatvertex;
                }
                dainyu = FloatToString(dou);
                pos_all.Add(dainyu);

                //Y座標
                //浮動小数点を16進数に変換する
                ext = sortlist3.Select(x => x.vertex_y);
                foreach (var floatvertex in ext)
                {
                    dou = floatvertex;
                }
                dainyu = FloatToString(dou);
                pos_all.Add(dainyu);

                for (int m = 0; m < pos_all.Count; m++)
                {
                    //hitdata_new.binに代入する
                    dainyu_byte = StringToBytes(pos_all.ElementAt(m));
                    fsp.Write(dainyu_byte, 0, 4);
                    seek_p += 4;
                    fsp.Seek(seek_p, SeekOrigin.Begin);
                }
                fsp.Write(zero4, 0, 4);
                seek_p += 4;
                fsp.Seek(seek_p, SeekOrigin.Begin);
                pos_all = new List<string>();

                //左リア_上点
                bgWorker.ReportProgress(2);
                sort2 = sortlist1.Min(x => x.vertex_y);
                sortlist2 = sortlist1.Where(x => x.vertex_y == sort2).ToList();
                sort3 = sortlist2.Max(x => x.vertex_z);
                sortlist3 = sortlist2.Where(x => x.vertex_z == sort3).ToList();

                //頂点座標の変換処理
                //X座標
                //浮動小数点を16進数に変換する
                ext = sortlist3.Select(x => x.vertex_x);
                foreach (var floatvertex in ext)
                {
                    dou = floatvertex;
                }
                dainyu = FloatToString(dou);
                pos_all.Add(dainyu);

                //Z座標
                //浮動小数点を16進数に変換する
                ext = sortlist3.Select(x => x.vertex_z);
                foreach (var floatvertex in ext)
                {
                    dou = floatvertex;
                }
                dainyu = FloatToString(dou);
                pos_all.Add(dainyu);

                //Y座標
                //浮動小数点を16進数に変換する
                ext = sortlist3.Select(x => x.vertex_y);
                foreach (var floatvertex in ext)
                {
                    dou = floatvertex;
                }
                dainyu = FloatToString(dou);
                pos_all.Add(dainyu);

                for (int m = 0; m < pos_all.Count; m++)
                {
                    //hitdata_new.binに代入する
                    dainyu_byte = StringToBytes(pos_all.ElementAt(m));
                    fsp.Write(dainyu_byte, 0, 4);
                    seek_p += 4;
                    fsp.Seek(seek_p, SeekOrigin.Begin);
                }
                fsp.Write(zero4, 0, 4);
                seek_p += 4;
                fsp.Seek(seek_p, SeekOrigin.Begin);
                pos_all = new List<string>();

                //左ルーフリア
                bgWorker.ReportProgress(3);
                List<float> sortlistf = sortlist1.Select(x => x.vertex_y).ToList();
                var sortf3 = sortlistf.OrderBy(n => n).Skip(2).FirstOrDefault();
                sortlist2 = sortlist1.Where(x => x.vertex_y == sortf3).ToList();

                //頂点座標の変換処理
                //X座標
                //浮動小数点を16進数に変換する
                ext = sortlist2.Select(x => x.vertex_x);
                foreach (var floatvertex in ext)
                {
                    dou = floatvertex;
                }
                dainyu = FloatToString(dou);
                pos_all.Add(dainyu);

                //Z座標
                //浮動小数点を16進数に変換する
                ext = sortlist2.Select(x => x.vertex_z);
                foreach (var floatvertex in ext)
                {
                    dou = floatvertex;
                }
                dainyu = FloatToString(dou);
                pos_all.Add(dainyu);

                //Y座標
                //浮動小数点を16進数に変換する
                ext = sortlist2.Select(x => x.vertex_y);
                foreach (var floatvertex in ext)
                {
                    dou = floatvertex;
                }
                dainyu = FloatToString(dou);
                pos_all.Add(dainyu);

                for (int m = 0; m < pos_all.Count; m++)
                {
                    //hitdata_new.binに代入する
                    dainyu_byte = StringToBytes(pos_all.ElementAt(m));
                    fsp.Write(dainyu_byte, 0, 4);
                    seek_p += 4;
                    fsp.Seek(seek_p, SeekOrigin.Begin);
                }
                fsp.Write(zero4, 0, 4);
                seek_p += 4;
                fsp.Seek(seek_p, SeekOrigin.Begin);
                pos_all = new List<string>();

                //左ルーフフロント
                bgWorker.ReportProgress(4);
                sortlistf = sortlist1.Select(x => x.vertex_y).ToList();
                sortf3 = sortlistf.OrderByDescending(n => n).Skip(2).FirstOrDefault();
                sortlist2 = sortlist1.Where(x => x.vertex_y == sortf3).ToList();

                //頂点座標の変換処理
                //X座標
                //浮動小数点を16進数に変換する
                ext = sortlist2.Select(x => x.vertex_x);
                foreach (var floatvertex in ext)
                {
                    dou = floatvertex;
                }
                dainyu = FloatToString(dou);
                pos_all.Add(dainyu);

                //Z座標
                //浮動小数点を16進数に変換する
                ext = sortlist2.Select(x => x.vertex_z);
                foreach (var floatvertex in ext)
                {
                    dou = floatvertex;
                }
                dainyu = FloatToString(dou);
                pos_all.Add(dainyu);

                //Y座標
                //浮動小数点を16進数に変換する
                ext = sortlist2.Select(x => x.vertex_y);
                foreach (var floatvertex in ext)
                {
                    dou = floatvertex;
                }
                dainyu = FloatToString(dou);
                pos_all.Add(dainyu);

                for (int m = 0; m < pos_all.Count; m++)
                {
                    //hitdata_new.binに代入する
                    dainyu_byte = StringToBytes(pos_all.ElementAt(m));
                    fsp.Write(dainyu_byte, 0, 4);
                    seek_p += 4;
                    fsp.Seek(seek_p, SeekOrigin.Begin);
                }
                fsp.Write(zero4, 0, 4);
                seek_p += 4;
                fsp.Seek(seek_p, SeekOrigin.Begin);
                pos_all = new List<string>();

                //左フロント_上点
                bgWorker.ReportProgress(5);
                sort2 = sortlist1.Max(x => x.vertex_y);
                sortlist2 = sortlist1.Where(x => x.vertex_y == sort2).ToList();
                sort3 = sortlist2.Max(x => x.vertex_z);
                sortlist3 = sortlist2.Where(x => x.vertex_z == sort3).ToList();

                //頂点座標の変換処理
                //X座標
                //浮動小数点を16進数に変換する
                ext = sortlist3.Select(x => x.vertex_x);
                foreach (var floatvertex in ext)
                {
                    dou = floatvertex;
                }
                dainyu = FloatToString(dou);
                pos_all.Add(dainyu);

                //Z座標
                //浮動小数点を16進数に変換する
                ext = sortlist3.Select(x => x.vertex_z);
                foreach (var floatvertex in ext)
                {
                    dou = floatvertex;
                }
                dainyu = FloatToString(dou);
                pos_all.Add(dainyu);

                //Y座標
                //浮動小数点を16進数に変換する
                ext = sortlist3.Select(x => x.vertex_y);
                foreach (var floatvertex in ext)
                {
                    dou = floatvertex;
                }
                dainyu = FloatToString(dou);
                pos_all.Add(dainyu);

                for (int m = 0; m < pos_all.Count; m++)
                {
                    //hitdata_new.binに代入する
                    dainyu_byte = StringToBytes(pos_all.ElementAt(m));
                    fsp.Write(dainyu_byte, 0, 4);
                    seek_p += 4;
                    fsp.Seek(seek_p, SeekOrigin.Begin);
                }
                fsp.Write(zero4, 0, 4);
                seek_p += 4;
                fsp.Seek(seek_p, SeekOrigin.Begin);
                pos_all = new List<string>();

                //右フロント_低点
                bgWorker.ReportProgress(6);
                sortlistf1_num = vertexlist.Select(x => x.num).ToList();
                sortlistf1_x = vertexlist.Select(x => x.vertex_x).ToList();
                sortlistf1_z = vertexlist.Select(x => x.vertex_z).ToList();
                sortlistf1_y = vertexlist.Select(x => x.vertex_y).ToList();
                sortlist1 = new List<Vertex>();
                for (int m = 0; m < 6; m++)
                {
                    float sort6_x = sortlistf1_x.OrderByDescending(n => n).Skip(m).FirstOrDefault();
                    List<int> sortlisti1 = vertexlist.Where(x => x.vertex_x == sort6_x).Select(x => x.num).ToList();
                    foreach (int item in sortlisti1)
                    {
                        int sort6_num = sortlistf1_num.Skip(item).FirstOrDefault();
                        float sort6_z = sortlistf1_z.Skip(item).FirstOrDefault();
                        float sort6_y = sortlistf1_y.Skip(item).FirstOrDefault();
                        sortlist1.Add(new Vertex(sort6_num, sort6_x, sort6_z, sort6_y));
                    }
                    if (sortlist1.Count() >= 6)
                        break;
                }
                sort2 = sortlist1.Max(x => x.vertex_y);
                sortlist2 = sortlist1.Where(x => x.vertex_y == sort2).ToList();
                sort3 = sortlist2.Min(x => x.vertex_z);
                sortlist3 = sortlist2.Where(x => x.vertex_z == sort3).ToList();

                //頂点座標の変換処理
                //X座標
                //浮動小数点を16進数に変換する
                ext = sortlist3.Select(x => x.vertex_x);
                foreach (var floatvertex in ext)
                {
                    dou = floatvertex;
                }
                dainyu = FloatToString(dou);
                pos_all.Add(dainyu);

                //Z座標
                //浮動小数点を16進数に変換する
                ext = sortlist3.Select(x => x.vertex_z);
                foreach (var floatvertex in ext)
                {
                    dou = floatvertex;
                }
                dainyu = FloatToString(dou);
                pos_all.Add(dainyu);

                //Y座標
                //浮動小数点を16進数に変換する
                ext = sortlist3.Select(x => x.vertex_y);
                foreach (var floatvertex in ext)
                {
                    dou = floatvertex;
                }
                dainyu = FloatToString(dou);
                pos_all.Add(dainyu);

                for (int m = 0; m < pos_all.Count; m++)
                {
                    //hitdata_new.binに代入する
                    dainyu_byte = StringToBytes(pos_all.ElementAt(m));
                    fsp.Write(dainyu_byte, 0, 4);
                    seek_p += 4;
                    fsp.Seek(seek_p, SeekOrigin.Begin);
                }
                fsp.Write(zero4, 0, 4);
                seek_p += 4;
                fsp.Seek(seek_p, SeekOrigin.Begin);
                pos_all = new List<string>();

                //右リア_低点
                bgWorker.ReportProgress(7);
                sort2 = sortlist1.Min(x => x.vertex_y);
                sortlist2 = sortlist1.Where(x => x.vertex_y == sort2).ToList();
                sort3 = sortlist2.Min(x => x.vertex_z);
                sortlist3 = sortlist2.Where(x => x.vertex_z == sort3).ToList();

                //頂点座標の変換処理
                //X座標
                //浮動小数点を16進数に変換する
                ext = sortlist3.Select(x => x.vertex_x);
                foreach (var floatvertex in ext)
                {
                    dou = floatvertex;
                }
                dainyu = FloatToString(dou);
                pos_all.Add(dainyu);

                //Z座標
                //浮動小数点を16進数に変換する
                ext = sortlist3.Select(x => x.vertex_z);
                foreach (var floatvertex in ext)
                {
                    dou = floatvertex;
                }
                dainyu = FloatToString(dou);
                pos_all.Add(dainyu);

                //Y座標
                //浮動小数点を16進数に変換する
                ext = sortlist3.Select(x => x.vertex_y);
                foreach (var floatvertex in ext)
                {
                    dou = floatvertex;
                }
                dainyu = FloatToString(dou);
                pos_all.Add(dainyu);

                for (int m = 0; m < pos_all.Count; m++)
                {
                    //hitdata_new.binに代入する
                    dainyu_byte = StringToBytes(pos_all.ElementAt(m));
                    fsp.Write(dainyu_byte, 0, 4);
                    seek_p += 4;
                    fsp.Seek(seek_p, SeekOrigin.Begin);
                }
                fsp.Write(zero4, 0, 4);
                seek_p += 4;
                fsp.Seek(seek_p, SeekOrigin.Begin);
                pos_all = new List<string>();

                //右リア_上点
                bgWorker.ReportProgress(8);
                sort2 = sortlist1.Min(x => x.vertex_y);
                sortlist2 = sortlist1.Where(x => x.vertex_y == sort2).ToList();
                sort3 = sortlist2.Max(x => x.vertex_z);
                sortlist3 = sortlist2.Where(x => x.vertex_z == sort3).ToList();

                //頂点座標の変換処理
                //X座標
                //浮動小数点を16進数に変換する
                ext = sortlist3.Select(x => x.vertex_x);
                foreach (var floatvertex in ext)
                {
                    dou = floatvertex;
                }
                dainyu = FloatToString(dou);
                pos_all.Add(dainyu);

                //Z座標
                //浮動小数点を16進数に変換する
                ext = sortlist3.Select(x => x.vertex_z);
                foreach (var floatvertex in ext)
                {
                    dou = floatvertex;
                }
                dainyu = FloatToString(dou);
                pos_all.Add(dainyu);

                //Y座標
                //浮動小数点を16進数に変換する
                ext = sortlist3.Select(x => x.vertex_y);
                foreach (var floatvertex in ext)
                {
                    dou = floatvertex;
                }
                dainyu = FloatToString(dou);
                pos_all.Add(dainyu);

                for (int m = 0; m < pos_all.Count; m++)
                {
                    //hitdata_new.binに代入する
                    dainyu_byte = StringToBytes(pos_all.ElementAt(m));
                    fsp.Write(dainyu_byte, 0, 4);
                    seek_p += 4;
                    fsp.Seek(seek_p, SeekOrigin.Begin);
                }
                fsp.Write(zero4, 0, 4);
                seek_p += 4;
                fsp.Seek(seek_p, SeekOrigin.Begin);
                pos_all = new List<string>();

                //右ルーフリア
                bgWorker.ReportProgress(9);
                sortlistf = sortlist1.Select(x => x.vertex_y).ToList();
                sortf3 = sortlistf.OrderBy(n => n).Skip(2).FirstOrDefault();
                sortlist2 = sortlist1.Where(x => x.vertex_y == sortf3).ToList();

                //頂点座標の変換処理
                //X座標
                //浮動小数点を16進数に変換する
                ext = sortlist2.Select(x => x.vertex_x);
                foreach (var floatvertex in ext)
                {
                    dou = floatvertex;
                }
                dainyu = FloatToString(dou);
                pos_all.Add(dainyu);

                //Z座標
                //浮動小数点を16進数に変換する
                ext = sortlist2.Select(x => x.vertex_z);
                foreach (var floatvertex in ext)
                {
                    dou = floatvertex;
                }
                dainyu = FloatToString(dou);
                pos_all.Add(dainyu);

                //Y座標
                //浮動小数点を16進数に変換する
                ext = sortlist2.Select(x => x.vertex_y);
                foreach (var floatvertex in ext)
                {
                    dou = floatvertex;
                }
                dainyu = FloatToString(dou);
                pos_all.Add(dainyu);

                for (int m = 0; m < pos_all.Count; m++)
                {
                    //hitdata_new.binに代入する
                    dainyu_byte = StringToBytes(pos_all.ElementAt(m));
                    fsp.Write(dainyu_byte, 0, 4);
                    seek_p += 4;
                    fsp.Seek(seek_p, SeekOrigin.Begin);
                }
                fsp.Write(zero4, 0, 4);
                seek_p += 4;
                fsp.Seek(seek_p, SeekOrigin.Begin);
                pos_all = new List<string>();

                //右ルーフフロント
                bgWorker.ReportProgress(10);
                sortlistf = sortlist1.Select(x => x.vertex_y).ToList();
                sortf3 = sortlistf.OrderByDescending(n => n).Skip(2).FirstOrDefault();
                sortlist2 = sortlist1.Where(x => x.vertex_y == sortf3).ToList();

                //頂点座標の変換処理
                //X座標
                //浮動小数点を16進数に変換する
                ext = sortlist2.Select(x => x.vertex_x);
                foreach (var floatvertex in ext)
                {
                    dou = floatvertex;
                }
                dainyu = FloatToString(dou);
                pos_all.Add(dainyu);

                //Z座標
                //浮動小数点を16進数に変換する
                ext = sortlist2.Select(x => x.vertex_z);
                foreach (var floatvertex in ext)
                {
                    dou = floatvertex;
                }
                dainyu = FloatToString(dou);
                pos_all.Add(dainyu);

                //Y座標
                //浮動小数点を16進数に変換する
                ext = sortlist2.Select(x => x.vertex_y);
                foreach (var floatvertex in ext)
                {
                    dou = floatvertex;
                }
                dainyu = FloatToString(dou);
                pos_all.Add(dainyu);

                for (int m = 0; m < pos_all.Count; m++)
                {
                    //hitdata_new.binに代入する
                    dainyu_byte = StringToBytes(pos_all.ElementAt(m));
                    fsp.Write(dainyu_byte, 0, 4);
                    seek_p += 4;
                    fsp.Seek(seek_p, SeekOrigin.Begin);
                }
                fsp.Write(zero4, 0, 4);
                seek_p += 4;
                fsp.Seek(seek_p, SeekOrigin.Begin);
                pos_all = new List<string>();

                //右フロント_上点
                bgWorker.ReportProgress(11);
                sort2 = sortlist1.Max(x => x.vertex_y);
                sortlist2 = sortlist1.Where(x => x.vertex_y == sort2).ToList();
                sort3 = sortlist2.Max(x => x.vertex_z);
                sortlist3 = sortlist2.Where(x => x.vertex_z == sort3).ToList();

                //頂点座標の変換処理
                //X座標
                //浮動小数点を16進数に変換する
                ext = sortlist3.Select(x => x.vertex_x);
                foreach (var floatvertex in ext)
                {
                    dou = floatvertex;
                }
                dainyu = FloatToString(dou);
                pos_all.Add(dainyu);

                //Z座標
                //浮動小数点を16進数に変換する
                ext = sortlist3.Select(x => x.vertex_z);
                foreach (var floatvertex in ext)
                {
                    dou = floatvertex;
                }
                dainyu = FloatToString(dou);
                pos_all.Add(dainyu);

                //Y座標
                //浮動小数点を16進数に変換する
                ext = sortlist3.Select(x => x.vertex_y);
                foreach (var floatvertex in ext)
                {
                    dou = floatvertex;
                }
                dainyu = FloatToString(dou);
                pos_all.Add(dainyu);

                for (int m = 0; m < pos_all.Count; m++)
                {
                    //hitdata_new.binに代入する
                    dainyu_byte = StringToBytes(pos_all.ElementAt(m));
                    fsp.Write(dainyu_byte, 0, 4);
                    seek_p += 4;
                    fsp.Seek(seek_p, SeekOrigin.Begin);
                }
                fsp.Write(kokomate, 0, 4);
                seek_p += 4;
                fsp.Seek(seek_p, SeekOrigin.Begin);
                pos_all = new List<string>();
                bgWorker.ReportProgress(12);

                fsp.Close();

                goto labelfinish;

            label2:;
                // ドラッグ＆ドロップされたhitdataファイル
                FileStream fsr2 = new FileStream(path[b].ToString(), FileMode.Open, FileAccess.Read);
                byte[] bs = new byte[fsr2.Length];
                fsr2.Read(bs, 0, bs.Length);

                StreamWriter sw = new StreamWriter(filedirectory + @"\hitdata_ext.obj", false);
                sw.WriteLine("# hitdata");

                for (int m = 0; m < 12; m++)
                {
                    sw.Write("v ");
                    for (int n = 0; n < 3; n++)
                    {
                        string vertexfloat16 = Getbytestr4l(bs, m * 16 + n * 4);
                        uint vertexint = Convert.ToUInt32(vertexfloat16, 16);
                        float vertexfloat = BitConverter.ToSingle(BitConverter.GetBytes(vertexint), 0);
                        string vertexfloats = vertexfloat.ToString("G8");
                        //MessageBox.Show(vertexfloats);
                        sw.Write(vertexfloats);
                        if (n != 2)
                            sw.Write(" ");
                    }
                    if (m != 11)
                        sw.WriteLine("");
                }
                sw.Close();

            labelfinish:;
                if (keikoku == true)
                    MessageBox.Show("頂点の数が12個じゃないので変換できませんでした！\nフロント低点と上点\nリア低点と上点\n" +
                        "ルーフリアとフロント\nそれぞれ左右セットで合計12個の頂点が必要です");
            }
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

        //byte配列4バイトをstringに変換して戻す(リトルエンディアン用)
        public static string Getbytestr4l(byte[] bytes, int seek)
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

            string returnstr = str4 + str3 + str2 + str1;

            return returnstr;
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

public class Vertex
{
    int number;
    float float_x, float_z, float_y;
    public Vertex(int _num, float _x, float _z, float _y)
    {
        number = _num;
        float_x = _x;
        float_z = _z;
        float_y = _y;
    }
    public int num
    {
        get { return number; }
    }
    public float vertex_x
    {
        get { return float_x; }
    }
    public float vertex_z
    {
        get { return float_z; }
    }

    public float vertex_y
    {
        get { return float_y; }
    }
}