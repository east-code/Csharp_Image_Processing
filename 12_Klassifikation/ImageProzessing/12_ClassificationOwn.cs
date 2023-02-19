/* Klasse ClassificationOwn: Die Klassifizierung ordnet eine Bedeutung anders gesagt einen Begriff den abgebildeten Objekten zu.
 * Verfasser: Horst-Christian Heinke
 * Datum: 19.12.2022
 * Letzte Änderung: 15.01.2023
 * Germany, Sachsen-Anhalt, Magdeburg
 * */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO; // Für File
using System.Text;
using System.Windows.Forms;
using ARRAY = ImageProcessing.ArrayOwn;

namespace ImageProcessing {
  internal class ClassificationOwn {
    #region 1. Attribute, Instanzvariablen
    /// <summary>
    ///  1.1 Matrix mit Daten [k,j,i]
    ///  k - Klasse
    ///  j - Eigenschaft
    ///  i - Element
    /// </summary>
    float[,,] M = new float[4, 2, 12];
    /// <summary>
    /// 1.2 Erwartungswerte Ex, Ey[k,j]
    /// </summary>
    float[,] EM = new float[4, 2];
    /// <summary>
    /// 1.3 Standardabweichungen Sx, Sy[k,j]
    /// </summary>
    float[,] SM = new float[4, 2];
    ///
    /// 1.5 Konfidenzintervalle ux, uy [k,j]
    /// </summary>
    float[,] uM = new float[4, 2];

    /// <summary>
    /// 1.6 Klassenbezogene Konfidenzgrenzen [k, g]
    /// k - Klasse
    /// j - Koordinate für 0=x- und 1=y-Koordinate
    /// g - Winkelschritte [0°,360°[
    /// </summary>
    float[,,] fu = new float[4, 2, 360];

    /// <summary>
    /// 1.7 Zweidimensionale normalverteilte Häufigkeitsverteilung (Dichtefunktion)
    /// k - Klasse 0,1,2,3, k=0 -> unbestimmt
    /// y - Ordinate (A)
    /// x - Abszisse (U)
    /// </summary>
    float[,,] hyx;
    #endregion

    #region 2. Konstruktoren
    /// <summary>
    /// 1. Konstruktor
    /// </summary>
    /// <param name="M"></param> Datenmatrix [k,j,i]
    /// <param name="t"></param> t-Wert zur Konfidenzintervallabschätzung
    public ClassificationOwn(float[,,] M, double t = 2.201) {
      this.M = M;
    }
    /// <summary>
    ///  2. Konstruktor
    /// </summary>
    public ClassificationOwn() {
      this.M = new float[4, 2, 12];
    }

    #endregion

    #region 3. Eigenschaftsmethoden Getter und Setter
    /// <summary>
    /// 3.1 Getter und Setter für Daten-Array
    /// </summary>
    public float[,,] isM {
      get { return this.M; }
      set { this.M = value; }
    }
    /// <summary>
    /// 3.2 Getter für Serienmittelwerte
    /// </summary>
    public float[,] getEM {
      get { return this.EM; }
    }
    /// <summary>
    /// 3.3 Getter für Konfidenzbereichsgrenzen
    /// </summary>
    public float[,,] getfu {
      get { return this.fu; }
    }
    #endregion

    #region 4. Methoden zur Statistik
    /// <summary>
    /// 4.1 Liefert die Mittelwerte (Dimension=2)
    /// - Verwendet ausschließlich Instanzvariablen
    /// </summary>
    public void average() {
      for (var k = 1; k < M.GetLength(0); k++)
        for (var j = 0; j < M.GetLength(1); j++) {
          EM[k, j] = 0;
          for (int i = 1; i < M.GetLength(2); i++)
            EM[k, j] += M[k, j, i];
          EM[k, j] /= (M.GetLength(2) - 1);
        }
    }

    /// <summary>
    /// 4.2 Liefert die Standardabweichungen und Konfidenzintervalle
    /// (Dimension = 2)
    /// - Verwendet Instanzvariablen
    /// </summary>
    /// <param name="t"></param>
    public void standard(double t = 2.201) {
      for (var k = 1; k < M.GetLength(0); k++)
        for (var j = 0; j < M.GetLength(1); j++) {
          SM[k, j] = 0;
          for (int i = 1; i < M.GetLength(2); i++)
            SM[k, j] += (M[k, j, i] - EM[k, j]) * (M[k, j, i] - EM[k, j]);
          SM[k, j] = (float)Math.Sqrt(SM[k, j] / (M.GetLength(2) - 2));
          uM[k, j] = (float)(SM[k, j] * t);
        }
    }

