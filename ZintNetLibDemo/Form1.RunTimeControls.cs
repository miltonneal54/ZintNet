using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ZintNet;

namespace ZintNetLibTest
{
    partial class Form12
    {
        private static string[] dmSizes = {
            "Automatic", "10 x 10", "12 x 12", "14 x 14", "16 x 16", "18 x 18", "20 x 20", "22 x 22", "24 x 24", "26 x 26", "32 x 32", "36 x 36", 
            "40 x 40", "44 x 44", "48 x 48", "52 x 52", "64 x 64", "72 x 72", "80 x 80", "88 x 88", "96 x 96", "104 x 104", "120 x 120", "132 x 132",
            "144 x 144", "8 x 18", "8 x 32", "12 x 26", "12 x 36", "16 x 36", "16 x 48", "8 x 48", "8 x 64", "12 x 64", "16 x 64", "24 x 32", "24 x 40",
            "24 x 48", "24 x 64", "26 x 32", "26 x 40", "26 x 48","26 x 64" };

        private static string[] qrSizes = {
            "21 x 21 (Version 1)", "25 x 25 (Version 2)", "29 x 29 (Version 3)", "33 x 33 (Version 4)", "37 x 37 (Version 5)", "41 x 41 (Version 6)",
            "45 x 45 (Version 7)", "49 x 49 (Version 8)", "53 x 53 (Version 9)", "57 x 57 (Version 10)", "61 x 61 (Version 11)", "65 x 65 (Version 12)",
            "69 x 69 (Version 13)", "73 x 73 (Version 14)", "77 x 77 (Version 15)", "81 x 81 (Version 16)", "85 x 85 (Version 17)", "89 x 89 (Version 18)",
            "93 x 93 (Version 19)", "97 x 97 (Version 20)", "101 x 101 (Version 21)", "105 x 105 (Version 22)", "109 x 109 (Version 23)", "113 x 113 (Version 24)",
            "117 x 117 (Version 25)", "121 x 121 (Version 26)", "125 x 125 (Version 27)", "129 x 129 (Version 28)", "133 x 133 (Version 29)", "137 x 137 (Version 30)",
            "141 x 141 (Version 31)", "145 x 145 (Version 32)", "149 x 149 (Version 33)", "153 x 153 (Version 34)", "157 x 151 (Version 35)", "161 x 161 (Version 36)",
            "165 x 165 (Version 37)", "169 x 169 (Version 38)", "173 x 173 (Version 39)", "177 x 177 (Version 40)" };

        private static string[] qrErrorLevels = {"Low (~20%)", "Medium (~37%)", "Quartile (~55%)", "High (~65%)" };

        private static string[] mqrSizes = { "11 x 11 (Version M1)", "13 x 13 (Version M2)", "15 x 15 (Version M3)", "17 x 17 (Version M4)" };

        private static string[] mqrErrorLevels = { "Low (~20%)", "Medium (~37%)", "Quartile (~55%)" };

        private static string[] aztecSizes = {
            "15 x 15 Compact", "19 x 19 Compact", "23 x 23 Compact", "27 x 27 Compact", "19 x 19", "23 x 23", "27 x 27", "31 x 31", "37 x 37", "41 x 41",
            "45 x 45", "49 x 49", "53 x 53", "57 x 57", "61 x 61", "67 x 67", "71 x 71", "75 x 75", "79 x 79", "81 x 81", "87 x 87", "91 x 91", "95 x 95",
            "101 x 101", "105 x 105", "109 x 109", "113 x 113", "117 x 117", "121 x 121", "125 x 125", "131 x 131", "135 x 135", "139 x 139", "143 x 143", 
            "147 x 147", "151 x 151"};

        private static string[] aztecErrorLevels = { "10% + 3 Words", "23% + 3 Words", "36% + 3 Words", "50% + 3 Words" };

        private static string[] expStackedSegements = {
            "Automatic", "2",  "4",  "6",  "8",  "10",  "12",  "14",  "16",  "18",  "20",  "22" };

        private static string[] pdfColumns = {
            "Automatic", "1",  "2",  "3",  "4",  "5",  "6",  "7",  "8",  "9",  "10", "11",  "12",  "13",  "14",  "15",  "16",  "17",  "18",  "19",  "20"};

        private static string[] pdfErrorCorrection = {
            "Automatic", "2 Words",  "4 Words",  "8 Words",  "16 Words",  "32 Words",  "64 Words",  "128 Words",  "256 Words",  "512 Words" };

        private static string[] pdfRowHeight = { "2", "3", "4", "5" };

