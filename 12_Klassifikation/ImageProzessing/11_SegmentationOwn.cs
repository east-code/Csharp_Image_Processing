/* Klasse SegmentationOwn: Detektiert Segmentflächen abgebilteter Gegenstände (Objekte).
 * Verfasser: Horst-Christian Heinke
 * Datum: 19.05.2022
 * Letzte Änderung: 15.01.2023
 * Germany, Sachsen-Anhalt, Magdeburg
 * */

using System;
using System.Collections.Generic;
using System.Drawing;
using ARRAY = ImageProcessing.ArrayOwn;
using FORM = System.Windows.Forms;

namespace ImageProcessing {



  internal class SegmentationOwn {

    #region 1. Instanzvariablen, Konstruktor 
    FORM.RichTextBox richTextBox;// Informationen werden hinterlegt.
    public SegmentationOwn(FORM.RichTextBox richTextBox) {
      this.richTextBox = richTextBox;
    }

    /// <summary>
    /// Neighbor indices as a shift in i, j
    /// </summary>
    public int[,] shift = { { -1, 0 }, { -1, -1 }, { 0, -1 }, { 1, -1 }, { 1, 0 }, { 1, +1 }, { 0, +1 }, { -1, +1 } };

    public int[,] shift_unclock = { { 1, 0 }, { 1, -1 }, { 0, -1 }, { -1, -1 }, { -1, 0 }, { -1, +1 }, { 0, +1 }, { 1, +1 } };



    /// <summary>
    /// Neighbor indices as a shift in i, j
    /// </summary>
    public int[,] shift4 = { { -1, 0 }, { 0, -1 }, { 1, 0 }, { 0, +1 } };

    /// <summary>
    /// Transformiet zweidimensionales Feld in ein eindimensionales Feld
    /// mit einer Punktstruktur.
    /// </summary>
    /// <param name="shift"></param> Zweidimensionales Feld
    /// <returns></returns> Eindimensionales Punktefeld.
    public Point[] toPoints(int[,] shift) {
      Point[] nachbar = new Point[shift.GetLength(0)];
      for (int s = 0; s < shift.GetLength(0); s++) {
        nachbar[s].X = shift[s, 0];
        nachbar[s].Y = shift[s, 1];
      }
      return nachbar;
    }

    #endregion

    #region 2. Eindimensionale Strukturfelder
    /// <summary> TIEFPASS
    /// 2.1 Eindimensionales Gauß-Strukturfeld beliebiger Größe
    /// zum Glätten diskreter Funktionswerte
    /// </summary>
    /// <param name="size"></param> Anzahl der Elemente
    /// <returns></returns> 1D-Feld mit Dichtewerten der Gaußverteilung.
    public float[] G_Gauß1D(int size = 3) {
      float sigma = (float)size / 6;
      // Zentralpixelversatz
      int m = (int)(size - 1) / 2;
      // Eindimensionales Gaußfeld
      float[] G = new float[size];
      // Diskrete Werte des Exponentialteils der Dichteverteilung.
      for (int x = -m; x <= m; x++)
        G[x + m] = (float)Math.Exp(-(x * x) / 2.0 / sigma / sigma);
      normalization(ref G);
      return G;
    }

    /// <summary>
    /// 2.2 Eindimensionales Gauß-Strukturfeld der Größe size=3
    /// zum Glätten diskreter Funktionswerte.
    /// </summary>
    /// <returns></returns> Strukturtape
    public float[] Gx_Gauß() {
      float[] Gx = { (float)0.106506974, (float)0.786986052, (float)0.106506974 };
      return Gx;
    }

    /// <summary>
    /// 2.3 Eindimensionales Binomialstrukturfeld der Größe size=3
    /// zum Glätten diskreter Funktionswerte.
    /// </summary>
    /// <returns></returns> Strukturtape
    public float[] Bx_Binomial() {
      float[] Bx = { (float)0.25, (float)0.5, (float)0.25 };
      return Bx;
    }

    /// <summary>
    /// 2.4 Normiert ein 1D-Feld (ein Strukturfeld),
    /// sodass die Summe aller Elemente = 1 ist.
    /// </summary>
    /// <param name="feld1D"></param> Strukturfeld
    ///                        ref -> Normiertes Strukturfeld
    public static void normalization(ref float[] feld1D) {
      // Summe aller Elemente des 2D-Arrays.
      float sum = 0;
      for (int i = 0; i < feld1D.GetLength(0); i++)
        sum += feld1D[i];
      // Normierung der Elemente des Feldes.
      for (int i = 0; i < feld1D.GetLength(0); i++) {
        feld1D[i] /= sum;
      }
    }

    /// <summary>
    /// 2.5 Gleichverteilung
    /// </summary>
    /// <param name="size"></param> Größe des Strukturfeldes
    /// <returns></returns>
    public float[] Gl_Gleich1D(int size = 5) {
      // Zentralpixelversatz
      int m = (int)(size - 1) / 2;
      // Eindimensionales Gaußfeld
      float[] Gl = new float[size];
      // Alle Elemente mit gleicher Wichte.
      for (int x = -m; x <= m; x++)
        Gl[x + m] = (float)(1.0 / size);
      return Gl;
    }


