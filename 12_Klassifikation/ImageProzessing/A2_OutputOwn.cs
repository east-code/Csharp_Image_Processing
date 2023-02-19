/* Klasse OutputOwn: Methodensammlung zur Unterstützung der Ausgabe
 * Verfasser: Horst-Christian Heinke
 * Datum: 19.12.2022
 * Letzte Änderung: 15.01.2023
 * Germany, Sachsen-Anhalt, Magdeburg
 * */
using System;
using System.Collections.Generic;
using System.Drawing;
using form = System.Windows.Forms;

namespace ImageProcessing {
  class OutputOwn {  //internal

    #region 1. Attribute, Konstruktor
    // 1.2 Komponenten des Formulars
    form.RichTextBox richTextBox;// Informationen werden hinterlegt.
    form.TabControl tabControl;  // Registerseite wird nach Ausgabe geöffnet
    form.PictureBox pictureBox;  // Ergebnis der Transformation              
    ImageArrayOwn imgArray;      // Instanz: Bilder in Arrays konvertieren und zurück
    Point pos = new Point(4, 4); // Startposition für Grafiktext      // +08

    /// <summary>
    /// 1.3 Konstruktor als Komponentenschnittstelle.
    /// </summary>
    /// <param name="richTextBox"></param> Hinweise, Informationen
    /// <param name="tabControl"></param>  Registerseite zum Öffnen 
    /// <param name="pictureBox"></param>  Bildbox für Ergebnisbild 
    public OutputOwn(form.RichTextBox richTextBox, form.TabControl tabControl, form.PictureBox pictureBox) {
      this.richTextBox = richTextBox;
      this.tabControl = tabControl;
      this.pictureBox = pictureBox;
      imgArray = new ImageArrayOwn();      // Instanzen
    }

    /// <summary> ÜBERLADUNG
    /// 1.3 Konstruktor
    /// </summary>
    public OutputOwn() {
      imgArray = new ImageArrayOwn();      // Instanzen
    }
    #endregion

    #region 2. Ergebnisbild darstellen und Hinweise ausgeben
    /// <summary>
    /// 2.1 Liefert Informationen zu einer Bitmap
    /// </summary>
    /// <param name="bitmap"></param> Bild
    /// <param name="info"></param> Optionale beliebige Bezeichnung.
    public void bitmapInfo(Bitmap bitmap, string info = "") {
      if (bitmap != null) {
        richTextBox.Text += "\n";
        richTextBox.Text += String.Format("Bild: {0}, {1} \n", info, bitmap);
        richTextBox.Text += String.Format("Pixelformat: {0}\n", bitmap.PixelFormat);
        richTextBox.Text += String.Format("Bildgröße: {0}\n\n", bitmap.Size);
      }
      else { richTextBox.Text = "Kein Bild erkannt!\n"; }
    }

    /// <summary> ANALOG
    /// 2.1a Liefert Textinformationen zu einem Bitmap
    /// </summary>
    /// <param name="bitmap"></param> Bild
    /// <param name="info"></param> Optionale beliebige Bezeichnung.
    /// <returns></returns> Informationen
    public string bitmapTxtInfo(Bitmap bitmap, string info = "") {
      string text = ("\n");
      if (bitmap != null) {
        text += String.Format("Bild: {0}, {1} \n", info, bitmap);
        text += String.Format("Pixelformat: {0}\n", bitmap.PixelFormat);
        text += String.Format("Bildgröße: {0}\n\n", bitmap.Size);
      }
      else { text += "Kein Bild erkannt!\n"; }
      return text;
    }

    /// <summary>                                                              
    /// 2.2 Informationen zur Bild-Array-Transformation
    /// </summary>
    /// <param name="bitmap"></param>  Bild zur Ausgabe in einer Bildbox
    /// <param name="info"></param>    Information zur Ausgabe in einer Textbox
    /// <param name="page"></param>    Registerseite wird aufgeschlagen
    public void bitmapShow(Bitmap bitmap, string info = "Bild", int page = 4) {
      pictureBox.Image = bitmap;
      tabControl.SelectedIndex = page;
      bitmapInfo(bitmap, info);
    }

