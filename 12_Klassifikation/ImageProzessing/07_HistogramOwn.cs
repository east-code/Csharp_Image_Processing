/* Klasse HistogramOwn: Erstellt für Bitmaps und Bildarrays Histogramme.
 * Verfasser: Horst-Christian Heinke
 * Datum: 19.05.2022
 * Letzte Änderung: 15.01.2023
 * Germany, Sachsen-Anhalt, Magdeburg
 * */

using System; // Für Klasse Type
using System.Collections.Generic;
using System.Windows.Forms.DataVisualization.Charting;
using form = System.Windows.Forms;// -> Komponenten

namespace ImageProcessing {
  internal class HistogramOwn {

    #region 1: Datenstrukturen, Instanzvariablen und Konstruktor

    // 1.1 Darstellung des Diagramms
    public form.DataVisualization.Charting.Chart chart;

    /// <summary>
    /// 1.2 Konstruktor übernimmt Chart-Komponente
    /// </summary>
    /// <param name="chart"></param> Diagramm
    public HistogramOwn(form.DataVisualization.Charting.Chart chart) {
      this.chart = chart;
    }

    /// <summary>
    /// 1.1 Datenstruktur des RGB-Histogramms
    /// </summary>
    public struct Histogram {
      public int r;
      public int g;
      public int b;
    }
    #endregion

    #region 2: Grauwert-Histogramme

    /// <summary>
    /// 2.1 Liefert das Grauwerthistogramm eines dreidimensionalen
    /// Arrays mit dem Elementetyp "byte". Die drei Kanäle
    /// werden als Summe abgebildet.
    /// </summary>
    /// <param name="array"></param> Dreidimensionales Bildarray
    /// <returns></returns> Grauwerthistogramm
    public int[] forGray(byte[,,] array) {
      int[] histogramm = new int[256];
      foreach (byte element in array)
        histogramm[element]++;
      return histogramm;
    }

    /// <summary> ÜBERLADUNG
    /// 2.2 Liefert das Grauwerthistogramm eines eindimensionalen 
    /// Dreikanalefeldes mit dem Elementetyp "byte". Die drei Kanäle
    /// werden als Summe abgebildet.
    /// </summary>
    /// <param name="feld"></param> Dreikanalfeld 
    /// <returns></returns> Grauwerthistogramm
    public int[] forGray(byte[] feld) {
      int[] histogramm = new int[256];
      foreach (byte element in feld)
        histogramm[element]++;
      return histogramm;
    }

    /// <summary>
    /// 2.3 Liefert das Grauwerthistogramm eines eindimensionalen
    /// Dreikanalfeldes mit dem Elementetyp "float". Die drei Kanäle
    /// werden als Summe abgebildet.
    /// </summary>
    /// <param name="feld"></param> Dreikanalfeld
    /// <returns></returns> Grauwerthistogramm
    public int[] forGray(float[] feld) {
      int[] histogramm = new int[256];
      // elemente durch drei Teilen da r,g,b gezält werden.
      for (int i = 0; i < feld.Length; i++) {
        // Index im Byte-Intervall ermitteln
        byte idx = (byte)(feld[i] * 255.0);
        histogramm[idx]++;
      }
      return histogramm;
    }

    #endregion

    #region 3: RGB-Histogramm

    /// <summary>
    /// 3.1 Liefert die Histogramme der Kanäle
    /// eines dreidimensionalen Bildarrays
    /// mit einem beliebigen Datentyp der Elemente.
    /// </summary>
    /// <typeparam name="Tin"></typeparam> Datentyp der Elemente des Bildarrays.
    /// <param name="array3D"></param> Bildarray
    /// <returns></returns> RGB-Histogramm
    public int[,] forRGB<Tin>(Tin[,,] array3D) {
      int[,] histogramm = new int[array3D.GetLength(0), 256];
      for (int c = 0; c < array3D.GetLength(0); c++)
        for (int y = 0; y < array3D.GetLength(1); y++)
          for (int x = 0; x < array3D.GetLength(2); x++) {
            float Idx = (float)Convert.ChangeType(array3D[c, y, x], typeof(float));
            if ((typeof(Tin).Equals(typeof(float))) || (typeof(Tin).Equals(typeof(double)))) {
              Idx *= 255;
            }
            histogramm[c, (byte)Idx]++;
          }
      return histogramm;
    }