        private ComboBox dmSizesComboBox = null;
        private CheckBox squareOnlyCheckBox = null;

        private RadioButton qrAztecAutoRadioButton = null;
        private RadioButton qrAztecSizeRadioButton = null;
        private RadioButton qrAztecErrorRadioButton = null;
        private ComboBox qrAztecSizesComboBox = null;
        private ComboBox qrAztecErrorComboBox = null;

        private CheckBox dmreCheckBox = null;
        private GroupBox modeGroupBox = null;
        private RadioButton standardRadioButton = null;
        private RadioButton gs1RadioButton = null;
        private RadioButton hibcRadioButton = null;

        private GroupBox compositeGroupBox = null;
        private RadioButton ccaRadioButton = null;
        private RadioButton ccbRadioButton = null;
        private RadioButton cccRadioButton = null;
        private TextBox compositeDataTextbox = null;

        private CheckBox optionalCheckDigitCheckBox = null;
        private CheckBox showCheckDigitCheckBox = null;

        private ComboBox expStackedSegementsComboBox = null;

        private TextBox supplementDataTextBox = null;

        // PDF controls.
        private ComboBox pdfColumnsComboBox = null;
        private ComboBox pdfErrorLevelComboBox = null;
        private ComboBox pdfRowHeightComboBox = null;

        private GroupBox bearerStyeGroupBox = null;
        private RadioButton noneRadioButton = null;
        private RadioButton horizonalRadioButton = null;
        private RadioButton rectangleRadioButton = null;

        /// <summary>
        /// Adds to Data Matrix specific controls to the symbol properties tab page.
        /// </summary>
        private void AddDataMatrixControls()
        {
            AddModeControls();
            var label1 = new Label();
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(10, 70);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(84, 13);
            label1.TabIndex = 0;
            label1.Text = "Data Matrix Size:";
            symbolPropertiesTabPage.Controls.Add(label1);

            dmSizesComboBox = new ComboBox();
            dmSizesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            dmSizesComboBox.FormattingEnabled = true;
            dmSizesComboBox.Location = new System.Drawing.Point(100, 65);
            dmSizesComboBox.Name = "dmSizesComboBox";
            dmSizesComboBox.Size = new System.Drawing.Size(80, 21);
            dmSizesComboBox.TabIndex = 0;
            dmSizesComboBox.MaxDropDownItems = 10;
            dmSizesComboBox.Items.AddRange(dmSizes);
            dmSizesComboBox.SelectedIndex = 0;
            symbolPropertiesTabPage.Controls.Add(dmSizesComboBox);
            dmSizesComboBox.SelectedIndexChanged += new System.EventHandler(this.dmSizesComboBox_SelectedIndexChanged);

            squareOnlyCheckBox = new CheckBox();
            squareOnlyCheckBox.AutoSize = true;
            squareOnlyCheckBox.Location = new System.Drawing.Point(10, 115);
            squareOnlyCheckBox.Name = "squareOnlyCheckBox";
            squareOnlyCheckBox.Size = new System.Drawing.Size(80, 17);
            squareOnlyCheckBox.TabIndex = 1;
            squareOnlyCheckBox.Text = "Suppress Rectangular Symbols in Auto Mode";
            squareOnlyCheckBox.UseVisualStyleBackColor = true;
            squareOnlyCheckBox.Checked = barcode.DataMatrixSquare;
            symbolPropertiesTabPage.Controls.Add(squareOnlyCheckBox);
            squareOnlyCheckBox.Click += new System.EventHandler(this.squareOnlyCheckBox_CheckedChanged);

            dmreCheckBox = new CheckBox();
            dmreCheckBox.AutoSize = true;
            dmreCheckBox.Location = new System.Drawing.Point(10, 135);
            dmreCheckBox.Name = "dmreCheckBox";
            dmreCheckBox.Size = new System.Drawing.Size(80, 17);
            dmreCheckBox.TabIndex = 2;
            dmreCheckBox.Text = "Allow DMRE in Auto Mode";
            dmreCheckBox.UseVisualStyleBackColor = true;
            dmreCheckBox.Checked = barcode.DataMatrixRectExtn;
            symbolPropertiesTabPage.Controls.Add(dmreCheckBox);
            dmreCheckBox.Click += new System.EventHandler(this.dmreCheckBox_CheckedChanged);
        }