    /// <summary>
    /// 4.3 Liefert die Konfidenzgrenzen der Klassen für einen vorgegeben
    /// t-Wert. Es handelt sich dabei um eine Isodichte, 
    /// eine Funktion mit gleichen Dichtewerten einer Klasse.
    /// Für den zweidimensionalen Fall ist diese Funktion eine Ellipse.
    /// </summary>
    /// <param name="t"></param> Studentverteilung
    /// <returns>fh</returns> Dichtefunktionswerte an den Konfidenzgrenzen
    /// Isoverteilungsdichte
    public double[] confidenceBounderyFunction(double t = 2.201) {
      double[] fh = new double[M.GetLength(0)];
      // Klassenschleife
      for (var k = 1; k < M.GetLength(0); k++) {
        fh[k] = 0;
        // Winkelschleife
        for (var g = 0; g < 360; g++) {
          double rad = g * Math.PI / 180;
          fu[k, 0, g] = (float)(EM[k, 0] + SM[k, 0] * t * (double)Math.Cos(rad));
          fu[k, 1, g] = (float)(EM[k, 1] + SM[k, 1] * t * (double)Math.Sin(rad));
          fh[k] += (double)pointOfTwoDimensionalDensity(k, fu[k, 1, g], fu[k, 0, g]);
        }
        // Eigentlich sind alle fh-Werte gleich, bis auf numerische Abweichungen.
        fh[k] /= 360;
      }
      return fh;
    }
    #endregion

    #region 5. Dichtefunktion


    /// <summary>
    /// 5.1 Liefert die Dichtefunktion der Teileklasse im angegebenen Intervall.
    /// </summary>
    /// <param name="interval"></param> Rechteck, Diagonalpunkte definieren das Intervall
    /// Left => X0, Top => Y0, Width => X1-X0, Hight => Y1-Y0
    /// <param name="nrClasses"></param> Teileklasse 0,1,2,3  (k<=3)
    /// <param name="nrPoints"></param> Anzahl der Stützpunkte in x und in y
    /// <returns>hyx</returns> Dichte-Funktionswerte des angegebenen Intervalls [c, y, x]
    public float[,,] twoDimensionalDensity(Rectangle interval, int nrClasses, int nrPoints = 60) {
      Point scale = new Point(nrPoints / interval.Width, nrPoints / interval.Height);
      hyx = new float[nrClasses, nrPoints, nrPoints];
      // Kanäle
      for (var k = 1; k <= nrClasses; k++) {
        for (var x = 0; x < nrPoints; x++)
          for (var y = 0; y < nrPoints; y++)
            // Liefert Dichtewert für einen Punkt
            hyx[k - 1, y, x] = (float)pointOfTwoDimensionalDensity(k, (float)y * interval.Height / nrPoints + interval.Top, (float)x * interval.Width / nrPoints + interval.Left);
      }
      return hyx;
    }

    /// <summary> ÜBERLADUNG
    /// 5.1b Liefert die Dichtefunktion der Teileklasse im angegebenen Intervall.
    /// </summary>
    /// <param name="interval"></param> Rechteck, Diagonalpunkte definieren das Intervall
    /// Left => X0, Top => Y0, Width => X1-X0, Hight => Y1-Y0
    /// <param name="nrClasses"></param> Teileklasse 0,1,2,3  (k<=3)
    /// <param name="scale"></param> Skaliert die Bildausgabe und definiert Zwischenpunkte
    /// <returns>hyx</returns> Dichte-Funktionswerte des angegebenen Intervalls [c, y, x]
    public float[,,] twoDimensionalDensity(Rectangle interval, int nrClasses, Point scale) {
      hyx = new float[nrClasses, interval.Height * scale.Y, interval.Width * scale.X];
      // Kanäle
      for (var k = 1; k <= nrClasses; k++) {
        for (var x = 0; x < interval.Width * scale.X; x++)
          for (var y = 0; y < interval.Height * scale.Y; y++)
            // Liefert Dichtewert für einen Punkt
            hyx[k - 1, y, x] = (float)pointOfTwoDimensionalDensity(k, (float)y / scale.Y + interval.Top, (float)x / scale.X + interval.Left);
      }
      return hyx;
    }

