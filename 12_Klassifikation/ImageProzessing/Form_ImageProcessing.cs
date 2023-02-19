/* Demonstartionsformularklasse zur Bildverarbeitung
 * Verfasser: Horst-Christian Heinke
 * Datum: 15.06.2022
 * Letzte Änderung: 15.02.2023
 * Germany, Sachsen-Anhalt, Magdeburg
 * */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
// Statische Elementarmethoden zu Verarbeitung von Arrays
using ARRAY = ImageProcessing.ArrayOwn;
namespace ImageProcessing {
  public partial class Form_ImageProcessing : Form {

    #region 01. Attribute, Instanzen, Konstruktor
    /// <summary>
    /// 01.1 Attribute, Instanzen
    /// </summary>
    // ... Own ...
    OutputOwn output;        // Ausgabemethoden              
    WebCamOwn cam;           // Einbinden von Kameras 
    ImageOwn image;          // Laden, speichern von Bildern
    ImageTableOwn imgTab;    // Bilder als Tabelle mit Ausgabe der Farbwerte
    ImageArrayOwn imgArray;  // Bild in ein Felde konvertieren und zurück
    HistogramOwn histogram;  // Histogramme von Bild-Feldern                
    PixelOperationOwn pixOp; // Objekt der Klasse Pixeloperationen                   
    SegmentFeatureOwn feature;   // Objekteigenschaften
    IdentifikationOwn ident;     // Identifikation von Objekten/Segmentflächen
    NeighborhoodOperationOwn neighborOp; // Nachbarschaftsoperationen
    SegmentationOwn segment; //Erweiterung zur Segmentierung
    ClassificationOwn classes;  // Klassifikation von Daten
    // ............................................
    Graphics graphics_img;       // Zeichnen
    //Bitmap bitmap = new Bitmap(ImageProcessing.Properties.Resources.Testbild_7x10_RGB); // Eingangsbild
    //Bitmap bitmap = new Bitmap(ImageProcessing.Properties.Resources.EinPfeil); // Eingangsbild
    Bitmap bitmap = new Bitmap(ImageProcessing.Properties.Resources.Pneumatikzylinder_Auflicht); // Eingangsbild
    Rectangle rahmenInPictureBox; // Rahmenkordinaten 
    Bitmap subBitmap; // Bildbereich, Rahmenkoordinaten
    Bitmap resultBitmap; // Bearbeitetes, verändertes Image

    /// <summary>
    /// 01.2 Konstruktor, Instanzen initialisieren
    /// </summary>
    public Form_ImageProcessing() {
      InitializeComponent();
      // Instanzen, Übergabe von Komponenten des Formulars.
      output = new OutputOwn(richTextBox_Info, tabControl_IP, pictureBox_imgResult);  // Ausgabe
      cam = new WebCamOwn(comboBox_Device, comboBox_Capabilities, panel_Cam, pictureBox_image); // Kamera
      image = new ImageOwn(this.pictureBox_image); // Bilder laden, speichern, ansehen
      imgTab = new ImageTableOwn(dataGridView_imgTable); // Tabellenbild
      imgArray = new ImageArrayOwn();  // Imageformate und Gestalt 
      histogram = new HistogramOwn(chart_Histogramm); // Histogramm-Instanz     
      pixOp = new PixelOperationOwn(); // Pixeloperationen
      feature = new SegmentFeatureOwn();    // Eigenschaften von Segmentflächen
      ident = new IdentifikationOwn();  // Identifikation, jede Segmentfläche erhält eine eigen ID.
      neighborOp = new NeighborhoodOperationOwn(); // Nachbarschaftsoperationen
      segment = new SegmentationOwn(richTextBox_Info);// Erweiterung und Optimierung zur Segmentierung
      classes = new ClassificationOwn();
      // Externe Klassen
      // zum Beschriften
      graphics_img = pictureBox_image.CreateGraphics();
      // Angeschlossene Wabcams zusammenstellen
      cam.EnumerationVideoDevices();
      // Startbedingungen, Informationen zum Testbild
      richTextBox_Info.Text = "Testbild: ImageProcessing.Properties.Resources.Testbild\n\n";
      // Registerseite nach Programmstart
      tabControl_IP.SelectedTab = tabPage_image;
      EnableConnectionControls(true);
    }
    #endregion

    #region 03. Bild Laden, Speichern
    /// <summary>
    /// 03.1 Bild aus Datei laden und in der PictureBox darstellen.
    /// </summary>
    private void bildLadenToolStripMenuItem_Click(object sender, EventArgs e) {
      this.bitmap = image.openBitmapFile();
      tabControl_IP.SelectedTab = tabPage_image;
      output.bitmapInfo(bitmap, "Geladenes Bitmap");
      // Unterdrückt den ungewollt erzeugten Auswahlrahmen während des Ladens eines Bildes.
      Thread.Sleep(100);
      rahmenInPictureBox = new Rectangle(0, 0, 1, 1);
    }
    /// <summary>
    /// 03.2 Speichert das Bild als Datei in einem beliebigen Format (bmp, jpg, png)
    /// </summary>
    private void bildSpeichernToolStripMenuItem_Click(object sender, EventArgs e) {
      image.saveBitmapFile(this.bitmap);
    }
    /// <summary>
    /// 03.3 Speichert das Bild der aufgeschlagenen Seite des Registers
    /// </summary>
    /// 
    private void bildSpeichernUnterToolStripMenuItem_Click(object sender, EventArgs e) {
      image.saveBitmapFile(getSelectedPictureBoxImage());
    }

    #endregion
    #region 03-1. Mausaktionen für Bildbereich
    /// <summary>
    /// 03-1.1: Linke Maustaste wird gedrückt 
    /// -> Linker oberer Eckpunkt
    /// </summary>
    private void pictureBox_Bild_MouseDown(object sender, MouseEventArgs e) {

      if (e.Button == MouseButtons.Left) {
        //Linke obere Ecke des Rahmens
        rahmenInPictureBox.X = e.Location.X;
        rahmenInPictureBox.Y = e.Location.Y;
      }
    }
    /// <summary>
    /// 03-1.2 Maus von links oben nach rechts unten bewegen 
    /// -> rechter unterer Diagonal-Punkt selektieren und
    /// Rahmen während der Bewegung mitzeichnen
    /// </summary>
    private void pictureBox_Bild_MouseMove(object sender, MouseEventArgs e) {
      Pen pen = new Pen(Color.Red, 3); // Zeichenstift definieren
      if (e.Button == MouseButtons.Left) {
        // Bereichsweite und Bereichshöhe
        // --> Rahmen ziehen nach rechts unten
        rahmenInPictureBox.Width = e.Location.X - rahmenInPictureBox.X;
        rahmenInPictureBox.Height = e.Location.Y - rahmenInPictureBox.Y;
        // Vermeidet Überzeichnungen, Bild neu in der PictureBox darstellen
        if (e.Location.Y % 3 == 0 || e.Location.X % 3 == 0)
          pictureBox_image.Image = new Bitmap(bitmap);
        // Zeichnet neuen Rahmen
        graphics_img.DrawRectangle(pen, rahmenInPictureBox);
      }
    }

    /// <summary>
    /// 03-1.3 Ermittelt den rechten unteren Eckpunkt des Bereichs
    /// </summary>
    private void pictureBox_Bild_MouseUp(object sender, MouseEventArgs e) {
      Pen pen = new Pen(Color.Yellow, 3);
      if (e.Button == MouseButtons.Left) {
        // Rahmen nach rechts unten gezogen.
        rahmenInPictureBox.Width = e.Location.X - rahmenInPictureBox.X;
        rahmenInPictureBox.Height = e.Location.Y - rahmenInPictureBox.Y;
        graphics_img.DrawRectangle(pen, rahmenInPictureBox);
      }
    }

    /// <summary>
    /// 03-1.4 Der markierte Bereich wird auf das Originalbild umgerechnet,
    /// als Bitmap abgelegt und in einer PicturBox angezeigt.
    /// </summary>
    private void pictureBox_Bild_MouseClick(object sender, MouseEventArgs e) {
      Pen pen = new Pen(Color.GreenYellow, 3);
      graphics_img.DrawRectangle(pen, rahmenInPictureBox);
      subBitmap = this.image.subBitmap(bitmap, rahmenInPictureBox);
      pictureBox_imgArea.Image = subBitmap;
      tabControl_IP.SelectedTab = tabPage_imgArea; // zur Bildbereichsseite
    }
    #endregion Mausaktionen

    #region 02-2. Bild modifizieren
    /// <summary>
    /// 02-2.1 Skaliert die Bitmaps, so dass sie der Größe der Bildbox entsprichen.
    /// Das Seitenverhältnis bleibt unverändert.
    /// </summary>
    private void einpassenToolStripMenuItem_Click(object sender, EventArgs e) {
      pictureBox_imgArea.SizeMode = PictureBoxSizeMode.Zoom;
      pictureBox_image.SizeMode = PictureBoxSizeMode.Zoom;

    }
    /// <summary>
    /// 02-2.2 Skaliert die Bitmap auf die Größe der Bildbox ohne Rücksicht auf
    /// das Seitenverhältnis.
    /// </summary>
    private void streckenToolStripMenuItem_Click(object sender, EventArgs e) {
      pictureBox_imgArea.SizeMode = PictureBoxSizeMode.StretchImage;
      pictureBox_image.SizeMode = PictureBoxSizeMode.StretchImage;
    }
    #endregion

    #region 04. Kamera
    /// <summary>
    /// 04.1 Ermittelt die verfügbaren Kamera-Auflösungen.
    /// </summary>
    private void comboBox_Device_SelectedIndexChanged(object sender, EventArgs e) {
      cam.VidioDevice();
      cam.EnumerationFrameSizes();
    }
    /// <summary>
    /// 04.2 Starten der Web-Cam
    /// </summary>
    private void startToolStripMenuItem_Click(object sender, EventArgs e) {
      cam.Start();
      tabControl_IP.SelectedTab = tabPage_Cam;  // Registerseite aufschlagen
      EnableConnectionControls(false);
    }
    /// <summary>
    /// 04.3 Stop der Übertragung
    /// </summary>
    private void stopToolStripMenuItem_Click(object sender, EventArgs e) {
      EnableConnectionControls(true);
      cam.Stop();
    }
    /// <summary>
    /// 04.4 Liefert ein Standbild mit Zusatzinformationen
    /// </summary>
    private void shotToolStripMenuItem_Click(object sender, EventArgs e) {
      this.bitmap = cam.shot();   // Instanzmethode zum Fangen eines Bildes aufrufen
      tabControl_IP.SelectedTab = tabPage_image;  // Registerseite aufschlagen
      // Instanzmethode kommentiert den Verlauf zur Kontrolle.
      output.bitmapInfo(this.bitmap, "Standbild");
    }
    /// <summary> Methode
    /// 04.5 Aktivieren/Deaktivieren von Kontrollelementen.
    /// </summary>
    /// <param name="enable"></param> Umschalter
    private void EnableConnectionControls(bool enable) {
      comboBox_Device.Enabled = enable;
      comboBox_Capabilities.Enabled = enable;
      startToolStripMenuItem.Enabled = enable;
      stopToolStripMenuItem.Enabled = !enable;
    }
    #endregion Kamera

