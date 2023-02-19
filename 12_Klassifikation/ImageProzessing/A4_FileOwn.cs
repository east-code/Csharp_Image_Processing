using System.Collections.Generic;
using System.IO; // Für File
using System.Text;

namespace ImageProcessing {
  internal class FileOwn {
    #region 1. Attribute
    /// <summary>
    /// Hauptspeicherpfad
    /// </summary>
    public static string mainPath = @"E:\01-03_Csharp2021_Bildverarbeitung (20221026)\SkriptBilder\11_Klassifizierung\Training\Daten\";
    #endregion

    #region 2. Speichern
    /// <summary>
    /// Speichert Daten einer Liste als Textdatei mit dem senkreten Strich "|"
    /// zum Trennen der einzelnen Werte.
    /// </summary>
    /// <param name="subPath"></param> Sub-Pfad
    /// <param name="data"></param> Daten ener Liste
    /// <param name="classname"></param> Teileklasse {Schrauben, Muttern, ...}
    /// <param name="attribut"></param> Eigenschaft {Umfang, Fläche, ...}
    public static void saveTXT(string subPath, List<float> data, string classname = "0", string attribut = "0") {
      // Klassenindex an den Anfang hinzufügen
      data.Insert(0, classname[0] - 48);
      // Eigenschaftsindex auf den vorhandenen Dummy schreiben.
      data[1] = attribut[0] - 48;
      //Speicherort
      string path = mainPath + subPath;
      // Trennzeichen zwischen den Werten
      string seperator = " | ";
      // Instanz zum Stringaufbau
      StringBuilder sb = new StringBuilder();
      // Stellt den Ausgabestring zusammen
      sb.Append(string.Join(seperator, data));
      File.AppendAllText(path, sb.ToString());
      File.AppendAllText(path, "\r\n");
    }

    #endregion




  }
}
