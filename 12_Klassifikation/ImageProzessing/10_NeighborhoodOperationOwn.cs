/* Klasse NeighborhoodOperationOwn: Führt Nachbarschaftsoperationen durch. Dazu gehören das Tiefpassfilter, das Hochpassfilter, das Rangordnungsfilter und morphologische Operationen.
 * Verfasser: Horst-Christian Heinke
 * Datum: 19.05.2022
 * Letzte Änderung: 15.01.2023
 * Germany, Sachsen-Anhalt, Magdeburg
 * */

using System;
using System.Drawing;
using ARRAY = ImageProcessing.ArrayOwn;

namespace ImageProcessing {
  internal class NeighborhoodOperationOwn {

    #region 0. Attribute und unterstützende Methoden
    /// <summary>
    /// 0.1 Instanzvariable zur Fehlermeldung
    /// </summary>
    private Exception exception;

    /// <summary>
    /// 0.2 Neighbor indices as a shift in i, j
    /// </summary>
    public int[,] shift = { { -1, 0 }, { -1, -1 }, { 0, -1 }, { 1, -1 }, { 1, 0 }, { 1, +1 }, { 0, +1 }, { -1, +1 } };

    /// <summary>
    /// 0.3 Transformiet zweidimensionales Feld in ein eindimensionales Feld
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

    #region 1. Struktrmatritzen des Faltungskerns für lineare Filter (convolution kernel)
    /// <summary>
    /// 1.1 Strukturmatrix für ein Mittelwertfilter der Größe 3 x 3 Pixel.
    /// </summary>
    /// <returns></returns> Normierte Strukturmatrix (Elementesumme = 1).
    public float[,] A3x3_Average() {
      // Nicht normierte Strukturmatrix (2D-Array).
      float[,] A = { { 1, 1, 1 },
                     { 1, 1, 1 },
                     { 1, 1, 1 } };
      // Normierung, sodass der Elementesumme = 1.
      normalization(ref A);
      return A;
    }
    /// <summary>
    /// 1.2 Strukturmatrix für ein Mittelwertfilter der Größe 5x5 Pixel
    /// </summary>
    /// <returns>"B"</returns> Normierte Strukturmatrix (Elementesumme = 1).
    public float[,] A5x5_Average() {
      float[,] A = { { 1, 1, 1, 1, 1 },
                     { 1, 1, 1, 1, 1 },
                     { 1, 1, 1, 1, 1 },
                     { 1, 1, 1, 1, 1 },
                     { 1, 1, 1, 1, 1 } };
      normalization(ref A);
      return A;
    }
    /// <summary>
    /// 1.3 Strukturmatrix für ein Binomialfilter der Größe 3x3 Pixel
    /// - Lineares Filter
    /// </summary>
    /// <returns></returns> Normierte Strukturmatrix (Elementesumme = 1).
    public float[,] B3x3_Binomial() {
      float[,] B = { { 1, 2, 1 },
                     { 2, 4, 2 },
                     { 1, 2, 1 } };
      normalization(ref B);
      return B;
    }
    /// <summary>
    /// 1.4 Strukturmatrix für ein Binomialfilter der Größe 5x5 Pixel
    /// - Lineares Filter
    /// </summary>
    /// <returns>"B"</returns>Binomialmatrix, normier auf die Summe=1
    public float[,] B5x5_Binomial() {
      float[,] B = { { 1,  4,  6,  4, 1 },
                     { 4, 16, 24, 16, 4 },
                     { 6, 24, 36, 24, 6 },
                     { 4, 16, 24, 16, 4 },
                     { 1,  4,  6,  4, 1 } };
      normalization(ref B);
      return B;
    }

    /// <summary>
    /// 1.5 Strukturmatrix für ein Gaußfilter beliebiger Größe.
    /// - Lineares Tiefpassfilter
    /// </summary>
    /// <param name="size"></param> Größe der Strukturmatrix
    /// <returns></returns> Gaußmatrix
    public float[,] G_Gauß(int size = 5) {
      // Standardabweichung sigma
      float sigma = (float)size / 6;
      // Strukturmatrix deklarieren
      float[,] G = new float[size, size];
      // Versatz mit Bezug zum Zentralpixel.
      int m = (int)(size - 1) / 2;
      // Doppelschleife zur Berechnung der Matrixwerte
      for (int y = -m; y <= +m; y++)
        for (int x = -m; x <= +m; x++)
          // Verteilung
          G[y + m, x + m] = (float)Math.Exp(-(x * x + y * y) / 2.0 / sigma / sigma);
      // Normieren, sodass Summe=1
      normalization(ref G);
      return G;
    }

