﻿using System;
using System.Collections.Generic;
using System.Drawing;

namespace ImageProcessing {
  class ArrayOwn_Copy {
    #region 1. Attribute, Datenstrukturen

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
    /// 3.1 Kopiert ein dreidimensionales Array
    /// </summary>
    /// <param name="array"></param> Source array
    /// <returns></returns> Destination Array
    public static float[,,] copy(float[,,] array) {
      float[,,] destinationArray = new float[array.GetLength(0),
                                             array.GetLength(1), array.GetLength(2)];
      for (int c = 0; c < array.GetLength(0); c++) {
        for (int y = 0; y < array.GetLength(1); y++)
          for (int x = 0; x < array.GetLength(2); x++)
            destinationArray[c, y, x] = (float)array[c, y, x];
      }
      return destinationArray;
    }

    /// <summary> ÜBERLADUNG
    /// 3.2 Kopiert einen Kanal eines zweidimensionalen Arrays.
    /// </summary>
    /// <param name="array"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    public static float[,] copy(float[,,] array, int c = 0) {
      float[,] destinationArray = new float[array.GetLength(1), array.GetLength(2)];
      for (int y = 0; y < array.GetLength(1); y++)
        for (int x = 0; x < array.GetLength(2); x++)
          destinationArray[y, x] = (float)array[c, y, x];
      return destinationArray;
    }

    /// <summary> ÜBERLADUNG, GENERISCH
    /// 3.4 Kopiert ein Kanal eines zweidimensionalen Arrays.
    /// </summary>
    /// <typeparam name="T"></typeparam> Datentyp der Elemente des Arrays
    /// <param name="array"></param> Zweidimensionales Eingangsarray
    /// <param name="c"></param> Kanal der entnommen wird
    /// <returns></returns> Eindimensionales Array
    public static float[] copy<T>(T[,] array, int c = 0) {
      float[] destinationArray = new float[array.GetLength(1)];
      for (int x = 0; x < array.GetLength(1); x++)
        destinationArray[x] = (float)Convert.ChangeType(array[c, x], typeof(float));
      return destinationArray;
    }


    /// <summary> UMKEHRMETHODE ZU 3.4
    /// 3.5 Schreibt die Werte eines eindimensionalen Feldes in ein
    /// zweidimensionales Feld mit dem Index c.
    /// </summary>
    /// <typeparam name="T"></typeparam> Datentyp der Elemente beider Felder
    /// <param name="destination"></param> zweidimensionales Feld
    /// <param name="source"></param> Eindimensionales feld
    /// <param name="c"></param> Index des Zweidimensionalen Feldes
    public static void extend<T>(ref T[,] destination, T[] source, int c = 0) {
      // Elemnte
      for (int x = 0; x < destination.GetLength(1); x++)
        destination[c, x] = source[x];
    }

    /// <summary>
    /// 3.6 Array um einen Rand erweitern (Aufplustern).
    /// - Alle Kanäle drei Kanäle
    /// </summary>
    /// <param name="array"></param> Bildarray
    /// <param name="margin"></param> Randbreite in Pixel
    /// <param name="marginValue"></param> Wert mit dem der Rand aufgefüllt wird.
    /// <returns></returns> 3D-Array mit einem zusätzlichen Rand
    public static float[,,] padding(float[,,] array, int margin, double marginValue) {
      float[,,] paddingArray = new float[3, array.GetLength(1) + 2 * margin, array.GetLength(2) + 2 * margin];
      // Padding (um Rand größeres Array)
      for (int c = 0; c < 3; c++)
        for (int y = 0; y < paddingArray.GetLength(1); y++)
          for (int x = 0; x < paddingArray.GetLength(2); x++)
            paddingArray[c, y, x] = (float)marginValue;
      // Übertragen der Werte der Basisarrays
      for (int c = 0; c < 3; c++)
        for (int y = 0; y < array.GetLength(1); y++)
          for (int x = 0; x < array.GetLength(2); x++)
            paddingArray[c, y + margin, x + margin] = array[c, y, x];
      return paddingArray;
    }