        // Adds the runtime controls for QR and Aztec symbols.
        private void AddQRAztecControls()
        {
            if(symbolID == Symbology.QRCode || symbolID == Symbology.Aztec)
                AddModeControls();

            qrAztecAutoRadioButton = new RadioButton();
            qrAztecAutoRadioButton.AutoSize = true;
            qrAztecAutoRadioButton.Location = new System.Drawing.Point(5, 65);
            qrAztecAutoRadioButton.Name = "autoSizeRadioButton";
            qrAztecAutoRadioButton.Size = new System.Drawing.Size(68, 17);
            qrAztecAutoRadioButton.TabIndex = 0;
            qrAztecAutoRadioButton.TabStop = true;
            qrAztecAutoRadioButton.Text = "Automatic Sizing";
            qrAztecAutoRadioButton.UseVisualStyleBackColor = true;
            qrAztecAutoRadioButton.Checked = true;
            symbolPropertiesTabPage.Controls.Add(qrAztecAutoRadioButton);
            qrAztecAutoRadioButton.CheckedChanged += new System.EventHandler(qrAztecAutoRadioButton_CheckedChanged);

            qrAztecSizeRadioButton = new RadioButton();
            qrAztecSizeRadioButton.AutoSize = true;
            qrAztecSizeRadioButton.Location = new System.Drawing.Point(5, 95);
            qrAztecSizeRadioButton.Name = "sizeRadioButton";
            qrAztecSizeRadioButton.Size = new System.Drawing.Size(68, 17);
            qrAztecSizeRadioButton.TabIndex = 0;
            qrAztecSizeRadioButton.TabStop = true;
            qrAztecSizeRadioButton.Text = "Adjust Size To:";
            qrAztecSizeRadioButton.UseVisualStyleBackColor = true;
            qrAztecSizeRadioButton.Checked = false;
            symbolPropertiesTabPage.Controls.Add(qrAztecSizeRadioButton);
            qrAztecSizeRadioButton.CheckedChanged += new System.EventHandler(qrAztecSizeRadioButton_CheckedChanged);

            qrAztecErrorRadioButton = new RadioButton();
            qrAztecErrorRadioButton.AutoSize = true;
            qrAztecErrorRadioButton.Location = new System.Drawing.Point(5, 125);
            qrAztecErrorRadioButton.Name = "errorRadioButton";
            qrAztecErrorRadioButton.Size = new System.Drawing.Size(68, 17);
            qrAztecErrorRadioButton.TabIndex = 0;
            qrAztecErrorRadioButton.TabStop = true;
            qrAztecErrorRadioButton.Text = "Error Correction:";
            qrAztecErrorRadioButton.UseVisualStyleBackColor = true;
            qrAztecErrorRadioButton.Checked = false;
            symbolPropertiesTabPage.Controls.Add(qrAztecErrorRadioButton);
            qrAztecErrorRadioButton.CheckedChanged += new System.EventHandler(qrAztecErrorRadioButton_CheckedChanged);

            qrAztecSizesComboBox = new ComboBox();
            qrAztecSizesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            qrAztecSizesComboBox.FormattingEnabled = true;
            qrAztecSizesComboBox.Location = new System.Drawing.Point(120, 95);
            qrAztecSizesComboBox.Name = "sizesComboBox";
            qrAztecSizesComboBox.Size = new System.Drawing.Size(130, 21);
            qrAztecSizesComboBox.DropDownHeight = 198;
            qrAztecSizesComboBox.TabIndex = 0;
            qrAztecSizesComboBox.MaxDropDownItems = 10;
            if (symbolID == Symbology.QRCode)
                qrAztecSizesComboBox.Items.AddRange(qrSizes);

            else if (symbolID == Symbology.MicroQRCode)
                qrAztecSizesComboBox.Items.AddRange(mqrSizes);

            else
                qrAztecSizesComboBox.Items.AddRange(aztecSizes);

            qrAztecSizesComboBox.SelectedIndex = 0;
            qrAztecSizesComboBox.Enabled = false;
            symbolPropertiesTabPage.Controls.Add(qrAztecSizesComboBox);
            qrAztecSizesComboBox.SelectedIndexChanged += new System.EventHandler(this.qrAztecSizesComboBox_SelectedIndexChanged);

            qrAztecErrorComboBox = new ComboBox();
            qrAztecErrorComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            qrAztecErrorComboBox.FormattingEnabled = true;
            qrAztecErrorComboBox.Location = new System.Drawing.Point(120, 125);
            qrAztecErrorComboBox.Name = "errorComboBox";
            qrAztecErrorComboBox.Size = new System.Drawing.Size(130, 21);
            qrAztecErrorComboBox.DropDownHeight = 198;
            qrAztecErrorComboBox.TabIndex = 0;
            qrAztecErrorComboBox.MaxDropDownItems = 10;
            if (symbolID == Symbology.QRCode)
                qrAztecErrorComboBox.Items.AddRange(qrErrorLevels);

            else if (symbolID == Symbology.MicroQRCode)
                qrAztecErrorComboBox.Items.AddRange(mqrErrorLevels);

            else
                qrAztecErrorComboBox.Items.AddRange(aztecErrorLevels);

            qrAztecErrorComboBox.SelectedIndex = 0;
            qrAztecErrorComboBox.Enabled = false;
            symbolPropertiesTabPage.Controls.Add(qrAztecErrorComboBox);
            qrAztecErrorComboBox.SelectedIndexChanged += new System.EventHandler(this.qrAztecErrorComboBox_SelectedIndexChanged);
        }

