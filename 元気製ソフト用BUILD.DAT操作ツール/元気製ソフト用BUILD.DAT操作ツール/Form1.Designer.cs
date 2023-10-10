namespace 元気製ソフト用BUILD.DAT操作ツール
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.Label1 = new System.Windows.Forms.Label();
            this.ProgressBar1 = new System.Windows.Forms.ProgressBar();
            this.BackgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.label2 = new System.Windows.Forms.Label();
            this.radioTouge = new System.Windows.Forms.RadioButton();
            this.radioNormal = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(10, 38);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(191, 12);
            this.Label1.TabIndex = 0;
            this.Label1.Text = "exeに直接ドラッグアンドドロップしてね！";
            // 
            // ProgressBar1
            // 
            this.ProgressBar1.Location = new System.Drawing.Point(12, 12);
            this.ProgressBar1.Name = "ProgressBar1";
            this.ProgressBar1.Size = new System.Drawing.Size(300, 18);
            this.ProgressBar1.TabIndex = 1;
            // 
            // BackgroundWorker1
            // 
            this.BackgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorker1_DoWork);
            this.BackgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BackgroundWorker1_ProgressChanged);
            this.BackgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorker1_RunWorkerCompleted);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "または...設定";
            this.label2.Visible = false;
            // 
            // radioTouge
            // 
            this.radioTouge.AutoSize = true;
            this.radioTouge.Checked = global::元気製ソフト用BUILD.DAT操作ツール.Properties.Settings.Default.Tougechecked;
            this.radioTouge.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::元気製ソフト用BUILD.DAT操作ツール.Properties.Settings.Default, "Tougechecked", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.radioTouge.Location = new System.Drawing.Point(65, 85);
            this.radioTouge.Name = "radioTouge";
            this.radioTouge.Size = new System.Drawing.Size(118, 16);
            this.radioTouge.TabIndex = 4;
            this.radioTouge.Text = "峠の伝説(不完全?)";
            this.radioTouge.UseVisualStyleBackColor = true;
            this.radioTouge.Visible = false;
            this.radioTouge.MouseClick += new System.Windows.Forms.MouseEventHandler(this.radioTouge_MouseClick);
            // 
            // radioNormal
            // 
            this.radioNormal.AutoSize = true;
            this.radioNormal.Checked = global::元気製ソフト用BUILD.DAT操作ツール.Properties.Settings.Default.Normalchecked;
            this.radioNormal.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::元気製ソフト用BUILD.DAT操作ツール.Properties.Settings.Default, "Normalchecked", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.radioNormal.Location = new System.Drawing.Point(12, 85);
            this.radioNormal.Name = "radioNormal";
            this.radioNormal.Size = new System.Drawing.Size(47, 16);
            this.radioNormal.TabIndex = 3;
            this.radioNormal.TabStop = true;
            this.radioNormal.Text = "通常";
            this.radioNormal.UseVisualStyleBackColor = true;
            this.radioNormal.Visible = false;
            this.radioNormal.MouseClick += new System.Windows.Forms.MouseEventHandler(this.radioNormal_MouseClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 61);
            this.Controls.Add(this.radioTouge);
            this.Controls.Add(this.radioNormal);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ProgressBar1);
            this.Controls.Add(this.Label1);
            this.Name = "Form1";
            this.Text = "元気製ソフト用BUILD.DAT操作ツール";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Label1;
        private System.Windows.Forms.ProgressBar ProgressBar1;
        private System.ComponentModel.BackgroundWorker BackgroundWorker1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton radioNormal;
        private System.Windows.Forms.RadioButton radioTouge;
    }
}

