namespace GameSaveBackup
{
    partial class GameSaveBackup
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnRegist = new System.Windows.Forms.Button();
            this.gameListBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // btnRegist
            // 
            this.btnRegist.Location = new System.Drawing.Point(273, 210);
            this.btnRegist.Name = "btnRegist";
            this.btnRegist.Size = new System.Drawing.Size(114, 73);
            this.btnRegist.TabIndex = 0;
            this.btnRegist.Text = "注册";
            this.btnRegist.UseVisualStyleBackColor = true;
            this.btnRegist.Click += new System.EventHandler(this.btnRegist_Click);
            // 
            // gameListBox
            // 
            this.gameListBox.FormattingEnabled = true;
            this.gameListBox.ItemHeight = 15;
            this.gameListBox.Location = new System.Drawing.Point(12, 29);
            this.gameListBox.Name = "gameListBox";
            this.gameListBox.Size = new System.Drawing.Size(227, 514);
            this.gameListBox.TabIndex = 1;
            // 
            // GameSaveBackup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(459, 574);
            this.Controls.Add(this.gameListBox);
            this.Controls.Add(this.btnRegist);
            this.Name = "GameSaveBackup";
            this.Text = "GameSaveBackup";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GameSaveBackup_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnRegist;
        private System.Windows.Forms.ListBox gameListBox;
    }
}