        private void AddCode39Controls()
        {
            if (symbolID != Symbology.Code93)
            {
                optionalCheckDigitCheckBox = new CheckBox();
                optionalCheckDigitCheckBox.AutoSize = true;
                optionalCheckDigitCheckBox.Location = new System.Drawing.Point(10, 100);
                optionalCheckDigitCheckBox.Name = "useCheckDigitCheckBox";
                optionalCheckDigitCheckBox.Size = new System.Drawing.Size(103, 17);
                optionalCheckDigitCheckBox.TabIndex = 0;
                optionalCheckDigitCheckBox.Text = "Use Check Digit";
                optionalCheckDigitCheckBox.UseVisualStyleBackColor = true;
                optionalCheckDigitCheckBox.Checked = true;
                symbolPropertiesTabPage.Controls.Add(optionalCheckDigitCheckBox);
                optionalCheckDigitCheckBox.Click += new System.EventHandler(this.optionalCheckDigitCheckBox_CheckedChanged);
            }

            showCheckDigitCheckBox = new CheckBox();
            showCheckDigitCheckBox.AutoSize = true;
            showCheckDigitCheckBox.Location = new System.Drawing.Point(10, 130);
            showCheckDigitCheckBox.Name = "showCheckDigitCheckBox";
            showCheckDigitCheckBox.Size = new System.Drawing.Size(147, 17);
            showCheckDigitCheckBox.TabIndex = 1;
            showCheckDigitCheckBox.Text = "Show Check Digit In Text";
            showCheckDigitCheckBox.UseVisualStyleBackColor = true;
            showCheckDigitCheckBox.Checked = false;
            symbolPropertiesTabPage.Controls.Add(showCheckDigitCheckBox);
            showCheckDigitCheckBox.Click += new System.EventHandler(this.showCheckDigitCheckBox_CheckedChange);
        }

        /// <summary>
        /// Adds to mode selection controls to the symbol properties tab page.
        /// </summary>
        private void AddModeControls()
        {
            standardRadioButton = new RadioButton();
            standardRadioButton.AutoSize = true;
            standardRadioButton.Location = new System.Drawing.Point(5, 19);
            standardRadioButton.Name = "standardRadioButton";
            standardRadioButton.Size = new System.Drawing.Size(68, 17);
            standardRadioButton.TabIndex = 0;
            standardRadioButton.TabStop = true;
            standardRadioButton.Text = "Standard";
            standardRadioButton.UseVisualStyleBackColor = true;
            standardRadioButton.Checked = true;
            standardRadioButton.CheckedChanged += new System.EventHandler(this.standardRadioButton_CheckedChanged);
            encodingMode = EncodingMode.Standard;

            gs1RadioButton = new RadioButton();
            gs1RadioButton.AutoSize = true;
            gs1RadioButton.Location = new System.Drawing.Point(80, 19);
            gs1RadioButton.Name = "gs1RadioButton";
            gs1RadioButton.Size = new System.Drawing.Size(46, 17);
            gs1RadioButton.TabIndex = 1;
            gs1RadioButton.TabStop = true;
            gs1RadioButton.Text = "GS1";
            gs1RadioButton.UseVisualStyleBackColor = true;
            gs1RadioButton.CheckedChanged += new System.EventHandler(this.gs1RadioButton_CheckedChanged);

            hibcRadioButton = new RadioButton();
            hibcRadioButton.AutoSize = true;
            hibcRadioButton.Location = new System.Drawing.Point(150, 19);
            hibcRadioButton.Name = "hibcRadioButton";
            hibcRadioButton.Size = new System.Drawing.Size(50, 17);
            hibcRadioButton.TabIndex = 2;
            hibcRadioButton.TabStop = true;
            hibcRadioButton.Text = "HIBC";
            hibcRadioButton.UseVisualStyleBackColor = true;
            hibcRadioButton.CheckedChanged += new System.EventHandler(this.hibcRadioButton_CheckedChanged);

            modeGroupBox = new GroupBox();
            modeGroupBox.Controls.Add(hibcRadioButton);
            modeGroupBox.Controls.Add(gs1RadioButton);
            modeGroupBox.Controls.Add(standardRadioButton);
            modeGroupBox.Location = new System.Drawing.Point(10, 6);
            modeGroupBox.Name = "modeGroupBox";
            modeGroupBox.Size = new System.Drawing.Size(210, 45);
            modeGroupBox.TabIndex = 10;
            modeGroupBox.TabStop = false;
            modeGroupBox.Text = "Mode";
            symbolPropertiesTabPage.Controls.Add(modeGroupBox);
        }

