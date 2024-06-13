using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ZintNet;

namespace ZintNetLibTest
{
    partial class Form1
    {
        private static string[] maxicodeModes = {
            "Mode 2, Structured Carrier Message", "Mode 3, Structured Carrier Message", "Mode 4, Standard Symbol", "Mode 5, Full ECC Symbol", "Mode 6, Reader Program" };

        private static string[] dmSizes = {
            "Automatic", "10 x 10", "12 x 12", "14 x 14", "16 x 16", "18 x 18", "20 x 20", "22 x 22", "24 x 24", "26 x 26", "32 x 32", "36 x 36", 
            "40 x 40", "44 x 44", "48 x 48", "52 x 52", "64 x 64", "72 x 72", "80 x 80", "88 x 88", "96 x 96", "104 x 104", "120 x 120", "132 x 132",
            "144 x 144", "8 x 18", "8 x 32", "12 x 26", "12 x 36", "16 x 36", "16 x 48", "8 x 48", "8 x 64", "12 x 64", "16 x 64",
            "24 x 48", "24 x 64", "26 x 40", "26 x 48","26 x 64" };

        private static string[] qrSizes = {
            "21 x 21 (Version 1)", "25 x 25 (Version 2)", "29 x 29 (Version 3)", "33 x 33 (Version 4)", "37 x 37 (Version 5)", "41 x 41 (Version 6)",
            "45 x 45 (Version 7)", "49 x 49 (Version 8)", "53 x 53 (Version 9)", "57 x 57 (Version 10)", "61 x 61 (Version 11)", "65 x 65 (Version 12)",
            "69 x 69 (Version 13)", "73 x 73 (Version 14)", "77 x 77 (Version 15)", "81 x 81 (Version 16)", "85 x 85 (Version 17)", "89 x 89 (Version 18)",
            "93 x 93 (Version 19)", "97 x 97 (Version 20)", "101 x 101 (Version 21)", "105 x 105 (Version 22)", "109 x 109 (Version 23)", "113 x 113 (Version 24)",
            "117 x 117 (Version 25)", "121 x 121 (Version 26)", "125 x 125 (Version 27)", "129 x 129 (Version 28)", "133 x 133 (Version 29)", "137 x 137 (Version 30)",
            "141 x 141 (Version 31)", "145 x 145 (Version 32)", "149 x 149 (Version 33)", "153 x 153 (Version 34)", "157 x 151 (Version 35)", "161 x 161 (Version 36)",
            "165 x 165 (Version 37)", "169 x 169 (Version 38)", "173 x 173 (Version 39)", "177 x 177 (Version 40)" };

        private static string[] qrErrorLevels = { "Low (~20%)", "Medium (~37%)", "Quartile (~55%)", "High (~65%)" };

        private static string[] mqrSizes = { "11 x 11 (Version M1)", "13 x 13 (Version M2)", "15 x 15 (Version M3)", "17 x 17 (Version M4)" };

        private static string[] mqrErrorLevels = { "Low (~20%)", "Medium (~37%)", "Quartile (~55%)" };

        private static string[] rMqrSizes = { "R7 x 43", "R7 x 59", "R7 x 77", "R7 x 99", "R9 x 139", "R9 x 43", "R9 x 59", "R9 x 77", "R9 x 99", "R9 x 139",
                                              "R11 x 27", "R11 x 43", "R11 x 59", "R11 x 77", "R11 x 99", "R11 x 139", "R13 x 27", "R13 x 43", "R13 x 59", "R13 x 77",
                                              "R13 x 99", "R13 x 139", "R15 x 43", "R15 x 59", "R15 x 77", "R15 x 99", "R17 x 139", "R17 x 43", "R17 x 59", "R17 x 77",
                                              "R17 x 99", "R17 x139", "R7 x Auto Width", "R9 x Auto Width", "R11 x Auto Width", "R13 x Auto Width", "R15 x Auto Width",
                                              "R17 x Auto Width" };

        private static string[] rMqrErrorLevels = { "Medium (~37%)", "High (~65%)" };