    /// <summary>
    /// 3.6 Array um einen Rand der Breite 1 erweitern (Aufplustern) und Werte aus
    /// Basisarray übernehmen.
    /// - Alle drei Kanäle
    /// </summary>
    /// <param name="array"></param> Bildarray
    /// <returns></returns> 3D-Array mit einem zusätzlichen Rand
    public static float[,,] padding(float[,,] array) {
      // Ein um den Rand größeres Array.
      float[,,] paddingArray = new float[3, array.GetLength(1) + 2, array.GetLength(2) + 2];
      // Hinzugefügten Rand mit den Werten aus Basisarray füllen.
      for (int c = 0; c < 3; c++) {
        for (int y = 0; y < array.GetLength(1); y++) {
          paddingArray[c, y + 1, 0] = array[c, y, 0];
          paddingArray[c, y + 1, paddingArray.GetUpperBound(2)] = array[c, y, array.GetUpperBound(2)];
        }
        for (int x = 0; x < array.GetLength(2); x++) {
          paddingArray[c, 0, x + 1] = array[c, 0, x];
          paddingArray[c, paddingArray.GetUpperBound(1), x + 1] = array[c, array.GetUpperBound(1), x];
        }
        // Setze die Ecken, //ggf. Mittelwert aus drei Nachbarelementen nur für Margin=1
        paddingArray[c, 0, 0] = array[c, 0, 0]; //ggf. Mittelwert aus drei Nachbarelementen
        paddingArray[c, 0, paddingArray.GetUpperBound(2)] =
                                    array[c, 0, array.GetUpperBound(2)];
        paddingArray[c, paddingArray.GetUpperBound(1), 0] =
                                    array[c, array.GetUpperBound(1), 0];
        paddingArray[c, paddingArray.GetUpperBound(1), paddingArray.GetUpperBound(2)] =
                                    array[c, array.GetUpperBound(1), array.GetUpperBound(2)];
      } // c-Schleife
      // Übertragen der Werte der Basisarrays
      for (int c = 0; c < 3; c++)
        for (int y = 0; y < array.GetLength(1); y++)
          for (int x = 0; x < array.GetLength(2); x++)
            paddingArray[c, y + 1, x + 1] = array[c, y, x];
      return paddingArray;
    }

    /// <summary>
    /// 3.7 Eindimensionales Feld beidseitig verlängern (Aufplustern).
    /// </summary>
    /// <param name="feld"></param> Kanal-Tapes
    /// <param name="marginValue"></param> Wert mit dem der Rand aufgefüllt wird.
    /// <param name="margin"></param> Verlängerung in Pixel
    /// <returns></returns> Beidseitig verlängertes 1D-Feld
    public static T[,] padding<T>(T[,] feld2D, float marginValue = 0, int margin = 1) {
      T[,] paddingFeld2D = new T[feld2D.GetLength(0), feld2D.GetLength(1) + 2 * margin];
      // Füllt das Ergebnisfeld komplett mit "borderValue".
      for (int c = 0; c < paddingFeld2D.GetLength(0); c++)
        for (int x = 0; x < paddingFeld2D.GetLength(1); x++)
          paddingFeld2D[c, x] = (T)Convert.ChangeType(marginValue, typeof(T));
      // Übertragen der Werte der Basismatrix in das Ergebnsidfeld.
      for (int c = 0; c < feld2D.GetLength(0); c++)
        for (int x = 0; x < feld2D.GetLength(1); x++)
          paddingFeld2D[c, x + margin] = feld2D[c, x];
      return paddingFeld2D;
    }


    /// <summary> ÜBERLADUNG, GENERISCH
    /// 3.8 Eindimensionales Feld beidseitig verlängern (Aufplustern).
    /// </summary>
    /// <param name="feld"></param> Tape
    /// <param name="marginValue"></param> Wert mit dem der Rand aufgefüllt wird.
    /// <param name="margin"></param> Verlängerung in Pixel
    /// <returns></returns> Beidseitig verlängertes 1D-Feld
    public static T[] padding<T>(T[] feld1D, float marginValue = 0, int margin = 1) {
      T[] paddingFeld1D = new T[feld1D.GetLength(0) + 2 * margin];
      // Füllt das Ergebnisfeld komplett mit "borderValue".
      for (int x = 0; x < paddingFeld1D.GetLength(0); x++)
        paddingFeld1D[x] = (T)Convert.ChangeType(marginValue, typeof(T));
      // Übertragen der Werte der Basismatrix in das Ergebnsidfeld.
      for (int x = 0; x < feld1D.GetLength(0); x++)
        paddingFeld1D[x + margin] = feld1D[x];
      return paddingFeld1D;
    }