    /// <summary> ÜBERLADUNG
    /// 3.1: Liefert die Histogramme der drei Grundfarben r,g,b
    /// eines eindimensionalen Dreikanalefeldes
    /// mit dem Elementetyp "byte".
    /// </summary>
    /// <param name="feld"></param> Dreikanaldatenfeld
    /// <returns></returns> RGB-Histogramm
    public Histogram[] forRGB(byte[] feld) {
      Histogram[] histogramm = new Histogram[256];
      int ebenenPixel = (int)feld.Length / 3;
      for (int i = 0; i < ebenenPixel; i++) {
        histogramm[feld[i]].r++;
        histogramm[feld[i + ebenenPixel]].g++;
        histogramm[feld[i + 2 * ebenenPixel]].b++;
      }
      return histogramm;
    }

    /// <summary> ÜBERLADUNG
    /// 3.2: Liefert die Histogramme der drei Grundfarben r,g,b
    /// eines eindimensionalen Dreikanalefeldes
    /// mit dem Elementetyp "float".
    /// </summary>
    /// <param name="feld"></param> Dreikanaldatenfeld
    /// <returns></returns> RGB-Histogramm
    public Histogram[] forRGB(float[] feld) {
      Histogram[] histogramm = new Histogram[256];
      int ebenenPixel = (int)feld.Length / 3;
      for (int i = 0; i < ebenenPixel; i++) {
        histogramm[(byte)(feld[i] * 255.0)].r++;
        histogramm[(byte)(feld[i + ebenenPixel] * 255.0)].g++;
        histogramm[(byte)(feld[i + 2 * ebenenPixel] * 255.0)].b++;
      }
      return histogramm;
    }

    /// <summary>ÜBERLADUNG, GENERISCH
    /// 3.3 Liefert die Histogramme der drei Grundfarben r,g,b
    /// eines eindimensionalen Dreikanalefeldes
    /// mit beliebigem Elementetyp.
    /// </summary>
    /// <typeparam name="Tin"></typeparam> Datentyp der Feldelemente {duble, float, byte}
    /// <param name="feld"></param> Eindimensionales Dreikanaldatenfeld
    /// <returns></returns> RGB-Histogramm
    public Histogram[] forRGB<Tin>(Tin[] feld) {
      Histogram[] histogramm = new Histogram[256];
      int ebenenPixel = (int)feld.Length / 3;
      Type type = typeof(Tin); //Typ der Matrixelemente
      for (int i = 0; i < feld.Length; i += 3) {
        float r = (float)Convert.ChangeType(feld[i], type);
        float g = (float)Convert.ChangeType(feld[i + ebenenPixel], type);
        float b = (float)Convert.ChangeType(feld[i + 2 * ebenenPixel], type);
        if ((type.Equals(typeof(float))) || (type.Equals(typeof(double)))) {
          r *= 255;
          g *= 255;
          b *= 255;
        }
        histogramm[(int)Math.Round(r)].r++;
        histogramm[(int)Math.Round(g)].g++;
        histogramm[(int)Math.Round(b)].b++;
      }
      return histogramm;
    }
    #endregion

    #region 4. Diagramm darstellen, Chart-Komponente

    /// <summary> GENERIC
    ///  4.1 Stellt das Grauwerthistogramm 
    ///  grafisch dar. Es handelt sich um einen Kanal.
    /// </summary>
    /// <typeparam name="Tin"></typeparam> Datentyp der Histogrammelemente.
    /// <param name="histogramm"></param> Histogramm
    /// <param name="serie"></param> Serie
    public void showChart<Tin>(Tin[] histogramm, int[] serie) {
      for (int i = 0; i < histogramm.GetLength(0); i++)
        chart.Series[serie[0]].Points.AddXY(i, histogramm[i]);
    }

