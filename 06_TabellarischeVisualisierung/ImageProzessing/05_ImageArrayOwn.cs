/* Transformiert Bitmaps in Array-Formate
 * und zurück ohne den Inhalt zu verändern.
 * Autor: Heinke
 * 19.05.2022
 *  * */
#region: using
using System;
using System.Drawing;        // Für Bitmaps
#endregion ---------------------------------------
namespace ImageProcessing {
  class ImageArrayOwn {

    #region 1. Image -> 3D-Array -> Image   
    /// <summary> GENERISCHE METHODE <T> byte, float, double</T>
    /// 3.1: Transformiert eine Bitmap in eine dreidimensionales 
    /// Array[C,Y,X]. Der Ergebnistyp ist generisch. Die Arrayelemente 
    /// können vom Typ "byte" im Intervall 0 bis 255 oder vom Typ 
    /// "double" oder float im Intervall von 0 bis 1 sein.
    /// </summary>
    /// <typeparam name="Tout"></typeparam> Ergebnisdatentyp [byte, float, double]
    /// <param name="bitmap"></param> Image vom Typ Bitmap
    /// <returns></returns> 3D-Array [Kanäle, Höhe, Weite]
    public Tout[,,] toArray3D<Tout>(Bitmap bitmap) {
      Type type = typeof(Tout); //Typ der Arrayelemente
      // Deklaration des Arrays
      Tout[,,] array3D = new Tout[3, bitmap.Height, bitmap.Width];
      // Zeilenweise
      for (int y = 0; y < bitmap.Height; y++)
        for (int x = 0; x < bitmap.Width; x++) {
          // Liefert den Farbwert
          Color color = bitmap.GetPixel(x, y);
          if (type.Equals(typeof(byte))) { // Byte-Werte
            array3D[0, y, x] = (Tout)Convert.ChangeType(color.R, type);
            array3D[1, y, x] = (Tout)Convert.ChangeType(color.G, type);
            array3D[2, y, x] = (Tout)Convert.ChangeType(color.B, type);
          }
          else { // Normierte Werte
            array3D[0, y, x] = (Tout)Convert.ChangeType(color.R / 255.0, type);
            array3D[1, y, x] = (Tout)Convert.ChangeType(color.G / 255.0, type);
            array3D[2, y, x] = (Tout)Convert.ChangeType(color.B / 255.0, type);
          }
        }
      return array3D;
    }

    /// <summary> GENERISCHE METHODE <T> byte, float, double</T>
    /// 1.2: Wandelt eine dreidimensionales Array[X,Y,C] in ein Bitmap um.
    /// Der Typparameter gibt den Datentyp der Arrayelemente an.
    /// Die Typen "byte", "float" und "double" sind möglich.
    /// </summary>
    /// <typeparam name="Tin"></typeparam> Eingangsdatentyp
    /// <param name="array3D"></param>  3D-Array[X,Y,C]
    /// <returns></returns> Image vom Typ Bitmap
    public Bitmap toBitmap<Tin>(Tin[,,] array3D) {
      byte r, g, b; // drei Grundfarben
      Type type = typeof(Tin); //Typ der Matrixelemente
      int height = array3D.GetLength(1);
      int width = array3D.GetLength(2);
      Bitmap bitmap = new Bitmap(width, height); // Deklaration des Bitmaps
      for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++) {       // Zeilenweise Verarbeitung
          if (type.Equals(typeof(byte))) {
            r = Convert.ToByte(array3D[0, y, x]);
            g = Convert.ToByte(array3D[1, y, x]);
            b = Convert.ToByte(array3D[2, y, x]);
          }
          else { // aus dem Typ float oder double
            r = (byte)(Convert.ToDouble(array3D[0, y, x]) * 255);
            g = (byte)(Convert.ToDouble(array3D[1, y, x]) * 255);
            b = (byte)(Convert.ToDouble(array3D[2, y, x]) * 255);
          }
          bitmap.SetPixel(x, y, Color.FromArgb(r, g, b));
        }
      return bitmap;
    }
    #endregion

    #region 2. Image -> 1DFeld --> Image
    /// <summary>
    /// 2.1 Wandelt ein Bitmap in ein eindimensionales Feld um.
    /// </summary>
    /// <typeparam name="Tout"></typeparam> Datentyp der Feldelemente
    /// <param name="bitmap"></param> Bild, Image vom Typ Bitmap
    /// <returns></returns> Eindimensionales Feld
    public Tout[] toFeld1D<Tout>(Bitmap bitmap) {
      Type type = typeof(Tout); //Typ der Elemente
      int channels = 3; //Anzahl der Kanäle R, G, B
      Tout[] feld1D = new Tout[channels * bitmap.Height * bitmap.Width];
      int ebenenPixel = bitmap.Height * bitmap.Width; // Pixel pro Kanalebene
      for (int y = 0; y < bitmap.Height; y++) // Zeilenweise Verarbeitung
        for (int x = 0; x < bitmap.Width; x++) {
          int i = y * bitmap.Width + x; // Pixelindex zeilenweise für ein Kanal
          Color color = bitmap.GetPixel(x, y); // Liefert den Farbwert
          if (type.Equals(typeof(byte))) { // RGB-Werte
            feld1D[i] = (Tout)Convert.ChangeType(color.R, type);
            feld1D[i + ebenenPixel] = (Tout)Convert.ChangeType(color.G, type);
            feld1D[i + 2 * ebenenPixel] = (Tout)Convert.ChangeType(color.B, type);
          }
          else {            // Normierte Werte
            feld1D[i] = (Tout)Convert.ChangeType(color.R / 255.0, type);
            feld1D[i + ebenenPixel] = (Tout)Convert.ChangeType(color.G / 255.0, type);
            feld1D[i + 2 * ebenenPixel] = (Tout)Convert.ChangeType(color.B / 255.0, type);
          }
        }
      return feld1D;
    }