    /// <summary> #
    /// 1.6 Normiert ein 2D-Array (eine Strukturmatrix),
    /// sodass die Summe aller Elemente = 1 ist.
    /// </summary>
    /// <param name="array2D"></param> Strukturmatrix
    /// --> Normiertes 2D-Array
    public static void normalization(ref float[,] array2D) {
      // Summe aller Elemente des 2D-Arrays.
      float arraySum = 0;
      for (int j = 0; j < array2D.GetLength(1); j++)
        for (int i = 0; i < array2D.GetLength(0); i++)
          arraySum += array2D[j, i];
      // Normierung der Elemente des 2D-Arrays.
      for (int j = 0; j < array2D.GetLength(1); j++)
        for (int i = 0; i < array2D.GetLength(0); i++)
          array2D[j, i] /= arraySum;
    }
    #endregion

    #region 2. Faltungskerne und Strukturelemente  zur Kantenerkennung (convolution kernel)
    /// <summary>
    /// 2.1 Strukturmatrix für ein Sobelfilter der x-Richtung.
    /// </summary>
    /// <returns>"Sx"</returns> Sobel-x-Matrix
    public float[,] Sx_Sobel_x() {
      float[,] Sx = { {-1, 0, 1},
                      {-2, 0, 2},
                      {-1, 0, 1} };
      return Sx;
    }
    /// <summary>
    /// 2.2 Strukturmatrix für ein Sobelfilter der y-Richtung.
    /// </summary>
    /// <returns>"Sy"</returns> Sobel-y-Matrix
    public float[,] Sy_Sobel_y() {
      float[,] Sy = { {-1,-2, -1},
                      { 0, 0, 0},
                      { 1, 2, 1} };
      return Sy;
    }

    /// <summary>
    /// 2.3 Normiert ein 2D-Array (eine Strukturmatrix),
    /// sodass die Summe innerhalb eines vorgegebenen Intervalls liegt.
    /// </summary>
    /// <param name="array2D"></param> Referenz der Strukturmatrize
    /// <param name="divisor"></param> Teiler: Für Sobeloperatoren ist dieser 4, sodass die
    /// Summe aller positiven Werte =1 und die Summe aller negativen Wertw =-1 beträgt.
    public static void normalization(ref float[,] array2D, float divisor = 4) {
      // Normierung der Elemente des 2D-Arrays.
      for (int j = 0; j < array2D.GetLength(1); j++)
        for (int i = 0; i < array2D.GetLength(0); i++)
          array2D[j, i] /= divisor;
    }


    /// <summary>
    /// 2.4 Strukturmatrix für ein Laplacefilter
    /// </summary>
    /// <param name="c"></param> // Zentrales Element
    /// <returns>"L"</returns> Laplacematrix
    public float[,] L8_Laplace(float c = 8) {
      float[,] L = { { -1, -1, -1 },
                     { -1,  c, -1 },
                     { -1, -1, -1 } };
      return L;
    }

    /// <summary>
    /// 2.5 Strukturmatrix für ein Laplacefilter
    /// </summary>
    /// <param name="c"></param> // Zentrales Element
    /// <returns>"L"</returns> Laplacematrix
    public float[,] L4_Laplace(float c = 4) {
      float[,] L = { {  0, -1,  0 },
                     { -1,  c, -1 },
                     {  0, -1,  0 } };
      return L;
    }

    /// <summary>
    /// 2.6 Strukturmatrix für ein Laplace-Gauß-Filter.
    /// Diese Kombination unterdrückt das Rauschen.
    /// Die implizite Gaußfilterung glättet das Bild.
    /// </summary>
    /// <param name="size"></param> Größe der Strukturmatrix.
    /// <returns></returns> Laplace-Gauß-Matrix.
    public float[,] LG_Laplacian_of_Gaussian(int size = 5) {
      float sigma = (float)size / 6;
      float[,] LG = new float[size, size];
      for (int j = 0; j < size; j++)
        for (int i = 0; i < size; i++) {
          int x = i - (int)size / 2; // Versatz im Bild mit Bezug zum Zentralpixel.
          int y = j - (int)size / 2;
          LG[j, i] = (float)Math.Exp(-(x * x + y * y) / 2 / sigma / sigma);
          LG[j, i] *= (1 - (x * x + y * y) / 2 / sigma / sigma);
          LG[j, i] /= (float)Math.PI / sigma / sigma / sigma / sigma;
        }
      return LG;
    }
    #endregion

