using System;
using System.Drawing;
using System.Windows.Forms;

namespace ImageProcessing {
  public partial class Form_ImageProcessing : Form {

    #region 01. Instanzen, Attribute, Konstruktor
    /// <summary>
    /// 01.1 Attribute, Instanzen
    /// </summary>
    OutputOwn output;         // Ausgabemethoden
    ImageOwn image;          // Laden und Speichern von Bildern
    Graphics graphics;       // Zeichnen
    Bitmap bitmap = new Bitmap(ImageProcessing.Properties.Resources.Testbild_7x10_RGB);   // Eingangsbild
    Rectangle rahmenInPictureBox; // Rahmenkoordinaten 
    Bitmap subBitmap; // Bildbereich, Rahmenkoordinaten

    /// <summary>
    /// 01.2 Konstruktor, Instanzen initialisieren
    /// </summary>
    public Form_ImageProcessing() {
      InitializeComponent();
      // Übergibt Komponenten des Formulars.
      output = new OutputOwn(richTextBox_Info);

      image = new ImageOwn(this.pictureBox_image);
      // Externe Klassen
      graphics = pictureBox_image.CreateGraphics();
      // Startbedingungen, Informationen zum Testbild
      richTextBox_Info.Text = "Testbild: ImageProcessing.Properties.Resources.Testbild_7x10_RGB\n\n";
      // Registerseite nach Programmstart
      tabControl_IP.SelectedTab = tabPage_image;

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
      string pageName = tabControl_IP.SelectedTab.Name.ToString();
      Bitmap bmp = new Bitmap(1, 1);
      switch (pageName) {
        case "tabPage_Bild":
          bmp = (Bitmap)pictureBox_image.Image;
          break;
        case "tabPage_Bereich":
          bmp = (Bitmap)pictureBox_imgArea.Image;
          break;
      }
      image.saveBitmapFile(bmp);
    }

    /// <summary>
    /// 02.99 Anwendung schließen
    /// </summary>
    private void beendenToolStripMenuItem_Click(object sender, EventArgs e) {
      Close();
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
    /// als Bitmap abgelegt und in einer PicturBox angezeigt.
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


  }
}