        /// <summary>
        /// Adds the composite data controls to the symbol properties tab page.
        /// </summary>
        private void AddCompositeControls()
        {
            ccaRadioButton = new RadioButton();
            ccaRadioButton.AutoSize = true;
            ccaRadioButton.Location = new System.Drawing.Point(5, 19);
            ccaRadioButton.Name = "ccaRadioButton";
            ccaRadioButton.Size = new System.Drawing.Size(46, 17);
            ccaRadioButton.TabIndex = 0;
            ccaRadioButton.TabStop = true;
            ccaRadioButton.Text = "CCA";
            ccaRadioButton.UseVisualStyleBackColor = true;
            ccaRadioButton.Checked = true;
            ccaRadioButton.CheckedChanged += new System.EventHandler(this.ccaRadioButton_CheckedChanged);
            compositeMode = CompositeMode.CCA;

            ccbRadioButton = new RadioButton();
            ccbRadioButton.AutoSize = true;
            ccbRadioButton.Location = new System.Drawing.Point(80, 19);
            ccbRadioButton.Name = "ccbRadioButton";
            ccbRadioButton.Size = new System.Drawing.Size(46, 17);
            ccbRadioButton.TabIndex = 1;
            ccbRadioButton.TabStop = true;
            ccbRadioButton.Text = "CCB";
            ccbRadioButton.UseVisualStyleBackColor = true;
            ccbRadioButton.CheckedChanged += new System.EventHandler(this.ccbRadioButton_CheckedChanged);

            cccRadioButton = new RadioButton();
            cccRadioButton.AutoSize = true;
            cccRadioButton.Location = new System.Drawing.Point(150, 19);
            cccRadioButton.Name = "cccRadioButton";
            cccRadioButton.Size = new System.Drawing.Size(46, 17);
            cccRadioButton.TabIndex = 2;
            cccRadioButton.TabStop = true;
            cccRadioButton.Text = "CCC";
            cccRadioButton.UseVisualStyleBackColor = true;
            cccRadioButton.CheckedChanged += new System.EventHandler(this.cccRadioButton_CheckedChanged);

            compositeGroupBox = new GroupBox();
            compositeGroupBox.Controls.Add(ccaRadioButton);
            compositeGroupBox.Controls.Add(ccbRadioButton);
            compositeGroupBox.Controls.Add(cccRadioButton);
            compositeGroupBox.Location = new System.Drawing.Point(10, 55);
            compositeGroupBox.Name = "compositeGroupBox";
            compositeGroupBox.Size = new System.Drawing.Size(210, 45);
            compositeGroupBox.TabIndex = 3;
            compositeGroupBox.TabStop = false;
            compositeGroupBox.Text = "Composite Type:";
            symbolPropertiesTabPage.Controls.Add(compositeGroupBox);

            var label1 = new Label();
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(10, 110);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(85, 13);
            label1.TabIndex = 0;
            label1.Text = "Composite Data:";
            symbolPropertiesTabPage.Controls.Add(label1);

            compositeDataTextbox = new TextBox();
            compositeDataTextbox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            compositeDataTextbox.Location = new System.Drawing.Point(10, 126);
            compositeDataTextbox.Multiline = true;
            compositeDataTextbox.Name = "compositeDataTextbox";
            compositeDataTextbox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            compositeDataTextbox.Size = new System.Drawing.Size(240, 39);
            compositeDataTextbox.Text = compositeText;
            compositeDataTextbox.TabIndex = 1;
            symbolPropertiesTabPage.Controls.Add(compositeDataTextbox);

            if (symbolID == ZintNet.Symbology.Code128)
            {
                compositeGroupBox.Enabled = false;
                compositeDataTextbox.Enabled = false;
            }
        }