    /// <summary> ÜBERLADUNG                                                             //++ 08
    /// 2.3 Informationen zur Bild-Array-Transformation
    /// </summary>
    /// <param name="array3D"></param>  3D-Array zur Ausgabe in einer Bildbox
    /// <param name="info"></param>    Information zur Ausgabe in einer Textbox
    /// <param name="page"></param>    Registerseite wird aufgeschlagen
    public void bitmapShow(float[,,] array3D, string info = "Bild", int page = 4) {
      Bitmap bitmap = imgArray.toBitmap(array3D);
      bitmapShow(bitmap, info, page);
    }

    /// <summary> ÜBERLADUNG                                                             //++
    /// 2.4 Informationen zur Bild-Array-Transformation
    /// </summary>
    /// <param name="build"></param>  Build-Struktur mit eindimensionalem Feld und Gestalt des Bildes.
    /// <param name="info"></param>    Information zur Ausgabe in einer Textbox
    /// <param name="page"></param>    Registerseite wird aufgeschlagen
    public void bitmapShow(ImageArrayOwn.BuildType<float> build, string info = "Bild", int page = 4) {
      Bitmap bitmap = imgArray.toBitmap(build);
      bitmapShow(bitmap, info, page);
    }
    #endregion

    #region 3. Grafikelemente einer Bitmap hinzufügen
    /// <summary>
    /// 3.1 Zeichnet ein Rechteck in eine Bitmap.
    /// </summary>
    /// <param name="bitmap"></param> Bitmap, Bild
    /// <param name="rect"></param> Rechteckkoordinaten
    /// <param name="thickness"></param> Dicke der Umrandung
    public void addRect(Bitmap bitmap, Rectangle rect, int thickness = 2) {
      Pen pen = new Pen(Color.Red, thickness);
      Graphics bmp = Graphics.FromImage(bitmap);
      bmp.DrawRectangle(pen, rect); // Zeichnet Rechteck
    }

    /// <summary>
    /// 3.2 Zeichnet einen Kreis als Punkt in eine Bitmap
    /// </summary>
    /// <param name="bitmap"></param> Bitmap, Bild
    /// <param name="point"></param> Position des Kreises bzw. des Punktes
    /// <param name="thickness"></param> Dicke der Umrandung
    public void addPoint(Bitmap bitmap, PointF point, int thickness = 1) {
      Point p0 = new Point((int)(point.X + 0.5), (int)(point.Y + 0.5));
      Pen pen = new Pen(Color.Red, thickness);
      Rectangle rect = new Rectangle(p0.X - thickness, p0.Y - thickness, 2 * thickness, 2 * thickness);
      Graphics bmp = Graphics.FromImage(bitmap);
      bmp.DrawArc(pen, rect, 0, 360);
      //bmp.DrawRectangle(pen, rect);
    }

    /// <summary> ERWEITERUNG
    /// 3.2a Zeichnet einen Kreis als Punkt in eine Bitmap
    /// </summary>
    /// <param name="bitmap"></param> Bitmap, Bild
    /// <param name="points"></param> Positionen der Kreise oder Punkte.
    /// <param name="thickness"></param> Dicke der Umrandung
    public void addPoints(Bitmap bitmap, PointF[] points, int thickness = 1) {
      for (int id = 1; id < points.Length; id++)
        addPoint(bitmap, points[id], thickness);
    }

    /// <summary>
    /// 3.3 Zeichnet eine Linie zur Kennzeichnung einer Richtung in ein Bild.
    /// </summary>
    /// <param name="bitmap"></param> Bild, Bitmap
    /// <param name="p0"></param> Startpunkt
    /// <param name="gamma"></param> Winkel für die Richtung
    /// <param name="l"></param> Länge der Richtungslinie
    /// <param name="thickness"></param> Dicke der Linie
    public void addPointer(Bitmap bitmap, PointF point, float gamma, int l = 10, int thickness = 2) {
      int x1 = (int)(point.X + l * Math.Cos(gamma) + 0.5);
      int y1 = (int)(point.Y + l * Math.Sin(gamma) + 0.5);
      Point p1 = new Point(x1, y1);
      Pen pen = new Pen(Color.Red, thickness);
      Graphics bmp = Graphics.FromImage(bitmap);
      bmp.DrawLine(pen, point, p1);
    }

