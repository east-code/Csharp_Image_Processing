/* Klasse IdentifikationOwn: Identifiziert anders gesagt Indexiert mehrere Segmentflächen eines Bildarrays.
 * Verfasser: Horst-Christian Heinke
 * Datum: 19.05.2022
 * Letzte Änderung: 15.01.2023
 * Germany, Sachsen-Anhalt, Magdeburg
 * */

using System.Collections.Generic;
using System.Drawing; // PointF = Punkt mit Gleitkommakoordinaten (float)
using ARRAY = ImageProcessing.ArrayOwn; // Statische Elementarmethoden

namespace ImageProcessing {
  internal class IdentifikationOwn {

    #region 1. Umrandung und Identifikation Konturverfolgung mit Freeman-Code
    /// <summary>
    /// 1.1 Liefert die Starpunkte, Konturen und Identifikationen der Segmentflächen.
    /// Eingangsbild ist ein Binärbild.
    /// Alle Kanäle haben eingangs identische Werte [0, 1].
    /// Der Kanal 0 bleibt unverändert.
    /// Die gefundenen Segmente (Objekte) erhalten eine Identifikationsnummer 
    /// von 1 beginnend. Diese Wert werden auf dem Kanal 1 abgespeichert.
    /// </summary>
    /// <param name="array"></param> 3D-Array, hier auch Freeman-Array genannt.
    /// Kanal 0: Unveränderte Bild
    /// Kanal 1: Ausgefüllte konturumschließende Flächen (Segmentflächen) mit Identitätswerten.
    /// Kanal 2: Kontur mit Identitätswerten markiert.
    /// <param name="segmentIntensity"></param> Intensität der Segmentfläche
    /// <returns></returns> Ketencode, Konturen
    public List<List<uint>> chaincodes(ref float[,,] array, double segmentIntensity = 1) {
      // Startpunkt, Konturanfang
      Point start = new Point(1, 1);
      Point startVorgaenger = start;
      // Differenzen zu den Nachbarn.
      Point[] dNaneighbor = ARRAY.toPoints(ARRAY.shift);
      // Nachbarpunkt
      Point neighbor = new Point(1, 1);
      // Fokussierte, aktuelle Position
      Point pos = new Point(1, 1);
      // Merker für Suchschleife
      bool suchen = true; // Merker für Suchschleife
      // Die Segment-Id=0 ist für den Hintergrund reserviert.
      int segmentId = 0; // Die Segment-Id= 0 ist für den Hintergrund reserviert.
      // Listen der Kettencode aller Konturen.
      List<List<uint>> chaincodes = new List<List<uint>>();
      // Kettencodeliste einer Kontur. Die ersten beiden Elemente geben den Starpunkt.
      List<uint> chaincode = new List<uint>(2);
      // Hintergrund startet an den Koordinaten (0, 0)
      chaincode.Add(0);
      chaincode.Add(0);
      // Liste 0, da segmentId=0 für Hintergrund reserviert mit 0, 0 für die ersten Werte.
      chaincodes.Add(chaincode);
      // Array-Layers um die Randbreite 1 erweitern und die Randwerte auf 0 setzen. 
      array = ARRAY.padding(array, 1, 0);
      // Alle Werte des Kanals 1 und des Kanals 2 werden auf 0 gesetzt.
      ARRAY.setArrayChannelValue(ref array, 1, 0);
      ARRAY.setArrayChannelValue(ref array, 2, 0);
      do {  // Schleife für alle Segmentflächen
        segmentId++; // ID für Kontur der aktuellen Segmentfläche.
        // Liste für Kettencode anlegen.
        chaincode = new List<uint>();
        // Merker für die Suche nach einer Kontur.
        suchen = true;
        // Suche beginnt an der Startposition der Vorgängerkontur oder bei 1,1
        pos = start;
        // Doppelschleife zum Auffinden eines Startpunktes
        do { //Y-Schleif
          do { // X-Schleife
            //  Ist es ein Startpunkt einer noch nicht gefundenen Kontur?
            if (array[0, pos.Y, pos.X] == segmentIntensity && array[1, pos.Y, pos.X] == 0) {
              start = pos;
              // Starpunkt der Liste des Kettencodes hinzufügen
              chaincode.Add((uint)start.X);
              chaincode.Add((uint)start.Y);
              // Der Startpunkt des Objektes wird auf dem Kanal 1 und auf dem Kanal 2
              // mit der fortlaufenden Segment-Id markiert.
              array[1, pos.Y, pos.X] = segmentId;
              array[2, pos.Y, pos.X] = segmentId; // nur für Rand
              // Konturstartpunkt gefunden.
              suchen = false;
            }
            // x-Koordinate witwerschalten
            pos.X += 1;
            // Ist das Zeilenende erreicht und die Suche beendet?
          } while (pos.X < array.GetLength(2) && suchen);
          // Gehe zum Zeilenanfang und y-Koordinate weiterschalten.
          pos.X = 0;
          pos.Y += 1;
          // Ist das Arrayende mit dem Spaltenende erreicht und ist die Suche beendet?
        } while (pos.Y < array.GetLength(1) && suchen);
        // Wird keine weiterer Startpunkt gefunden?
        if (startVorgaenger == start) // Kontrolle
          break; // Abbruch der Suche nach weiteren Startpunkten und Konturen.
        // Merker des Startpunktes für Abbruchbedingung. 
        startVorgaenger = start;
        // Folge der Kontur vom Startpunkt ausgehend bis zum Startpunkt.
        // Die Konturverfolgung beginnt beim Nachbarn 1. Er ist kein Punkt der Segmentfläche,
        // da er links von der gefundenen Kontur liegt.
        // Der Startpunkt ist zugleich ein Konturpunkt.
        Point chainPoint = new Point(start.X, start.Y);
        // s-Zeiger zu einem Nachbarn (fortlaufend).
        int s = 0;
        // s-Zeiger zu einem Nachbarn des Kreisintervalls [0,8[.
        int s8 = s;
        // Doppelschleife zur Konturverfolgung.
        do {
          // Nur zur Sicherheit. Zyklus von 8 verhindert eine Suche ins Unendliche.
          uint zyklus = 0;
          do {
            // s-Index oder Zeiger zum Nachbarn inkrementieren.
            s += 1;
            // Zyklus inkrementieren für sicheren Ausstieg aus der Schleife
            zyklus += 1;
            // s wächst kontinuierlich um 1, muss auf das Intervall [0, 8[ reduziert werden.
            s8 = ARRAY.circleInterval(s, 8);
            // Nachbarpunkt ist einer der Nachbarn des aktuellen Kettenpunktes  
            neighbor = new Point(chainPoint.X + dNaneighbor[s8].X, chainPoint.Y + dNaneighbor[s8].Y);
            //- neighbor = new Point(chainPoint.X + ARRAY.shift[s_check, 0], chainPoint.Y + ARRAY.shift[s_check, 1]);
          }
          // Gehört der Nachbar nicht zur Segmentfläche und wurden noch nicht alle Nachbarn kontrolliert?
          // dann bleibe in der Schleife und kontrolliere den Folgenachbarn.
          while (array[0, neighbor.Y, neighbor.X] != segmentIntensity && zyklus <= 8);
          // Ein Segmentpunkt am Rand der Segmentfläche wurde gefunden.
          // Markiere den Rand-Segment-Punkt mit der aktuellen Id.
          array[1, neighbor.Y, neighbor.X] = segmentId;
          array[2, neighbor.Y, neighbor.X] = segmentId;
          // Richtung der Kettencodeliste hinzufügen.
          chaincode.Add((uint)s8);
          // Gehe einen oder zwei Schritte zurück, damit der Zeiger
          // auf ein Element vor der Segmentfläche zeigt.
          // Zu unterscheiden ist zwischen direkter und diagonaler Nachbarschaft.
          s -= 3; // Nebenpunkt
          // Handelt es sich um einen Diagonlpunkt?
          if (neighbor.X != chainPoint.X && neighbor.Y != chainPoint.Y) {
            s -= 1; // Diagonalpunkt
          }
          // Der neue Kettencodepunkt ergibt sich aus dem Nachbarpunkt.
          chainPoint = neighbor;
        }
        // Ist die Kontur noch nicht bis zum Startpunkt geschlossen?
        // Dann setze die Suche fort.
        while (chainPoint.X != start.X || chainPoint.Y != start.Y);
        // Der Kettencode einer Kontur ist vollständig
        // und wird an die Liste der Kettencode angehängt.
        chaincodes.Add(chaincode);
        // Die geschlossene Kontur ist mit der segmentflächenspezifischen Id
        // auszufüllen. Dafür ist der Kanal 1 vorgesehen.
        segmentFill(ref array, segmentId);
      }
      while (true);
      // Das gesamte Array ist nach Segmentflächen abgesucht worden.
      // Der Rand mit der Breite 1 wird entfernt.
      array = ARRAY.unpadding(array);
      // Startpunkte der Kettencode werden um 1 verringert.
      ARRAY.pointsUnpadding(ref chaincodes);
      return chaincodes;
    }