        private static string[] hanXinSizes = {
            "23 x 23 (Version 1)", "25 x 25 (Version 2)", "27 x 27 (Version 3)", "29 x 29 (Version 4)", "31 x 31 (Version 5)", "33 x 33 (Version 6)",
            "35 x 35 (Version 7)", "37 x 37 (Version 8)", "39 x 39 (Version 9)", "41 x 41 (Version 10)", "43 x 43 (Version 11)", "45 x 45 (Version 12)",
            "47 x 47 (Version 13)", "49 x 49 (Version 14)", "51 x 51 (Version 15)", "53 x 53 (Version 16)", "55 x 55 (Version 17)", "57 x 57 (Version 18)",
            "59 x 59 (Version 19)", "61 x 61 (Version 20)", "63 x 63 (Version 21)", "65 x 65 (Version 22)", "67 x 67 (Version 23)", "69 x 69 (Version 24)",
            "71 x 71 (Version 25)", "73 x 73 (Version 26)", "75 x 75 (Version 27)", "77 x 77 (Version 28)", "79 x 79 (Version 29)", "81 x 81 (Version 30)",
            "83 x 83 (Version 31)", "85 x 85 (Version 32)", "87 x 87 (Version 33)", "89 x 89 (Version 34)", "91 x 91 (Version 35)", "93 x 93 (Version 36)",
            "95 x 95 (Version 37)", "97 x 97 (Version 38)", "99 x 99 (Version 39)", "101 x 101 (Version 40)", "103 x 103 (Version 41)",
            "105 x 105 (Version 42)", "107 x 107 (Version 43)", "109 x 109 (Version 44)", "111 x 111 (Version 45)", "113 x 113 (Version 46)",
            "115 x 115 (Version 47)", "117 x 117 (Version 48)", "119 x 119 (Version 49)", "121 x 121 (Version 50)", "123 x 123 (Version 51)",
            "125 x 125 (Version 52)", "127 x 127 (Version 53)", "129 x 129 (Version 54)", "131 x 131 (Version 55)", "133 x 133 (Version 56)",
            "135 x 135 (Version 57)", "137 x 137 (Version 58)", "139 x 139 (Version 59)", "141 x 141 (Version 60)", "143 x 143 (Version 61)", 
            "145 x 145 (Version 62)", "147 x 147 (Version 63)", "149 x 149 (Version 64)", "151 x 151 (Version 65)", "153 x 153 (Version 66)",
            "155 x 155 (Version 67)", "157 x 157 (Version 68)", "159 x 159 (Version 69)", "161 x 161 (Version 70)", "163 x 163 (Version 71)",
            "165 x 165 (Version 72)", "167 x 167 (Version 73)", "169 x 169 (Version 74)", "171 x 171 (Version 75)", "173 x 173 (Version 76)",
            "175 x 175 (Version 77)", "177 x 177 (Version 78)", "179 x 179 (Version 79)", "181 x 181 (Version 80)", "183 x 183 (Version 81)",
            "185 x 185 (Version 82)", "187 x 187 (Version 83)", "189 x 189 (Version 84)" };

        private static string[] hanXinErrorLevels = { "Level 1 (~8%)", "Level 2 (~15%)", "Level 3 (~23%)", "Level 4 (~30%)" };

        private static string[] aztecSizes = {
            "15 x 15 Compact", "19 x 19 Compact", "23 x 23 Compact", "27 x 27 Compact", "19 x 19", "23 x 23", "27 x 27", "31 x 31", "37 x 37", "41 x 41",
            "45 x 45", "49 x 49", "53 x 53", "57 x 57", "61 x 61", "67 x 67", "71 x 71", "75 x 75", "79 x 79", "81 x 81", "87 x 87", "91 x 91", "95 x 95",
            "101 x 101", "105 x 105", "109 x 109", "113 x 113", "117 x 117", "121 x 121", "125 x 125", "131 x 131", "135 x 135", "139 x 139", "143 x 143", 
            "147 x 147", "151 x 151"};

        private static string[] aztecErrorLevels = { "10% + 3 Words", "23% + 3 Words", "36% + 3 Words", "50% + 3 Words" };

        private static string[] gridMatrixSizes = {
            "18 x 18", "30 x 30", "42 x 42", "54 x 54", "66 x 66", "78 x 78", "90 x 90", "102 x 102", "114 x 114", "126 x 126", "138 x 138", "150 x 150", "162 x 162" };

        private static string[] gridMatrixErrorLevels = { "~10%", "~20%", "~30%", "~40%", "~50%" };
            
        private static string[] codeOneSizes = {
            "Automatic", "16 x 18 (Version A)", "22 x 22 (Version B)", "28 x 32 (Version C)", "40 x 42 (Version D)", "52 x 54 (Version E)",
             "70 x 76 (Version F)", "104 x 98 (Version G)",  "148 x 134 (Version H)", "8 x Height (Version S)", "16 x Height (Version T)"};

        private static string[] expandedStackedSegements = {
            "Automatic", "2",  "4",  "6",  "8",  "10",  "12",  "14",  "16",  "18",  "20",  "22" };