    /// <summary> ERWEITERUNG
    /// 3.3a Zeichnet Linie zur Kennzeichnung der Hauptträgheitsachse
    /// einer Segmentfläche in ein Bild.
    /// </summary>
    /// <param name="bitmap"></param> Bitmap, Bild
    /// <param name="points"></param> Positionen der Kreise oder Punkte.
    /// <param name="gammas"></param> Winkel für die Richtung
    /// <param name="l"></param> Länge der Richtungslinien
    /// <param name="thickness"></param> Dicke der Linien
    public void addPointers(Bitmap bitmap, PointF[] points, float[] gammas, int l = 10, int thickness = 2) {
      for (int id = 1; id < points.Length; id++)
        addPointer(bitmap, points[id], gammas[id], l, thickness);
    }

    /// <summary>
    /// 3.4 Schreibt Grafiktext in ein Bild.
    /// </summary>
    /// <param name="bitmap"></param> Bitmap, Bild
    /// <param name="point"></param> Anfangspunkt des Textes.
    /// <param name="text"></param> Text
    public void addText(Bitmap bitmap, Point point, String text, int size = 12) {
      // Schriftgestaltung
      Pen pen = new Pen(Color.Blue, 1);
      Font font = new Font("Microsoft Sans Serif", size);
      SolidBrush brush = new SolidBrush(Color.Aqua);
      Graphics bmp = Graphics.FromImage(bitmap);
      bmp.DrawString(text, font, brush, point); // Schreibe Grafiktext
    }

    /// <summary> ÜBERLADUNG
    /// 3.4 Schreibt Text in ein Bild.
    /// </summary>
    /// <param name="bitmap"></param> Bitmap, Bild
    /// <param name="zeile"></param> Datenzeile
    /// <param name="text"></param> Text
    public void addText(Bitmap bitmap, int zeile, String text, int size = 12) {
      // Schriftgestaltung
      Point point = new Point(0, size + size * zeile);
      addText(bitmap, point, text, zeile);
    }
    /// <summary>
    /// 3.5 Fügt die Identifikationsnummern der Segmente einer Bitmap hinzu.
    /// </summary>
    /// <param name="bitmap"></param> Bitmap
    /// <param name="kettencodes"></param> Kettencode mit den
    // zwei ersten Werten als Startpunkt.
    /// <param name="size"></param> Schriftgröße
    public void addIds(Bitmap bitmap, List<List<uint>> kettencodes, int size = 12) {
      Point point;
      for (int id = 0; id < kettencodes.Count; id++) {
        point = new Point(Convert.ToInt32(kettencodes[id][0]), Convert.ToInt32(kettencodes[id][1]));
        addText(bitmap, point, id.ToString(), size);
      }
    }

    /// <summary> ÜBERLADUNG
    /// 3.5b Fügt die Id's in eine Bitmap ein.
    /// </summary>
    /// <param name="bitmap"></param> Bitmap
    /// <param name="startPoints"></param> Liste mit Startpunkten
    /// <param name="size"></param> Schriftgröße
    public void addIds(Bitmap bitmap, List<Point> startPoints, int size = 12) {
      for (int id = 0; id < startPoints.Count; id++) {
        addText(bitmap, (Point)startPoints[id], id.ToString(), size);
      }
    }

    /// <summary> ERGÄNZUNG
    /// 3.6 Schreibt die Id's und Bezeichner in einer Bitmap.
    /// </summary>
    /// <param name="bitmap"></param> Bitmap
    /// <param name="kettencodes"></param> Kettencode mit den
    // zwei ersten Werten als Startpunkt.
    /// <param name="classOfSegment"></param> Liste mit Klassenbezeichnungen der Segmente.
    /// <param name="size"></param> Schriftgröße
    public void addIdsAndNames(Bitmap bitmap, List<List<uint>> kettencodes, List<string> classOfSegment, int size = 12) {
      Point point;
      for (int id = 0; id < kettencodes.Count; id++) {
        point = new Point(Convert.ToInt32(kettencodes[id][0]), Convert.ToInt32(kettencodes[id][1]));
        string text = String.Format("{0:d} {1:s}", id, classOfSegment[id]);
        addText(bitmap, point, text, size);
      }
    }