    #endregion
    #region 2. Füllen von Flächen
    /// <summary> 
    /// 2.1 Füllt umrandete Segmentflächen des Chain-Arrays mit der übergebenen Id.
    /// Die Gestalt der Segmentfläche ist bekannt und ist über den Kanal 0 verfügbar. 
    /// Es ist ein einfacher, aber nicht perfekter Algorithmus. Schatten lassen sich nicht immer
    /// vermeiden. Je komplizierter die Geometrien der Objekte umso mehr Aufwand.
    /// Kanal 0 ist das Original mit Segmentflächen der Intensität >0 (1)
    /// Kanal 1 ID-Umrandung wird ausgefüllt.
    /// Kanal 2 ID-Umrandung bleibt unverändert
    /// <param name="array"></param> Array mit Segmentflächen.
    /// <param name="segmentId "></param> Identifikationsnummer Id
    /// <param name="c"></param> Kanal deren Segmentflächen ausgefüllt werden sollen. 
    public void segmentFill(ref float[,,] array, int segmentId = 1, int c = 1) {
      // a) von rechts unten nach links oben
      for (int x = array.GetLength(2) - 2; x > 0; x--)
        for (int y = array.GetLength(1) - 2; y > 0; y--) {
          bool segment = false;
          bool idItem = false;
          // Ist das Element einer Segmentfläche zugehörig?
          if (array[0, y, x] > 0)
            // Das Element ist Segmentflächenelement.
            segment = true;
          // Besitzt eines der Vorgängerelemente eine ID?
          if (array[c, y, x + 1] == segmentId || array[c, y + 1, x] == segmentId || array[c, y + 1, x + 1] == segmentId)
            // Vorgänger: Das Element besitzt eine Id.
            idItem = true;
          // Trifft beides zu?
          if (segment && idItem)
            // Dann muss dieses Element auch dazugehören.
            array[c, y, x] = segmentId;
        }

      // b) von links unten nach rechts oben
      for (int x = 1; x < array.GetLength(2); x++)
        for (int y = array.GetLength(1) - 2; y > 0; y--) {
          bool segment = false;
          bool idItem = false;
          if (array[0, y, x] > 0)
            segment = true;
          if (array[c, y, x - 1] == segmentId || array[c, y + 1, x] == segmentId || array[c, y + 1, x - 1] == segmentId)
            idItem = true;
          if (segment && idItem)
            array[c, y, x] = segmentId;
        }

      // c) von links oben nach rechts unten
      for (int y = 1; y < array.GetLength(1); y++)
        for (int x = 1; x < array.GetLength(2); x++) {
          bool segment = false;
          bool idItem = false;
          if (array[0, y, x] > 0)
            segment = true;
          if (array[c, y, x - 1] == segmentId || array[c, y - 1, x] == segmentId || array[c, y - 1, x - 1] == segmentId)
            idItem = true;
          if (segment && idItem)
            array[c, y, x] = segmentId;
        }

      // d) von rechts oben nach links unten
      for (int y = 1; y < array.GetLength(1); y++)
        for (int x = array.GetLength(2) - 2; x > 0; x--) {
          bool segment = false;
          bool idItem = false;
          if (array[0, y, x] > 0)
            segment = true;
          if (array[c, y, x + 1] == segmentId || array[c, y - 1, x] == segmentId || array[c, y - 1, x + 1] == segmentId)
            idItem = true;
          if (segment && idItem)
            array[c, y, x] = segmentId;
        }
    }



