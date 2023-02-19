/* Klasse ImageTableyOwn: Stellt Bitmaps und Bildarrays auszugsweise als Tabelle dar. Intensitätswerte werden numerisch ausgegeben.
 * Verfasser: Horst-Christian Heinke
 * Datum: 19.05.2022
 * Letzte Änderung: 15.01.2023
 * Germany, Sachsen-Anhalt, Magdeburg
 * */

using System;
using System.Drawing; // - Bitamp
using ARRAY = ImageProcessing.ArrayOwn; // Arrays modifizieren
using form = System.Windows.Forms; // Komponenten des Formulars verwenden

namespace ImageProcessing {
  internal class ImageTableOwn {

    #region 1. Instanzvariablen und Konstruktor
    // 1.1 Darstellung
    public form.DataGridView dataGridView; // Tabellenkomponente
    public ImageArrayOwn image; // Klasse zum Konvertieren

    /// <summary>
    /// 1.2 Konstruktor als Komponentenschnittstelle
    /// </summary>
    /// <param name="dataGridView"></param> Tabellenkomponente
    public ImageTableOwn(form.DataGridView dataGridView) {
      this.dataGridView = dataGridView;
      image = new ImageArrayOwn();
    }
    #endregion

    #region 2. Bilder und Arrays als Bildtabelle darstellen.
    /// <summary>
    /// 2.3 Visualisiert Kanäle eines 3D-Array als Bld-Tabelle
    /// mit numerischer Ausgabe der Farbwerte der Bildpunkte.
    /// </summary>
    /// <param name="array"></param> 3D-Array, Elemente vom Typ Float zwischen 0 und 1.
    /// <param name="canals"></param> Feld mit Kanalindizes {0, 1, 2}.
    /// <param name="outputFormat"></param> Format der Farbwerte
    ///                                  {hexa[0, FF], int[0, 255], float[0.0, 1.0]}
    /// <param name="maxHeight"></param> Maximale Höhe des Bildausschnitts
    /// <param name="maxWidth"></param> Maximale Weite des Bildausschnitts
    public void viewAsTable(float[,,] array, int[] canals, string outputFormat = "hexa", int maxHeight = 100, int maxWidth = 100) {
      // Liefert die Extremwerte der Kanäle.
      ARRAY.Extrem[] extrem = ARRAY.canalExtrem(array);
      // Größe der Bildtabelle einschränken
      int height = array.GetLength(1);
      int width = array.GetLength(2);
      if (width > maxWidth)
        width = maxWidth;
      if (height > maxHeight)
        height = maxHeight;
      // Tabellengestaltung
      dataGridView.RowCount = height;
      dataGridView.ColumnCount = width;
      // Schriftart
      dataGridView.DefaultCellStyle.Font = new Font("Dubai", 10);
      // Spaltenbreite an Inhalt anpassen
      dataGridView.AutoSizeColumnsMode = form.DataGridViewAutoSizeColumnsMode.AllCells;
      // Tabellenbeschriftung
      for (int x = 0; x < width; x++) { // Spaltenüberschriften
        dataGridView.Columns[x].HeaderText = x.ToString();
      }
      for (int y = 0; y < height; y++) { // Zeilenbeschriftung
        dataGridView.Rows[y].HeaderCell.Value = y.ToString();
      }
      //Daten aus Array verarbeiten
      for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++) {
          // In Farben
          byte[] Irgb = { 0, 0, 0 }; // Intesität in Byte [0, 255] für r, g, b
          for (int c = 0; c < array.GetLength(0); c++) {
            // Konvertiert die Arraywerte in einen darstellbaren Intervall.
            Irgb[c] = (byte)ARRAY.toIntervall(array[c, y, x], extrem[c], ARRAY.getIntervall(0, 255)[c]);
          }
          string Istring = "";
          for (int c = 0; c < canals.Length; c++) {
            if (outputFormat == "hexa")
              Istring += String.Format("{0:X02}", Irgb[canals[c]]);
            if (outputFormat == "int")
              Istring += String.Format("{0:F0},", Irgb[canals[c]]);
            if (outputFormat == "float")
              Istring += String.Format("{0:F2} ", array[canals[c], y, x]);
          }
          // Farbzuweisung einer Zelle entsprechend der Pixelfarbe
          dataGridView[x, y].Style.BackColor = Color.FromArgb(Irgb[0], Irgb[1], Irgb[2]);
          // Wertzuweisung einer Zelle entsprechend der Intensität
          dataGridView[x, y].Value = Istring;
          // Schriftfarbe abhängig vom Hintergrund
          dataGridView[x, y].Style.ForeColor = Color.Black;
          if ((Irgb[0] + Irgb[1] + Irgb[2]) < 256)
            dataGridView[x, y].Style.ForeColor = Color.White;
        }
      dataGridView.Height = dataGridView.Width;
    }

    /// <summary> ÜBERLADUNG
    /// 2.4 Visualisiert Layers eines Bitmaps als Bild-Tabelle mit numerischer Ausgabe
    ///     der Farbwerte der Bildpunkte.
    /// </summary>
    /// <param name="bitmap"></param> Bitmap, Bild
    /// <param name="canals"></param> Feld mit Kanalindizes {0, 1, 2}.
    /// <param name="outputFormat"></param> Format der Farbwerte
    ///                                     {hexa[0, FF], int[0, 255], float[0.0, 1.0]}
    /// <param name="maxHeight"></param> Maximale Höhe des Bildausschnitts
    /// <param name="maxWidth"></param> Maximale Weite des Bildausschnitts
    public void viewAsTable(Bitmap bitmap, int[] canals, string outputFormat = "hexa", int maxHeight = 100, int maxWidth = 100) {
      viewAsTable(image.toArray3D<float>(bitmap), canals, outputFormat, maxHeight, maxWidth);
    }
    #endregion

  }
}