        private static string[] pdfColumns = {
            "Automatic", "1",  "2",  "3",  "4",  "5",  "6",  "7",  "8",  "9",  "10", "11",  "12",  "13",  "14",  "15",  "16",  "17",  "18",  "19",  "20"};

        private static string[] mPdfColumns = { "Automatic", "1", "2", "3", "4" };

        private static string[] pdfErrorCorrection = {
            "Automatic", "2 Words",  "4 Words",  "8 Words",  "16 Words",  "32 Words",  "64 Words",  "128 Words",  "256 Words",  "512 Words" };

        private static string[] pdfRowHeight = { "2", "3", "4", "5" };

        private TabPage symbolPropertiesTabPage = null;

        // Maxicode controls.
        private ComboBox maxicodeModeComboBox = null;

        // QR, Micro QR, Aztec & Han Xin 2D controls.
        private RadioButton autoSize2DRadioButton = null;
        private RadioButton sizes2DRadioButton = null;
        private RadioButton errorCorrection2DRadioButton = null;
        private ComboBox sizes2DComboBox = null;
        private ComboBox errorCorrection2DComboBox = null;

        // Datamatrix controls.
        private ComboBox dmSizesComboBox = null;
        private CheckBox squareOnlyCheckBox = null;
        private CheckBox dmreCheckBox = null;

        private GroupBox modeGroupBox = null;
        private RadioButton standardRadioButton = null;
        private RadioButton gs1RadioButton = null;
        private RadioButton hibcRadioButton = null;

        // Composite data controls.
        private GroupBox compositeGroupBox = null;
        private RadioButton ccaRadioButton = null;
        private RadioButton ccbRadioButton = null;
        private RadioButton cccRadioButton = null;
        private TextBox compositeDataTextbox = null;

        private CheckBox optionalCheckDigitCheckBox = null;
        private CheckBox showCheckDigitCheckBox = null;

        private TextBox supplementDataTextBox = null;

        // PDF controls.
        private ComboBox columnsComboBox = null;
        private ComboBox errorLevelComboBox = null;
        private ComboBox pdfRowHeightComboBox = null;

        // ITF14 controls.
        private GroupBox bearerStyeGroupBox = null;
        private RadioButton noneRadioButton = null;
        private RadioButton horizonalRadioButton = null;
        private RadioButton rectangleRadioButton = null;

        /// <summary>
        /// Adds a second tab page if required.
        /// </summary>
        private void AddTabPage()
        {
            symbolPropertiesTabPage = new System.Windows.Forms.TabPage();
            symbolPropertiesTabPage.Text = symbologyComboBox.Text;
            symbolPropertiesTabPage.UseVisualStyleBackColor = true;
            this.tabControl1.Controls.Add(symbolPropertiesTabPage);
        }

        private void AddMaxiCodeControls(int index)
        {
            var label1 = new Label();
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(10, 10);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(84, 13);
            label1.TabIndex = 0;
            label1.Text = "Encoding Mode:";
            symbolPropertiesTabPage.Controls.Add(label1);

            maxicodeModeComboBox = new ComboBox();
            maxicodeModeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            maxicodeModeComboBox.FormattingEnabled = true;
            maxicodeModeComboBox.Location = new System.Drawing.Point(10, 30);
            maxicodeModeComboBox.Name = "maxicodeModeComboBox";
            maxicodeModeComboBox.Size = new System.Drawing.Size(200, 21);
            maxicodeModeComboBox.TabIndex = 0;
            maxicodeModeComboBox.MaxDropDownItems = 10;
            maxicodeModeComboBox.Items.AddRange(maxicodeModes);
            maxicodeModeComboBox.SelectedIndex = index;
            symbolPropertiesTabPage.Controls.Add(maxicodeModeComboBox);
            maxicodeModeComboBox.SelectedIndexChanged += new System.EventHandler(this.maxicodeModeComboBox_SelectedIndexChanged);
        }
        /// <summary>
        /// Adds Data Matrix specific controls to the symbol properties tab page.
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
            squareOnlyCheckBox.Checked = myBarcode.DataMatrixSquare;
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
            dmreCheckBox.Checked = myBarcode.DataMatrixRectExtn;
            symbolPropertiesTabPage.Controls.Add(dmreCheckBox);
            dmreCheckBox.Click += new System.EventHandler(this.dmreCheckBox_CheckedChanged);
        }

