using System;
using System.Drawing;
using System.Windows.Forms;
using ARRAY = ImageProcessing.ArrayOwn; // // Statische Methoden zur Array-Verarbeitung 

namespace ImageProcessing {
  public partial class Form_ImageProcessing : Form {

    #region 01. Instanzen, Attribute, Konstruktor
    /// <summary>
    /// 01.1 Attribute, Instanzen
    /// </summary>
    OutpuOwn output;         // Ausgabemethoden
    WebCamOwn cam;              // Einbinden von Kameras 
    ImageOwn image;          // Laden, speichern von Bildern
    ImageTableOwn imgTab;    // Bilder als Tabelle mit Ausgabe des Farbwerte
    Graphics graphics;       // Zeichnen
    Bitmap bitmap = new Bitmap(ImageProcessing.Properties.Resources.Testbild_7x10_RGB);   // Eingangsbild
    Rectangle rahmenInPictureBox; // Rahmenkoordinaten 
    Bitmap subBitmap; // Bildbereich
    Bitmap resultBitmap; // Bearbeitetes, verändertes Image


    /// <summary>
    /// 01.2 Konstruktor, Instanzen initialisieren
    /// </summary>
    public Form_ImageProcessing() {
      InitializeComponent();
      // Übergibt Komponenten des Formulars.
      output = new OutpuOwn(richTextBox_Info);
      cam = new WebCamOwn(comboBox_Device, comboBox_Capabilities, panel_Cam, pictureBox_image);
      image = new ImageOwn(this.pictureBox_image);
      imgTab = new ImageTableOwn(dataGridView_imgTable);
      // Externe Klassen
      graphics = pictureBox_image.CreateGraphics();
      // Angeschlossene Wabcams zusammenstellen
      cam.EnumerationVideoDevices();
      // Startbedingungen, Informationen zum Testbild
      richTextBox_Info.Text = "Testbild: ImageProcessing.Properties.Resources.Testbild_7x10_RGB\n\n";
      // Registerseite nach Programmstart
      tabControl_IP.SelectedTab = tabPage_image;
      EnableConnectionControls(true);
    }
    #endregion

    #region 02. Bild Laden, Speichern
    /// <summary>
    /// 2.1 Bild aus Datei laden und in der PictureBox darstellen.
    /// </summary>
    private void bildLadenToolStripMenuItem_Click(object sender, EventArgs e) {
      this.bitmap = image.openBitmapFile();
      tabControl_IP.SelectedTab = tabPage_image;
      output.bitmapInfo(bitmap, "Geladenes Bitmap");
    }
    /// <summary>
    /// 2.2 Speichert das Bild als Datei in einem beliebigen Format (bmp, jpg, png)
    /// </summary>
    private void bildSpeichernToolStripMenuItem_Click(object sender, EventArgs e) {
      image.saveBitmapFile(this.bitmap);
    }
    /// <summary>
    /// 2.3 Speichert das Bild der aufgeschlagenen Seite des Registers
    /// </summary>
    private void bildToolStripMenuItem_Click(object sender, EventArgs e) {
      // 99. Hilfsmethode: Liefert Bitmap der Picture-Box der aufgeschlagenen Registerseite
      Bitmap bmp = getSelectedPictureBoxImage();
      image.saveBitmapFile(bmp);
    }