    /// <summary>
    /// 5.2 Liefert einen Funktionswert der zweidimensionalen Dichtefunktion
    /// </summary>
    /// <param name="x"></param> Abszissenwert
    /// <param name="y"></param> Ordinatenwert
    /// <param name="k"></param> Teileklasse
    /// <returns></returns> Dichtefunktionswert
    public double pointOfTwoDimensionalDensity(int k, double y, double x) {
      // Konstanten
      double Kx = 1 / SM[k, 0] / Math.Sqrt(2 * Math.PI);
      double Ky = 1 / SM[k, 1] / Math.Sqrt(2 * Math.PI);
      // Exponenten
      double Ex = (x - EM[k, 0]) / SM[k, 0] / 2;
      double Ey = (y - EM[k, 1]) / SM[k, 1] / 2;
      return Kx * Math.Exp(-Ex * Ex) * Ky * Math.Exp(-Ey * Ey);
    }

    /// <summary>
    /// 5.3 Bereichsgrenzen, Grenzen zwischen zwei Klassen
    /// </summary>
    /// <param name="array"></param> Bildarray der Verteilungsdichte
    /// [c=0: Regionsgrenze, c=1: Region der Klasse c1, c=2: Region der Klasse c2]
    /// <param name="c1"></param> Bildkanal 1
    /// <param name="c2"></param> Bildkanal 2
    /// <returns>arrayBorder</returns> Bildarray, Regionsgrenze im Kanal 0
    public float[,,] bordersOfClasses(float[,,] array, int c1 = 1, int c2 = 2, double dh = 0.001, bool add = true) {
      float[,,] arrayBorder = new float[array.GetLength(0), array.GetLength(1), array.GetLength(2)];
      ARRAY.setArrayChannelValue(ref arrayBorder, 0);
      for (var y = 0; y < array.GetLength(1); y++)
        for (int x = 0; x < array.GetLength(2); x++)
          // Vergleich
          if (Math.Abs(array[c1, y, x] - array[c2, y, x]) < dh)
            arrayBorder[0, y, x] = 1;
      if (add) {
        // Oder-Verknüpfung zweier Arrays gleicher Dimensionen.
        ARRAY.arrayAddArray(ref arrayBorder, ref array);
      }
      return arrayBorder;
    }
    #endregion

    #region 6. Dateien, Daten formatieren, speichern und laden
    /// <summary>
    /// 6.1 Ergänzt eine vorhandenen Datei um Trainingsdaten.
    /// </summary>
    /// <param name="u_Ep"></param> Umfang von 11 Stichproben
    /// <param name="a_Ap"></param> Fläche von 11 Stichproben
    /// <param name="classname"></param> Klassennummer und Name
    public void appendTrainData(List<float> u_Ep, List<float> a_Ap, string classname) {
      SaveFileDialog appendFileDialog = new SaveFileDialog();
      appendFileDialog.CheckFileExists = false;
      appendFileDialog.CheckPathExists = false;
      appendFileDialog.OverwritePrompt = false;
      appendFileDialog.Filter = "Daten als Text (*.txt)|*.txt|Alle Dateien (*.*)|*.*";
      appendFileDialog.Title = "Trainingsdaten in eine Txt-Datei schreiben";
      // Standard-Datei-Name
      appendFileDialog.FileName = "Trainingsdaten";
      // Standard-Datei-Typ
      appendFileDialog.DefaultExt = "txt";
      if (appendFileDialog.ShowDialog() == DialogResult.OK) {
        File.AppendAllText(appendFileDialog.FileName, String.Format("\n{0:s} | {1:s} | {2,7:s} --> {3,10:s}\n", "k", "j", "M[k,j,i]", classname));
        saveDataAsTxt(appendFileDialog.FileName, u_Ep, classname, "0. Umfang");
        saveDataAsTxt(appendFileDialog.FileName, a_Ap, classname, "1. Fläche");
      }
    }