    #region 05. Bild->Array->Bild zum Testen der Transformation
    /// <summary>
    /// 05.1 Konvertiert ein Bitmap in ein 3D-Array und wieder zurück.
    /// Das Ergebnisbild wird angezeigt und muss dem Original entsprechen.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void bild3DArrayBildToolStripMenuItem_Click(object sender, EventArgs e) {
      float[,,] array3D = new float[3, bitmap.Height, bitmap.Width];
      array3D = imgArray.toArray3D<float>(bitmap);
      resultBitmap = imgArray.toBitmap(array3D);
      output.bitmapShow(bitmap, "\nTest: Transformiert ein Bild in ein 3D-Array und zurück in ein Bild", 4);
    }

    /// <summary>
    /// 05.2 Konvertiert ein Bitmap in ein 1D-Feld und wieder zurück.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void bild1DFeldBildToolStripMenuItem_Click(object sender, EventArgs e) {
      float[] feld1D = new float[3 * bitmap.Height * bitmap.Width];
      feld1D = imgArray.toFeld1D<float>(bitmap);
      int[] shape = imgArray.giveShape(bitmap);
      resultBitmap = imgArray.toBitmap(feld1D, shape);
      output.bitmapShow(bitmap, "\nTest: Transformiert ein Bild in ein 1D-Feld und zurück in ein Bild", 4);
    }

    /// <summary>
    /// 05.3 Geschwindigkeitstest - toGray
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void speedTesttoGrayToolStripMenuItem_Click(object sender, EventArgs e) {
      // 1) float-flatten des Bildes
      ImageArrayOwn.BuildType<float> buildFloat = imgArray.toBuild<float>(this.bitmap);
      // 2) Byte-flatten des Bildes
      ImageArrayOwn.BuildType<byte> buildByte = imgArray.toBuild<byte>(this.bitmap);
      // 3) Matrix des Bildes
      float[,,] floatArray3D = imgArray.toArray3D<float>(bitmap);
      // 4) Matrix des Bildes
      byte[,,] byteArray3D = imgArray.toArray3D<byte>(bitmap);
      Stopwatch stopwatchFloatFeld = new Stopwatch();
      Stopwatch stopwatchByteFeld = new Stopwatch();
      Stopwatch stopwatchFloatArray3D = new Stopwatch();
      Stopwatch stopwatchByteArray3D = new Stopwatch();
      // Fall1: Pixeloperation auf float-Feld
      stopwatchFloatFeld.Start();
      pixOp.toGray(ref buildFloat.feld1D);
      stopwatchFloatFeld.Stop();
      // Fall2:Pixeloperation auf byte-Feld
      stopwatchByteFeld.Start();
      pixOp.toGray(ref buildByte.feld1D);
      stopwatchByteFeld.Stop();
      // Fall3:Pixeloperation auf doubleMatrix
      stopwatchFloatArray3D.Start();
      pixOp.toGray(ref floatArray3D);
      stopwatchFloatArray3D.Stop();
      // Fall4:Pixeloperation byteMatrix
      stopwatchByteArray3D.Start();
      pixOp.toGray(ref byteArray3D);
      stopwatchByteArray3D.Stop();
      richTextBox_Info.Text = "Geschwindigkeitsmessung der Umwandlung eines Bildes in ein Grauwertbild mit unterschiedlichen Methoden.\n";
      richTextBox_Info.Text += "Methode                                    |       Werte in ms ";
      richTextBox_Info.Text += "\n 1) public void toGray(float[] feld1D)     " + stopwatchFloatFeld.ElapsedMilliseconds;
      richTextBox_Info.Text += "\n 2) public void toGray(byte[] feld1D)      " + stopwatchByteFeld.ElapsedMilliseconds;
      richTextBox_Info.Text += "\n 3) public void toGray(float[,,] array3D)  " + stopwatchFloatArray3D.ElapsedMilliseconds;
      richTextBox_Info.Text += "\n 4) public void toGray(byte[,,] array3D)   " + stopwatchByteArray3D.ElapsedMilliseconds;
    }

    /// <summary>
    /// 05.4 Geschwindigkeitstest für Transformation in ein Falschfarbenbild
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void speedTesttoFalseColorsToolStripMenuItem_Click(object sender, EventArgs e) {
      // 1) float-flatten des Bildes
      ImageArrayOwn.BuildType<float> buildFloat = imgArray.toBuild<float>(this.bitmap);
      // 2) Byte-flatten des Bildes
      ImageArrayOwn.BuildType<byte> buildByte = imgArray.toBuild<byte>(this.bitmap);
      // 3) Matrix des Bildes
      float[,,] floatArray3D = imgArray.toArray3D<float>(bitmap);
      // 4) Matrix des Bildes
      byte[,,] byteArray3D = imgArray.toArray3D<byte>(bitmap);
      Stopwatch stopwatchFloatFeld = new Stopwatch();
      Stopwatch stopwatchByteFeld = new Stopwatch();
      Stopwatch stopwatchFloatArray3D = new Stopwatch();
      Stopwatch stopwatchByteArray3D = new Stopwatch();
      // Fall1: Pixeloperation auf float-Feld
      stopwatchFloatFeld.Start();
      pixOp.toFalseColors(buildFloat.feld1D);
      stopwatchFloatFeld.Stop();
      // Fall2:Pixeloperation auf byte-Feld
      stopwatchByteFeld.Start();
      pixOp.toFalseColors(buildByte.feld1D);
      stopwatchByteFeld.Stop();
      // Fall3:Pixeloperation auf doubleMatrix
      stopwatchFloatArray3D.Start();
      pixOp.toFalseColors(floatArray3D);
      stopwatchFloatArray3D.Stop();
      // Fall4:Pixeloperation byteMatrix
      stopwatchByteArray3D.Start();
      pixOp.toFalseColors(byteArray3D);
      stopwatchByteArray3D.Stop();
      richTextBox_Info.Text = "Geschwindigkeitsmessung der Umwandlung eines Bildes in ein Falschfarbenbild mit unterschiedlichen Verfahren und Datentypen.\n";
      richTextBox_Info.Text += "Methode                                    |       Werte in ms ";
      richTextBox_Info.Text += "\n 1) public void toFalseColors(float[] feld1D)     " + stopwatchFloatFeld.ElapsedMilliseconds;
      richTextBox_Info.Text += "\n 2) public void toFalseColors(byte[] feld1D)      " + stopwatchByteFeld.ElapsedMilliseconds;
      richTextBox_Info.Text += "\n 3) public void toFalseColors(float[,,] array3D)  " + stopwatchFloatArray3D.ElapsedMilliseconds;
      richTextBox_Info.Text += "\n 4) public void toFalseColors(byte[,,] array3D)   " + stopwatchByteArray3D.ElapsedMilliseconds;
      // Rücktransformation 
      //Bitmap bmp = imgArray.toBitmap(byteArray3D);
      Bitmap bmp = imgArray.toBitmap(buildByte);
      // Visualisierung 
      pictureBox_imgResult.Image = bmp;
      imgTab.viewAsTable(bmp, ARRAY.toCanals(0, 1, 2), "int");
    }
    #endregion

    #region 06. Extras, Bildtabelle und Arraytabellen
    /// <summary>
    /// 06.1 Ausgabe eines Bildes oder eines Arrays als Tabelle mit Intensitätswerten.
    /// </summary>
    private void bildtabelleToolStripMenuItem_Click(object sender, EventArgs e) {
      imgTab.viewAsTable(getSelectedBitmap(), ARRAY.toCanals(0, 1, 2), "float");
      tabControl_IP.SelectedIndex = 4;
    }
    /// <summary>
    ///06.2 Für Kanal 0
    /// </summary>
    private void bildtabelleKanal0ToolStripMenuItem_Click(object sender, EventArgs e) {
      imgTab.viewAsTable(getSelectedBitmap(), ARRAY.toCanals(0), "float");
      tabControl_IP.SelectedIndex = 4;
    }
    /// <summary>
    /// 06.3 Für Kanal 1
    /// </summary>
    private void bildtabelleKanal1ToolStripMenuItem_Click(object sender, EventArgs e) {
      imgTab.viewAsTable(getSelectedBitmap(), ARRAY.toCanals(1), "float");
      tabControl_IP.SelectedIndex = 4;
    }
    /// <summary>
    /// 06.4 Für Kanal 2
    /// </summary>
    private void bildtabelleKanal2ToolStripMenuItem_Click(object sender, EventArgs e) {
      imgTab.viewAsTable(getSelectedBitmap(), ARRAY.toCanals(2), "float");
      tabControl_IP.SelectedIndex = 4;
    }

    /// <summary>
    /// 06.5 Erweitert ein Bild um einen Rand.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void bildPaddingToolStripMenuItem_Click(object sender, EventArgs e) {
      float[,,] array3D = imgArray.toArray3D<float>(getSelectedBitmap());
      // Ruft Sobelfilter auf und berechnet Gradientenbild.
      array3D = ARRAY.padding(array3D);
      // Konvertiert in eine Bitmap zwecks Visualisierung.
      resultBitmap = new Bitmap(imgArray.toBitmap(array3D));
      // Visualisierung 
      output.bitmapShow(resultBitmap, "\nnSobel-Gradienten, x-y-Filter", 3); // Bild
      imgTab.viewAsTable(array3D, ARRAY.toCanals(0), "float", 34, 34); // Bildtabelle
    }

    #endregion

    #region 07. Histogramm
    /// <summary>
    /// 07.1 Erzeugt für ein Bild das Grauwert-Histogramm und stellt dieses dar.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void grayHistogrammToolStripMenuItem_Click(object sender, EventArgs e) {
      // Erzeuge ein 3D-Array 
      byte[,,] array3D = imgArray.toArray3D<byte>(getSelectedBitmap());
      // Diagrammserien initialisieren
      histogram.initChart(histogram.toSeries(0));
      // Erzeugt ein Grauwert-Histogramm
      int[] histoGray = histogram.forGray(array3D);
      // Zeigt das Grauwerthistogramm
      histogram.showChart(histoGray, histogram.toSeries(0));
      // Schlägt Diagramm-Registerseite auf
      tabControl_IP.SelectedIndex = 5;

    }

    /// <summary>
    /// 07.2 Erzeugt für ein Bild das RGB-Histogramm und stellt dieses dar.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void rGBHistogrammToolStripMenuItem_Click(object sender, EventArgs e) {
      // Erzeuge ein 3D-Array 
      byte[,,] array3D = imgArray.toArray3D<byte>(getSelectedBitmap());
      // Invertieren
      //pixOp.toInvert(ref array3D);
      // Diagrammserien initialisieren
      histogram.initChart(histogram.toSeries(1, 2, 3), SeriesChartType.Spline);
      // Erzeugt RGB-Histogramm h=[c,I]
      int[,] histoRGB = histogram.forRGB(array3D);
      // Zeigt das RGB-Histogramm 
      histogram.showChart(histoRGB, histogram.toSeries(1, 2, 3));
      // Schlägt Diagramm-Registerseite auf
      tabControl_IP.SelectedIndex = 5;
    }
    /// <summary> ERWEITERUNG ZU 07.2
    /// 07.2b Erzeugt für ein Bild sowohl das RGB-Histogramm als auch
    /// das Grauwerthistogramm und stellt beide dar.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void grayRGBHistogrammToolStripMenuItem_Click(object sender, EventArgs e) {
      // Erzeuge ein 3D-Array 
      byte[,,] array3D = imgArray.toArray3D<byte>(getSelectedBitmap());
      // Diagrammserien initialisieren
      histogram.initChart(histogram.toSeries(0, 1, 2, 3), SeriesChartType.StackedColumn);
      // Erzeugt RGB-Histogramm h=[c,I]
      int[,] histoRGB = histogram.forRGB(array3D);
      // Zeigt das RGB-Histogramm 
      histogram.showChart(histoRGB, ARRAY.toSeries(1, 2, 3));
      // Erzeugt ein Grauwert-Histogramm
      int[] histoGray = histogram.forGray(array3D);
      // Fügt das Grauwerthistogramm dazu.
      histogram.showChart(histoGray, ARRAY.toSeries(0));
      // Schlägt Diagramm-Registerseite auf
      tabControl_IP.SelectedIndex = 5;
    }

    /// <summary>
    /// 07.3 Löscht das Histogramm
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void histogrammpunkteLöschenToolStripMenuItem_Click(object sender, EventArgs e) {
      histogram.clearChart();
    }
    #endregion

    #region 08. Pixeloperationen
    // In dieser Region zur Pixeltransformation werden eindimensionale Felder
    // mit dem Elementetyp „float“ im Intervall von [0, 1] verwendet.

    /// <summary>
    /// 08.1 Attribut, Variable dieser Region.
    /// "build" ist eine Strukturvariable. Sie enthält ein eindimensionales Feld
    /// und die Gestalt "shape" des assoziativen Bildes.
    /// </summary>
    ImageArrayOwn.BuildType<float> build;

    /// <summary>
    /// 08.2 Grauwertbild erzeugen
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void grauwertbildToolStripMenuItem_Click(object sender, EventArgs e) {
      // Transformation einer Bitmaps in eine Build-Struktur
      build = imgArray.toBuild<float>(getSelectedBitmap());
      // Pixeloperation zur Grauwerttransformation aufrufen.
      pixOp.toGray(ref build.feld1D); // Pixel umwandeln nach Grau.
      // VISUALISIERUNG
      // Darstellung des Ergebnisbildes in eine Bildbox
      resultBitmap = new Bitmap(imgArray.toBitmap(build));
      output.bitmapShow(build, "Pixeloperation: Grauwertbild", 3);   // Gemeinsame Ausgaben
      imgTab.viewAsTable(imgArray.toBitmap<float>(build), ARRAY.toCanals(0), "hexa");    // Ausgabe als Bildtabelle
    }

    /// <summary>
    /// 08.3 Binärbild erzeugen
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void binaerbildToolStripMenuItem_Click(object sender, EventArgs e) {
      build = imgArray.toBuild<float>(getSelectedBitmap());
      pixOp.toBinaer(ref build.feld1D, 0.3); // Binärbild erzeugen
      resultBitmap = new Bitmap(imgArray.toBitmap(build));
      output.bitmapShow(build, "Pixeloperation: Binärbild", 3);
    }

    /// <summary>
    /// 08.4 Imvertiert ein Bild
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void invertiertesBildToolStripMenuItem_Click(object sender, EventArgs e) {
      build = imgArray.toBuild<float>(getSelectedBitmap());
      pixOp.toInvert(ref build.feld1D);// Invertiert Bild
      // Konvertieren in eine Bitmap zwecks Visualisierung.
      resultBitmap = new Bitmap(imgArray.toBitmap(build));
      output.bitmapShow(build, "Pixeloperation: Invertiertes Bild", 3);
    }

    /// <summary>
    /// 08.5 Intensitätsformung
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void intensitätsformungToolStripMenuItem_Click(object sender, EventArgs e) {
      build = imgArray.toBuild<float>(getSelectedBitmap());
      richTextBox_Info.Text += String.Format(" Basisbild - Intensität, Min: {0}, Max: {1}", build.feld1D.Min(), build.feld1D.Max());
      pixOp.sigmoidFormung(ref build.feld1D, 1, 5);
      pixOp.linearSpreizung(ref build.feld1D);        // Aufteilung auf das gesamten Grauwertspektrums
      resultBitmap = new Bitmap(imgArray.toBitmap(build));
      output.bitmapShow(build, String.Format(" Intensitätsformung - Intensität, Min: {0}, Max: {1}", build.feld1D.Min(), build.feld1D.Max()), 3);
    }

    /// <summary>
    /// 08.6 Erzeugt ein Falschfarbenbild aus einem Grauwertbild. Ein Farbbild wird
    /// intern vorab in ein Grauwertbild umgewandelt.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void falschfarbenbildToolStripMenuItem_Click(object sender, EventArgs e) {
      build = imgArray.toBuild<float>(getSelectedBitmap());
      pixOp.toFalseColors(build.feld1D, 3, 0.5);
      resultBitmap = new Bitmap(imgArray.toBitmap(build));
      output.bitmapShow(build, String.Format("Falschfarbendarstellung - Intensität, Min: {0}, Max: {1}", build.feld1D.Min(), build.feld1D.Max()), 3);
    }

    /// <summary>
    /// 08.7 Kombiniert mehrer Pixeloperationen zur Verabeitung eines Bildes.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void kombinationenToolStripMenuItem_Click(object sender, EventArgs e) {
      build = imgArray.toBuild<float>(getSelectedBitmap());
      pixOp.sigmoidFormung(ref build.feld1D, 0.4, 8);  // Grauwerttransformation entsprechend der Sigmoidfumktion
      pixOp.linearSpreizung(ref build.feld1D); // Teilt Intensitätswerte auf den verfügbaren Intervall auf.
      pixOp.toFalseColors(build.feld1D, 3, 0.7); // Falschfarbendarstellung eines Grauwertbildes
      resultBitmap = new Bitmap(imgArray.toBitmap(build));
      output.bitmapShow(build, String.Format("Grauwerte nach Sigmoid - Intensität, Min: {0}, Max: {1}", build.feld1D.Min(), build.feld1D.Max()), 3);
    }

    /// <summary>
    /// 08.8 Binärbild Schwarz-Weiß
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void binaerbildSWToolStripMenuItem_Click(object sender, EventArgs e) {
      build = imgArray.toBuild<float>(getSelectedBitmap());
      pixOp.toGray(ref build.feld1D); // Pixeltrans to Gray mit überladener Methode
      pixOp.toBinaer(ref build.feld1D, 0.7); // Binärbild erzeugen
      resultBitmap = new Bitmap(imgArray.toBitmap(build));
      output.bitmapShow(build, "Pixeloperationen: Schwarz-Weiß-Bild", 3);
    }
    #endregion

    #region 09. Eigenschaften von Einzelobjekten eines Bildes
    /// <summary>
    /// 09.1 Erzeugt ein Binärbild des Bildes der aufgeschlagenen Registerseite
    /// und legt es als Ergebnisbild ab. Der Hintergrund soll den Intensitätswert I=0
    /// und das Objekt den Intensitätswert I=1 erhalten.
    /// </summary>
    private void binärbildThreashold01ToolStripMenuItem_Click(object sender, EventArgs e) {
      // Grenzwert zum Erzeugen eines Schwarz-Weiß-Bildes.
      float threashold = (float)Convert.ToDouble(toolStripTextBox_Threshold.Text);
      // Build mit eindimensinalem Feld des Bildes der aufgeschlagenen Registerseite.
      build = imgArray.toBuild<float>(getSelectedBitmap());
      pixOp.toGray(ref build.feld1D);      // Graufeld erzeugen
      pixOp.toBinaer(ref build.feld1D, threashold); // Binärfeld erzeugen
      pixOp.toInvert(ref build.feld1D); // Invertieren -> I(Hintergrund) = 0,  I(Objekt) > 0 , =1
      // Konvertieren in eine Bitmap zwecks Visualisierung.
      resultBitmap = new Bitmap(imgArray.toBitmap(build));
      // Visualisierung 
      output.bitmapShow(resultBitmap, "Binärbild", 3);
    }

    /// <summary>
    /// 09.2 Begrenzungsrechteck anders gesagt umrahmendes Rechteck ermitteln.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void begrenzungsrechteckToolStripMenuItem_Click(object sender, EventArgs e) {
      //float[,,] array3D = imgArray.toArray3D<float>(getSelectedBitmap());    // Array 3D
      build = imgArray.toBuild<float>(resultBitmap); // mit BuildType<float>  build.feld1D //
      Rectangle rectangle = feature.rechteck(build, 1);
      //Ausgabe
      // Übernimmt das Rechteck in die Bitmap
      output.addRect(resultBitmap, rectangle, 3);
      output.addText(resultBitmap, new Point(0, 0), "# " + rectangle + " Pixel");
      output.bitmapShow(resultBitmap, "Begrenzungsrechteck", 3);
      String[] text = { String.Format("Begrenzungsrechteck alle Objekte der Darstellung: {0} Pixel", rectangle) };
      output.textBoxText(text); //Ausgabe als Information in die Richt-Text-Box
    }

    /// <summary>
    /// 09.4 Fläche eines Objektes berechnen und Ausgabe der Werte.
    /// </summary>
    private void flächeToolStripMenuItem_Click(object sender, EventArgs e) {
      // Bild als Build-Struktur
      build = imgArray.toBuild<float>(resultBitmap);
      // Liefert Pixelanzahl des Abbildes des Gegenstandes (Segmentfläche).
      int a_Ap = feature.flaeche(build.feld1D, 1);
      // Abstand der Kamera zum Gegenstand.
      float g = Convert.ToSingle(toolStripTextBox_g.Text);
      // Umrechnung von Pixel in mm².
      float A_g = feature.flaecheSi(a_Ap, g, cam.getParaMSLifeCam_VX2000);
      // Umrechnung von Pixel in mm².
      float A_g_Alpha = feature.flaecheSi_Alpha(a_Ap, g, cam.getParaMSLifeCam_VX2000);
      // Ausgabe
      String[] text = new String[3];
      text[0] = String.Format("Fläche des Gegegenstandes (A_g< mit Betrachtungswinkel berechnet)");
      text[1] = String.Format("g: {0} mm, ap: {1} Pixel, Ag: {2:F0} mm², (Ag<: {3:F0} mm²)", g, a_Ap, A_g, A_g_Alpha);
      output.addText(resultBitmap, new Point(0, 0), text[1], 16); // Ausgabe als Grafiktext
      output.bitmapShow(resultBitmap, "Fläche", 3);
      output.textBoxText(text); //Ausgabe als Information in die Richt-Text-Box
    }

    /// <summary>
    /// 09.5 Schwerpunkt eines Objektes
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void schwerpunktToolStripMenuItem_Click(object sender, EventArgs e) {
      // Konvertiert Bitmap in ein 3D-Array.
      float[,,] array3D = imgArray.toArray3D<float>(resultBitmap);
      PointF schwerpunkt = feature.flaechenschwerpunkt(array3D, 0, 1);
      // VISUALISIERUNG
      string[] text = new string[2];
      text[0] = "Schwerpunkt:";
      text[1] = String.Format(" Sx= {0:f}, Sy= {1:f} Pixel", schwerpunkt.X, schwerpunkt.Y);
      output.addPoint(resultBitmap, schwerpunkt, 5);
      output.addText(resultBitmap, new Point(0, 0), text[1], 16);
      output.bitmapShow(resultBitmap, "Binärbild", 3);      // Visualisierung 
      output.textBoxText(text); //Ausgabe als Information in der Richt-Text-Box
    }

    /// <summary>
    /// 09.6 Orientierung und Schwerpunkt eines Objektes.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void orientierungToolStripMenuItem_Click(object sender, EventArgs e) {
      // Konvertiert Bitmap in ein 3D-Array.
      float[,,] array3D = imgArray.toArray3D<float>(resultBitmap);
      PointF schwerpunkt = feature.flaechenschwerpunkt(array3D, 0, 1);
      float gamma = feature.orientierung(array3D, schwerpunkt, 0, 1);
      // VISUALISIERUNG
      string[] text = new string[2];
      text[0] = "Schwerpunkt und Orientierung (Im Uhrzeigersinn positiv)";
      text[1] = String.Format(" Sx= {0:F1} | Sy= {1:F1} Pixel | Gamma= {2:F1}°", schwerpunkt.X, schwerpunkt.Y, gamma * 180 / Math.PI);
      output.addPoint(resultBitmap, schwerpunkt, 5);
      output.addPointer(resultBitmap, schwerpunkt, gamma, 40, 3);
      output.addText(resultBitmap, new Point(0, 0), text[1], 16);
      output.bitmapShow(resultBitmap, "Binärbild", 3); // Visualisierung
      output.textBoxText(text);
    }

    /// <summary>
    /// 09.7 Äußerer Umfang eines Gegenstandes ermitteln.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void umfangToolStripMenuItem_Click(object sender, EventArgs e) {
      // Ereignismethode: Erzeugt ein Binärbild mit dem im Menü
      // editierbaren Schwellwert.
      binärbildThreashold01ToolStripMenuItem_Click(sender, e);
      // Bildtransformation
      float[,,] array3D = imgArray.toArray3D<float>(resultBitmap);
      // Kettencode als Liste und als referenziertes Array
      // (Kanal 0: Original-Binärbild, Kanal 1: Segmentfläche, Kanal 2: Segmentumrandung)
      List<uint> kettencode = feature.chaincode(ref array3D, 1, 1);
      pixOp.linearFormung(ref array3D, 1); // Werte in Farbbereich verschieben.
      pixOp.linearFormung(ref array3D, 2);
      // Abstand der Kamera zum Gegenstand wird benötigt.
      float g = Convert.ToSingle(toolStripTextBox_g.Text);
      // Umfang der Flächen in mm
      float uSi = feature.umfangSi(kettencode, g, cam.getParaMSLifeCam_VX2000);
      // VISUALISIERUNG
      // Zusammenstellung des Ausgabetextes
      int n = 3; // Für alle Objekte
      string[] text = new string[n];
      text[0] = string.Format("Kettenkode");
      text[1] = string.Format("u[{0:d}]= {1:d} Pixel, {2:f} mm, Startpunkt x= {3:d}, y= {4:d}, Kette:",
      1, kettencode.Count - 2, uSi, kettencode[0], kettencode[1]);
      for (int j = 2; j < kettencode.Count; j++)
        text[2] += kettencode[j];
      // Konvertiert Array in ein Bitmap und ergänzt Grafiktext.
      resultBitmap = new Bitmap(imgArray.toBitmap(array3D));
      for (int i = 1; i < text.Length; i++)
        output.addText(resultBitmap, new Point(0, 18 * i - 12), text[i], 16);
      output.bitmapShow(resultBitmap, "Umfang", 3);
      output.textBoxText(text); //Ausgabe als Information in der Richt-Text-Box
    }

    /// <summary>
    /// 09.8 Projekt Pneumatikzylinder
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void projektPneumatikzylinderToolStripMenuItem_Click(object sender, EventArgs e) {
      // Ereignismethode: Erzeugt ein Binärbild mit dem im Menü
      // editierbaren Schwellwert.
      binärbildThreashold01ToolStripMenuItem_Click(sender, e);
      // Bitmap in 3D-Array konvertieren
      float[,,] array3D = imgArray.toArray3D<float>(resultBitmap);
      // Abstand der Kamera zum Gegenstand wird benötigt.
      float g = Convert.ToSingle(toolStripTextBox_g.Text);
      // Liefert die Fläche des Gegenstandes (Objektfläche) in Pixel.
      int a_Ap = feature.flaeche(build.feld1D, 1);
      // Umrechnung der Fläche von Pixel in mm².
      float A_g = feature.flaecheSi(a_Ap, g, cam.getParaMSLifeCam_VX2000);
      // Liefert den Schwerpunkt der Fläche des Pneumatikzylinders
      PointF schwerpunkt = feature.flaechenschwerpunkt(array3D, 0, 1);
      // Liefert den Orientierungswinkel des Objektes.
      float gamma = feature.orientierung(array3D, schwerpunkt, 0, 1);
      List<uint> kettencode = feature.chaincode(ref array3D, 1, 1);
      // Umrechnung des Umfangs von Pixel in mm,
      float uSi = feature.umfangSi(kettencode, g, cam.getParaMSLifeCam_VX2000);
      // VISUALISIERUNG
      // Text zusammenstellen
      String[] text = new String[4];
      text[0] = String.Format("Projekt: Pneumatikzylinder, Abstand zu Kamere d= {0} ", g);
      text[1] = String.Format("Fläche ap= {0} Pixel, Ag: {1:F0} mm²", a_Ap, A_g);
      text[2] = string.Format("Umfang u= {0:d} Pixel, {1:f} mm, Start x= {2:d}, y= {3:d} Pixel",
                               kettencode.Count - 2, uSi, kettencode[0], kettencode[1]);
      text[3] = String.Format("Schwerpunkt: Sx= {0:F1} | Sy= {1:F1} Pixel | Orientierung: Gamma= {2:F1}°",
                                schwerpunkt.X, schwerpunkt.Y, gamma * 180 / Math.PI);
      // Konvertiert die Werte der Kanalebenen 1 und 2 in das Intervall [0, 1]
      pixOp.linearFormung(ref array3D, 1); // Farbintervall
      pixOp.linearFormung(ref array3D, 2);
      // Konvertiert Array in eine Bitmap und ergänzt Grafiktext.
      resultBitmap = new Bitmap(imgArray.toBitmap(array3D));
      output.addPoint(resultBitmap, schwerpunkt, 5);
      output.addPointer(resultBitmap, schwerpunkt, gamma, 40, 3);
      for (int i = 1; i < text.Length; i++) // Grafiktext
        output.addText(resultBitmap, new Point(0, 18 * i - 12), text[i]);
      // Bild in einer Bildbox darstellen
      output.bitmapShow(resultBitmap, "Projekt: Pneumatikzylinder", 3);
      // Text in eine Textbox schreiben
      output.textBoxText(text);
    }

    #endregion

    #region 10. Nachbarschaftsoperationen - Tiefpassfilter, Hochpassfilter, Rangordnungsfilter, Morphologische Operationen
    #region 10.1 - 10.3 Lineare Filter, Teil: Tiefpassfilter 
    /// <summary>
    /// 10.1 Mittelwertfilter mit einem Kernel von 3 x 3 Pixel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void mittelwertfilterToolStripMenuItem_Click(object sender, EventArgs e) {
      // Konvertiert Bitmap in ein 3D-Array.
      float[,,] array3D = imgArray.toArray3D<float>(getSelectedBitmap());
      // Rand mit der Breite eines Pixels und dem Wert 0 hinzufügen.
      array3D = ARRAY.padding(array3D, 1, 0);
      // Mittelwertfilter aufrufen
      array3D = neighborOp.toAverage(array3D);
      // VISUALISIERUNG 
      // Konvertieren in eine Bitmap zwecks Visualisierung.
      resultBitmap = new Bitmap(imgArray.toBitmap(array3D));
      // als Bild 
      output.bitmapShow(resultBitmap, "Mittelwertfilter", 3);
      // Als Bildtabell
      imgTab.viewAsTable(array3D, ARRAY.toCanals(0), "float");
    }

    /// <summary>
    /// 10.2 Binomialfilter mit einem Kernel 3 x 3
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void binomialfilterToolStripMenuItem_Click(object sender, EventArgs e) {
      // Konvertiert Bitmap in ein 3D-Array.
      float[,,] array3D = imgArray.toArray3D<float>(getSelectedBitmap());
      // Aufruf eines linearen Filters mit der Binomialmatrix.
      array3D = neighborOp.linearFilter(array3D, neighborOp.B3x3_Binomial(), ARRAY.toCanals(0, 1, 2), 0, 1);
      // VISUALISIERUNG 
      // Konvertieren in eine Bitmap zwecks Visualisierung.
      resultBitmap = new Bitmap(imgArray.toBitmap(array3D));
      // Als Bild
      output.bitmapShow(resultBitmap, "Binomialfilter", 3);
      // Kanal 0 des Arrays als Bildtabelle.
      imgTab.viewAsTable(array3D, ARRAY.toCanals(0), "float");
    }

    /// <summary>
    /// 10.3 Gaußfilter
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void gaußfilterToolStripMenuItem_Click(object sender, EventArgs e) {
      // Konvertiert Bitmap in ein 3D-Array.
      float[,,] array3D = imgArray.toArray3D<float>(getSelectedBitmap());
      // Ausgabe der Matrixwerte auf der Informationsseite.
      output.textBoxMatrix(neighborOp.G_Gauß(3));
      // Aufruf des linearen Gaußfilters.
      array3D = neighborOp.linearFilter(array3D, neighborOp.G_Gauß(3), ARRAY.toCanals(0, 1, 2), 0, 1);
      // VISUALISIERUNG 
      // Konvertieren in eine Bitmap zwecks Visualisierung.
      resultBitmap = new Bitmap(imgArray.toBitmap(array3D));
      // Als Bild
      output.bitmapShow(resultBitmap, "\nGaußfilter", 4);
      // Kanal 0 des Arrays als Bildtabelle.
      imgTab.viewAsTable(array3D, ARRAY.toCanals(0), "float");
    }
    #endregion

    #region 10.4 - 10.8 Lineare Filter, Teil Hochpassfilter
    /// <summary>
    /// 10.4 Sobelfilter für die x-Richtung
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void sobelXFilterToolStripMenuItem_Click(object sender, EventArgs e) {
      // Konvertiert Bitmap in ein 3D-Array.
      float[,,] array3D = imgArray.toArray3D<float>(getSelectedBitmap());
      output.textBoxMatrix(neighborOp.Sx_Sobel_x());
      array3D = neighborOp.linearFilter(array3D, neighborOp.Sx_Sobel_x(), ARRAY.toCanals(0, 1, 2), 0, 1);
      // Konvertieren in eine Bitmap zwecks Visualisierung.
      resultBitmap = new Bitmap(imgArray.toBitmap(array3D));
      //+ resultBitmap = new Bitmap(imgArray.toBitmap(ArrayOwn.normalizationArray3DCanal(array3D)));
      // Visualisierung 
      output.bitmapShow(resultBitmap, "\nSobel-x-Filter", 3);
      imgTab.viewAsTable(array3D, ARRAY.toCanals(0), "float");
    }
    /// <summary>
    /// 10.5 Sobelfilter für die y-Richtung
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void sobelYFilterToolStripMenuItem_Click(object sender, EventArgs e) {
      // Konvertiert Bitmap in ein 3D-Array.
      float[,,] array3D = imgArray.toArray3D<float>(getSelectedBitmap());
      output.textBoxMatrix(neighborOp.Sy_Sobel_y());
      array3D = neighborOp.linearFilter(array3D, neighborOp.Sy_Sobel_y(), ARRAY.toCanals(0, 1, 2), 0, 1);
      // Konvertieren in eine Bitmap zwecks Visualisierung.
      resultBitmap = new Bitmap(imgArray.toBitmap(array3D));
      //+ resultBitmap = new Bitmap(imgArray.toBitmap(ArrayOwn.normalizationArray3DCanal(array3D)));
      // Visualisierung 
      output.bitmapShow(resultBitmap, "\nSobel-y-Filter", 3);
      imgTab.viewAsTable(array3D, ARRAY.toCanals(0), "float");
    }
    /// <summary>
    /// 10.6 Sobel-Gradientenbild
    /// Normierte Resultierende der X- und der Y-Richtung.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void sobelGradientXyToolStripMenuItem_Click(object sender, EventArgs e) {
      // Nehme das Bild der aktuell sichtbaren Picturebox
      // (Alternative zum zwischengespeichertem Bitmap) 
      // Bitmap bitmap = getSelectedPictureBoxImage();
      // Konvertiert Bitmap in ein 3D-Array.
      float[,,] array3D = imgArray.toArray3D<float>(getSelectedBitmap());
      // Ruft Sobelfilter auf und berechnet Gradientenbild.
      neighborOp.sobelFilter(ref array3D, neighborOp.Sx_Sobel_x(), neighborOp.Sy_Sobel_y(), ARRAY.toCanals(0, 1, 2));
      // VISUALISIERUNG 
      // Normierung
      ARRAY.normalizationArray3D(ref array3D);
      // Konvertiert in eine Bitmap zwecks Visualisierung.
      resultBitmap = new Bitmap(imgArray.toBitmap(array3D));
      // Bild
      output.bitmapShow(resultBitmap, "\nnSobel-Gradienten, x-y-Filter", 3);
      // Bildtabelle
      imgTab.viewAsTable(array3D, ARRAY.toCanals(0), "float", 16, 16);
    }

    /// <summary>
    /// 10.7 Laplacefilter L
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void laplaceFilterToolStripMenuItem_Click(object sender, EventArgs e) {
      // Konvertiert Bitmap in ein 3D-Array.
      float[,,] array3D = imgArray.toArray3D<float>(getSelectedBitmap());
      // Ruft das Laplacefilter auf.
      // (Zentralelement =8, Arrayerweiterung mit dem Wert 0 mit der Breite 1)
      array3D = neighborOp.linearFilter(array3D, neighborOp.L8_Laplace(8), ARRAY.toCanals(0, 1, 2), 0, 1);
      //+ Nachbearbeitung zur Detektion des Vorzeichenwechsels
      array3D = ARRAY.signChange(array3D, ARRAY.ahead8);
      // Visualisierung 
      resultBitmap = new Bitmap(imgArray.toBitmap(array3D));
      output.bitmapShow(resultBitmap, "\nLaplace-Hochpassfilter", 3); // Bild
      imgTab.viewAsTable(array3D, ARRAY.toCanals(0), "float"); // Bildtabelle
    }
    /// <summary>
    /// 10.8 Laplace-Gauß-Filter (Laplacian-of-Gaussian-Filter - Log-Filter)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void laplacianOfGaussianFilterToolStripMenuItem_Click(object sender, EventArgs e) {
      // Konvertiert Bitmap in ein 3D-Array.
      float[,,] array3D = imgArray.toArray3D<float>(getSelectedBitmap());
      output.textBoxMatrix(neighborOp.LG_Laplacian_of_Gaussian(5));
      array3D = neighborOp.linearFilter(array3D, neighborOp.LG_Laplacian_of_Gaussian(5), ARRAY.toCanals(0, 1, 2), 0, 1);
      // Konvertieren in eine Bitmap zwecks Visualisierung.
      resultBitmap = new Bitmap(imgArray.toBitmap(array3D));
      // Visualisierung 
      output.bitmapShow(resultBitmap, "\nLaplacian-of-Gaussian-Filter", 3);
      imgTab.viewAsTable(array3D, ARRAY.toCanals(0), "float");
    }
    #endregion

    #region 10.9 . 10.11 Rangordnungsfilter -------------------
    /// <summary>
    /// 10.9 Rangordnungsfilter: Median
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void medianfilterToolStripMenuItem_Click(object sender, EventArgs e) {
      // Konvertiert Bitmap in ein 3D-Array.
      float[,,] array3D = imgArray.toArray3D<float>(getSelectedBitmap());
      // Rangordnungsfilter, Median der Größe 3 x 3 Pixel
      array3D = neighborOp.rankFilter(array3D, neighborOp.median, ARRAY.toCanals(0, 1, 2), 3, 0, 1);
      // VISUALISIERUNG
      // Fehlermeldung
      richTextBox_Info.Text += ($"\n{neighborOp.getException}");
      // Konvertieren in eine Bitmap
      resultBitmap = new Bitmap(imgArray.toBitmap(array3D));
      // Als Bild
      output.bitmapShow(resultBitmap, "\nMedianfilter", 3);
      // Als Bildtabelle
      imgTab.viewAsTable(array3D, ARRAY.toCanals(0), "float");
    }

    /// <summary>
    /// 10.10 Rangordnungsfilter Maximum, MaxPool: size=2, step=2
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void maximumfilterToolStripMenuItem_Click(object sender, EventArgs e) {
      // Konvertiert Bitmap in ein 3D-Array.
      float[,,] array3D = imgArray.toArray3D<float>(getSelectedBitmap());
      // Rangordnungsfilter mit Parametrierung für "MaxPool"
      array3D = neighborOp.rankFilter(array3D, neighborOp.maximum, ARRAY.toCanals(0, 1, 2), 2, 0, 2);
      // VISUALISIERUNG
      // Fehlermeldung
      richTextBox_Info.Text += ($"\n{neighborOp.getException}");
      // Konvertiert Array in ein Bitmap zwecks Visualisierung.
      resultBitmap = new Bitmap(imgArray.toBitmap(array3D));
      // Visualisierung 
      output.bitmapShow(resultBitmap, "\nMaximum-Filter", 3); // Als Bild
      imgTab.viewAsTable(array3D, ARRAY.toCanals(0), "float"); // Als Bildtabelle
    }

    /// <summary>
    /// 10.11 Rangordnungsfilter: Minimum
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void minimumfilterToolStripMenuItem_Click(object sender, EventArgs e) {
      // Konvertiert Bitmap in ein 3D-Array.
      float[,,] array3D = imgArray.toArray3D<float>(getSelectedBitmap());
      array3D = neighborOp.rankFilter(array3D, neighborOp.minimum, ARRAY.toCanals(0, 1, 2), 3, 0, 1);
      // Fehlermeldung
      richTextBox_Info.Text += ($"\n{neighborOp.getException}");
      // Konvertiert Array in ein Bitmap zwecks Visualisierung.
      resultBitmap = new Bitmap(imgArray.toBitmap(array3D));
      // Visualisierung 
      output.bitmapShow(resultBitmap, "\nMinimum-Filter", 3);
      imgTab.viewAsTable(array3D, ARRAY.toCanals(0), "float");
    }
    #endregion

    #region 10.12 - 10.13  Morphologische Operationen -------------------------
    /// <summary>
    /// 10.12 Erosion
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void erosionToolStripMenuItem_Click(object sender, EventArgs e) {
      // Konvertiert Bitmap in ein 3D-Array.
      float[,,] array3D = imgArray.toArray3D<float>(getSelectedBitmap());
      // Erosion
      // (Bild-Array, Kanäle ,Id-Dilatation, Id-Erosion, Strukturmatrixgröße)
      array3D = neighborOp.morphologie(array3D, ARRAY.toCanals(0), neighborOp.M_Morph(3, 0, 1));
      // array3D = neighborOp.morphologie(ref array3D, ARRAY.toCanals(0), neighborOp.ME_Erosion5_1(1));
      // VISUALISERUNG
      // Konvertiert Array in eine Bitmap.
      resultBitmap = new Bitmap(imgArray.toBitmap(array3D));
      // Als Bild
      output.bitmapShow(resultBitmap, "\nErosion", 3);
      // Als Bildtabelle
      imgTab.viewAsTable(array3D, ARRAY.toCanals(0), "float");
    }

    /// <summary>
    /// 10.13 Dialatation
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void dilatationToolStripMenuItem_Click(object sender, EventArgs e) {
      // Konvertiert Bitmap in ein 3D-Array.
      float[,,] array3D = imgArray.toArray3D<float>(getSelectedBitmap());
      // Dilatation der Segmentflächen mit der Id=1
      array3D = neighborOp.morphologie(array3D, ARRAY.toCanals(0), neighborOp.M_Morph(3, 1, 0));
      // VISUALISERUNG
      // Konvertiert Array in eine Bitmap zwecks Visualisierung.
      resultBitmap = new Bitmap(imgArray.toBitmap(array3D));
      // // Als Bild 
      output.bitmapShow(resultBitmap, "\nErosion", 3);
      // Als Bildtabelle
      imgTab.viewAsTable(array3D, ARRAY.toCanals(0), "float");
    }
    #endregion

    #region 10.14 - 10-15 Identifikation, Identnummern
    /// <summary>
    /// 10.14 Identnummern für Segmentflächen auf Grundlage der Konturierung mit dem kettencode.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void idByChainCodeToolStripMenuItem_Click(object sender, EventArgs e) {
      // Bildtransformation
      float[,,] array3D = imgArray.toArray3D<float>(getSelectedBitmap());
      // Freemancode und Umfang mehrerer Segmente (Objekte) in einer Liste. Hier muss das Array referenziert werden.
      List<List<uint>> kettencodes = ident.chaincodes(ref array3D, 1);
      // VISUALISIERUNG
      // Ausgabetexte zusammenstellen
      int n = 1 + kettencodes.Count; // Für alle Objekte
      string[] text = new string[n];
      text[0] = string.Format("Kettenkode");
      for (int i = 1; i < kettencodes.Count; i++) {
        text[i] = string.Format("u[{0:d}]= {1:d} Pixel, Startpunkt x= {2:d}, y= {3:d}, Kette:",
        i, kettencodes[i].Count - 2, kettencodes[i][0], kettencodes[i][1]);
        for (int j = 2; j < kettencodes[i].Count; j++)
          text[i] += kettencodes[i][j];
        text[i] += "\n";
      }
      // Bildtabelle: Der Kanal 1 enthält die Id's der Segmentflächen
      imgTab.viewAsTable(array3D, ARRAY.toCanals(1), "float"); // Als Bildtabelle
      // Konvertiert die Arraywerte in ein farblich darstellbares Intervall.
      ARRAY.normalizationArray3D(ref array3D, ARRAY.getIntervall(0, 1));
      // Konvertiert Array in eine Bitmap.
      resultBitmap = new Bitmap(imgArray.toBitmap(array3D));
      // Darstellung als Bild
      output.bitmapShow(resultBitmap, "nIdentnummern", 3);
      // Informationen in der Richt-Text-Box.
      output.textBoxText(text);
    }

    /// <summary>
    /// 10.15 Identnummernvergabe für Segmentflächen durch Auswertung der
    /// nachbarschaftlichen Beziehungen.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void idByNeighborRelationsToolStripMenuItem_Click(object sender, EventArgs e) {
      // Bildtransformation
      float[,,] array3D = imgArray.toArray3D<float>(getSelectedBitmap());
      // Liefert die Kontur der Segmentflächen mit dem Wert 1 auf dem Kanal 2.
      ident.contour(ref array3D, 1);
      // Gibt den Segmentflächen eine fortlaufende ID.
      ident.segments(ref array3D, ARRAY.toPoints(ARRAY.backwardShift), 1);
      // VISUALISIERUNG
      //Bildtabelle: Der Kanal 1 enthält die Id's der Segmentflächen
      imgTab.viewAsTable(array3D, ARRAY.toCanals(1), "float");
      // Konvertiert die Arraywerte in ein farblich darstellbares Intervall.
      ARRAY.normalizationArray3D(ref array3D, ARRAY.getIntervall(0, 1));
      // Konvertiert ein Array in eine Bitmap.
      resultBitmap = new Bitmap(imgArray.toBitmap(array3D));
      // Darstellung als Bild.
      output.bitmapShow(resultBitmap, "nIdentnummern", 3);

    }

    /// <summary>
    /// 10.16 Identnummernzuweisung bei sich berührenden Segmentflächen.
    /// (Sich berührende Segmentflächen unterscheiden.)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void idForTouchingSegmentsToolStripMenuItem_Click(object sender, EventArgs e) {
      // Konvertiert Bitmap in ein 3D-Array. Bild ist binär.
      float[,,] array3D = imgArray.toArray3D<float>(getSelectedBitmap());
      // Erstellt Kopie des Kanal 0 zum späteren Vergleich.
      float[,] arrayKanal = ARRAY.copy(array3D, 0);
      // Erosion, um Objekte zu trennen. 
      neighborOp.erosion(ref array3D, ARRAY.toCanals(0), 1, 0, 3, 1);
      // Objekte Identifizieren, Hintergrund-Id: 0, Objekte mit Id: 1, 2, 3
      // Kanal 0: Original-ID, Kanal 1: Segmentflächen-ID, Kanal 2: Umrandungs-Id
      // Liefert Freemancode mehrerer Segmentflächen als Liste und referenziertes Array
      // mit Objekt-Id's im Kanal 1 sowie Umrandung im Kanal 2.
      List<List<uint>> kettencodes = ident.chaincodes(ref array3D, 1);
      // Anzahl der Objekte. Id=0 ist Hintergrund und wird nicht betrachtet.
      int objectCount = kettencodes.Count - 1; //int objectCount >= 1;
      // Segmentflächen vergrößern mit Überschreitung der Originalgrößen.
      // Deshalb zweimalige Ausführung.
      neighborOp.dilatation(ref array3D, ARRAY.toCanals(1), objectCount, 0, 3, 1);
      neighborOp.dilatation(ref array3D, ARRAY.toCanals(1), objectCount, 0, 3, 1);
      // Abgleich mit den Originalgrößen der Segmentflächen.
      neighborOp.isObject(ref array3D, arrayKanal);


      // VISUALISIERUNG
      // Bildtabelle: Der Kanal 1 enthält die Id's der Segmentflächen.
      imgTab.viewAsTable(array3D, ARRAY.toCanals(1), "float");
      // Konvertiert die Arraywerte in ein farblich darstellbares Intervall.
      ARRAY.normalizationArray3D(ref array3D, ARRAY.getIntervall(0, 1));
      // Konvertiert Array in eine Bitmap.
      resultBitmap = new Bitmap(imgArray.toBitmap(array3D));
      // Darstellung als Bild.
      output.bitmapShow(resultBitmap, "\nIdentnummern berührender Segmente", 3);
    }
    #endregion

    #region 10.17 - 10.19 Projekte mit Nachbarn
    /// <summary>
    /// 10.17 Normteile durchnummerieren 
    /// unter Verwendung des Kettencodes.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void idForStandardPartsToolStripMenuItem_Click(object sender, EventArgs e) {
      // Konvertiert Bitmap in ein 3D-Array
      float[,,] array3D = imgArray.toArray3D<float>(getSelectedBitmap());
      // Grauwertbild erzeugen
      pixOp.toGray(ref array3D);
      // Grauwertbild invertieren
      pixOp.toInvert(ref array3D);
      // Intensitätsformung, Verstärkung des Kontrastes
      pixOp.sigmoidFormung(ref array3D, 0, 4);
      // Aufteilung auf das gesamten Grauwertspektrum
      pixOp.linearSpreizung(ref array3D);
      // Binärbild erzeugen
      pixOp.toBinaer(ref array3D, 0.6);
      // Kettencode für mehrere Objekte in einer Liste.
      List<List<uint>> kettencodes = ident.chaincodes(ref array3D, 1);
      // VISUALISIERUNG
      // Ausgabe für alle Segmentflächen zusammenstellen.
      int n = 1 + kettencodes.Count;
      string[] text = new string[n];
      text[0] = string.Format("Kettenkode");
      for (int i = 1; i < kettencodes.Count; i++) {
        text[i] = string.Format("u[{0:d}]= {1:d} Pixel, Startpunkt x= {2:d}, y= {3:d}, Kette:",
        i, kettencodes[i].Count - 2, kettencodes[i][0], kettencodes[i][1]);
        for (int j = 2; j < kettencodes[i].Count; j++)
          text[i] += kettencodes[i][j];
        text[i] += "\n";
      }
      // Kovertiert ID's in das Farbintervall.
      ARRAY.normalizationArray3D(ref array3D, ARRAY.getIntervall(0, 1));
      // Konvertiert Array in eine Bitmap.
      resultBitmap = new Bitmap(imgArray.toBitmap(array3D));
      // Bild mit ID's beschriften.
      output.addIds(resultBitmap, kettencodes, 16);
      // Ausgabe des Bildes
      output.bitmapShow(resultBitmap, "Identifikation", 3);
      // Ausgabe eines Textes in der Informationsbox.
      output.textBoxText(text);
    }

    /// <summary>
    /// 10.18 Normteile durchnummerieren
    /// durch Identifikation der Segmentflächen
    /// mithilfe der Nachbarn.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void idNeighborForStandardPartsToolStripMenuItem_Click(object sender, EventArgs e) {
      // Konvertiert Bitmap in ein 3D-Array
      float[,,] array3D = imgArray.toArray3D<float>(getSelectedBitmap());
      // Grauwertbild erzeugen
      pixOp.toGray(ref array3D);
      // Intensitätsformung, Verstärkung des Kontrastes
      pixOp.toInvert(ref array3D);
      // Intensitätsformung, Verstärkung des Kontrastes
      pixOp.sigmoidFormung(ref array3D, 0, 4);
      // Aufteilung auf das gesamten Grauwertspektrum
      pixOp.linearSpreizung(ref array3D);
      // Binärbild erzeugen
      pixOp.toBinaer(ref array3D, 0.6);
      // Closing, um Kanten auszugleichen und Artefakte zu entfernen.
      array3D = neighborOp.closing(array3D, ARRAY.toCanals(0, 1, 2), 7);
      // Liefert die Kontur der Segmentflächen mit dem Wert 1 auf dem Kanal 2.
      ident.contour(ref array3D, 1);
      // Vergibt den Segmentflächen eine fortlaufende ID ab 1 aufwärts.
      List<Point> startPoints = ident.segments(ref array3D, ARRAY.toPoints(ARRAY.backwardShift), 1);
      // VISUALISIERUNG
      // Text für alle Segmentflächen zur Ausgabe in der Informationsbox.
      string[] text = new string[1 + startPoints.Count];
      text[0] = string.Format("Segmentflächen per Nachbarschaftsbeziehungen ermitteln.");
      for (int id = 0; id < startPoints.Count; id++)
        text[id] = string.Format("u[{0:d}], Startpunkt x= {1:d}, y= {2:d} \n", id, startPoints[id].X, startPoints[id].Y);
      // Konvertiert ID's in den Farbintervall.
      ARRAY.normalizationArray3D(ref array3D, ARRAY.getIntervall(0, 1));
      // Konvertiert Array in ein Bitmap.
      resultBitmap = new Bitmap(imgArray.toBitmap(array3D));
      // Bild beschriften
      output.addIds(resultBitmap, startPoints, 16);
      // Ausgabe des Bildes
      output.bitmapShow(resultBitmap, "Identifikation", 3);
      // Ausgabe eines Textes in der Informationsbox.
      output.textBoxText(text);
    }

    /// <summary>
    /// 10.19 Die Segmentflächen mehrerer sich berührender Schrauben voneinander unterscheiden
    /// und ihnen Eigenschaften zuordnen.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void idForTouchingScrewsToolStripMenuItem_Click(object sender, EventArgs e) {
      // Konvertiert Bitmap in ein 3D-Array. Das Bild ist binär.
      float[,,] array3D = imgArray.toArray3D<float>(getSelectedBitmap());
      // Bild glätten
      array3D = neighborOp.linearFilter(array3D, neighborOp.G_Gauß(3), ARRAY.toCanals(0, 1, 2), 0, 1);
      // Graufeld erzeugen
      pixOp.toGray(ref array3D);
      // Maxpooling: Bildfläche wird geviertelt und dominante Pixel hervorgehoben.
      array3D = neighborOp.rankFilter(array3D, neighborOp.maximum, ARRAY.toCanals(0), 2, 0, 2);
      // Binärdarstellung erzeugen
      pixOp.toBinaer(ref array3D, 0.5);
      // Invertieren -> I(Hintergrund) = 0,  I(Objekt) > 0 , 1, 2, 3, ...
      pixOp.toInvert(ref array3D);
      // Der Kanal 0 des Basisbildes wird kopiert zum Späteren Konturabgleich.
      float[,] arrayKanal = ARRAY.copy(array3D, 0);
      // Starke Erosion um Segmentflächen zu trennen. 
      array3D = neighborOp.morphologie(array3D, ARRAY.toCanals(0), neighborOp.M_Morph(11, 0, 1));
      // Objekte Identifizieren, Hintergrund-Id: 0, Objekte mit Id: 1, 2, 3
      // Kanal 0: Original-ID, Kanal 1: Segmentflächen-ID, Kanal 2: Umrandungs-Id
      // Liefert Freemancode mehrerer Segmentflächen als Liste und referenziertes Array
      // mit Objekt-Id's im Kanal 1 sowie Umrandung im Kanal 2.
      List<List<uint>> kettencodes = ident.chaincodes(ref array3D, 1);
      // Anzahl der Objekte. Id=0 ist Hintergrund und wird nicht betrachtet.
      int segmentCount = kettencodes.Count - 1; //int objectCount >= 1;
      // Segmentflächen vergrößern mit Überschreitung der Originalgröße.
      neighborOp.dilatation(ref array3D, ARRAY.toCanals(1), segmentCount, 0, 3, 6);
      // Abgleich der Segmentflächen mit der Originalkontur.
      neighborOp.isObject(ref array3D, arrayKanal);
      // Schwerpunkt und Orientierung der Segmentflächen bestimmen.
      PointF[] schwerpunkte = new PointF[segmentCount + 1];
      float[] gammas = new float[segmentCount + 1];
      for (int id = 1; id <= segmentCount; id++) {
        schwerpunkte[id] = feature.flaechenschwerpunkt(array3D, 1, id);
        gammas[id] = feature.orientierung(array3D, schwerpunkte[id], 1, id);
      }
      // VISUALISIERUNG
      // Segment-Id's in das Farbintervall konvertieren.
      ARRAY.normalizationArray3D(ref array3D, ARRAY.getIntervall(0, 1));
      // Array in eine Bitmap konvertieren
      resultBitmap = new Bitmap(imgArray.toBitmap(array3D));
      // Bild mit ID's beschriften.
      output.addIds(resultBitmap, kettencodes, 16);
      // Zeichnet Schwerpunkte und Orientierungen in das Bild.
      output.addPoints(resultBitmap, schwerpunkte, 5);
      output.addPointers(resultBitmap, schwerpunkte, gammas, 40, 3);
      // Ausgabe des Bildes
      output.bitmapShow(resultBitmap, "\nProjekt: Mehrere Gegenstände", 3);
    }
    #endregion 
    #endregion // Nachbarschaftsbeziehungen

    #region 011. Segmentation
    /// <summary>
    /// 11.1 Erzeugt ein Binärbild mit optimierten Schwellwerten
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void schwellwertOptimierenToolStripMenuItem_Click(object sender, EventArgs e) {
      // Erzeuge ein 3D-Array 
      float[,,] array3D = imgArray.toArray3D<float>(getSelectedBitmap());
      // Invertieren
      pixOp.toInvert(ref array3D);
      // Erzeugt RGB-Histogramm
      int[,] histo = histogram.forRGB(array3D);
      // Ermittelt die kanalbezogenen Schwellwerte und gibt Filterarrays zur Visualisierung aus.
      List<byte> thresholds = segment.giveCanalThresholds(histo, out float[,] histoFiltered2D, out float[,] histoFiltered2D_abl, true);
      // Initialisieren des Diagramms
      histogram.initChart(histogram.toSeries(1, 2, 3, 4, 5, 6, 7, 8, 9), SeriesChartType.Spline);
      // Visualisierung des gefilterten Histogramms
      histogram.showChart(histoFiltered2D, ARRAY.toSeries(1, 2, 3));
      // Visualisierung der ersten Ableitung des gefilterten Histogramms
      histogram.showChart(histoFiltered2D_abl, ARRAY.toCanals(4, 5, 6));
      // Minima einzeichnen
      histogram.showChartPoinst(thresholds, ARRAY.toSeries(7, 8, 9));
      // Mehrkanalbinärbild erzeugen
      pixOp.toBinaer(ref array3D, thresholds);
      // Einkanalbinärbild erzeugen
      pixOp.orCanals(ref array3D, 1);
      // VISUALISIERUNG
      // Konvertiert Array in ein Bitmap zur weiteren Verarbeitung.
      resultBitmap = new Bitmap(imgArray.toBitmap(array3D));
      // Ausgabe des Arrays als Bild auf einer Registerseite
      output.bitmapShow(array3D, "Optimiertes Binärbild", 3);
      // Schwellwerte auf der Informationsseite ausgeben.
      output.thresholds(thresholds);
      // Schlägt Diagramm-Registerseite auf
      tabControl_IP.SelectedIndex = 5;
    }

    /// <summary>
    /// 11.2 Kantendetektion mit dem Sobeloperator und 
    /// dem Canny Edge Detector.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void cannyEdgeOperatorToolStripMenuItem_Click(object sender, EventArgs e) {
      // Konvertiert Bitmap in ein 3D-Array.
      float[,,] array3D = imgArray.toArray3D<float>(getSelectedBitmap());
      // Aufruf des linearen Gaußfilters zur Glättung des Bildarrays.
      array3D = neighborOp.linearFilter(array3D, neighborOp.G_Gauß(3), ARRAY.toCanals(0, 1, 2));
      // Grauwertarray erzeugen
      pixOp.toGray(ref array3D);
      // Ruft Sobelfilter auf und berechnet Gradientenbild.
      neighborOp.sobelFilter(ref array3D, neighborOp.Sx_Sobel_x(), neighborOp.Sy_Sobel_y(), ARRAY.toCanals(0, 1, 2));
      // Intensitätsformung, Verstärkung der Linien
      pixOp.sigmoidFormung(ref array3D, 0, 8);
      // Aufteilung auf das gesamten Grauwertspektrum
      pixOp.linearSpreizung(ref array3D);
      // Canny Edge mit Nachbaroperationen
      segment.cannyEdge(ref array3D, 0.25, 0.10);
      // Closing, optional
      // array3D = neighborOp.closing(array3D, ARRAY.toCanals(0, 1, 2), 7);
      /// VISUALISIERUNG
      // Kopiert Kanallayer 2 auf alle Kanäle
      ARRAY.copyCanalToCanal(ref array3D, 2);
      // Konvertiert Array in ein Bitmap zur weiteren Verarbeitung.
      resultBitmap = new Bitmap(imgArray.toBitmap(array3D));
      // Ausgabe des Arrays als Bild auf einer Registerseite
      output.bitmapShow(array3D, "Canny Edge", 3);
      // Schlägt Diagramm-Registerseite auf
      tabControl_IP.SelectedIndex = 3;
    }

    /// <summary>
    /// 11.3 Segmentfläche nach Kantendetektion definieren.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void kantenSegmentflächeToolStripMenuItem_Click(object sender, EventArgs e) {
      // Kantendetektor
      cannyEdgeOperatorToolStripMenuItem_Click(sender, e);
      // Konvertiert Bitmap in ein 3D-Array.
      float[,,] array3D = imgArray.toArray3D<float>(getSelectedBitmap());
      // Closing, optional
      array3D = neighborOp.closing(array3D, ARRAY.toCanals(0, 1, 2), 7);
      // Kopiere Elemente des Kanal 1 auf alle Kanäle.
      ARRAY.copyCanalToCanal(ref array3D, 1);
      // Hintergrundumgebung markieren
      segment.fillOutside(ref array3D);
      // Kanal 2 des Arrays invertieren -> Segmentfläche markiert
      pixOp.toInvert(ref array3D, 2);
      /// VISUALISIERUNG
      // Kopiert Elemente des Kanals 2 auf alle Kanäle.
      ARRAY.copyCanalToCanal(ref array3D, 2);
      // Konvertiert Array in eine Bitmap zur weiteren Verarbeitung.
      resultBitmap = new Bitmap(imgArray.toBitmap(array3D));
      // Ausgabe des Arrays als Bild auf einer Registerseite.
      output.bitmapShow(array3D, "Segmentfläche", 3);
      // Schlägt Registerseite auf.
      tabControl_IP.SelectedIndex = 3;
    }
    #endregion

    #region 12. Klassifikation
    /// <summary>
    /// 12.1 Eigenschaften einer Klasse ermitteln. 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void stichprobeToolStripMenuItem_Click(object sender, EventArgs e) {
      // Konvertiert Bitmap in ein 3D-Array
      float[,,] array3D = imgArray.toArray3D<float>(getSelectedBitmap());
      // Bild invertieren
      pixOp.toInvert(ref array3D);
      // Intensitätsformung, Verstärkung des Kontrastes
      pixOp.sigmoidFormung(ref array3D, 0, 4);
      // Aufteilung auf das gesamte Grauwertspektrum
      pixOp.linearSpreizung(ref array3D);
      // Erzeugt RGB-Histogramm
      int[,] histo = histogram.forRGB(array3D);
      // Liefert Schwellwerte
      List<byte> thresholds = segment.giveCanalThresholds(histo, out float[,] histoFiltered2D, out float[,] histoFiltered2D_abl, true);
      // Mehrkanalbinärbild erzeugen
      pixOp.toBinaer(ref array3D, thresholds);
      // Einkanalbinärbild erzeugen
      pixOp.orCanals(ref array3D, 1);
      // Kettencode für mehrere Objekte in einer Liste.
      List<List<uint>> kettencodes = ident.chaincodes(ref array3D, 1);
      // Anzahl der Objekte. Id=0 ist Hintergrund und wird nicht betrachtet.
      int segmentCount = kettencodes.Count - 1; //int objectCount >= 1;
      // Liefert den Umfang in Einheitspixel
      List<float> u_Ep = feature.umfangEp(kettencodes);
      // Liefert die Größe der Segmentflächen Pixel².
      List<float> a_Ap = feature.flaechen(ref array3D, 1, segmentCount);
      // VISUALISIERUNG
      // Ausgabe für alle Segmentflächen zusammenstellen.
      int n = 6 + segmentCount;
      string[] text = new string[n];
      string classname = toolStripComboBox_Teileklassen.Text;
      text[0] = string.Format("Training zur Klassifikation: " + classname);
      // Ausgabe der Einzelwerte
      int id = 1;
      for (id = 1; id <= segmentCount; id++) {
        text[id] = string.Format("Id[{0:d2}],  Umfang U: {1,7:F1} E-Pixel, Fläche A: {2,7:F0} Pixel²",
        id, u_Ep[id], a_Ap[id]);
      }
      // Konvertiert ID's in das Farbintervall.
      ARRAY.normalizationArray3D(ref array3D, ARRAY.getIntervall(0, 1));
      // Konvertiert Array in eine Bitmap.
      resultBitmap = new Bitmap(imgArray.toBitmap(array3D));
      // Bild mit ID's beschriften.
      output.addIds(resultBitmap, kettencodes, 16);
      // Ausgabe des Bildes
      output.bitmapShow(resultBitmap, "TrainingRGB", 3);
      // Ausgabe eines Textes in der Informationsbox.
      output.textBoxText(text);
      // Trainingsdaten einer Klasse im TXT-Format speichern oder ergänzen.
      classes.appendTrainData(u_Ep, a_Ap, classname);
    }

    /// <summary>
    /// 12.2 Trainingsdaten laden
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void datenLadenToolStripMenuItem_Click(object sender, EventArgs e) {
      OpenFileDialog openFileDialog = new OpenFileDialog();
      if (openFileDialog.ShowDialog() == DialogResult.OK) {
        // Trainingsdaten mehrerer Teileklassen laden
        classes.loadDataFromTxt(openFileDialog.FileName);
        // Statistische Kenngrößen berechnen
        classes.average();
        classes.standard();
        classes.confidenceBounderyFunction();
        // Modell speichern
        string model = classes.saveModelAsTxt();
        // Ausgabe zur Information
        // output.textBoxText(classes.arrayToText(classes.isM));
        //output.textBoxText(model);
        richTextBox_Info.Text += classes.arrayToText(classes.isM);
        richTextBox_Info.Text += model;
      }
      // Schlägt Registerseite auf.
      tabControl_IP.SelectedIndex = 7;
    }

    /// <summary>
    /// 12.3 Lädt die Trainingsdaten, berechnet statistische Kenngrößen,
    /// visualisiert Teileklassen und Konfidenzregionen.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void konfidenzintervalleToolStripMenuItem_Click(object sender, EventArgs e) {
      OpenFileDialog openFileDialog = new OpenFileDialog();
      if (openFileDialog.ShowDialog() == DialogResult.OK) {
        // Trainingsdaten mehrerer Teileklassen laden
        classes.loadDataFromTxt(openFileDialog.FileName);
        // Statistische Kenngrößen berechnen
        classes.average();
        classes.standard();
        classes.confidenceBounderyFunction();
        // Modell speichern
        string model = classes.saveModelAsTxt();
        // Ausgabe zur Information
        richTextBox_Info.Text += classes.arrayToText(classes.isM);
        richTextBox_Info.Text += model;
      }
      // VISUALISIERUNG
      // Alte Datenpunkte löschen
      // Schlägt Registerseite auf.
      //+ tabControl_IP.SelectedIndex = 7;
      // Intervall einrichten
      //+ chart_classes.ChartAreas[0].AxisX.Minimum = 180;
      //+ chart_classes.ChartAreas[0].AxisX.Maximum = 220;
      //+ chart_classes.ChartAreas[0].AxisX.Interval = 10;
      //+ chart_classes.ChartAreas[0].AxisY.Minimum = 1600;
      //+ chart_classes.ChartAreas[0].AxisY.Maximum = 2600;
      //+ chart_classes.ChartAreas[0].AxisY.Interval = 200;
      // VISUALISIERUNG
      // Alte Datenpunkte löschen
      for (var k = 0; k < chart_classes.Series.Count; k++)
        chart_classes.Series[k].Points.Clear();
      // Datenpunkte der Teileklassen darstellen
      for (var k = 1; k < 4; k++) {
        for (var i = 1; i <= 11; i++) {
          chart_classes.Series[k].Points.AddXY(classes.isM[k, 0, i], classes.isM[k, 1, i]);
        }
        // Erwartungswerte der Teileklassen darstellen
        chart_classes.Series[4].Points.AddXY(classes.getEM[k, 0], classes.getEM[k, 1]);
        // Konfidenzbereiche der Teileklassen darstellen
        for (var g = 0; g < 360; g++)
          chart_classes.Series[k + 4].Points.AddXY(classes.getfu[k, 0, g], classes.getfu[k, 1, g]);
      }
      // Schlägt Registerseite auf.
      tabControl_IP.SelectedIndex = 6;
    }

    /// <summary>
    /// 12.4 Stellt die Verteilungsdichte zweier Teileklassen und deren Grenzbereich
    /// als Farbbild dar. Die Intensitätswerte entsprechen den Dichtewerten.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void verteilungsdichteInFarbeToolStripMenuItem_Click(object sender, EventArgs e) {
      // Definition des darzustellenden Intervalls [Umin, Amin, DeltaU, DeltaA]
      // Rectangle interval = new Rectangle(180, 1500, 420 - 180, 5000 - 1500);
      Rectangle interval = new Rectangle(180, 1700, 210 - 180, 2300 - 1700);
      int nrClasses = 3; //Gesamtanzahl der Teileklassen 
      // Anzahl der Stützpunkte in x und in y, Bitmapgröße
      int nrPoints = 600;
      // Liefert die Verteilungsdichte
      float[,,] array = classes.twoDimensionalDensity(interval, nrClasses, nrPoints);
      // Normalisierung der Dichtewerte
      ARRAY.normalizationArray3Ddepending(ref array, 0, 1);
      // Erlaubte Abweichung zwischen den Häufigkeitswerten zweier Klassen
      double dh = 0.001;
      // Klassengrenzen ermitteln (Klasse 2->Kanal 1, Klasse 3 -> Kanal 2)
      array = classes.bordersOfClasses(array, 1, 2, dh, true);
      // Spiegeln der Y-Werte des Bildes.
      ARRAY.yInvers(ref array);
      // VISUALISIERUNG
      resultBitmap = imgArray.toBitmap(array);
      // Ausgabe des Bildes
      output.bitmapShow(resultBitmap, "\nVerteilung als Bild", 3);
      pictureBox_imgResult.SizeMode = PictureBoxSizeMode.StretchImage;
    }

    /// <summary>
    /// 12.5 Segmentflächen werden den Teileklassen unter Verwendung
    /// der antrainierten Modelldaten zugeordnet. Nicht zuordenbare Teile
    /// werden ans unbekannt gekennzeichnet.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void prognoseToolStripMenuItem_Click(object sender, EventArgs e) {
      // Konvertiert Bitmap in ein 3D-Array
      float[,,] array3D = imgArray.toArray3D<float>(getSelectedBitmap());
      // Bild invertieren
      pixOp.toInvert(ref array3D);
      // Bild glätten mit dem Gaußfilter.
      array3D = neighborOp.linearFilter(array3D, neighborOp.G_Gauß(3), ARRAY.toCanals(0, 1, 2), 0, 1);
      // Intensitätsformung, Verstärkung des Kontrastes
      pixOp.sigmoidFormung(ref array3D, 0, 4);
      // Aufteilung auf das gesamte Intensitätsspektrum
      pixOp.linearSpreizung(ref array3D);
      // Erzeugt RGB-Histogramm
      int[,] histo = histogram.forRGB(array3D);
      // Liefert Schwellwerte
      List<byte> thresholds = segment.giveCanalThresholds(histo, out float[,] histoFiltered2D, out float[,] histoFiltered2D_abl, true);
      // Mehrkanalbinärbild erzeugen
      pixOp.toBinaer(ref array3D, thresholds);
      // Einkanalbinärbild erzeugen
      pixOp.orCanals(ref array3D, 1);
      // Kettencode für mehrere Objekte in einer Liste.
      List<List<uint>> kettencodes = ident.chaincodes(ref array3D, 1);
      // Anzahl der Objekte. Id=0 ist Hintergrund und wird nicht betrachtet.
      int segmentCount = kettencodes.Count - 1;
      // Liefert den Umfang in Einheitspixel der Testdaten
      List<float> u_Ep = feature.umfangEp(kettencodes);
      // Liefert die Größe der Segmentflächen Pixel².
      List<float> a_Ap = feature.flaechen(ref array3D, 1, segmentCount);
      // Lädt die Modelldaten, um Prognose vorzunehmen
      classes.loadModelFromTxt();
      // Berechnet die Konfidenzgrenzen für t(99%) und
      // liefert die Isodichte fh[k=1,2,3]
      double[] fh = classes.confidenceBounderyFunction(3.106);
      // Ist-Dichtewerte
      double[,] density = classes.actualDensity(a_Ap, u_Ep);
      // Ausgabe des Dichtearrays
      output.textBoxMatrix(density, "Dichte, Zielen: Klassen k, Spalten: Teile");
      // Index der Klasse mit der größten Wahrscheinlichkeit
      int[] kMax = classes.indexOfMax(density);
      // Ausgabe der Dichte
      richTextBox_Info.Text += classes.densityToText(density, kMax, fh, "Indexes of density");
      // Liefert eine Datenstruktur mit der Klassenzugehörigkeit, mit den Eigenschaftswerten und den Segment-Ids.
      ClassificationOwn.prediction part = classes.segments(density, kMax, fh, u_Ep, a_Ap, 0);
      // VISUALISIERUNG
      // Prognoseergebnisse als Text
      richTextBox_Info.Text += classes.predictionToText(part.properties, part.k, part.kName);
      // Alte Datenpunkte des Diagramms löschen
      for (var k = 0; k < chart_classes.Series.Count; k++)
        chart_classes.Series[k].Points.Clear();
      // Extremwerte, Konfidenzregionen und Segmente darstellen
      for (var k = 0; k < classes.getEM.GetLength(0); k++) {
        // Erwartungswerte
        chart_classes.Series[4].Points.AddXY(classes.getEM[k, 0], classes.getEM[k, 1]);
        // Konfidenzregionen
        for (var g = 0; g < 360; g++)
          chart_classes.Series[k + 4].Points.AddXY(classes.getfu[k, 0, g], classes.getfu[k, 1, g]);
      }
      // Datenpunkte der gefundenen Teileklassen in das Diagramm eintragen und beschriften
      for (var i = 1; i < part.k.Count; i++) {
        // Punkte einzeichnen (p - Punktnummer in der Serie)
        int p = chart_classes.Series[part.k[i]].Points.AddXY(part.properties[i][0], part.properties[i][1]);
        chart_classes.Series[part.k[i]].Points[p].Label = (i).ToString();
      }
      // Konvertiert ID's in das Farbintervall.
      ARRAY.normalizationArray3D(ref array3D, ARRAY.getIntervall(0, 1));
      // Konvertiert Array in eine Bitmap.
      resultBitmap = new Bitmap(imgArray.toBitmap(array3D));
      // Bild mit ID's beschriften.
      output.addIdsAndNames(resultBitmap, kettencodes, part.kName, 16);
      // Ausgabe des Bildes
      output.bitmapShow(resultBitmap, "Training", 3);
      //HISTOGRAMM
      // Diagrammserien initialisieren
      histogram.initChart(histogram.toSeries(1, 2, 3, 7, 8, 9), SeriesChartType.Spline);
      // Zeigt das RGB-Histogramm 
      histogram.showChart(histoFiltered2D, histogram.toSeries(1, 2, 3));
      // Minima, Schwellwerte einzeichnen
      histogram.showChartPoinst(thresholds, ARRAY.toSeries(7, 8, 9));
    }
    #endregion

    // . . . 

    #region 99. Hilfsmethoden
    /// <summary>
    /// 99.1 Liefert das Bild der aufgeschlagenen Registerseite.
    /// </summary>
    /// <returns> bitmap </returns> Bild der aufgeschlagenen Seite.
    private Bitmap getSelectedBitmap() {
      switch (tabControl_IP.SelectedIndex) {
        case 1:
          return bitmap; // Basisbild, Eingangsbild
        case 2:
          return subBitmap; // Bildausschnitt
        case 3:
          return resultBitmap; // Ergebnisbild
      }
      return bitmap;
    }

    /// <summary>
    /// 99.2 Liefert das Bild der Picture-Box der aufgeschlagenen Registerseite.
    /// </summary>
    /// </summary>
    /// <returns> bitmap </returns> Bild
    private Bitmap getSelectedPictureBoxImage() {
      string pageName = tabControl_IP.SelectedTab.Name.ToString();
      switch (pageName) {
        case "tabPage_Cam":
          return (Bitmap)panel_Cam.BackgroundImage;
        case "tabPage_image":
          return (Bitmap)pictureBox_image.Image;
        case "tabPage_imgArea":
          return (Bitmap)pictureBox_imgArea.Image;
        case "tabPage_imgResult":
          return (Bitmap)pictureBox_imgResult.Image;
      }
      return bitmap;
    }
    #endregion

    #region 100. Beenden
    /// <summary>
    /// 100.1 Anwendung schließen
    /// </summary>
    private void beendenToolStripMenuItem_Click(object sender, EventArgs e) {
      // Stoppt die Darstellung
      stopToolStripMenuItem_Click(sender, e);
      Close();
    }







































    #endregion

    private void trainingToolStripMenuItem1_Click(object sender, EventArgs e) {

    }

    private void identifikationVonSegmentflächenToolStripMenuItem_Click(object sender, EventArgs e) {

    }

    private void toolStripComboBox_Teileklassen_Click(object sender, EventArgs e) {

    }

    private void tiefpassfilterToolStripMenuItem_Click(object sender, EventArgs e) {

    }

    private void dateiToolStripMenuItem_Click(object sender, EventArgs e) {

    }
  }
}