    #endregion

    #region 3. Nachbarschaft: Kontur und Segment

    /// <summary>
    /// 3.1 Konturen per Nachbarschaftsoperationen abbilden.
    /// </summary>
    /// <param name="array"></param> 3D-Array
    /// Kanal 0: unverändert
    /// Kanal 1: unverändert (reserviert für ID's der Segmentflächen)
    /// Kanal 2: Konturen, Umrandung der Segmente
    /// <param name="segmentIntensity"></param> Intensität der Elemente der Segmentflächen
    public void contour(ref float[,,] array, double segmentIntensity = 1) {
      int segmentId = 1;
      // Verschiebung zu den Nachbarn als Punkte.
      Point[] dNeighbor = ARRAY.toPoints(ARRAY.shift);
      // Arrayebenen um den Rand 1 erweitern und die Randwerte auf 0 setzen. 
      array = ARRAY.padding(array, 1, 0);
      // Alle Elemente des Kanal 2 erhalten den Wert 0.
      // Auf diesem Kanal werden die Konturen abgebildet.
      ARRAY.setArrayChannelValue(ref array, 2, 0);
      // Doppelschleife für alle echten Elemente des Arrays.
      for (int y = 1; y < array.GetLength(1) - 1; y++) // Für alle Bildpunkte und
        for (int x = 1; x < array.GetLength(2) - 1; x++) { // -> Rand beachten
          // Ist es ein Punkt der Segmentfläche?
          if (array[0, y, x] >= segmentIntensity)
            // Hat der Punkt einen Nachbarn, der nicht zum Segment gehört?
            for (int s = 0; s < dNeighbor.Length; s++) {
              Point neighbor = new Point(x + dNeighbor[s].X, y + dNeighbor[s].Y);
              if (array[0, neighbor.Y, neighbor.X] == 0) {
                // Es ist ein Randpunkt.
                array[2, y, x] = segmentId;
                break;
              }
            }
        }
      // Rand der Breite 1 wird entfernt.
      array = ARRAY.unpadding(array);
    }

