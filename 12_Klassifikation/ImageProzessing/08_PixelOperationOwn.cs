/* Klasse PixelOperationOwn: Bildpunktoperationen für Bitmaps und Bildarrays. Dazu gehören die Intensitätsformung und die Falschfarbendarstellung.
 * Verfasser: Horst-Christian Heinke
 * Datum: 19.05.2022
 * Letzte Änderung: 15.01.2023
 * Germany, Sachsen-Anhalt, Magdeburg
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using ARRAY = ImageProcessing.ArrayOwn; // Statische Elementarmethoden

namespace ImageProcessing {
  internal class PixelOperationOwn {

    #region 1. Grauwertbild
    /// <summary>
    /// 1.1 Wandelt ein dreidimensionales Array mit Farbwerten in eine Grauwertarray um.
    /// Die Feldelemente sind vom Typ “float”.
    /// </summary>
    /// <param name="array3D"></param> 3D-Farb-Array mit float-Elementen
    /// <returns></returns> Grauwert-Array
    public void toGray(ref float[,,] array3D) {
      int channels = array3D.GetLength(0); // Gestalt
      int height = array3D.GetLength(1);
      int width = array3D.GetLength(2);
      float gray = 0;
      for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++) {
          gray = (float)((array3D[0, y, x] + array3D[1, y, x] + array3D[2, y, x]) / array3D.GetLength(0));
          for (int c = 0; c < array3D.GetLength(0); c++)
            array3D[c, y, x] = gray;
        }
    }

    /// <summary> ÜBERLADUNG
    /// 1.2 Wandelt ein dreidimensionales Array mit Farbwerten in eine Grauwertarray um.
    /// Für Byte-Werte
    /// </summary>
    /// <param name="array3D"></param> 3D-Farb-Array mit byte-Elementen
    /// <returns></returns> Grauwert-Array
    public void toGray(ref byte[,,] array3D) {
      int channels = array3D.GetLength(0); // Gestalt
      int height = array3D.GetLength(1);
      int width = array3D.GetLength(2);
      byte gray = 0;
      for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++) {
          gray = (byte)((array3D[0, y, x] + array3D[1, y, x] + array3D[2, y, x]) / array3D.GetLength(0));
          for (int c = 0; c < array3D.GetLength(0); c++)
            array3D[c, y, x] = gray;
        }
    }

    /// <summary> ÜBERLADUNG
    /// 1.3 Wandelt die Elemente eines gefletteten Bildes in
    /// Grauwerte um. Die Länge bleibt erhalten.
    /// </summary>
    /// <param name="feld"></param> Eindimensionales Feld
    /// <returns></returns> Grauwert-Feld
    public void toGray(ref float[] feld) {
      int ebenenPixel = (int)feld.Length / 3; // Versatz zwischen den Kanalebenen
      for (int i = 0; i < ebenenPixel; i++) {
        float gray = (float)((feld[i] + feld[i + ebenenPixel] + feld[i + 2 * ebenenPixel]) / 3);
        feld[i] = gray;
        feld[i + ebenenPixel] = gray;
        feld[i + 2 * ebenenPixel] = gray;
      }
    }

    /// <summary> ÜBERLADUNG
    /// 1.4 Wandelt die Elemente eines gefletteten Bildes in
    /// Grauwerte um. Die Länge bleibt erhalten.
    /// </summary>
    /// <param name="feld"></param> Eindimensionales Feld
    /// <returns></returns> Grauwert-Feld
    public void toGray(ref byte[] feld) {
      int ebenenPixel = (int)(feld.Length / 3);
      for (int i = 0; i < ebenenPixel; i++) {
        byte gray = (byte)((feld[i] + feld[i + ebenenPixel] + feld[i + 2 * ebenenPixel]) / 3);
        feld[i] = gray;
        feld[i + ebenenPixel] = gray;
        feld[i + 2 * ebenenPixel] = gray;
      }
    }
    #endregion

    #region 1a. Delegate zur Erzeugung eines Grauwertbildes.
    /// <summary>
    /// 1a.1 Delegate zur Grauwertberechnung aus den drei typparametrierten Grundfarben.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="R"></param> Kanal 0, Rot
    /// <param name="G"></param> Kanal 0, Grün
    /// <param name="B"></param> Kanal 0, Blau
    /// <returns></returns> Grauwert
    public delegate T grayDelegate<T>(T R, T G, T B);

    /// <summary>
    /// 1a.2 Variante 1: R, G, B und Ergebnis sind vom Type Byte
    /// </summary>
    /// <param name="R"></param> Rot
    /// <param name="G"></param> Grün
    /// <param name="B"></param> Blau
    /// <returns></returns> Gray in Byte
    public byte Byte(byte R, byte G, byte B) {
      return (byte)((R + G + B) / 3);
    }

    /// <summary>
    /// 1a.3 Varainte 2:R, G, B und Ergebnis sind vom Type Float
    /// </summary>
    /// <param name="R"></param> Rot
    /// <param name="G"></param> Grün
    /// <param name="B"></param> Blau
    /// <returns></returns> Gray in Float
    public float Float(float R, float G, float B) {
      return (float)((R + G + B) / 3);
    }

    /// <summary>
    /// 1a.4 Wandelt die Elemente eines gefletteten Bildes in
    /// Grauwerte um. Die Länge bleibt erhalten. Über den Delegaten
    /// kann die Vewendung für die Datentypen "Byte" oder "Float"
    /// vorgegeben werden.
    /// </summary>
    /// <typeparam name="T"></typeparam> Datentyp [byte, float]
    /// <param name="feld"></param> Eindimensionales Feld
    /// <param name="grayDelegate"></param> Delegate [Byte, Float]
    /// <returns></returns>
    public void toGray<T>(T[] feld, grayDelegate<T> grayDelegate) {
      int ebenenPixel = (int)(feld.Length / 3);
      for (int i = 0; i < ebenenPixel; i++) {
        T R = feld[i];
        T G = feld[i + ebenenPixel];
        T B = feld[i + ebenenPixel + ebenenPixel];
        T gray = grayDelegate(R, G, B);
        feld[i] = gray;
        feld[i + ebenenPixel] = gray;
        feld[i + 2 * ebenenPixel] = gray;
      }
    }
    #endregion

    #region 2. Binaer
    /// <summary>
    /// 2.1 Erzeugt ein Binärbild im RGB-Spektrum. Der Schwellwert entscheidet,
    /// ob der Intensitätswert eines Pixel auf Maximum oder auf Minimum gesetzt wird [0, 1].
    /// Ein Schwarz-Weiß-Bild läßt sich auf basis eines Grauwertbildes erzeugen.
    /// </summary>
    /// <param name="feld"></param> Referenz eines normierten, eindimensionalen Feldes
    /// <param name="threshold"></param> Schwellwert
    public void toBinaer(ref float[] feld, double threshold = 0.5) {
      for (int i = 0; i < feld.Length; i++)
        if (feld[i] > threshold)
          feld[i] = 1;
        else
          feld[i] = 0;
    }

    /// <summary> ÜBERLADUNG
    /// 2.2 Erzeugt ein Binärbild im RGB-Spektrum. Der Schwellwert entscheidet,
    /// ob der Intensitätswert eines Pixel auf Maximum oder auf Minimum gesetzt wird (0, 255).
    /// Ein Schwarz-Weiß-Bild läßt sich auf Basis eines Grauwertbildes erzeugen.
    /// </summary>
    /// <param name="feld"></param> Normiertes, eindimensionales Feld
    /// <param name="threshold"></param> Schwellwert
    public void toBinaer(ref byte[] feld, byte threshold = 127) {
      for (int i = 0; i < feld.Length; i++)
        if (feld[i] > threshold)
          feld[i] = 255;
        else
          feld[i] = 0;
    }

    /// <summary> ÜBERLADUNG
    /// 2.3 Erzeugt ein Binärbild im RGB-Spektrum. Der Schwellwert entscheidet,
    /// ob der Intensitätswert eines Pixel auf Maximum oder auf Minimum gesetzt wird [0, 1].
    /// Ein Schwarz-Weiß-Bild läßt sich auf basis eines Grauwertbildes erzeugen.
    /// </summary>
    /// <param name="array"></param> Dreidimensionales Feld
    /// <param name="threshold"></param> Schwellwert
    public void toBinaer(ref float[,,] array, double threshold = 0.5) {
      for (int c = 0; c < array.GetLength(0); c++)
        for (var y = 0; y < array.GetLength(1); y++)
          for (var x = 0; x < array.GetLength(2); x++) {
            if (array[c, y, x] > threshold)
              array[c, y, x] = 1;
            else
              array[c, y, x] = 0;
          }
    }

    /// <summary> ÜBERLADUNG
    /// 2.4 Erzeugt ein Binärbild im RGB-Spektrum. Der Schwellwert[c] entscheidet,
    /// ob der Intensitätswert eines Pixel auf ein Minimum oder
    /// ein Maximum gesetzt wird [0, 1].
    /// Ein Schwarz-Weiß-Bild läßt sich auf basis eines Grauwertbildes erzeugen.
    /// </summary>
    /// <param name="array"></param> Dreidimensionales Feld
    /// <param name="threshold"></param> Schwellwert-Feld, für jeden Kanal separat
    public void toBinaer(ref float[,,] array, List<List<byte>> threshold) {
      for (int c = 0; c < array.GetLength(0); c++)
        for (var y = 0; y < array.GetLength(1); y++)
          for (var x = 0; x < array.GetLength(2); x++) {
            if (array[c, y, x] > threshold[c][0] / 255.0)
              array[c, y, x] = (float)1.0;
            else
              array[c, y, x] = (float)0.0;
          }
    }

    /// <summary> ÜBERLADUNG
    /// 2.5 Generiert kanalbezogene Binärbilder
    /// </summary>
    /// <param name="array"></param> Referenz eines dreidimensionalen Bildarrays
    /// <param name="threshold"></param> Liste mit kanalbezogenen Schwellwerten
    public void toBinaer(ref float[,,] array, List<byte> threshold) {
      for (int c = 0; c < array.GetLength(0); c++)
        for (var y = 0; y < array.GetLength(1); y++)
          for (var x = 0; x < array.GetLength(2); x++) {
            if (array[c, y, x] > threshold[c] / 255.0)
              array[c, y, x] = (float)1.0;
            else
              array[c, y, x] = (float)0.0;
          }
    }

    /// <summary> ÜBERLADUNG
    /// 2.6 Erzeugt ein Binärbild im RGB-Spektrum. Der Schwellwert[c] entscheidet,
    /// ob der Intensitätswert eines Pixel auf ein Minimum oder ein Maximum
    /// gesetzt wird [0, 255].
    /// Ein Schwarz-Weiß-Bild läßt sich auf basis eines Grauwertbildes erzeugen.
    /// </summary>
    /// <param name="array"></param> Dreidimensionales Array [c,y,x]
    /// <param name="threshold"></param> Schwellwert-Feld, für jeden Kanal separat
    public void toBinaer(ref byte[,,] array, List<List<byte>> threshold) {
      for (int c = 0; c < array.GetLength(0); c++)
        for (var y = 0; y < array.GetLength(1); y++)
          for (var x = 0; x < array.GetLength(2); x++) {
            if (array[c, y, x] > threshold[c][0])
              array[c, y, x] = 255;
            else
              array[c, y, x] = 0;
          }
    }

    #endregion

    #region 3. Invers
    /// <summary>
    /// 3.1 Invertiert die Elemenrte eines eindimensionalen Feldes.
    /// Das entspricht den Komplementärfarben eines Bildes.
    /// </summary>
    /// <param name="feld"></param> Referenz eines normierten, eindimensionalen Feldes.
    public void toInvert(ref float[] feld) {
      for (int i = 0; i < feld.Length; i++)
        feld[i] = 1 - feld[i];
    }

    /// <summary> ÜBERLADUNG
    /// 3.2 Invertiert die Elemente eins dreidimensionales Array. Das entspricht den 
    /// Komplementärfarben eines Bildes.
    /// </summary>
    /// <param name="array"></param> dreidimensionales Array mit Gleitkommawerten.
    public void toInvert(ref float[,,] array) {
      for (int c = 0; c < array.GetLength(0); c++)
        for (var y = 0; y < array.GetLength(1); y++)
          for (var x = 0; x < array.GetLength(2); x++)
            array[c, y, x] = 1 - array[c, y, x];
    }

    /// <summary> ÜBERLADUNG
    /// 3.3 Invertiert die Elemente eins dreidimensionales Array. Das entspricht den 
    /// Komplementärfarben eines Bildes.
    /// </summary>
    /// <param name="array"></param> dreidimensionales Array mit Byte-Werten.
    public void toInvert(ref byte[,,] array) {
      for (int c = 0; c < array.GetLength(0); c++)
        for (var y = 0; y < array.GetLength(1); y++)
          for (var x = 0; x < array.GetLength(2); x++)
            array[c, y, x] = (byte)(255 - array[c, y, x]);
    }

    /// <summary> ÜBERLADUNG
    /// 3.4 Invertiert die Elemente eins eines Kanals eines dreidimensionales Array.
    /// </summary>
    /// <param name="array"></param> dreidimensionales Array mit Gleitkommawerten.
    public void toInvert(ref float[,,] array, int canal = 1) {
      for (var y = 0; y < array.GetLength(1); y++)
        for (var x = 0; x < array.GetLength(2); x++)
          array[canal, y, x] = 1 - array[canal, y, x];
    }

    #endregion

    #region 4.Intensitätswertespreizung,  Intensitätsformung
    /// <summary>
    /// 4.1 Lineare Intensitätsspreizung zwischen dem
    /// minimalen und dem maximalen Intensitätswert im 
    /// Intervall von 0 bis 1 über alle Kanäle
    /// </summary> 
    /// <param name="feld"></param> Feld mit Intensitätswerten
    /// <returns></returns> [Intensitätsminimum, Intensitätsmaximum]
    public float[] linearSpreizung(ref float[] feld) {
      float[] Iextrem = new float[2];  // Extremwerte
      Iextrem[0] = feld.Min();
      Iextrem[1] = feld.Max();
      for (int i = 0; i < feld.Length; i++) {
        feld[i] = (feld[i] - Iextrem[0]) / (Iextrem[1] - Iextrem[0]);
      }
      return Iextrem;
    }

    /// <summary> ÜBERLADUNG
    /// 4.1a Lineare Intensitätsspreizung zwischen dem
    /// minimalen und dem maximalen Intensitätswert im 
    /// Intervall von 0 bis 1 für jeden Kanal spezifisch.
    /// </summary> 
    /// <param name="array"></param> Dreidimensionales Bildarray
    /// <returns></returns> Feld-Struktur der Extremwerte[c].min ...max
    public ARRAY.Extrem[] linearSpreizung(ref float[,,] array) {
      // Liefert die Minma und die Maxima aller Kanäle.
      ARRAY.Extrem[] Iextrem = ARRAY.canalExtrem(array);
      for (int c = 0; c < array.GetLength(0); c++)
        for (var y = 0; y < array.GetLength(1); y++)
          for (var x = 0; x < array.GetLength(2); x++)
            array[c, y, x] = (array[c, y, x] - Iextrem[c].min) / (Iextrem[c].max - Iextrem[c].min);
      return Iextrem;
    }

    /// <summary> ÜBERLADUNG für ausgewählte Kanäle
    /// 4.1b Lineare Intensitätsspreizung zwischen dem
    /// minimalen und dem maximalen Intensitätswert im 
    /// Intervall von 0 bis 1 für jeden Kanal spezifisch.
    /// </summary> 
    /// <param name="array"></param> Dreidimensionales Bildarray
    /// <param name="canals"></param> Ausgewählte Kanäle als Feld
    /// <returns></returns> Feld-Struktur der Extremwerte[c].min ...max
    public ARRAY.Extrem[] linearSpreizung(ref float[,,] array, int[] canals) {
      // Liefert die Minma und die Maxima aller Kanäle.
      ARRAY.Extrem[] Iextrem = ARRAY.canalExtrem(array);
      for (int c = 0; c < canals.GetLength(0); c++)
        for (var y = 0; y < array.GetLength(1); y++)
          for (var x = 0; x < array.GetLength(2); x++)
            array[canals[c], y, x] = (array[canals[c], y, x] - Iextrem[canals[c]].min) / (Iextrem[canals[c]].max - Iextrem[c].min);
      return Iextrem;
    }

    /// <summary>
    /// 4.2 Lineare Intensitätsformung zwischen einem 
    /// unterem und oberen Grenzwert im 
    /// Intervall von 0 bis 1. Alle Werte unter dem unteren Grenzwert
    /// werden auf 0 und alle Werte über dem oberen Grenzwert werden
    /// auf 1 gesetzt.
    /// </summary>
    /// <param name="feld"></param>Feld mit Intensitätswerten
    /// <param name="min"></param> unterer Grenzwert
    /// <param name="max"></param> oberer Grenzwert
    public void linearFormung(ref float[] feld, double min, double max) {
      for (int i = 0; i < feld.Length; i++) {
        feld[i] = (feld[i] - (float)min) / (float)(max - min);
        if (feld[i] < 0.0)
          feld[i] = 0;
        if (feld[i] > 1.0)
          feld[i] = 1;
      }
    }

    /// <summary> ÜBERLADUNG
    /// 4.2a Lineare Intensitätsspreizung zwischen dem
    /// 0 und dem maximalen Intensitätswert im 
    /// Intervall von 0 bis 1. Normalisiert Float-Elemente eines Kanals
    /// eines 3D-Feldes in einen darstellbaren Farbbereich.
    /// Umrechnung in den Intervall [0,1].
    /// </summary>
    /// <param name="array"></param> 3D-Array
    /// <param name="canal"></param> Kanal, Layer, Bildebene
    public void linearFormung(ref float[,,] array, int canal = 1) {
      float max = 0; //  Maximum des Kanal 1
      float min = 1;
      for (var y = 0; y < array.GetLength(1); y++)
        for (var x = 0; x < array.GetLength(2); x++) {
          if (max < array[canal, y, x])
            max = array[canal, y, x];
          if (min > array[canal, y, x])
            min = array[canal, y, x];
        }
      for (var y = 0; y < array.GetLength(1); y++)
        for (var x = 0; x < array.GetLength(2); x++)
          array[canal, y, x] = (array[canal, y, x] - (float)min) / (float)(max - min);
    }

    /// <summary>
    /// 4.3 Intensitätsänderung mittels Sigmoidfunktion.
    /// </summary>
    /// <param name="feld"></param> Feld
    /// <param name="a0"></param> Funktionswendepunkt in horizontaler Richtung
    /// <param name="a1"></param> Streckung und Stauchung der Funktion
    public void sigmoidFormung(ref float[] feld, double a0, double a1) {
      for (int i = 0; i < feld.Length; i++) {
        feld[i] = (float)(1 / (1 + Math.Exp((-feld[i] + a0) * a1)));
      }
    }

    /// <summary> ÜBERLADUNG
    /// 4.3a Intensitätsänderung mittels Sigmoidfunktion.
    /// </summary>
    /// <param name="feld"></param> Feld
    /// <param name="a0"></param> Funktionswendepunkt in horizontaler Richtung
    /// <param name="a1"></param> Streckung und Stauchung der Funktion
    public void sigmoidFormung(ref float[,,] array, double a0, double a1) {
      for (int c = 0; c < array.GetLength(0); c++)
        for (var y = 0; y < array.GetLength(1); y++)
          for (var x = 0; x < array.GetLength(2); x++)
            array[c, y, x] = (float)(1 / (1 + Math.Exp((-array[c, y, x] + a0) * a1)));
    }

    /// <summary> ÜBERLADUNG
    /// 4.3a Intensitätsänderung mittels Sigmoidfunktion.
    /// </summary>
    /// <param name="feld"></param> Feld
    /// <param name="a0"></param> Funktionswendepunkt in horizontaler Richtung
    /// <param name="a1"></param> Streckung und Stauchung der Funktion
    /// <param name="canals"></param> Ausgewählte Kanäle als Feld
    public void sigmoidFormung(ref float[,,] array, double a0, double a1, int[] canals) {
      for (int c = 0; c < canals.GetLength(0); c++)
        for (var y = 0; y < array.GetLength(1); y++)
          for (var x = 0; x < array.GetLength(2); x++)
            array[canals[c], y, x] = (float)(1 / (1 + Math.Exp((-array[c, y, x] + a0) * a1)));
    }


    /// <summary>
    /// Liefert kanalbezogene Intervallgrenzen [0, 255]
    /// </summary>
    /// <param name="array"></param>
    /// <returns></returns>
    public float[,] getMinMax(float[,,] array) {
      float[,] minMax = { { 255, 255, 255 }, { 0, 0, 0 } };

      //..............

      return minMax;
    }
    #endregion

    #region 5. Falschfarben
    /// <summary>
    /// 5.1 Transformiert ein Grauwertbild in ein Falschfarbenbild
    /// </summary>
    /// <param name="feld"></param> Feldreferenz: Grauwertfeld -> Falschfarbenfeld
    /// <param name="perioden"></param> Perioden bezogen auf den Intervall [0,1]
    /// <param name="anstieg"></param> Anstieg der linearen Funktion
    public void toFalseColors(float[] feld, double perioden = 3, double anstieg = 0.5) {
      int ebenenPixel = (int)(feld.Length / 3);
      for (int i = 0; i < ebenenPixel; i++) {
        // Aus grau wird grau, aus realen Farben wird grau
        float gray = (feld[i] + feld[i + ebenenPixel] + feld[i + 2 * ebenenPixel]) / 3;
        // Perioden gilt für Intervall [0, 2* pi], daher Multiplikation mit 2
        float phi = (float)(2 * perioden * gray * Math.PI);       // Winkel [0 .. pi]
        float zunahme = (float)(anstieg * gray);              // Linearanteil
        float Dphi = (float)(2 * Math.PI / 3); // Phasenverschiebung 2/3 pi
        float A = (float)((1 - anstieg) / 2);  // Konstante
        feld[i] = (float)(A * Math.Sin(phi) + zunahme + A);    //R
        feld[i + ebenenPixel] = (float)(A * Math.Sin(phi + Dphi) + zunahme + A);     //G
        feld[i + 2 * ebenenPixel] = (float)(A * Math.Sin(phi + Dphi + Dphi) + zunahme + A); // B
      }
    }

    /// <summary>
    /// 5.2 Transformiert ein Grauwertbild in ein Falschfarbenbild
    /// </summary>
    /// <param name="array"></param> dreidimensionalöes Grauwertarray -> Falschfarbenarray
    /// <param name="perioden"></param> Perioden bezogen auf den Intervall [0,1]
    /// <param name="anstieg"></param> Mittlerer Intensitätsanstieg im Falschfarbenbild
    public void toFalseColors(float[,,] array, double perioden = 3, double anstieg = 0.5) {
      for (int y = 0; y < array.GetLength(1); y++)
        for (int x = 0; x < array.GetLength(2); x++) {
          // Aus grau wird grau, aus realen Farben wird grau
          float gray = (array[0, y, x] + array[1, y, x] + array[2, y, x]) / 3;
          // Perioden gilt für Intervall [0, 2* pi], daher Multiplikation mit 2
          float phi = (float)(2 * perioden * gray * Math.PI);       // Winkel [0 .. pi]
          float zunahme = (float)(anstieg * gray);              // Linearanteil
          float Dphi = (float)(2 * Math.PI / 3); // Phasenverschiebung 2/3 pi
          float A = (float)((1 - anstieg) / 2);  // Konstante
          array[0, y, x] = (float)(A * Math.Sin(phi) + zunahme + A);    //R
          array[1, y, x] = (float)(A * Math.Sin(phi + Dphi) + zunahme + A);     //G
          array[2, y, x] = (float)(A * Math.Sin(phi + Dphi + Dphi) + zunahme + A); // B
        }
    }

    /// <summary>
    /// 5.3 Transformiert ein Grauwertbild in ein Falschfarbenbild
    /// </summary>
    /// <param name="feld"></param> Grauwertfeld -> Falschfarbenfeld
    /// <param name="perioden"></param> Perioden bezogen auf den Intervall [0,255]
    /// <param name="anstieg"></param> Grauwertanstieg
    public void toFalseColors(byte[] feld, double perioden = 3, double anstieg = 0.5) {
      int ebenenPixel = (int)(feld.Length / 3);
      for (int i = 0; i < ebenenPixel; i++) {
        // Aus grau wird grau, aus realen Farben wird grau
        byte gray = (byte)((feld[i] + feld[i + ebenenPixel] + feld[i + 2 * ebenenPixel]) / 3);
        float phi = (float)(perioden * gray * Math.PI / 127);       // Winkel [0 .. pi]
        byte zunahme = (byte)(anstieg * gray);              // Linearanteil
        float Dphi = (float)(2 * Math.PI / 3); // Phasenverschiebung 2/3 pi
        byte A = (byte)(255 / 2 * (1 - anstieg));  // Konstante
        feld[i] = (byte)(A * Math.Sin(phi) + zunahme + A);    //R
        feld[i + ebenenPixel] = (byte)(A * Math.Sin(phi + Dphi) + zunahme + A);     //G
        feld[i + 2 * ebenenPixel] = (byte)(A * Math.Sin(phi + Dphi + Dphi) + zunahme + A); // B
      }
    }

    /// <summary>
    /// 5.4 Transformiert ein Grauwertbild in ein Falschfarbenbild
    /// </summary>
    /// <param name="array"></param> dreidimensionales Array, Intervall der Elemente [0,255]
    /// <param name="anstieg"></param> Mittlerer Intensitätsanstieg im Falschfarbenbild
    public void toFalseColors(byte[,,] array, double perioden = 3, double anstieg = 0.5) {
      for (int y = 0; y < array.GetLength(1); y++)
        for (int x = 0; x < array.GetLength(2); x++) {
          // Aus grau wird grau, aus realen Farben wird grau
          byte gray = (byte)((array[0, y, x] + array[1, y, x] + array[2, y, x]) / 3);
          float phi = (float)(perioden * gray * Math.PI / 127);       // Winkel [0 .. pi]
          byte zunahme = (byte)(anstieg * gray);              // Linearanteil
          float Dphi = (float)(2 * Math.PI / 3); // Phasenverschiebung 2/3 pi
          byte A = (byte)(127 * (1 - anstieg));  // Konstante
          array[0, y, x] = (byte)(A * Math.Sin(phi) + zunahme + A);    //R
          array[1, y, x] = (byte)(A * Math.Sin(phi + Dphi) + zunahme + A);     //G
          array[2, y, x] = (byte)(A * Math.Sin(phi + Dphi + Dphi) + zunahme + A); // B
        }
    }
    #endregion

    #region 6. logische Verknüpfungen

    /// <summary>
    /// 6.1 Oder-Verknüpfung der Bildpunkte der Kanäle untereinander.
    /// </summary>
    /// <param name="array"></param> Referenz eines dreidimensionalen Bildarrays
    /// <param name="I"></param> zu verknüpfende Intensität 
    public void orCanals(ref float[,,] array, float I = 0) {
      for (int c = 0; c < array.GetLength(0); c++)
        for (var y = 0; y < array.GetLength(1); y++)
          for (var x = 0; x < array.GetLength(2); x++)
            if (array[c, y, x] == I)
              for (int c1 = 0; c1 < array.GetLength(0); c1++)
                array[c1, y, x] = I;
    }

    #endregion

  }
}