    /// <summary> ÜBERLADUNG
    /// 3.9 Histogramm-Felder beidseitig verlängern (Aufplustern).
    /// - r, g, b - Kanäle
    /// </summary>
    /// <param name="feld"></param> Bildarray
    /// <param name="borderValue"></param> Wert mit dem der Rand aufgefüllt wird.
    /// <param name="margin"></param> Verlängerung in Pixel
    /// <returns></returns> Beidseitig verlängertes 1D-Feld
    public static int[] padding(HistogramOwn.Histogram[] feld, int borderValue = 0, int margin = 1) {
      int[] paddingFeld = new int[feld.GetLength(0) + 2 * margin];
      // Padding (Verlängerung)
      for (int x = 0; x < paddingFeld.GetLength(0); x++)
        paddingFeld[x] = borderValue;
      // Übertragen der Werte der Basismatrix
      for (int x = 0; x < feld.GetLength(0); x++)
        paddingFeld[x + margin] = feld[x].r;
      return paddingFeld;
    }


    /// <summary>
    /// 3.10 Entfernt den Rand eines 3D-Arrays
    /// der Breite von einem Pixel.
    /// </summary>
    /// <param name="paddingArray"></param> Array mit Rand
    /// <returns></returns> Array ohne Rand
    public static float[,,] unpadding(float[,,] paddingArray) {
      float[,,] array = new float[3, paddingArray.GetLength(1) - 2, paddingArray.GetLength(2) - 2];
      // Übertragen der Werte
      for (int c = 0; c < 3; c++)
        for (int y = 0; y < array.GetLength(1); y++)
          for (int x = 0; x < array.GetLength(2); x++)
            array[c, y, x] = paddingArray[c, y + 1, x + 1];
      return array;
    }

    /// <summary>
    /// 3.10 Entfernt den Rand eines 3D-Arrays
    /// der Breite von einem Pixel.
    /// </summary>
    /// <param name="paddingArray"></param> Array mit Rand
    /// <returns></returns> Array ohne Rand
    public static float[,,] unpadding(float[,,] paddingArray, int margin = 1) {
      float[,,] array = new float[3, paddingArray.GetLength(1) - 2 * margin, paddingArray.GetLength(2) - 2 * margin];
      // Übertragen der Werte
      for (int c = 0; c < 3; c++)
        for (int y = 0; y < array.GetLength(1); y++)
          for (int x = 0; x < array.GetLength(2); x++)
            array[c, y, x] = paddingArray[c, y + margin, x + margin];
      return array;
    }

    /// <summary>
    /// 3.10a Verschiebt die Startkoordinaten der Freeman-Code.
    /// </summary>
    /// <param name="chaincodes"></param> Doppelliste mit Freemancode
    public static void pointsUnpadding(ref List<List<uint>> chaincodes) {
      for (int i = 1; i < chaincodes.Count; i++) {
        chaincodes[i][0] -= 1;
        chaincodes[i][1] -= 1;
      }
    }

    /// <summary>
    ///  3.11 Setzt alle Werte eines Kanals eines Arrays auf einen festen Wert.
    /// </summary>
    /// <param name="array"></param> 3D-Array
    /// <param name="canal"></param> Kanal, Layer, Bildebene
    /// <param name="value"></param> Wert, der gesetzt werden soll.
    public static void setArrayChannelValue(ref float[,,] array, int canal = 1, float value = 0) {
      for (var y = 0; y < array.GetLength(1); y++)
        for (var x = 0; x < array.GetLength(2); x++)
          array[canal, y, x] = value;
    }
    /// <summary> ÜBERLADUNG
    /// 3.11a Setzt alle Werte aller Kanäle eines Arrays auf einen festen Wert.
    /// </summary>
    /// <param name="array"></param> Referenz-Array
    /// <param name="value"></param> Wert der gesetzt wird.
    public static void setArrayChannelValue(ref float[,,] array, float value = 0) {
      for (var c = 0; c < array.GetLength(0); c++)
        for (var y = 0; y < array.GetLength(1); y++)
          for (var x = 0; x < array.GetLength(2); x++)
            array[c, y, x] = value;
    }