    /// <summary> HOCHPASS
    /// 2.5 Strukturtape für erste Ableitung
    /// </summary>
    /// <returns></returns> Strukturtape
    public float[] Sx_Sobel() {
      float[] Sx = { -2, 0, +2 };
      return Sx;
    }


    /// <summary>
    /// 2.6 Strukturtape für erste Ableitung mit 
    /// parametrierbaren Elementen.
    /// </summary>
    /// <param name="size"></param> Länge des Tapes
    /// <returns></returns> Strukturtape
    public float[] Ax_Anstieg(int size = 5) {
      int m = (size - 1) / 2;
      float[] Ax = new float[size];
      for (int x = 0; x < size; x++)
        Ax[x] = x - m;
      return Ax;
    }

    /// <summary>
    /// 2.7 Strukturtape für zweite Ableitung
    /// </summary>
    /// <param name="c"></param> Zentralelement
    /// <returns></returns> Strukturtape
    public float[] Lx_Laplace(int c = 6) {
      float[] L = { -1, -2, c, -2, -1 };
      return L;
    }


    #endregion

    #region 3. Filter
    /// <summary>
    /// 3.1 Lineares Filter für diskrete Funktionswerte eines Kanals.
    /// Eine typische Anwendung ist die Filterung des Histogramms.
    /// </summary>
    /// <typeparam name="Tin"></typeparam> Datentyp des eindimensionalen Eingangsarrays.
    /// <param name="array1D"></param> Eingangsarray, Eingangsfeld
    /// <param name="strukturFeld"></param> Strukturfeld, Strukturtape
    /// <param name="marginValue"></param> Wert des erweiterten Randes.
    /// <param name="step"></param> Schrittweite
    /// <returns></returns> Gefilterte Werte des eindimensionalen Feldes
    public float[] linearFilter1D<Tin>(Tin[] array1D, float[] strukturFeld, int marginValue = 0, int step = 1) {
      // Ergenisarray deklarieren
      float[] filterArray1D = new float[(array1D.GetLength(0)) / step];
      // Der hinzuzufügende Rand ergibt sich aus der Größe des Faltungskerns. 
      int m = (int)(strukturFeld.GetLength(0) - 1) / 2;
      // Array um einen Rand ausdehnen.
      array1D = ARRAY.padding<Tin>(array1D, marginValue, m);
      // Schleife für Kanal/Kanäle in x-Richtung.
      for (int x = m; x < array1D.GetLength(0) - m; x += step) { // -> Rand beachten
        // Schleife des Tapes.
        float I = 0; // Intensitätshäufigkeit 
        for (int i = -m; i <= m; i++) {
          // Verarbeitung des Filtertapes.
          float element = (float)Convert.ChangeType(array1D[x + i], typeof(float));
          I += element * strukturFeld[i + m];
        }
        // Filterwert setzen
        filterArray1D[(int)((x - m) / step)] = I;
      }
      return filterArray1D;//    
    }

    /// <summary> TIEFPASS- UND HOCHPASSFILTER
    /// 4.2 Lineares Filter, geeignet als Tiefpassfilter und als Hochpassfilter.
    /// Der Faltungskern beeinflusst die Filterwirkung.
    /// </summary>
    /// <typeparam name="Tin"></typeparam> Datentyp der Elemente des Eingangsarrays.
    /// <param name="array2D"></param> zweidimensionales Eingangsarray
    /// <param name="strukturFeld"></param> Struktufeld, Strukturtape
    /// <param name="marginValue"></param> Wert für Felderweiterung
    /// <param name="step"></param> Schritweite
    /// <returns></returns> gefiltertes Array
    public float[,] linearFilter1D<Tin>(Tin[,] array2D, float[] strukturFeld, int marginValue = 0, int step = 1) {
      // Ergebnisarray deklarieren
      float[,] filterArray2D = new float[array2D.GetLength(0), (array2D.GetLength(1)) / step];
      // Der hinzuzufügende Rand ergibt sich aus der Größe des Faltungskerns. 
      int m = (int)(strukturFeld.GetLength(0) - 1) / 2;
      // Array um einen Rand ausdehnen.
      array2D = ARRAY.padding<Tin>(array2D, marginValue, m);
      // Doppelschleife für Kanal/Kanäle in x-Richtung.
      for (int c = 0; c < array2D.GetLength(0); c++)
        for (int x = m; x < array2D.GetLength(1) - m; x += step) { // -> Rand beachten
                                                                   // Schleife des Tape.
          float I = 0; // Intensitätshäufigkeit 
          for (int i = -m; i <= m; i++) {
            // Verarbeitung des Filtertapes.
            float element = (float)Convert.ChangeType(array2D[c, x + i], typeof(float));
            I += element * strukturFeld[i + m];
          }
          // Filterwert setzen
          filterArray2D[c, (int)((x - m) / step)] = I;
        }
      return filterArray2D;//      filterArray2D;
    }