    #endregion
    #region 02-1. Mausaktionen für Bildbereich
    /// <summary>
    /// 02-1.1: Linke Maustaste wird gedrückt 
    /// -> Linker oberer Eckpunkt
    /// </summary>
    private void pictureBox_Bild_MouseDown(object sender, MouseEventArgs e) {
      if (e.Button == MouseButtons.Left) {
        //Linke obere Ecke des Rahmens
        rahmenInPictureBox.X = e.Location.X;
        rahmenInPictureBox.Y = e.Location.Y;
      }
    }
    /// <summary>
    /// 02-1.2 Maus von links oben nach rechts unten bewegen 
    /// -> rechter unterer Diagonal-Punkt selektieren und
    /// Rahmen während der Bewegung mitzeichnen
    /// </summary>
    private void pictureBox_Bild_MouseMove(object sender, MouseEventArgs e) {
      Pen pen = new Pen(Color.Red, 3); // Zeichenstift definieren
      if (e.Button == MouseButtons.Left) {
        // Bereichsweite und Bereichshöhe
        // --> Rahmen ziehen nach rechts unten
        rahmenInPictureBox.Width = e.Location.X - rahmenInPictureBox.X;
        rahmenInPictureBox.Height = e.Location.Y - rahmenInPictureBox.Y;
        // Vermeidet Überzeichnungen, Bild neu in der PictureBox darstellen
        if (e.Location.Y % 3 == 0 || e.Location.X % 3 == 0)
          pictureBox_image.Image = new Bitmap(bitmap);
        // Zeichnet neuen Rahmen
        graphics.DrawRectangle(pen, rahmenInPictureBox);
      };
    }

    /// <summary>
    /// 02-1.3 Ermittelt den rechten unteren Eckpunkt des Bereichs
    /// </summary>
    private void pictureBox_Bild_MouseUp(object sender, MouseEventArgs e) {
      Pen pen = new Pen(Color.Yellow, 3);
      if (e.Button == MouseButtons.Left) {
        // Rahmen nach rechts unten gezogen.
        rahmenInPictureBox.Width = e.Location.X - rahmenInPictureBox.X;
        rahmenInPictureBox.Height = e.Location.Y - rahmenInPictureBox.Y;
        graphics.DrawRectangle(pen, rahmenInPictureBox);
      };
    }

    /// <summary>
    /// 02-1.4 Der markierte Bereich wird auf das Originalbild umgerechnet,
    /// als Bitmap abgelegt und in einer Picture-Box angezeigt.
    /// </summary>
    private void pictureBox_Bild_MouseClick(object sender, MouseEventArgs e) {
      Pen pen = new Pen(Color.GreenYellow, 3);
      graphics.DrawRectangle(pen, rahmenInPictureBox);
      subBitmap = this.image.subBitmap(bitmap, rahmenInPictureBox);
      pictureBox_imgArea.Image = subBitmap;
      tabControl_IP.SelectedTab = tabPage_imgArea; // zur Bildbereichsseite
    }
    #endregion Mausaktionen
    #region 02-2. Bild modifizieren
    /// <summary>
    /// 02-2.1 Skaliert die Bitmap, so dass es der Größe der Bildbox entspricht
    /// Das Seitenverhältnis bleibt unverändert.
    /// </summary>
    private void einpassenToolStripMenuItem_Click(object sender, EventArgs e) {
      pictureBox_imgArea.SizeMode = PictureBoxSizeMode.Zoom;
    }
    /// <summary>
    /// 02-2.2 Skaliert die Bitmap auf die Größe der Bildbox ohne Rücksicht auf
    /// das Seitenverhältnis.
    /// </summary>
    private void streckenToolStripMenuItem_Click(object sender, EventArgs e) {
      pictureBox_imgArea.SizeMode = PictureBoxSizeMode.StretchImage;
    }
    #endregion

    #region 03. Kamera
    /// <summary>
    /// 03.1 Ermittelt die verfügbaren Kamera-Auflösungen.
    /// </summary>
    private void comboBox_Device_SelectedIndexChanged(object sender, EventArgs e) {
      cam.VidioDevice();
      cam.EnumerationFrameSizes();
    }
    /// <summary>
    /// 03.2 Starten der Web-Cam
    /// </summary>
    private void startToolStripMenuItem_Click(object sender, EventArgs e) {
      cam.Start();
      tabControl_IP.SelectedTab = tabPage_Cam;  // Registerseite aufschlagen
      EnableConnectionControls(false);
    }
    /// <summary>
    /// 03.3 Stop der Übertragung
    /// </summary>
    private void stopToolStripMenuItem_Click(object sender, EventArgs e) {
      EnableConnectionControls(true);
      cam.Stop();
    }
    /// <summary>
    /// 03.4 Liefert ein Standbild mit Zusatzinformationen
    /// </summary>
    private void shotToolStripMenuItem_Click(object sender, EventArgs e) {
      this.bitmap = cam.shot();   // Instanzmethode zum Fangen eines Bildes aufrufen
      tabControl_IP.SelectedTab = tabPage_image;  // Registerseite aufschlagen
                                                  // Instanzmethode kommentiert den Verlauf zur Kontrolle.
      output.bitmapInfo(this.bitmap, "Standbild");
    }
    /// <summary> Methode
    /// 03.5 Aktivieren/Deaktivieren von Kontrollelementen.
    /// </summary>
    /// <param name="enable"></param> Umschalter
    private void EnableConnectionControls(bool enable) {
      comboBox_Device.Enabled = enable;
      comboBox_Capabilities.Enabled = enable;
      startToolStripMenuItem.Enabled = enable;
      stopToolStripMenuItem.Enabled = !enable;
    }
    #endregion Kamera

