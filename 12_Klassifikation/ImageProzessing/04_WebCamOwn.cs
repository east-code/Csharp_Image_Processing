/* Klasse CamParameter: Datenstruktur zum Verwalten der Kameraparater
 * Klasse WebCamOwn: Arbeit mit Web-Cams unter Einbeziehung der externen Bibliothek „AForge“. 
 * Verfasser: Horst-Christian Heinke
 * Datum: 15.06.2022
 * Letzte Änderung: 15.01.2023
 * Germany, Sachsen-Anhalt, Magdeburg
 * */
// using: AForge-Klassen zum Einbinden von Kameras
using AForge.Controls;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Drawing;
using System.Threading; // -> Pause
using form = System.Windows.Forms;

namespace ImageProcessing {

  /// Klassen:
  /// CamParameter - Datenstruktur für Kameraparameter
  /// WebCamOwn -    Verwendung einer WebCam
  ///..................................................
  /// <summary>
  /// Struktur der Kameraparameter
  /// </summary>
  public class CamParameter {
    public float f; // in mm
    public float w; //in mm
    public float h; // in mm
    public int w_p; // in Pixel
    public int h_p; // in Pixel
    public float alpha_w; // Betrachtungswinkel in °, horizontal
    public float alpha_h; // Betrachtungswinkel in °, vertikal
  }

  /// <summary>
  /// Verwendung einer Webcmam
  /// </summary>
  internal class WebCamOwn {

    #region 1. Instanzen und Klassen
    /// <summary>
    /// 1.1 Attribute und Instanzen
    /// </summary>
    private FilterInfoCollection videoDevices; // Angeschlossene Kameras
    private VideoCaptureDevice videoDevice;  // Kameraauswahl
    private VideoCapabilities[] videoCapabilities; // Auflösung der Kameraauswahl
    private form.Cursor cursor; // Zeiger
    private Bitmap bitmap; // Ablage des Kamerabildes, des Standbildes
    public form.ComboBox comboBox_Devices; // Auswahlbox für Kameraauswahl
    public form.ComboBox comboBox_Capabilities; // Auswahlbox für Auflösung
    public form.Panel panel;                    // Panel zur Ausgabe des Videos
    public form.PictureBox pictureBox;          // Picturebox für Standbild

    /// <summary>
    /// 1.2 Konstruktor
    /// </summary>
    /// <param name="comboBox_Devices"></param> Auswahlbox für Geräte (Devices)
    /// <param name="comboBox_Capabilities"></param> Auswahlbox für für Auflösung
    /// <param name="panel"></param> Videoausgabe
    /// <param name="pictureBox"></param> Schnappschuss-Ausgabe
    public WebCamOwn(form.ComboBox comboBox_Devices, form.ComboBox comboBox_Capabilities, form.Panel panel, form.PictureBox pictureBox) {
      this.comboBox_Devices = comboBox_Devices;
      this.comboBox_Capabilities = comboBox_Capabilities;
      this.panel = panel;
      this.pictureBox = pictureBox;
    }

    /// <summary>
    /// 1.3 Videoplayer vorbereiten
    /// </summary>
    public VideoSourcePlayer videoSourcePlayer = new VideoSourcePlayer() {
      // AutoSizeControl = true,  //Tatsächliche Größe
      BackColor = SystemColors.ControlDarkDark,
      ForeColor = Color.White,
      Location = new Point(0, 0),  // Links oben
      Name = "videoSourcePlayer",
      Size = new System.Drawing.Size(644, 484),  // Größe des Videofensters
      VideoSource = null,
      Visible = true
    };

    #endregion

    #region 2. Komponentenmethoden 
    ///     - Kommunizieren mit den Komponenten des Formulars
    /// <summary>
    /// 2.1 Auflistung der angeschlossenen Kameras
    /// </summary>
    public void EnumerationVideoDevices() {
      // Aufzählung der Videogeräte
      videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
      if (videoDevices.Count != 0) {
        // Alle Geräte (Kameras) der Combobox hinzufügen
        foreach (FilterInfo device in videoDevices) {
          comboBox_Devices.Items.Add(device.Name);
        }
      }
      else {
        comboBox_Devices.Items.Add("No DirectShow devices found");
      }
      comboBox_Devices.SelectedIndex = 0;
    }
    /// <summary>
    /// Kameraauswahl 
    /// </summary>
    public void VidioDevice() {
      if (this.videoDevices.Count != 0) {
        this.videoDevice = new VideoCaptureDevice(this.videoDevices[comboBox_Devices.SelectedIndex].MonikerString);
      }
    }

    /// <summary>
    /// 2.2 Zusammenstellung der Video-Auflösungen
    /// </summary>
    public void EnumerationFrameSizes() {
      this.cursor = form.Cursors.WaitCursor;
      comboBox_Capabilities.Items.Clear();
      try {
        videoCapabilities = this.videoDevice.VideoCapabilities;
        foreach (VideoCapabilities capabilty in videoCapabilities) {
          comboBox_Capabilities.Items.Add(string.Format("{0} x {1}",
              capabilty.FrameSize.Width, capabilty.FrameSize.Height));
        }
        if (videoCapabilities.Length == 0) {
          comboBox_Capabilities.Items.Add("Not supported");
        }
        comboBox_Capabilities.SelectedIndex = 0;
      }
      finally { this.cursor = form.Cursors.Default; }
    }