    #region 3. Strukturmatrix zur Morphologie

    /// <summary>
    /// 3.1 Strukturmatrix zur Erosion und zur Dilatation generieren.
    /// Das Zentralelement steht für die Erosion und die Nachbarelemente
    /// für die Dilatation.
    /// </summary>
    /// <param name="size"></param> Größe der Matrix, 3 -> 3 x 3 Elemente.
    /// <param name="Idilataton"></param> Nachbarn, Intensität zur Dilatation {0, 1, ...}.
    /// <param name="Ierosion"></param> Zentralelement, Intensität zur Erosion {0, 1, ...}.
    /// <returns>"M"</returns> Strukturmatrix zur morphologischen Operation.
    public float[,] M_Morph(int size = 3, float Idilatation = 0, float Ierosion = 1) {
      int m = (size - 1) / 2; // Medianindex
      // Strukturmatrix deklarieren
      float[,] M = new float[size, size];
      // Doppelschleife zum Setzen der Matrixwerte.
      for (int j = 0; j < size; j++)
        for (int i = 0; i < size; i++) {
          // Zuweisung
          M[j, i] = Idilatation;
        }
      M[m, m] = Ierosion;
      return M;
    }
    /// <summary>
    /// 3.2 Erosin 
    /// Hintergrund: 0
    /// Segmentfläche(n): I
    /// </summary>
    /// <param name="c"></param> Id der zu erodierenden Segmentfläche
    /// <returns></returns> Strukturmatrix 3x3
    public float[,] ME_Erosion3(float I = 1) {
      float[,] M = { { 0, 0, 0 },
                     { 0, I, 0 },
                     { 0, 0, 0 }};
      return M;
    }

    /// <summary>
    /// 3.3 Dilatation
    /// Hintergrund: 0
    /// Segmentfläche(n): I
    /// </summary>
    /// <param name="c"></param> Id der zu dilatierenden Segmentfläche
    /// <returns></returns> Strukturmatrix 3x3
    public float[,] MD_Dilatation3(float I = 1) {
      float[,] M = { { I, I, I },
                     { I, 0, I },
                     { I, I, I }};
      return M;
    }

    /// <summary>
    /// 3.4 Erosion
    /// Hintergrund: 0
    /// Segmentfläche(n): I
    /// </summary>
    /// <param name="c"></param> Id der zu erodierenden Segmentfläche
    /// <returns></returns> Strukturmatrix 5x5
    public float[,] ME_Erosion5(float I = 1) {
      float[,] M = { { 0, 0, 0, 0, 0 },
                     { 0, 0, 0, 0, 0 },
                     { 0, 0, I, 0, 0 },
                     { 0, 0, 0, 0, 0 },
                     { 0, 0, 0, 0, 0 } };
      return M;
    }


    /// <summary>
    /// 3.4 Erosion
    /// Hintergrund: 0, 1
    /// Segmentfläche(n): I
    /// </summary>
    /// <param name="c"></param> Id der zu erodierenden Segmentfläche
    /// <returns></returns> Strukturmatrix 5x5
    public float[,] ME_Erosion5b(float I = 1) {
      float[,] M = { { I, I, I, I, I },
                     { I, 0, 0, 0, I },
                     { I, 0, I, 0, I },
                     { I, 0, 0, 0, I },
                     { I, I, I, I, I } };
      return M;
    }


    #endregion

    #region 4. Lineare Filter 

    /// <summary>
    /// 4.1 Tiefpass-Mittelwertfilter mit einem Faltungskern von 3x3 Pixel
    /// </summary>
    /// <param name="array"></param> Bildarray
    /// <returns></returns> Geglättetes Bildarray
    public float[,,] toAverage(float[,,] array) {
      float[,,] averageArray = new float[array.GetLength(0), array.GetLength(1) - 2, array.GetLength(2) - 2];
      for (int c = 0; c < array.GetLength(0); c++) // Für alle Kanäle
        for (int y = 1; y < array.GetLength(1) - 1; y++) // Für alle Bildpunkte und
          for (int x = 1; x < array.GetLength(2) - 1; x++) { // -> Rand beachten
            float I = 0;  // Intensität eines Elementes
            for (int j = 0; j < 3; j++)
              for (int i = 0; i < 3; i++) {
                // Gleichverteilte Gewichtung von 1/9 für jeden Bildpunkt.
                I += array[c, y + j - 1, x + i - 1] / 9;
              }
            averageArray[c, y - 1, x - 1] = I;
          }
      return averageArray;
    }

