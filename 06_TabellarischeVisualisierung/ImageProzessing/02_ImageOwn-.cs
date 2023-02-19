using System;
using System.Drawing;             // Bitmap
using System.Drawing.Imaging; // zur Bildverarbeitung
using System.IO; // zum extrahieren des Dateityps
using form = System.Windows.Forms;  // Komponenten

namespace ImageProcessing {
  class OwnImage {

    #region 1. Instanzen und Konstruktor
    private form.OpenFileDialog openFileDialog = new form.OpenFileDialog();
    private form.SaveFileDialog saveFileDialog = new form.SaveFileDialog();
    private form.PictureBox pictureBox;
    private Bitmap bitmap;  // Basisbild
    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="pictureBox"></param> Bildbox des geladenen Bildes
    public OwnImage(form.PictureBox pictureBox) {
      this.pictureBox = pictureBox;
    }
    #endregion

    #region 3. Komponentenmethoden
    /// <summary>
    /// 3.1 Laden eines Bildes aus einer Datei.
    /// Der Pfad wird abgefragt
    /// </summary>
    /// <returns></returns> Bild
    public Bitmap openBitmapFile() {
      // Der Dialog zum Öffnen einer Datei wird ausgeführt.
      if (openFileDialog.ShowDialog() == form.DialogResult.OK) {
        // Bild wird an das Bildobjekt übergeben.
        this.bitmap = (Bitmap)Bitmap.FromFile(openFileDialog.FileName);
        // Bild in der Bildbox des Formulars dargestellt
        pictureBox.Image = new Bitmap(this.bitmap);
        // Einpassen des Bildes, 
        // notwendig für korrekte Maßstabsberechnung
        pictureBox.SizeMode = form.PictureBoxSizeMode.StretchImage;
      }
      return this.bitmap;
    }

    /// <summary>
    /// 3.2 Speichert ein Bitmap nach Abfrage des Pfades
    /// und Angabe des Dateityps.
    /// </summary>
    /// <param name="bitmap"></param>
    public void saveBitmapFile(Bitmap bitmap) {
      // Filter für Dateitypen
      saveFileDialog.Filter = "alle Dateien(*.*) | *.*| Bild(*.bmp) | *.bmp | Bild(*.jpg) | *.jpg | Bild(*.png) | *.png";
      // Dialog zum Speichern des Bildes
      if (saveFileDialog.ShowDialog() == form.DialogResult.OK) {
        // Ermittelt das ausgewählte Bildformat (-> Eigene Klasse)
        ImageFormat imageFormat = imgExtToFormat(saveFileDialog.FileName);
        // Speichert das Bild in dem gewählten Format
        bitmap.Save(saveFileDialog.FileName, imageFormat);
      }
    }
    #endregion Komponentenmethoden

    #region 4. Methoden
    /// <summary> 
    /// 4.1: Konvertiert einen Bildtyp als String in das äquivalenten Bildformat
    /// </summary>
    /// <param name="fileName"></param> Dateipfad, Dateiname
    /// <returns></returns> Bildformat
    public ImageFormat imgExtToFormat(string fileName) {
      // Extrahiert den Dateityp aus dem Dateipfad (System.IO)
      string fileExtension = Path.GetExtension(fileName);
      // Neue Instanz für ein Bild-Format
      ImageFormat imageFormat = new ImageFormat(Guid.NewGuid());
      // Standardbildtyp
      // Auswahl des äquivalenten Bildformats
      switch (fileExtension) {
        case ".bmp": { imageFormat = ImageFormat.Bmp; break; }
        case ".jpg": { imageFormat = ImageFormat.Jpeg; break; }
        case ".png": { imageFormat = ImageFormat.Png; break; }
        case ".gif": { imageFormat = ImageFormat.Gif; break; }
        case ".tif": { imageFormat = ImageFormat.Tiff; break; }
        case ".ico": { imageFormat = ImageFormat.Icon; break; }
        case ".emf": { imageFormat = ImageFormat.Emf; break; }
        case ".wmf": { imageFormat = ImageFormat.Wmf; break; }
        case ".exif": { imageFormat = ImageFormat.Exif; break; }
      }
      return imageFormat;
    }

    /// <summary>
    /// 4.2. Liefert einen Bildausschnitt eines Bitmaps, der zuvor in einer
    /// PictureBox umrahmt wurde. Entsprechend der Größenverhältnisse
    /// erfolgt eine Maßstabsumrechgung des Rahmens.
    /// </summary>
    /// <param name="bitmap"></param> Basisbild
    /// <param name="box"></param> Rahmen in der PictureBox
    /// <returns></returns> Bildausschnitt
    public Bitmap subBitmap(Bitmap bitmap, Rectangle box) {
      // Bild muss gestreckt sein.
      this.pictureBox.SizeMode = form.PictureBoxSizeMode.StretchImage;
      Rectangle bitmapRect = new Rectangle();
      // Umrechnungsmaßstab
      double Mx = (double)(bitmap.Width) / this.pictureBox.Width;
      double My = (double)(bitmap.Height) / this.pictureBox.Height;
      // Bereich des Bildes berechnen
      bitmapRect.X = (int)(box.X * Mx);
      bitmapRect.Y = (int)(box.Y * My);
      bitmapRect.Width = (int)(box.Width * Mx);
      bitmapRect.Height = (int)(box.Height * My);
      // Bildausschnitt kopieren
      Bitmap subBitmap = new Bitmap(bitmapRect.Width, bitmapRect.Height);
      for (int x = 0; x < bitmapRect.Width; x++)
        for (int y = 0; y < bitmapRect.Height; y++)
          subBitmap.SetPixel(x, y, bitmap.GetPixel(bitmapRect.X + x, bitmapRect.Y + y));
      return subBitmap;
    }
    #endregion

    #region 5. Eigenschaftsmethoden
    /// <summary>
    /// 5.1 Liefert das Basisbild (Standbild)
    /// </summary>
    public Bitmap getBitmap {
      get { return this.bitmap; }
    }
    #endregion Eigenschaftsmethoden

  } // Klasse
}  //Namespace