    /// <summary> 
    /// 3.3 Medianfilter - Nichtlineares Filter
    /// </summary>
    /// <param name="array"></param> 3D-Array
    /// <param name="size"></param> Größe des Faltungskerns
    /// <param name="borderValue"></param> Werte des hinzugefügten Randes
    /// <param name="step"></param> Schritweiter
    /// <returns></returns> Gefiltertes Bild
    public float[,] medianFilter1D<T>(T[,] array2D, int size = 3, double marginValue = 0, int step = 1) {
      // Ergenisarray deklarieren
      float[,] rankingArray = new float[array2D.GetLength(0), (array2D.GetLength(1)) / step];
      int m = (int)(size - 1) / 2; // Der hinzuzufügende Rand ergibt sich aus der Größe des Faltungskerns. 
      array2D = ARRAY.padding<T>(array2D, (float)marginValue, m);
      for (int c = 0; c < array2D.GetLength(0); c++) {  // Für jeden Kanal Kanäle
        for (int x = m; x < array2D.GetLength(1) - m; x += step) { // -> Rand beachten
                                                                   // Kernel
          float[] I = new float[size];  // Intensität eines Elementes
          for (int i = -m; i < +m; i++) {
            //I[i + m] = (float) array2D[c, x + i];
            I[i + m] = (float)Convert.ChangeType(array2D[c, x + i], typeof(float));
          }
          Array.Sort(I);
          rankingArray[c, (int)(x - m / step)] = I[m]; //Median
                                                       // rankingArray[c, (int)(x - m / step)] = I[size - 1]; // Max
                                                       //rankingArray[c, (int)(x - m / step)] = I[0]; //Min
        }
      }
      return rankingArray;
    }
    #endregion

    #region 4. Extrem
    /// <summary>
    ///  4.1 Ermittelt die Maxima-Intensitäten
    /// </summary>
    /// <param name="array"></param> Eindimensionales Feld, Histogramm
    /// <returns></returns> Liste aller Maxima der Funktionswerte.
    public List<byte> giveListMaxima(float[] array) {
      List<byte> maxima = new List<byte>();
      for (byte x = 0; x < array.GetLength(0) - 1; x++) {
        bool max = Math.Sign(array[x]) > Math.Sign(array[x + 1]);
        if (max) {
          maxima.Add(x);
        }
      }
      return maxima;
    }

    /// <summary>
    /// 4.2 Ermittelt das Minimum der Intensität zwischen zwei lokalen Maxima.
    /// Eine Anwendung sind die Histogrammwerte.
    /// </summary>
    /// <param name="array"></param> Eindimensionales Feld
    /// <param name="x0"></param> Untere Intervallgrenze, Maximum
    /// <param name="x1"></param> Obere Intervallgrenze, nachfolgendes Maximum
    /// <returns></returns> Minimum zwischen zwei Maxima (Intervall)
    public byte giveMinimum(float[] array, byte x0, byte x1) {
      byte minimum = x0;
      for (byte x = x0; x < x1 - 1; x++) {
        // Vorzeichenwechsel von - nach +
        bool min = false;
        min = Math.Sign(array[x]) < Math.Sign(array[x + 1]);
        if (min) {
          minimum = x;
        }
      }
      return minimum;
    }



    /// <summary>
    /// 4.3 Speicherstruktur für einen Extrempunkt
    /// </summary>
    public struct PointExtrem {
      public int c; // Canal, Kanal
      public int x;  // Abszissenkoordinate
      public float y; // Ordinatenwert y'
      public bool min; // Minimum=true, Maximum=false
    }
    /// <summary>
    /// 4.4 Liefert die Extremwerte auf Basis der ersten Ableitung einer
    /// diskreten Funktion. Diese Methode wertet die Richtungen der Nullduchgänge.
    /// Anders gesagt: Ein Wechsel von plus nach minus und umgekehrt.
    /// </summary>
    /// <param name="array"></param> zweidimensionales Feld, [Kanal c, Ordinate y]
    /// der Index ist der Abszissenwert x
    /// <returns></returns>
    public List<PointExtrem> getExtrem(float[,] array) {
      List<PointExtrem> extrem = new List<PointExtrem>();
      for (var c = 0; c < array.GetLength(0); c++)
        for (var x = 1; x < array.GetLength(1) - 1; x++) {
          bool max = Math.Sign(array[c, x]) > Math.Sign(array[c, x + 1]);
          bool min = Math.Sign(array[c, x]) < Math.Sign(array[c, x + 1]);
          PointExtrem p = new PointExtrem();
          if (min) {
            p.c = c;
            p.x = x;
            p.y = array[c, x];
            p.min = true;
            extrem.Add(p); //(float) c, (float) array[c, x])
          }
          if (max) {
            p.c = c;
            p.x = x;
            p.y = array[c, x];
            p.min = false;
            extrem.Add(p); //(float) c, (float) array[c, x])
          }

        }
      return extrem;
    }

