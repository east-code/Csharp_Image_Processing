/* Klasse Traegheitsmoment: Datenstruktur für Flächenträgheitsmomente
 * Klasse Points: Datenstruktur Punkte
 * Klasse SegmentFeatureOwn: Liefert Eigenschaften, Position und Orientierung einer einzelnen Segmentfläche.
 * Verfasser: Horst-Christian Heinke
 * Datum: 19.05.2022
 * Letzte Änderung: 15.01.2023
 * Germany, Sachsen-Anhalt, Magdeburg
 * */

using System;
using System.Collections.Generic;
using System.Drawing; // PointF = Punkt mit Gleitkommakoordinaten (float)
using static ImageProcessing.ImageArrayOwn;

namespace ImageProcessing {

  #region 0. Klassen zur Definition von Datenstrukturen
  /// <summary>
  /// 0.1 Struktur der Flächenträgheitsmomente J
  /// </summary>
  internal class Traegheitsmoment {
    /// <summary>
    ///  Attribute (this)
    /// </summary>
    public double x;  //um die x-Achse
    public double y;  // um die y-Achse
    public double xy; // Deviationsmoment
    /// <summary>
    /// 0.2 Konstuktor
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="xy"></param>
    public Traegheitsmoment(double x, double y, double xy) {
      this.x = x;
      this.y = y;
      this.xy = xy;
    }
  }

  /// <summary>
  /// 0.3 Klasse definiert Punkte
  /// </summary>
  class Points {
    public PointF schwerpunkt;
    public PointF pixelMax;
    public Points(PointF schwerpunkt, PointF pixelMax) {
      this.schwerpunkt = schwerpunkt;
      this.pixelMax = pixelMax;
    }
  }
  #endregion

  internal class SegmentFeatureOwn {
    #region 1. Attribute, Strukturen, Nachbarn

    /// <summary>
    /// 1.2 Neighbor indices as a shift in i, j
    /// </summary>
    public int[,] shift = { { -1, 0 }, { -1, -1 }, { 0, -1 }, { 1, -1 }, { 1, 0 }, { 1, +1 }, { 0, +1 }, { -1, +1 } };

    /// <summary>
    /// 1.3 Transformiet zweidimensionales Feld in ein eindimensionales Feld
    /// mit einer Punktstruktur.
    /// </summary>
    /// <param name="shift"></param> Zweidimensionales Feld
    /// <returns></returns> Eindimensionales Punktefeld.
    public Point[] toPoints(int[,] shift) {
      Point[] nachbar = new Point[shift.GetLength(0)];
      for (int s = 0; s < shift.GetLength(0); s++) {
        nachbar[s].Y = shift[s, 0];
        nachbar[s].X = shift[s, 1];
      }
      return nachbar;
    }

    //- OutputOwn output = new OutputOwn();        // Ausgabemethoden     
    #endregion

    #region 2: Begrenzungsrechteck
    /// <summary>
    /// 2.1 Liefert die Diagonalpunkte eines objektumschließenden Rechtecks
    /// für ein 3D-Array.
    /// </summary>
    /// <param name="array3D"></param>  Matrix[c->3, y->h, x->w,]
    /// <param name="segmentIntensity"></param> Intensität des Gegenstandes [0..1]
    /// <returns></returns> Rechteck mit (Left,Top,Width,Height)
    public Rectangle rechteck(float[,,] array3D, float segmentIntensity = 1) {
      // Anfangswerte für das Rechteck ( x, y, width, hight)
      Rectangle rect = new Rectangle(array3D.GetLength(2), array3D.GetLength(1), 0, 0);
      Point pMax = new Point(0, 0); //Hilfspunkt, um Weite und Höhe des Rechtecks erst am Ende zu berechnen.
      for (int y = 0; y < array3D.GetLength(1); y++)
        for (int x = 0; x < array3D.GetLength(2); x++)
          if (array3D[0, y, x] == segmentIntensity) {
            if (x < rect.X)
              rect.X = x;
            if (y < rect.Y)
              rect.Y = y;
            if (x > pMax.X)
              pMax.X = x;
            if (y > pMax.Y)
              pMax.Y = y;
          }
      rect.Width = pMax.X - rect.Left;
      rect.Height = pMax.Y - rect.Top;
      return rect;
    }