    /// <summary> ÜBERLADUNG
    /// 3.6b Schreibt die Id's und Bezeichner in eine Bitmap.
    /// </summary>
    /// <param name="bitmap"></param Bitmap
    /// <param name="kettencodes"></param> Kettencode mit den zwei ersten Werten als Startpunkt.
    /// <param name="classOfSegment"></param> Feld mit Klassenbezeichnungen der Segmente
    /// <param name="size"></param> Schriftgröße
    public void addIdsAndNames(Bitmap bitmap, List<List<uint>> kettencodes, string[] classOfSegment, int size = 12) {
      List<string> list = new List<string>();
      // Konvertiert Feld in eine Liste
      foreach (var elementInit in classOfSegment)
        list.Add(elementInit.ToString());
      addIdsAndNames(bitmap, kettencodes, classOfSegment, 16);
    }
    #endregion

    #region 4. Ausgabe von Texten auf der Informationsseite und in der Picture-Box.
    /// <summary> 
    /// 4.1 Schreibt ein Text-Array in eine Text-Box.
    /// </summary>
    /// <param name="text"></param> Text als String-Array
    public void textBoxText(string[] text) {
      richTextBox.Text += "\n";
      for (int i = 0; i < text.Length; i++) {
        richTextBox.Text += text[i] + "\n";
      }
    }

    /// <summary> ÜBERLADUNG
    /// 4.1b Schreibt Text in eine Text-Box.
    /// </summary>
    /// <param name="text"></param> Text als Sting-Array
    public void textBoxText(string text) {
      richTextBox.Text += "\n\nAusgabe:\n";
      richTextBox.Text += text + "\n";

    }

    /// <summary>
    /// 4.2 Schreibt Text in eine Picture-Box.
    /// </summary>
    /// <param name="text"></param> Text als Sting-Array
    public void pictureBoxText(string[] text) {
      // In diese Bildbox wird gezeichnet. 
      Graphics graphics = pictureBox.CreateGraphics();
      // Schriftgestaltung
      Pen pen = new Pen(Color.Red, 2);
      Font font = new Font("Microsoft Sans Serif", 12);
      SolidBrush brush = new SolidBrush(Color.Red);
      // Grafikschrift
      for (int i = 0; i < text.Length; i++) {
        graphics.DrawString(text[i], font, brush, pos);
        pos.Y += 20; // Nächste Zeile
      }
    }

    /// <summary>
    /// 4.3 Ausgabe der Strukturmatrix oder eines quadratischen 2D-Arrays.
    /// </summary>
    /// <typeparam name="Tin"></typeparam> Datentyp der Elemente des Eingangsarrays
    /// <param name="array2D"></param> 2D-Array, Strukturmatrix
    /// <param name="title"></param> Überschrift, Titel für die Ausgabe
    public void textBoxMatrix<Tin>(Tin[,] array2D, string title = "Strukturmatrix Size X x Y:") {
      richTextBox.Text += String.Format("\n {0:s} {1:d} x {2:d} Elemente", title, array2D.GetLength(0), array2D.GetLength(1));
      double matrixSum = 0.0;
      for (int j = 0; j < array2D.GetLength(0); j++) {
        richTextBox.Text += "\n";
        for (int i = 0; i < array2D.GetLength(1); i++) {
          richTextBox.Text += String.Format(" {0:F9} ", array2D[j, i]);
          matrixSum += Convert.ToDouble(array2D[j, i]);
        }
      }
      richTextBox.Text += String.Format("\nSumme aller Elemente Sum: {0:F4}", (float)matrixSum);
    }