    /// <summary>
    /// 3.2 Erkennt Segmentflächen per Nachbarschaftsbeziehungen 
    /// </summary>
    /// <param name="array"></param> Array-Referenz
    /// Kanal 0: Unverändert
    /// Kanal 1: Segmentflächen werden gefüllt mit einer fortlaufenden Id.
    /// Kanal 2: Unverändert (reserviert für Umrandung).
    /// <param name="dBackwardNeighbor"></param> Feld mit zurückliegenden Punkten
    /// <param name="segmentIntensity"></param> Intensität der Segmentelemente
    /// <returns>"startPonts"</returns> Startpunkte der Segmentflächen
    public List<Point> segments(ref float[,,] array, Point[] dBackwardNeighbor, double segmentIntensity = 1) {
      // Startpunkte der Segmentflächen
      List<Point> startPonts = new List<Point>();
      // Hintergrund
      startPonts.Add(new Point(0, 0));
      // Id's für Segmente  
      int segmentId = 0;
      // Array-Layers um den Rand 1 erweitern und die Randwerte auf 0 setzen. 
      array = ARRAY.padding(array, 2, 0);
      // Alle Elemente des  Kanals 1 erhalten den Wert 0.
      // Auf dem Kanal werden die Segmentflächen abgebildet.
      ARRAY.setArrayChannelValue(ref array, 1, 0);
      // Doppelschleife für alle echten Elemente des Arrays.
      for (int y = 1; y < array.GetLength(1) - 1; y++)
        for (int x = 1; x < array.GetLength(2) - 1; x++) {
          // Ist es ein Punkt der Segmentfläche?
          if (array[0, y, x] >= segmentIntensity) {
            // Merker für zurückliegende Nachbarn
            int segments = 0;
            // Schleife für zurückliegende Nachbarn
            for (int s = 0; s < dBackwardNeighbor.Length; s++) {
              // Zurückliegender Nachbar als Punkt
              Point backwardNeighbor = new Point(x + dBackwardNeighbor[s].X, y + dBackwardNeighbor[s].Y);
              // Gehört der zurückliegende Nachbar zum Hintergrund?
              if (array[0, backwardNeighbor.Y, backwardNeighbor.X] == segmentIntensity) {
                // Dann gehört das Element zu einer bereits erkannten Segmentfläche.
                array[1, y, x] = array[1, backwardNeighbor.Y, backwardNeighbor.X];
                break;
              }
              else {
                // Andernfalls zähle diesen Nachbarn als Hintergrund.
                segments++;
              }
            }
            // Sind alle zurückliegenden Nachbarn keiner Segmentfläche zugehörig?
            if (segments == dBackwardNeighbor.Length) {
              // Dann handelt es sich um ein Element einer neuen Segmentfläche mit einer neuen Id.
              segmentId++;
              // Startpunkt in Liste übernehmen. Die Subtraktion mit -1 berücksichtigt 
              // vorausschauend,dass es sich um ein erweitertes (vergrößertes) Array handelt. 
              startPonts.Add(new Point(x - 1, y - 1));
              // Element des Arrays markieren.
              array[1, y, x] = segmentId;
            }
          }
        }
      // Rand mit der Breite 1 entfernen.
      array = ARRAY.unpadding(array, 2);
      return startPonts;
    }

