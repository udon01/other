using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 頭文字Dエクストリームステージ_car_info系ファイル操作ツール
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
        public static byte[] byte1 = new byte[1];
        public static byte[] byte2 = new byte[2];
        public static byte[] byte4 = new byte[4];
        public static int sousuu = 0;
        public static int bs_seek = 0;
        public static int str_seek = 0;

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

                if (Path.GetExtension(path[b]).ToLower() == ".txt")
                    goto label_to_pack;

                filecount = path.Count() - 1;

                //car_info系ファイルをテキストファイルに変換する
                FileStream fsr_out_txt = new FileStream(path[b], FileMode.Open, FileAccess.Read);
                byte[] bs_pack = new byte[fsr_out_txt.Length];
                fsr_out_txt.Read(bs_pack, 0, bs_pack.Length);
                fsr_out_txt.Close();

                string unpackfile_path = Path.GetDirectoryName(path[b]) + @"\" + Path.GetFileName(path[b]) + @".txt";
                bs_seek = 0;
                string info_text = "";
                sousuu = 0;

                string car_color_info_text = "//1行目...総カラー数(再変換する時は自動でカウントされます！)\r\n//2行目以降...カラー名称,カラーモデルファイル名,フラグ,ライバル仕様名称1,ライバルアイコンID1,ライバル仕様名称2,ライバルアイコンID2,塗装値段,ベースカラー[RGBA],ハイライトカラー[RGBA],ツートンカラー[RGB]\r\n//ライバルアイコンIDは不使用の場合-1がセットされています\r\n//レコードの終端にセミコロン(;)が付いています。\r\n";
                string car_parameter_info_text = "//トレッド前,トレッド後,ホイールベース前,ホイールベース後,ボディX前,ボディX後,ボディZ前,ボディZ後,タイヤ外径前,タイヤ外径後,サスストローク fmax/fmin/rmax/rmin,車種特性フラグ\r\n//レコードの終端にセミコロン(;)が付いています。\r\n";
                string car_parts_info_text = "//1行目...総パーツ数(再変換する時は自動でカウントされます！)\r\n//2行目以降...パーツID,パーツ名称,モデルファイル名,値段,パーツメーカーID,ロケーター内包フラグ\r\n//レコードの終端にセミコロン(;)が付いています。\r\n";
                string car_partsset_info_text = "//1行目...総パーツセット数(再変換する時は自動でカウントされます！)\r\n//2行目以降...パーツセットID,パーツセット名称,値段,パーツメーカーID,パーツセット配列数,パーツセット配列\r\n//レコードの終端にセミコロン(;)が付いています。\r\n";
                string car_tuning_info_text = "//1行目...総チューニング要素ファイル数(再変換する時は自動でカウントされます！)\r\n//2行目以降...チューニングレベルID,チューニングレベルサブID,チューニングポイント,チューニングタイトル,チューニング説明,チューニング質問,チューン換装後メッセージ,値段,チューニングレベル,チューニングモデルID,チューニングタイトル略称,ライバル仕様名称,アクロバータ顔2D呼び出しID,carSpec仕様変更フラグ(ターボ・スーチャー・改造タコメーター装着)\r\n//以降繰り返し\r\n//レコードの終端にセミコロン(;)が付いています。\r\n";
                string shop_parts_info_text = "//1行目...総ショップパーツ数(再変換する時は自動でカウントされます！)\r\n//2行目以降...パーツID,ショップサブメニューID,値段,ライバル仕様名称1,ライバルアイコンID1,ライバル仕様名称2,ライバルアイコンID2\r\n//ライバルアイコンIDは不使用の場合-1がセットされています\r\n//レコードの終端にセミコロン(;)が付いています。\r\n";
                string car_maker_info_text = "//1行目...総メーカー数(再変換する時は自動でカウントされます！)\r\n//2行目以降...メーカー名称,メーカー略称\r\n//レコードの終端にセミコロン(;)が付いています。\r\n";
                string parts_maker_info_text = "//1行目...総メーカー数(再変換する時は自動でカウントされます！)\r\n//2行目以降...フラグ,メーカー名,メーカー略称,アクロバータ2D呼び出しID\r\n//レコードの終端にセミコロン(;)が付いています。\r\n";
                string makername_car_info_text = "//1行目...総車種数(再変換する時は自動でカウントされます！)\r\n//2行目以降...車ID,名称,略称,値段,封印カーフラグ(DLC)\r\n//レコードの終端にセミコロン(;)が付いています。\r\n";

                string filename_bin = Path.GetFileName(path[b]);
                if (filename_bin.IndexOf("car_color_info") >= 0)
                {
                    info_text = car_color_info_text;
                    goto label_car_color_info_to_txt;
                }
                else if (filename_bin.IndexOf("car_parameter_info") >= 0)
                {
                    info_text = car_parameter_info_text;
                    goto label_car_parameter_info_to_txt;
                }
                else if (filename_bin.IndexOf("car_parts_info") >= 0)
                {
                    info_text = car_parts_info_text;
                    goto label_car_parts_info_to_txt;
                }
                else if (filename_bin.IndexOf("car_partsset_info") >= 0)
                {
                    info_text = car_partsset_info_text;
                    goto label_car_partsset_info_to_txt;
                }
                else if (filename_bin.IndexOf("car_tuning_info") >= 0)
                {
                    info_text = car_tuning_info_text;
                    goto label_car_tuning_info_to_txt;
                }
                else if (filename_bin.IndexOf("shop_parts_info") >= 0)
                {
                    info_text = shop_parts_info_text;
                    goto label_shop_parts_info_to_txt;
                }
                else if (filename_bin.IndexOf("car_maker_info") >= 0)
                {
                    info_text = car_maker_info_text;
                    goto label_car_maker_info_to_txt;
                }
                else if (filename_bin.IndexOf("parts_maker_info") >= 0)
                {
                    info_text = parts_maker_info_text;
                    goto label_parts_maker_info_to_txt;
                }
                else if (filename_bin.IndexOf("_car_info") >= 0)
                {
                    info_text = makername_car_info_text;
                    goto label_makername_car_info_to_txt;
                }


            label_car_color_info_to_txt:;

                //総カラー数
                Array.Copy(bs_pack, bs_seek, byte1, 0, 1);
                sousuu = byte1[0];
                info_text = info_text + sousuu.ToString() + ",\r\n";
                bs_seek += 1;

                for (int i = 0 ; i < sousuu; i++)
                {
                    bs_seek += 1;

                    //カラー名称
                    info_text = jis_copy_to_str(bs_pack, info_text);

                    //カラーモデルファイル名
                    info_text = jis_copy_to_str(bs_pack, info_text);

                    //フラグ
                    info_text = byte1_copy_to_str(bs_pack, info_text);

                    //ライバル仕様名称1
                    info_text = jis_copy_to_str(bs_pack, info_text);

                    //ライバルアイコンID1
                    info_text = jis_copy_to_str(bs_pack, info_text);

                    //ライバル仕様名称2
                    info_text = jis_copy_to_str(bs_pack, info_text);

                    //ライバルアイコンID2
                    info_text = jis_copy_to_str(bs_pack, info_text);

                    //塗装値段
                    info_text = int4_copy_to_str(bs_pack, info_text);

                    //ベースカラー[RGBA]
                    for (int n = 0; n < 4; n++)
                    {
                        info_text = byte1_copy_to_str(bs_pack, info_text);
                    }

                    //ハイライトカラー[RGBA]
                    for (int n = 0; n < 4; n++)
                    {
                        info_text = byte1_copy_to_str(bs_pack, info_text);
                    }

                    //ツートンカラー[RGB]
                    for (int n = 0; n < 3; n++)
                    {
                        info_text = byte1_copy_to_str(bs_pack, info_text);
                    }

                    info_text = info_text + ";";
                    if (i + 1 < sousuu)
                        info_text = info_text + "\r\n";
                }
                goto label_output_txt;


            label_car_parameter_info_to_txt:;

                //トレッド前,トレッド後,ホイールベース前,ホイールベース後,ボディX前,ボディX後,ボディZ前,ボディZ後,タイヤ外径前,タイヤ外径後,サスストローク fmax,fmin,rmax,rmin
                for (int i = 0; i < 14; i++)
                {
                    info_text = float_copy_to_str(bs_pack, info_text);
                }

                //車種特性フラグ
                info_text = int4_copy_to_str(bs_pack, info_text);

                info_text = info_text + ";";
                goto label_output_txt;


            label_car_parts_info_to_txt:;

                //総パーツ数
                Array.Copy(bs_pack, bs_seek, byte1, 0, 1);
                sousuu = byte1[0];
                info_text = info_text + sousuu.ToString() + ",\r\n";
                bs_seek += 1;

                for (int i = 0; i < sousuu; i++)
                {
                    //パーツID
                    info_text = int2_copy_to_str(bs_pack, info_text);

                    //パーツ名称
                    info_text = jis_copy_to_str(bs_pack, info_text);

                    //モデルファイル名
                    info_text = jis_copy_to_str(bs_pack, info_text);

                    //値段
                    info_text = int4_copy_to_str(bs_pack, info_text);

                    //パーツメーカーID
                    info_text = int2_copy_to_str(bs_pack, info_text);

                    //ロケーター内包フラグ
                    info_text = int4_copy_to_str(bs_pack, info_text);

                    info_text = info_text + ";";
                    if (i + 1 < sousuu)
                        info_text = info_text + "\r\n";
                }
                goto label_output_txt;


            label_car_partsset_info_to_txt:;

                //総パーツセット数
                Array.Copy(bs_pack, bs_seek, byte1, 0, 1);
                sousuu = byte1[0];
                info_text = info_text + sousuu.ToString() + ",\r\n";
                bs_seek += 1;

                for (int i = 0; i < sousuu; i++)
                {
                    //パーツセットID
                    info_text = int2_copy_to_str(bs_pack, info_text);

                    //パーツセット名称
                    info_text = jis_copy_to_str(bs_pack, info_text);

                    //値段
                    info_text = int4_copy_to_str(bs_pack, info_text);

                    //パーツメーカーID
                    info_text = byte1_copy_to_str(bs_pack, info_text);

                    //パーツセット配列数
                    Array.Copy(bs_pack, 0, byte1, 0, 1);
                    info_text = byte1_copy_to_str(bs_pack, info_text);

                    //パーツセット配列
                    for (int n = 0; n < byte1[0]; n++)
                    {
                        info_text = int2_copy_to_str(bs_pack, info_text);
                    }

                    info_text = info_text + ";";
                    if (i + 1 < sousuu)
                        info_text = info_text + "\r\n";
                }
                goto label_output_txt;


            label_car_tuning_info_to_txt:;

                for (int i = 0; i < 5; i++)
                {
                    //総チューニング要素ファイル数
                    Array.Copy(bs_pack, bs_seek, byte1, 0, 1);
                    sousuu = byte1[0];
                    info_text = info_text + sousuu.ToString() + ",\r\n";
                    bs_seek += 1;

                    for (int n = 0; n < sousuu; n++)
                    {
                        //チューニングレベルID
                        info_text = byte1_copy_to_str(bs_pack, info_text);

                        //チューニングレベルサブID
                        info_text = byte1_copy_to_str(bs_pack, info_text);

                        //チューニングポイント
                        info_text = byte1_copy_to_str(bs_pack, info_text);

                        //チューニングタイトル
                        info_text = jis_copy_to_str(bs_pack, info_text);

                        //チューニング説明
                        info_text = jis_copy_to_str(bs_pack, info_text);

                        //チューニング質問
                        info_text = jis_copy_to_str(bs_pack, info_text);

                        //チューン換装後メッセージ
                        info_text = jis_copy_to_str(bs_pack, info_text);

                        //値段
                        info_text = int2_copy_to_str(bs_pack, info_text);

                        //チューニングレベル
                        info_text = jis_copy_to_str(bs_pack, info_text);

                        //チューニングモデルID
                        info_text = int2_copy_to_str(bs_pack, info_text);

                        //チューニングタイトル略称
                        info_text = jis_copy_to_str(bs_pack, info_text);

                        //ライバル仕様名称
                        info_text = jis_copy_to_str(bs_pack, info_text);

                        //アクロバータ顔2D呼び出しID
                        info_text = short2_copy_to_str(bs_pack, info_text);

                        //carSpec仕様変更フラグ(ターボ・スーチャー・改造タコメーター装着)
                        info_text = int4_copy_to_str(bs_pack, info_text);

                        info_text = info_text + ";";
                        if (n + 1 < sousuu)
                            info_text = info_text + "\r\n";
                    }
                    if (i + 1 < 5)
                        info_text = info_text + "\r\n";
                }
                goto label_output_txt;


            label_shop_parts_info_to_txt:;

                //総ショップパーツ数
                Array.Copy(bs_pack, bs_seek, byte1, 0, 1);
                sousuu = byte1[0];
                info_text = info_text + sousuu.ToString() + ",\r\n";
                bs_seek += 1;

                for (int i = 0; i < sousuu; i++)
                {
                    //パーツID
                    info_text = int2_copy_to_str(bs_pack, info_text);

                    //ショップサブメニューID
                    info_text = byte1_copy_to_str(bs_pack, info_text);

                    //値段
                    info_text = int4_copy_to_str(bs_pack, info_text);

                    //ライバル仕様名称1
                    info_text = jis_copy_to_str(bs_pack, info_text);

                    //ライバルアイコンID1
                    info_text = sbyte1_copy_to_str(bs_pack, info_text);

                    //ライバル仕様名称2
                    info_text = jis_copy_to_str(bs_pack, info_text);

                    //ライバルアイコンID2
                    info_text = sbyte1_copy_to_str(bs_pack, info_text);

                    info_text = info_text + ";";
                    if (i + 1 < sousuu)
                        info_text = info_text + "\r\n";
                }
                goto label_output_txt;


            label_car_maker_info_to_txt:;

                //総メーカー数
                Array.Copy(bs_pack, bs_seek, byte1, 0, 1);
                sousuu = byte1[0];
                info_text = info_text + sousuu.ToString() + ",\r\n";
                bs_seek += 1;

                for (int i = 0; i < sousuu; i++)
                {
                    //メーカー名称
                    info_text = jis_copy_to_str(bs_pack, info_text);

                    //メーカー略称
                    info_text = jis_copy_to_str(bs_pack, info_text);

                    info_text = info_text + ";";
                    if (i + 1 < sousuu)
                        info_text = info_text + "\r\n";
                }
                goto label_output_txt;


            label_parts_maker_info_to_txt:;

                //総パーツメーカー数
                Array.Copy(bs_pack, bs_seek, byte1, 0, 1);
                sousuu = byte1[0];
                info_text = info_text + sousuu.ToString() + ",\r\n";
                bs_seek += 1;

                for (int i = 0; i < sousuu; i++)
                {
                    //フラグ
                    info_text = sbyte1_copy_to_str(bs_pack, info_text);

                    //メーカー名
                    info_text = jis_copy_to_str(bs_pack, info_text);

                    //メーカー略称
                    info_text = jis_copy_to_str(bs_pack, info_text);

                    //アクロバータ2D呼び出しID
                    info_text = sbyte1_copy_to_str(bs_pack, info_text);

                    info_text = info_text + ";";
                    if (i + 1 < sousuu)
                        info_text = info_text + "\r\n";
                }
                goto label_output_txt;


            label_makername_car_info_to_txt:;

                //総車種数
                Array.Copy(bs_pack, bs_seek, byte1, 0, 1);
                sousuu = byte1[0];
                info_text = info_text + sousuu.ToString() + ",\r\n";
                bs_seek += 1;

                for (int i = 0; i < sousuu; i++)
                {
                    //車ID
                    info_text = int2_copy_to_str(bs_pack, info_text);

                    //名称
                    info_text = jis_copy_to_str(bs_pack, info_text);

                    //略称
                    info_text = jis_copy_to_str(bs_pack, info_text);

                    //封印カーフラグ(DLC)
                    info_text = byte1_copy_to_str(bs_pack, info_text);

                    info_text = info_text + ";";
                    if (i + 1 < sousuu)
                        info_text = info_text + "\r\n";
                }
                goto label_output_txt;


            label_output_txt:;
                string output_txt_path = Path.GetDirectoryName(path[b]) + @"\" + Path.GetFileNameWithoutExtension(path[b]) + ".txt";
                StreamWriter sw = new StreamWriter(output_txt_path);
                sw.Write(info_text);
                sw.Close();

                a += 1;
                bgWorker.ReportProgress(a);
                goto labelfinish;

                
            label_to_pack:;

                //テキストファイルをcar_info系ファイルに変換する
                byte[] bs_pack_new = new byte[0];
                string filename_txt = Path.GetFileName(path[b]);
                StreamReader sr_rp = new StreamReader(filename_txt);
                List<string> lines = new List<string> { };
                while (!sr_rp.EndOfStream)
                {
                    string line = sr_rp.ReadLine();
                    if (line.Substring(0, 2) != "//")
                        lines.Add(line);
                }

                sousuu = 0;

                if (filename_txt.IndexOf("car_color_info") >= 0)
                    goto label_car_color_info_to_bin;
                else if (filename_txt.IndexOf("car_parameter_info") >= 0)
                    goto label_car_parameter_info_to_bin;
                else if (filename_txt.IndexOf("car_parts_info") >= 0)
                    goto label_car_parts_info_to_bin;
                else if (filename_txt.IndexOf("car_partsset_info") >= 0)
                    goto label_car_partsset_info_to_bin;
                else if (filename_txt.IndexOf("car_tuning_info") >= 0)
                    goto label_car_tuning_info_to_bin;
                else if (filename_txt.IndexOf("shop_parts_info") >= 0)
                    goto label_shop_parts_info_to_bin;
                else if (filename_txt.IndexOf("car_maker_info") >= 0)
                    goto label_car_maker_info_to_bin;
                else if (filename_txt.IndexOf("parts_maker_info") >= 0)
                    goto label_parts_maker_info_to_bin;
                else if (filename_txt.IndexOf("_car_info") >= 0)
                    goto label_makername_car_info_to_bin;


                label_car_color_info_to_bin:;
                for (int i = 0; i < lines.Count(); i++)
                {
                    if (CountOf(lines[i], ",") != 1)
                    {
                        sousuu += 1;
                        str_seek = 0;
                        Array.Resize(ref bs_pack_new, bs_pack_new.Length + 1);

                        //カラー名称
                        bs_pack_new = jis_copy_to_bytes(lines[i], bs_pack_new);

                        //カラーモデルファイル名
                        bs_pack_new = jis_copy_to_bytes(lines[i], bs_pack_new);

                        //フラグ
                        bs_pack_new = byte1_copy_to_bytes(lines[i], bs_pack_new);

                        //ライバル仕様名称1
                        bs_pack_new = jis_copy_to_bytes(lines[i], bs_pack_new);

                        //ライバルアイコンID1
                        bs_pack_new = jis_copy_to_bytes(lines[i], bs_pack_new);

                        //ライバル仕様名称2
                        bs_pack_new = jis_copy_to_bytes(lines[i], bs_pack_new);

                        //ライバルアイコンID2
                        bs_pack_new = jis_copy_to_bytes(lines[i], bs_pack_new);

                        //塗装値段
                        bs_pack_new = int4_copy_to_bytes(lines[i], bs_pack_new);

                        //ベースカラー[RGBA]
                        for (int n = 0; n < 4; n++)
                        {
                            bs_pack_new = byte1_copy_to_bytes(lines[i], bs_pack_new);
                        }

                        //ハイライトカラー[RGBA]
                        for (int n = 0; n < 4; n++)
                        {
                            bs_pack_new = byte1_copy_to_bytes(lines[i], bs_pack_new);
                        }

                        //ツートンカラー[RGB]
                        for (int n = 0; n < 3; n++)
                        {
                            bs_pack_new = byte1_copy_to_bytes(lines[i], bs_pack_new);
                        }
                    }
                }
                byte1 = BitConverter.GetBytes(sousuu);
                bs_pack_new = bytes_copy_to_bytes_sentou(bs_pack_new, byte1);

                goto label_output_bin;


            label_car_parameter_info_to_bin:;
                for (int i = 0; i < lines.Count(); i++)
                {
                    str_seek = 0;

                    //トレッド前,トレッド後,ホイールベース前,ホイールベース後,ボディX前,ボディX後,ボディZ前,ボディZ後,タイヤ外径前,タイヤ外径後,サスストローク fmax,fmin,rmax,rmin
                    for (int n = 0; n < 14; n++)
                    {
                        bs_pack_new = float_copy_to_bytes(lines[i], bs_pack_new);
                    }

                    //車種特性フラグ
                    bs_pack_new = int4_copy_to_bytes(lines[i], bs_pack_new);
                }

                goto label_output_bin;


            label_car_parts_info_to_bin:;
                for (int i = 0; i < lines.Count(); i++)
                {
                    if (CountOf(lines[i], ",") != 1)
                    {
                        sousuu += 1;
                        str_seek = 0;

                        //パーツID
                        bs_pack_new = int2_copy_to_bytes(lines[i], bs_pack_new);

                        //パーツ名称
                        bs_pack_new = jis_copy_to_bytes(lines[i], bs_pack_new);

                        //モデルファイル名
                        bs_pack_new = jis_copy_to_bytes(lines[i], bs_pack_new);

                        //値段
                        bs_pack_new = int4_copy_to_bytes(lines[i], bs_pack_new);

                        //パーツメーカーID
                        bs_pack_new = int2_copy_to_bytes(lines[i], bs_pack_new);

                        //ロケーター内包フラグ
                        bs_pack_new = int4_copy_to_bytes(lines[i], bs_pack_new);
                    }
                }
                byte1 = BitConverter.GetBytes(sousuu);
                bs_pack_new = bytes_copy_to_bytes_sentou(bs_pack_new, byte1);

                goto label_output_bin;


            label_car_partsset_info_to_bin:;
                for (int i = 0; i < lines.Count(); i++)
                {
                    if (CountOf(lines[i], ",") != 1)
                    {
                        sousuu += 1;
                        str_seek = 0;

                        //パーツセットID
                        bs_pack_new = int2_copy_to_bytes(lines[i], bs_pack_new);

                        //パーツセット名称
                        bs_pack_new = jis_copy_to_bytes(lines[i], bs_pack_new);

                        //値段
                        bs_pack_new = int4_copy_to_bytes(lines[i], bs_pack_new);

                        //パーツメーカーID
                        bs_pack_new = byte1_copy_to_bytes(lines[i], bs_pack_new);

                        //パーツセット配列数
                        bs_pack_new = byte1_copy_to_bytes(lines[i], bs_pack_new);
                        Array.Copy(bs_pack_new, bs_pack_new.Length - 1, byte1, 0, 1);

                        //パーツセット配列
                        for (int n = 0; n < byte1[0]; n++)
                        {
                            bs_pack_new = int2_copy_to_bytes(lines[i], bs_pack_new);
                        }
                    }
                }
                byte1 = BitConverter.GetBytes(sousuu);
                bs_pack_new = bytes_copy_to_bytes_sentou(bs_pack_new, byte1);

                goto label_output_bin;


            label_car_tuning_info_to_bin:;
                byte[] bs_pack_new_cti = new byte[0];
                for (int i = 0; i < lines.Count(); i++)
                {
                    if (CountOf(lines[i], ",") != 1)
                    {
                        sousuu += 1;
                        str_seek = 0;

                        //チューニングレベルID
                        bs_pack_new_cti = byte1_copy_to_bytes(lines[i], bs_pack_new_cti);

                        //チューニングレベルサブID
                        bs_pack_new_cti = byte1_copy_to_bytes(lines[i], bs_pack_new_cti);

                        //チューニングポイント
                        bs_pack_new_cti = byte1_copy_to_bytes(lines[i], bs_pack_new_cti);

                        //チューニングタイトル
                        bs_pack_new_cti = jis_copy_to_bytes(lines[i], bs_pack_new_cti);

                        //チューニング説明
                        bs_pack_new_cti = jis_copy_to_bytes(lines[i], bs_pack_new_cti);

                        //チューニング質問
                        bs_pack_new_cti = jis_copy_to_bytes(lines[i], bs_pack_new_cti);

                        //チューン換装後メッセージ
                        bs_pack_new_cti = jis_copy_to_bytes(lines[i], bs_pack_new_cti);

                        //値段
                        bs_pack_new_cti = int2_copy_to_bytes(lines[i], bs_pack_new_cti);

                        //チューニングレベル
                        bs_pack_new_cti = jis_copy_to_bytes(lines[i], bs_pack_new_cti);

                        //チューニングモデルID
                        bs_pack_new_cti = int2_copy_to_bytes(lines[i], bs_pack_new_cti);

                        //チューニングタイトル略称
                        bs_pack_new_cti = jis_copy_to_bytes(lines[i], bs_pack_new_cti);

                        //ライバル仕様名称
                        bs_pack_new_cti = jis_copy_to_bytes(lines[i], bs_pack_new_cti);

                        //アクロバータ顔2D呼び出しID
                        bs_pack_new_cti = short2_copy_to_bytes(lines[i], bs_pack_new_cti);

                        //carSpec仕様変更フラグ(ターボ・スーチャー・改造タコメーター装着)
                        bs_pack_new_cti = int4_copy_to_bytes(lines[i], bs_pack_new_cti);
                    }
                    else
                    {
                        if (i != 0)
                        {
                            byte1 = BitConverter.GetBytes(sousuu);
                            bs_pack_new_cti = bytes_copy_to_bytes_sentou(bs_pack_new_cti, byte1);
                            Array.Resize(ref bs_pack_new, bs_pack_new.Length + bs_pack_new_cti.Length);
                            Array.Copy(bs_pack_new_cti, 0, bs_pack_new, bs_pack_new.Length - bs_pack_new_cti.Length, bs_pack_new_cti.Length);
                            bs_pack_new_cti = new byte[0];
                            sousuu = 0;
                        }
                    }
                }
                byte1 = BitConverter.GetBytes(sousuu);
                bs_pack_new_cti = bytes_copy_to_bytes_sentou(bs_pack_new_cti, byte1);
                Array.Resize(ref bs_pack_new, bs_pack_new.Length + bs_pack_new_cti.Length);
                Array.Copy(bs_pack_new_cti, 0, bs_pack_new, bs_pack_new.Length - bs_pack_new_cti.Length, bs_pack_new_cti.Length);

                goto label_output_bin;


            label_shop_parts_info_to_bin:;
                for (int i = 0; i < lines.Count(); i++)
                {
                    if (CountOf(lines[i], ",") != 1)
                    {
                        sousuu += 1;
                        str_seek = 0;

                        //パーツID
                        bs_pack_new = int2_copy_to_bytes(lines[i], bs_pack_new);

                        //ショップサブメニューID
                        bs_pack_new = byte1_copy_to_bytes(lines[i], bs_pack_new);

                        //値段
                        bs_pack_new = int4_copy_to_bytes(lines[i], bs_pack_new);

                        //ライバル仕様名称1
                        bs_pack_new = jis_copy_to_bytes(lines[i], bs_pack_new);

                        //ライバルアイコンID1
                        bs_pack_new = sbyte1_copy_to_bytes(lines[i], bs_pack_new);

                        //ライバル仕様名称2
                        bs_pack_new = jis_copy_to_bytes(lines[i], bs_pack_new);

                        //ライバルアイコンID2
                        bs_pack_new = sbyte1_copy_to_bytes(lines[i], bs_pack_new);
                    }
                }
                byte1 = BitConverter.GetBytes(sousuu);
                bs_pack_new = bytes_copy_to_bytes_sentou(bs_pack_new, byte1);

                goto label_output_bin;


            label_car_maker_info_to_bin:;
                for (int i = 0; i < lines.Count(); i++)
                {
                    if (CountOf(lines[i], ",") != 1)
                    {
                        sousuu += 1;
                        str_seek = 0;

                        //メーカー名称
                        bs_pack_new = jis_copy_to_bytes(lines[i], bs_pack_new);

                        //メーカー略称
                        bs_pack_new = jis_copy_to_bytes(lines[i], bs_pack_new);
                    }
                }
                byte1 = BitConverter.GetBytes(sousuu);
                bs_pack_new = bytes_copy_to_bytes_sentou(bs_pack_new, byte1);

                goto label_output_bin;


            label_parts_maker_info_to_bin:;
                for (int i = 0; i < lines.Count(); i++)
                {
                    if (CountOf(lines[i], ",") != 1)
                    {
                        sousuu += 1;
                        str_seek = 0;

                        //フラグ
                        bs_pack_new = sbyte1_copy_to_bytes(lines[i], bs_pack_new);

                        //メーカー名
                        bs_pack_new = jis_copy_to_bytes(lines[i], bs_pack_new);

                        //メーカー略称
                        bs_pack_new = jis_copy_to_bytes(lines[i], bs_pack_new);

                        //アクロバータ2D呼び出しID
                        bs_pack_new = sbyte1_copy_to_bytes(lines[i], bs_pack_new);
                    }
                }
                byte1 = BitConverter.GetBytes(sousuu);
                bs_pack_new = bytes_copy_to_bytes_sentou(bs_pack_new, byte1);

                goto label_output_bin;


            label_makername_car_info_to_bin:;
                for (int i = 0; i < lines.Count(); i++)
                {
                    if (CountOf(lines[i], ",") != 1)
                    {
                        sousuu += 1;
                        str_seek = 0;

                        //車ID
                        bs_pack_new = int2_copy_to_bytes(lines[i], bs_pack_new);

                        //名称
                        bs_pack_new = jis_copy_to_bytes(lines[i], bs_pack_new);

                        //略称
                        bs_pack_new = jis_copy_to_bytes(lines[i], bs_pack_new);

                        //封印カーフラグ(DLC)
                        bs_pack_new = byte1_copy_to_bytes(lines[i], bs_pack_new);
                    }
                }
                byte1 = BitConverter.GetBytes(sousuu);
                bs_pack_new = bytes_copy_to_bytes_sentou(bs_pack_new, byte1);

                goto label_output_bin;


            label_output_bin:;
                string output_bin_path = Path.GetDirectoryName(path[b]) + @"\new\";
                if (!Directory.Exists(output_bin_path))
                    Directory.CreateDirectory(output_bin_path);
                FileStream fsw_rp = new FileStream(output_bin_path + Path.GetFileNameWithoutExtension(path[b]) + ".bin", FileMode.Create, FileAccess.Write);
                fsw_rp.Write(bs_pack_new, 0, bs_pack_new.Length);
                fsw_rp.Close();

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
        /// SHIFT-JIS形式のデータをコピーしてstringに返す
        /// </summary>
        public static string jis_copy_to_str(byte[] bytes, string text)
        {
            Array.Copy(bytes, bs_seek, byte1, 0, 1);
            if (byte1[0] != 0x00)
            {
                int name_length = byte1[0];
                byte[] name_byte = new byte[name_length];
                Array.Copy(bytes, bs_seek + 1, name_byte, 0, name_length);
                bs_seek += name_length + 1;
                text = text + Encoding.GetEncoding("Shift_JIS").GetString(name_byte) + ",";
            }
            else
            {
                text = text + ",";
                bs_seek += 1;
            }
            return text;
        }

        /// <summary>
        /// sbyte形式(1バイト)のデータをコピーしてstringに返す
        /// </summary>
        public static string sbyte1_copy_to_str(byte[] bytes, string text)
        {
            Array.Copy(bytes, bs_seek, byte1, 0, 1);
            sbyte sbytedata = (sbyte)byte1[0];
            bs_seek += 1;
            text = text + sbytedata.ToString() + ",";
            return text;
        }

        /// <summary>
        /// byte形式(1バイト)のデータをコピーしてstringに返す
        /// </summary>
        public static string byte1_copy_to_str(byte[] bytes, string text)
        {
            Array.Copy(bytes, bs_seek, byte1, 0, 1);
            bs_seek += 1;
            text = text + byte1[0].ToString() + ",";
            return text;
        }

        /// <summary>
        /// short形式(2バイト)のデータをコピーしてstringに返す
        /// </summary>
        public static string short2_copy_to_str(byte[] bytes, string text)
        {
            Array.Copy(bytes, bs_seek, byte2, 0, 2);
            short shortdata = BitConverter.ToInt16(byte2, 0);
            bs_seek += 2;
            text = text + shortdata.ToString() + ",";
            return text;
        }

        /// <summary>
        /// int形式(2バイト)のデータをコピーしてstringに返す
        /// </summary>
        public static string int2_copy_to_str(byte[] bytes, string text)
        {
            Array.Copy(bytes, bs_seek, byte2, 0, 2);
            int intdata = BitConverter.ToUInt16(byte2, 0);
            bs_seek += 2;
            text = text + intdata.ToString() + ",";
            return text;
        }

        /// <summary>
        /// int形式(4バイト)のデータをコピーしてstringに返す
        /// </summary>
        public static string int4_copy_to_str(byte[] bytes, string text)
        {
            Array.Copy(bytes, bs_seek, byte4, 0, 4);
            int intdata = BitConverter.ToInt32(byte4, 0);
            bs_seek += 4;
            text = text + intdata.ToString() + ",";
            return text;
        }

        /// <summary>
        /// float形式のデータをコピーしてstringに返す
        /// </summary>
        public static string float_copy_to_str(byte[] bytes, string text)
        {
            Array.Copy(bytes, bs_seek, byte4, 0, 4);
            float floatdata = BitConverter.ToSingle(byte4, 0);
            bs_seek += 4;
            text = text + floatdata.ToString() + ",";
            return text;
        }

        /// <summary>
        /// 文字列をSHIFT-JIS形式でコピーしてbyte配列に返す
        /// </summary>
        public static byte[] jis_copy_to_bytes(string text, byte[] bytes)
        {
            string str_copy = "";
            while (true)
            {
                string str1 = text.Substring(str_seek + str_copy.Length, 1);
                if (str1 == ",")
                    break;
                str_copy = str_copy + str1;
            }

            if (string.IsNullOrEmpty(str_copy) == false)
            {
                byte[] bytes_copy = Encoding.GetEncoding("Shift_JIS").GetBytes(str_copy);
                byte[] bytes_copy_length = BitConverter.GetBytes(bytes_copy.Length);
                Array.Resize(ref bytes, bytes.Length + 1);
                Array.Copy(bytes_copy_length, 0, bytes, bytes.Length - 1, 1);
                Array.Resize(ref bytes, bytes.Length + bytes_copy.Length);
                Array.Copy(bytes_copy, 0, bytes, bytes.Length - bytes_copy.Length, bytes_copy.Length);
            }
            else
                Array.Resize(ref bytes, bytes.Length + 1);

            str_seek += str_copy.Length + 1;
            return bytes;
        }

        /// <summary>
        /// sbyte形式(1バイト)のデータをコピーしてbyte配列に返す
        /// </summary>
        public static byte[] sbyte1_copy_to_bytes(string text, byte[] bytes)
        {
            string str_copy = "";
            while (true)
            {
                string str1 = text.Substring(str_seek + str_copy.Length, 1);
                if (str1 == ",")
                    break;
                str_copy = str_copy + str1;
            }

            if (string.IsNullOrEmpty(str_copy) == false)
            {
                sbyte byte1 = sbyte.Parse(str_copy);
                byte[] bytes_copy = BitConverter.GetBytes(byte1);
                Array.Resize(ref bytes, bytes.Length + 1);
                Array.Copy(bytes_copy, 0, bytes, bytes.Length - 1, 1);
            }
            else
                Array.Resize(ref bytes, bytes.Length + 1);

            str_seek += str_copy.Length + 1;
            return bytes;
        }

        /// <summary>
        /// byte形式(1バイト)のデータをコピーしてbyte配列に返す
        /// </summary>
        public static byte[] byte1_copy_to_bytes(string text, byte[] bytes)
        {
            string str_copy = "";
            while (true)
            {
                string str1 = text.Substring(str_seek + str_copy.Length, 1);
                if (str1 == ",")
                    break;
                str_copy = str_copy + str1;
            }

            if (string.IsNullOrEmpty(str_copy) == false)
            {
                byte byte1 = byte.Parse(str_copy);
                byte[] bytes_copy = BitConverter.GetBytes(byte1);
                Array.Resize(ref bytes, bytes.Length + 1);
                Array.Copy(bytes_copy, 0, bytes, bytes.Length - 1, 1);
            }
            else
                Array.Resize(ref bytes, bytes.Length + 1);

            str_seek += str_copy.Length + 1;
            return bytes;
        }

        /// <summary>
        /// short形式(2バイト)のデータをコピーしてbyte配列に返す
        /// </summary>
        public static byte[] short2_copy_to_bytes(string text, byte[] bytes)
        {
            string str_copy = "";
            while (true)
            {
                string str1 = text.Substring(str_seek + str_copy.Length, 1);
                if (str1 == ",")
                    break;
                str_copy = str_copy + str1;
            }

            if (string.IsNullOrEmpty(str_copy) == false)
            {
                short byte2 = short.Parse(str_copy);
                byte[] bytes_copy = BitConverter.GetBytes(byte2);
                Array.Resize(ref bytes, bytes.Length + 2);
                Array.Copy(bytes_copy, 0, bytes, bytes.Length - 2, 2);
            }
            else
                Array.Resize(ref bytes, bytes.Length + 2);

            str_seek += str_copy.Length + 1;
            return bytes;
        }

        /// <summary>
        /// int形式(2バイト)のデータをコピーしてbyte配列に返す
        /// </summary>
        public static byte[] int2_copy_to_bytes(string text, byte[] bytes)
        {
            string str_copy = "";
            while (true)
            {
                string str1 = text.Substring(str_seek + str_copy.Length, 1);
                if (str1 == ",")
                    break;
                str_copy = str_copy + str1;
            }

            if (string.IsNullOrEmpty(str_copy) == false)
            {
                int byte2 = int.Parse(str_copy);
                byte[] bytes_copy = BitConverter.GetBytes(byte2);
                Array.Resize(ref bytes, bytes.Length + 2);
                Array.Copy(bytes_copy, 0, bytes, bytes.Length - 2, 2);
            }
            else
                Array.Resize(ref bytes, bytes.Length + 2);

            str_seek += str_copy.Length + 1;
            return bytes;
        }

        /// <summary>
        /// int形式(4バイト)のデータをコピーしてbyte配列に返す
        /// </summary>
        public static byte[] int4_copy_to_bytes(string text, byte[] bytes)
        {
            string str_copy = "";
            while (true)
            {
                string str1 = text.Substring(str_seek + str_copy.Length, 1);
                if (str1 == ",")
                    break;
                str_copy = str_copy + str1;
            }

            if (string.IsNullOrEmpty(str_copy) == false)
            {
                int byte4 = int.Parse(str_copy);
                byte[] bytes_copy = BitConverter.GetBytes(byte4);
                Array.Resize(ref bytes, bytes.Length + 4);
                Array.Copy(bytes_copy, 0, bytes, bytes.Length - 4, 4);
            }
            else
                Array.Resize(ref bytes, bytes.Length + 4);

            str_seek += str_copy.Length + 1;
            return bytes;
        }

        /// <summary>
        /// float形式(4バイト)のデータをコピーしてbyte配列に返す
        /// </summary>
        public static byte[] float_copy_to_bytes(string text, byte[] bytes)
        {
            string str_copy = "";
            while (true)
            {
                string str1 = text.Substring(str_seek + str_copy.Length, 1);
                if (str1 == ",")
                    break;
                str_copy = str_copy + str1;
            }

            if (string.IsNullOrEmpty(str_copy) == false)
            {
                float byte4 = float.Parse(str_copy);
                byte[] bytes_copy = BitConverter.GetBytes(byte4);
                Array.Resize(ref bytes, bytes.Length + 4);
                Array.Copy(bytes_copy, 0, bytes, bytes.Length - 4, 4);
            }
            else
                Array.Resize(ref bytes, bytes.Length + 4);

            str_seek += str_copy.Length + 1;
            return bytes;
        }

        /// <summary>
        /// byte配列のデータ(総数)を別のbyte配列の先頭にコピーする
        /// </summary>
        public static byte[] bytes_copy_to_bytes_sentou(byte[] bytes, byte[] bytes_copy_moto)
        {
            Array.Resize(ref bytes, bytes.Length + 1);
            Array.Copy(bytes, 0, bytes, 1, bytes.Length - 1);
            Array.Copy(bytes_copy_moto, 0, bytes, 0, 1);
            return bytes;
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
        /*
        The MIT License (MIT)
        Copyright (c) 2016 DOBON! <http://dobon.net>
        Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
        The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
        THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
        */

        /// <summary>
        /// 指定した文字列がいくつあるか
        /// </summary>
        public static int CountOf(string target, params string[] strArray)
        {
            int count = 0;

            foreach (string str in strArray)
            {
                int index = target.IndexOf(str, 0);
                while (index != -1)
                {
                    count++;
                    index = target.IndexOf(str, index + str.Length);
                }
            }

            return count;
        }
    }
}