    /// <summary>
    /// 4.4 Ausgabe eines Feldes
    /// </summary>
    /// <param name="feld1D"></param>
    public void textBoxMatrix<Tin>(Tin[] feld1D, string title = "Strukturmatrix Size X:") {
      richTextBox.Text += String.Format("\n{0:s} {1:d} Elemente", title, feld1D.GetLength(0));
      double matrixSum = 0.0;
      for (int i = 0; i < feld1D.GetLength(0); i++) {
        richTextBox.Text += String.Format("\n {0,9:d} | {1:F9} ", i, feld1D[i]);
        matrixSum += Convert.ToDouble(feld1D[i]);
      }
      richTextBox.Text += String.Format("\nSumme aller Elemente Sum: {0:F4}", matrixSum);
    }

    /// <summary>
    ///  Trainingsdaten auf der Infoseite ausgeben
    /// </summary>
    /// <param name="array"></param>
    /// <param name="page"></param>
    public void textBox3DArray(float[,,] array, int page = 7) {
      richTextBox.Text += String.Format("\n3D-Array m x n x o: {0:d} Klassen x {1:d} Eigenschaften x {2:d} Elemente", array.GetLength(0), array.GetLength(1), array.GetLength(2));
      for (int k = 0; k < array.GetLength(0); k++)
        for (int j = 0; j < array.GetLength(1); j++) {
          richTextBox.Text += "\n";
          richTextBox.Text += String.Format("k{0:d}, j{1:d}: ", k, j);
          for (int i = 0; i < array.GetLength(2); i++) {
            richTextBox.Text += String.Format(" {0,7:F2} ", array[k, j, i]);

          }
        }
      tabControl.SelectedIndex = page;
    }
    #endregion

    #region 5. Grafikelemente in Arrays
    /// <summary>
    /// 5.1 Zeichnet Umrandung in ein Feld
    /// </summary>
    /// <param name="array"></param> 3D_Array
    /// <param name="chain"></param> Kettencode einer Kontur
    /// <param name="shift"></param> Definition der Folgeelemente
    public void graphicChain(float[,,] array, List<uint> chain, int[,] shift) {
      Point p = new Point((int)chain[0], (int)chain[1]);
      array[1, p.Y, p.X] = (float)0.2; // Startpunkt
      array[2, p.Y, p.X] = (float)0.2;
      for (int k = 2; k < chain.Count; k++) {
        p.X += shift[k, 0];
        p.Y += shift[k, 1];
        array[1, p.Y, p.X] = (float)1; // Konturpunkte
        array[2, p.Y, p.X] = (float)1;
      }
    }

    /// <summary>
    /// 5.2 Erzeugt ein Symbol in einem Array.
    /// </summary>
    /// <param name="array"></param> 3D-Array
    /// <param name="point"></param> zu markierender Punkt
    public void graphicCenter(float[,,] array, PointF pointF) {
      Point p = new Point((int)(pointF.X) + 1, (int)(pointF.Y) + 1);
      int ry = array.GetLength(1) / 40 + 1;
      int rx = array.GetLength(2) / 40 + 1;
      for (int y = p.Y - ry; y < p.Y + ry; y++)
        for (int x = p.X - rx; x < p.X + rx; x++) {
          array[1, y, x] = (float)0.6; //Farbe indirekt festlegen.
          array[2, y, x] = (float)0.6;
        }
    }

    /// <summary>
    /// 5.3 Zeichnet eine Linie.
    /// </summary>
    /// <param name="array"></param>
    /// <param name="pointF"></param>
    /// <param name="gamma"></param>
    public void graphicRichtung(float[,,] array, PointF pointF, float gamma) {
      int l = array.GetLength(1) / 5 + 2; // Länge der Pfeillinie
      Point p0 = new Point((int)(pointF.X) + 1, (int)(pointF.Y) + 1);
      int x, y;
      // gamma += (float)Math.PI; // Lösung 1 oder 2
      for (int r = 0; r < l; r++) {
        x = (int)(p0.X + r * Math.Cos(gamma));
        y = (int)(p0.Y + r * Math.Sin(gamma));
        array[1, y, x] = (float)0.7;  //Farbe indirekt festlegen.
        array[2, y, x] = (float)0.7;
      }
    }

