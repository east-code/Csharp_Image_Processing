
using System;
using System.Drawing;
using form = System.Windows.Forms;

namespace ImageProcessing {
  class OutputOwn {

    #region 1. Attribute, Konstruktor

    // 1.2 Komponente des Formulars
    form.RichTextBox richTextBox;

    /// <summary>
    /// 1.3 Konstruktor übernimmt Chart-Komponente
    /// </summary>
    /// <param name="richTextBox"></param> Hinweise, Informationen
    public OutputOwn(form.RichTextBox richTextBox) {
      this.richTextBox = richTextBox;
    }
    #endregion

    #region 2. Ergebnisbild und Hinweise ausgeben

    /// <summary>
    /// 2.2 Liefert Informationen zu einem Bitmap
    /// </summary>
    /// <param name="bitmap"></param> Bild
    /// <param name="bezeichnung"></param> Optionale beliebige Bezeichnung
    /// <returns></returns> Informationen
    public void bitmapInfo(Bitmap bitmap, string bezeichnung = "") {
      if (bitmap != null) {
        richTextBox.Text += String.Format("Bild: {0}, {1} \n", bezeichnung, bitmap);
        richTextBox.Text += String.Format("Pixelformat: {0}\n", bitmap.PixelFormat);
        richTextBox.Text += String.Format("Bildgröße: {0}\n\n", bitmap.Size);
      }
      else { richTextBox.Text = "Kein Bild erkannt!\n"; }
    }

    /// <summary>
    /// 2.3 Informationen zur Bild-Array-Transformation
    /// </summary>
    /// <param name="bitmap"></param>
    /// <param name="page"></param>
    public void transformShow(Bitmap bitmap, int page = 3) {
      pictureBox.Image = bitmap;
      bitmapInfo(bitmap, "\nTest: Transformiert ein Bild in ein Array und zurück in ein Bild");
      tabControl.SelectedIndex = page;
    }


    #endregion







  }
}