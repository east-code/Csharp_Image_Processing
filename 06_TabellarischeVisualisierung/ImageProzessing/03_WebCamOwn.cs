// using: AForge-Klassen zum Einbinden von Kameras
using AForge.Controls;
using AForge.Video;
using AForge.Video.DirectShow;

using System.Drawing;
using System.Threading; // -> Pause
using form = System.Windows.Forms;

namespace ImageProcessing {
  class WebCamOwn {

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
        // add all devices to combo
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
    /// 2.5 Liefert ein aktuelles Standbild zum Videostram
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
    #endregion Komponentenmethoden

    #region 3. Methoden, Hintergrundmethoden
    /// <summary> Hintergrundprozess
    /// 3.1 Liefert Bildfolge (Methode als Argument der aufrufenden Methode).
    /// Jedes ankommende Objekt wird als Bitmap übernommen.
    /// (.Clone() verhindert Zugriffsverletzungen)
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
  }
}