    /// <summary>
    /// 6.2 Speichert Daten einer Liste als Textdatei mit dem senkreten Strich "|"
    /// zum Trennen der einzelnen Werte.
    /// </summary>
    /// <param name="path"></param> Sub-Pfad
    /// <param name="dataList"></param> Daten einer Liste
    /// <param name="classname"></param> Teileklasse {Schrauben, Muttern, ...}
    /// <param name="attribut"></param> Eigenschaft {Umfang, Fläche, ...}
    public void saveDataAsTxt(string path, List<float> dataList, string classname = "0", string attribut = "0") {
      // Klassenindex an den Anfang hinzufügen
      dataList.Insert(0, classname[0] - 48);
      // Eigenschaftsindex auf den vorhandenen Dummy schreiben.
      dataList[1] = attribut[0] - 48;
      // Trennzeichen zwischen den Werten
      string seperator = " | ";
      // Instanz zum Stringaufbau
      StringBuilder sb = new StringBuilder();
      // Stellt den Ausgabestring zusammen
      sb.Append(string.Join(seperator, dataList));
      File.AppendAllText(path, sb.ToString());
      File.AppendAllText(path, "\r\n");
    }

    /// <summary>
    /// 6.3 Trainingsdaten zur Ausgabe formatieren
    /// </summary>
    /// <param name="array"></param> Trainingsdaten
    /// <returns>text</returns> Trainingsdaten als Text
    /// k=0 - Reserviert für nicht zuordenbare Segmente (Klassen)
    /// i=0 - Reserviert (Messwerte)
    public string arrayToText(float[,,] array) {
      string text = "";
      text += String.Format("\n3D-Array m x n x o: {0:d} Klassen k x {1:d} Eigenschaften j x {2:d} Elemente i", array.GetLength(0) - 1, array.GetLength(1), array.GetLength(2) - 1);
      text += String.Format("k=0 ist reserviert für nicht zuordenbare Segmente und i=0 wird nicht verwendet.");
      text += String.Format("\n {0:s} | {1:s} | {2,7:s} --> {3,10:s}", "k", "j", "M[k,j,i]", ". . .");
      for (int k = 0; k < array.GetLength(0); k++)
        for (int j = 0; j < array.GetLength(1); j++) {
          text += "\n";
          text += String.Format(" {0:d} | {1:d} | ", k, j);
          for (int i = 0; i < array.GetLength(2); i++) {
            text += String.Format(" {0,7:F2} ", array[k, j, i]);
          }
        }
      return text + "\n";
    }

    /// <summary>
    /// 6.4 Das Modell enthält die statistischen Kenngrößen, die aus dem
    /// Trainingsdaten berechnet wurden.
    /// Trainiert wurden k=1: Schrauben, k=2: Muttern, K=3: Scheiben
    /// </summary>
    /// <param name="path"></param> Speicherpfad/Speicherdatei
    /// <returns>model</returns> Statistische Kenngrößen der Normalverteilung
    public string saveModelAsTxt(string path = @".\model_SchraubenMutternScheiben.txt") {
      // Überschrift
      string model = "- Statistische Kenngrößen der  Klassen k mit den Eigenschaften j";
      File.Delete(path);
      // Tabellenkopf
      model += String.Format("\n {0:s} | {1:s} | {2,10:s} | {3,10:s} | {4,10:s} \n", "k", "j", "EM[k, j]", "SM[k, j]", "uM[k, j]");
      // Wertetabelle
      for (var k = 0; k < EM.GetLength(0); k++) {
        for (var j = 0; j < EM.GetLength(1); j++) {
          model += String.Format(" {0:d} | {1:d} | {2,10:F3} | {3,10:F5} | {4,10:F5} \n", k, j, EM[k, j], SM[k, j], uM[k, j]);
        }
      }
      File.AppendAllText(path, model);
      return model;
    }

    /// <summary>
    /// 6.5 Lädt Trainins-Daten mehrerer Teileklassen
    /// </summary>
    /// <param name="path"></param> Datenspeicherort
    public string loadDataFromTxt(string path) {
      string noData = "Zeilen: ";
      string[] text = System.IO.File.ReadAllLines(path);
      char[] chars = { '\r', '\n', ' ' };
      for (var l = 0; l < text.Length; l++) {
        string[] line = text[l].Split('|');
        try {
          int k = Convert.ToInt32(line[0]);
          int j = Convert.ToInt32(line[1]);
          for (int i = 1; i < line.GetLength(0) - 1; i++)
            M[k, j, i] = (float)Convert.ToDouble(line[i + 1]);
        }
        catch (Exception) { noData += l.ToString() + ", "; };
      }
      return noData;
    }