    /// <summary>
    /// 2.2: Liefert die Diagonalpunkte eines objektumschließenden Rechtecks
    /// für eine Build-Datenstruktur als Eingangsparameter.
    /// </summary>
    /// <param name="build"></param>  Build: Eindimensionales Feld + Gestalt
    /// <param name="segmentIntensity"></param> Intensität des Gegenstandes [0..1]
    /// <returns>rect</returns> Rechteck mit (Left,Top,Width,Height)
    public Rectangle rechteck(BuildType<float> build, float segmentIntensity = 0) {
      // Anfangswerte für das Rechteck ( x, y, width, hight)
      Rectangle rect = new Rectangle(build.shape[2], build.shape[1], 0, 0);
      int ebenenPixel = build.shape[1] * build.shape[2];
      Point p;
      Point pMax = new Point(0, 0);
      int width = build.shape[2];
      for (int i = 0; i < ebenenPixel; i++)
        if (build.feld1D[i] == segmentIntensity) {
          // X-Y-Koorinaten aus Index und Weite des assoziierendem Bild berechnen.
          p = new Point(i % width, (int)(i / width));
          if (p.X < rect.X)
            rect.X = p.X;
          if (p.Y < rect.Y)
            rect.Y = p.Y;
          if (p.X > pMax.X)
            pMax.X = p.X;
          if (p.Y > pMax.Y)
            pMax.Y = p.Y;
        }
      rect.Width = pMax.X - rect.Left;
      rect.Height = pMax.Y - rect.Top;
      return rect;
    }

    /// <summary>
    /// Berechnet die X-Y-Koordinaten aus dem Index i
    /// und der Bildweite.
    /// </summary>
    /// <param name="i"></param> Index i des eindimensionalen Feldes
    /// <param name="width"></param> Weite des assoziierenden Bildes oder 3D-Arrays.
    /// <returns></returns> Punkt im der X-Y-Arrayebene
    public Point toPoint(int i, int width) {
      return new Point(i % width, (int)i / width);
    }

    #endregion

    #region 3. Flaeche von Gegenständes, Objekten eines Binärfeldes 
    //       [Hintergrund=0, Abbild des Gegenstands (Segmentfläche)= segmentIntensity]

    /// <summary>
    /// 3.1 Liefert die Segmentfläche in Pixel
    /// </summary>
    /// <param name="feld"></param> Eindimensionales Feld
    /// <param name=" segmentIntensity "></param> Intensität oder Segment-Id [0..1]
    /// <returns> a_Ap </returns> Segmentfläche in Pixel
    public int flaeche(float[] feld, float segmentId = 1) {
      int a_Ap = 0;
      for (int i = 0; i < feld.Length / 3; i++)
        if (feld[i] == segmentId)
          a_Ap++;
      return a_Ap;
    }


    /// <summary> ÜBERLADUNG
    ///  3.2 Liefert die Fläche eines Binärobjektes
    /// </summary>
    /// <param name="feld"></param> geflättetes Bild
    /// <param name="segmentIntensity"></param> Intensität des Gegenstandes [0..255]
    /// <returns>a_Ap</returns> Segmentfläche in Pixel
    public int flaeche(byte[] feld, byte segmentIntensity = 255) {
      int a_Ap = 0;
      for (int i = 0; i < feld.Length / 3; i++)
        if (feld[i] == segmentIntensity)
          a_Ap++;
      return a_Ap;
    }

    /// <summary> ÜBERLADUNG
    /// 3.3 Liefert die Segmentfläche mit der Intensitiät oder Segment-Id eines 3D-Bild-Arrays
    /// Nur bei Übereinstimmung der Id-Werte handelt es sich
    /// um ein Element der Segmentfläche.
    /// </summary>
    /// <param name="array"></param> Referenz eines dreidimensionalen Arrays
    /// <param name="canal"></param> Kanal, Layer, Bildebene
    /// <param name="segmentId"></param> Intensität oder Segment-Id
    /// <returns> a_Ap </returns> Fläche in Pixel
    public int flaeche(ref float[,,] array, int canal = 0, float segmentId = 1) {
      int a_Ap = 0;
      // Doppelschleife zum Zählen der Segmentpixel
      for (int y = 0; y < array.GetLength(1); y++)
        for (int x = 0; x < array.GetLength(2); x++)
          if (array[canal, y, x] == segmentId)
            a_Ap++;
      return a_Ap;
    }


    /// <summary> ERGÄNZUNG
    /// 3.3a Liefert die Flächen aller Segmente eines Bildarrays.
    /// </summary>
    /// <param name="array"></param> Referenz eines dreidimensionalen Arrays
    /// <param name="canal"></param> Kanal, Layer, Bildebene
    /// <param name="segmentCount"></param> Segmentanzahl
    /// <returns>"a_Ap"</returns> Liste der Segmentflächen
    public List<float> flaechen(ref float[,,] array, int canal = 1, float segmentCount = 1) {
      List<float> a_Ap = new List<float>();
      // Dummy für Hintergrund 0
      a_Ap.Add(0);
      // Schleife zur Bearbeitung aller Segmente
      for (int id = 1; id <= segmentCount; id++)
        a_Ap.Add(flaeche(ref array, canal, id));
      return a_Ap;
    }

