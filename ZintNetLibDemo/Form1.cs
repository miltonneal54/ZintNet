using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ZintNet;

namespace ZintNetLibTest
{
    public partial class Form1 : Form
    {
        /* 
         * This demo program sets the X dimension to 0.264583f (1 pixel at 96dpi)
         * and the multiplier is in units of 1 in order to product screen scanable barcodes
         * and 96 dpi images. A default bar height of 20mm, is user settable at 1mm units.
         * When printing barcodes (output to printer) these symbol properties should be set
         * as to generate a symbol size that meets the symbols' organisation, designer or
         * manufacturer specifications.
         */

        private string outputFile = "out";
        private ZintNetLib myBarcode = null;
        private Symbology symbolID = Symbology.Code128;
        private Color barcodeColor = Color.Black;
        private Color backgroundColor = Color.White;
        private Color textColor = Color.Black;
        private Font barcodeTextFont = new Font("Arial", 10.0f, FontStyle.Regular);
        private int rotationAngle = 0;
        float textMargin = 0.0f;
        float barHeight = 20.0f;
        float multiplierValue = 1.0f;

        private int qrVersion = 0;
        private QRCodeEccLevel qrErrorLevel = (QRCodeEccLevel)(-1);

        private int aztecVersion = 0;
        private int aztecErrorLevel = -1;

        private int hanXinVersion = 0;
        private int hanXinErrorLevel = -1;

        private int gridMatrixVersion = 0;
        private int gridMatrixErrorLevel = -1;

        private int ultracodeCompression = 0;
        private int ultracodeErrorLevel = -1;

        private int pdfErrorLevel = -1;

        private EncodingMode encodingMode = EncodingMode.Standard;
        private CompositeMode compositeMode = CompositeMode.CCA;

        // Barcodes string values.
        private string barcodeData = String.Empty;
        private string compositeText = String.Empty;
        private string supplementText = String.Empty;

        private ITF14BearerStyle itf14BearerStyle = ITF14BearerStyle.Rectangle;
        private TextAlignment textAlignment = TextAlignment.Center;
        private TextPosition textPosition = TextPosition.UnderBarcode;

        public Form1()
        {
            InitializeComponent();
            // Double buffer the barcode image panel.
            typeof(Panel).InvokeMember(
                "DoubleBuffered",
                BindingFlags.NonPublic |
                BindingFlags.Instance |
                BindingFlags.SetProperty,
                null,
                imagePanel,
                new object[] { true });
        }

        private void Form1Load(object sender, EventArgs e)
        {
            myBarcode = new ZintNetLib();
            if (myBarcode != null)
                GetSymbologies();

            // Set some menu options.
            printToolStripMenuItem.Enabled = false;
            saveAsToolStripMenuItem.Enabled = false;
            generateButton.Enabled = false;
            textMarginNumericUpDown.Value = (decimal)(myBarcode.TextMargin);
            barHeightNumericUpDown.Value = (decimal)(myBarcode.BarcodeHeight);
            rotateTextBox.Text = rotationAngle.ToString() + (char)176;
            textPositionComboBox.SelectedIndex = 0;
            textAlignComboBox.SelectedIndex = 0;
        }

        private void Form1Shown(object sender, EventArgs e)
        {
            barcodeDataTextBox.Focus();
        }

        private void PrintToolStripMenuItemClick(object sender, EventArgs e)
        {
            PrintDocument barcodeDocument = new PrintDocument();
            PrintDialog pd = new PrintDialog();
            pd.UseEXDialog = true;
            pd.Document = barcodeDocument;
            if (pd.ShowDialog() == DialogResult.OK)
            {
                barcodeDocument.PrintPage += new PrintPageEventHandler(this.PrintBarcode);
                barcodeDocument.Print();
            }
        }

        void PrintBarcode(object sender, PrintPageEventArgs e)
        {
            myBarcode.DrawBarcode(e.Graphics, new Point(40, 40));
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (myBarcode != null)
                myBarcode.Dispose();

            Application.Exit();
        }

