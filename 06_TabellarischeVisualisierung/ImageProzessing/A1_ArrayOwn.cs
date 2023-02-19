using System;

namespace ImageProcessing {
  class ArrayOwn {
    #region 1. Attribute, Konstruktor, Datenstrukturen

    // 1.1 Atribut, Eigenschaft
    public float[,,] element;

    /// <summary>
    /// 1.2 Dient der Arrayübernahme mit Bezug zur Operatorüberladung.
    /// </summary>
    /// <param name="array"></param> 3D-Array mit [Kanal, Höhe, Weite]
    public ArrayOwn(float[,,] array) {
      this.element = array;
    }

    /// <summary>
    /// 1.3 Element des Arrays mit Bezug zur Operatorüberladung.
    /// </summary>
    /// <param name="matrix"></param>
    public ArrayOwn(int c, int y, int x) {
      this.element = new float[c, y, x];
    }


    /// <summary>
    /// 1.4 Typ-Deklaration der Extremwerte
    /// </summary>
    public struct Extrem {
      public float min;
      public float max;
    }
    // 1.5 Typ-Deklaration des Intervalls
    public struct Intervall {
      public float unten;
      public float oben;
    }


    #endregion

    #region 2. 2D-Arrayoperationen, Kerneloperationen
    /// <summary>
    /// Liefert die Summe aller Werte eines Kernels
    /// </summary>
    /// <param name="kernel"></param>
    /// <returns></returns>
    public static float array2DSum(float[,] array2D) {
      float arraySum = 0;
      for (int j = 0; j < array2D.GetLength(1); j++)
        for (int i = 0; i < array2D.GetLength(0); i++)
          arraySum += array2D[j, i];
      return arraySum;
    }


    /// <summary>
    /// Normiert den Kernel, sodass Summe aller Elemente =1 ist
    /// </summary>
    /// <param name="kernel"></param>
    /// <returns></returns>
    public static void normierung(ref float[,] array2D) {
      float arraySum = array2DSum(array2D); // 
      for (int j = 0; j < array2D.GetLength(1); j++) // Summe aller Element =1
        for (int i = 0; i < array2D.GetLength(0); i++)
          array2D[j, i] /= arraySum;
    }
    #endregion

    #region 3. Array modifizieren und kopieren
    /// <summary>
    /// Kopiert ein dreidimensionales Array
    /// </summary>
    /// <param name="array"></param> Source array
    /// <returns></returns> Destination Array
    public static float[,,] copy(float[,,] array) {
      float[,,] destinationArray = new float[3, array.GetLength(1), array.GetLength(2)];
      for (int c = 0; c < 3; c++)
        for (int y = 0; y < array.GetLength(1); y++)
          for (int x = 0; x < array.GetLength(2); x++)
            destinationArray[c, y, x] = (float)array[c, y, x];
      return destinationArray;
    }

    /// <summary>
    /// 0.1 Array um einen Rand erweitern (Aufplustern).
    /// - Alle Kanäle drei Kanäle
    /// </summary>
    /// <param name="array"></param> Bildarray
    /// <param name="borderValue"></param> Wert mit dem der Rand aufgefüllt wird.
    /// <param name="margin"></param> Randbreite in Pixel
    /// <returns></returns> 3D-Array mit einem zusätzlichen Rand
    public static float[,,] padding(float[,,] array, double borderValue = 0, int margin = 1) {
      float[,,] paddingArray = new float[3, array.GetLength(1) + 2 * margin, array.GetLength(2) + 2 * margin];
      // Padding (um Rand größeres Array)
      for (int c = 0; c < 3; c++)
        for (int y = 0; y < paddingArray.GetLength(1); y++)
          for (int x = 0; x < paddingArray.GetLength(2); x++)
            paddingArray[c, y, x] = (float)borderValue;
      // Übertragen der Werte der Basismatrix
      for (int c = 0; c < 3; c++)
        for (int y = 0; y < array.GetLength(1); y++)
          for (int x = 0; x < array.GetLength(2); x++)
            paddingArray[c, y + margin, x + margin] = array[c, y, x];
      return paddingArray;
    }

    #endregion