    /// <summary>
    /// 3.4 Liefert das Verhältnis der Gegenstansfläche 
    /// zur Gesamtfläche.
    /// </summary>
    /// <param name="feld"></param> geflättetes Bild
    /// <param name="segmentId"></param> Intensität des Gegenstandes [0..1]
    /// <returns></returns> Flächenenverhältnis: Objekt a_Ap zu Gesamt a_p  (a_Ap/a_p)
    public double flaechenverhaeltnis(float[] feld, float segmentId = 1) {
      int a_p = 0; // Anzahl aller Pixel, Gesamtfläche in Pixel
      int a_Ap = 0; // Fläche des Gegenstandes in Pixel
      for (int i = 0; i < feld.Length; i += 3) {
        a_p++;
        if (feld[i] == segmentId)
          a_Ap++;
      }
      return a_Ap / a_p;
    }


    /// <summary>
    /// 3.5 Berechnet die Objektfläche in mm² bei Kenntnis der Brennweite.
    /// Kameraparameter befinden sich in der Klassenstruktur "CamParameter".
    /// </summary>
    /// <param name="a_Ap"></param> Fläche in Pixel
    /// <param name="g"></param>  Abstand des Objektes von der Kamera in mm
    /// <param name="cam"></param> Parameter der Kamera
    /// <returns> A_g </returns> Fläche in mm²
    public float flaecheSi(int a_Ap, float g, CamParameter cam) {
      float A_g = a_Ap * (cam.h * cam.w) * (g - cam.f) * (g - cam.f) / (cam.h_p * cam.w_p) / cam.f / cam.f;
      return A_g;
    }

    /// <summary>
    /// 3.6 Berechnet die Objektfläche in mm² unter Verwendung der Betrachtungswinkel
    /// ohne Kenntnis der Brennweite.
    /// Kameraparameter befinden sich in der Klassenstruktur "CamParameter".
    /// </summary>
    /// <param name="a_Ap"></param> Fläche in Pixel
    /// <param name="g"></param>  Abstand des Objektes von der Kamera in mm
    /// <param name="cam"></param> Parameter der Kamera
    /// <returns> A_g </returns> Fläche in mm²
    public float flaecheSi_Alpha(int a_Ap, float g, CamParameter cam) {
      float A_g = (float)(4 * a_Ap * g * g * Math.Tan(cam.alpha_h / 2) * Math.Tan(cam.alpha_w / 2) / (cam.h_p * cam.w_p));
      return A_g;
    }

    #region 3-1 mit Lambda Expression
    /// <summary>
    /// 3.7 Delegate-Deklaration zur Flächenberechnung
    /// </summary>
    /// <param name="a_Ap"></param> Fläche in Pixel
    /// <param name="g"></param>  Abstand des Objektes von der Kamera in mm
    /// <param name="cam"></param> Parameter der Kamera
    /// <returns> A_g </returns> Fläche in mm²
    public delegate float delegateToSi(int a_Ap, float g, CamParameter cam);
    /// <summary>
    /// 3.7a Berechnet die Objektfläche in mm² bei Kenntnis der Brennweite.
    /// Kameraparameter befinden sich in der Klassenstruktur "CamParameter".
    /// </summary>
    public delegateToSi flaeche_Si = (a_Ap, g, cam) => { return a_Ap * (cam.h * cam.w) * (g - cam.f) * (g - cam.f) / (cam.h_p * cam.w_p) / cam.f / cam.f; };
    /// <summary>
    /// 3.7b Berechnet die Objektfläche in mm² unter Verwendung der Betrachtungswinkel
    /// ohne Kenntnis der Brennweite.
    /// </summary>
    public delegateToSi flaeche_SiAlpha = (a_Ap, g, cam) => { return (float)(4 * a_Ap * g * g * Math.Tan(cam.alpha_h / 2) * Math.Tan(cam.alpha_w / 2) / (cam.h_p * cam.w_p)); };
    #endregion

    #endregion

    #region 5. Flaechenschwerpunkt
    /// <summary>
    /// 5.1 Liefert den Schwerpunkt einer Fläche
    /// </summary>
    /// <param name="array"></param> 3D-Array eines Bildes
    /// <param name="canal"></param> Kanal der betrachtet werden soll.
    /// <param name="segmentId"></param> Id oder Intensitätswert des Objektes.
    /// <returns>schwerpunkt</returns> Schwerpunktkoordinaten
    public PointF flaechenschwerpunkt(float[,,] array, int canal = 0, float segmentId = 1) {
      PointF schwerpunkt = new PointF(0, 0);
      int n = 0;
      // Koordinatensummen
      for (int y = 0; y < array.GetLength(1); y++)
        for (int x = 0; x < array.GetLength(2); x++)
          if (array[canal, y, x] == segmentId) {
            schwerpunkt.X += x;
            schwerpunkt.Y += y;
            n++;
          }
      schwerpunkt.X /= n;
      schwerpunkt.Y /= n;
      //- output.graphicCenter(array, schwerpunkt);
      return schwerpunkt;
    }

