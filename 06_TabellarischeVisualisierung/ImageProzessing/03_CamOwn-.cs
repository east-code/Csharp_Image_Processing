// using: AForge-Klassen zum einbinden von Kameras
using form = System.Windows.Forms;

namespace ImageProcessing {
  class OwnCam {

    #region 1. Instanzen und Klassen
    private FilterInfoCollection videoDevices;
    private VideoCaptureDevice videoDevice;
    private VideoCapabilities[] videoCapabilities;
    private form.Cursor Cursor;
    private Bitmap bitmap; // Ablage des Kamerabildes, des Standbildes
    public form.ComboBox comboBox_Devices; // Auswahlbox für Kameraauswahl
    public form.ComboBox comboBox_Capabilities; // Auswahlbox für Auflösung
    public form.Panel panel;                    // Panel zur Ausgabe des Videos
    public form.PictureBox pictureBox;          // Picturebox für Standbild
    /// <summary>
    /// Konstruktor 
    /// </summary>
    /// <param name="comboBox"></param> Auswahlbox für Geräte (Devices)
    public OwnCam(form.ComboBox comboBox_Devices, form.ComboBox comboBox_Capabilities, form.Panel panel, form.PictureBox pictureBox) {
      this.comboBox_Devices = comboBox_Devices;
      this.comboBox_Capabilities = comboBox_Capabilities;
      this.panel = panel;
      this.pictureBox = pictureBox;
    }

    /// <summary>
    /// Videoplayer vorbereiten
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
    /// Zusammenstellung der Video-Auflösungen
    /// </summary>
    /// <param name="comboBox"></param> comboBox_Capabilities
    public void EnumerationFrameSizes(form.ComboBox comboBox) {
      this.Cursor = form.Cursors.WaitCursor;
      comboBox.Items.Clear();
      try {
        videoCapabilities = this.videoDevice.VideoCapabilities;
        foreach (VideoCapabilities capabilty in videoCapabilities) {
          comboBox.Items.Add(string.Format("{0} x {1}",
              capabilty.FrameSize.Width, capabilty.FrameSize.Height));
        }
        if (videoCapabilities.Length == 0) {
          comboBox.Items.Add("Not supported");
        }
        comboBox.SelectedIndex = 0;
      }
      finally { this.Cursor = form.Cursors.Default; }
    }
    /// <summary>
    /// Start der Kameraübertragung
    /// </summary>
    /// <param name="panel"></param> panel_Cam
    /// <param name="comboBox"></param comboBox_Capabilities
    /// <returns></returns>
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
    /// Stop der Kameraübertragung
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
    /// Liefert ein aktuelles Standbild zum Videostram
    /// </summary>
    /// <param name="pictureBox"></param>
    /// <returns></returns>
    public Bitmap shot() {
      // Rufe einen Parallelprozess „newFrame“
      videoDevice.NewFrame += new NewFrameEventHandler(newFrame);
      // Zeit zur Ausführung des Parallelprozesses, Bildratenabhängig
      Thread.Sleep(200);
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
    /// Liefert Bildfolge (Methode als Argument der aufrufenden Methode).
    /// Jedes ankommende Objekt wird als Bitmap übernommen.
    /// (.Clone() verhindert Zugriffsverletzungen)
    /// </summary>
    private void newFrame(object sender, NewFrameEventArgs eventArgs) {
      // Sperrt den Zugriff durch parallel laufende Prozesse (Threads)
      lock (this) {
        if (bitmap != null) // gibt Speicherplatz frei
          bitmap.Dispose();
        // Fängt ankommenden Frame
        bitmap = (Bitmap)eventArgs.Frame.Clone();
      }
    }
    #endregion Methoden, Hintergrundmethoden
  }
}