    /// <summary>
    /// 6.6 Lädt trainierte Modelldaten
    /// k- Klassenindex
    /// j - Eigenschaftsindex
    /// </summary>
    /// <param name="path"></param> Speicherort
    /// EM, SM - Instanzvariablen
    public void loadModelFromTxt(string path = @".\model_SchraubenMutternScheiben.txt") {
      string[] text = System.IO.File.ReadAllLines(path);
      char[] chars = { '\r', '\n', ' ' };
      string[] line;
      for (var l = 0; l < text.Length; l++) {
        line = text[l].Split('|');
        // Handelt es sich um eine Datenzeile?
        if (l > 1) {
          int k = Convert.ToInt32(line[0]);
          int j = Convert.ToInt32(line[1]);
          // Schätzwert des Mittelwerts, Erwartungswert
          EM[k, j] = (float)Convert.ToDouble(line[2]);
          // Schätzwert der Standardabweichung
          SM[k, j] = (float)Convert.ToDouble(line[3]);
          uM[k, j] = (float)Convert.ToDouble(line[4]);
        }
      }
    }
    #endregion

    #region 7. Prognose, Test und Zuordnung
    /// <summary>
    /// 7.1 Liefert für alle Stichproben die Wahrscheinlichkeitswerte
    /// in Bezug auf ihre Klassenzugehörigkeit.
    /// i - Index einer Stichprobe
    /// k - Index der Klasse
    /// </summary>
    /// <param name="y"></param> Ordinatenwert
    /// <param name="x"></param> Abszissenwert
    /// <returns>P</returns> Wahrscheinlichkeitsmatrix [k,i]
    public double[,] whatProbabilities(List<float> y, List<float> x) {
      double[,] h = new double[EM.GetLength(0), y.Count];
      double[] hSum = new double[y.Count];
      for (var i = 1; i < y.Count; i++) {
        hSum[i] = 0;
        for (var k = 0; k < EM.GetLength(0); k++) {
          h[k, i] = (double)pointOfTwoDimensionalDensity(k, y[i], x[i]);
          hSum[i] += (double)h[k, i];
        }
      }
      double[,] P = new double[EM.GetLength(0), y.Count];
      for (var k = 0; k < EM.GetLength(0); k++)
        for (var i = 1; i < y.Count; i++)
          P[k, i] = (double)(h[k, i] / hSum[i]);
      return P;
    }

    /// <summary>
    /// 7.2 Liefert Wert der Dichtefunktion für alle 
    /// Stichproben und alle bekannten Teileklassen.
    /// </summary>
    /// <param name="y"></param> x-Eigenschaften (U)
    /// <param name="x"></param> y-Eigenschaften (A)
    /// <returns>h</returns> Dichtewerte h=f(y,x)
    public double[,] actualDensity(List<float> y, List<float> x) {
      double[,] h = new double[EM.GetLength(0), y.Count];
      double[] hSum = new double[y.Count];
      for (var i = 1; i < y.Count; i++) {
        for (var k = 0; k < EM.GetLength(0); k++) {
          // Dichtefunktionsaufruf
          h[k, i] = (double)pointOfTwoDimensionalDensity(k, y[i], x[i]);
        }
      }
      return h;
    }

    /// <summary>
    /// 7.3 Liefert die Indizes der Maximalwerte der Elemente 
    /// des Kanal 0 eines zweidimensionalen Feldes.
    /// </summary>
    /// <param name="D"></param> Dichtematrix aller Klassen und Segmente 
    /// <returns>kMax</returns> Indizes der Maximalwerte
    public int[] indexOfMax(double[,] D) {
      int[] kMax = new int[D.GetLength(1)];
      for (var i = 0; i < D.GetLength(1); i++) {
        double max = 0;
        for (var k = 0; k < D.GetLength(0); k++)
          if (D[k, i] > max) {
            max = D[k, i];
            kMax[i] = k;
          }
      }
      return kMax;
    }