    /// <summary> ERWEITERUNG
    /// 5.1b Liefert die Schwerpunkte mehrerer Segmentflächen
    /// </summary>
    /// <param name="array"></param> 3D-Array eines Bildes
    /// <param name="canal"></param> Kanal der betrachtet werden soll.
    /// <param name="itemId"></param> Id oder Intensitätswert des Objektes.
    /// <returns></returns> Schwerpunktkoordinaten
    public PointF[] flaechenschwerpunkte(float[,,] array, int canal = 0, int segmentCount = 1) {
      PointF[] schwerpunkte = new PointF[segmentCount + 1];
      for (int id = 1; id <= segmentCount; id++) {
        schwerpunkte[id] = flaechenschwerpunkt(array, 1, id);
      }
      return schwerpunkte;
    }

    /// <summary> 
    /// 5.2 Fasst Punkte zusammen für die Rückgabe
    /// </summary>
    /// <param name="array"></param> 3D-Array eines Bildes
    /// <param name="canal"></param> Kanal der betrachtet werden soll.
    /// <param name="itemId"></param> Id oder Intensitätswert des Objektes.
    /// <returns></returns>
    public Points flaechenpunkt(float[,,] array, int canal = 0, float itemId = 1) {
      PointF schwerpunkt = flaechenschwerpunkt(array, canal, itemId);
      PointF pixelMax = maxAbstand(array, schwerpunkt, 0, itemId);
      Points punkte = new Points(schwerpunkt, pixelMax);
      return punkte;
    }

    #endregion

    #region 6. Orientierung, Ausrichtung
    /// <summary>
    /// 6.1 Liefert den maximalem Abstand eines
    /// Segmentelementes (Punkt) in
    /// Bezug zum Schwerpunkt der Segmentfläche.
    /// </summary>
    /// <param name="array"></param>
    /// <param name="schwerpunkt"></param> Flächenschwerpunkt eines Gegenstandes.
    /// <param name="canal"></param> Kanal der betrachtet werden soll.
    /// <param name="segmentId "></param>  Id oder Intensitätswert des Objektes.
    /// <returns>"pointMax"</returns> Koordinaten des zum Schwerpunkt entfernesten Punktes.
    public Point maxAbstand(float[,,] array, PointF schwerpunkt, int canal = 0, float segmentId = 1) {
      float abstandMax = 0;
      Point pointMax = new Point();
      for (int y = 0; y < array.GetLength(1); y++)
        for (int x = 0; x < array.GetLength(2); x++)
          if (array[canal, y, x] == segmentId) {
            float abstand = (x - schwerpunkt.X) * (x - schwerpunkt.X) + (y - schwerpunkt.Y) * (y - schwerpunkt.Y);
            if (abstand > abstandMax) {
              abstandMax = abstand;
              pointMax.X = x;
              pointMax.Y = y;
            }
          }
      return pointMax;
    }

    /// <summary>
    /// 6.2 Bestimmt die Traegheitsmomente Jx, Jy, Jxy bezogen 
    /// auf das Basiskoordinatensystem x, y mit dem Schwerpunkt als
    /// Ursprung des Koordinatensystems.
    /// </summary>
    /// <param name="array"></param> 3D-Array eines Bildes
    /// <param name="schwerpunkt"></param> Flächenschwerpunkt eines Gegenstandes.
    /// <param name="canal"></param> Kanal der betrachtet werden soll.
    /// <param name="segmentId "></param> Id oder Intensitätswert des Objektes.
    /// <returns>"J"</returns> Trägheitsmomente mit Bezug zum Basiskoordinatensytem x,y
    public Traegheitsmoment traegheitsmomente(float[,,] array, PointF schwerpunkt, int canal = 0, float segmentId = 1) {
      Traegheitsmoment J = new Traegheitsmoment(0.0, 0.0, 0.0);
      for (int y = 0; y < array.GetLength(1); y++)
        for (int x = 0; x < array.GetLength(2); x++)
          if (array[canal, y, x] == segmentId) {
            // Jeder Pixel besitzt die virtuelle Einheitsmasse m = 1g
            J.x += (x - schwerpunkt.X) * (x - schwerpunkt.X);
            J.y += (y - schwerpunkt.Y) * (y - schwerpunkt.Y);
            J.xy += (x - schwerpunkt.X) * (y - schwerpunkt.Y);
          }
      return J;
    }

