using System;
using System.Collections.Generic;

namespace ImageProcessing {
  internal class StatisticOwn {
    #region 1. Attribute, Datenstrukturen

    /// <summary>
    /// 1.1 Strukur zu den statistischen Kenngrößen
    /// </summary>
    public struct normalDistribution {
      public double mean; // Mittelwert
      public double standardDeviation; // Standardabweichung
      public double confidenceInterval; // Konfidenzintervall
    }


    #endregion

    #region 2. Methoden zur Statistik
    /// <summary>
    /// 2.1 Liefert den Mittelwert
    /// </summary>
    /// <typeparam name="T"></typeparam> Datentyp der Listenelemente
    /// <param name="liste"></param>Liste mit Werten
    /// <param name="idStart"></param> Element mit dem Index beginnt Berechnung
    /// <returns></returns> Mittelwert
    public static double average<T>(float[] array) {
      double mean = 0;
      for (int id = 0; id < array.GetLength(0); id++)
        mean += array[id];
      return mean / (array.GetLength(0));
    }

    /// <summary>
    /// Liefert die Standardabweichung eines Arrays
    /// </summary>
    /// <typeparam name="T"></typeparam> Datentyp der felderlemente
    /// <param name="array"></param> Array
    /// <param name="mean"></param> Mittelwert
    /// <returns></returns>Standardabweichung
    public static double standard<T>(float[] array, double mean) {
      double std = 0;
      for (int id = 0; id < array.GetLength(0); id++)
        std += (array[id] - mean) * (array[id] - mean);
      std = Math.Sqrt(std / (array.GetLength(0) - 1));
      return std;
    }
    /// <summary> ÜBERLADUNG
    /// Liefert die Standardabweichung mit interner Berechnung des Mittelwertes
    /// </summary>
    /// <typeparam name="T"></typeparam> Datentyp der feldelemente
    /// <param name="array"></param> Array
    /// <returns></returns> Standardabweichung
    public static double standard<T>(float[] array) {
      double mean = average<float>(array);
      return standard<T>(array, mean);
    }

    /// <summary> ÜBERLADUNG
    /// Liefert die Standardabweichung mit interner Berechnung des Mittelwertes
    /// </summary>
    /// <typeparam name="T"></typeparam> Datentyp der feldelemente
    /// <param name="array"></param> Array
    /// <returns></returns> Standardabweichung
    public static double confidence<T>(float std, double t = 2.201) {
      return std * t;
    }



    /// <summary>
    /// 2.2 Liefert Kenngrößen einer normalverteilten Menge
    /// </summary>
    /// <typeparam name="T"></typeparam> Datentyp der Listenelemennte
    /// <param name="list"></param> Liste mit Werten
    /// <param name="idStart"></param> Element mit dem Index beginnt Berechnung
    /// <param name="t"></param> Studentverteilungswert t(n=11, alpha=95%)=2.201
    /// <returns></returns> Kenngrößen der Normalverteilung entsprechend 1.1
    public static normalDistribution confidence<T>(ref List<T> list, int idStart = 0, double t = 2.201) {
      // Struktur der Normalverteilungskenngrößen
      normalDistribution nd;
      // Anzahl der Elemente in der Liste
      int n = list.Count;
      // Mittelwertschleife
      nd.mean = 0;
      for (int id = idStart; id < n; id++)
        nd.mean += Convert.ToDouble(list[id]);
      nd.mean /= (n - idStart);
      // Standardabweichung
      nd.standardDeviation = 0;
      for (int id = idStart; id < n; id++) {
        double value = Convert.ToDouble(list[id]);
        nd.standardDeviation += (value - nd.mean) * (value - nd.mean);
      }
      nd.standardDeviation = Math.Sqrt(nd.standardDeviation / (n - 1));
      // Konfidenzintervall
      nd.confidenceInterval = nd.standardDeviation * t;
      return nd;
    }


    #endregion



  }
}