        /// <summary>
        /// Adds common 2D controls to the symbol properties tab page.
        /// </summary>
        private void Add2DControls()
        {
            int startY = 10;

            if (symbolID == Symbology.QRCode || symbolID == Symbology.Aztec || symbolID == Symbology.RectangularMicroQRCode)
            {
                AddModeControls();
                startY = 65;
            }

            autoSize2DRadioButton = new RadioButton();
            autoSize2DRadioButton.AutoSize = true;
            autoSize2DRadioButton.Location = new System.Drawing.Point(10, startY);
            autoSize2DRadioButton.Name = "autoSize2DRadioButton";
            autoSize2DRadioButton.Size = new System.Drawing.Size(68, 17);
            autoSize2DRadioButton.TabIndex = 0;
            autoSize2DRadioButton.TabStop = true;
            autoSize2DRadioButton.Text = "Automatic Sizing";
            autoSize2DRadioButton.UseVisualStyleBackColor = true;
            autoSize2DRadioButton.Checked = true;
            symbolPropertiesTabPage.Controls.Add(autoSize2DRadioButton);
            autoSize2DRadioButton.CheckedChanged += new System.EventHandler(Auto2DRadioButton_CheckedChanged);

            sizes2DRadioButton = new RadioButton();
            sizes2DRadioButton.AutoSize = true;
            sizes2DRadioButton.Location = new System.Drawing.Point(10, startY + 30);
            sizes2DRadioButton.Name = "sizes2DRadioButton";
            sizes2DRadioButton.Size = new System.Drawing.Size(68, 17);
            sizes2DRadioButton.TabIndex = 0;
            sizes2DRadioButton.TabStop = true;
            sizes2DRadioButton.Text = "Adjust Size To:";
            sizes2DRadioButton.UseVisualStyleBackColor = true;
            sizes2DRadioButton.Checked = false;
            symbolPropertiesTabPage.Controls.Add(sizes2DRadioButton);
            sizes2DRadioButton.CheckedChanged += new System.EventHandler(Sizes2DRadioButton_CheckedChanged);

            errorCorrection2DRadioButton = new RadioButton();
            errorCorrection2DRadioButton.AutoSize = true;
            errorCorrection2DRadioButton.Location = new System.Drawing.Point(10, startY + 60);
            errorCorrection2DRadioButton.Name = "errorCorrection2DRadioButton";
            errorCorrection2DRadioButton.Size = new System.Drawing.Size(68, 17);
            errorCorrection2DRadioButton.TabIndex = 0;
            errorCorrection2DRadioButton.TabStop = true;
            errorCorrection2DRadioButton.Text = "Error Correction:";
            errorCorrection2DRadioButton.UseVisualStyleBackColor = true;
            errorCorrection2DRadioButton.Checked = false;
            symbolPropertiesTabPage.Controls.Add(errorCorrection2DRadioButton);
            errorCorrection2DRadioButton.CheckedChanged += new System.EventHandler(Errorcorrection2DRadioButton_CheckedChanged);

            sizes2DComboBox = new ComboBox();
            sizes2DComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            sizes2DComboBox.FormattingEnabled = true;
            sizes2DComboBox.Location = new System.Drawing.Point(130, startY + 30);
            sizes2DComboBox.Name = "sizes2DComboBox";
            sizes2DComboBox.Size = new System.Drawing.Size(120, 21);
            sizes2DComboBox.DropDownHeight = 198;
            sizes2DComboBox.TabIndex = 0;
            sizes2DComboBox.MaxDropDownItems = 10;

            errorCorrection2DComboBox = new ComboBox();
            errorCorrection2DComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            errorCorrection2DComboBox.FormattingEnabled = true;
            errorCorrection2DComboBox.Location = new System.Drawing.Point(130, startY + 60);
            errorCorrection2DComboBox.Name = "errorCorrection2DComboBox";
            errorCorrection2DComboBox.Size = new System.Drawing.Size(120, 21);
            errorCorrection2DComboBox.DropDownHeight = 198;
            errorCorrection2DComboBox.TabIndex = 0;
            errorCorrection2DComboBox.MaxDropDownItems = 10;

            switch (symbolID)
            {
                case Symbology.QRCode:
                    sizes2DComboBox.Items.AddRange(qrSizes);
                    errorCorrection2DComboBox.Items.AddRange(qrErrorLevels);
                    break;

                case Symbology.MicroQRCode:
                    sizes2DComboBox.Items.AddRange(mqrSizes);
                    errorCorrection2DComboBox.Items.AddRange(mqrErrorLevels);
                    break;

                case Symbology.RectangularMicroQRCode:
                    sizes2DComboBox.Items.AddRange(rMqrSizes);
                    errorCorrection2DComboBox.Items.AddRange(rMqrErrorLevels);
                    break;

                case Symbology.Aztec:
                    sizes2DComboBox.Items.AddRange(aztecSizes);
                    errorCorrection2DComboBox.Items.AddRange(aztecErrorLevels);
                    break;

                case Symbology.HanXin:
                    sizes2DComboBox.Items.AddRange(hanXinSizes);
                    errorCorrection2DComboBox.Items.AddRange(hanXinErrorLevels);
                    break;

                case Symbology.GridMatrix:
                    sizes2DComboBox.Items.AddRange(gridMatrixSizes);
                    errorCorrection2DComboBox.Items.AddRange(gridMatrixErrorLevels);
                    break;
            }

            sizes2DComboBox.SelectedIndex = 0;
            errorCorrection2DComboBox.SelectedIndex = 0;
            sizes2DComboBox.Enabled = false;
            errorCorrection2DComboBox.Enabled = false;
            symbolPropertiesTabPage.Controls.Add(sizes2DComboBox);
            symbolPropertiesTabPage.Controls.Add(errorCorrection2DComboBox);
            sizes2DComboBox.SelectedIndexChanged += new System.EventHandler(this.Sizes2DComboBox_SelectedIndexChanged);
            errorCorrection2DComboBox.SelectedIndexChanged += new System.EventHandler(this.ErrorCorrection2DComboBox_SelectedIndexChanged);
        }