    /// <summary> 
    /// 4.2 Lineares Filter, geeignet als Tiefpassfilter und Hochpassfilter.
    /// Der Faltungskern beeinflusst die Filterwirkung.
    /// </summary>
    /// <param name="array"></param> 3D-Array, Bildarray
    /// <param name="strukturmatrix"></param> Filtermatrix, Strukturmatrix
    /// <param name="canals"></param> Feld mit Kanalindizes {0, 1, 2}.
    /// <param name="marginValue"></param> Werte des hinzugefügten Randes
    ///                                    bzw. Wert des Hintergrundes
    /// <param name="step"></param> Schrittweite
    /// <returns></returns> Gefiltertes Bildarray
    public float[,,] linearFilter(float[,,] array, float[,] strukturmatrix, int[] canals, double marginValue, int step = 1) {
      // Ergebnisarray deklarieren
      float[,,] filterArray = new float[array.GetLength(0), array.GetLength(1) / step, array.GetLength(2) / step];
      // Der hinzuzufügende Rand ergibt sich aus der Größe des Faltungskerns. 
      int m = (int)(strukturmatrix.GetLength(0) - 1) / 2;
      // Array um einen Rand ausdehnen.
      array = ARRAY.padding(array, m, marginValue);
      // Trippelschleife für Kanal/Kanäle in y- und x-Richtung.
      for (int c = 0; c < canals.GetLength(0); c++)
        // Für alle Bildpunkte und Rand beachten
        for (int y = m; y < array.GetLength(1) - m; y += step)
          for (int x = m; x < array.GetLength(2) - m; x += step) {
            // Intensität eines Elementes
            float I = 0;
            // Doppelschleife des Kernels.
            for (int j = -m; j <= m; j++)
              for (int i = -m; i <= m; i++) {
                // Verarbeitung der Filtermatrix
                I += array[canals[c], y + j, x + i] * strukturmatrix[j + m, i + m];
              }
            // Filterwert setzen
            filterArray[c, (int)((y - m) / step), (int)((x - m) / step)] = I;
          }
      return filterArray;
    }

    /// <summary> ÜBERLADUNG
    /// 4.2a Lineares Filter, geeignet als Tiefpassfilter und Hochpassfilter.
    /// Der Faltungskern beeinflusst die Filterwirkung.
    /// Nur für Faltungskerne der Größe 3x3 geeignet.
    /// </summary>
    /// <param name="array"></param> 3D-Array, Bildarray
    /// <param name="strukturmatrix"></param> Filtermatrix, Strukturmatrix
    /// <param name="canals"></param> Feld mit Kanalindizes {0, 1, 2}.
    /// <param name="step"></param> Schrittweite
    /// <returns></returns> Gefiltertes Bildarray
    public float[,,] linearFilter(float[,,] array, float[,] strukturmatrix, int[] canals, int step = 1) {
      // Ergebnisarray deklarieren
      float[,,] filterArray = new float[array.GetLength(0), array.GetLength(1) / step, array.GetLength(2) / step];
      // int m = 1; // Rand = 1
      // Array um den Rand mit der Breite 1 ausdehnen.
      array = ARRAY.padding(array);
      // Trippelschleife für Kanal/Kanäle in y- und x-Richtung.
      for (int c = 0; c < canals.GetLength(0); c++)
        // Für alle Bildpunkte und Rand beachten
        for (int y = 1; y < array.GetLength(1) - 1; y += step)
          for (int x = 1; x < array.GetLength(2) - 1; x += step) {
            // Intensität eines Elementes
            float I = 0;
            // Doppelschleife des Kernels.
            for (int j = -1; j <= 1; j++)
              for (int i = -1; i <= 1; i++) {
                // Verarbeitung der Filtermatrix
                I += array[canals[c], y + j, x + i] * strukturmatrix[j + 1, i + 1];
              }
            // Filterwert setzen
            filterArray[c, (int)((y - 1) / step), (int)((x - 1) / step)] = I;
          }
      return filterArray;
    }