    /// <summary>
    /// 6.3 Liefert die Trägheitsmomente um die Hauptachsen 
    /// -> Jh.XY= 0
    /// </summary>
    /// <param name="J"></param> Trägheitsmomente um das Basiskoordnatensystem
    /// <returns>"Jh"</returns> Haupträgheitsmomente
    public PointF hauptTraegheitsmomente(Traegheitsmoment J) {
      PointF Jh = new PointF(0, 0);
      double D = Math.Sqrt((J.x + J.y) * (J.x + J.y) / 4 + J.xy * J.xy);
      Jh.X = (float)((J.x + J.y) / 2 + D);
      Jh.Y = (float)((J.x + J.y) / 2 - D);
      return Jh;
    }
    /// <summary>
    ///  6.4 Liefert den Winkel zwischen dem Basiskoordinatensystem 
    ///  und dem Hauptträgheitskoordinatensystem
    ///  - Der Tangens liefert zwei Lösungen 
    ///  -> Daher kann die Orientierung nicht eindeutig sein
    /// </summary>
    /// <param name="J"></param> Trägheitsmomente
    /// <returns></returns> Hauptwinkel
    public float hauptWinkel(Traegheitsmoment J) {
      float gamma = (float)Math.Atan2(2 * J.xy, (J.x - J.y)) / 2;
      return gamma;
    }

    /// <summary>
    /// 6.5 Liefert den Winkel zwischen dem Basiskoordinatensystem 
    ///  und dem Hauptträgheitskoordinatensystem (Kompakt).
    /// </summary>
    /// <param name="array"></param> 3D-Array eines Bildes
    /// <param name="schwerpunkt"></param> Flächenschwerpunkt eines Gegenstandes.
    /// <param name="canal"></param> Kanal der betrachtet werden soll.
    /// <param name="segmentId"></param> Id oder Intensitätswert eines Segmentes (Objekt).
    /// <returns>"gamma"</returns> Hauptwinkel zwischen X-Basiskoordinate und Hauptträgheitsachse.
    /// Es gibt stets zwei Lösungen: gamma und (gamma + Pi)
    public float orientierung(float[,,] array, PointF schwerpunkt, int canal = 0, float segmentId = 1) {
      Traegheitsmoment J = traegheitsmomente(array, schwerpunkt, canal, segmentId);
      float gamma = hauptWinkel(J);
      return gamma;
    }

    /// <summary> ERWEITERUNG
    /// 6.5b Liefert die Winkel zwischen dem Basiskoordinatensystem 
    ///  und dem Hauptträgheitskoordinatensystem (Kompakt) für mehrere Segmentflächen
    /// </summary>
    /// <param name="array"></param> 3D-Array eines Bildes
    /// <param name="schwerpunkt"></param> Flächenschwerpunkt eines Gegenstandes.
    /// <param name="canal"></param> Kanal der betrachtet werden soll.
    /// <param name="segmentCount"></param> Anzahl der Segmentflächen
    /// <returns>"gamma"</returns> Hauptwinkel zwischen X-Basiskoordinate und Hauptträgheitsachse.
    /// Es gibt stets zwei Lösungen: gamma und (gamma + Pi)
    public float[] orientierungen(float[,,] array, PointF schwerpunkt, int canal = 0, int segmentCount = 1) {
      float[] gamma = new float[segmentCount + 1];
      for (int id = 1; id <= segmentCount; id++) {
        Traegheitsmoment J = traegheitsmomente(array, schwerpunkt, canal, id);
        gamma[id] = hauptWinkel(J);
      }
      return gamma;
    }
    #endregion