    #region 04. Bildtabelle ++++++++++++++++++++++++++++++
    /// <summary>
    /// 04.1 Testen der View-Methode zur Bildtabelle
    /// </summary>
    private void bildtabelleToolStripMenuItem_Click(object sender, EventArgs e) {
      imgTab.viewAsTable(getSelectedBitmap(), ARRAY.toCanals(0, 1, 2), "float");
      tabControl_IP.SelectedIndex = 4;
    }

    /// <summary>
    /// 04.2 Für Kanal 0
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void bildtabelleKanal0ToolStripMenuItem_Click(object sender, EventArgs e) {
      imgTab.viewAsTable(getSelectedBitmap(), ARRAY.toCanals(0), "float");
      tabControl_IP.SelectedIndex = 4;
    }

    /// <summary>
    /// 04.2 Für Kanal 1
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void bildtabelleKanal1ToolStripMenuItem_Click(object sender, EventArgs e) {
      imgTab.viewAsTable(getSelectedBitmap(), ARRAY.toCanals(1), "float");
      tabControl_IP.SelectedIndex = 4;
    }

    /// <summary>
    /// 04.2 Für Kanal 2
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void bildtabelleKanal2ToolStripMenuItem_Click(object sender, EventArgs e) {
      imgTab.viewAsTable(getSelectedBitmap(), ARRAY.toCanals(2), "float");
      tabControl_IP.SelectedIndex = 4;
    }

    #endregion


    // . . . 
    #region 99. Hilfsmethoden ++++++++++++++++++++++++++++++++++++
    /// <summary>
    /// 99.1 Liefert das Bild der aufgeschlagenen Registerseite.
    /// </summary>
    /// <returns></returns> Bild der aufgeschlagenen Seite.
    private Bitmap getSelectedBitmap() {
      switch (tabControl_IP.SelectedIndex) {
        case 1:
          return bitmap; // Basisbild
        case 2:
          return subBitmap; // Bildausschnitt
        case 3:
          return resultBitmap; // Ergebnisbild
      }
      return bitmap;
    }

    /// <summary>
    /// 99.2 Liefert das Bild der Picture-Box der aufgeschlagenen Registerseite.
    /// </summary>
    /// </summary>
    /// <returns></returns> Bild
    private Bitmap getSelectedPictureBoxImage() {
      string pageName = tabControl_IP.SelectedTab.Name.ToString();
      switch (pageName) {
        case "tabPage_Cam":
          return (Bitmap)panel_Cam.BackgroundImage;
        case "tabPage_image":
          return (Bitmap)pictureBox_image.Image;
        case "tabPage_imgArea":
          return (Bitmap)pictureBox_imgArea.Image;
        case "tabPage_imgResult":
          return (Bitmap)pictureBox_imgResult.Image;
      }
      return bitmap;
    }
    #endregion


    #region 100. Beenden
    /// <summary>
    /// 100.1 Anwendung schließen
    /// </summary>
    private void beendenToolStripMenuItem_Click(object sender, EventArgs e) {
      // Stoppt die Darstellung
      stopToolStripMenuItem_Click(sender, e);
      Close();
    }


    #endregion


  }
}