    /// <summary>
    /// 4.5 Liefert die Extremwerte auf Basis der ersten Ableitung einer
    /// diskreten Funktion. Diese Methode wertet die Richtungen der Nullduchgänge.
    /// Anders gesagt: Ein Wechsel von plus nach minus und umgekehrt.
    /// </summary>
    /// <param name="array"></param> Feld, Histogramm[c, y'], Index <=> x
    /// <returns></returns> Liste [canal, abszisse, Ordinate, min/max]
    public List<float[]> getExtremList(float[,] array) {
      List<float[]> extreme = new List<float[]>();
      for (var c = 0; c < array.GetLength(0); c++) {
        for (var x = 0; x < array.GetLength(1) - 1; x++) {
          bool min = Math.Sign(array[c, x]) < Math.Sign(array[c, x + 1]);
          bool max = Math.Sign(array[c, x]) > Math.Sign(array[c, x + 1]);
          float[] p = new float[4];
          if (min) {
            p[0] = c;
            p[1] = x;
            p[2] = (float)array[c, x];
            p[3] = 0;
            extreme.Add(p);
          }
          if (max) {
            p[0] = c;
            p[1] = x;
            p[2] = (float)array[c, x];
            p[3] = 1;
            extreme.Add(p);
          }
        }
      }
      return extreme;
    }

    /// <summary> ÜBERLADUNG
    /// 4.6 Liefert die Extremwerte auf Basis der ersten Ableitung einer
    /// diskreten Funktion. Diese Methode wertet die Richtungen der Nullduchgänge.
    /// Anders gesagt: Ein Wechsel von plus nach minus und umgekehrt.
    /// </summary>
    /// <param name="array"></param> 1D-Feld, Histogramm[ y'], Index <=> x
    /// <returns></returns> Liste [canal, abszisse, Ordinate, min/max]
    public List<float[]> getListExtrema(float[] array) {
      List<float[]> extreme = new List<float[]>();
      for (var x = 0; x < array.GetLength(0) - 1; x++) {
        bool min = Math.Sign(array[x]) < Math.Sign(array[x + 1]);
        bool max = Math.Sign(array[x]) > Math.Sign(array[x + 1]);
        float[] p = new float[4];
        if (min) {
          p[0] = -1;
          p[1] = x;
          p[2] = (float)array[x];
          p[3] = 0;
          extreme.Add(p);
        }
        if (max) {
          p[0] = -1;
          p[1] = x;
          p[2] = (float)array[x];
          p[3] = 1;
          extreme.Add(p);
        }

      }
      return extreme;
    }
    #endregion

    #region 5. Schwellwert-Iteration
    /// <summary>
    /// 5.1 Liefert die optimierten Schwellwerte aller Kanäle (R, G, B) auf Basis eines Histogramms.
    /// </summary>
    /// <typeparam name="T"></typeparam> Datentyp der Elemente des Arrays
    /// <param name="array2D"></param> Eingangsarray H=[c,I]
    /// <param name="arrayFiltered2D"></param> gefiltertes Array
    /// <param name="arrayFiltered2D_"></param> Erste Ableitung des gefilterten Arrays.
    /// <returns></returns> Liste mit den kanalbezogenen Schwellwerten I(schwellwert)=[c]
    public List<byte> giveCanalThresholds<T>(T[,] array2D, out float[,] arrayFiltered2D, out float[,] arrayFiltered2D_, bool giveMinima = true) {
      // Maxima entsprechen Segmente des Bildarrays.
      // Filtervorgang erfolgt zyklisch bis zwei Maxima vorliegen.
      int segments = 2;
      // Liste der Minima der Kanäle
      List<byte> minima = new List<byte>();
      // Liste der Mittenwerte der Kanäle
      List<byte> median = new List<byte>();
      // gefiltertes Histogramm
      arrayFiltered2D = new float[array2D.GetLength(0), array2D.GetLength(1)];
      // Erste Ableitung des gefilterten Histogramms.
      arrayFiltered2D_ = new float[array2D.GetLength(0), array2D.GetLength(1)];
      // Kanalweise Verarbeitung
      for (int c = 0; c < array2D.GetLength(0); c++) {
        // Liste der Maxima eines Kanals
        List<byte> maxima = new List<byte>();
        // Kopie eines Kanals als Anfangszustand.
        float[] arrayFiltered1D = ARRAY.copy(array2D, c);
        // Deklaration des Feldes für die erste Ableitung
        float[] arrayFiltered1D_ = new float[array2D.GetLength(1)];
        // Zyklus zum Filtern bis zwei Maxima übrig bleiben.
        do { // Relevante Maxima bestimmen.
             // Tiefpass zum Glätten wird mehrfach angewendet.
          arrayFiltered1D = linearFilter1D(arrayFiltered1D, Gx_Gauß(), 0, 1);//Bx_Binomial()
                                                                             // Ableitung
          arrayFiltered1D_ = linearFilter1D(arrayFiltered1D, Sx_Sobel(), 0, 1);
          // Bestimme Maxima
          maxima = giveListMaxima(arrayFiltered1D_);
        } // Bis nur noch die gewünschte Anzahl der Segmente bzw. Maxima
          // vorhanden sind.
        while (maxima.Count > segments);
        // Minimum zwischen den Maxima bestimmen
        byte minimum = giveMinimum(arrayFiltered1D_, maxima[0], maxima[1]);
        // Minimum wird der kanalübergreifenden Liste hinzugefügt.
        minima.Add(minimum);
        // Mittenwert wird der kanalübergreifenden Liste hinzugefügt.
        median.Add((byte)((maxima[0] + maxima[1]) / 2));
        // Ergänzung der kanalübergreifenden Arrays.
        ARRAY.extend(ref arrayFiltered2D, arrayFiltered1D, c);
        ARRAY.extend(ref arrayFiltered2D_, arrayFiltered1D_, c);
      } // for c
        // Minima zwischen den Maxima sind die optimierten Schwellwerte.
      if (giveMinima)
        return minima;
      else
        return median;
    }