    #region 7. Umrandung, Konturverfolgung mit Freemen-Code
    /// <summary>
    /// 7.1 Liefert den Startpunkt und die Kontur einer einzelnen Segmentfläche.
    /// Das Eingangsbild ist binär. Alle Kanäle haben identische Werte {0, 1}.
    /// Der Kanal 0 bleibt unverändert.
    /// Die gefunden Objekte erhalten eine Identifikationsnummer.
    /// Dieser Wert wird auf dem Kanal 1 abgespeichert.
    /// </summary>
    /// <param name="array"></param> 3D-Array, hier auch Freeman-Array genannt.
    /// <param name="segmentIntensity"></param> Intensität der Eingangssegmentfläche
    /// <param name="segmentId"></param> Segementfläche erhält diese Identifikation
    /// <returns></returns> Kontur
    /// Kanal 0: Unverändertes Binärbild
    /// Kanal 1: Ausgefüllte konturumschließende Flächen (Segmentflächen).
    /// Kanal 2: Kontur mit Identitätswert
    public List<uint> chaincode(ref float[,,] array, double segmentIntensity = 1, int segmentId = 1) {
      // Startpunkt, Konturanfang
      Point start = new Point(1, 1);
      Point neighbor = new Point(0, 0); // Nachbarpunkt
      Point pos = new Point(1, 1);
      bool suchen = true; // Merker für Suchschleife
      // Liste des Kettencodes, die ersten beiden Elemente geben den Starpunkt an.
      List<uint> chaincode = new List<uint>();
      // Array-Layers um den Rand 1 erweitern und die Randwerte auf 0 setzen. 
      array = arrayPadding(array, 0);
      // Alle Werte der Kanäle 1 und 2 auf 0 setzen (Initialisieren).
      setArrayChannelValue(ref array, 1, 0);
      //? setArrayChannelValue(ref array, 2, 0);
      // Suche beginnt an der Startposition
      pos = start;
      do { //Y-Schleife
        do { // X-Schleife
          //  Handelt es sich um einen Punkt des Items bzw. der Segmentfläche?
          if (array[0, pos.Y, pos.X] == segmentIntensity
                    && array[1, pos.Y, pos.X] == 0) {
            start = pos;
            chaincode.Add((uint)start.X); // Startpunkt
            chaincode.Add((uint)start.Y);
            // Der Startpunkt des Objektes wird auf den Kanälen 1 und 2 markiert.
            array[1, pos.Y, pos.X] = segmentId;
            //? array[2, pos.Y, pos.X] = segmentId;
            // Der Merker wird auf false gesetzt, damit die Schleifen jetzt verlassen werden können.
            suchen = false;
          }
          pos.X += 1;
          // Wurde Startpunkt gefunden?
        } while (pos.X < array.GetLength(2) && suchen);
        pos.X = 0;
        pos.Y += 1;
        // Wurde der Startpunkt gefunden?
      } while (pos.Y < array.GetLength(1) && suchen);
      // Folge der Kontur vom Startpunkt ausgehend bis zum Startpunkt (Startpunkt=Zielpunkt)
      // Die Konturverfolgung beginnt beim Nachbarn 1. Er ist kein Segmentpunkt,
      // da er links von der gefundenen Kontur liegt.
      Point chainPoint = new Point(start.X, start.Y);
      uint s = 0;
      uint s_check = s;
      // Zyklus von 8 verhindert eine Suche ins Unendliche.
      //+ uint zyklus = 0;
      // Konturverfolgung
      do {
        do { // suche Konturpunkt
          s += 1;
          //+ zyklus += 1;
          // s_check wächst kontinuierlich um 1, muss auf das Intervall [0, 8[ reduziert werden.
          s_check = s % 8;
          neighbor = new Point(chainPoint.X + shift[s_check, 0], chainPoint.Y + shift[s_check, 1]);
        }
        while (array[0, neighbor.Y, neighbor.X] != segmentIntensity); //+ && zyklus <= 8);
        //+ zyklus = 0;
        // Konturpunkt gefunden
        array[1, neighbor.Y, neighbor.X] = segmentId; // Markiere den Konturpunkt
        array[2, neighbor.Y, neighbor.X] = segmentId;
        // Konturverlauf der Liste hinzufügen.
        chaincode.Add(s_check);
        // Geht einen oder zwei Schritte zurück, abhängig vom Nachbarn (Neben-Nachbar, Diagonalnachbar).
        s -= 3; // Nebenpunkt
        if (neighbor.X != chainPoint.X && neighbor.Y != chainPoint.Y) {
          s -= 1; // Diagonalpunkt
        }
        chainPoint = neighbor;
      }
      // Ist die Kontur bis zum Startpunkt geschlossen?
      while (chainPoint.X != start.X || chainPoint.Y != start.Y);
      // Füllen der Kontur des Kanal 1
      //- itemFill(ref array, segmentId, 1); // erst später vollständig
      // Rand 1 wieder entfernen.
      array = arrayUnpadding(array);
      // Startpunkt des Chaincodes um -1 verschieben, da jetzt ohne Rand
      chaincode[0] -= 1;
      chaincode[1] -= 1;
      return chaincode;
    }


