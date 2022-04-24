﻿namespace GameSaveBackup
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
            this.chkRegistBackup = new System.Windows.Forms.CheckBox();
            this.chkShowMsg = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnRegist
            // 
            this.btnRegist.Location = new System.Drawing.Point(307, 252);
            this.btnRegist.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnRegist.Name = "btnRegist";
            this.btnRegist.Size = new System.Drawing.Size(128, 88);
            this.btnRegist.TabIndex = 0;
            this.btnRegist.Text = "注册";
            this.btnRegist.UseVisualStyleBackColor = true;
            this.btnRegist.Click += new System.EventHandler(this.btnRegist_Click);
            // 
            // gameListBox
            // 
            this.gameListBox.FormattingEnabled = true;
            this.gameListBox.ItemHeight = 18;
            this.gameListBox.Location = new System.Drawing.Point(14, 35);
            this.gameListBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gameListBox.Name = "gameListBox";
            this.gameListBox.Size = new System.Drawing.Size(255, 616);
            this.gameListBox.TabIndex = 1;
            // 
            // chkRegistBackup
            // 
            this.chkRegistBackup.AutoSize = true;
            this.chkRegistBackup.Checked = true;
            this.chkRegistBackup.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRegistBackup.Location = new System.Drawing.Point(320, 500);
            this.chkRegistBackup.Name = "chkRegistBackup";
            this.chkRegistBackup.Size = new System.Drawing.Size(124, 22);
            this.chkRegistBackup.TabIndex = 2;
            this.chkRegistBackup.Text = "注册时备份";
            this.chkRegistBackup.UseVisualStyleBackColor = true;
            // 
            // chkShowMsg
            // 
            this.chkShowMsg.AutoSize = true;
            this.chkShowMsg.Checked = true;
            this.chkShowMsg.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowMsg.Location = new System.Drawing.Point(320, 552);
            this.chkShowMsg.Name = "chkShowMsg";
            this.chkShowMsg.Size = new System.Drawing.Size(160, 22);
            this.chkShowMsg.TabIndex = 3;
            this.chkShowMsg.Text = "保存加载时弹窗";
            this.chkShowMsg.UseVisualStyleBackColor = true;
            // 
            // GameSaveBackup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(516, 689);
            this.Controls.Add(this.chkShowMsg);
            this.Controls.Add(this.chkRegistBackup);
            this.Controls.Add(this.gameListBox);
            this.Controls.Add(this.btnRegist);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "GameSaveBackup";
            this.Text = "GameSaveBackup";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GameSaveBackup_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnRegist;
        private System.Windows.Forms.ListBox gameListBox;
        private System.Windows.Forms.CheckBox chkRegistBackup;
        private System.Windows.Forms.CheckBox chkShowMsg;
    }
}