    /// <summary> IN WORK
    /// 5.2 Schwellwertiteration für mehr als zwei Segmente.
    /// (Mehrbereichsbilder)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array2D"></param>
    /// <param name="arrayFiltered2D"></param>
    /// <param name="arrayFiltered2D_"></param>
    /// <param name="segments"></param>
    /// <returns></returns>
    public List<List<byte>> giveThresholds<T>(T[,] array2D, out float[,] arrayFiltered2D, out float[,] arrayFiltered2D_, int segments) {
      List<byte> maxima = new List<byte>();
      List<List<byte>> minimaList = new List<List<byte>>();
      arrayFiltered2D = new float[array2D.GetLength(0), array2D.GetLength(1)];
      arrayFiltered2D_ = new float[array2D.GetLength(0), array2D.GetLength(1)];
      // Kanalweise Verarbeitung
      for (int c = 0; c < array2D.GetLength(0); c++) {
        float[] arrayFiltered1D = ARRAY.copy(array2D, c);
        float[] arrayFiltered1D_ = new float[array2D.GetLength(1)];
        List<byte> minima = new List<byte>();
        do { // Relevante Maxima bestimmen.
             // Tiefpass zum Glätten wird mehrfach angewendet.
          arrayFiltered1D = linearFilter1D(arrayFiltered1D, Gx_Gauß(), 0, 1);
          // Ableitung
          arrayFiltered1D_ = linearFilter1D(arrayFiltered1D, Sx_Sobel(), 0, 1);
          // Bestimme Maxima
          maxima = giveListMaxima(arrayFiltered1D_);
        } // Bis nur noch die gewünschte Anzahl der Segmente abgebildet wird.
        while (maxima.Count > segments);
        // Minima zwischen den Maxima bestimmen
        for (int i = 0; i < maxima.Count - 1; i++) {
          byte minimum = giveMinimum(arrayFiltered1D_, maxima[i], maxima[i + 1]);
          minima.Add(minimum);
        }
        // Zusammmenfassung der Kanäle
        ARRAY.extend(ref arrayFiltered2D, arrayFiltered1D, c);
        ARRAY.extend(ref arrayFiltered2D_, arrayFiltered1D_, c);
        minimaList.Add(minima);
      } // for c
        // Minima zwischen den Maxima sind die Schwellwerte
      return minimaList;
    }



    #endregion