    /// <summary> 
    /// 7.2 Füllt umrandete Segmentflächenfläche des Chain-Arrays mit der übergebenen Id.
    /// (Einfacher aber nicht perfekter Algorithmus)
    /// Das Vermeiden von Schatten: 
    /// Je komplizierter die Geometrien der Objekte umso mehr Aufwand.
    /// Kanal 0 ist das Original
    /// Kanal 1 ist Umrandung
    /// <param name="array"></param> Array mit Objekt.
    /// <param name="segmentId"></param> Identifikationsnummer Id
    /// <param name="c"></param> Kanalnummer
    public void itemFill(ref float[,,] array, int segmentId = 1, int c = 1) {
      // a) von rechts unten nach links oben
      for (int x = array.GetLength(2) - 2; x > 0; x--)
        for (int y = array.GetLength(1) - 2; y > 0; y--) {
          bool item = false;
          bool idItem = false;
          // Ist dieses Element kein Hintergrund?
          if (array[0, y, x] > 0)
            item = true;
          // Sind diese Elemente Segmentflächenelemente?
          if (array[c, y, x + 1] == segmentId || array[c, y + 1, x] == segmentId ||
                                                 array[c, y + 1, x + 1] == segmentId)
            idItem = true; // Vorgänger: Das Element besitzt eine Id.
          // Segment und zugehörig?
          if (item && idItem)
            // Dann muss dieses Element auch dazugehören.
            array[c, y, x] = segmentId;
        }

      // b) von links unten nach rechts oben
      for (int x = 1; x < array.GetLength(2); x++)
        for (int y = array.GetLength(1) - 2; y > 0; y--) {
          bool item = false;
          bool idItem = false;
          if (array[0, y, x] > 0)
            item = true;
          if (array[c, y, x - 1] == segmentId || array[c, y + 1, x] == segmentId ||
                                                                  array[c, y + 1, x - 1] == segmentId)
            idItem = true;
          if (item && idItem)
            array[c, y, x] = segmentId;
        }

      // c) von links oben nach rechts unten
      for (int y = 1; y < array.GetLength(1); y++)
        for (int x = 1; x < array.GetLength(2); x++) {
          bool item = false;
          bool idItem = false;
          if (array[0, y, x] > 0)
            item = true;
          if (array[c, y, x - 1] == segmentId || array[c, y - 1, x] == segmentId ||
                                                                  array[c, y - 1, x - 1] == segmentId)
            idItem = true;
          if (item && idItem)
            array[c, y, x] = segmentId;
        }

      // d) von rechts oben nach links unte
      for (int y = 1; y < array.GetLength(1); y++)
        for (int x = array.GetLength(2) - 2; x > 0; x--) {
          bool item = false;
          bool idItem = false;
          if (array[0, y, x] > 0)
            item = true;
          if (array[c, y, x + 1] == segmentId || array[c, y - 1, x] == segmentId ||
                                                                   array[c, y - 1, x + 1] == segmentId)
            idItem = true;
          if (item && idItem)
            array[c, y, x] = segmentId;
        }
    }



    /// <summary>
    ///  7.3 Setzt alle Werte eines Kanals eines Arrays auf einen festen Wert.
    /// </summary>
    /// <param name="array"></param> 3D-Array
    /// <param name="canal"></param> Kanal, Layer, Bildebene
    /// <param name="value"></param> Wert, der gesetzt werden soll.
    public void setArrayChannelValue(ref float[,,] array, int canal = 1, float value = 0) {
      for (var y = 0; y < array.GetLength(1); y++)
        for (var x = 0; x < array.GetLength(2); x++)
          array[canal, y, x] = value;
    }

    /// <summary>
    /// 7.4 Array um einen Rand erweitern (Aufplustern).
    /// - Alle Kanäle
    /// </summary>
    /// <param name="array"></param> Bildarray
    /// <param name="borderValue"></param> Wert mit dem der Rand aufgefüllt wird.
    /// <returns></returns> 3D-Array mit einem zusätzlichen Rand
    public float[,,] arrayPadding(float[,,] array, float borderValue = 0) {
      float[,,] paddingArray = new float[3, array.GetLength(1) + 2, array.GetLength(2) + 2];
      // Padding (um Rand größeres Array)
      for (int c = 0; c < 3; c++)
        for (int y = 0; y < paddingArray.GetLength(1); y++)
          for (int x = 0; x < paddingArray.GetLength(2); x++)
            paddingArray[c, y, x] = borderValue;
      // Übertragen der Werte der Basismatrix
      for (int c = 0; c < 3; c++)
        for (int y = 0; y < array.GetLength(1); y++)
          for (int x = 0; x < array.GetLength(2); x++)
            paddingArray[c, y + 1, x + 1] = array[c, y, x];
      return paddingArray;
    }

    /// <summary>
    /// 7.5 Entfernt den Rand der Breite 1 eines 3D-Arrays
    /// über alle Kanäle
    /// </summary>
    /// <param name="paddingArray"></param> Array mit Rand
    /// <returns></returns> Array ohne Rand
    public float[,,] arrayUnpadding(float[,,] paddingArray) {
      float[,,] array = new float[3, paddingArray.GetLength(1) - 2, paddingArray.GetLength(2) - 2];
      // Übertragen der Werte
      for (int c = 0; c < 3; c++)
        for (int y = 0; y < array.GetLength(1); y++)
          for (int x = 0; x < array.GetLength(2); x++)
            array[c, y, x] = paddingArray[c, y + 1, x + 1];
      return array;
    }


    /// <summary>
    /// 7.6 Verschiebt die Startkoordinaten der Freeman-Code.
    /// </summary>
    /// <param name="chaincodes"></param> Doppelliste mit Freemancode
    public void pointsUnpadding(ref List<List<uint>> chaincodes) {
      for (int i = 1; i < chaincodes.Count; i++) {
        chaincodes[i][0] -= 1;
        chaincodes[i][1] -= 1;
      }
    }