    // ----------------- HOCHPASS --------------------------------------------
    /// <summary>
    /// 4.3 Sobelfilter zur Kantendedektion für einen ausgewählten Kanal - Hochpassfilter
    /// </summary>
    /// <param name="array"></param> 3D-Arrays, Bildarray zur Ein- und Rückgabe.
    /// Zurückgegeben wird das Gradientenarray
    /// <param name="sobel_x"></param> Sobel-x-Matrix
    /// <param name="sobel_y"></param> Sobel-y-Matrix
    /// <param name="canals"></param> Feld mit Kanalindizes {0, 1, 2}.
    public void sobelFilter(ref float[,,] array, float[,] sobel_x, float[,] sobel_y, int[] canals) {
      // Berechnung der Sobelarrays für die x- und die y-Richtung.
      float[,,] array_x = linearFilter(array, sobel_x, canals);
      float[,,] array_y = linearFilter(array, sobel_y, canals);
      // Liefert das Gradientenarray.
      for (int c = 0; c < canals.GetLength(0); c++) {
        // Resultierende
        sobelGradienten(ref array, array_x, array_y, canals[c]);
        // Normierung der Werte im Zielintervall [0, 1]
        ARRAY.normalizationArray3D(ref array, ARRAY.getIntervall(0, 1), canals[c]);
      }
    }

    /// <summary>
    /// 4.5 Berechnet die Gradienten anders gesagt die Resultierenden zweier Arrays gleicher Größe.
    /// </summary>
    /// <param name="array"></param> Referenz-Array mit den Resultierenden
    /// <param name="array_x"></param> Sobelarray für die x-Richtung
    /// <param name="array_y"></param> Sobelarray für die y-Richtung
    public void sobelGradienten(ref float[,,] arrayGradent, float[,,] array_x, float[,,] array_y, int c = 0) {
      // Doppelschleife für Kanal y- und in x-Richtung
      for (int y = 0; y < array_x.GetLength(1); y++) // Für alle Bildpunkte
        for (int x = 0; x < array_x.GetLength(2); x++) {
          // Elementeweise Berechnung.
          arrayGradent[c, y, x] = (float)Math.Sqrt(array_x[c, y, x] * array_x[c, y, x] + array_y[c, y, x] * array_y[c, y, x]);
        }
    }

    #endregion

    #region 5. Rangordnungsfilter sind nichtlineare Filter ohne Strukturmatrix.
    /// <summary> 
    /// 5.1 Medianfilter - Nichtlineares Filter
    /// </summary>
    /// <param name="array"></param> 3D-Array
    /// <param name="size"></param> Größe des Faltungskerns
    /// <param name="marginValue"></param> Werte des hinzugefügten Randes
    /// <param name="step"></param> Schritweiter
    /// <returns></returns> Gefiltertes Bild
    public float[,,] medianFilter(float[,,] array, int size = 3, double marginValue = 1, int step = 1) {
      // Ergenisarray deklarieren
      float[,,] rankingArray = new float[array.GetLength(0), (array.GetLength(1)) / step, (array.GetLength(2)) / step];
      int margin = (int)(size - 1) / 2; // Der hinzuzufügende Rand ergibt sich aus der Größe des Faltungskerns. 
      array = ARRAY.padding(array, margin, marginValue);
      for (int c = 0; c < array.GetLength(0); c++) {  // Für jeden Kanal Kanäle
        for (int y = margin; y < array.GetLength(1) - margin; y += step) // Für alle Bildpunkte und
          for (int x = margin; x < array.GetLength(2) - margin; x += step) { // -> Rand beachten
            // Kernel
            float[] I = new float[size * size];  // Intensität eines Elementes
            for (int j = 0; j < size; j++)
              for (int i = 0; i < size; i++) {
                I[i * j] = array[c, y + j - margin, x + i - margin];
              }
            Array.Sort(I);
            rankingArray[c, (y - margin) / step, (x - margin) / step] = I[(size + 1) / 2];
          }
      }
      return rankingArray;
    }

    /// <summary>
    /// 5.2 Deklaration eines Delegaten
    /// </summary>
    /// <param name="Intensities"></param> Feld mit Intensitätswerten
    /// <returns></returns> ein Wert
    public delegate float DelegateRank(float[] intensities);

    /// <summary>
    ///  5.3 Liefert den Wert in der Mitte eines Feldes.
    /// </summary>
    /// <param name="intensities"></param> Intensitätswerte eines Kernels.
    /// <returns></returns> Mittenwert
    public float median(float[] intensities) {
      // Mittlerer Wert der sortierten Reihenfolge.
      return intensities[(intensities.GetLength(0) - 1) / 2];
    }
    /// <summary>
    /// 5.4 Liefert den größten Wert eines Feldes.
    /// </summary>
    /// <param name="intensities"></param> Intensitätswerte eines Kernels.
    /// <returns></returns> Maximum
    public float maximum(float[] intensities) {
      // Letzter Wert der sortierten Reihenfolge.
      return intensities[intensities.GetLength(0) - 1];
    }
    /// <summary>
    /// 5.5 Liefert den kleinsten Wert eines Feldes.
    /// </summary>
    /// <param name="intensities"></param> Intensitätswerte eines Kernels.
    /// <returns></returns> Minimum
    public float minimum(float[] intensities) {
      // Erster Wert der sortierten Reihenfolge.
      return intensities[0];
    }