        private void AddCode39Controls()
        {
            int startY = -20;

            if (symbolID == Symbology.Code39)
            {
                AddModeControls();
                gs1RadioButton.Enabled = false;
            }

            if (symbolID != Symbology.Code93)
            {
                startY = 20;
                if (symbolID == Symbology.Code39)
                    startY = 70;

                optionalCheckDigitCheckBox = new CheckBox();
                optionalCheckDigitCheckBox.AutoSize = true;
                optionalCheckDigitCheckBox.Location = new System.Drawing.Point(10, startY);
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
            showCheckDigitCheckBox.Location = new System.Drawing.Point(10, startY + 30);
            showCheckDigitCheckBox.Name = "showCheckDigitCheckBox";
            showCheckDigitCheckBox.Size = new System.Drawing.Size(147, 17);
            showCheckDigitCheckBox.TabIndex = 1;
            showCheckDigitCheckBox.Text = "Show Check Digit(s) In Text";
            showCheckDigitCheckBox.UseVisualStyleBackColor = true;
            showCheckDigitCheckBox.Checked = false;
            symbolPropertiesTabPage.Controls.Add(showCheckDigitCheckBox);
            showCheckDigitCheckBox.Click += new System.EventHandler(this.showCheckDigitCheckBox_CheckedChange);
        }

        /// <summary>
        /// Adds mode selection controls to the symbol properties tab page.
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
            modeGroupBox.Location = new System.Drawing.Point(10, 10);
            modeGroupBox.Name = "modeGroupBox";
            modeGroupBox.Size = new System.Drawing.Size(200, 45);
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
            int startY = 10;
            if (symbolID == Symbology.DatabarExpandedStacked)
            {
                AddExpStackedControls();
                startY = 50;
            }

            if (symbolID == Symbology.EAN13 || symbolID == Symbology.EAN8 || symbolID == Symbology.UPCA || symbolID == Symbology.UPCE)
            {
                AddSupplimentDataControls();
                startY = 60;
            }

            if (symbolID == Symbology.Code128)
                startY = 60;

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
            compositeGroupBox.Location = new System.Drawing.Point(10, startY);
            compositeGroupBox.Name = "compositeGroupBox";
            compositeGroupBox.Size = new System.Drawing.Size(200, 45);
            compositeGroupBox.TabIndex = 3;
            compositeGroupBox.TabStop = false;
            compositeGroupBox.Text = "Composite Type:";
            symbolPropertiesTabPage.Controls.Add(compositeGroupBox);

            var label1 = new Label();
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(10, startY + 50);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(85, 13);
            label1.TabIndex = 0;
            label1.Text = "Composite Data:";
            symbolPropertiesTabPage.Controls.Add(label1);

            compositeDataTextbox = new TextBox();
            compositeDataTextbox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            compositeDataTextbox.Location = new System.Drawing.Point(10, startY + 70);
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

        private void AddDotCodeControls()
        {
            AddModeControls();

            var label1 = new Label();
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(10, 70);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(84, 13);
            label1.TabIndex = 0;
            label1.Text = "Number Of Data Columns:";
            symbolPropertiesTabPage.Controls.Add(label1);

            columnsComboBox = new ComboBox();
            columnsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            columnsComboBox.FormattingEnabled = true;
            columnsComboBox.Location = new System.Drawing.Point(150, 65);
            columnsComboBox.Name = "columnsComboBox";
            columnsComboBox.Size = new System.Drawing.Size(80, 21);
            columnsComboBox.TabIndex = 0;
            columnsComboBox.MaxDropDownItems = 10;
            columnsComboBox.Items.Add("Automatic");
            for (int x = 1; x < 51; x++)
                columnsComboBox.Items.Add(x.ToString());

            columnsComboBox.SelectedIndex = 0;
            symbolPropertiesTabPage.Controls.Add(columnsComboBox);
            columnsComboBox.SelectedIndexChanged += new System.EventHandler(this.ColumnsComboBox_SelectedIndexChanged);
        }

        private void AddUltracodeControls()
        {
            AddModeControls();

            var label1 = new Label();
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(10, 70);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(84, 13);
            label1.TabIndex = 0;
            label1.Text = "Error Correction:";
            symbolPropertiesTabPage.Controls.Add(label1);

            errorLevelComboBox = new ComboBox();
            errorLevelComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            errorLevelComboBox.FormattingEnabled = true;
            errorLevelComboBox.Location = new System.Drawing.Point(110, 65);
            errorLevelComboBox.Name = "errorCorrectionComboBox";
            errorLevelComboBox.Size = new System.Drawing.Size(120, 21);
            errorLevelComboBox.TabIndex = 0;
            errorLevelComboBox.MaxDropDownItems = 10;
            errorLevelComboBox.Items.Add("Automatic");
            for (int x = 1; x <= 6; x++)
                errorLevelComboBox.Items.Add("Level " + x.ToString());

            errorLevelComboBox.SelectedIndex = 0;
            symbolPropertiesTabPage.Controls.Add(errorLevelComboBox);
            errorLevelComboBox.SelectedIndexChanged += new System.EventHandler(this.ErrorCorrectionComboBox_SelectedIndexChanged);
        }

        private void AddCodeOneControls()
        {
            AddModeControls();

            var label1 = new Label();
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(10, 70);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(84, 13);
            label1.TabIndex = 0;
            label1.Text = "Symbol Size:";
            symbolPropertiesTabPage.Controls.Add(label1);

            columnsComboBox = new ComboBox();
            columnsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            columnsComboBox.FormattingEnabled = true;
            columnsComboBox.Location = new System.Drawing.Point(100, 65);
            columnsComboBox.Name = "sizeComboBox";
            columnsComboBox.Size = new System.Drawing.Size(140, 21);
            columnsComboBox.TabIndex = 0;
            columnsComboBox.MaxDropDownItems = 10;
            columnsComboBox.Items.AddRange(codeOneSizes);

            columnsComboBox.SelectedIndex = 0;
            symbolPropertiesTabPage.Controls.Add(columnsComboBox);
            columnsComboBox.SelectedIndexChanged += new System.EventHandler(this.ColumnsComboBox_SelectedIndexChanged);
        }

        private void AddChannelCodeControls()
        {
            var label1 = new Label();
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(10, 15);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(84, 13);
            label1.TabIndex = 0;
            label1.Text = "Number Of Channels:";
            symbolPropertiesTabPage.Controls.Add(label1);

            columnsComboBox = new ComboBox();
            columnsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            columnsComboBox.FormattingEnabled = true;
            columnsComboBox.Location = new System.Drawing.Point(150, 10);
            columnsComboBox.Name = "columnsComboBox";
            columnsComboBox.Size = new System.Drawing.Size(80, 21);
            columnsComboBox.TabIndex = 0;
            columnsComboBox.MaxDropDownItems = 10;
            columnsComboBox.Items.Add("Automatic");
            for (int x = 3; x < 9; x++)
                columnsComboBox.Items.Add(x.ToString());

            columnsComboBox.SelectedIndex = 0;
            symbolPropertiesTabPage.Controls.Add(columnsComboBox);
            columnsComboBox.SelectedIndexChanged += new System.EventHandler(this.ColumnsComboBox_SelectedIndexChanged);
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

            if (symbolID != Symbology.MicroPDF417)
            {
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
            }

            columnsComboBox = new ComboBox();
            columnsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            columnsComboBox.FormattingEnabled = true;
            columnsComboBox.Location = new System.Drawing.Point(150, 65);
            columnsComboBox.Name = "columnsComboBox";
            columnsComboBox.Size = new System.Drawing.Size(80, 21);
            columnsComboBox.TabIndex = 0;
            columnsComboBox.MaxDropDownItems = 10;
            if (symbolID != Symbology.MicroPDF417)
                columnsComboBox.Items.AddRange(pdfColumns);

            else
                columnsComboBox.Items.AddRange(mPdfColumns);

            columnsComboBox.SelectedIndex = 0;
            symbolPropertiesTabPage.Controls.Add(columnsComboBox);
            columnsComboBox.SelectedIndexChanged += new System.EventHandler(this.ColumnsComboBox_SelectedIndexChanged);

            if (symbolID == Symbology.PDF417)
            {
                errorLevelComboBox = new ComboBox();
                errorLevelComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
                errorLevelComboBox.FormattingEnabled = true;
                errorLevelComboBox.Location = new System.Drawing.Point(150, 90);
                errorLevelComboBox.Name = "errorCorrectionComboBox";
                errorLevelComboBox.Size = new System.Drawing.Size(80, 21);
                errorLevelComboBox.TabIndex = 0;
                errorLevelComboBox.MaxDropDownItems = 10;
                errorLevelComboBox.Items.AddRange(pdfErrorCorrection);
                errorLevelComboBox.SelectedIndex = 0;
                symbolPropertiesTabPage.Controls.Add(errorLevelComboBox);
                errorLevelComboBox.SelectedIndexChanged += new System.EventHandler(this.pdfErrorLevelComboBox_SelectedIndexChanged);

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
        }

        private void AddExpStackedControls()
        {
            int startY = 10;

            var label1 = new Label();
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(10, startY);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(85, 13);
            label1.TabIndex = 2;
            label1.Text = "Number Of Segments:";
            symbolPropertiesTabPage.Controls.Add(label1);

            columnsComboBox = new ComboBox();
            columnsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            columnsComboBox.FormattingEnabled = true;
            columnsComboBox.Location = new System.Drawing.Point(150, startY);
            columnsComboBox.Name = "columnsComboBox";
            columnsComboBox.Size = new System.Drawing.Size(80, 21);
            columnsComboBox.TabIndex = 0;
            columnsComboBox.MaxDropDownItems = 10;
            columnsComboBox.Items.AddRange(expandedStackedSegements);
            columnsComboBox.SelectedIndex = 0;
            symbolPropertiesTabPage.Controls.Add(columnsComboBox);
            columnsComboBox.SelectedIndexChanged += new System.EventHandler(this.ColumnsComboBox_SelectedIndexChanged);
        }

        /// <summary>
        /// Adds the EAN/UPC/ISBN suppliment controls.
        /// </summary>
        private void AddSupplimentDataControls()
        {
            int startY = 10;

            var label1 = new Label();
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(10, startY);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(114, 13);
            label1.TabIndex = 0;
            if (symbolID == Symbology.ISBN)
                label1.Text = "ISBN Suppliment";

            else
                label1.Text = "EAN/UPC Suppliment:";

            symbolPropertiesTabPage.Controls.Add(label1);
            supplementDataTextBox = new TextBox();
            supplementDataTextBox.Location = new System.Drawing.Point(10, startY + 20);
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
            itf14BearerStyle = ITF14BearerStyle.Rectangle;

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
            int numberOfControls;

            if (symbolPropertiesTabPage != null)
            {
                numberOfControls = symbolPropertiesTabPage.Controls.Count;
                if (numberOfControls > 0)
                {
                    // Before removing the controls it is necessary to remove any event handlers created.
                    for (int i = numberOfControls - 1; i >= 0; i--)
                    {
                        if (symbolPropertiesTabPage.Controls[i].Name == "dmSizesComboBox")
                            dmSizesComboBox.SelectedIndexChanged -= new System.EventHandler(dmSizesComboBox_SelectedIndexChanged);

                        if (symbolPropertiesTabPage.Controls[i].Name == "squareOnlyCheckBox")
                            squareOnlyCheckBox.Click -= new System.EventHandler(squareOnlyCheckBox_CheckedChanged);

                        if (symbolPropertiesTabPage.Controls[i].Name == "dmreCheckBox")
                            dmreCheckBox.Click -= new System.EventHandler(dmreCheckBox_CheckedChanged);

                        if (symbolPropertiesTabPage.Controls[i].Name == "maxicodeModeComboBox")
                            maxicodeModeComboBox.SelectedIndexChanged -= new System.EventHandler(maxicodeModeComboBox_SelectedIndexChanged);

                        if (symbolPropertiesTabPage.Controls[i].Name == "showCheckDigitCheckBox")
                            showCheckDigitCheckBox.Click -= new System.EventHandler(showCheckDigitCheckBox_CheckedChange);

                        if (symbolPropertiesTabPage.Controls[i].Name == "useCheckDigitCheckBox")
                            optionalCheckDigitCheckBox.Click -= new System.EventHandler(optionalCheckDigitCheckBox_CheckedChanged);

                        if (symbolPropertiesTabPage.Controls[i].Name == "modeGroupBox")
                        {
                            hibcRadioButton.CheckedChanged -= new System.EventHandler(hibcRadioButton_CheckedChanged);
                            hibcRadioButton.Dispose();
                            gs1RadioButton.CheckedChanged -= new System.EventHandler(gs1RadioButton_CheckedChanged);
                            gs1RadioButton.Dispose();
                            standardRadioButton.CheckedChanged -= new System.EventHandler(standardRadioButton_CheckedChanged);
                            standardRadioButton.Dispose();
                        }

                        if (symbolPropertiesTabPage.Controls[i].Name == "compositeGroupBox")
                        {
                            ccaRadioButton.CheckedChanged -= new System.EventHandler(ccaRadioButton_CheckedChanged);
                            ccaRadioButton.Dispose();
                            ccbRadioButton.CheckedChanged -= new System.EventHandler(ccbRadioButton_CheckedChanged);
                            ccbRadioButton.Dispose();
                            cccRadioButton.CheckedChanged -= new System.EventHandler(cccRadioButton_CheckedChanged);
                            cccRadioButton.Dispose();
                        }

                        if (symbolPropertiesTabPage.Controls[i].Name == "bearerStyleGroupBox")
                        {
                            noneRadioButton.CheckedChanged -= new System.EventHandler(noneRadioButton_CheckedChanged);
                            noneRadioButton.Dispose();
                            horizonalRadioButton.CheckedChanged -= new System.EventHandler(horizonalRadioButton_CheckedChanged);
                            horizonalRadioButton.Dispose();
                            rectangleRadioButton.CheckedChanged -= new System.EventHandler(rectangleRadioButton_CheckedChanged);
                            rectangleRadioButton.Dispose();
                        }

                        if (symbolPropertiesTabPage.Controls[i].Name == "columnsComboBox")
                            columnsComboBox.SelectedIndexChanged -= new System.EventHandler(this.ColumnsComboBox_SelectedIndexChanged);

                        if (symbolPropertiesTabPage.Controls[i].Name == "errorCorrectionComboBox")
                            errorLevelComboBox.SelectedIndexChanged -= new System.EventHandler(this.pdfErrorLevelComboBox_SelectedIndexChanged);

                        if (symbolPropertiesTabPage.Controls[i].Name == "pdfRowHeightComboBox")
                            pdfRowHeightComboBox.SelectedIndexChanged -= new System.EventHandler(this.pdfRowHeightComboBox_SelectedIndexChanged);

                        if (symbolPropertiesTabPage.Controls[i].Name == "autoSize2DRadioButton")
                            autoSize2DRadioButton.CheckedChanged -= new System.EventHandler(this.Auto2DRadioButton_CheckedChanged);

                        if (symbolPropertiesTabPage.Controls[i].Name == "sizes2DRadioButton")
                            autoSize2DRadioButton.CheckedChanged -= new System.EventHandler(this.Sizes2DRadioButton_CheckedChanged);

                        if (symbolPropertiesTabPage.Controls[i].Name == "errorCorrection2DRadioButton")
                            autoSize2DRadioButton.CheckedChanged -= new System.EventHandler(this.Errorcorrection2DRadioButton_CheckedChanged);

                        if (symbolPropertiesTabPage.Controls[i].Name == "sizes2DComboBox")
                            sizes2DComboBox.SelectedIndexChanged -= new System.EventHandler(this.Sizes2DComboBox_SelectedIndexChanged);

                        if (symbolPropertiesTabPage.Controls[i].Name == "errorCorrection2DComboBox")
                            errorCorrection2DComboBox.SelectedIndexChanged += new System.EventHandler(this.ErrorCorrection2DComboBox_SelectedIndexChanged);

                        symbolPropertiesTabPage.Controls[i].Dispose();
                    }

                    symbolPropertiesTabPage.Controls.Clear();
                }

                // Finally remove the tab page.
                this.tabControl1.Controls.Remove(symbolPropertiesTabPage);
                symbolPropertiesTabPage = null;
            }
        }
    }
}