    #region 6. Kantendetektion und Kantenverfolgung
    /// <summary>
    /// 6.1 Kantenverstärkung unter Verwendung des Canny Edge Operators.
    /// Die Suche erfolgt vorwärts und rückwärts.
    /// Kanalbelegung:
    /// </summary>
    /// <param name="array"></param> Grauwert-Array
    /// Eingang: Alle Kanäle sind identisch.
    /// Kanal c0 - Unverändertes Original
    /// Kanal c1 - Identifizierte Startpunkte
    /// Kanal c2 - Identifizierte Start- und Kantenpunkte
    /// <param name="Istart"></param> Obere Intensitätswertgrenze für Start einer Kontur
    /// <param name="Icontinue"></param> Untere Intensitätswertgrenze für Fortsetzung der Kontur
    public void cannyEdge(ref float[,,] array, double Istart, double Icontur) {
      // Versatz als Punkte - Umwandlung soll die Übersichtlichkeit verbessern.
      Point[] dNeighbor = toPoints(shift);
      // Kanal 0 wird untersucht, bleibt aber unverändert.
      // Alle Werte der Kanäle 1 und 2 werden auf 0 (Hintergrund) gesetzt.
      ARRAY.setArrayChannelValue(ref array, 1, 0);
      ARRAY.setArrayChannelValue(ref array, 2, 0);
      // Doppelschleife zur Verarbeitung der Bildarraypunkte.
      for (int y = 1; y < array.GetLength(1) - 1; y++)
        for (int x = 1; x < array.GetLength(2) - 1; x++) {
          // Ist der Punkt ein Kanten-Startpunkt (>T1)?
          if (array[0, y, x] >= Istart) {
            // Elemente der Kanäle 1 und 2 werden markiert.
            array[1, y, x] = 1;
            array[2, y, x] = 1;
          }
          else {
            // Handelt es sich nicht um einen Startpunkt dann Kontrolle in Bezug auf
            // Kanten-Folgepunkt. Ist Intensitätswert des Basispixels >Schwellwert (>T2)
            // und Zentralpixel bisher kein Kantenpunkt?
            if (array[0, y, x] >= Icontur & array[2, y, x] == 0)
              // Dann kontrolliere die Nachbarpunkte mit einer Schleife.
              for (int s = 0; s < dNeighbor.GetLength(0); s++)
                // Ist einer der Nachbarn bereits ein Kantenpunkt oder ein Kantenstartpunkt?
                if (array[2, y + dNeighbor[s].Y, x + dNeighbor[s].X] == 1)
                  // Dann ist das Zentralpixel auch Kantenpixel und erhält den Wert 1.
                  array[2, y, x] = 1;
          } // end else
        } // end for

      // Rückwärtslauf für vorausliegende Kantenpunkte
      for (int y = array.GetLength(1) - 2; y > 0; y--)
        for (int x = array.GetLength(2) - 2; x > 0; x--) {
          // Handelt es sich um einen Kantenpunkt, der noch nicht gefunden wurde?
          if (array[0, y, x] >= Icontur & array[2, y, x] == 0)
            // Dann kontrolliere die Nachbarpunkte mit einer Schleife.
            for (int s = 0; s < dNeighbor.GetLength(0); s++)
              // Ist eines der Nachbarn bereits ein Kantenpunkt oder ein Kantenstartpunkt?
              if (array[2, y + dNeighbor[s].Y, x + dNeighbor[s].X] == 1)
                // Dann ist das Zentralpixel auch Kantenpixel und erhält den Wert 1.
                array[2, y, x] = 1;
        } // end for
    }

    #endregion

    #region 7. Segmentumrandungen füllen 
    /// <summary> - IN WORK
    /// 7.1 Füllt Segmentflächen - 
    /// </summary>
    /// <param name="array"></param>
    /// <param name="itemId"></param>
    /// <param name="cEdge"></param>
    /// <param name="cSegmentArea"></param>
    public void fill(ref float[,,] array, int itemId = 2, int cEdge = 2, int cSegmentArea = 1) {
      // von links oben nach rechts unten
      for (int y = 1; y < array.GetLength(1); y++) {
        // Zähler für Kantenübergänge
        int countEdge = 0;
        for (int x = 0; x < array.GetLength(2) - 1; x++) {
          // Kantenübergang?
          if (array[cEdge, y, x] == itemId & array[cEdge, y, x + 1] != itemId)
            // Zähle Kantenübergang in der Zeile
            countEdge++;
          // Ist die Kantenanzahl ungerade?
          if (countEdge % 2 != 0)
            // Dann innerhalb der Segmentfläche
            array[cSegmentArea, y, x] = itemId;
        }
      }
    }

    /// <summary>
    /// 7.2 Liefert den Kreisintervall einer ganzenZahl ür den Intervall von 0 bos ns.
    /// Intervall [0, ns[
    /// </summary>
    /// <param name="s"></param> ganze Zahl
    /// <param name="ns"></param> exclusive obere Intervallgrenze
    /// <returns></returns> Wert innerhalb des Kreisintervalls
    private int cicleIntervall(int s, int ns = 8) {
      s = s % ns;
      if (s < 0)
        s = ns + s;
      return s;
    }
    /// <summary> ÜBERLADUNG
    /// 7.2b Liefert den Kreisintervall einer ganzenZahl ür den Intervall von 0 bos ns.
    /// Intervall [0, ns[
    /// </summary>
    /// <param name="s"></param> ganze Zahl --ref--> exclusive obere Intervallgrenze
    /// <param name="ns"></param> exclusive obere Intervallgrenze
    private void cicleIntervall(ref int s, int ns = 8) {
      s = s % ns;
      if (s < 0)
        s = ns + s;
    }


