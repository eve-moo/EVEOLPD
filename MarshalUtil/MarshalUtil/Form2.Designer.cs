namespace MarshalUtil
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
      this.button1 = new System.Windows.Forms.Button();
      this.evePathTxtBox = new System.Windows.Forms.TextBox();
      this.btnProcess = new System.Windows.Forms.Button();
      this.txtOutput = new System.Windows.Forms.TextBox();
      this.progressBar1 = new System.Windows.Forms.ProgressBar();
      this.singleFile = new System.Windows.Forms.CheckBox();
      this.SuspendLayout();
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(12, 12);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(75, 23);
      this.button1.TabIndex = 0;
      this.button1.Text = "Packet Dir";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // evePathTxtBox
      // 
      this.evePathTxtBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.evePathTxtBox.Enabled = false;
      this.evePathTxtBox.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.evePathTxtBox.Location = new System.Drawing.Point(93, 15);
      this.evePathTxtBox.Name = "evePathTxtBox";
      this.evePathTxtBox.Size = new System.Drawing.Size(324, 20);
      this.evePathTxtBox.TabIndex = 1;
      // 
      // btnProcess
      // 
      this.btnProcess.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.btnProcess.Enabled = false;
      this.btnProcess.Location = new System.Drawing.Point(93, 41);
      this.btnProcess.Name = "btnProcess";
      this.btnProcess.Size = new System.Drawing.Size(324, 23);
      this.btnProcess.TabIndex = 2;
      this.btnProcess.Text = "Process";
      this.btnProcess.UseVisualStyleBackColor = true;
      this.btnProcess.Click += new System.EventHandler(this.btnProcess_Click);
      // 
      // txtOutput
      // 
      this.txtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtOutput.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtOutput.HideSelection = false;
      this.txtOutput.Location = new System.Drawing.Point(12, 70);
      this.txtOutput.Multiline = true;
      this.txtOutput.Name = "txtOutput";
      this.txtOutput.ReadOnly = true;
      this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.txtOutput.Size = new System.Drawing.Size(405, 149);
      this.txtOutput.TabIndex = 3;
      // 
      // progressBar1
      // 
      this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.progressBar1.Location = new System.Drawing.Point(12, 225);
      this.progressBar1.Name = "progressBar1";
      this.progressBar1.Size = new System.Drawing.Size(405, 23);
      this.progressBar1.TabIndex = 4;
      // 
      // singleFile
      // 
      this.singleFile.AutoSize = true;
      this.singleFile.Checked = true;
      this.singleFile.CheckState = System.Windows.Forms.CheckState.Checked;
      this.singleFile.Location = new System.Drawing.Point(12, 45);
      this.singleFile.Name = "singleFile";
      this.singleFile.Size = new System.Drawing.Size(71, 17);
      this.singleFile.TabIndex = 5;
      this.singleFile.Text = "Single file";
      this.singleFile.UseVisualStyleBackColor = true;
      // 
      // Form2
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(429, 260);
      this.Controls.Add(this.singleFile);
      this.Controls.Add(this.progressBar1);
      this.Controls.Add(this.txtOutput);
      this.Controls.Add(this.btnProcess);
      this.Controls.Add(this.evePathTxtBox);
      this.Controls.Add(this.button1);
      this.Name = "Form2";
      this.Text = "Bulk Packet Decode";
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox evePathTxtBox;
        private System.Windows.Forms.Button btnProcess;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.ProgressBar progressBar1;
    private System.Windows.Forms.CheckBox singleFile;
  }
}