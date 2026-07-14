namespace HallThrusterTestSystem
{
    partial class MainForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.xbdPump1 = new xbd.ControlLib.xbdPump();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.xbdFlowControl1 = new xbd.ControlLib.xbdFlowControl();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1045, 515);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 18);
            this.label1.TabIndex = 0;
            this.label1.Tag = "fk/ai0";
            this.label1.Text = "label1";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(209, 703);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // xbdPump1
            // 
            this.xbdPump1.BackColor = System.Drawing.SystemColors.Control;
            this.xbdPump1.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(218)))), ((int)(((byte)(227)))));
            this.xbdPump1.Color2 = System.Drawing.Color.LightGray;
            this.xbdPump1.Color3 = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(135)))), ((int)(((byte)(69)))));
            this.xbdPump1.Color4 = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(208)))), ((int)(((byte)(214)))));
            this.xbdPump1.Color5 = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(213)))), ((int)(((byte)(220)))));
            this.xbdPump1.Color6 = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(160)))), ((int)(((byte)(169)))));
            this.xbdPump1.Color7 = System.Drawing.Color.FromArgb(((int)(((byte)(92)))), ((int)(((byte)(100)))), ((int)(((byte)(111)))));
            this.xbdPump1.Export = 6;
            this.xbdPump1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.xbdPump1.IsRun = true;
            this.xbdPump1.Location = new System.Drawing.Point(454, 637);
            this.xbdPump1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.xbdPump1.MoveSpeed = 1F;
            this.xbdPump1.Name = "xbdPump1";
            this.xbdPump1.PumpStyle = xbd.ControlLib.DirectionStyle.Vertical;
            this.xbdPump1.Size = new System.Drawing.Size(92, 89);
            this.xbdPump1.TabIndex = 9;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImage = global::HallThrusterTestSystem.Properties.Resources.poster;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(157, -340);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1317, 1147);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 8;
            this.pictureBox1.TabStop = false;
            // 
            // xbdFlowControl1
            // 
            this.xbdFlowControl1.BackColor = System.Drawing.Color.Transparent;
            this.xbdFlowControl1.ColorPipeLineCenter = System.Drawing.Color.DodgerBlue;
            this.xbdFlowControl1.EdgeColor = System.Drawing.Color.DimGray;
            this.xbdFlowControl1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.xbdFlowControl1.LineCenterColor = System.Drawing.Color.LightGray;
            this.xbdFlowControl1.Location = new System.Drawing.Point(476, 432);
            this.xbdFlowControl1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.xbdFlowControl1.MoveSpeed = 0.2F;
            this.xbdFlowControl1.Name = "xbdFlowControl1";
            this.xbdFlowControl1.PipeLineActive = false;
            this.xbdFlowControl1.PipeLineGap = 2;
            this.xbdFlowControl1.PipeLineLength = 2;
            this.xbdFlowControl1.PipeLineStyle = xbd.ControlLib.DirectionStyle.Vertical;
            this.xbdFlowControl1.PipeLineWidth = 5;
            this.xbdFlowControl1.PipeTurnLeft = xbd.ControlLib.PipeTurnDirection.None;
            this.xbdFlowControl1.PipeTurnRight = xbd.ControlLib.PipeTurnDirection.None;
            this.xbdFlowControl1.Size = new System.Drawing.Size(23, 208);
            this.xbdFlowControl1.TabIndex = 10;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1197, 515);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 18);
            this.label2.TabIndex = 0;
            this.label2.Tag = "Mod1/ai0";
            this.label2.Text = "label1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(1045, 570);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 18);
            this.label3.TabIndex = 0;
            this.label3.Tag = "fk/ai1";
            this.label3.Text = "label1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(1197, 570);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 18);
            this.label4.TabIndex = 0;
            this.label4.Tag = "Mod1/ai1";
            this.label4.Text = "label1";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(1045, 622);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 18);
            this.label5.TabIndex = 0;
            this.label5.Tag = "fk/ai2";
            this.label5.Text = "label1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1648, 912);
            this.Controls.Add(this.xbdPump1);
            this.Controls.Add(this.xbdFlowControl1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Name = "MainForm";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private xbd.ControlLib.xbdPump xbdPump1;
        private xbd.ControlLib.xbdFlowControl xbdFlowControl1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}