    /// <summary>
    /// 7.3 Fülle die Fläche außerhalb der Segmentfläche.
    /// Blicke aus vier Richtungen auf die Segmentfläche.
    /// Kanal 0: Original bleibt unverändert, Intensität
    /// Kanal 1: ID's werden nicht benötigt und bleiben unverändert.
    /// Kanal 2: <- Elemente außerhalb der Segmentfläche werden gesetzt.
    /// </summary>
    /// <param name="array"></param> 3D-Bildarray
    /// Kanal 0: Original bleibt unverändert, Intensität
    /// Kanal 1: ID's werden nicht benötigt und bleiben unverändert.
    /// Kanal 2: <- Elemente außerhalb der Segmentfläche werden gesetzt.
    public void fillOutside(ref float[,,] array) {
      // Blickrichtungen: Elemente des Kanals 2 bis zu einem Hindernis ausfüllen.
      // - von links nach rechts
      ARRAY.setArrayChannelValue(ref array, 2, 0);
      for (int y = 0; y < array.GetLength(1); y++)
        for (int x = 0; x < array.GetLength(2) - 1; x++) {
          if (array[0, y, x] == 0)
            if (array[0, y, x + 1] > 0)
              break;
          array[2, y, x] = 1;
        }
      // - von rechts nach links
      for (int y = 0; y < array.GetLength(1); y++)
        for (int x = array.GetLength(2) - 1; x > 0; x--) {
          if (array[0, y, x] == 0)
            if (array[0, y, x - 1] > 0)
              break;
          array[2, y, x] = 1;
        }
      // - von oben nach unten
      for (int x = 0; x < array.GetLength(2); x++)
        for (int y = 0; y < array.GetLength(1) - 1; y++) {
          if (array[0, y, x] == 0)
            if (array[0, y + 1, x] > 0)
              break;
          array[2, y, x] = 1;
        }
      // - von unten nach oben
      for (int x = 0; x < array.GetLength(2); x++)
        for (int y = array.GetLength(1) - 1; y > 0; y--) {
          if (array[0, y, x] == 0)
            if (array[0, y - 1, x] > 0)
              break;
          array[2, y, x] = 1;
        }
    }

    #endregion

    #region 0815 SONSTIGES
    /// <summary> NUR FÜR KONVEXE KONTUREN
    /// Suche einen Startpunkt innerhalb einer umrandeten Kontur.
    /// Kanal 0 - Kontur
    /// Kanal 1 - Startpunkt
    /// Kanal 2 - Startpunkt
    /// </summary>
    /// <param name="array"></param> 3D-Array
    /// <param name="itemId"></param> Zu suchende Id
    /// <param name="startGefundenemId"></param> Ist ein Startpunkt gefunden?
    /// <returns></returns>
    private Point findStartInContour(ref float[,,] array, int itemId, out bool startGefunden) {
      // Startpunkt als Merker
      Point posStart = new Point(0, 0);
      // Aktueller Punkt wird kontinuierlich überschrieben.
      Point pos = new Point(0, 0);
      // Äquidistante Bahnen abfahren...
      // Inneren Startpunkt suchen
      startGefunden = false;
      for (int y = 1; y < array.GetLength(1); y++) {
        for (int x = 1; x < array.GetLength(2); x++) {
          // Kantenübergang?
          // Ist ein Hintergrundpunkt und links und oberhalb sind Konturpunkte mit der Id.
          if (array[0, y, x] != itemId && array[0, y, x - 1] == itemId && array[0, y - 1, x] == itemId) {
            //Startpunkt
            pos = new Point(x, y);
            for (int x1 = pos.X; x1 < array.GetLength(2); x1++) {
              if (array[0, pos.Y, x1] == itemId && x1 < array.GetLength(2) - 1 && y < array.GetLength(1) - 1) {
                // rechten Konturrand gefunden
                posStart = pos;
                array[1, posStart.Y, posStart.X] = itemId;
                // array[2, posStart.Y, posStart.X] = itemId;
                startGefunden = true;
                break;
              }
            }
          }
          // Kein Punkt gefunden
          if (posStart == new Point(0, 0) && y >= array.GetLength(1) - 1) {
            startGefunden = false;
            return new Point(0, 0);
          }
          if (startGefunden)
            break;
        }
        if (startGefunden)
          break;
      }
      return posStart;
    }

    /// <summary>
    /// Füllt eine umrandete Fläche äquidistant von der Umrandung ausgehend
    /// nach innen.
    /// </summary>
    /// <param name="array"></param>
    /// <param name="itemId"></param>
    /// <param name="cFill"></param>
    public bool fillAequdist(ref float[,,] array, int itemId) {
      // Aktueller Punkt wird kontinuierlich überschrieben.
      Point pos = new Point(0, 0);
      // Finde die erste mögliche Position innerhalb der Kontur
      pos = findStartInContour(ref array, itemId, out bool startGefunden);
      if (!startGefunden)
        return startGefunden;
      // Verschiebung zwischen aktuellem Punkt und Nachbarpunkt
      Point[] dNeighbor = toPoints(shift);
      // Nacbarpunkt, Vierer-Nachbarpunkt
      Point posNeighbor, posNeighbor_1 = new Point();
      // Suchrichtungen in Relation zur aktuellen Richtung s nach Priorität.
      // Das letzte Element ist ein Dummy und ist dem 0-Elent gleich.
      int[] dS = { 0, +1, +2, +3, -1, -2, -3, 0 };
      int s0 = 4; // Richtung, direction
      int s = 4;  // Test-Richtung
      uint cyles = 0; // Nur Hilfsgröße für vorzeiztigen Programmabbruch.
      bool richtung = false;
      bool rand = false;
      bool findPoint = true;
      do {
        try {
          do {
            richtung = true;
            rand = true;
            // Mögliche Richtung analysieren und neu festlegen.
            for (int i = 0; i < dS.Length; i++) {
              s = cicleIntervall(s0 + dS[i]);
              int s1 = cicleIntervall(s - 1);
              posNeighbor = new Point(pos.X + dNeighbor[s].X, pos.Y + dNeighbor[s].Y);
              richtung = array[1, posNeighbor.Y, posNeighbor.X] == 0;
              posNeighbor_1 = new Point(pos.X + dNeighbor[s1].X, pos.Y + dNeighbor[s1].Y);
              rand = array[1, posNeighbor_1.Y, posNeighbor_1.X] > 0;
              if ((richtung && rand)) {
                break;
              }
              if (i >= dS.Length - 1) {
                findPoint = false;
                break;
              }
            }
          } while (!(richtung && rand) && findPoint);
          s0 = s;
          pos = new Point(pos.X + dNeighbor[s0].X, pos.Y + dNeighbor[s0].Y);
          cyles++;
          // Pixel der konturierten Fläche des Kanal 1
          array[1, pos.Y, pos.X] = itemId;
          array[2, pos.Y, pos.X] = itemId;//++
          richTextBox.Text += pos; //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

        }// try
        catch (Exception ex) {
          richTextBox.Text += ex + " - Unterdrückter Indexfehler in fillAequdist";
          break;
        }
      } while (findPoint);
      return startGefunden;
    } // fill