        private void ImagePanelPaint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            if (myBarcode != null && myBarcode.IsValid)
            {
                try
                {
                    Size bcSize = myBarcode.SymbolSize(graphics);
                    Point location = new Point((imagePanel.Width / 2) - (bcSize.Width / 2),
                                                (imagePanel.Height / 2) - (bcSize.Height / 2));

                    myBarcode.DrawBarcode(graphics, location);
                    outputTextBox.Text = myBarcode.ToString();
                }

                catch (ZintNetDLLException ex)
                {
                    outputTextBox.Text = String.Empty;
                    string errorMessage = ex.Message;
                    if (ex.InnerException != null)
                        errorMessage += ex.InnerException.Message;

                    System.Windows.Forms.MessageBox.Show(errorMessage, "ZintNet Barcode Demo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                finally
                {
                    UpdateMenus();
                }
            }
        }

        private void ImagePanelResize(object sender, EventArgs e)
        {
            imagePanel.Invalidate();
        }

        // Querry the library and get a list of support symbologies.
        private void GetSymbologies()
        {
            symbologyComboBox.Items.AddRange(ZintNetLib.GetSymbolNames());
            symbologyComboBox.Sorted = true;
            symbologyComboBox.SelectedIndex = 0;
        }

        private void SymbologyComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            symbolID = ZintNetLib.GetSymbolId(symbologyComboBox.Text);
            SetControlsAndOptions();
            SetBarCodeDefaults();
        }

        // Set the users requested properties and generates the barcode.
        private void SetBarcodeProperties()
        {
            // Common properties.
            myBarcode.ElementXDimension = 0.264583f;  // Equals 1 pixel at 96 dpi
            myBarcode.Multiplier = multiplierValue;
            myBarcode.TextMargin = textMargin;
            myBarcode.BarcodeColor = barcodeColor;
            myBarcode.BarcodeTextColor = textColor;
            myBarcode.Font = barcodeTextFont;
            myBarcode.Rotation = rotationAngle;
            myBarcode.BarcodeHeight = barHeight;
            myBarcode.TextVisible = showTextCheckBox.Checked;
            myBarcode.TextAlignment = textAlignment;
            myBarcode.TextPosition = textPosition;

            // Symbol specific properties.
            myBarcode.EncodingMode = encodingMode;
            if (compositeDataTextbox != null)
                compositeText = compositeDataTextbox.Text;

            if (supplementDataTextBox != null)
                supplementText = supplementDataTextBox.Text;

            // ITF14.
            if (symbolID == Symbology.ITF14)
                myBarcode.ITF14BearerStyle = itf14BearerStyle;

            // Code 39.
            if (symbolID == Symbology.Code39 || symbolID == Symbology.Code39Extended || symbolID == Symbology.Code93 || symbolID == Symbology.LOGMARS)
            {
                if (symbolID != Symbology.Code93)
                    myBarcode.OptionalCheckDigit = optionalCheckDigitCheckBox.Checked;

                myBarcode.ShowCheckDigit = showCheckDigitCheckBox.Checked;
            }

            // MaxiCode properties.
            if (symbolID == Symbology.MaxiCode)
            {
                myBarcode.MaxicodeMode = (MaxicodeMode)maxicodeModeComboBox.SelectedIndex + 2;
            }

            // DataMatrix Properties.
            if (symbolID == Symbology.DataMatrix)
            {
                myBarcode.DataMatrixSize = (DataMatrixSize)dmSizesComboBox.SelectedIndex;
                myBarcode.DataMatrixRectExtn = dmreCheckBox.Checked;
                myBarcode.DataMatrixSquare = squareOnlyCheckBox.Checked;
            }

            if (symbolID == Symbology.QRCode || symbolID == Symbology.MicroQRCode || symbolID == Symbology.RectangularMicroQRCode)
            {
                myBarcode.QRVersion = qrVersion;
                myBarcode.QRCodeEccLevel = qrErrorLevel;
            }

            if (symbolID == Symbology.Aztec)
            {
                myBarcode.AztecSize = aztecVersion;
                myBarcode.AztecErrorLevel = aztecErrorLevel;
            }

            if (symbolID == Symbology.HanXin)
            {
                myBarcode.HanXinVersion = hanXinVersion;
                myBarcode.HanXinErrorLevel = hanXinErrorLevel;
            }

            if (symbolID == Symbology.Ultracode)
            {
                myBarcode.UltracodeCompression = ultracodeCompression;
                myBarcode.UltracodeErrorLevel = ultracodeErrorLevel;
            }

            if (symbolID == Symbology.Code128)
            {
                if (encodingMode == EncodingMode.GS1)
                {
                    myBarcode.CompositeMode = compositeMode;
                    myBarcode.CompositeMessage = compositeText;
                }
            }

            if (myBarcode.IsGS1Databar())
            {
                myBarcode.CompositeMode = compositeMode;
                myBarcode.CompositeMessage = compositeText;
                if (symbolID == Symbology.DatabarExpandedStacked)
                    myBarcode.DatabarExpandedSegments = columnsComboBox.SelectedIndex * 2;
            }

            if (myBarcode.IsEanUpc())
            {
                if (symbolID != Symbology.ISBN)
                {
                    myBarcode.CompositeMode = compositeMode;
                    myBarcode.CompositeMessage = compositeText;
                }

                myBarcode.SupplementMessage = supplementText;
            }

            if (symbolID == Symbology.PDF417 || symbolID == Symbology.PDF417Truncated)
            {
                myBarcode.PDF417Columns = columnsComboBox.SelectedIndex;
                myBarcode.PDF417ErrorLevel = pdfErrorLevel;
                myBarcode.PDF417RowHeight = pdfRowHeightComboBox.SelectedIndex + 2;
            }

            if (symbolID == Symbology.MicroPDF417)
                myBarcode.PDF417Columns = columnsComboBox.SelectedIndex;

            if (symbolID == Symbology.DotCode)
            {
                myBarcode.ElementXDimension = 0.529166f;  // equals 2 pixels.
                myBarcode.DotCodeColumns = columnsComboBox.SelectedIndex;
            }

            if (symbolID == Symbology.CodeOne)
                myBarcode.CodeOneSize = columnsComboBox.SelectedIndex;

            if (symbolID == Symbology.ChannelCode)
                myBarcode.ChannelCodeLevel = columnsComboBox.SelectedIndex + 2;

            if(symbolID == Symbology.GridMatrix)
            {
                myBarcode.GridMatixVersion = gridMatrixVersion;
                myBarcode.GridMatrixEccLevel = gridMatrixErrorLevel;
            }

            BarcodeCreate();
        }