        private void AddPDFControls()
        {
            var label1 = new Label();
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(10, 70);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(84, 13);
            label1.TabIndex = 0;
            label1.Text = "Number Of Data Columns:";
            symbolPropertiesTabPage.Controls.Add(label1);

            var label2 = new Label();
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(10, 95);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(84, 13);
            label2.TabIndex = 0;
            label2.Text = "Error Correction Capacity:";
            symbolPropertiesTabPage.Controls.Add(label2);

            var label3 = new Label();
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(10, 120);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(84, 13);
            label3.TabIndex = 0;
            label3.Text = "Row Height:";
            symbolPropertiesTabPage.Controls.Add(label3);

            pdfColumnsComboBox = new ComboBox();
            pdfColumnsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            pdfColumnsComboBox.FormattingEnabled = true;
            pdfColumnsComboBox.Location = new System.Drawing.Point(150, 65);
            pdfColumnsComboBox.Name = "dataColumnsComboBox";
            pdfColumnsComboBox.Size = new System.Drawing.Size(80, 21);
            pdfColumnsComboBox.TabIndex = 0;
            pdfColumnsComboBox.MaxDropDownItems = 10;
            pdfColumnsComboBox.Items.AddRange(pdfColumns);
            pdfColumnsComboBox.SelectedIndex = 0;
            symbolPropertiesTabPage.Controls.Add(pdfColumnsComboBox);
            pdfColumnsComboBox.SelectedIndexChanged += new System.EventHandler(this.pdfColumnsComboBox_SelectedIndexChanged);

            pdfErrorLevelComboBox = new ComboBox();
            pdfErrorLevelComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            pdfErrorLevelComboBox.FormattingEnabled = true;
            pdfErrorLevelComboBox.Location = new System.Drawing.Point(150, 90);
            pdfErrorLevelComboBox.Name = "errorCorrectionComboBox";
            pdfErrorLevelComboBox.Size = new System.Drawing.Size(80, 21);
            pdfErrorLevelComboBox.TabIndex = 0;
            pdfErrorLevelComboBox.MaxDropDownItems = 10;
            pdfErrorLevelComboBox.Items.AddRange(pdfErrorCorrection);
            pdfErrorLevelComboBox.SelectedIndex = 0;
            symbolPropertiesTabPage.Controls.Add(pdfErrorLevelComboBox);
            pdfErrorLevelComboBox.SelectedIndexChanged += new System.EventHandler(this.pdfErrorLevelComboBox_SelectedIndexChanged);

            pdfRowHeightComboBox = new ComboBox();
            pdfRowHeightComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            pdfRowHeightComboBox.FormattingEnabled = true;
            pdfRowHeightComboBox.Location = new System.Drawing.Point(150, 115);
            pdfRowHeightComboBox.Name = "pdfRowHeightComboBox";
            pdfRowHeightComboBox.Size = new System.Drawing.Size(80, 21);
            pdfRowHeightComboBox.TabIndex = 0;
            pdfRowHeightComboBox.MaxDropDownItems = 10;
            pdfRowHeightComboBox.Items.AddRange(pdfRowHeight);
            pdfRowHeightComboBox.SelectedIndex = 0;
            symbolPropertiesTabPage.Controls.Add(pdfRowHeightComboBox);
            pdfRowHeightComboBox.SelectedIndexChanged += new System.EventHandler(this.pdfRowHeightComboBox_SelectedIndexChanged);
        }

        private void AddExpStackedControls()
        {
            var label1 = new Label();
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(10, 20);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(85, 13);
            label1.TabIndex = 2;
            label1.Text = "Number Of Segments:";
            symbolPropertiesTabPage.Controls.Add(label1);

            expStackedSegementsComboBox = new ComboBox();
            expStackedSegementsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            expStackedSegementsComboBox.FormattingEnabled = true;
            expStackedSegementsComboBox.Location = new System.Drawing.Point(140, 18);
            expStackedSegementsComboBox.Name = "expStackedSegmentsComboBox";
            expStackedSegementsComboBox.Size = new System.Drawing.Size(80, 21);
            expStackedSegementsComboBox.TabIndex = 0;
            expStackedSegementsComboBox.MaxDropDownItems = 10;
            expStackedSegementsComboBox.Items.AddRange(expStackedSegements);
            expStackedSegementsComboBox.SelectedIndex = 0;
            symbolPropertiesTabPage.Controls.Add(expStackedSegementsComboBox);
            expStackedSegementsComboBox.SelectedIndexChanged += new System.EventHandler(this.expStackedSegementsComboBox_SelectedIndexChanged);
        }

