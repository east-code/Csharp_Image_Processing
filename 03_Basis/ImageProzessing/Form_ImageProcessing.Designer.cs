namespace ImageProcessing
{
  partial class Form_ImageProcessing
  {
    /// <summary>
    /// Erforderliche Designervariable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Verwendete Ressourcen bereinigen.
    /// </summary>
    /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Vom Windows Form-Designer generierter Code

    /// <summary>
    /// Erforderliche Methode für die Designerunterstützung.
    /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      this.menuStrip = new System.Windows.Forms.MenuStrip();
      this.dateiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.bildLadenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.bildSpeichernToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.bildToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
      this.beendenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.contextMenuStrip_SizeMode = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.einpassenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.streckenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.img3DImgToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.img1DImgToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.tabPage_Info = new System.Windows.Forms.TabPage();
      this.richTextBox_Info = new System.Windows.Forms.RichTextBox();
      this.tabPage_imgArea = new System.Windows.Forms.TabPage();
      this.pictureBox_imgArea = new System.Windows.Forms.PictureBox();
      this.tabPage_image = new System.Windows.Forms.TabPage();
      this.pictureBox_image = new System.Windows.Forms.PictureBox();
      this.tabControl_IP = new System.Windows.Forms.TabControl();
      this.menuStrip.SuspendLayout();
      this.contextMenuStrip_SizeMode.SuspendLayout();
      this.tabPage_Info.SuspendLayout();
      this.tabPage_imgArea.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox_imgArea)).BeginInit();
      this.tabPage_image.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox_image)).BeginInit();
      this.tabControl_IP.SuspendLayout();
      this.SuspendLayout();
      // 
      // menuStrip
      // 
      this.menuStrip.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.menuStrip.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
      this.menuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
      this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dateiToolStripMenuItem});
      this.menuStrip.Location = new System.Drawing.Point(0, 0);
      this.menuStrip.Name = "menuStrip";
      this.menuStrip.Size = new System.Drawing.Size(893, 39);
      this.menuStrip.TabIndex = 0;
      this.menuStrip.Text = "menuStrip";
      // 
      // dateiToolStripMenuItem
      // 
      this.dateiToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bildLadenToolStripMenuItem,
            this.bildSpeichernToolStripMenuItem,
            this.bildToolStripMenuItem,
            this.toolStripSeparator2,
            this.beendenToolStripMenuItem});
      this.dateiToolStripMenuItem.Name = "dateiToolStripMenuItem";
      this.dateiToolStripMenuItem.Size = new System.Drawing.Size(84, 35);
      this.dateiToolStripMenuItem.Text = "Datei";
      // 
      // bildLadenToolStripMenuItem
      // 
      this.bildLadenToolStripMenuItem.Name = "bildLadenToolStripMenuItem";
      this.bildLadenToolStripMenuItem.Size = new System.Drawing.Size(358, 40);
      this.bildLadenToolStripMenuItem.Text = "Bild laden";
      this.bildLadenToolStripMenuItem.Click += new System.EventHandler(this.bildLadenToolStripMenuItem_Click);
      // 
      // bildSpeichernToolStripMenuItem
      // 
      this.bildSpeichernToolStripMenuItem.Name = "bildSpeichernToolStripMenuItem";
      this.bildSpeichernToolStripMenuItem.Size = new System.Drawing.Size(358, 40);
      this.bildSpeichernToolStripMenuItem.Text = "Bild speichern";
      this.bildSpeichernToolStripMenuItem.Click += new System.EventHandler(this.bildSpeichernToolStripMenuItem_Click);
      // 
      // bildToolStripMenuItem
      // 
      this.bildToolStripMenuItem.Name = "bildToolStripMenuItem";
      this.bildToolStripMenuItem.Size = new System.Drawing.Size(358, 40);
      this.bildToolStripMenuItem.Text = "Bild der Seite speichern";
      this.bildToolStripMenuItem.ToolTipText = "Save the image of the page";
      this.bildToolStripMenuItem.Click += new System.EventHandler(this.bildToolStripMenuItem_Click);
      // 
      // toolStripSeparator2
      // 
      this.toolStripSeparator2.Name = "toolStripSeparator2";
      this.toolStripSeparator2.Size = new System.Drawing.Size(355, 6);
      // 
      // beendenToolStripMenuItem
      // 
      this.beendenToolStripMenuItem.Name = "beendenToolStripMenuItem";
      this.beendenToolStripMenuItem.Size = new System.Drawing.Size(358, 40);
      this.beendenToolStripMenuItem.Text = "Beenden";
      this.beendenToolStripMenuItem.Click += new System.EventHandler(this.beendenToolStripMenuItem_Click);
      // 
      // contextMenuStrip_SizeMode
      // 
      this.contextMenuStrip_SizeMode.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.contextMenuStrip_SizeMode.ImageScalingSize = new System.Drawing.Size(20, 20);
      this.contextMenuStrip_SizeMode.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.einpassenToolStripMenuItem,
            this.streckenToolStripMenuItem});
      this.contextMenuStrip_SizeMode.Name = "contextMenuStrip_SizeMode";
      this.contextMenuStrip_SizeMode.Size = new System.Drawing.Size(191, 80);
      // 
      // einpassenToolStripMenuItem
      // 
      this.einpassenToolStripMenuItem.Name = "einpassenToolStripMenuItem";
      this.einpassenToolStripMenuItem.Size = new System.Drawing.Size(190, 38);
      this.einpassenToolStripMenuItem.Text = "Einpassen";
      this.einpassenToolStripMenuItem.Click += new System.EventHandler(this.einpassenToolStripMenuItem_Click);
      // 
      // streckenToolStripMenuItem
      // 
      this.streckenToolStripMenuItem.Name = "streckenToolStripMenuItem";
      this.streckenToolStripMenuItem.Size = new System.Drawing.Size(190, 38);
      this.streckenToolStripMenuItem.Text = "Strecken";
      this.streckenToolStripMenuItem.Click += new System.EventHandler(this.streckenToolStripMenuItem_Click);
      // 
      // img3DImgToolStripMenuItem
      // 
      this.img3DImgToolStripMenuItem.Name = "img3DImgToolStripMenuItem";
      this.img3DImgToolStripMenuItem.Size = new System.Drawing.Size(270, 34);
      this.img3DImgToolStripMenuItem.Text = "Img-3D-Img";
      this.img3DImgToolStripMenuItem.ToolTipText = "Transformiert Bitmap in ein 3D-Array und wieder zurück in ein Bild.";
      // 
      // img1DImgToolStripMenuItem
      // 
      this.img1DImgToolStripMenuItem.Name = "img1DImgToolStripMenuItem";
      this.img1DImgToolStripMenuItem.Size = new System.Drawing.Size(270, 34);
      this.img1DImgToolStripMenuItem.Text = "Img-1D-Img";
      this.img1DImgToolStripMenuItem.ToolTipText = "Transformiert ein Bild in ein eindimensionales feld und wieder zurück in ein  Bil" +
    "d.";
      // 
      // tabPage_Info
      // 
      this.tabPage_Info.Controls.Add(this.richTextBox_Info);
      this.tabPage_Info.Location = new System.Drawing.Point(4, 38);
      this.tabPage_Info.Name = "tabPage_Info";
      this.tabPage_Info.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage_Info.Size = new System.Drawing.Size(885, 599);
      this.tabPage_Info.TabIndex = 2;
      this.tabPage_Info.Text = "Infos";
      this.tabPage_Info.ToolTipText = "Gibt Informationen zu den Bildern aus.";
      this.tabPage_Info.UseVisualStyleBackColor = true;
      // 
      // richTextBox_Info
      // 
      this.richTextBox_Info.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
      this.richTextBox_Info.Dock = System.Windows.Forms.DockStyle.Fill;
      this.richTextBox_Info.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.richTextBox_Info.Location = new System.Drawing.Point(3, 3);
      this.richTextBox_Info.Name = "richTextBox_Info";
      this.richTextBox_Info.Size = new System.Drawing.Size(879, 593);
      this.richTextBox_Info.TabIndex = 0;
      this.richTextBox_Info.Text = "";
      // 
      // tabPage_imgArea
      // 
      this.tabPage_imgArea.Controls.Add(this.pictureBox_imgArea);
      this.tabPage_imgArea.Location = new System.Drawing.Point(4, 38);
      this.tabPage_imgArea.Name = "tabPage_imgArea";
      this.tabPage_imgArea.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage_imgArea.Size = new System.Drawing.Size(885, 599);
      this.tabPage_imgArea.TabIndex = 1;
      this.tabPage_imgArea.Tag = "Zeigt den markierten Bildausschnitt.";
      this.tabPage_imgArea.Text = "Bildbereich";
      this.tabPage_imgArea.UseVisualStyleBackColor = true;
      // 
      // pictureBox_imgArea
      // 
      this.pictureBox_imgArea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
      this.pictureBox_imgArea.ContextMenuStrip = this.contextMenuStrip_SizeMode;
      this.pictureBox_imgArea.Dock = System.Windows.Forms.DockStyle.Fill;
      this.pictureBox_imgArea.Location = new System.Drawing.Point(3, 3);
      this.pictureBox_imgArea.Name = "pictureBox_imgArea";
      this.pictureBox_imgArea.Size = new System.Drawing.Size(879, 593);
      this.pictureBox_imgArea.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
      this.pictureBox_imgArea.TabIndex = 1;
      this.pictureBox_imgArea.TabStop = false;
      // 
      // tabPage_image
      // 
      this.tabPage_image.Controls.Add(this.pictureBox_image);
      this.tabPage_image.Location = new System.Drawing.Point(4, 38);
      this.tabPage_image.Name = "tabPage_image";
      this.tabPage_image.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage_image.Size = new System.Drawing.Size(885, 599);
      this.tabPage_image.TabIndex = 0;
      this.tabPage_image.Tag = "Zeigt das Standbild.";
      this.tabPage_image.Text = "Bild";
      this.tabPage_image.UseVisualStyleBackColor = true;
      // 
      // pictureBox_image
      // 
      this.pictureBox_image.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
      this.pictureBox_image.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
      this.pictureBox_image.Dock = System.Windows.Forms.DockStyle.Fill;
      this.pictureBox_image.Image = global::ImageProcessing.Properties.Resources.Testbild_7x10_RGB;
      this.pictureBox_image.ImageLocation = "";
      this.pictureBox_image.InitialImage = null;
      this.pictureBox_image.Location = new System.Drawing.Point(3, 3);
      this.pictureBox_image.Name = "pictureBox_image";
      this.pictureBox_image.Size = new System.Drawing.Size(879, 593);
      this.pictureBox_image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
      this.pictureBox_image.TabIndex = 0;
      this.pictureBox_image.TabStop = false;
      this.pictureBox_image.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_Bild_MouseClick);
      this.pictureBox_image.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_Bild_MouseDown);
      this.pictureBox_image.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox_Bild_MouseMove);
      this.pictureBox_image.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox_Bild_MouseUp);
      // 
      // tabControl_IP
      // 
      this.tabControl_IP.Controls.Add(this.tabPage_image);
      this.tabControl_IP.Controls.Add(this.tabPage_imgArea);
      this.tabControl_IP.Controls.Add(this.tabPage_Info);
      this.tabControl_IP.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl_IP.Location = new System.Drawing.Point(0, 39);
      this.tabControl_IP.Multiline = true;
      this.tabControl_IP.Name = "tabControl_IP";
      this.tabControl_IP.SelectedIndex = 0;
      this.tabControl_IP.Size = new System.Drawing.Size(893, 641);
      this.tabControl_IP.TabIndex = 1;
      // 
      // Form_ImageProcessing
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 29F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(893, 680);
      this.Controls.Add(this.tabControl_IP);
      this.Controls.Add(this.menuStrip);
      this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.MainMenuStrip = this.menuStrip;
      this.Margin = new System.Windows.Forms.Padding(4);
      this.Name = "Form_ImageProcessing";
      this.Text = "Bildverarbeitung";
      this.menuStrip.ResumeLayout(false);
      this.menuStrip.PerformLayout();
      this.contextMenuStrip_SizeMode.ResumeLayout(false);
      this.tabPage_Info.ResumeLayout(false);
      this.tabPage_imgArea.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox_imgArea)).EndInit();
      this.tabPage_image.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox_image)).EndInit();
      this.tabControl_IP.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.MenuStrip menuStrip;
    private System.Windows.Forms.ToolStripMenuItem dateiToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem bildLadenToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem bildSpeichernToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem beendenToolStripMenuItem;
    private System.Windows.Forms.ContextMenuStrip contextMenuStrip_SizeMode;
    private System.Windows.Forms.ToolStripMenuItem einpassenToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem streckenToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem bildToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    private System.Windows.Forms.ToolStripMenuItem img3DImgToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem img1DImgToolStripMenuItem;
    private System.Windows.Forms.TabPage tabPage_Info;
    private System.Windows.Forms.RichTextBox richTextBox_Info;
    private System.Windows.Forms.TabPage tabPage_imgArea;
    private System.Windows.Forms.PictureBox pictureBox_imgArea;
    private System.Windows.Forms.TabPage tabPage_image;
    private System.Windows.Forms.PictureBox pictureBox_image;
    private System.Windows.Forms.TabControl tabControl_IP;
  }
}

