namespace DeadlyPremonitionTool
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.injectXCP = new System.Windows.Forms.Button();
            this.injectDPSerial = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.fileCounter = new System.Windows.Forms.Label();
            this.packSerialBtn = new System.Windows.Forms.Button();
            this.genFoldersOnly = new System.Windows.Forms.CheckBox();
            this.unpackXPC = new System.Windows.Forms.Button();
            this.messageBoxCheckbox = new System.Windows.Forms.CheckBox();
            this.fixFileSizes = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // injectXCP
            // 
            this.injectXCP.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.injectXCP.Location = new System.Drawing.Point(236, 12);
            this.injectXCP.Name = "injectXCP";
            this.injectXCP.Size = new System.Drawing.Size(218, 81);
            this.injectXCP.TabIndex = 0;
            this.injectXCP.Text = "Pack XPC";
            this.injectXCP.UseVisualStyleBackColor = true;
            this.injectXCP.Click += new System.EventHandler(this.injectXCP_Click);
            // 
            // injectDPSerial
            // 
            this.injectDPSerial.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.injectDPSerial.Location = new System.Drawing.Point(460, 12);
            this.injectDPSerial.Name = "injectDPSerial";
            this.injectDPSerial.Size = new System.Drawing.Size(218, 81);
            this.injectDPSerial.TabIndex = 1;
            this.injectDPSerial.Text = "Unpack DPSerial";
            this.injectDPSerial.UseVisualStyleBackColor = true;
            this.injectDPSerial.Click += new System.EventHandler(this.injectDPSerial_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 100);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Total Files: ";
            // 
            // fileCounter
            // 
            this.fileCounter.AutoSize = true;
            this.fileCounter.Location = new System.Drawing.Point(90, 100);
            this.fileCounter.Name = "fileCounter";
            this.fileCounter.Size = new System.Drawing.Size(0, 16);
            this.fileCounter.TabIndex = 3;
            // 
            // packSerialBtn
            // 
            this.packSerialBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.packSerialBtn.Location = new System.Drawing.Point(684, 12);
            this.packSerialBtn.Name = "packSerialBtn";
            this.packSerialBtn.Size = new System.Drawing.Size(218, 81);
            this.packSerialBtn.TabIndex = 4;
            this.packSerialBtn.Text = "Pack DPSerial";
            this.packSerialBtn.UseVisualStyleBackColor = true;
            this.packSerialBtn.Click += new System.EventHandler(this.packSerialBtn_Click);
            // 
            // genFoldersOnly
            // 
            this.genFoldersOnly.AutoSize = true;
            this.genFoldersOnly.Location = new System.Drawing.Point(737, 100);
            this.genFoldersOnly.Name = "genFoldersOnly";
            this.genFoldersOnly.Size = new System.Drawing.Size(164, 20);
            this.genFoldersOnly.TabIndex = 5;
            this.genFoldersOnly.Text = "Generate Folders Only";
            this.genFoldersOnly.UseVisualStyleBackColor = true;
            // 
            // unpackXPC
            // 
            this.unpackXPC.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.unpackXPC.Location = new System.Drawing.Point(12, 12);
            this.unpackXPC.Name = "unpackXPC";
            this.unpackXPC.Size = new System.Drawing.Size(218, 81);
            this.unpackXPC.TabIndex = 6;
            this.unpackXPC.Text = "Unpack XPC";
            this.unpackXPC.UseVisualStyleBackColor = true;
            this.unpackXPC.Click += new System.EventHandler(this.unpackXPC_Click);
            // 
            // messageBoxCheckbox
            // 
            this.messageBoxCheckbox.AutoSize = true;
            this.messageBoxCheckbox.Location = new System.Drawing.Point(559, 100);
            this.messageBoxCheckbox.Name = "messageBoxCheckbox";
            this.messageBoxCheckbox.Size = new System.Drawing.Size(163, 20);
            this.messageBoxCheckbox.TabIndex = 7;
            this.messageBoxCheckbox.Text = "Show Message Boxes";
            this.messageBoxCheckbox.UseVisualStyleBackColor = true;
            // 
            // fixFileSizes
            // 
            this.fixFileSizes.AutoSize = true;
            this.fixFileSizes.Checked = true;
            this.fixFileSizes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.fixFileSizes.Location = new System.Drawing.Point(437, 99);
            this.fixFileSizes.Name = "fixFileSizes";
            this.fixFileSizes.Size = new System.Drawing.Size(107, 20);
            this.fixFileSizes.TabIndex = 8;
            this.fixFileSizes.Text = "Fix File Sizes";
            this.fixFileSizes.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(915, 127);
            this.Controls.Add(this.fixFileSizes);
            this.Controls.Add(this.messageBoxCheckbox);
            this.Controls.Add(this.unpackXPC);
            this.Controls.Add(this.genFoldersOnly);
            this.Controls.Add(this.packSerialBtn);
            this.Controls.Add(this.fileCounter);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.injectDPSerial);
            this.Controls.Add(this.injectXCP);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Deadly Premonition Tool";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button injectXCP;
        private System.Windows.Forms.Button injectDPSerial;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label fileCounter;
        private System.Windows.Forms.Button packSerialBtn;
        private System.Windows.Forms.CheckBox genFoldersOnly;
        private System.Windows.Forms.Button unpackXPC;
        private System.Windows.Forms.CheckBox messageBoxCheckbox;
        private System.Windows.Forms.CheckBox fixFileSizes;
    }
}