        private void AddSupplimentDataControls()
        {
            var label1 = new Label();
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(10, 10);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(114, 13);
            label1.TabIndex = 0;
            label1.Text = "EAN/UPC Suppliment:";
            symbolPropertiesTabPage.Controls.Add(label1);

            supplementDataTextBox = new TextBox();
            supplementDataTextBox.Location = new System.Drawing.Point(10, 28);
            supplementDataTextBox.Name = "supplimentDataTextbox";
            supplementDataTextBox.Size = new System.Drawing.Size(242, 20);
            supplementDataTextBox.Text = supplementText;
            supplementDataTextBox.TabIndex = 1;
            symbolPropertiesTabPage.Controls.Add(this.supplementDataTextBox);
        }

        private void AddITF14Controls()
        {
            noneRadioButton = new RadioButton();
            noneRadioButton.AutoSize = true;
            noneRadioButton.Location = new System.Drawing.Point(5, 19);
            noneRadioButton.Name = "noneRadioButton";
            noneRadioButton.Size = new System.Drawing.Size(68, 17);
            noneRadioButton.TabIndex = 0;
            noneRadioButton.TabStop = true;
            noneRadioButton.Text = "None";
            noneRadioButton.UseVisualStyleBackColor = true;
            noneRadioButton.CheckedChanged += new System.EventHandler(this.noneRadioButton_CheckedChanged);
            encodingMode = EncodingMode.Standard;

            horizonalRadioButton = new RadioButton();
            horizonalRadioButton.AutoSize = true;
            horizonalRadioButton.Location = new System.Drawing.Point(5, 39);
            horizonalRadioButton.Name = "horizonalRadioButton";
            horizonalRadioButton.Size = new System.Drawing.Size(46, 17);
            horizonalRadioButton.TabIndex = 1;
            horizonalRadioButton.TabStop = true;
            horizonalRadioButton.Text = "Horizontal Bars";
            horizonalRadioButton.UseVisualStyleBackColor = true;
            horizonalRadioButton.CheckedChanged += new System.EventHandler(this.horizonalRadioButton_CheckedChanged);

            rectangleRadioButton = new RadioButton();
            rectangleRadioButton.AutoSize = true;
            rectangleRadioButton.Location = new System.Drawing.Point(5, 59);
            rectangleRadioButton.Name = "rectangleRadioButton";
            rectangleRadioButton.Size = new System.Drawing.Size(50, 17);
            rectangleRadioButton.TabIndex = 2;
            rectangleRadioButton.TabStop = true;
            rectangleRadioButton.Text = "Rectangle";
            rectangleRadioButton.UseVisualStyleBackColor = true;
            rectangleRadioButton.Checked = true;
            rectangleRadioButton.CheckedChanged += new System.EventHandler(this.rectangleRadioButton_CheckedChanged);
            itfBearerStyle = ITF14BearerStyle.Rectangle;

            bearerStyeGroupBox = new GroupBox();
            bearerStyeGroupBox.Controls.Add(noneRadioButton);
            bearerStyeGroupBox.Controls.Add(rectangleRadioButton);
            bearerStyeGroupBox.Controls.Add(horizonalRadioButton);
            bearerStyeGroupBox.Location = new System.Drawing.Point(10, 6);
            bearerStyeGroupBox.Name = "bearerStyeGroupBox";
            bearerStyeGroupBox.Size = new System.Drawing.Size(125, 85);
            bearerStyeGroupBox.TabIndex = 10;
            bearerStyeGroupBox.TabStop = false;
            bearerStyeGroupBox.Text = "Bearer Style";
            symbolPropertiesTabPage.Controls.Add(bearerStyeGroupBox);
        }