    #region 4. 3D-Arrayoperationen 
    /// <summary>
    /// 4.1 Elementeweise Multiplikation zweier Arrays gleicher Größe.
    /// </summary>
    /// <param name="array1"></param> Array 1
    /// <param name="array2"></param> Array 2
    /// <returns></returns> Multiplikationsergebnis
    public static float[,,] multiply(float[,,] array1, float[,,] array2) {
      for (int c = 0; c < array1.GetLength(0); c++) // Für alle Kanäle
        for (int y = 1; y < array1.GetLength(1); y++) // Für alle Bildpunkte und
          for (int x = 1; x < array1.GetLength(2); x++)  // -> Rand beachten
            array1[c, y, x] *= array2[c, y, x];
      return array1;
    }


    /// <summary>
    /// 4.2 Operatorüberladung zur elementeweise Multiplikation zweier Arrays gleicher Größe.
    /// </summary>
    /// <param name="array1"></param> Array 1
    /// <param name="array2"></param> Array 2
    /// <returns></returns> Multiplikationsergebnis
    public static ArrayOwn operator *(ArrayOwn array1, ArrayOwn array2) {
      for (int c = 0; c < array1.element.GetLength(0); c++) // Für alle Kanäle
        for (int y = 1; y < array1.element.GetLength(1); y++) // Für alle Bildpunkte und
          for (int x = 1; x < array1.element.GetLength(2); x++) {   // -> Rand beachten
            array1.element[c, y, x] *= array2.element[c, y, x];
          }
      return array1;
    }

    #endregion

    #region 5. Normalisieren und Intervall

    /// <summary>
    /// </summary>  RUFT PARTNERMETHODE ALS ÜBERLADUNG
    /// 5.1 Normalisert Elemente des Arrays für jeden Kanal
    /// unabhängig voneinander.
    /// Die Neuen Intervallgrenzen können vorgegeb werden. Zum Beispiel 0, 1
    /// <param name="array"></param> 3D-Array
    /// <param name="unten"></param> neue untere Intervallgrenze [0, 255]
    /// <param name="oben"></param> neue ober Intervallgrenze [0, 255]
    /// <returns></returns> Aray in den neuen Intervallgrenzen.
    public static void normalizationArray3Dcomplete(ref float[,,] array, float unten = 0, float oben = 1) {
      for (int c = 0; c < array.GetLength(0); c++)
        normalizationArray3D(ref array, c, unten, oben);
    }
    /// <summary> ÜBERLADUNG
    /// 5.2 Normalisiere anders gesagt lineare Intensitätsspreizung zwischen dem
    /// minimalen und dem maximalen Intensitätswert im 
    /// Intervall von 0 bis .... Normalisiert Float-Elemente des Kanals
    /// eines 3D-Arrays in einen darstellbaren Farbbereich.
    /// Umrechnung in den Intervall [0,1].
    /// </summary>
    /// <param name="array"></param> 3D-Array
    /// <param name="canal"></param> Kanal, Layer, Bildebene
    public static void normalizationArray3D(ref float[,,] array, int canal = 1, float unten = 0, float oben = 1) {
      Extrem extrem = new Extrem();
      extrem.max = 0; //  Maximum des Kanal 1
      extrem.min = 255;
      Intervall intervall = new Intervall();
      intervall.oben = oben;
      intervall.unten = unten;
      for (var y = 0; y < array.GetLength(1); y++)
        for (var x = 0; x < array.GetLength(2); x++) {
          if (extrem.max < array[canal, y, x])
            extrem.max = array[canal, y, x];
          if (extrem.min > array[canal, y, x])
            extrem.min = array[canal, y, x];
        }
      for (var y = 0; y < array.GetLength(1); y++)
        for (var x = 0; x < array.GetLength(2); x++)
          toIntervall(ref array[canal, y, x], ref extrem, ref intervall);
    }