        private void BarcodeCreate()
        {
            if (myBarcode != null && !String.IsNullOrEmpty(barcodeData))
            {
                try
                {
                    myBarcode.CreateBarcode(symbolID, barcodeData);
                }

                catch (ZintNetDLLException ex)
                {
                    outputTextBox.Text = String.Empty;
                    string errorMessage = ex.Message;
                    if (ex.InnerException != null)
                        errorMessage += ex.InnerException.Message;

                    System.Windows.Forms.MessageBox.Show(errorMessage, "ZintNet Barcode Demo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                finally
                {
                    UpdateMenus();
                }
            }
        }

        private void UpdateMenus()
        {
            printToolStripMenuItem.Enabled = myBarcode.IsValid;
            saveAsToolStripMenuItem.Enabled = myBarcode.IsValid;
        }

        // Set some options and the controls depending on the users barcode symbol selection.
        private void SetControlsAndOptions()
        {
            outputTextBox.Text = String.Empty;
            barHeightNumericUpDown.Enabled = true;
            textMarginNumericUpDown.Enabled = true;
            showTextCheckBox.Enabled = true;
            RemoveRunTimeControls();
            switch (symbolID)
            {
                case Symbology.MaxiCode:
                    barHeightNumericUpDown.Enabled = false;
                    textMarginNumericUpDown.Enabled = false;
                    showTextCheckBox.Enabled = false;
                    AddTabPage();
                    AddMaxiCodeControls((int)myBarcode.MaxicodeMode - 2);
                    break;

                case Symbology.DataMatrix:
                    barHeightNumericUpDown.Enabled = false;
                    textMarginNumericUpDown.Enabled = false;
                    showTextCheckBox.Enabled = false;
                    AddTabPage();
                    AddDataMatrixControls();
                    break;

                case Symbology.QRCode:
                case Symbology.MicroQRCode:
                case Symbology.RectangularMicroQRCode:
                case Symbology.Aztec:
                case Symbology.HanXin:
                case Symbology.GridMatrix:
                    barHeightNumericUpDown.Enabled = false;
                    textMarginNumericUpDown.Enabled = false;
                    showTextCheckBox.Enabled = false;
                    AddTabPage();
                    Add2DControls();
                    break;

                case Symbology.AztecRunes:
                    barHeightNumericUpDown.Enabled = false;
                    textMarginNumericUpDown.Enabled = false;
                    showTextCheckBox.Enabled = false;
                    break;

                case Symbology.EAN13:
                case Symbology.EAN8:
                case Symbology.UPCA:
                case Symbology.UPCE:
                    textMarginNumericUpDown.Enabled = false;
                    showTextCheckBox.Enabled = false;
                    AddTabPage();
                    AddCompositeControls();
                    cccRadioButton.Enabled = false;
                    break;

                case Symbology.ISBN:
                    textMarginNumericUpDown.Enabled = false;
                    showTextCheckBox.Enabled = false;
                    AddTabPage();
                    AddSupplimentDataControls();
                    break;

                case Symbology.DatabarExpanded:
                case Symbology.DatabarExpandedStacked:
                case Symbology.DatabarLimited:
                case Symbology.DatabarOmni:
                case Symbology.DatabarOmniStacked:
                case Symbology.DatabarStacked:
                case Symbology.DatabarTruncated:
                    barHeightNumericUpDown.Enabled = false;
                    textMarginNumericUpDown.Enabled = false;
                    showTextCheckBox.Enabled = false;
                    AddTabPage();
                    AddCompositeControls();
                    cccRadioButton.Enabled = false;
                    break;

                case Symbology.Code128:
                    AddTabPage();
                    AddModeControls();
                    AddCompositeControls();
                    break;

                case Symbology.PDF417:
                case Symbology.PDF417Truncated:
                case Symbology.MicroPDF417:
                    barHeightNumericUpDown.Enabled = false;
                    textMarginNumericUpDown.Enabled = false;
                    showTextCheckBox.Enabled = false;
                    AddTabPage();
                    AddModeControls();
                    gs1RadioButton.Enabled = false;
                    AddPDFControls();
                    break;

                case Symbology.Code39:
                case Symbology.Code39Extended:
                case Symbology.Code93:
                    AddTabPage();
                    AddCode39Controls();
                    break;

                case Symbology.ITF14:
                    barHeightNumericUpDown.Enabled = true;
                    textMarginNumericUpDown.Enabled = true;
                    showTextCheckBox.Enabled = true;
                    AddTabPage();
                    AddITF14Controls();
                    break;

                case Symbology.CodablockF:
                    barHeightNumericUpDown.Enabled = false;
                    textMarginNumericUpDown.Enabled = false;
                    showTextCheckBox.Enabled = false;
                    break;

                case Symbology.DotCode:
                    barHeightNumericUpDown.Enabled = false;
                    textMarginNumericUpDown.Enabled = false;
                    showTextCheckBox.Enabled = false;
                    AddTabPage();
                    AddDotCodeControls();
                    hibcRadioButton.Enabled = false;
                    break;

                case Symbology.CodeOne:
                    barHeightNumericUpDown.Enabled = false;
                    textMarginNumericUpDown.Enabled = false;
                    showTextCheckBox.Enabled = false;
                    AddTabPage();
                    AddCodeOneControls();
                    hibcRadioButton.Enabled = false;
                    break;

                case Symbology.Code49:
                case Symbology.Code16K:
                    barHeightNumericUpDown.Enabled = false;
                    textMarginNumericUpDown.Enabled = false;
                    showTextCheckBox.Enabled = false;
                    AddTabPage();
                    AddModeControls();
                    hibcRadioButton.Enabled = false;
                    break;

                case Symbology.ChannelCode:
                    AddTabPage();
                    AddChannelCodeControls();
                    break;

                case Symbology.Ultracode:
                    barHeightNumericUpDown.Enabled = false;
                    textMarginNumericUpDown.Enabled = false;
                    showTextCheckBox.Enabled = false;
                    AddTabPage();
                    AddUltracodeControls();
                    hibcRadioButton.Enabled = false;
                    break;
            }
        }

        private void SetBarCodeDefaults()
        {
            qrVersion = 0;
            qrErrorLevel = (QRCodeEccLevel)(-1);

            aztecVersion = 0;
            aztecErrorLevel = -1;

            hanXinVersion = 0;
            hanXinErrorLevel = 0;

            encodingMode = EncodingMode.Standard;
            compositeMode = CompositeMode.CCA;

            // Barcodes string values.
            barcodeData = String.Empty;
            compositeText = String.Empty;
            supplementText = String.Empty;
        }

        #region Save As Image
        private void pNGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveTo(outputFile + ".png", ImageFormat.Png);
        }

        private void bMPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveTo(outputFile + ".bmp", ImageFormat.Bmp);
        }