    /// <summary>
    /// 7.4 Stellt Ausgabetext zusammen
    /// </summary>
    /// <param name="D"></param> Verteilungsdichtewerte aller Klassen
    /// <param name="kMax"></param> Indexfeld der Maximalwerte
    /// <param name="title"></param> Titel
    /// <returns>text</returns> Formatierter Ausgabetext
    /// der Teileklassenzuordnung,der Dichtewerte, der Regionsgrenzen
    public string densityToText(double[,] D, int[] kMax, double[] fh, string title = "Prognose") {
      string[] selection = { "unbekannt", "0. Schraube", "1. Mutter", "2. Scheibe" };
      string text = String.Format("\n{0:s} k={1:d} x i={2:d}\n", title, D.GetLength(0), D.GetLength(1));
      text += String.Format("{0,10:s} | {1,10:s} | {2,20:s} | {3,20:s} | {4,20:s}\n", "Probe i", "k_max", "D(k_max)", "fh(u)[k_max]", "Segment");
      for (int i = 1; i < D.GetLength(1); i++) {
        int k = 0;
        if (D[kMax[i], i] > fh[kMax[i]])
          k = kMax[i];
        text += String.Format(" {0,10:d} | {1,10:d} | {2,20:F18} | {3,20:F18} | {4,20:s}\n", i, kMax[i], D[kMax[i], i], fh[kMax[i]], selection[k]);
      }
      return text;
    }




    /// <summary>
    /// 7.5 Zuordnung der Segmente einer Klasse.
    /// Liefert eine Liste mit der Klassenzugehörigkeit in String-Format.
    /// </summary>
    /// <param name="D"></param> // Dichtematrix aller Klassen k und Segmente id
    /// <param name="kMax"></param> // Index der Bestwerte der Segmente
    /// <param name="fh"></param> Werte der Verteilungsdichte an der Konfidenzgrenze
    /// für jede Klasse
    /// <returns>segmentClasses</returns> Klassenzugehörigkeit der Segmente
    public List<String> ofSegments(double[,] D, int[] kMax, double[] fh) {
      string[] selection = { "(unbekannt)", "(k1:Schraube)", "(k2:Mutter)", "(k3:Scheibe)" };
      List<string> segmentClasses = new List<string>();
      segmentClasses.Add("Hintergrund");
      for (int id = 1; id < D.GetLength(1); id++) {
        int k = 0;
        if (D[kMax[id], id] > fh[kMax[id]])
          k = kMax[id];
        segmentClasses.Add(selection[k]);
      }
      return segmentClasses;
    }

    /// <summary> ÜBERLADUNG
    /// 7.5b Zuordnung der Segmente einer Klasse.
    /// Liefert ein Array zur Klassenzugehörigkeit, mit den
    /// Eigenschaftswerten und der Id's der Segmente.
    /// </summary>
    /// <param name="D"></param> // Dichtematrix aller Klassen k und Segmente id
    /// <param name="kMax"></param> // Index der Bestwerte der Segmente
    /// <param name="fh"></param> Werte der Verteilungsdichte an der Konfidenzgrenze
    /// für jede Klasse
    /// <returns>Mt</returns> Klassenzugehörigkeit der Segmente
    /// k - Klassenzugehörigkeit {1,2,3} 0 -unbekannt
    /// j - Eigenschaft {0 -Abszisse, Umfang | 1 - Ordinate, Fläche}
    /// id - Testsegment {1,2,3,4,5,...} | 0 - Hintergrund
    public float[,,] ofSegments(double[,] D, int[] kMax, double[] fh, List<float> u_Ep, List<float> a_Ap) {
      float[,,] Mt = new float[D.GetLength(0) + 1, 2, D.GetLength(1)];
      // segmentClasses.Add("Hintergrund");
      for (int id = 1; id < D.GetLength(1); id++) {
        int k = 0;
        if (D[kMax[id], id] > fh[kMax[id]])
          k = kMax[id];
        Mt[k, 0, id] = u_Ep[id];
        Mt[k, 1, id] = a_Ap[id];
      }
      return Mt;
    }

