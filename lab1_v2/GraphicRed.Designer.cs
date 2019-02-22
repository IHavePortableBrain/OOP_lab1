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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "Curve"}, 0, System.Drawing.Color.Empty, System.Drawing.SystemColors.Window, null);
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Ellipse", 1);
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Line", 2);
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("Rectangle", 3);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(form_graphic));
            this.button1 = new System.Windows.Forms.Button();
            this.PB = new System.Windows.Forms.PictureBox();
            this.button2 = new System.Windows.Forms.Button();
            this.LVfigures = new System.Windows.Forms.ListView();
            this.ImgList = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.PB)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(96, 11);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(260, 51);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // PB
            // 
            this.PB.BackColor = System.Drawing.SystemColors.Window;
            this.PB.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.PB.Location = new System.Drawing.Point(12, 69);
            this.PB.Margin = new System.Windows.Forms.Padding(5);
            this.PB.Name = "PB";
            this.PB.Padding = new System.Windows.Forms.Padding(5);
            this.PB.Size = new System.Drawing.Size(1238, 577);
            this.PB.TabIndex = 2;
            this.PB.TabStop = false;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(418, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(217, 50);
            this.button2.TabIndex = 3;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // LVfigures
            // 
            listViewItem1.StateImageIndex = 0;
            listViewItem1.Tag = "";
            listViewItem2.Checked = true;
            listViewItem2.StateImageIndex = 1;
            listViewItem3.Checked = true;
            listViewItem3.StateImageIndex = 2;
            listViewItem4.Checked = true;
            listViewItem4.StateImageIndex = 3;
            this.LVfigures.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4});
            this.LVfigures.LargeImageList = this.ImgList;
            this.LVfigures.Location = new System.Drawing.Point(844, 2);
            this.LVfigures.MultiSelect = false;
            this.LVfigures.Name = "LVfigures";
            this.LVfigures.Size = new System.Drawing.Size(332, 61);
            this.LVfigures.SmallImageList = this.ImgList;
            this.LVfigures.TabIndex = 5;
            this.LVfigures.TileSize = new System.Drawing.Size(200, 200);
            this.LVfigures.UseCompatibleStateImageBehavior = false;
            this.LVfigures.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.LVfigures_ItemChecked);
            this.LVfigures.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.LVfigures_ItemSelectionChanged);
            this.LVfigures.SelectedIndexChanged += new System.EventHandler(this.LVfigures_SelectedIndexChanged);
            // 
            // ImgList
            // 
            this.ImgList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ImgList.ImageStream")));
            this.ImgList.TransparentColor = System.Drawing.Color.Transparent;
            this.ImgList.Images.SetKeyName(0, "curve.png");
            this.ImgList.Images.SetKeyName(1, "ellipse.png");
            this.ImgList.Images.SetKeyName(2, "line.png");
            this.ImgList.Images.SetKeyName(3, "rect.png");
            // 
            // form_graphic
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1262, 673);
            this.Controls.Add(this.LVfigures);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.PB);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "form_graphic";
            this.Text = "Graphic redactor";
            this.Activated += new System.EventHandler(this.form_graphic_Activated);
            ((System.ComponentModel.ISupportInitialize)(this.PB)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox PB;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ListView LVfigures;
        private System.Windows.Forms.ImageList ImgList;
    }
}