    /// <summary> OPTIONAL
    /// 3.3 Setzt vorauseilende Nachbarelemente, wenn diese zur Segmentfläche gehören.
    /// Kanal 0: Unverändert
    /// Kanal 1: Segmentflächen werden gefüllt mit einer fortlaufenden ID.
    /// Kanal 2: Unverändert (reserviert für Umrandung).
    /// </summary>
    /// <param name="array"></param> 3D-Array
    /// <param name="pos"></param> fokussierte Position
    /// <param name="segmentIntensity"></param> Intensitätswert der Segmentflächenelemente.
    public void setForward(ref float[,,] array, Point pos, double segmentIntensity) {
      // Verschiebung zu den Nachbarn.
      Point[] dForwardNeighbor = ARRAY.toPoints(ARRAY.forwardShift);
      Point forwardNeighbor;
      if (array[0, pos.Y, pos.X] >= segmentIntensity)
        for (int s = 0; s < dForwardNeighbor.Length; s++) {
          forwardNeighbor = new Point(pos.X + dForwardNeighbor[s].X, pos.Y + dForwardNeighbor[s].Y);
          if (array[0, forwardNeighbor.Y, forwardNeighbor.X] >= segmentIntensity)
            array[1, forwardNeighbor.Y, forwardNeighbor.X] = 1;
          else
            // wird ein Anschluss mehr gefunden, dann breche ab.
            break;
        }
    }



    #endregion

  }
}