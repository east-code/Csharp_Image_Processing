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
      this.kameraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.shotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.extrasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.bildtabelleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
      this.tabPage_image = new System.Windows.Forms.TabPage();
      this.pictureBox_image = new System.Windows.Forms.PictureBox();
      this.tabControl_IP = new System.Windows.Forms.TabControl();
      this.tabPage_Cam = new System.Windows.Forms.TabPage();
      this.splitContainer_Cam = new System.Windows.Forms.SplitContainer();
      this.button_Shot = new System.Windows.Forms.Button();
      this.label_Size = new System.Windows.Forms.Label();
      this.label_Cam = new System.Windows.Forms.Label();
      this.comboBox_Capabilities = new System.Windows.Forms.ComboBox();
      this.comboBox_Device = new System.Windows.Forms.ComboBox();
      this.panel_Cam = new System.Windows.Forms.Panel();
      this.tabPage_imgArea = new System.Windows.Forms.TabPage();
      this.pictureBox_imgArea = new System.Windows.Forms.PictureBox();
      this.contextMenuStrip_SizeMode = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.einpassenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.streckenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.tabPage_imgTable = new System.Windows.Forms.TabPage();
      this.dataGridView_imgTable = new System.Windows.Forms.DataGridView();
      this.tabPage_Info = new System.Windows.Forms.TabPage();
      this.richTextBox_Info = new System.Windows.Forms.RichTextBox();
      this.img3DImgToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.img1DImgToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.bildtabelleKanal0ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.bildtabelleKanal1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.bildtabelleKanal2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.bild3DArrayBildToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.bild1DFeldBildToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.tabPage_imgResult = new System.Windows.Forms.TabPage();
      this.pictureBox_imgResult = new System.Windows.Forms.PictureBox();
      this.menuStrip.SuspendLayout();
      this.tabPage_image.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox_image)).BeginInit();
      this.tabControl_IP.SuspendLayout();
      this.tabPage_Cam.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer_Cam)).BeginInit();
      this.splitContainer_Cam.Panel1.SuspendLayout();
      this.splitContainer_Cam.Panel2.SuspendLayout();
      this.splitContainer_Cam.SuspendLayout();
      this.tabPage_imgArea.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox_imgArea)).BeginInit();
      this.contextMenuStrip_SizeMode.SuspendLayout();
      this.tabPage_imgTable.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView_imgTable)).BeginInit();
      this.tabPage_Info.SuspendLayout();
      this.tabPage_imgResult.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox_imgResult)).BeginInit();
      this.SuspendLayout();
      // 
      // menuStrip
      // 
      this.menuStrip.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.menuStrip.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
      this.menuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
      this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dateiToolStripMenuItem,
            this.kameraToolStripMenuItem,
            this.extrasToolStripMenuItem});
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
      // kameraToolStripMenuItem
      // 
      this.kameraToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.shotToolStripMenuItem});
      this.kameraToolStripMenuItem.Name = "kameraToolStripMenuItem";
      this.kameraToolStripMenuItem.Size = new System.Drawing.Size(122, 35);
      this.kameraToolStripMenuItem.Text = "WebCam";
      // 
      // startToolStripMenuItem
      // 
      this.startToolStripMenuItem.Name = "startToolStripMenuItem";
      this.startToolStripMenuItem.Size = new System.Drawing.Size(165, 40);
      this.startToolStripMenuItem.Text = "Start";
      this.startToolStripMenuItem.Click += new System.EventHandler(this.startToolStripMenuItem_Click);
      // 
      // stopToolStripMenuItem
      // 
      this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
      this.stopToolStripMenuItem.Size = new System.Drawing.Size(165, 40);
      this.stopToolStripMenuItem.Text = "Stop";
      this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
      // 
      // shotToolStripMenuItem
      // 
      this.shotToolStripMenuItem.Name = "shotToolStripMenuItem";
      this.shotToolStripMenuItem.Size = new System.Drawing.Size(165, 40);
      this.shotToolStripMenuItem.Text = "Shot";
      this.shotToolStripMenuItem.Click += new System.EventHandler(this.shotToolStripMenuItem_Click);
      // 
      // extrasToolStripMenuItem
      // 
      this.extrasToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bildtabelleToolStripMenuItem,
            this.toolStripSeparator3,
            this.bildtabelleKanal0ToolStripMenuItem,
            this.bildtabelleKanal1ToolStripMenuItem,
            this.bildtabelleKanal2ToolStripMenuItem,
            this.toolStripSeparator1,
            this.bild3DArrayBildToolStripMenuItem,
            this.bild1DFeldBildToolStripMenuItem});
      this.extrasToolStripMenuItem.Name = "extrasToolStripMenuItem";
      this.extrasToolStripMenuItem.Size = new System.Drawing.Size(91, 35);
      this.extrasToolStripMenuItem.Text = "Extras";
      // 
      // bildtabelleToolStripMenuItem
      // 
      this.bildtabelleToolStripMenuItem.Name = "bildtabelleToolStripMenuItem";
      this.bildtabelleToolStripMenuItem.Size = new System.Drawing.Size(349, 40);
      this.bildtabelleToolStripMenuItem.Text = "Bildtabelle";
      this.bildtabelleToolStripMenuItem.ToolTipText = "Zeigt tabellarisch die Pixel eines Bildes mit Farbcodierung ";
      this.bildtabelleToolStripMenuItem.Click += new System.EventHandler(this.bildtabelleToolStripMenuItem_Click);
      // 
      // toolStripSeparator3
      // 
      this.toolStripSeparator3.Name = "toolStripSeparator3";
      this.toolStripSeparator3.Size = new System.Drawing.Size(346, 6);
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
      this.tabControl_IP.Controls.Add(this.tabPage_Cam);
      this.tabControl_IP.Controls.Add(this.tabPage_image);
      this.tabControl_IP.Controls.Add(this.tabPage_imgArea);
      this.tabControl_IP.Controls.Add(this.tabPage_imgResult);
      this.tabControl_IP.Controls.Add(this.tabPage_imgTable);
      this.tabControl_IP.Controls.Add(this.tabPage_Info);
      this.tabControl_IP.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl_IP.Location = new System.Drawing.Point(0, 39);
      this.tabControl_IP.Multiline = true;
      this.tabControl_IP.Name = "tabControl_IP";
      this.tabControl_IP.SelectedIndex = 0;
      this.tabControl_IP.Size = new System.Drawing.Size(893, 641);
      this.tabControl_IP.TabIndex = 1;
      // 
      // tabPage_Cam
      // 
      this.tabPage_Cam.Controls.Add(this.splitContainer_Cam);
      this.tabPage_Cam.Location = new System.Drawing.Point(4, 38);
      this.tabPage_Cam.Name = "tabPage_Cam";
      this.tabPage_Cam.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage_Cam.Size = new System.Drawing.Size(885, 599);
      this.tabPage_Cam.TabIndex = 3;
      this.tabPage_Cam.Text = "Cam";
      this.tabPage_Cam.ToolTipText = "Zeigt den Kamera-Stream.";
      this.tabPage_Cam.UseVisualStyleBackColor = true;
      // 
      // splitContainer_Cam
      // 
      this.splitContainer_Cam.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer_Cam.Location = new System.Drawing.Point(3, 3);
      this.splitContainer_Cam.Name = "splitContainer_Cam";
      this.splitContainer_Cam.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer_Cam.Panel1
      // 
      this.splitContainer_Cam.Panel1.Controls.Add(this.button_Shot);
      this.splitContainer_Cam.Panel1.Controls.Add(this.label_Size);
      this.splitContainer_Cam.Panel1.Controls.Add(this.label_Cam);
      this.splitContainer_Cam.Panel1.Controls.Add(this.comboBox_Capabilities);
      this.splitContainer_Cam.Panel1.Controls.Add(this.comboBox_Device);
      // 
      // splitContainer_Cam.Panel2
      // 
      this.splitContainer_Cam.Panel2.Controls.Add(this.panel_Cam);
      this.splitContainer_Cam.Size = new System.Drawing.Size(879, 593);
      this.splitContainer_Cam.SplitterDistance = 93;
      this.splitContainer_Cam.TabIndex = 2;
      // 
      // button_Shot
      // 
      this.button_Shot.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this.button_Shot.Location = new System.Drawing.Point(521, 14);
      this.button_Shot.Name = "button_Shot";
      this.button_Shot.Size = new System.Drawing.Size(123, 54);
      this.button_Shot.TabIndex = 21;
      this.button_Shot.Text = "Shot";
      this.button_Shot.UseVisualStyleBackColor = true;
      this.button_Shot.Click += new System.EventHandler(this.shotToolStripMenuItem_Click);
      // 
      // label_Size
      // 
      this.label_Size.AutoSize = true;
      this.label_Size.ForeColor = System.Drawing.Color.Black;
      this.label_Size.Location = new System.Drawing.Point(333, 8);
      this.label_Size.Name = "label_Size";
      this.label_Size.Size = new System.Drawing.Size(119, 29);
      this.label_Size.TabIndex = 20;
      this.label_Size.Text = "Auflösung";
      // 
      // label_Cam
      // 
      this.label_Cam.AutoSize = true;
      this.label_Cam.ForeColor = System.Drawing.Color.Black;
      this.label_Cam.Location = new System.Drawing.Point(29, 8);
      this.label_Cam.Name = "label_Cam";
      this.label_Cam.Size = new System.Drawing.Size(63, 29);
      this.label_Cam.TabIndex = 19;
      this.label_Cam.Text = "Cam";
      // 
      // comboBox_Capabilities
      // 
      this.comboBox_Capabilities.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
      this.comboBox_Capabilities.ForeColor = System.Drawing.Color.Black;
      this.comboBox_Capabilities.FormattingEnabled = true;
      this.comboBox_Capabilities.Location = new System.Drawing.Point(338, 40);
      this.comboBox_Capabilities.Name = "comboBox_Capabilities";
      this.comboBox_Capabilities.Size = new System.Drawing.Size(148, 37);
      this.comboBox_Capabilities.TabIndex = 18;
      // 
      // comboBox_Device
      // 
      this.comboBox_Device.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
      this.comboBox_Device.ForeColor = System.Drawing.Color.Black;
      this.comboBox_Device.FormattingEnabled = true;
      this.comboBox_Device.Location = new System.Drawing.Point(23, 40);
      this.comboBox_Device.Name = "comboBox_Device";
      this.comboBox_Device.Size = new System.Drawing.Size(286, 37);
      this.comboBox_Device.TabIndex = 17;
      this.comboBox_Device.SelectedIndexChanged += new System.EventHandler(this.comboBox_Device_SelectedIndexChanged);
      // 
      // panel_Cam
      // 
      this.panel_Cam.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
      this.panel_Cam.Dock = System.Windows.Forms.DockStyle.Fill;
      this.panel_Cam.Location = new System.Drawing.Point(0, 0);
      this.panel_Cam.Name = "panel_Cam";
      this.panel_Cam.Size = new System.Drawing.Size(879, 496);
      this.panel_Cam.TabIndex = 2;
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
      // tabPage_imgTable
      // 
      this.tabPage_imgTable.Controls.Add(this.dataGridView_imgTable);
      this.tabPage_imgTable.Location = new System.Drawing.Point(4, 38);
      this.tabPage_imgTable.Name = "tabPage_imgTable";
      this.tabPage_imgTable.Size = new System.Drawing.Size(885, 599);
      this.tabPage_imgTable.TabIndex = 4;
      this.tabPage_imgTable.Text = "Bildtabelle";
      this.tabPage_imgTable.ToolTipText = "Zeigt die tabellarische Darstellung eines Bildes.";
      this.tabPage_imgTable.UseVisualStyleBackColor = true;
      // 
      // dataGridView_imgTable
      // 
      this.dataGridView_imgTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView_imgTable.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dataGridView_imgTable.Location = new System.Drawing.Point(0, 0);
      this.dataGridView_imgTable.Name = "dataGridView_imgTable";
      this.dataGridView_imgTable.RowHeadersWidth = 51;
      this.dataGridView_imgTable.RowTemplate.Height = 24;
      this.dataGridView_imgTable.Size = new System.Drawing.Size(885, 599);
      this.dataGridView_imgTable.TabIndex = 0;
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
      // bildtabelleKanal0ToolStripMenuItem
      // 
      this.bildtabelleKanal0ToolStripMenuItem.Name = "bildtabelleKanal0ToolStripMenuItem";
      this.bildtabelleKanal0ToolStripMenuItem.Size = new System.Drawing.Size(349, 40);
      this.bildtabelleKanal0ToolStripMenuItem.Text = "Bildtabelle, Kanal 0";
      this.bildtabelleKanal0ToolStripMenuItem.Click += new System.EventHandler(this.bildtabelleKanal0ToolStripMenuItem_Click);
      // 
      // bildtabelleKanal1ToolStripMenuItem
      // 
      this.bildtabelleKanal1ToolStripMenuItem.Name = "bildtabelleKanal1ToolStripMenuItem";
      this.bildtabelleKanal1ToolStripMenuItem.Size = new System.Drawing.Size(349, 40);
      this.bildtabelleKanal1ToolStripMenuItem.Text = "Bildtabelle, Kanal 1";
      this.bildtabelleKanal1ToolStripMenuItem.Click += new System.EventHandler(this.bildtabelleKanal1ToolStripMenuItem_Click);
      // 
      // bildtabelleKanal2ToolStripMenuItem
      // 
      this.bildtabelleKanal2ToolStripMenuItem.Name = "bildtabelleKanal2ToolStripMenuItem";
      this.bildtabelleKanal2ToolStripMenuItem.Size = new System.Drawing.Size(349, 40);
      this.bildtabelleKanal2ToolStripMenuItem.Text = "Bildtabelle, Kanal 2";
      this.bildtabelleKanal2ToolStripMenuItem.Click += new System.EventHandler(this.bildtabelleKanal2ToolStripMenuItem_Click);
      // 
      // toolStripSeparator1
      // 
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new System.Drawing.Size(346, 6);
      // 
      // bild3DArrayBildToolStripMenuItem
      // 
      this.bild3DArrayBildToolStripMenuItem.Name = "bild3DArrayBildToolStripMenuItem";
      this.bild3DArrayBildToolStripMenuItem.Size = new System.Drawing.Size(349, 40);
      this.bild3DArrayBildToolStripMenuItem.Text = "Bild -> 3D-Array->Bild";
      // 
      // bild1DFeldBildToolStripMenuItem
      // 
      this.bild1DFeldBildToolStripMenuItem.Name = "bild1DFeldBildToolStripMenuItem";
      this.bild1DFeldBildToolStripMenuItem.Size = new System.Drawing.Size(349, 40);
      this.bild1DFeldBildToolStripMenuItem.Text = "Bild->1D-Feld->Bild";
      // 
      // tabPage_imgResult
      // 
      this.tabPage_imgResult.Controls.Add(this.pictureBox_imgResult);
      this.tabPage_imgResult.Location = new System.Drawing.Point(4, 38);
      this.tabPage_imgResult.Name = "tabPage_imgResult";
      this.tabPage_imgResult.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage_imgResult.Size = new System.Drawing.Size(885, 599);
      this.tabPage_imgResult.TabIndex = 5;
      this.tabPage_imgResult.Text = "Ergebnisbild";
      this.tabPage_imgResult.UseVisualStyleBackColor = true;
      // 
      // pictureBox_imgResult
      // 
      this.pictureBox_imgResult.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
      this.pictureBox_imgResult.ContextMenuStrip = this.contextMenuStrip_SizeMode;
      this.pictureBox_imgResult.Dock = System.Windows.Forms.DockStyle.Fill;
      this.pictureBox_imgResult.Location = new System.Drawing.Point(3, 3);
      this.pictureBox_imgResult.Name = "pictureBox_imgResult";
      this.pictureBox_imgResult.Size = new System.Drawing.Size(879, 593);
      this.pictureBox_imgResult.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
      this.pictureBox_imgResult.TabIndex = 2;
      this.pictureBox_imgResult.TabStop = false;
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
      this.tabPage_image.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox_image)).EndInit();
      this.tabControl_IP.ResumeLayout(false);
      this.tabPage_Cam.ResumeLayout(false);
      this.splitContainer_Cam.Panel1.ResumeLayout(false);
      this.splitContainer_Cam.Panel1.PerformLayout();
      this.splitContainer_Cam.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer_Cam)).EndInit();
      this.splitContainer_Cam.ResumeLayout(false);
      this.tabPage_imgArea.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox_imgArea)).EndInit();
      this.contextMenuStrip_SizeMode.ResumeLayout(false);
      this.tabPage_imgTable.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView_imgTable)).EndInit();
      this.tabPage_Info.ResumeLayout(false);
      this.tabPage_imgResult.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox_imgResult)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.MenuStrip menuStrip;
    private System.Windows.Forms.ToolStripMenuItem dateiToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem bildLadenToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem bildSpeichernToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem beendenToolStripMenuItem;
		private System.Windows.Forms.TabPage tabPage_image;
		private System.Windows.Forms.PictureBox pictureBox_image;
		private System.Windows.Forms.TabControl tabControl_IP;
    private System.Windows.Forms.TabPage tabPage_imgArea;
    private System.Windows.Forms.PictureBox pictureBox_imgArea;
    private System.Windows.Forms.ContextMenuStrip contextMenuStrip_SizeMode;
    private System.Windows.Forms.ToolStripMenuItem einpassenToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem streckenToolStripMenuItem;
    private System.Windows.Forms.TabPage tabPage_Info;
    private System.Windows.Forms.RichTextBox richTextBox_Info;
    private System.Windows.Forms.ToolStripMenuItem kameraToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem shotToolStripMenuItem;
    private System.Windows.Forms.TabPage tabPage_Cam;
    private System.Windows.Forms.SplitContainer splitContainer_Cam;
    private System.Windows.Forms.ComboBox comboBox_Device;
    private System.Windows.Forms.Panel panel_Cam;
    private System.Windows.Forms.Label label_Size;
    private System.Windows.Forms.Label label_Cam;
    private System.Windows.Forms.ComboBox comboBox_Capabilities;
    private System.Windows.Forms.Button button_Shot;
    private System.Windows.Forms.TabPage tabPage_imgTable;
    private System.Windows.Forms.DataGridView dataGridView_imgTable;
    private System.Windows.Forms.ToolStripMenuItem bildToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    private System.Windows.Forms.ToolStripMenuItem extrasToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem bildtabelleToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem img3DImgToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem img1DImgToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    private System.Windows.Forms.ToolStripMenuItem bildtabelleKanal0ToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem bildtabelleKanal1ToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem bildtabelleKanal2ToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.ToolStripMenuItem bild3DArrayBildToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem bild1DFeldBildToolStripMenuItem;
    private System.Windows.Forms.TabPage tabPage_imgResult;
    private System.Windows.Forms.PictureBox pictureBox_imgResult;
  }
}