    /// <summary>
    /// 5.3 Normalisiert ein einzelnes Element
    /// </summary>
    /// <param name="x"></param> Element x als Wert
    /// <param name="extrem"></param> Extremwerte für alle Kanäle
    /// <param name="intervall"></param> Zielintervall für alle Kanäle
    /// <param name="canal"></param> Kanal zur Bearbeitung
    /// <returns></returns> Wert im neuen Intervall
    public static float toIntervall(float x, Extrem[] extrem, Intervall[] intervall, int canal = 0) {
      return (float)((x - extrem[canal].min) * (intervall[canal].oben - intervall[canal].unten) / (extrem[canal].max - extrem[canal].min) + intervall[canal].unten);
    }
    /// <summary> ÜBERLADUNG
    /// 5.4 Normalisiert ein einzelnes Element
    /// </summary>
    /// <param name="x"></param> Element x als Wert
    /// <param name="extrem"></param> Extremwerte für einen Kanal c
    /// <param name="intervall"></param> Zielintervall für einen Kanal
    /// <returns></returns> Wert im neuen Intervall
    public static float toIntervall(float x, Extrem extrem, Intervall intervall) {
      return (float)((x - extrem.min) * (intervall.oben - intervall.unten) / (extrem.max - extrem.min) + intervall.unten);
    }
    /// <summary> ÜBERLADUNG
    /// 5.5 Normalisiert ein einzelnes Element
    /// </summary>
    /// <param name="x"></param> Element x als Wert
    /// <param name="extrem"></param> Extremwerte für alle Kanäle
    /// <param name="intervall"></param> Zielintervall für einen Kanal
    /// <returns></returns> Werte im neuen Intervall
    public static float[] toIntervall(float[] x, Extrem[] extrem, Intervall[] intervall) {
      float[] I = new float[3];
      for (int c = 0; c < 3; c++) {
        I[c] = toIntervall(x[c], extrem[c], intervall[c]);
      }
      return I;
    }
    /// <summary> ÜBERLADUNG ALS REFERENZ
    /// 5.6 Normalisiert ein einzelnes Element
    /// </summary>
    /// <param name="x"></param> Element x mit Referenz
    /// <param name="extrem"></param> Extremwerte für alle Kanäle
    /// <param name="intervall"></param> Zielintervall für alle Kanäle
    /// <param name="canal"></param> Kanal zur Bearbeitung
    public static void toIntervall(ref float x, Extrem[] extrem, Intervall[] intervall, int canal = 0) {
      //x= (float)((x - extrem.min[canal]) / (extrem.max[canal] - extrem.min[canal]) * 255);
      x = (float)((x - extrem[canal].min) * (intervall[canal].oben - intervall[canal].unten) / (extrem[canal].max - extrem[canal].min) + intervall[canal].unten);
    }
    /// <summary> ÜBERLADUNG
    ///  5.7 Normalisiert ein einzelnes Element
    /// </summary>
    /// <param name="x"></param> Element x mit Referenz
    /// <param name="extrem"></param> Extremwerte eines Kanals
    /// <param name="intervall"></param> Zielintervall eines kanals
    public static void toIntervall(ref float x, ref Extrem extrem, ref Intervall intervall) {
      x = (float)((x - extrem.min) * (intervall.oben - intervall.unten) / (extrem.max - extrem.min) + intervall.unten);
    }

    /// <summary>
    /// 5.8 Liefert die kanalbezogenen Extremwerte.
    /// </summary>
    /// <param name="array"></param> 3D-Array mit maximal drei Kanälen.
    /// <returns></returns> Datenstruktur mit Extremwerten
    public static Extrem[] canalExtrem(float[,,] array) {
      Extrem[] extrem = new Extrem[3];
      // Anfangswerte
      for (var c = 0; c < 3; c++) {
        extrem[c].min = array[0, 0, 0];
        extrem[c].max = array[0, 0, 0];
      }
      // Trippelschleife
      for (int c = 0; c < array.GetLength(0); c++)
        for (int y = 0; y < array.GetLength(1); y++)
          for (int x = 0; x < array.GetLength(2); x++) {
            // Kumulativer Vergleich
            if (array[c, y, x] > extrem[c].max)
              extrem[c].max = array[c, y, x];
            if (array[c, y, x] < extrem[c].min)
              extrem[c].min = array[c, y, x];
          }
      return extrem;
    }

    /// <summary>
    /// 5.9 Liefert ein Feld mit den drei oberen und den drei unteren Intervallgrenzen.
    /// Alle Kanäle erhalten identische Werte. Dient der vereinfachten Werteübergabe.
    /// </summary>
    /// <param name="unten"></param> untere Grenze für alle Kanäle.
    /// <param name="oben"></param> obere Grenze für alle Kanäle
    /// <returns></returns> Intervallfeld 
    public static Intervall[] getIntervall(float unten = 1, float oben = 0) {
      Intervall[] intervall = new Intervall[3];
      // Kanalschleife
      for (var c = 0; c < 3; c++) {
        intervall[c].unten = unten;
        intervall[c].oben = oben;
      }
      return intervall;
    }