        /// <summary>
        /// Clears all runtime controls from the symbol properties tab page.
        /// </summary>
        private void RemoveRunTimeControls()
        {
            int numberOfControls = symbolPropertiesTabPage.Controls.Count;
            if (numberOfControls > 0)
            {
                // Remove any event handlers.
                for (int i = numberOfControls - 1; i >= 0; i--)
                {
                    if (symbolPropertiesTabPage.Controls[i].Name == "dmSizesComboBox")
                        dmSizesComboBox.SelectedIndexChanged -= new System.EventHandler(dmSizesComboBox_SelectedIndexChanged);

                    if (symbolPropertiesTabPage.Controls[i].Name == "squareOnlyCheckBox")
                        squareOnlyCheckBox.Click -= new System.EventHandler(squareOnlyCheckBox_CheckedChanged);

                    if (symbolPropertiesTabPage.Controls[i].Name == "dmreCheckBox")
                        dmreCheckBox.Click -= new System.EventHandler(dmreCheckBox_CheckedChanged);

                    if (symbolPropertiesTabPage.Controls[i].Name == "showCheckDigitCheckBox")
                        showCheckDigitCheckBox.Click -= new System.EventHandler(showCheckDigitCheckBox_CheckedChange);

                    if (symbolPropertiesTabPage.Controls[i].Name == "useCheckDigitCheckBox")
                        optionalCheckDigitCheckBox.Click -= new System.EventHandler(optionalCheckDigitCheckBox_CheckedChanged);

                    if (symbolPropertiesTabPage.Controls[i].Name == "modeGroupBox")
                    {
                        hibcRadioButton.CheckedChanged -= new System.EventHandler(hibcRadioButton_CheckedChanged);
                        gs1RadioButton.CheckedChanged -= new System.EventHandler(gs1RadioButton_CheckedChanged);
                        standardRadioButton.CheckedChanged -= new System.EventHandler(standardRadioButton_CheckedChanged);
                    }

                    if (symbolPropertiesTabPage.Controls[i].Name == "compositeGroupBox")
                    {
                        ccaRadioButton.CheckedChanged -= new System.EventHandler(ccaRadioButton_CheckedChanged);
                        ccbRadioButton.CheckedChanged -= new System.EventHandler(ccbRadioButton_CheckedChanged);
                        cccRadioButton.CheckedChanged -= new System.EventHandler(cccRadioButton_CheckedChanged);
                    }

                    if (symbolPropertiesTabPage.Controls[i].Name == "bearerStyleGroupBox")
                    {
                        noneRadioButton.CheckedChanged -= new System.EventHandler(noneRadioButton_CheckedChanged);
                        horizonalRadioButton.CheckedChanged -= new System.EventHandler(horizonalRadioButton_CheckedChanged);
                        rectangleRadioButton.CheckedChanged -= new System.EventHandler(rectangleRadioButton_CheckedChanged);
                    }

                    if (symbolPropertiesTabPage.Controls[i].Name == "dataColumnsComboBox")
                        pdfColumnsComboBox.SelectedIndexChanged -= new System.EventHandler(this.pdfColumnsComboBox_SelectedIndexChanged);

                    if (symbolPropertiesTabPage.Controls[i].Name == "errorCorrectionComboBox")
                        pdfErrorLevelComboBox.SelectedIndexChanged -= new System.EventHandler(this.pdfErrorLevelComboBox_SelectedIndexChanged);

                    if (symbolPropertiesTabPage.Controls[i].Name == "pdfRowHeightComboBox")
                        pdfRowHeightComboBox.SelectedIndexChanged -= new System.EventHandler(this.pdfRowHeightComboBox_SelectedIndexChanged);

                    if (symbolPropertiesTabPage.Controls[i].Name == "autoSizeRadioButton")
                        qrAztecAutoRadioButton.CheckedChanged -= new System.EventHandler(this.qrAztecAutoRadioButton_CheckedChanged);

                    if (symbolPropertiesTabPage.Controls[i].Name == "sizeRadioButton")
                        qrAztecAutoRadioButton.CheckedChanged -= new System.EventHandler(this.qrAztecSizeRadioButton_CheckedChanged);

                    if (symbolPropertiesTabPage.Controls[i].Name == "errorRadioButton")
                        qrAztecAutoRadioButton.CheckedChanged -= new System.EventHandler(this.qrAztecErrorRadioButton_CheckedChanged);

                    if (symbolPropertiesTabPage.Controls[i].Name == "sizesComboBox")
                        qrAztecSizesComboBox.SelectedIndexChanged -= new System.EventHandler(this.qrAztecSizesComboBox_SelectedIndexChanged);

                    if (symbolPropertiesTabPage.Controls[i].Name == "errorComboBox")
                        qrAztecErrorComboBox.SelectedIndexChanged += new System.EventHandler(this.qrAztecErrorComboBox_SelectedIndexChanged);

                    if (symbolPropertiesTabPage.Controls[i].Name == "expStackedSegmentsComboBox")
                        expStackedSegementsComboBox.SelectedIndexChanged -= new System.EventHandler(this.expStackedSegementsComboBox_SelectedIndexChanged);

                    symbolPropertiesTabPage.Controls[i].Dispose();
                }

                symbolPropertiesTabPage.Controls.Clear();
            }
        }
    }
}

