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
        private int gridMatrixErrorLevel = 0;

        private int ultracodeCompression = 0;
        private int ultracodeErrorLevel = -1;

        private int pdfErrorLevel = -1;

        private EncodingFormat encodingMode = EncodingFormat.Standard;
        private CompositeMode compositeMode = CompositeMode.CCA;

        // Barcodes string values.
        private string barcodeData = string.Empty;
        private string compositeText = string.Empty;
        private string supplementText = string.Empty;

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
            {
                GetSymbologies();
            }

            // Set some menu options.
            printToolStripMenuItem.Enabled = false;
            saveAsToolStripMenuItem.Enabled = false;
            generateButton.Enabled = false;
            textMarginNumericUpDown.Value = (decimal)(myBarcode.TextMargin);
            heightNumericUpDown.Value = (decimal)(myBarcode.BarcodeHeight);
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
            {
                myBarcode.Dispose();
            }

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
                    outputTextBox.Text = string.Empty;
                    string errorMessage = ex.Message;
                    if (ex.InnerException != null)
                    {
                        errorMessage += ex.InnerException.Message;
                    }

                    MessageBox.Show(errorMessage, "ZintNet Barcode Demo", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            if (!string.IsNullOrEmpty(barcodeDataTextBox.Text))
            {
                generateButton.Enabled = true;
                barcodeData = barcodeDataTextBox.Text;
            }
        }

        // Set the users requested properties and generates the barcode.
        private bool SetBarcodeProperties()
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
            myBarcode.RowSeparatorHeight = 1;

            // Symbol specific properties.
            myBarcode.EncodingMode = encodingMode;
            if (compositeDataTextbox != null)
            {
                compositeText = compositeDataTextbox.Text;
            }

            if (supplementDataTextBox != null)
            {
                supplementText = supplementDataTextBox.Text;
            }

            // Aztec Code.
            if (symbolID == Symbology.Aztec)
            {
                if (autoResizeRadioButton.Checked)
                {
                    // Automatic resizing.
                    myBarcode.AztecSize = 0;
                    myBarcode.AztecErrorLevel = -1;
                }

                else if (sizesRadioButton.Checked)
                {
                    // Adjust to size.
                    myBarcode.AztecSize = versionSizesComboBox.SelectedIndex + 1;
                }

                else
                {
                    // Add error correction.
                    myBarcode.AztecSize = 0;
                    myBarcode.AztecErrorLevel = errorCorrectionComboBox.SelectedIndex + 1;
                }
            }

            // Channel Code.
            if (symbolID == Symbology.ChannelCode)
            {
                myBarcode.ChannelCodeLevel = columnsComboBox.SelectedIndex + 2;
            }

            // Codabar.
            if (symbolID == Symbology.Codabar)
            {
                myBarcode.OptionalCheckDigit = checkDigitCheckBox.Checked;
                myBarcode.ShowCheckDigit = showCheckDigitCheckBox.Checked;
            }

            // Codablock F.
            if (symbolID == Symbology.CodablockF)
            {
                int value;

                // 0 = Automatic, valid range 9 - 67.
                value = columnsComboBox.SelectedIndex;
                if (value > 0)
                {
                    value += 8;
                }

                myBarcode.CodablockFColumns = value;

                // 0 = Automatic, valid range 2 - 44.
                value = rowsComboBox.SelectedIndex;
                if (value > 0)
                {
                    value += 1;
                }

                myBarcode.CodablockFRows = value;

                // Valid range 1-4;
                value = separatorHeightComboBox.SelectedIndex + 1;
                if (value > 4)
                {
                    value = 4;
                }

                myBarcode.RowSeparatorHeight = value;
            }

            // Code 128.
            if (symbolID == Symbology.Code128)
            {
                if (encodingMode == EncodingFormat.GS1)
                {
                    myBarcode.CompositeMode = compositeMode;
                    myBarcode.CompositeMessage = compositeText;
                }

                myBarcode.Code128SuppressCodeC = codeSetCCheckBox.Checked;
            }

            // Code 16K and Code 49.
            if (symbolID == Symbology.Code16K || symbolID == Symbology.Code49)
            {
                int value;

                // 0 = Automatic, valid range 3-16.
                value = rowsComboBox.SelectedIndex;
                if (value > 0)
                {
                    value += 2;
                }

                myBarcode.Code16KMinimumRows = value;

                // Valid range 1-4;
                value = separatorHeightComboBox.SelectedIndex + 1;
                if (value > 4)
                {
                    value = 4;
                }

                myBarcode.RowSeparatorHeight = value;
            }

            // Code 39.
            if (symbolID == Symbology.Code39 || symbolID == Symbology.Code39Extended || symbolID == Symbology.Code93 || symbolID == Symbology.LOGMARS)
            {
                if (symbolID != Symbology.Code93)
                {
                    myBarcode.OptionalCheckDigit = checkDigitCheckBox.Checked;
                }

                myBarcode.ShowCheckDigit = showCheckDigitCheckBox.Checked;
            }

            // Code One
            if (symbolID == Symbology.CodeOne)
            {
                // 0 = Automatic, valid range 1-10.
                myBarcode.CodeOneSize = versionSizesComboBox.SelectedIndex;
            }

            if (symbolID == Symbology.Industrial2of5 || symbolID == Symbology.Standard2of5 ||
                symbolID == Symbology.IATA2of5 || symbolID == Symbology.DataLogic2of5 || symbolID == Symbology.Interleaved2of5)
            {
                myBarcode.OptionalCheckDigit = checkDigitCheckBox.Checked;
                myBarcode.ShowCheckDigit = showCheckDigitCheckBox.Checked;
            }

            // DataMatrix Properties.
            if (symbolID == Symbology.DataMatrix)
            {
                int index = versionSizesComboBox.SelectedIndex;
                squareOnlyCheckBox.Enabled = index == 0;
                dmreCheckBox.Enabled = index == 0 && !squareOnlyCheckBox.Checked;
                myBarcode.DataMatrixSize = (DataMatrixSize)index;
                myBarcode.DataMatrixRectExtn = dmreCheckBox.Checked;
                myBarcode.DataMatrixSquare = squareOnlyCheckBox.Checked;
            }

            // Grid Matrix.
            if (symbolID == Symbology.GridMatrix)
            {
                myBarcode.GridMatixVersion = versionSizesComboBox.SelectedIndex;
                myBarcode.GridMatrixEccLevel = errorCorrectionComboBox.SelectedIndex;
            }

            // Han Xin.
            if (symbolID == Symbology.HanXin)
            {
                myBarcode.HanXinVersion = versionSizesComboBox.SelectedIndex;
                myBarcode.HanXinErrorLevel = errorCorrectionComboBox.SelectedIndex;
                myBarcode.UserMask = userMaskComboBox.SelectedIndex;
            }

            // ITF14.
            if (symbolID == Symbology.ITF14)
            {
                myBarcode.OptionalCheckDigit = false;
                myBarcode.ShowCheckDigit = false;
                myBarcode.ITF14BearerStyle = itf14BearerStyle;
            }

            // Vin Code.
            if (symbolID == Symbology.VINCode)
            {
                myBarcode.OptionalCheckDigit = false;
                myBarcode.ShowCheckDigit = false;
            }

            // MaxiCode properties.
            if (symbolID == Symbology.MaxiCode)
            {
                myBarcode.MaxicodeMode = (MaxicodeMode)maxicodeModeComboBox.SelectedIndex + 2;
            }

            // QR code.
            if (symbolID == Symbology.QRCode || symbolID == Symbology.MicroQRCode || symbolID == Symbology.RectangularMicroQRCode)
            {
                myBarcode.QRVersion = versionSizesComboBox.SelectedIndex;
                if (symbolID == Symbology.RectangularMicroQRCode)
                {
                    // rMQR only supports Medium and High ECC.
                    int value = errorCorrectionComboBox.SelectedIndex - 1;
                    if (value == 0)
                    {
                        myBarcode.QRCodeEccLevel = QRCodeEccLevel.Medium;
                    }

                    else if (value == 1)
                    {
                        myBarcode.QRCodeEccLevel = QRCodeEccLevel.High;
                    }

                    else
                    {
                        myBarcode.QRCodeEccLevel = QRCodeEccLevel.Automatic;
                    }
                }

                else
                {
                    myBarcode.QRCodeEccLevel = (QRCodeEccLevel)errorCorrectionComboBox.SelectedIndex - 1;
                    myBarcode.UserMask = userMaskComboBox.SelectedIndex;
                }
            }

            if (symbolID == Symbology.Ultracode)
            {
                myBarcode.UltracodeCompression = ultracodeCompression;
                myBarcode.UltracodeErrorLevel = ultracodeErrorLevel;
            }

            if (myBarcode.IsGS1Databar())
            {
                myBarcode.CompositeMode = compositeMode;
                myBarcode.CompositeMessage = compositeText;
                if (symbolID == Symbology.DatabarExpandedStacked)
                {
                    myBarcode.DatabarExpandedSegments = columnsComboBox.SelectedIndex * 2;
                }
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
            {
                myBarcode.PDF417Columns = columnsComboBox.SelectedIndex;
            }

            if (symbolID == Symbology.DotCode)
            {
                myBarcode.ElementXDimension = 0.529166f;  // equals 2 pixels.
                myBarcode.DotCodeColumns = columnsComboBox.SelectedIndex;
            }

            if(symbolID == Symbology.UPNQR)
            {
                myBarcode.UserMask = userMaskComboBox.SelectedIndex;
            }

            return BarcodeCreate();
        }

        private void UpdateProperties()
        {
            switch (symbolID)
            {
                case Symbology.Aztec:
                    versionSizesComboBox.SelectedIndex = myBarcode.AztecSize;
                    break;
            }
        }

        private bool BarcodeCreate()
        {
            bool result = false;

            if (myBarcode != null && !string.IsNullOrEmpty(barcodeData))
            {
                try
                {
                    myBarcode.CreateBarcode(symbolID, barcodeData);
                    result = true;
                }

                catch (ZintNetDLLException ex)
                {
                    outputTextBox.Text = string.Empty;
                    string errorMessage = ex.Message;
                    if (ex.InnerException != null)
                    {
                        errorMessage += ex.InnerException.Message;
                    }

                    MessageBox.Show(errorMessage, "ZintNet Barcode Demo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                finally
                {
                    UpdateMenus();
                }
            }

            //UpdateProperties();
            return result;
        }

        private void UpdateMenus()
        {
            printToolStripMenuItem.Enabled = myBarcode.IsValid;
            saveAsToolStripMenuItem.Enabled = myBarcode.IsValid;
        }

        // Set some options and the controls depending on the users barcode symbol selection.
        private void SetControlsAndOptions()
        {
            foreach (Control c in commomPropertiesTabPage.Controls)
            {
                c.Enabled = false;
            }

            outputTextBox.Text = string.Empty;
            multiplierLabel.Enabled = true;
            multiplierNumericUpDown.Enabled = true;
            barcodeColorLabel.Enabled = true;
            barcodeColorButton.Enabled = true;
            rotateButton.Enabled = true;

            RemoveRunTimeControls();

            switch (symbolID)
            {
                case Symbology.AusPostRedirect:
                case Symbology.AusPostReplyPaid:
                case Symbology.AusPostRouting:
                case Symbology.AusPostStandard:
                case Symbology.KixCode:
                case Symbology.PLANET:
                case Symbology.POSTNET:
                case Symbology.RoyalMail4SCC:
                case Symbology.RoyalMailMailmark:
                case Symbology.JapanPost:
                case Symbology.DXFilmEdge:
                case Symbology.FIM:
                    SetTextControls(false);
                    break;

                case Symbology.Aztec:
                    AddTabPage();
                    AddAztecControls();
                    break;

                case Symbology.ChannelCode:
                    heightLabel.Enabled = true;
                    heightNumericUpDown.Enabled = true;
                    SetTextControls(true);
                    AddTabPage();
                    AddChannelCodeControls();
                    break;

                case Symbology.CodablockF:
                    AddTabPage();
                    AddCodablockControls();
                    break;

                // Code 128 based.
                case Symbology.Code128:
                    heightLabel.Enabled = true;
                    heightNumericUpDown.Enabled = true;
                    SetTextControls(true);
                    AddTabPage();
                    AddCode128Controls();
                    break;

                case Symbology.Code16K:
                case Symbology.Code49:
                    AddTabPage();
                    AddCode16KCode49Controls();
                    break;

                case Symbology.CodeOne:
                    heightNumericUpDown.Enabled = false;
                    textMarginNumericUpDown.Enabled = false;
                    showTextCheckBox.Enabled = false;
                    AddTabPage();
                    AddCodeOneControls();
                    hibcRadioButton.Enabled = false;
                    break;

                // Code 39 based.
                case Symbology.Code39:
                case Symbology.Code39Extended:
                    SetTextControls(true);
                    heightLabel.Enabled = true;
                    heightNumericUpDown.Enabled = true;
                    checkDigitCheckBox.Checked = true;
                    showCheckDigitCheckBox.Checked = true;
                    checkDigitCheckBox.Enabled = true;
                    showCheckDigitCheckBox.Enabled = true;
                    AddTabPage();
                    AddCode39Controls();
                    break;

                case Symbology.Code93:
                    SetTextControls(true);
                    heightLabel.Enabled = true;
                    heightNumericUpDown.Enabled = true;
                    showCheckDigitCheckBox.Checked = true;
                    checkDigitCheckBox.Enabled = false;
                    showCheckDigitCheckBox.Enabled = true;
                    break;

                case Symbology.DataMatrix:
                    heightNumericUpDown.Enabled = false;
                    textMarginNumericUpDown.Enabled = false;
                    showTextCheckBox.Enabled = false;
                    AddTabPage();
                    AddDataMatrixControls();
                    break;

                case Symbology.DotCode:
                    heightNumericUpDown.Enabled = false;
                    textMarginNumericUpDown.Enabled = false;
                    showTextCheckBox.Enabled = false;
                    AddTabPage();
                    AddDotCodeControls();
                    hibcRadioButton.Enabled = false;
                    break;

                case Symbology.GridMatrix:
                    heightNumericUpDown.Enabled = false;
                    textMarginNumericUpDown.Enabled = false;
                    showTextCheckBox.Enabled = false;
                    AddTabPage();
                    AddGridMatrixControls();
                    break;

                case Symbology.HanXin:
                    heightNumericUpDown.Enabled = false;
                    textMarginNumericUpDown.Enabled = false;
                    showTextCheckBox.Enabled = false;
                    AddTabPage();
                    AddHanXinControls();
                    break;

                case Symbology.MaxiCode:
                    heightNumericUpDown.Enabled = false;
                    textMarginNumericUpDown.Enabled = false;
                    showTextCheckBox.Enabled = false;
                    AddTabPage();
                    AddMaxiCodeControls((int)myBarcode.MaxicodeMode - 2);
                    break;

                case Symbology.QRCode:
                case Symbology.MicroQRCode:
                case Symbology.RectangularMicroQRCode:
                    AddTabPage();
                    AddQRCodeControls();
                    break;

                case Symbology.DPDCode:
                case Symbology.DeutschePostIdentCode:
                case Symbology.DeutschePostLeitCode:
                case Symbology.KoreaPost:
                case Symbology.BC412:
                    heightLabel.Enabled = true;
                    heightNumericUpDown.Enabled = true;
                    SetTextControls(true);
                    break;

                case Symbology.Standard2of5:
                case Symbology.Interleaved2of5:
                case Symbology.Industrial2of5:
                case Symbology.IATA2of5:
                case Symbology.DataLogic2of5:
                case Symbology.Code11:
                case Symbology.Codabar:
                    heightLabel.Enabled = true;
                    heightNumericUpDown.Enabled = true;
                    SetTextControls(true);
                    checkDigitCheckBox.Enabled = true;
                    showCheckDigitCheckBox.Enabled = true;
                    break;

                case Symbology.ITF14:
                    heightNumericUpDown.Enabled = true;
                    textMarginNumericUpDown.Enabled = true;
                    heightLabel.Enabled = true;
                    heightNumericUpDown.Enabled = true;
                    SetTextControls(true);
                    AddTabPage();
                    AddITF14Controls();
                    break;

                case Symbology.ISBN:
                case Symbology.EAN13:
                case Symbology.EAN8:
                case Symbology.UPCA:
                case Symbology.UPCE:
                    heightLabel.Enabled = true;
                    heightNumericUpDown.Enabled = true;
                    SetTextControls(true);
                    textAlignmentLabel.Enabled = false;
                    textAlignComboBox.Enabled = false;
                    textPositionLabel.Enabled = false;
                    textPositionComboBox.Enabled = false;
                    textMarginNumericUpDown.Enabled = false;
                    showTextCheckBox.Enabled = false;
                    checkDigitCheckBox.Enabled = false;
                    showCheckDigitCheckBox.Enabled = false;
                    AddTabPage();
                    AddEanUpcControls();
                    if (symbolID != Symbology.ISBN)
                    {
                        cccRadioButton.Enabled = false;
                    }

                    break;

                case Symbology.DatabarExpanded:
                case Symbology.DatabarExpandedStacked:
                case Symbology.DatabarLimited:
                case Symbology.DatabarOmni:
                case Symbology.DatabarOmniStacked:
                case Symbology.DatabarStacked:
                case Symbology.DatabarTruncated:
                    heightNumericUpDown.Enabled = false;
                    textMarginNumericUpDown.Enabled = false;
                    showTextCheckBox.Enabled = false;
                    checkDigitCheckBox.Enabled = false;
                    showCheckDigitCheckBox.Enabled = false;
                    AddTabPage();
                    //AddCompositeControls();
                    cccRadioButton.Enabled = false;
                    break;

                case Symbology.SSCC18:
                case Symbology.EAN14:
                case Symbology.UPUS10Code:
                    checkDigitCheckBox.Enabled = false;
                    showCheckDigitCheckBox.Enabled = false;
                    break;

                // PDF417.
                case Symbology.PDF417:
                case Symbology.PDF417Truncated:
                case Symbology.MicroPDF417:
                    heightNumericUpDown.Enabled = false;
                    textMarginNumericUpDown.Enabled = false;
                    showTextCheckBox.Enabled = false;
                    AddTabPage();
                    AddModeControls();
                    gs1RadioButton.Enabled = false;
                    AddPDFControls();
                    break;

                case Symbology.VINCode:
                case Symbology.Code32:
                case Symbology.PharmaZentralNummer:
                case Symbology.Pharmacode:
                case Symbology.Pharmacode2Track:
                case Symbology.UKPlessey:
                    checkDigitCheckBox.Checked = false;
                    showCheckDigitCheckBox.Checked = false;
                    checkDigitCheckBox.Enabled = false;
                    showCheckDigitCheckBox.Enabled = false;
                    break;

                case Symbology.Ultracode:
                    heightNumericUpDown.Enabled = false;
                    textMarginNumericUpDown.Enabled = false;
                    showTextCheckBox.Enabled = false;
                    AddTabPage();
                    AddUltracodeControls();
                    hibcRadioButton.Enabled = false;
                    break;

                case Symbology.UPNQR:
                    AddTabPage();
                    AddUPNQRControls();
                    break;
            }
        }

        private void SetTextControls(bool showText)
        {
            showTextCheckBox.Enabled = true;
            showTextCheckBox.Checked = showText;
            textFontLabel.Enabled = showText;
            textFontButton.Enabled = showText;
            textMarginLabel.Enabled = showText;
            textMarginNumericUpDown.Enabled = showText;
            textColorLabel.Enabled = showText;
            textColorButton.Enabled = showText;
            textAlignmentLabel.Enabled = showText;
            textAlignComboBox.Enabled = showText;
            textPositionLabel.Enabled = showText;
            textPositionComboBox.Enabled = showText;
            textMarginNumericUpDown.Enabled = showText;
        }

        private void SetBarCodeDefaults()
        {
            qrVersion = 0;
            qrErrorLevel = (QRCodeEccLevel)(-1);

            aztecVersion = 0;
            aztecErrorLevel = -1;

            hanXinVersion = 0;
            hanXinErrorLevel = 0;

            encodingMode = EncodingFormat.Standard;
            compositeMode = CompositeMode.CCA;

            // Barcodes string values.
            barcodeData = string.Empty;
            compositeText = string.Empty;
            supplementText = string.Empty;
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
                {
                    SaveToImage(saveFileDialog.FileName, format);
                }
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
                {
                    newBitmap.Dispose();
                }
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

        private void MultiplierNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            multiplierValue = (float)multiplierNumericUpDown.Value;
            if (SetBarcodeProperties())
            {
                imagePanel.Invalidate();
            }
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
            {
                rotationAngle = 0;
            }

            rotateTextBox.Text = rotationAngle.ToString() + (char)176;
            SetBarcodeProperties();
            imagePanel.Invalidate();

        }

        private void GenerateButton_Click(object sender, EventArgs e)
        {
            SetBarcodeProperties();
            imagePanel.Invalidate();
        }

        private void BarcodeDataTextBox_TextChanged(object sender, EventArgs e)
        {
            barcodeData = barcodeDataTextBox.Text;
            generateButton.Enabled = !string.IsNullOrEmpty(barcodeData);
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
            barHeight = (float)heightNumericUpDown.Value;
            SetBarcodeProperties();
            imagePanel.Invalidate();
        }

        private void ShowTextCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetTextControls(showTextCheckBox.Checked);
            SetBarcodeProperties();
            imagePanel.Invalidate();
        }

        private void ShowCheckDigitCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetBarcodeProperties();
            imagePanel.Invalidate();
        }
        private void CheckDigitCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetBarcodeProperties();
            imagePanel.Invalidate();
        }

        private void showOptCheckDigitCheckBox_CheckedChange(object sender, EventArgs e)
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

        private void RowsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetBarcodeProperties();
            imagePanel.Invalidate();
        }

        private void SeparatorHeightComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetBarcodeProperties();
            imagePanel.Invalidate();
        }

        private void CodeSetCCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetBarcodeProperties();
            imagePanel.Invalidate();
        }


        private void MaxicodeModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetBarcodeProperties();
            imagePanel.Invalidate();
        }

        #region Data Matrix Controls Events
        /*private void dmSizesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetBarcodeProperties();
            imagePanel.Invalidate();
        }*/

        private void SquareOnlyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetBarcodeProperties();
            imagePanel.Invalidate();
        }

        private void DmreCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetBarcodeProperties();
            imagePanel.Invalidate();
        }
        #endregion

        #region 2D Shared Controls Events

        // Aztec Auto Resize button event.
        private void AutoResizeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (autoResizeRadioButton.Checked)
            {
                versionSizesComboBox.Enabled = false;
                errorCorrectionComboBox.Enabled = false;
                SetBarcodeProperties();
                imagePanel.Invalidate();
            }
        }

        private void SizesRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (sizesRadioButton.Checked)
            {
                versionSizesComboBox.Enabled = true;
                errorCorrectionComboBox.Enabled = false;
                qrVersion = versionSizesComboBox.SelectedIndex + 1;
                qrErrorLevel = (QRCodeEccLevel)(-1);

                SetBarcodeProperties();
                imagePanel.Invalidate();
            }
        }

        private void ErrorCorrectionRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (errorCorrectionRadioButton.Checked)
            {
                errorCorrectionComboBox.Enabled = true;
                versionSizesComboBox.Enabled = false;
                qrVersion = 0;
                qrErrorLevel = (QRCodeEccLevel)errorCorrectionComboBox.SelectedIndex;
                if (symbolID == Symbology.RectangularMicroQRCode)
                {
                    int level = errorCorrectionComboBox.SelectedIndex;
                    if (level == 0)
                    {
                        qrErrorLevel = QRCodeEccLevel.Medium;
                    }
                    else
                    {
                        qrErrorLevel = QRCodeEccLevel.High;
                    }
                }

                SetBarcodeProperties();
                imagePanel.Invalidate();
            }
        }

        private void SymbolSizesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            qrVersion = versionSizesComboBox.SelectedIndex + 1;
            if (symbolID == Symbology.RectangularMicroQRCode)
            {
                int level = errorCorrectionComboBox.SelectedIndex;
                if (level == 0)
                {
                    qrErrorLevel = QRCodeEccLevel.Medium;
                }

                else
                {
                    qrErrorLevel = QRCodeEccLevel.High;
                }
            }

            if (versionSizesComboBox.Enabled)
            {
                SetBarcodeProperties();
                imagePanel.Invalidate();
            }
        }

        private void UserMaskComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetBarcodeProperties();
            imagePanel.Invalidate();
        }

        private void ErrorCorrectionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            qrVersion = 0;
            qrErrorLevel = (QRCodeEccLevel)errorCorrectionComboBox.SelectedIndex;
            if (symbolID == Symbology.RectangularMicroQRCode)
            {
                int level = errorCorrectionComboBox.SelectedIndex;
                if (level == 0)
                {
                    qrErrorLevel = QRCodeEccLevel.Medium;
                }
                else
                {
                    qrErrorLevel = QRCodeEccLevel.High;
                }
            }

            SetBarcodeProperties();
            imagePanel.Invalidate();
        }

        #endregion

        #region Mode Controls Events
        private void StandardRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (standardRadioButton.Checked)
            {
                encodingMode = EncodingFormat.Standard;
                SetBarcodeProperties();
                imagePanel.Invalidate();
            }
        }

        private void GS1RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (gs1RadioButton.Checked)
            {
                encodingMode = EncodingFormat.GS1;
                SetBarcodeProperties();
                imagePanel.Invalidate();
            }

            if (symbolID == Symbology.Code128)
            {
                codeSetCCheckBox.Checked = false;
                codeSetCCheckBox.Enabled = !gs1RadioButton.Checked;
                compositeDataLabel.Enabled = gs1RadioButton.Checked;
                compositeGroupBox.Enabled = gs1RadioButton.Checked;
                compositeDataTextbox.Enabled = gs1RadioButton.Checked;
            }
        }

        private void HIBCRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (hibcRadioButton.Checked)
            {
                encodingMode = EncodingFormat.HIBC;
                SetBarcodeProperties();
                imagePanel.Invalidate();
            }

            if (symbolID == Symbology.Code128)
            {
                codeSetCCheckBox.Checked = false;
                codeSetCCheckBox.Enabled = false;
                compositeDataLabel.Enabled = false;
                compositeGroupBox.Enabled = false;
                compositeDataTextbox.Enabled = false;
            }
        }

        #endregion

        #region Composite Controls Events.
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