    /// <summary> 
    /// 5.6 Allgemeingültiges Rangordnungsfilter - Nichtlineares Filter
    /// </summary>
    /// <param name="array"></param> 3D-Array
    /// <param name="rankElement"></param> Deligierte Methode zur Rankordnungsauswahl.
    /// <param name="canals"></param> Feld mit Kanalindizes {0, 1, 2}.
    /// <param name="size"></param> Größe des Faltungskerns
    /// <param name="marginValue"></param> Werte des hinzugefügten Randes
    /// <param name="step"></param> Schrittweite
    /// <returns></returns> Gefiltertes Bild
    public float[,,] rankFilter(float[,,] array, DelegateRank rankElement, int[] canals, int size = 3, double marginValue = 1, int step = 1) {
      exception = null; // Fehlervariable
      // Ergebnisarray deklarieren
      float[,,] rankingArray = new float[array.GetLength(0), (array.GetLength(1)) / step, (array.GetLength(2)) / step];
      // Der hinzuzufügende Rand ergibt sich aus der Größe des Kernels.
      int m = (int)(size - 1) / 2; // Rand "mergin"
      array = ARRAY.padding(array, m, marginValue);
      // Trippelschleife für die parametrierten Kanäle/Kanal in y- und in x-Richtung
      for (int c = 0; c < canals.GetLength(0); c++)
        for (int y = m; y < array.GetLength(1) - m; y += step) // Für alle Bildpunkte und
          for (int x = m; x < array.GetLength(2) - m; x += step) { // -> Rand beachten
            try {
              // Kernel Intensität eines Elementes
              float[] I = new float[size * size];
              // Faltungskern-Doppelschleife
              for (int j = 0; j < size; j++)
                for (int i = 0; i < size; i++) {
                  I[j * size + i] = array[canals[c], y + j - m, x + i - m];
                }
              // Sortieren
              Array.Sort(I);
              // Zuweisung des Filterwertes
              rankingArray[c, (y - m) / step, (x - m) / step] = rankElement(I);
            }
            catch (Exception ex) {
              // Bei einigen Parameterkombinationen kann ein Indexfehler auftreten. Dieser soll angezeigt werden.
              // Ein Programmannruch ist nicht vorgesehen.
              exception = ex; // An Instanzvariable
            };
          }

      return rankingArray;
    }

    /// <summary>
    ///  5.7 Getter zur Übermittlung der Fehlermeldung
    /// </summary>
    public Exception getException {
      get { return exception; }
    }
    #endregion

    #region 6. Morphologische Operationen
    /// <summary>
    /// 6.1 Morphologische Basisoperation zur Erosion und zur Dilatation
    ///     der Kanäle eines 3D-Arrays.
    /// </summary>
    /// <param name="array"></param> Eingangsarray
    /// <param name="canals"></param> Feld mit Kanalindizes {0, 1, 2}.
    /// <param name="Morph"></param> Strukturmatrix der morphologischen Operation
    /// <returns>"morphArray"</returns> Array mit morphologisch veränderten Elementen.
    public float[,,] morphologie(float[,,] array, int[] canals, float[,] Morph) {
      // Erstellt eine Kopie des Arrays, das morphologisch verändert wird.
      float[,,] morphArray = ARRAY.copy(array);
      // Grenzen in Abhängigkeit von der Größe der Strukturmatrix.
      int m = (Morph.GetLength(0) - 1) / 2;
      // Trippelschleife für die paramtrierten Kanäle/Kanal in y- und in x-Richtung.
      for (int c = 0; c < canals.GetLength(0); c++) {
        for (int y = m; y < array.GetLength(1) - m; y++) // Für alle Bildpunkte
          for (int x = m; x < array.GetLength(2) - m; x++) {    // -> Rand beachten
            // Prüft den Intensitätswert bzw. Id des fokussierten Elementes.
            if (array[canals[c], y, x] == Morph[m, m]) { // Fokussiertes Element, Zentralelement?
              // Kerneldoppelschleife zur Bearbeitung der Kernelelemente.
              for (int j = -m; j <= m; j++) {      // Nachbarn 
                for (int i = -m; i <= m; i++) {
                  // Gleicht ein Nachbar des Kernels einem Element der Matrix,
                  // ausgenommen dem Zentralpixel?
                  if (array[canals[c], y + j, x + i] == Morph[j + m, i + m] && !(j == 0 && i == 0)) {
                    // Fokussiertes Element des Arrays dilatieren.
                    morphArray[canals[c], y, x] = Morph[j + m, i + m]; //- ok = true;
                    break; // . . .
                  }
                }
                // Nach Zuweisung des fokussierten Elementes die Kernelschleifen sofort beenden.
                continue;//if (ok) // break;
              }
            }
          }
      }
      return morphArray;
    }