    // <summary>
    /// 5.4 Zeichnet ein + an die Position,
    /// zum Beispiel zur kennzeichnung des Schwerpunktes,
    /// </summary>
    /// <param name="matrix"></param> Matrix als Referenz
    /// <param name="p"></param> Position
    ///   /// <param name="l"></param> Länge des der Schenkel 
    public static void graphicPlus(float[,,] array, PointF pointF, int l = 3) {
      Point p = new Point((int)(pointF.X) + 1, (int)(pointF.Y) + 1);
      try {
        for (int x = p.X - l; x <= p.X + l; x++) {
          array[x, p.Y, 0] = 1;   // rot
          array[x, p.Y, 1] = 0;
          array[x, p.Y, 2] = 1;
        }
        for (int y = p.Y - l; y <= p.Y + l; y++) {
          array[p.X, y, 0] = 1;
          array[p.X, y, 1] = 0;
          array[p.X, y, 2] = 1;
        }
      }
      catch { }
      array[p.X, p.Y, 0] = 1;   // rot
      array[p.X, p.Y, 1] = 0;
      array[p.X, p.Y, 2] = 0;
    }
    #endregion

    #region 6. Histogramminformationen

    /// <summary>
    /// 6.1 Schreibt die Extrempunkte in eine Textbox.
    /// </summary>
    /// <param name="pointExtrems"></param> Extrempunkte als Liste
    public void extremPoints(List<SegmentationOwn.PointExtrem> pointExtrems) {
      richTextBox.Text += "\n";
      for (int i = 0; i < pointExtrems.Count; i++) {
        richTextBox.Text += String.Format("\np[{0:d}]: c;{1:d}, x:{2:d}, y':{3:F1}, min:{4:s}\n", i, pointExtrems[i].c, pointExtrems[i].x, pointExtrems[i].y, pointExtrems[i].min);
      }
    }
    /// <summary> ÜBERLADUNG
    /// 6.2 Schreibt die Extrempunkte in eine Textbox.
    /// </summary>
    /// <param name="pointExtrems"></param>Extrempunkte als Listen-Feld
    public void extremPoints(List<float[]> pointExtrems) {
      richTextBox.Text += "\n";
      for (int i = 0; i < pointExtrems.Count; i++) {
        richTextBox.Text += String.Format("\np[{0:d}]: c:{1:F0}, x:{2:F0}, y':{3:F1}, min=0/max=1:{4:F0}\n", i, pointExtrems[i][0], pointExtrems[i][1], pointExtrems[i][2], pointExtrems[i][3]);
      }
    }

    /// <summary>
    /// 6.3 Gibt die Schwellwerte auf der Infoseite aus.
    /// </summary>
    /// <param name="thresholds"></param> Liste2D der Schwellwerte
    public void thresholds(List<List<byte>> thresholds) {
      string[] ausgabe = new string[thresholds.Count + thresholds[0].Count + thresholds[1].Count + thresholds[2].Count + 1];
      int n = 0;
      ausgabe[n] = String.Format("\nSchwellwerte");
      n++;
      for (int c = 0; c < thresholds.Count; c++) {
        ausgabe[n] = String.Format("\nKanal {0:d}", c);
        n++;
        for (int i = 0; i < thresholds[c].Count; i++) {
          ausgabe[n] = String.Format(" {0:d} |", thresholds[c][i]);
          n++;
        }
      }
      textBoxText(ausgabe);
    }

    /// <summary> ÜBERLADUNG
    /// 6.4 Ausgabe der kanalbezogenen Schwellwerte
    /// </summary>
    /// <param name="thresholds"></param> Liste mit Schwellwerten
    /// Indizes 0, 1, 2 entsprechen den Grundfarben R, G, B
    public void thresholds(List<byte> thresholds) {
      string[] ausgabe = new string[2 * thresholds.Count + 1];
      int n = 0;
      ausgabe[n] = String.Format("\nSchwellwerte");
      n++;
      for (int c = 0; c < thresholds.Count; c++) {
        ausgabe[n] = String.Format("\nKanal {0:d}", c);
        n++;
        ausgabe[n] = String.Format("I= {0:d} ", thresholds[c]);
        n++;
      }
      textBoxText(ausgabe);
    }


    #endregion






  }
}