    /// <summary> ÜBERLADUNG
    /// 5.10 Normalisiert ein 3D-Array. Diese Methode verschiebt die Werte eines 3D-Arrays für jeden Kanal separat
    /// in einen beliebigen Intervall.
    /// </summary>
    /// <param name="array"></param> 3D-Array als Referenz
    /// <param name="intervall"></param> Neue Intevallgrenzen 
    public static void normalizationArray3D(ref float[,,] array, Intervall[] intervall) {
      // Ermittelt Extremwerte
      Extrem[] extrem = canalExtrem(array);
      // Trippelschleife
      for (var c = 0; c < array.GetLength(0); c++)
        for (var y = 0; y < array.GetLength(1); y++)
          for (var x = 0; x < array.GetLength(2); x++)
            // Intervaltransformation
            toIntervall(ref array[c, y, x], ref extrem[c], ref intervall[c]);
    }


    public static void normalizationArray3D(ref float[,,] array, Intervall[] intervall, int c = 0) {
      // Ermittelt Extremwerte
      Extrem[] extrem = canalExtrem(array);
      // Doppeltschleife
      for (var y = 0; y < array.GetLength(1); y++)
        for (var x = 0; x < array.GetLength(2); x++)
          // Intervaltransformation
          toIntervall(ref array[c, y, x], ref extrem[c], ref intervall[c]);
    }
    #endregion

    #region 6. Nullstellen, Vorzeichenwechsel

    /// <summary>
    /// 6.1 Indizes der vorausliegenden und zu untersuchenden Nachbarelemente. 
    /// </summary>
    public static int[,] ahead8 = { { 1, -1 }, { 1, 0 }, { 1, 1 }, { 0, 1 } };
    public static int[,] ahead4 = { { 1, 0 }, { 0, 1 } };
    /// <summary>
    /// 6.2 Detektiert Vorzeichenwechsel.
    /// Findet ein Vorzeichenwechsel statt, dann wird das Element auf 1 gesetzt.
    /// </summary>
    /// <param name="array"></param> 3D-Array
    /// <param name="ahead"></param> Indizes der vorausliegenden Nachbarelemente
    /// <returns></returns> 3D-Vorzeichenwechsel-Array
    public static float[,,] signChange(float[,,] array, int[,] ahead) {
      // Vorzeichenwechselarray deklarieren
      float[,,] arraySign = new float[array.GetLength(0), array.GetLength(1), array.GetLength(2)];
      // Trippelschleife
      for (var c = 0; c < array.GetLength(0); c++)
        for (var y = 1; y < array.GetLength(1) - 1; y++)
          for (var x = 1; x < array.GetLength(2) - 1; x++) {
            arraySign[c, y, x] = 0;
            // Schleife für die vorausliegenden Nachbarelemente.
            for (var e = 0; e < ahead.GetLength(0); e++) {
              arraySign[c, y, x] = 0; // Hintergrund =0 als Anfangswert
              // Boolesche Operation zur Vorzeichenwechseldetektion.
              bool vzw = (Math.Sign(array[c, y, x]) != Math.Sign(array[c, y + ahead[e, 0], x + ahead[e, 1]]));
              // Boolesche Abfrage des Elementestatus
              bool not0 = false;
              if (Math.Sign(array[c, y, x]) != 0)
                not0 = true;
              // Alternative zum Markieren des Wechsels (Kante =1).
              if (vzw & not0) {
                arraySign[c, y, x] = 1;
                break;
              }
            }
          }
      return arraySign;
    }
    #endregion

    #region 7. Unterstützende Methoden
    /// <summary>
    /// 7.1 Übergibt einem eindimensionalem Feld eine beliebige Parameteranzahl.
    /// </summary>
    /// <param name="canals"></param> Kanalnummern
    /// <returns></returns> Feld mit Kanalnummern
    public static int[] toCanals(params int[] canals) {
      return canals;
    }
    #endregion

  }
}
