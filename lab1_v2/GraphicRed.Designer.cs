namespace lab1_v2
{
    partial class form_graphic
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.PB = new System.Windows.Forms.PictureBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnColor = new System.Windows.Forms.Button();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.numericPenWidth = new System.Windows.Forms.NumericUpDown();
            this.FiguresListBox = new System.Windows.Forms.ListBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.btnStartHosting = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.maskedTextBox = new System.Windows.Forms.MaskedTextBox();
            this.numericPort = new System.Windows.Forms.NumericUpDown();
            this.btnCommit = new System.Windows.Forms.Button();
            this.btnPull = new System.Windows.Forms.Button();
            this.btnDisconnect = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.PB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericPenWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericPort)).BeginInit();
            this.SuspendLayout();
            // 
            // PB
            // 
            this.PB.BackColor = System.Drawing.SystemColors.Window;
            this.PB.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.PB.Location = new System.Drawing.Point(12, 154);
            this.PB.Margin = new System.Windows.Forms.Padding(0);
            this.PB.Name = "PB";
            this.PB.Size = new System.Drawing.Size(1345, 785);
            this.PB.TabIndex = 2;
            this.PB.TabStop = false;
            this.PB.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PB_MouseDown);
            this.PB.MouseLeave += new System.EventHandler(this.PB_MouseLeave);
            this.PB.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PB_MouseMove);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(476, 70);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(120, 47);
            this.btnClear.TabIndex = 6;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.BtnClear_Click);
            // 
            // btnColor
            // 
            this.btnColor.Location = new System.Drawing.Point(191, 70);
            this.btnColor.Name = "btnColor";
            this.btnColor.Size = new System.Drawing.Size(113, 47);
            this.btnColor.TabIndex = 7;
            this.btnColor.Text = "color";
            this.btnColor.UseVisualStyleBackColor = true;
            this.btnColor.Click += new System.EventHandler(this.BtnColor_Click);
            // 
            // numericPenWidth
            // 
            this.numericPenWidth.Location = new System.Drawing.Point(22, 70);
            this.numericPenWidth.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numericPenWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericPenWidth.Name = "numericPenWidth";
            this.numericPenWidth.Size = new System.Drawing.Size(120, 22);
            this.numericPenWidth.TabIndex = 8;
            this.numericPenWidth.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.numericPenWidth.ValueChanged += new System.EventHandler(this.NumericPenWidth_ValueChanged);
            this.numericPenWidth.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NumericPenWidth_KeyPress);
            // 
            // FiguresListBox
            // 
            this.FiguresListBox.FormattingEnabled = true;
            this.FiguresListBox.ItemHeight = 16;
            this.FiguresListBox.Location = new System.Drawing.Point(22, 14);
            this.FiguresListBox.Name = "FiguresListBox";
            this.FiguresListBox.Size = new System.Drawing.Size(282, 36);
            this.FiguresListBox.TabIndex = 9;
            this.FiguresListBox.SelectedIndexChanged += new System.EventHandler(this.FiguresListBox_SelectedIndexChanged);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(332, 14);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(120, 47);
            this.btnSave.TabIndex = 10;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(476, 14);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(120, 47);
            this.btnLoad.TabIndex = 11;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.BtnLoad_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog1";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "dat";
            this.saveFileDialog.Filter = "dat files (*.dat)|*.dat|All files (*.*)|*.*";
            // 
            // btnStartHosting
            // 
            this.btnStartHosting.Location = new System.Drawing.Point(730, 14);
            this.btnStartHosting.Name = "btnStartHosting";
            this.btnStartHosting.Size = new System.Drawing.Size(105, 47);
            this.btnStartHosting.TabIndex = 12;
            this.btnStartHosting.Text = "Start hosting";
            this.btnStartHosting.UseVisualStyleBackColor = true;
            this.btnStartHosting.Click += new System.EventHandler(this.BtnStartHosting_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(924, 12);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(105, 47);
            this.btnConnect.TabIndex = 13;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.BtnConnect_Click);
            // 
            // maskedTextBox
            // 
            this.maskedTextBox.AsciiOnly = true;
            this.maskedTextBox.Location = new System.Drawing.Point(924, 118);
            this.maskedTextBox.Name = "maskedTextBox";
            this.maskedTextBox.Size = new System.Drawing.Size(100, 22);
            this.maskedTextBox.TabIndex = 14;
            this.maskedTextBox.Text = "127.0.0.1";
            this.maskedTextBox.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            // 
            // numericPort
            // 
            this.numericPort.Location = new System.Drawing.Point(730, 82);
            this.numericPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericPort.Name = "numericPort";
            this.numericPort.Size = new System.Drawing.Size(105, 22);
            this.numericPort.TabIndex = 15;
            this.numericPort.Value = new decimal(new int[] {
            7788,
            0,
            0,
            0});
            // 
            // btnCommit
            // 
            this.btnCommit.Location = new System.Drawing.Point(1122, 14);
            this.btnCommit.Name = "btnCommit";
            this.btnCommit.Size = new System.Drawing.Size(121, 47);
            this.btnCommit.TabIndex = 16;
            this.btnCommit.Text = "Commit";
            this.btnCommit.UseVisualStyleBackColor = true;
            this.btnCommit.Click += new System.EventHandler(this.BtnCommit_Click);
            // 
            // btnPull
            // 
            this.btnPull.Location = new System.Drawing.Point(1122, 95);
            this.btnPull.Name = "btnPull";
            this.btnPull.Size = new System.Drawing.Size(121, 40);
            this.btnPull.TabIndex = 17;
            this.btnPull.Text = "Pull";
            this.btnPull.UseVisualStyleBackColor = true;
            this.btnPull.Click += new System.EventHandler(this.BtnPull_Click);
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Location = new System.Drawing.Point(924, 65);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(105, 47);
            this.btnDisconnect.TabIndex = 18;
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.BtnDisconnect_Click);
            // 
            // form_graphic
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1396, 953);
            this.Controls.Add(this.btnDisconnect);
            this.Controls.Add(this.btnPull);
            this.Controls.Add(this.btnCommit);
            this.Controls.Add(this.numericPort);
            this.Controls.Add(this.maskedTextBox);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.btnStartHosting);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.FiguresListBox);
            this.Controls.Add(this.numericPenWidth);
            this.Controls.Add(this.btnColor);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.PB);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.Name = "form_graphic";
            this.Text = "Graphic redactor";
            this.Load += new System.EventHandler(this.Form_graphic_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form_graphic_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Form_graphic_KeyPress);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form_graphic_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.PB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericPenWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericPort)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox PB;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnColor;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.NumericUpDown numericPenWidth;
        private System.Windows.Forms.ListBox FiguresListBox;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.Button btnStartHosting;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.MaskedTextBox maskedTextBox;
        private System.Windows.Forms.NumericUpDown numericPort;
        private System.Windows.Forms.Button btnCommit;
        private System.Windows.Forms.Button btnPull;
        private System.Windows.Forms.Button btnDisconnect;
    }
}