    /// <summary>
    /// 3.12 Kopiert Kanallayer auf alle anderen Kanallayers.
    /// </summary>
    /// <param name="array"></param> Referenz: 3D-Array
    /// <param name="sourceCanal"></param> Quellkanal
    public static void copyCanalToCanal(ref float[,,] array, int sourceCanal = 2) {
      for (var c = 0; c < array.GetLength(0); c++)
        for (var y = 0; y < array.GetLength(1); y++)
          for (var x = 0; x < array.GetLength(2); x++)
            array[c, y, x] = array[sourceCanal, y, x];
    }

    /// <summary>
    /// 3.13 Kopiert Quell-Kanallayer auf Ziel-Kanallayers.
    /// </summary>
    /// <param name="array"></param> Referenz: 3D-Array
    /// <param name="sourceCanal"></param> Quellkanal
    /// <param name="destinationCanal"></param> Zielkanal
    public static void copyCanalToCanal(ref float[,,] array, int sourceCanal, int destinationCanal) {
      for (var y = 0; y < array.GetLength(1); y++)
        for (var x = 0; x < array.GetLength(2); x++)
          array[destinationCanal, y, x] = array[sourceCanal, y, x];
    }

    #endregion

    #region 4. 2D-Array



    /// <summary>
    /// 4.1 Verändert die Größe eines zweidimensionalen Arrays
    /// </summary>
    /// <typeparam name="T"></typeparam> Datentyp der Arrayelemente.
    /// <param name="array"></param> zweidimensionales Array
    /// <param name="newRows"></param> neue Zeilenzah y
    /// <param name="newCols"></param> neue Spaltenanzahl x
    /// <returns></returns> Array mit geenderter Größe.
    public T[,] Resize2D<T>(T[,] array, int newRows, int newCols) {
      var arrayResize = new T[newRows, newCols];
      int minRows = Math.Min(newRows, array.GetLength(0));
      int minCols = Math.Min(newCols, array.GetLength(1));
      for (int y = 0; y < minRows; y++)
        for (int x = 0; x < minCols; x++)
          arrayResize[y, x] = array[y, x];
      return arrayResize;
    }