    /// <summary>
    /// 7.7 Verschiebt den Startpunkt der Freemen-Codes um den zu entfernenden Rand auszugleichen.
    /// </summary>
    /// <param name="chencode"></param> Freemencode.
    /// <returns></returns> Verschobener Freemencode.
    public List<uint> chencodesUnpadding(List<uint> chencode) {
      chencode[0] -= 1;
      chencode[1] -= 1;
      return chencode;
    }

    #endregion

    #region 8. Umfang auf Basis des Kettenkodes

    /// <summary> 
    /// 8.1 Berechnet den Umfang in mm (Si-Einheit) für einen Kettencode
    /// unter Verwendung der Kameraparameter und der Entfernung.
    /// </summary>
    /// <param name="kette"></param> Kettencode (Freeman-Code).
    /// <param name="g"></param> Abstand des Objektes von der Kamera in mm.
    /// <param name="cam"></param> Parameter der verwendeten Kamera.
    /// <returns></returns> Umfang einer Kontur in mm.
    public float umfangSi(List<uint> kette, float g, CamParameter cam) {
      // Abstand für nebeneinander liegende Pixel in mm.
      float difPixel = (cam.w + cam.h) / (cam.w_p + cam.h_p);
      difPixel *= ((g - cam.f) / cam.f);
      // Abstand zwischen diagonal liegende Pixel in mm
      float difDiagonalPixel = (float)(difPixel * Math.Sqrt(2));
      float uSi = 0;
      for (int j = 2; j < kette.Count; j++) { //Kettencodeschleife
        if (kette[j] % 2 == 0)
          uSi += difPixel; // Gerade Ziffer -> nebeneinander liegend
        else
          uSi += difDiagonalPixel; // Ungerade Ziffer -> diagonal liegend.
      }
      return uSi;
    }

    /// <summary> VARIANTE
    /// 8.2 Berechnet die Umfänge in mm
    /// auf Basis der Kettencode.
    /// </summary>
    /// <param name="ketten"></param> Kettencode (Freeman-Code).
    /// <param name="g"></param> Abstand des Objektes von der Kamera in mm.
    /// <param name="cam"></param> Parameter der verwendeten Kamera.
    /// <returns></returns> Umfänge der Objekte.
    public List<float> umfangSi(List<List<uint>> ketten, float g, CamParameter cam) {
      List<float> uSiList = new List<float>();
      uSiList.Add(0); // Liste mit Index 0 bleibt leer.
                      // Abstand für nebeneinander liegende Pixel in mm.
      float difPixel = (cam.w + cam.h) / (cam.w_p + cam.h_p);
      difPixel *= ((g - cam.f) / cam.f);
      // Abstand zwischen diagonal liegende Pixel in mm
      float difDiagonalPixel = (float)(difPixel * Math.Sqrt(2));
      for (int i = 1; i < ketten.Count; i++) { // Ketten in einer Liste
        float uSi = 0;
        for (int j = 2; j < ketten[i].Count; j++) { //Kettencodeschleife
          if (ketten[i][j] % 2 == 0)
            uSi += difPixel; // Gerade Ziffer -> nebeneinander liegend
          else
            uSi += difDiagonalPixel; // Ungerade Ziffer -> diagonal liegend.
        }
        uSiList.Add((float)uSi);
      }
      return uSiList;
    }

    /// <summary> VARIANTE
    /// 8.2b Berechnet die Umfänge in Einheitspixel
    /// auf Basis des Kettencodes.
    /// Zwischen nebeneinander liegenden und diagonal liegenden Elementen
    /// wird unterschieden.
    /// </summary>
    /// <param name="ketten"></param> Liste des Kettencodes
    /// <returns>"uEpList"</returns> Umfänge der Objekte in Einheitspixel
    public List<float> umfangEp(List<List<uint>> ketten) {
      List<float> uEpList = new List<float>();
      uEpList.Add(0); // Liste mit Index 0 bleibt leer.
      // Abstand zwischen diagonal liegende Pixel als Einheitspixel
      float difDiagonalPixel = (float)Math.Sqrt(2);
      // Ketten der Liste (id=0 ist Hintergrund)
      for (int id = 1; id < ketten.Count; id++) { // Ketten in einer Liste
        float uEp = 0;
        // Kettencodeschleife mit Startpunkt und Freemancodes
        for (int j = 2; j < ketten[id].Count; j++) {
          // Element geradzahlig?
          if (ketten[id][j] % 2 == 0)
            uEp += 1; // Gerade Ziffer -> nebeneinander liegend
          else
            uEp += difDiagonalPixel; // Ungerade Ziffer -> diagonal liegend.
        }
        uEpList.Add((float)uEp);
      }
      return uEpList;
    }
    #endregion


    ///------------------------------------------------------
  }
}