    /// <summary> ÜBERLADUNG, GENERIC
    /// 4.2 Stellt das RGB-Histogramm grafisch dar. die drei Kanäle 0,1,2
    /// werden Kurven dargestellt.
    /// </summary>
    /// <typeparam name="Tin"></typeparam> Datentyp der Elemente des Histogramms.
    /// <param name="histogramm"></param> Histogramm [canal, intensity]
    /// <param name="series"></param> Serien des Diagramms zur Darstellung der Kanäle.
    public void showChart<Tin>(Tin[,] histogramm, int[] series) {
      for (int c = 0; c < histogramm.GetLength(0); c++)
        // Häufigkeit der Intensitäten
        for (int i = 0; i < histogramm.GetLength(1); i++)
          chart.Series[series[c]].Points.AddXY(i, histogramm[c, i]);
    }

    /// <summary> ÜBERLADUNG
    /// 4.3 Stellt das Grauwerthistogramm grafisch dar.
    /// </summary>
    /// <param name="histogramm"></param> Histogramm
    public void showChart(int[] histogramm) {
      // Serie erhält neue Daten für Intervall [0, 1]
      chart.ChartAreas[0].AxisX.Maximum = 255;
      for (int i = 0; i < histogramm.Length; i++)
        chart.Series[0].Points.AddXY(i, histogramm[i]);
      chart.ChartAreas.FindByName("ChartArea_Ableitung").Visible = false;
    }

    /// <summary> ÜBERLADUNG
    /// 4.4 Stellt das Grauwerthistogramm grafisch dar. Eine Intervallskalierung 
    /// ist möglich und Vorgängerdaten können gelöscht werden.
    /// </summary>
    /// <param name="histogramm"></param> Histogramm
    /// <param name="intervall"></param> Intervall der Abszisse
    /// <param name="clear"></param> Löschen der Vorgängerdaten [Ja, nein]
    public void showChart(int[] histogramm, float intervall = 255, bool clear = true) {
      if (clear) // Löschen Vorgängerpunkte
        clearChart();
      // Serie erhält neue Daten für Intervall [0, 1]
      chart.ChartAreas[0].AxisX.Maximum = intervall;
      for (int i = 0; i < histogramm.Length; i++) {
        float x = (float)(i * intervall / 255.0);
        chart.Series[0].Points.AddXY(x, histogramm[i]);
      }
    }

    /// <summary> ÜBERLADUNG
    /// 4.3 Stellt das RGB-Histogramm grafisch dar.
    /// </summary>
    /// <param name="histogramm"></param> RGB-Histogramm
    public void showChart(Histogram[] histogramm) {
      // Seriendaten neu setzen
      for (int i = 0; i < histogramm.Length; i++) {
        chart.Series[3].Points.AddXY(i, histogramm[i].r);
        chart.Series[2].Points.AddXY(i, histogramm[i].g);
        chart.Series[1].Points.AddXY(i, histogramm[i].b);
      }
      chart.ChartAreas.FindByName("ChartArea_Ableitung").Visible = false;
    }

    /// <summary> ÜBERLADUNG
    /// 4.4 Stellt das RGB-Histogramm grafisch dar. die drei Kanäle RGB
    /// werden als gestapelte Säulen dargestellt. Eine Intervallskalierung 
    /// ist möglich und Vorgängerdaten können gelöscht werden.
    /// </summary>
    /// <param name="histogramm"></param> Histogramm
    /// <param name="intervall"></param> Intervall der Abszisse
    /// <param name="clear"></param> Löschen der Vorgängerdaten [Ja, nein]
    public void showChart(Histogram[] histogramm, float intervall = 255, bool clear = true) {
      if (clear) // Löschen Vorgängerpunkte
        clearChart();
      // Seriandaten neu setzen
      for (int i = 0; i < histogramm.Length; i++) {
        float x = (float)(i * intervall / 255.0);
        chart.Series[3].Points.AddXY(x, histogramm[i].r);
        chart.Series[2].Points.AddXY(x, histogramm[i].g);
        chart.Series[1].Points.AddXY(x, histogramm[i].b);
      }
    }