    /// <summary> NICHT GETESTET [https://stackoverflow.com/questions/6539571/how-to-resize-multidimensional-2d-array-in-c]
    /// 4.2 Verändert die Größe eines zweidimensionalen Arrays
    /// </summary>
    /// <typeparam name="T"></typeparam>Datentyp der Arrayelemente.
    /// <param name="array"></param> zweidimensionales Array
    /// <param name="newWidth"></param> Neue Größe in x
    /// <param name="newHeight"></param> Neue Größe in y
    /// <param name="offsetX"></param> Verschiebung in x
    /// <param name="offsetY"></param> Veschiebung in y
    /// <returns></returns> Array mit neuer Größe
    public static T[,] ResizeArray<T>(T[,] array, int newHeight, int newWidth, int offsetX = 0, int offsetY = 0) {
      T[,] newArray = new T[newHeight, newWidth];
      int height = array.GetLength(0);
      int width = array.GetLength(1);
      for (int x = 0; x < width; x++) {
        Array.Copy(array, x * height, newArray, (x + offsetX) * newHeight + offsetY, height);
      }
      return newArray;
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
    /// </summary>  RUFT PARTNERMETHODE
    /// 5.1 Normalisert Elemente des Arrays für jeden Kanal
    /// unabhängig voneinander.
    /// Die Neuen Intervallgrenzen können vorgegeb werden. Zum Beispiel 0, 1
    /// <param name="array"></param> 3D-Array
    /// <param name="unten"></param> neue untere Intervallgrenze [0, 255]
    /// <param name="oben"></param> neue ober Intervallgrenze [0, 255]
    /// <returns></returns> Aray in den neuen Intervallgrenzen.
    public static void normalizationArray3Dcanales(ref float[,,] array, float unten = 0, float oben = 1) {
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

    /// <summary> VERWANDT
    /// Normalisiert zwischen minimalen und dem maximalen Intensitätswert
    /// aller Kanäle im Intervall von 0 bis .... Normalisiert Float-Elemente der Kanäle
    /// eines 3D-Arrays in einen darstellbaren Farbbereich.
    /// </summary>
    /// <param name="array"></param> 3D-Array [k,y,x]
    /// <param name="unten"></param> Untere Inervallgrenze nach der Normalisierung
    /// <param name="oben"></param> Obere Intervallgrenze nach der Normalisierung
    public static void normalizationArray3Ddepending(ref float[,,] array, float unten = 0, float oben = 1) {
      Extrem extrem;
      extrem.max = 0; //      array[0, 0, 0]; //  Maximum des Kanal 1
      extrem.min = 255; // array[0, 0, 0];
      Intervall intervall;
      intervall.oben = oben;
      intervall.unten = unten;
      for (var k = 0; k < array.GetLength(0); k++)
        for (var y = 0; y < array.GetLength(1); y++)
          for (var x = 0; x < array.GetLength(2); x++) {
            if (extrem.max < array[k, y, x])
              extrem.max = array[k, y, x];
            if (extrem.min > array[k, y, x])
              extrem.min = array[k, y, x];
          }
      for (var k = 0; k < array.GetLength(0); k++)
        for (var y = 0; y < array.GetLength(1); y++)
          for (var x = 0; x < array.GetLength(2); x++)
            toIntervall(ref array[k, y, x], ref extrem, ref intervall);
    }

    /// <summary>
    /// Spiegelt um die Y-Achse.
    /// Damit sich der Koordinstenursprung
    ///  in der linken unteren Ecke befindet. 
    /// </summary>
    /// <param name="array"></param> 3D-Array
    public static void yInvers(ref float[,,] array) {
      float[] zeile = new float[array.GetLength(2)];
      for (var k = 0; k < array.GetLength(0); k++)
        for (var y = 0; y < array.GetLength(1) / 2; y++) {
          for (var x = 0; x < array.GetLength(2); x++)
            zeile[x] = array[k, y, x]; //merke
          for (var x = 0; x < array.GetLength(2); x++) {
            array[k, y, x] = array[k, array.GetLength(1) - 1 - y, x];
            array[k, array.GetLength(1) - 1 - y, x] = zeile[x];
          }
        }





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
    /// <param name="extrem"></param> Extremwerte für einen Kanal
    /// <param name="intervall"></param> Zielintervall für einen Kanal
    /// <returns></returns> Wert im neuen Intervall
    public static float toIntervall(float x, Extrem extrem, Intervall intervall) {
      return (float)((x - extrem.min) * (intervall.oben - intervall.unten) / (extrem.max - extrem.min) + intervall.unten);
    }
    /// <summary> ÜBERLADUNG
    /// 5.5 Normalisiert ein einzelnes Element
    /// </summary>
    /// <param name="x"></param> Element x als Wert
    /// <param name="extrem"></param> Extremwerte für einen Kanal
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

    #region 7. Konvertieren
    /// <summary>
    /// 7.1 Übergibt einem eindimensionalem Feld eine beliebige Parameteranzahl.
    /// </summary>
    /// <param name="canals"></param> Kanalnummern
    /// <returns></returns> Feld mit Kanalnummern
    public static int[] toCanals(params int[] canals) {
      return canals;
    }
    /// <summary>
    /// 7.2 Übergibt einem eindimensionalem Feld eine beliebige Parameteranzahl
    /// mit parametrierbarem Datentyp.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="feld"></param>
    /// <returns></returns>
    public static T[] toField<T>(params T[] feld) {
      return feld;
    }

    public static T[] toSeries<T>(params T[] feld) {
      return feld;
    }


    /// <summary>
    /// Konvertiert eine zweidimensionale Listenstruktur in ein
    /// zweidimensionales dynamisches Feld.
    /// </summary>
    /// <param name="list2D"></param> zweidimensionale Liste
    /// <returns></returns> zweidimensionales dynamisches Feld
    public static float[][] toField(List<List<float>> list2D) {
      float[][] feld2Dyn = new float[list2D.Count][];
      for (int c = 0; c < list2D.Count; c++) {
        feld2Dyn[c] = new float[list2D[c].Count];
        for (int i = 0; i < list2D[i].Count; i++) {
          feld2Dyn[c][i] = list2D[c][i];
        }
      }
      return feld2Dyn;
    }

    /// <summary>
    /// Konvertiert eine Liste in ein
    /// eindimensionales Feld.
    /// </summary>
    /// <param name="list1D"></param> Liste
    /// <returns></returns> Feld
    public static float[] toField(List<float> list1D) {
      float[] feld1D = new float[list1D.Count];
      for (int c = 0; c < list1D.Count; c++)
        feld1D[c] = list1D[c];
      return feld1D;
    }
    #endregion

    #region 8. Nachbarn

    /// <summary>
    /// 8.1 Verschiebung der Nachbarn für ein 3x3-Kernel, Index i für x-Verschiebung, Index j für y-Verschiebung 
    /// </summary>
    public static int[,] shift = { { -1, 0 }, { -1, -1 }, { 0, -1 }, { 1, -1 }, { 1, 0 }, { 1, +1 }, { 0, +1 }, { -1, +1 } };

    /// <summary>
    /// 8.2 Rückwärts-Verschiebung der Nachbarn, Index i für x-Verschiebung, Index j für y-Verschiebung.
    /// Die Frage lautet, wieweit sollte zurückgeblickt werden?
    /// </summary>
    public static int[,] backwardShift = { { -1, 0 }, { -1, -1 }, { 0, -1 }, { +1, -1 } };

    /// <summary>
    /// 8.2 Rückwärts-Verschiebung der Nachbarn, Index i für x-Verschiebung, Index j für y-Verschiebung.
    /// Die Frage lautet, wieweit sollte zurückgeblickt werden?
    /// </summary>
    public static int[,] backwardShift2 = {
            { -1, 0 }, { -1, -1 }, { 0, -1 }, { +1, -1 } ,
            {-2,0 },{-2,-1},{-2,-2},{-1,-2},{0,-2},{1,-2},{2,-2},{2,1 }
                                           };



    /// <summary>
    /// 8.2b Rückwärts-Verschiebung der Nachbarn, Index i für x-Verschiebung, Index j für y-Verschiebung.
    /// Die Frage lautet, wieweit sollte zurückgeblickt werden?
    /// </summary>
    public static int[,] backwardShift12 = { { -1, 0 }, { -1, -1 }, { 0, -1 }, { 1, -1 },
                                           {  2, -1}, {  3, -1 }, { 4, -1 }, { 5, -1 },
                                           {  6, -1}, {  7, -1 }, { 8, -1 }, { 9, -1 }};

    /// <summary>
    /// 8.3 Vorwärts-Verschiebung der Nachbarn, Index i für x-Verschiebung, Index j für y-Verschiebung 
    /// </summary>
    public static int[,] forwardShift = { { 1, 0 }, { 1, +1 }, { 0, +1 }, { -1, +1 } };




    /// <summary>
    /// 8.4 Verschiebung der Nachbarn für ein 5x5-Kernel, Index i für x-Verschiebung, Index j für y-Verschiebung.
    /// Reihenfolge Stern
    /// </summary>
    public static int[,] shift5 = { { -1, 0 }, { -1, -1 }, { 0, -1 }, { 1, -1 }, { 1, 0 }, { 1, +1 }, { 0, +1 }, { -1, +1 },
                                    { -2, 0 }, { -2, -2 }, { 0, -2 }, { 2, -2 }, { 2, 0 }, { +2, +2 }, { 0, +2 }, { -2, +2 },
                                    { -2, -1 }, { -1, -2 }, { 1, -2 }, { 2, -1 }, { 2, 1 }, { 1, +2 }, { -1, +2 }, { -2,+1 }};

    /// <summary>
    /// 8.5 Verschiebung der Nachbarn für ein 5x5-Kernel, Index i für x-Verschiebung, Index j für y-Verschiebung,
    /// Reihenfolge im Uhrzeigersinn
    /// </summary>
    public static int[,] shift5_ = {{ -1, 0 }, {-1, -1 }, { 0, -1 }, { 1, -1 }, { 1, 0 }, { 1, +1 }, { 0, +1 }, { -1, +1},
                                    { -2, 0 },{ -2, -1 }, { -2, -2 }, { -1, -2 }, { 0, -2 },{ 1, -2 },{ 2, -2 }, { 2, -1 },
                                    { 2, 0 }, {  2, 1 },  { +2, +2 }, { 1, +2 }, { 0, +2 },{ -1, +2 }, { -2, +2 },{ -2,+1 }};

    /// <summary>
    /// 8.6 Transformiert zweidimensionales Feld in ein eindimensionales Feld
    /// mit einer Punktstruktur.
    /// </summary>
    /// <param name="shift"></param> Zweidimensionales Feld
    /// <returns></returns> Eindimensionales Punktefeld.
    public static Point[] toPoints(int[,] shift) {
      Point[] nachbar = new Point[shift.GetLength(0)];
      for (int s = 0; s < shift.GetLength(0); s++) {
        nachbar[s].X = shift[s, 0];
        nachbar[s].Y = shift[s, 1];
      }
      return nachbar;
    }


    /// <summary>
    /// 8.7 Liefert den Kreisintervall einer ganzenZahl ür den Intervall von 0 bis ns.
    /// Intervall [0, ns[
    /// </summary>
    /// <param name="s"></param> ganze Zahl
    /// <param name="ns"></param> exclusive obere Intervallgrenze
    /// <returns></returns> Wert innerhalb des Kreisintervalls
    public static int circleInterval(int s, int ns = 8) {
      s = s % ns;
      if (s < 0)
        s = ns + s;
      return s;
    }
    /// <summary> ÜBERLADUNG
    /// 8.8 Liefert den Kreisintervall einer ganzen Zahl für den Intervall von 0 bis ns.
    /// Intervall [0, ns[
    /// </summary>
    /// <param name="s"></param> ganze Zahl --ref--> exclusive obere Intervallgrenze
    /// <param name="ns"></param> exclusive obere Intervallgrenze
    public static void circleInterval(ref int s, int ns = 8) {
      s = s % ns;
      if (s < 0)
        s = ns + s;
    }
    #endregion

    #region 9.1 Logische Operationen, 3D-Arrays

    /// <summary>
    /// 9.1 Verknüpft die Elemente zweier Kanäle mit Or
    /// canal 2 erhält das Ergebnis der logischen verknüpfung
    /// </summary>
    /// <param name="array"></param> Referenz: 3D-Array
    /// <param name="canal1"></param> Canal 1
    /// <param name="canal2"></param> Canal 2, Ergebnsi
    public static void canalOrCanal(ref float[,,] array, int canal1, int canal2) {
      for (var y = 0; y < array.GetLength(1); y++)
        for (var x = 0; x < array.GetLength(2); x++)
          if (array[canal1, y, x] == 1 || array[canal2, y, x] == 1)
            array[canal2, y, x] = 1;

    }

    /// <summary>
    /// Oderverknüpfung zweier Arrays
    /// </summary>
    /// <param name="array"></param> Array wird verändert
    /// <param name="array1"></param> Array1 bleibt unverändert
    public static void arrayOrArray(ref float[,,] array, ref float[,,] array1) {
      for (var c = 0; c < array.GetLength(0); c++)
        for (var y = 0; y < array.GetLength(1); y++)
          for (var x = 0; x < array.GetLength(2); x++)
            if (array1[c, y, x] == 1)
              array[c, y, x] = 1;
    }



    /// <summary>
    /// Oder-Verknüpfung zur Kanaladdition
    /// </summary>
    /// <param name="array"></param> Array wird verändert
    /// <param name="array1"></param> Array1 bleibt unverändert
    public static void arrayAddArray(ref float[,,] array, ref float[,,] array1) {
      for (var c = 0; c < array.GetLength(0); c++)
        for (var y = 0; y < array.GetLength(1); y++)
          for (var x = 0; x < array.GetLength(2); x++)
            array[c, y, x] += array1[c, y, x];
      normalizationArray3Dcanales(ref array);
    }

    #endregion


  }
}
