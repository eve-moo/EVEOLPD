namespace MarshalUtil
{
    partial class ProcessStatus
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.directoryName = new System.Windows.Forms.TextBox();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.directoryProgress = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // directoryName
            // 
            this.directoryName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.directoryName.Location = new System.Drawing.Point(0, 0);
            this.directoryName.Name = "directoryName";
            this.directoryName.Size = new System.Drawing.Size(416, 20);
            this.directoryName.TabIndex = 0;
            // 
            // txtOutput
            // 
            this.txtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutput.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOutput.HideSelection = false;
            this.txtOutput.Location = new System.Drawing.Point(0, 26);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ReadOnly = true;
            this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtOutput.Size = new System.Drawing.Size(416, 128);
            this.txtOutput.TabIndex = 4;
            this.txtOutput.WordWrap = false;
            // 
            // directoryProgress
            // 
            this.directoryProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.directoryProgress.Location = new System.Drawing.Point(0, 156);
            this.directoryProgress.Name = "directoryProgress";
            this.directoryProgress.Size = new System.Drawing.Size(416, 23);
            this.directoryProgress.TabIndex = 5;
            // 
            // ProcessStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.directoryProgress);
            this.Controls.Add(this.txtOutput);
            this.Controls.Add(this.directoryName);
            this.Name = "ProcessStatus";
            this.Size = new System.Drawing.Size(419, 182);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox directoryName;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.ProgressBar directoryProgress;
    }
}