    /// <summary> VARIANTE MIT SHIFT-ARRAY
    /// 6.1_v2 Morphologische Basisoperation zur Erosion und zur Dilatation
    ///     der Kanäle eines 3D-Arrays.
    /// </summary>
    /// <param name="array"></param> Eingangsarray
    /// <param name="canals"></param> Feld mit Kanalindizes {0, 1, 2}.
    /// <param name="Morph"></param> Strukturmatrix der morphologischen Operation
    /// <returns></returns> Array mit morphologisch veränderten Flächensegmenten.
    public void morphologie2(ref float[,,] array, int[] canals, float[,] Morph) {
      // Versatz als Punkte - Umwandlung soll die Übersichtlichkeit verbessern.
      Point[] neighbor = toPoints(shift);
      // Erstellt eine Kopie des Arrays, die morphologisch verändert wird.
      float[,,] morphArray = ARRAY.copy(array);
      // Grenzen in Abhängigkeit von der Größe der Strukturmatrix.
      int m = (Morph.GetLength(0) - 1) / 2;
      // Trippelschleife für die paramtrierten Kanäle/Kanal in y- und in x-Richtung.
      for (int c = 0; c < canals.GetLength(0); c++)
        for (int y = m; y < array.GetLength(1) - m; y++) // Für alle Bildpunkte und
          for (int x = m; x < array.GetLength(2) - m; x++) {    // -> Rand beachten
            // Prüft den Intensitätswert bzw. Id des fokussierten Elementes.
            if (array[canals[c], y, x] == Morph[m, m]) { // Fokussiertes Element, Zentralelement?
              // Dann kontrolliere die Nachbarpunkte mit einer Schleife.
              for (int s = 0; s < neighbor.GetLength(0); s++)
                // Gleicht ein Nachbar des Kernels einem Element der Matrix?
                if (array[canals[c], y + neighbor[s].Y, x + neighbor[s].X] == Morph[neighbor[s].Y + m, neighbor[s].X + m]) { // und Intensität des Horizontpixel == Intensität des Zentralpixel
                  // Fokussiertes Element des Arrays anders gesagt das Zentralpixel des Kernels neu setzen.
                  morphArray[canals[c], y, x] = Morph[neighbor[s].Y + m, neighbor[s].X + m];
                  break; // . . .
                }
            }
          }
      array = morphArray;
    }

    /// <summary> ÜBERLADUNG
    ///  6.2 Morphologische Operation für Kanäle mit vereinfachter
    ///      Parametrierung der Dilatation. Alle Elemente der Strukturmatrix
    ///      besitzen gleichen Wert.
    /// </summary>
    /// <param name="array"></param> Eingangsarray
    /// <param name="canals"></param> Feld mit Kanalindizes {0, 1, 2}.
    /// <param name="Idilatation"></param> Intensität, die dilatiert werden soll.
    /// <param name="Ierosion"></param> Intensität, die erodiert werden soll.
    /// <param name="size"></param> Größe der Strukturmatrix.
    /// <returns></returns> Array mit morphologisch veränderten Elementen.
    public void morphologie(ref float[,,] array, int[] canals, float Idilatation = 0, float Ierosion = 1, int size = 3) {
      float[,] Morph = M_Morph(size, Idilatation, Ierosion);
      array = morphologie(array, canals, Morph);
    }