        private void gIFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveTo(outputFile + ".gif", ImageFormat.Gif);
        }

        private void tIFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveTo(outputFile + ".tif", ImageFormat.Tiff);
        }

        private void SaveTo(string fileName, ImageFormat format)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.DefaultExt = Path.GetExtension(fileName);
                saveFileDialog.FileName = fileName;
                saveFileDialog.Title = "Save To Image";
                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    SaveToImage(saveFileDialog.FileName, format);
            }
        }

        /// <summary>
        /// Creates and saves an image of the barcode.
        /// </summary>
        /// <param name="fileName">save path and filename</param>
        /// <param name="imageFormat">image format to save as</param>
        private void SaveToImage(string fileName, ImageFormat imageFormat)
        {
            Rectangle section = Rectangle.Empty;
            Size symbolSize;
            Bitmap newBitmap = null;
            try
            {
                using (Bitmap bitmap = new Bitmap(10000, 10000))
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    if (myBarcode.IsValid)
                    {
                        // Keep a copy of rotation.
                        // Set roation 0 degrees.
                        // Rotate the graphics here!
                        int rotation = myBarcode.Rotation;
                        myBarcode.Rotation = 0;
                        graphics.Clear(Color.White);
                        myBarcode.ElementXDimension = 0.264583f;
                        myBarcode.DrawBarcode(graphics, new Point(2, 2));
                        symbolSize = myBarcode.SymbolSize(graphics);
                        section.Width = symbolSize.Width + 4;
                        section.Height = symbolSize.Height + 4;
                        newBitmap = CopyBitMapSection(bitmap, section);
                        switch (rotation)
                        {
                            case 90:
                                newBitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                break;

                            case 180:
                                newBitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                break;

                            case 270:
                                newBitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
                                break;
                        }

                        newBitmap.Save(fileName, imageFormat);
                        // Restore our rotation.
                        myBarcode.Rotation = rotation;
                    }
                }
            }

            catch (Exception ex)
            {
                throw new ZintNetDLLException("Error generating output image.", ex);
            }

            finally
            {
                if (newBitmap != null)
                    newBitmap.Dispose();
            }
        }

        private Bitmap CopyBitMapSection(Bitmap sourceBitmap, Rectangle section)
        {
            // Create the new bitmap and associated graphics object
            Bitmap bitmap = new Bitmap(section.Width, section.Height);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                // Copy the specified section of the source bitmap to the new one
                graphics.DrawImage(sourceBitmap, 0, 0, section, GraphicsUnit.Pixel);
            }

            return bitmap;
        }

        #endregion

        private void multiplierNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            multiplierValue = (float)multiplierNumericUpDown.Value;
            SetBarcodeProperties();
            imagePanel.Invalidate();
        }

        private void barcodeColorButton_Click(object sender, EventArgs e)
        {
            using (ColorDialog cd = new ColorDialog())
            {
                cd.Color = this.barcodeColorButton.BackColor;
                cd.ShowDialog();
                this.barcodeColorButton.BackColor = cd.Color;
                barcodeColor = cd.Color;
            }

            SetBarcodeProperties();
            imagePanel.Invalidate();
        }

        private void textColorButton_Click(object sender, EventArgs e)
        {
            using (ColorDialog cd = new ColorDialog())
            {
                cd.Color = this.textColorButton.BackColor;
                cd.ShowDialog();
                textColorButton.BackColor = cd.Color;
                textColor = cd.Color;
            }

            SetBarcodeProperties();
            imagePanel.Invalidate();
        }

        private void rotateButton_Click(object sender, EventArgs e)
        {
            rotationAngle += 90;
            if (rotationAngle > 270)
                rotationAngle = 0;

            rotateTextBox.Text = rotationAngle.ToString() + (char)176;
            SetBarcodeProperties();
            imagePanel.Invalidate();

        }

        private void generateButton_Click(object sender, EventArgs e)
        {
            barcodeData = barcodeDataTextBox.Text;
            SetBarcodeProperties();
            imagePanel.Invalidate();
        }

        private void barcodeDataTextBox_TextChanged(object sender, EventArgs e)
        {
            generateButton.Enabled = !string.IsNullOrEmpty(this.barcodeDataTextBox.Text);
        }

        private void fontButton_Click(object sender, EventArgs e)
        {
            using (FontDialog fd = new FontDialog())
            {
                fd.Font = barcodeTextFont;
                fd.ShowDialog();
                barcodeTextFont = fd.Font;
            }

            SetBarcodeProperties();
            imagePanel.Invalidate();
        }

        private void textMarginNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            textMargin = (float)textMarginNumericUpDown.Value;
            SetBarcodeProperties();
            imagePanel.Invalidate();
        }

        private void barHeightNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            barHeight = (float)barHeightNumericUpDown.Value;
            SetBarcodeProperties();
            imagePanel.Invalidate();
        }

        private void showTextCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetBarcodeProperties();
            imagePanel.Invalidate();
        }

        private void showCheckDigitCheckBox_CheckedChange(object sender, EventArgs e)
        {
            SetBarcodeProperties();
            imagePanel.Invalidate();
        }

        private void optionalCheckDigitCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetBarcodeProperties();
            imagePanel.Invalidate();
        }

        private void textPositionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            textPosition = (TextPosition)(textPositionComboBox.SelectedIndex);
            SetBarcodeProperties();
            imagePanel.Invalidate();
        }

        private void textAlignComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            textAlignment = (TextAlignment)(textAlignComboBox.SelectedIndex);
            SetBarcodeProperties();
            imagePanel.Invalidate();
        }

        private void ColumnsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetBarcodeProperties();
            imagePanel.Invalidate();
        }

        private void maxicodeModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetBarcodeProperties();
            imagePanel.Invalidate();
        }

        #region Data Matrix Controls Events
        private void dmSizesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetBarcodeProperties();
            imagePanel.Invalidate();
        }

        private void squareOnlyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetBarcodeProperties();
            imagePanel.Invalidate();
        }

        private void dmreCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetBarcodeProperties();
            imagePanel.Invalidate();
        }
        #endregion

        #region 2D Shared Controls Events
        private void Auto2DRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (autoSize2DRadioButton.Checked)
            {
                sizes2DComboBox.Enabled = false;
                errorCorrection2DComboBox.Enabled = false;
                qrVersion = 0;
                qrErrorLevel = (QRCodeEccLevel) (-1);
                aztecVersion = 0;
                aztecErrorLevel = -1;
                hanXinVersion = 0;
                hanXinErrorLevel = -1;
                gridMatrixVersion = 0;
                gridMatrixErrorLevel = -1;
                SetBarcodeProperties();
                imagePanel.Invalidate();
            }
        }

        private void Sizes2DRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (sizes2DRadioButton.Checked)
            {
                sizes2DComboBox.Enabled = true;
                errorCorrection2DComboBox.Enabled = false;
                qrVersion = sizes2DComboBox.SelectedIndex + 1;
                qrErrorLevel = (QRCodeEccLevel)(-1);
                aztecVersion = sizes2DComboBox.SelectedIndex + 1;
                aztecErrorLevel = -1;
                hanXinVersion = sizes2DComboBox.SelectedIndex + 1; ;
                hanXinErrorLevel = -1;
                gridMatrixVersion = sizes2DComboBox.SelectedIndex + 1;
                gridMatrixErrorLevel = -1;
                SetBarcodeProperties();
                imagePanel.Invalidate();
            }
        }

        private void Errorcorrection2DRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (errorCorrection2DRadioButton.Checked)
            {
                errorCorrection2DComboBox.Enabled = true;
                sizes2DComboBox.Enabled = false;
                qrVersion = 0;
                qrErrorLevel = (QRCodeEccLevel)errorCorrection2DComboBox.SelectedIndex;
                if (symbolID == Symbology.RectangularMicroQRCode)
                {
                    int level = errorCorrection2DComboBox.SelectedIndex;
                    if (level == 0)
                        qrErrorLevel = QRCodeEccLevel.Medium;

                    else
                        qrErrorLevel = QRCodeEccLevel.High;
                }

                aztecVersion = 0;
                aztecErrorLevel = errorCorrection2DComboBox.SelectedIndex + 1;
                hanXinVersion = 0;
                hanXinErrorLevel = errorCorrection2DComboBox.SelectedIndex + 1;
                gridMatrixVersion = 0;
                gridMatrixErrorLevel = errorCorrection2DComboBox.SelectedIndex + 1;
                SetBarcodeProperties();
                imagePanel.Invalidate();
            }
        }

        private void Sizes2DComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            qrVersion = sizes2DComboBox.SelectedIndex + 1;
            if (symbolID == Symbology.RectangularMicroQRCode)
            {
                int level = errorCorrection2DComboBox.SelectedIndex;
                if (level == 0)
                    qrErrorLevel = QRCodeEccLevel.Medium;

                else
                    qrErrorLevel = QRCodeEccLevel.High;
            }

            aztecVersion = sizes2DComboBox.SelectedIndex + 1;
            aztecErrorLevel = -1;
            hanXinVersion = sizes2DComboBox.SelectedIndex + 1;
            hanXinErrorLevel = -1;
            gridMatrixVersion = sizes2DComboBox.SelectedIndex + 1;
            gridMatrixErrorLevel = -1;
            SetBarcodeProperties();
            imagePanel.Invalidate();
        }

        private void ErrorCorrection2DComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            qrVersion = 0;
            qrErrorLevel = (QRCodeEccLevel)errorCorrection2DComboBox.SelectedIndex;
            if (symbolID == Symbology.RectangularMicroQRCode)
            {
                int level = errorCorrection2DComboBox.SelectedIndex;
                if (level == 0)
                    qrErrorLevel = QRCodeEccLevel.Medium;

                else
                    qrErrorLevel = QRCodeEccLevel.High;
            }

            aztecVersion = 0;
            aztecErrorLevel = errorCorrection2DComboBox.SelectedIndex + 1;
            hanXinVersion = 0;
            hanXinErrorLevel = errorCorrection2DComboBox.SelectedIndex + 1;
            gridMatrixVersion = 0;
            gridMatrixErrorLevel = errorCorrection2DComboBox.SelectedIndex + 1;
            SetBarcodeProperties();
            imagePanel.Invalidate();
        }

        #endregion

        private void ErrorCorrectionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ultracodeErrorLevel = errorLevelComboBox.SelectedIndex;
            SetBarcodeProperties();
            imagePanel.Invalidate();
        }

        #region Mode Controls Events
        private void standardRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (standardRadioButton.Checked)
            {
                encodingMode = EncodingMode.Standard;
                SetBarcodeProperties();
                imagePanel.Invalidate();
            }
        }

        private void gs1RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (gs1RadioButton.Checked)
            {
                if (symbolID == Symbology.Code128)
                {
                    compositeGroupBox.Enabled = true;
                    compositeDataTextbox.Enabled = true;
                }

                encodingMode = EncodingMode.GS1;
                SetBarcodeProperties();
                imagePanel.Invalidate();
            }

            else if (compositeGroupBox != null)
            {
                compositeGroupBox.Enabled = false;
                compositeDataTextbox.Enabled = false;
            }
        }

        private void hibcRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (hibcRadioButton.Checked)
            {
                encodingMode = EncodingMode.HIBC;
                SetBarcodeProperties();
                imagePanel.Invalidate();
            }
        }

        #endregion

        #region Composite Controls Events
        private void ccaRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (ccaRadioButton.Checked)
            {
                compositeMode = CompositeMode.CCA;
                SetBarcodeProperties();
                imagePanel.Invalidate();
            }
        }

        private void ccbRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (ccbRadioButton.Checked)
            {
                compositeMode = CompositeMode.CCB;
                SetBarcodeProperties();
                imagePanel.Invalidate();
            }
        }

        private void cccRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (cccRadioButton.Checked)
            {
                compositeMode = CompositeMode.CCC;
                SetBarcodeProperties();
                imagePanel.Invalidate();
            }
        }

        #endregion

        #region PDF417 Control Events
        private void pdfErrorLevelComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            pdfErrorLevel = errorLevelComboBox.SelectedIndex - 1;
            SetBarcodeProperties();
            imagePanel.Invalidate();
        }

        private void pdfRowHeightComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetBarcodeProperties();
            imagePanel.Invalidate();
        }
        #endregion

        #region ITF14 Controls Events
        private void noneRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (noneRadioButton.Checked)
            {
                itf14BearerStyle = ITF14BearerStyle.None;
                SetBarcodeProperties();
                imagePanel.Invalidate();
            }
        }

        private void horizonalRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (horizonalRadioButton.Checked)
            {
                itf14BearerStyle = ITF14BearerStyle.Horizonal;
                SetBarcodeProperties();
                imagePanel.Invalidate();
            }
        }

        private void rectangleRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (rectangleRadioButton.Checked)
            {
                itf14BearerStyle = ITF14BearerStyle.Rectangle;
                SetBarcodeProperties();
                imagePanel.Invalidate();
            }
        }

        #endregion
    }
}