    #endregion

    #region 5 Diagrammpunkte setzen
    /// <summary>
    /// 5.1 Setzt Punkte in das Diagramm.
    /// </summary>
    /// <param name="threshold"></param> Schwellwertliste I[c]
    /// <param name="serien"></param> Serienfeld s[c]
    public void showChartPoinst(List<byte> threshold, int[] serien) {
      // Ordnet die Schwellwerte den Seiten zu.
      for (int c = 0; c < threshold.Count; c++)
        chart.Series[serien[c]].Points.AddXY(threshold[c], 0);
    }

    /// <summary> ÜBERLADUNG
    /// 5.2 Setzt Punkte in das Diagramm.
    /// Pro Kanla liegen mehrerte Schwellwerte vor.
    /// </summary>
    /// <param name="thresholds"></param> Doppelte Schwellwertliste I[c,i]
    /// <param name="serien"></param> Serienfeld s[c, i]
    public void showChartPoinst(List<List<byte>> thresholds, int[] serien) {
      // Ordnet mehrere Schwellwerte den Seien zu.
      for (int c = 0; c < thresholds.Count; c++)
        for (int i = 0; i < thresholds[i].Count; i++) {
          chart.Series[serien[c]].Points.AddXY(thresholds[c][i], 0);
        }
    }
    #endregion

    #region 6 Initialisieren und Löschen
    /// <summary>
    /// 6.1 Löscht das Diagramms mit allen
    /// Serien und der Legende.
    /// </summary>
    public void clearChart() {
      foreach (var element in chart.Series) {
        element.IsVisibleInLegend = false;
        element.Points.Clear();
      }
    }

    /// <summary>
    /// 6.2 Initialisiert das Histogramm für parametrierte Serien.
    /// </summary>
    /// <param name="series"></param> Serien die initialisiert werden sollen
    /// <param name="chartType"></param> Diagramm-Typ der Serien 1,2 und 3.
    public void initChart(int[] series, SeriesChartType chartType = SeriesChartType.Column) {
      // Zeige nur das Histogramm bzw. die Funktionen
      chart.ChartAreas.FindByName("ChartArea_Ableitung").Visible = false;
      // Alles löschen
      foreach (var element in chart.Series) {
        element.IsVisibleInLegend = false;
        element.Points.Clear();
      }
      // Serienabhängig zuschalten
      for (int i = 0; i < series.Length; i++) {
        chart.Series[series[i]].IsVisibleInLegend = true;
        if (series[i] > 3) {
          // Zeige zeigt auch die 1. Ableitung des Histogramms
          chart.ChartAreas.FindByName("ChartArea_Ableitung").Visible = true;
        }
      }
      // Grauwerthistogramm, Kanal 0, stets als Balken
      chart.Series[0].ChartType = SeriesChartType.Column;
      // RGB-Histogramm anwendungsabhängig, daher parametriert.
      for (int s = 1; s < 4; s++) {
        chart.Series[s].ChartType = chartType;
      }
      // Ableitung des RGB-Histogramms immer als Spline.
      for (int s = 4; s < 7; s++)
        chart.Series[s].ChartType = SeriesChartType.Spline;
    }


    /// <summary>
    /// 6.3 Generiert eine Feld aus einzelnen Parametern
    /// </summary>
    /// <typeparam name="T"></typeparam> Datentyp der Parameter und
    /// der Feldelemente
    /// <param name="feld"></param> Einzelparameter als Feld
    /// <returns></returns> Eindimensionales Feld
    public T[] toSeries<T>(params T[] feld) {
      return feld;
    }

    #endregion

  }
}
