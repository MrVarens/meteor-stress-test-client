namespace Meteor.StressTest.Forms
{
    partial class Main
    {
        /// <summary>
        /// Wymagana zmienna projektanta.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Wyczyść wszystkie używane zasoby.
        /// </summary>
        /// <param name="disposing">prawda, jeżeli zarządzane zasoby powinny zostać zlikwidowane; Fałsz w przeciwnym wypadku.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kod generowany przez Projektanta formularzy systemu Windows

        /// <summary>
        /// Wymagana metoda obsługi projektanta — nie należy modyfikować 
        /// zawartość tej metody z edytorem kodu.
        /// </summary>
        private void InitializeComponent()
        {
            this.rtbLogs = new System.Windows.Forms.RichTextBox();
            this.gbLogs = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lServerState = new System.Windows.Forms.Label();
            this.lTestState = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cbShowDebug = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lRunningCount = new System.Windows.Forms.Label();
            this.lExecutedCount = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.gbLogs.SuspendLayout();
            this.SuspendLayout();
            // 
            // rtbLogs
            // 
            this.rtbLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbLogs.Location = new System.Drawing.Point(6, 19);
            this.rtbLogs.Name = "rtbLogs";
            this.rtbLogs.Size = new System.Drawing.Size(398, 137);
            this.rtbLogs.TabIndex = 1;
            this.rtbLogs.Text = "";
            // 
            // gbLogs
            // 
            this.gbLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbLogs.Controls.Add(this.rtbLogs);
            this.gbLogs.Location = new System.Drawing.Point(12, 38);
            this.gbLogs.Name = "gbLogs";
            this.gbLogs.Size = new System.Drawing.Size(410, 162);
            this.gbLogs.TabIndex = 2;
            this.gbLogs.TabStop = false;
            this.gbLogs.Text = "Logs";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Server:";
            // 
            // lServerState
            // 
            this.lServerState.AutoSize = true;
            this.lServerState.Location = new System.Drawing.Point(50, 9);
            this.lServerState.Name = "lServerState";
            this.lServerState.Size = new System.Drawing.Size(10, 13);
            this.lServerState.TabIndex = 4;
            this.lServerState.Text = "-";
            // 
            // lTestState
            // 
            this.lTestState.AutoSize = true;
            this.lTestState.Location = new System.Drawing.Point(50, 22);
            this.lTestState.Name = "lTestState";
            this.lTestState.Size = new System.Drawing.Size(10, 13);
            this.lTestState.TabIndex = 6;
            this.lTestState.Text = "-";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Test:";
            // 
            // cbShowDebug
            // 
            this.cbShowDebug.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbShowDebug.AutoSize = true;
            this.cbShowDebug.Location = new System.Drawing.Point(331, 37);
            this.cbShowDebug.Name = "cbShowDebug";
            this.cbShowDebug.Size = new System.Drawing.Size(88, 17);
            this.cbShowDebug.TabIndex = 7;
            this.cbShowDebug.Text = "Show Debug";
            this.cbShowDebug.UseVisualStyleBackColor = true;
            this.cbShowDebug.Visible = false;
            this.cbShowDebug.CheckedChanged += new System.EventHandler(this.cbShowDebug_CheckedChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(328, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Executed:";
            // 
            // lRunningCount
            // 
            this.lRunningCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lRunningCount.AutoSize = true;
            this.lRunningCount.Location = new System.Drawing.Point(383, 9);
            this.lRunningCount.Name = "lRunningCount";
            this.lRunningCount.Size = new System.Drawing.Size(10, 13);
            this.lRunningCount.TabIndex = 9;
            this.lRunningCount.Text = "-";
            // 
            // lExecutedCount
            // 
            this.lExecutedCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lExecutedCount.AutoSize = true;
            this.lExecutedCount.Location = new System.Drawing.Point(383, 22);
            this.lExecutedCount.Name = "lExecutedCount";
            this.lExecutedCount.Size = new System.Drawing.Size(10, 13);
            this.lExecutedCount.TabIndex = 11;
            this.lExecutedCount.Text = "-";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(328, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(50, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Running:";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 212);
            this.Controls.Add(this.lExecutedCount);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lRunningCount);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbShowDebug);
            this.Controls.Add(this.lTestState);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lServerState);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.gbLogs);
            this.Name = "Main";
            this.Text = "Meteor - Stress test";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.Load += new System.EventHandler(this.Main_Load);
            this.gbLogs.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.RichTextBox rtbLogs;
        private System.Windows.Forms.GroupBox gbLogs;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lServerState;
        private System.Windows.Forms.Label lTestState;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox cbShowDebug;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lRunningCount;
        private System.Windows.Forms.Label lExecutedCount;
        private System.Windows.Forms.Label label5;
    }
}