    /// <summary> NUR FÜR KONTUREN OHNE VERZWEIGUNGEN UND SACKGASSEN
    /// Ergänzt Kontur um zusätzliche Punkte an Ecken und Schrägen,
    /// um eine geschlossenen Kontur zu erhalten.
    /// Achtung: Sehr unregelmäßige Konturzüge führen zu einem Abbruch.
    /// </summary>
    /// <param name="array"></param>
    /// <param name="itemId"></param>
    /// <param name="cFill"></param>
    public void fillChain(ref float[,,] array, int itemId = 1, int cFill = 2) {
      // Verschiebung zwischen aktuellem Punkt und Nachbarpunkt
      Point[] dNeighbor = toPoints(shift);
      // Nacbarpunkt, Vierer-Nachbarpunkt
      Point posNeighbor = new Point();
      // Startpunkt als Merker
      Point posStart = new Point(1, 1);
      // Aktueller Punkt wird kontinuierlich überschrieben.
      Point pos = new Point(0, 0);
      // Äquidistante Bahnen abfahren...
      // Inneren Startpunkt suchen
      bool gefundenStart = false;
      // Abbruchbedingung, wenn keine Konturfortsetzung möglich.
      bool abbruch = false;

      int cycle = 0;
      //ARRAY.setArrayChannelValue(ref array, 1, 0);
      //ARRAY.setArrayChannelValue(ref array, 2, 0);
      for (int y = 1; y < array.GetLength(1); y++) {
        for (int x = 1; x < array.GetLength(2); x++)
          // Kantenübergang?
          if (array[0, y, x] == itemId) {
            //Startpunkt
            posStart = new Point(x, y);
            array[1, posStart.Y, posStart.X] = itemId;
            array[2, posStart.Y, posStart.X] = itemId;
            gefundenStart = true;
            break;
          }
        if (gefundenStart)
          break;
      }
      // Innenkontur-Äquidistante
      pos = posStart;
      posNeighbor = posStart;
      // Suchrichtungen in Relation zur aktuellen Richtung s nach Priorität.
      // Das letzte Element ist ein Dummy und ist dem 0-Elent gleich.
      int[] dS = { 0, +1, -1, +2, -2, +3, -3, 0 };
      int s0 = 4; // Richtung, direction
      int s = 4;  // Test-Richtung
      bool richtung = false;
      do {
        try {
          do {
            richtung = true;
            // Mögliche Richtung analysieren und neu festlegen.
            for (int i = 0; i < dS.Length; i++) {
              s = cicleIntervall(s0 + dS[i]);
              // int s1 = cicleIntervall(s - 1);
              posNeighbor = new Point(pos.X + dNeighbor[s].X, pos.Y + dNeighbor[s].Y);
              richtung = array[0, posNeighbor.Y, posNeighbor.X] == itemId;
              if (richtung)
                break;
              if (i == dS.Length - 1)
                abbruch = true;
            } // for zum Suchen
          } while (!richtung && !abbruch);
          // Diagonalpunkte?
          if (pos.X != posNeighbor.X && pos.Y != posNeighbor.Y) {
            array[cFill, posNeighbor.Y, pos.X] = itemId;
            array[cFill, pos.Y, posNeighbor.X] = itemId;
          }
          // Neue Richtung mit gefundenem Nachbarn
          s0 = s;
          pos = posNeighbor;
          // Pixel der konturierten Fläche des Kanal 1 <<<<<<<<<<<<<<<<
          richTextBox.Text += pos; //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
        }// try
        catch (Exception ex) {
          richTextBox.Text += "Error: " + ex;
          break;
        }
        cycle++;
      } while (pos != posStart && !abbruch && cycle < 10000);

    }
    #endregion 0815

  }
}