    /// <summary>
    /// 2.3 Start der Kameraübertragung
    /// </summary>
    public void Start() {
      if (videoDevice != null) {
        if ((videoCapabilities != null) && (videoCapabilities.Length != 0)) {
          videoDevice.VideoResolution = videoCapabilities[comboBox_Capabilities.SelectedIndex];
        }
        videoSourcePlayer.VideoSource = videoDevice;
        videoSourcePlayer.Size = videoCapabilities[comboBox_Capabilities.SelectedIndex].FrameSize;
        videoSourcePlayer.Start();
        // Darstellung auf einem Panel
        panel.Controls.Add(videoSourcePlayer);

      }
    }
    /// <summary>
    /// 2.4 Stop der Kameraübertragung
    /// </summary>
    public void Stop() {
      if (videoSourcePlayer.VideoSource != null) {
        // stop video device
        videoSourcePlayer.SignalToStop();
        videoSourcePlayer.WaitForStop();
        videoSourcePlayer.VideoSource = null;
        videoDevice.NewFrame -= new NewFrameEventHandler(newFrame);
      }
    }

    /// <summary>
    /// 2.5 Liefert ein aktuelles Standbild zum Videostream
    /// </summary>
    /// <param name="pictureBox"></param>
    /// <returns></returns>
    public Bitmap shot() {
      // Rufe einen Parallelprozess „newFrame“
      videoDevice.NewFrame += new NewFrameEventHandler(newFrame);
      // Zeit zur Ausführung des Parallelprozesses, Bildratenabhängig
      Thread.Sleep(100);
      // Bild darstellen
      pictureBox.SizeMode = form.PictureBoxSizeMode.StretchImage;
      pictureBox.Image = this.bitmap;
      // Nach Fangen des Bildes Aufruf beenden
      videoDevice.NewFrame -= new NewFrameEventHandler(newFrame);
      return this.bitmap;
    }
    #endregion

    #region 3. Methoden, Hintergrundmethoden
    /// <summary> Hintergrundprozess
    /// 3.1 Liefert Bildfolge (Methode als Argument der aufrufenden Methode).
    /// Jedes ankommende Objekt wird als Bitmap übernommen.
    /// (Clone() verhindert Zugriffsverletzungen)
    /// </summary>
    private void newFrame(object sender, NewFrameEventArgs eventArgs) {
      // Sperrt den Zugriff durch parallel laufende Prozesse (Threads)
      lock (this) {
        if (bitmap != null) // Gibt Speicherplatz frei
          bitmap.Dispose();
        // Fängt ankommenden Frame
        bitmap = (Bitmap)eventArgs.Frame.Clone();
      }
    }
    #endregion Methoden, Hintergrundmethoden

    #region 4. Eigenschaftsmethoden zu den Parametern einiger Kameras

    /// <summary>
    /// 4.1: Liefert die Parameter einer Webcam
    /// </summary>
    public CamParameter getPparaCam {
      get {
        CamParameter cam = new CamParameter();
        cam.f = (float)7.028;
        cam.w = (float)4.561;
        cam.h = (float)3.473;
        cam.w_p = 640;
        cam.h_p = 480;
        return cam;
      }
    }

    /// <summary>
    ///  4.2 Liefert die Parameter der MsWebCam
    ///  - Focus automatisch
    /// </summary>
    public CamParameter getParaMsCam {
      get {
        CamParameter cam = new CamParameter();
        cam.f = (float)1.992935;
        cam.w = (float)2.238953;
        cam.h = (float)1.294994;
        cam.w_p = 640;
        cam.h_p = 480;
        return cam;
      }
    }

    /// <summary>
    ///  4.3 Liefert die Parameter der Microsoft® LifeCam VX-2000
    ///  - g bezieht sich auf Objektoberfläche
    /// </summary>
    public CamParameter getParaMSLifeCam_VX2000_obj {
      get {
        CamParameter cam = new CamParameter();
        cam.f = (float)3.053; // Brennweite f in mm
        cam.w = (float)2.558; // Matrixweite in mm
        cam.h = (float)1.919; // Matrixhöhe in mm
        cam.w_p = 640;        // Matrixweite in Pixel
        cam.h_p = 480;        // Matrixhöhe in Pixel
        cam.alpha_w = (float)45.5; // Horizontaler Betrachtungswinkel in °
        cam.alpha_h = (float)34.9; // Vertikaler Betrachtungswinkel in °
        return cam;
      }
    }

    /// <summary>
    ///  4.4 Liefert die Parameter der Microsoft® LifeCam VX-2000
    ///  - g bezieht sich auf Hintergrundbeleuchtung
    /// </summary>
    public CamParameter getParaMSLifeCam_VX2000 {
      get {
        CamParameter cam = new CamParameter();
        cam.f = (float)6.55334372; // Brennweite f in mm
        cam.w = (float)5.490733801657711; // Matrixweite in mm
        cam.h = (float)4.118050351243284; // Matrixhöhe in mm
        cam.w_p = 640;        // Matrixweite in Pixel
        cam.h_p = 480;        // Matrixhöhe in Pixel
        cam.alpha_w = (float)0.79342959; // Horizontaler Betrachtungswinkel in rad, 45.46016678 °
        cam.alpha_h = (float)0.60885591; // Vertikaler Betrachtungswinkel in rad, 34.88487374°
        return cam;
      }
    }

    #endregion

  } // Klasse WebCamOwn
} // Namensraum