    /// <summary>
    /// 2.2 Liefert die Gestalt "shape" eines Bildes und legt die 
    /// Werte in ein Feld [Kanäle, Höhe, Weite]
    /// </summary>
    /// <param name="bitmap"></param> Bild
    /// <returns></returns> Größe, Struktur
    public int[] giveShape(Bitmap bitmap) {
      int[] shape = { 3, bitmap.Height, bitmap.Width };
      return shape;
    }

    /// <summary>
    /// 2.3 Transformiert ein eindimensionales Feld in ein Bild unter Kenntnis der
    /// Bildgröße (Shape)
    /// </summary>
    /// <typeparam name="Tin"></typeparam>
    /// <param name="feld1D"></param> Eindimensionale Feld, repräsentiert ein Bild
    /// <param name="shape"></param> Gestalt des Bildes [Kanäle, Höhe, Weite]
    /// <returns></returns> Bild, Image vom Typ Bitmap
    public Bitmap toBitmap<Tin>(Tin[] feld1D, int[] shape) {
      byte r, g, b; // drei Grundfarben
      Type type = typeof(Tin); //Typ der Feldelemente
      int channels = shape[0], height = shape[1], width = shape[2];
      int ebenenPixel = height * width; // Pixel pro Kanalebene
      Bitmap bitmap = new Bitmap(width, height); // Deklaration des Bitmaps
      for (int y = 0; y < height; y++)      // Zeilenweise Verarbeitung
        for (int x = 0; x < width; x++) {
          int i = y * width + x; // Index eines Ebenenelementes
          if (type.Equals(typeof(byte))) {
            r = (byte)Convert.ToByte(feld1D[i]);
            g = (byte)Convert.ToByte(feld1D[i + ebenenPixel]);
            b = (byte)Convert.ToByte(feld1D[i + 2 * ebenenPixel]);
          }
          else { // float oder double
            r = (byte)(Convert.ToDouble(feld1D[i]) * 255);
            g = (byte)(Convert.ToDouble(feld1D[i + ebenenPixel]) * 255);
            b = (byte)(Convert.ToDouble(feld1D[i + 2 * ebenenPixel]) * 255);
          }
          bitmap.SetPixel(x, y, Color.FromArgb(r, g, b));
        }
      return bitmap;
    }
    #endregion

    #region 3. Feldstruktur, Bild -> Build-Struktur --> Bild
    /// <summary>
    /// 3.1 Datenstruktur für eindimensionale Felder (flatten)
    /// </summary>
    /// <typeparam name="T"></typeparam> Datentyp der Feldelemente
    public struct BuildType<T> {
      public T[] feld1D;  // Feld enthält Intensitätswerte
      public Type type;   // Typ der Datenelemente 
      public int[] shape; // [Kanäle, Höhe, Weite] des Bildes
    }

    /// <summary>
    /// 3.2 Baut eine Struktur aus einem eindimensionalen Feld,
    /// dem Datentyp der Feldelemente und der Imagegröße. 
    /// </summary>
    /// <typeparam name="T"></typeparam> Datentyp der feldelemente
    /// <param name="feld1D"></param> Eindimensionales Feld
    /// <param name="shape"></param> Gestalt des Bildes [Kanäle, Höhe, Weite]
    /// <returns></returns> Build: Gesamtaufbau
    public BuildType<T> toBuild<T>(T[] feld1D, int[] shape) {
      // Deklaration
      BuildType<T> build = new BuildType<T>();
      // Zuweisungen
      build.feld1D = feld1D;
      build.type = typeof(T);
      build.shape = shape;
      return build;
    }
    /// <summary> ÜBERLADUNG
    /// 3.3 Wandeld ein Bild in ein eindimensionales Feld um und
    /// baut eine Struktur aus dem Feld, dem Elementetyp und der Imagegröße.
    /// </summary>
    /// <typeparam name="T"></typeparam> Datentyp der Feldelemente
    /// <param name="bitmap"></param> Bild, Image vom Typ Bitmap
    /// <returns></returns> Build
    public BuildType<T> toBuild<T>(Bitmap bitmap) {
      // Deklaration
      T[] feld1D = new T[3 * bitmap.Height * bitmap.Width];
      int[] shape = { 3, bitmap.Height, bitmap.Width };
      BuildType<T> build = new BuildType<T>();
      // Bild als eindimensionales Feld
      feld1D = toFeld1D<T>(bitmap);
      build = toBuild(feld1D, shape); //Methode
      return build;
    }

    /// <summary> // ÜBERLADUNG
    /// 3.4 Wandelt eine Build-Struktur in ein Bitmap.
    /// </summary>
    /// <typeparam name="Tin"></typeparam> Datentyp der Feldelemente
    /// <param name="build"></param> Build-Struktur: Feld, Datentyp, Shape
    /// <returns></returns> Bild, Image vom Typ Bitmap
    public Bitmap toBitmap<Tin>(BuildType<Tin> build) {
      return toBitmap<Tin>(build.feld1D, build.shape);
    }
    #endregion

    #region 4. Hilfsmethoden 
    /// <summary> ---
    /// 4.1 Verkürzte Schreibweise
    /// Wandelt ein Bitmap in ein eindimensionales Feld.
    /// </summary>
    /// <param name="bild"></param> Bild, Image vom Typ Bitmap
    /// <returns></returns> Build
    private BuildType<float> toBuild(Bitmap bild) {
      return toBuild<float>(bild);
    }
    #endregion

  }
}