    /// <summary> ÜBERLADUNG
    /// 7.5c Zuordnung der Segmente einer Klasse.
    /// Liefert ein Array zur Klassenzugehörigkeit, mit den
    /// Eigenschaftswerten und der Id's der Segmente.
    /// Liefert per Ausgabeparameter eine Liste mit der Klassenzugehörigkeit in String-Format.
    /// </summary>
    /// <param name="D"></param> // Dichtematrix aller Klassen k und Segmente id
    /// <param name="kMax"></param> // Index der Bestwerte der Segmente
    /// <param name="fh"></param> Werte der Verteilungsdichte an der Konfidenzgrenze
    /// für jede Klasse
    /// <returns>Mt</returns> Klassenzugehörigkeit der Segmente
    /// k - Klassenzugehörigkeit {1,2,3} 0 -unbekannt
    /// j - Eigenschaft {0 -Abszisse, Umfang | 1 - Ordinate, Fläche}
    /// id - Testsegment {1,2,3,4,5,...} | 0 - Hintergrund
    public float[,,] ofSegments(double[,] D, int[] kMax, double[] fh, List<float> u_Ep, List<float> a_Ap, out List<string> segmentClasses) {
      segmentClasses = new List<string>();
      segmentClasses.Add("Hintergrund");
      float[,,] Mt = new float[D.GetLength(0) + 1, 2, D.GetLength(1)];
      Mt[0, 0, 0] = (float)0.0;
      string[] selection = { "(k0:unbekannt)", "(k1:Schraube)", "(k2:Mutter)", "(k3:Scheibe)" };
      // segmentClasses.Add("Hintergrund");
      for (int id = 1; id < D.GetLength(1); id++) {
        int k = 0;
        if (D[kMax[id], id] > fh[kMax[id]])
          k = kMax[id];
        Mt[k, 0, id] = u_Ep[id];
        Mt[k, 1, id] = a_Ap[id];
        segmentClasses.Add(selection[k]);
      }
      return Mt;
    }


    /// <summary>
    /// 7.6 Vorhersagestruktur
    /// </summary>
    public struct prediction {
      public List<float[]> properties; // Liste mit den Eigenschaften der Stichproben
      public List<int> k; // Prognostizierte Teileklassen der Stichproben
      public List<string> kName; //Prognostizierte Teileklassen-Namen der Stichproben
    }

    /// <summary>
    /// 7.7 Aufbereitung und Einordnung der Prognosedaten
    /// </summary>
    /// <param name="D"></param> // Dichtematrix aller Klassen k und Segmente id
    /// <param name="kMax"></param> // Index der Bestwerte der Segmente
    /// <param name="fh"></param> Dichtewerte an der Konfidenzgrenze für alle Klassen
    /// <param name="u_Ep"></param> Eigenschaften: Umfang
    /// <param name="a_Ap"></param> Eigenschaften: Fläche
    /// <returns>part</returns> Liste der Teile mit prognostizierten Klassen und Eigenschaften.
    public prediction segments(double[,] D, int[] kMax, double[] fh, List<float> u_Ep, List<float> a_Ap, int grupp = 0) {
      prediction part = new prediction();
      part.properties = new List<float[]>();
      part.k = new List<int>();
      part.kName = new List<string>();

      List<string[]> select = new List<string[]>();
      String[] selection = { "(k0:unbekannt)", "(k1:Schraube)", "(k2:Mutter)", "(k3:Scheibe)" };
      select.Add(selection);
      String[] selection2 = { "(k0:unbekannt)", "(k1:Bohne)", "(k2:Mais)", "(k3:Sonnenbl)" };
      select.Add(selection2);

      // id=0: Reserviert für Hintergrund
      part.properties.Add(new float[] { 0, 0 });
      part.k.Add(0);
      part.kName.Add("Hintergrund");
      // Für alle Segmente 1,2, ..., m
      for (int id = 1; id < D.GetLength(1); id++) {
        int k = 0;
        // Liegt Bestwert kMax in der Konfidenzregion?
        if (D[kMax[id], id] > fh[kMax[id]])
          k = kMax[id];
        // Liste erweitern
        part.properties.Add(new float[] { u_Ep[id], a_Ap[id] });
        part.k.Add(k);
        //part.kName.Add(selection[k]);
        part.kName.Add(select[grupp][k]);
      }
      return part;
    }

    /// <summary>
    /// Textbereitstellung der Prognoseergebnisse
    /// </summary>
    /// <param name="properties"></param>
    /// <param name="kMax"></param>
    /// <param name="kName"></param>
    /// <param name="title"></param>
    /// <returns></returns>
    public string predictionToText(List<float[]> properties, List<int> kMax, List<string> kName, string title = "Prognose") {
      string text = String.Format("\n{0:s}, Anzahl der Teile: {1:d}\n", title, kMax.Count - 1);
      text += String.Format("{0,8:s} | {1,6:s} | {2,6:s} | {3,6:s} | {4,18:s}\n", "Id", "Klasse", "Umfang", "Fläche", "Segment ");
      for (int i = 1; i < properties.Count; i++) {
        text += String.Format("{0,8:d} | {1,6:d} | {2,6:F1} | {3,6:F0} | {4,18:s}\n", i, kMax[i], properties[i][0], properties[i][1], kName[i]);
      }
      return text;
    }

    #endregion


  }
}