    /// <summary>
    /// 6.3 Dilatation mehrerer Objekte
    ///     mit mehreren Durchläufen.
    /// </summary>
    /// <param name="array"></param> Eingangsarray
    /// <param name="canals"></param> Feld mit Kanalindizes {0, 1, 2}.
    /// <param name="segmentCount"></param> Anzahl n der zu dilatierenden Segmentflächen.
    /// <param name="Ierosion"></param>ID oder Intensitätswert des zu erodierenden Hintergrunds.
    /// <param name="size"></param> Größe der Strukturmatrix
    /// <param name="zyklen"></param> Anzahl der Zyklen
    public void dilatation(ref float[,,] array, int[] canals, int segmentCount, float Ierosion = 0, int size = 3, int zyklen = 1) {
      // Mehrfachaufruf
      for (var z = 0; z < zyklen; z++)
        // Segmentzyklus
        for (int id = 1; id <= segmentCount; id++) {
          // Strukturmatrix generieren
          float[,] Morph = M_Morph(size, id, Ierosion);
          // Id-spezifische Vergrößerung der Objekte.
          array = morphologie(array, canals, Morph);
        }
    }

    /// <summary>
    /// 6.4 Erosion mehrerer Objekte
    ///     mit mehreren Durchläufen.
    /// </summary>
    /// <param name="array"></param> Eingangsarray
    /// <param name="canals"></param> Feld mit Kanalindizes {0, 1, 2}.
    /// <param name="segmentCount"></param> Anzahl n der zu erodierenden Segmentflächen.
    /// <param name="Idilatation"></param>ID oder Intensitätswert des zu dilatierenden Hintergrunds.
    /// <param name="size"></param> Größe der Strukturmatrix
    /// <param name="zyklen"></param> Anzahl der Zyklen
    public void erosion(ref float[,,] array, int[] canals, int segmentCount, float Idilatation = 0, int size = 3, int zyklen = 1) {
      // Mehrfachaufruf
      for (var z = 0; z < zyklen; z++)
        // Objektzyklus
        for (int id = 1; id <= segmentCount; id++) {
          // Strukturmatrix generieren
          float[,] Morph = M_Morph(size, Idilatation, id);
          // Id-spezifische Verkleinerung der Objekte.
          array = morphologie(array, canals, Morph);
        }
    }

    /// <summary>
    /// 6.5 Entfernt Artefakte
    /// </summary>
    /// <param name="array"></param> 3D-Array
    /// <param name="canals"></param> Kanäle 
    /// <param name="size"></param> Größe des Strukturmatrix
    /// <returns></returns> 3D-Matrix
    public float[,,] opening(float[,,] array, int[] canals, int size = 3) {
      // Erosion
      array = morphologie(array, canals, M_Morph(size, 0, 1));
      // Dilatation
      return morphologie(array, canals, M_Morph(size, 1, 0));
    }

    /// <summary>
    /// 6.6 Schließt kleinere Öffnungen in eine Segmentfläche
    /// und glättet den Konturverlauf.
    /// </summary>
    /// <param name="array"></param> 3D-Array
    /// <param name="canals"></param> Kanäle 
    /// <param name="size"></param> Größe des Strukturmatrix
    /// <returns></returns> 3D-Matrix
    public float[,,] closing(float[,,] array, int[] canals, int size = 3) {
      // Dilatation
      array = morphologie(array, canals, M_Morph(size, 1, 0));
      // Erosion
      return morphologie(array, canals, M_Morph(size, 0, 1));
    }


    #endregion

    #region 7. Projekt: Mehrere Gegenstände

    /// <summary>
    /// 7.1 Prüft die zugehörigkeit der Bildpunkte zu einem Objekt
    /// durch einen Vergleich mit dem Original.
    /// </summary>
    /// <param name="array"></param>
    /// <param name="array3Doriginal"></param>
    public void isObject(ref float[,,] array, float[,,] arrayOriginal) {
      for (int y = 0; y < arrayOriginal.GetLength(1); y++)
        for (int x = 0; x < arrayOriginal.GetLength(2); x++)
          if (arrayOriginal[0, y, x] == 0) {
            array[0, y, x] = 0;
            array[1, y, x] = 0;
            array[2, y, x] = 0;
          }
    }

    /// <summary>
    /// 7.2 Prüft die zugehörigkeit der Bildpunkte zu einem Objekt
    /// durch einen Vergleich mit einem Kanal des Originals.
    /// </summary>
    /// <param name="array"></param> 3D-Array
    /// <param name="arrayKanal"></param> 2D-Array, Kanal eines 3D-Arrays
    public void isObject(ref float[,,] array, float[,] arrayKanal) {
      for (int y = 0; y < arrayKanal.GetLength(0); y++)
        for (int x = 0; x < arrayKanal.GetLength(1); x++)
          if (arrayKanal[y, x] == 0) {
            array[0, y, x] = 0;
            array[1, y, x] = 0;
            array[2, y, x] = 0;
          }
    }
    #endregion

  }
}
