using System;
using System.Windows.Forms;

using ZintNet;

namespace ZintNetLibTest
{
    #region Tables.

    partial class Form1
    {
        private readonly string[] userMask4 = { "Automatic", "0", "1", "2", "3" };

        private readonly string[] userMask8 = { "Automatic", "0", "1", "2", "3", "4", "5", "6", "7" };

        private readonly string[] separatorHeight = { "1x", "2x", "3x", "4x" };

        private readonly string[] codablockFColumns = {
            "Automatic", "9 (4 data)", "10 (5 data)", "11 (6 data)", "12 (7 data)", "13 (8 data)", "14 (9 data)", "15 (10 data)", "16 (11 data)", "17 (12 data)",
            "18 (13 data)", "19 (14 data)", "20 (15 data)", "21 (16 data)", "22 (17 data)", "23 (18 data)", "24 (19 data)", "25 (20 data)", "26 (21 data)",
            "27 (22 data)", "28 (23 data)", "29 (24 data)", "30 (25 data)", "31 (26 data)", "32 (27 data)", "33 (28 data)", "34 (29 data)", "35 (30 data)",
            "36 (31 data)", "37 (32 data)", "38 (33 data)", "39 (34 data)", "40 (35 data)", "41 (36 data)", "42 (37 data)", "43 (38 data)", "44 (39 data)",
            "45 (40 data)", "46 (41 data)", "47 (42 data)", "48 (43 data)", "49 (44 data)", "50 (45 data)", "51 (46 data)", "52 (47 data)", "53 (48 data)",
            "54 (49 data)", "55 (50 data)", "56 (51 data)", "57 (52 data)", "58 (53 data)", "59 (54 data)", "60 (55 data)", "61 (56 data)", "62 (57 data)",
            "63 (58 data)", "64 (59 data)", "65 (60 data)", "66 (61 data)", "67 (62 data)" };

        private readonly string[] codablockFRows = {
            "Automatic", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24",
            "25", "26",  "27", "28", "29", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "40", "41", "42", "43", "44" };

        private readonly string[] c16KMinimumRows = { "Automatic", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16" };

        private readonly string[] c49MinimumRows = { "Automatic", "3", "4", "5", "6", "7", "8" };

        private readonly string[] maxicodeModes = {
            "Mode 2, Structured Carrier Message", "Mode 3, Structured Carrier Message", "Mode 4, Standard Symbol", "Mode 5, Full ECC Symbol", "Mode 6, Reader Program" };

        private readonly string[] dmSizes = {
            "Automatic", "10 x 10", "12 x 12", "14 x 14", "16 x 16", "18 x 18", "20 x 20", "22 x 22", "24 x 24", "26 x 26", "32 x 32", "36 x 36",
            "40 x 40", "44 x 44", "48 x 48", "52 x 52", "64 x 64", "72 x 72", "80 x 80", "88 x 88", "96 x 96", "104 x 104", "120 x 120", "132 x 132",
            "144 x 144", "8 x 18", "8 x 32", "12 x 26", "12 x 36", "16 x 36", "16 x 48", "8 x 48 (DMRE)", "8 x 64 (DMRE)", "8 x 80 (DMRE)", "8 x 96 (DMRE)",
            "8 x 120 (DMRE)", "8 x 144 (DMRE)", "12 x 64 (DMRE)", "16 x 64 (DMRE)", "20 x 36 (DMRE)", "20 x 44 (DMRE)", "20 x 64 (DMRE)", "24 x 48 (DMRE)",
            "24 x 64 (DMRE)", "26 x 40 (DMRE)", "26 x 48 (DMRE)","26 x 64 (DMRE)" };

        private readonly string[] qrSizes = { "Automatic", 
            "21 x 21 (Version 1)", "25 x 25 (Version 2)", "29 x 29 (Version 3)", "33 x 33 (Version 4)", "37 x 37 (Version 5)", "41 x 41 (Version 6)",
            "45 x 45 (Version 7)", "49 x 49 (Version 8)", "53 x 53 (Version 9)", "57 x 57 (Version 10)", "61 x 61 (Version 11)", "65 x 65 (Version 12)",
            "69 x 69 (Version 13)", "73 x 73 (Version 14)", "77 x 77 (Version 15)", "81 x 81 (Version 16)", "85 x 85 (Version 17)", "89 x 89 (Version 18)",
            "93 x 93 (Version 19)", "97 x 97 (Version 20)", "101 x 101 (Version 21)", "105 x 105 (Version 22)", "109 x 109 (Version 23)", "113 x 113 (Version 24)",
            "117 x 117 (Version 25)", "121 x 121 (Version 26)", "125 x 125 (Version 27)", "129 x 129 (Version 28)", "133 x 133 (Version 29)", "137 x 137 (Version 30)",
            "141 x 141 (Version 31)", "145 x 145 (Version 32)", "149 x 149 (Version 33)", "153 x 153 (Version 34)", "157 x 151 (Version 35)", "161 x 161 (Version 36)",
            "165 x 165 (Version 37)", "169 x 169 (Version 38)", "173 x 173 (Version 39)", "177 x 177 (Version 40)" };

        private readonly string[] qrErrorLevels = { "Automatic", "Low (~20%)", "Medium (~37%)", "Quartile (~55%)", "High (~65%)" };

        private readonly string[] mqrSizes = { "Automatic", "11 x 11 (Version M1)", "13 x 13 (Version M2)", "15 x 15 (Version M3)", "17 x 17 (Version M4)" };

        private readonly string[] mqrErrorLevels = { "Automatic", "Low (~20%)", "Medium (~37%)", "Quartile (~55%)" };

        private readonly string[] rmqrSizes = { "Automatic", "R7 x 43", "R7 x 59", "R7 x 77", "R7 x 99", "R9 x 139", "R9 x 43", "R9 x 59", "R9 x 77", "R9 x 99", "R9 x 139",
                                              "R11 x 27", "R11 x 43", "R11 x 59", "R11 x 77", "R11 x 99", "R11 x 139", "R13 x 27", "R13 x 43", "R13 x 59", "R13 x 77",
                                              "R13 x 99", "R13 x 139", "R15 x 43", "R15 x 59", "R15 x 77", "R15 x 99", "R17 x 139", "R17 x 43", "R17 x 59", "R17 x 77",
                                              "R17 x 99", "R17 x139", "R7 x Auto Width", "R9 x Auto Width", "R11 x Auto Width", "R13 x Auto Width", "R15 x Auto Width",
                                              "R17 x Auto Width" };

        private readonly string[] rmqrErrorLevels = { "Automatic", "Medium (~37%)", "High (~65%)" };

        private readonly string[] hanXinSizes = {
            "Automatic", "23 x 23 (Version 1)", "25 x 25 (Version 2)", "27 x 27 (Version 3)", "29 x 29 (Version 4)", "31 x 31 (Version 5)", "33 x 33 (Version 6)",
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

        private readonly string[] hanXinErrorLevels = { "Automatic", "Level 1 (~8%)", "Level 2 (~15%)", "Level 3 (~23%)", "Level 4 (~30%)" };

        private readonly string[] aztecSizes = {
            "15 x 15 Compact", "19 x 19 Compact", "23 x 23 Compact", "27 x 27 Compact", "19 x 19", "23 x 23", "27 x 27", "31 x 31", "37 x 37", "41 x 41",
            "45 x 45", "49 x 49", "53 x 53", "57 x 57", "61 x 61", "67 x 67", "71 x 71", "75 x 75", "79 x 79", "81 x 81", "87 x 87", "91 x 91", "95 x 95",
            "101 x 101", "105 x 105", "109 x 109", "113 x 113", "117 x 117", "121 x 121", "125 x 125", "131 x 131", "135 x 135", "139 x 139", "143 x 143",
            "147 x 147", "151 x 151"};

        private readonly string[] aztecErrorLevels = { "10% + 3 Words", "23% + 3 Words", "36% + 3 Words", "50% + 3 Words" };

        private readonly string[] gridMatrixSizes = {
            "Automatic", "18 x 18 (Version 1)", "30 x 30 (Version 2)", "42 x 42 (Version 3)", "54 x 54 (Version 4)", "66 x 66 (Version 5)",
            "78 x 78 (Version 6)", "90 x 90 (Version 7)", "102 x 102 (Version 8)", "114 x 114 (Version 9)", "126 x 126 (Version 10)",
            "138 x 138 (Version 11)", "150 x 150 (Version 12)", "162 x 162 (Version 13)" };

        private readonly string[] gridMatrixErrorLevels = { "Automatic", "~10%", "~20%", "~30%", "~40%", "~50%" };

        private readonly string[] codeOneSizes = {
            "Automatic", "16 x 18 (Version A)", "22 x 22 (Version B)", "28 x 32 (Version C)", "40 x 42 (Version D)", "52 x 54 (Version E)",
             "70 x 76 (Version F)", "104 x 98 (Version G)",  "148 x 134 (Version H)", "8 x Height (Version S)", "16 x Height (Version T)"};

        private readonly string[] expandedStackedSegements = {
            "Automatic", "2",  "4",  "6",  "8",  "10",  "12",  "14",  "16",  "18",  "20",  "22" };

        private readonly string[] pdfColumns = {
            "Automatic", "1",  "2",  "3",  "4",  "5",  "6",  "7",  "8",  "9",  "10", "11",  "12",  "13",  "14",  "15",  "16",  "17",  "18",  "19",  "20"};

        private readonly string[] mPdfColumns = { "Automatic", "1", "2", "3", "4" };

        private readonly string[] pdfErrorCorrection = {
            "Automatic", "2 Words",  "4 Words",  "8 Words",  "16 Words",  "32 Words",  "64 Words",  "128 Words",  "256 Words",  "512 Words" };

        private readonly string[] pdfRowHeight = { "2", "3", "4", "5" };

        #endregion


        private TabPage symbolPropertiesTabPage = null;


        // QR, Micro QR, Aztec & Han Xin 2D controls.
        private RadioButton autoResizeRadioButton = null;
        private RadioButton sizesRadioButton = null;
        private RadioButton errorCorrectionRadioButton = null;


        // Datamatrix controls.
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

        private CheckBox codeSetCCheckBox = null;
        private CheckBox optionalCheckDigitCheckBox = null;
        private CheckBox showOptCheckDigitCheckBox = null;

        private TextBox supplementDataTextBox = null;

        // ComboBoxs.
        private ComboBox maxicodeModeComboBox = null;
        private ComboBox rowsComboBox = null;
        private ComboBox columnsComboBox = null;
        private ComboBox separatorHeightComboBox = null;
        private ComboBox errorLevelComboBox = null;
        private ComboBox pdfRowHeightComboBox = null;
        private ComboBox versionSizesComboBox = null;
        private ComboBox errorCorrectionComboBox = null;
        private ComboBox userMaskComboBox = null;

        // ITF14 controls.
        private GroupBox bearerStyeGroupBox = null;
        private RadioButton noneRadioButton = null;
        private RadioButton horizonalRadioButton = null;
        private RadioButton rectangleRadioButton = null;

        // Labels.
        private Label compositeDataLabel = null;
        private Label columnLabel = null;
        private Label rowLabel = null;
        private Label separatorHeightLabel = null;
        private Label sizeLabel = null;
        private Label errorCorrectionLabel = null;
        private Label userMaskLabel = null;

        /// <summary>
        /// Adds a second tab page if required.
        /// </summary>
        private void AddTabPage()
        {
            symbolPropertiesTabPage = new TabPage
            {
                Text = symbologyComboBox.Text,
                UseVisualStyleBackColor = true
            };

            tabControl1.Controls.Add(symbolPropertiesTabPage);
        }

        #region Aztec

        /// <summary>
        /// Adds the Aztec controls and event handlers to the tab page.
        /// </summary>
        private void AddAztecControls()
        {
            int startY;

            AddModeControls();
            startY = 65;
            autoResizeRadioButton = new RadioButton
            {
                AutoSize = true,
                Location = new System.Drawing.Point(8, startY),
                Name = "autoSizeRadioButton",
                Size = new System.Drawing.Size(68, 17),
                TabIndex = 0,
                TabStop = true,
                Text = "Automatic Resizing",
                UseVisualStyleBackColor = true,
                Checked = true
            };

            sizesRadioButton = new RadioButton
            {
                AutoSize = true,
                Location = new System.Drawing.Point(8, startY + 30),
                Name = "sizesRadioButton",
                Size = new System.Drawing.Size(68, 17),
                TabIndex = 0,
                TabStop = true,
                Text = "Adjust Size To:",
                UseVisualStyleBackColor = true,
                Checked = false
            };

            errorCorrectionRadioButton = new RadioButton
            {
                AutoSize = true,
                Location = new System.Drawing.Point(8, startY + 60),
                Name = "errorCorrectionRadioButton",
                Size = new System.Drawing.Size(68, 17),
                TabIndex = 0,
                TabStop = true,
                Text = "Add Error Correction:",
                UseVisualStyleBackColor = true,
                Checked = false
            };

            versionSizesComboBox = new ComboBox
            {
                Enabled = false,
                DropDownStyle = ComboBoxStyle.DropDownList,
                FormattingEnabled = true,
                Location = new System.Drawing.Point(132, startY + 30),
                Name = "sizesComboBox",
                Size = new System.Drawing.Size(120, 21),
                DropDownHeight = 198,
                TabIndex = 0,
                MaxDropDownItems = 10
            };

            errorCorrectionComboBox = new ComboBox
            {
                Enabled = false,
                DropDownStyle = ComboBoxStyle.DropDownList,
                FormattingEnabled = true,
                Location = new System.Drawing.Point(132, startY + 60),
                Name = "errorCorrectionComboBox",
                Size = new System.Drawing.Size(120, 21),
                DropDownHeight = 198,
                TabIndex = 0,
                MaxDropDownItems = 10
            };

            versionSizesComboBox.Items.AddRange(aztecSizes);
            errorCorrectionComboBox.Items.AddRange(aztecErrorLevels);
            versionSizesComboBox.SelectedIndex = 0;
            errorCorrectionComboBox.SelectedIndex = 0;
            symbolPropertiesTabPage.Controls.Add(autoResizeRadioButton);
            symbolPropertiesTabPage.Controls.Add(sizesRadioButton);
            symbolPropertiesTabPage.Controls.Add(errorCorrectionRadioButton);
            symbolPropertiesTabPage.Controls.Add(versionSizesComboBox);
            symbolPropertiesTabPage.Controls.Add(errorCorrectionComboBox);
            autoResizeRadioButton.CheckedChanged += new EventHandler(AutoResizeRadioButton_CheckedChanged);
            sizesRadioButton.CheckedChanged += new EventHandler(SizesRadioButton_CheckedChanged);
            errorCorrectionRadioButton.CheckedChanged += new EventHandler(ErrorCorrectionRadioButton_CheckedChanged);
            versionSizesComboBox.SelectedIndexChanged += new EventHandler(SymbolSizesComboBox_SelectedIndexChanged);
            errorCorrectionComboBox.SelectedIndexChanged += new EventHandler(ErrorCorrectionComboBox_SelectedIndexChanged);
        }

        #endregion

        /// <summary>
        /// Adds the Codablock F controls and event handlers to the tab page.
        /// </summary>
        private void AddCodablockControls()
        {
            int startY;

            AddModeControls();
            gs1RadioButton.Enabled = false;
            startY = 65;

            columnLabel = new Label
            {
                Location = new System.Drawing.Point(10, startY + 2),
                Name = "columnsLabel",
                AutoSize = true,
                Text = "Data Columns:"
            };

            rowLabel = new Label
            {
                Location = new System.Drawing.Point(10, startY + 30 + 2),
                Name = "rowLabel",
                AutoSize = true,
                Text = "Rows:"
            };

            separatorHeightLabel = new Label
            {
                Location = new System.Drawing.Point(10, startY + 60 + 2),
                Name = "separatorHeightLabel",
                AutoSize = true,
                Text = "Separator Height:"
            };

            columnsComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FormattingEnabled = true,
                Location = new System.Drawing.Point(130, startY),
                Name = "columunsComboBox",
                Size = new System.Drawing.Size(120, 21),
                DropDownHeight = 198,
                TabIndex = 0,
                MaxDropDownItems = 10,
            };

            rowsComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FormattingEnabled = true,
                Location = new System.Drawing.Point(130, startY + 30),
                Name = "rowsComboBox",
                Size = new System.Drawing.Size(120, 21),
                DropDownHeight = 198,
                TabIndex = 0,
                MaxDropDownItems = 10
            };

            separatorHeightComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FormattingEnabled = true,
                Location = new System.Drawing.Point(130, startY + 60),
                Name = "separatorHeightComboBox",
                Size = new System.Drawing.Size(120, 21),
                DropDownHeight = 198,
                TabIndex = 0,
                MaxDropDownItems = 10
            };

            columnsComboBox.Items.AddRange(codablockFColumns);
            columnsComboBox.SelectedIndex = 0;
            rowsComboBox.Items.AddRange(codablockFRows);
            rowsComboBox.SelectedIndex = 0;
            separatorHeightComboBox.Items.AddRange(separatorHeight);
            separatorHeightComboBox.SelectedIndex = 0;
            symbolPropertiesTabPage.Controls.Add(columnLabel);
            symbolPropertiesTabPage.Controls.Add(columnsComboBox);
            symbolPropertiesTabPage.Controls.Add(rowLabel);
            symbolPropertiesTabPage.Controls.Add(rowsComboBox);
            symbolPropertiesTabPage.Controls.Add(separatorHeightLabel);
            symbolPropertiesTabPage.Controls.Add(separatorHeightComboBox);
            columnsComboBox.SelectedIndexChanged += new EventHandler(ColumnsComboBox_SelectedIndexChanged);
            rowsComboBox.SelectedIndexChanged += new EventHandler(RowsComboBox_SelectedIndexChanged);
            separatorHeightComboBox.SelectedIndexChanged += new EventHandler(SeparatorHeightComboBox_SelectedIndexChanged);
        }

        /// <summary>
        /// Adds the Code 16K or Code 49 controls and event handlers to the tab page.
        /// </summary>
        private void AddCode16KCode49Controls()
        {
            int startY;

            AddModeControls();
            hibcRadioButton.Enabled = false;
            startY = 65;

            rowLabel = new Label
            {
                Location = new System.Drawing.Point(10, startY + 2),
                Name = "minimumRowsLabel",
                AutoSize = true,
                Text = "Minimum Rows:"
            };

            separatorHeightLabel = new Label
            {
                Location = new System.Drawing.Point(10, startY + 30 + 2),
                Name = "separatorHeightLabel",
                AutoSize = true,
                Text = "Separator Height:"
            };

            rowsComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FormattingEnabled = true,
                Location = new System.Drawing.Point(130, startY),
                Name = "minimumRowsComboBox",
                Size = new System.Drawing.Size(120, 21),
                DropDownHeight = 198,
                TabIndex = 0,
                MaxDropDownItems = 10
            };

            separatorHeightComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FormattingEnabled = true,
                Location = new System.Drawing.Point(130, startY + 30),
                Name = "separatorHeightComboBox",
                Size = new System.Drawing.Size(120, 21),
                DropDownHeight = 198,
                TabIndex = 0,
                MaxDropDownItems = 10
            };

            if (symbolID == Symbology.Code16K)
            {
                rowsComboBox.Items.AddRange(c16KMinimumRows);
            }

            else
            {
                rowsComboBox.Items.AddRange(c49MinimumRows);
            }

            rowsComboBox.SelectedIndex = 0;
            separatorHeightComboBox.Items.AddRange(separatorHeight);
            separatorHeightComboBox.SelectedIndex = 0;
            symbolPropertiesTabPage.Controls.Add(rowLabel);
            symbolPropertiesTabPage.Controls.Add(rowsComboBox);
            symbolPropertiesTabPage.Controls.Add(separatorHeightLabel);
            symbolPropertiesTabPage.Controls.Add(separatorHeightComboBox);
            rowsComboBox.SelectedIndexChanged += new EventHandler(RowsComboBox_SelectedIndexChanged);
            separatorHeightComboBox.SelectedIndexChanged += new EventHandler(SeparatorHeightComboBox_SelectedIndexChanged);
        }

        /// <summary>
        /// Adds the Code One controls and event handlers to the tab page.
        /// </summary>
        private void AddCodeOneControls()
        {
            int startY;

            AddModeControls();
            startY = 65;

            sizeLabel = new Label
            {
                AutoSize = true,
                Location = new System.Drawing.Point(10, startY + 2),
                Name = "code1SizesLabel",
                Size = new System.Drawing.Size(84, 13),
                TabIndex = 0,
                Text = "Symbol Size:"
            };
            symbolPropertiesTabPage.Controls.Add(sizeLabel);

            versionSizesComboBox = new ComboBox
            {
                DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList,
                FormattingEnabled = true,
                Location = new System.Drawing.Point(130, startY),
                Name = "code1SizesComboBox",
                Size = new System.Drawing.Size(120, 21),
                TabIndex = 0,
                MaxDropDownItems = 10
            };

            versionSizesComboBox.Items.AddRange(codeOneSizes);
            versionSizesComboBox.SelectedIndex = 0;
            symbolPropertiesTabPage.Controls.Add(versionSizesComboBox);
            versionSizesComboBox.SelectedIndexChanged += new EventHandler(SymbolSizesComboBox_SelectedIndexChanged);
        }

        private void AddCode128Controls()
        {
            int startY;
            AddModeControls();
            startY = 65;

            codeSetCCheckBox = new CheckBox
            {
                AutoSize = true,
                Location = new System.Drawing.Point(10, startY),
                Name = "codeSetCCheckBox",
                Size = new System.Drawing.Size(80, 17),
                TabIndex = 2,
                Text = "Code Set C Suppression",
                UseVisualStyleBackColor = true,
                Checked = false
            };

            AddCompositeControls(startY + 30);
            symbolPropertiesTabPage.Controls.Add(codeSetCCheckBox);
            codeSetCCheckBox.Click += new EventHandler(CodeSetCCheckBox_CheckedChanged);
        }
        /// <summary>
        /// Adds Data Matrix specific controls to the symbol properties tab page.
        /// </summary>
        private void AddDataMatrixControls()
        {
            int startY;

            AddModeControls();
            startY = 65;

            sizeLabel = new Label
            {
                AutoSize = true,
                Location = new System.Drawing.Point(10, startY + 2),
                Name = "dmSizesLabel",
                Size = new System.Drawing.Size(84, 13),
                TabIndex = 0,
                Text = "Data Matrix Size:"
            };

            versionSizesComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FormattingEnabled = true,
                Location = new System.Drawing.Point(130, startY),
                Name = "symbolSizesComboBox",
                Size = new System.Drawing.Size(120, 21),
                TabIndex = 0,
                MaxDropDownItems = 10
            };

            squareOnlyCheckBox = new CheckBox
            {
                AutoSize = true,
                Location = new System.Drawing.Point(10, startY + 40),
                Name = "squareOnlyCheckBox",
                Size = new System.Drawing.Size(80, 17),
                TabIndex = 1,
                Text = "Suppress Rectangular Symbols in Auto Mode",
                UseVisualStyleBackColor = true,
                Checked = false
            };

            dmreCheckBox = new CheckBox
            {
                AutoSize = true,
                Location = new System.Drawing.Point(10, startY + 70),
                Name = "dmreCheckBox",
                Size = new System.Drawing.Size(80, 17),
                TabIndex = 2,
                Text = "Allow DMRE in Auto Mode",
                UseVisualStyleBackColor = true,
                Checked = false
            };

            symbolPropertiesTabPage.Controls.Add(squareOnlyCheckBox);
            symbolPropertiesTabPage.Controls.Add(dmreCheckBox);
            symbolPropertiesTabPage.Controls.Add(sizeLabel);
            versionSizesComboBox.Items.AddRange(dmSizes);
            versionSizesComboBox.SelectedIndex = 0;
            symbolPropertiesTabPage.Controls.Add(versionSizesComboBox);
            squareOnlyCheckBox.Click += new EventHandler(SquareOnlyCheckBox_CheckedChanged);
            dmreCheckBox.Click += new EventHandler(DmreCheckBox_CheckedChanged);
            versionSizesComboBox.SelectedIndexChanged += new EventHandler(SymbolSizesComboBox_SelectedIndexChanged);
        }


        private void AddDotCodeControls()
        {
            int startY;
            AddModeControls();
            startY = 65;

            columnLabel = new Label
            {
                AutoSize = true,
                Location = new System.Drawing.Point(10, startY + 2),
                Name = "columnLabel",
                Size = new System.Drawing.Size(84, 13),
                TabIndex = 0,
                Text = "Data Columns:"
            };

            userMaskLabel = new Label
            {
                Location = new System.Drawing.Point(10, startY + 2 + 30),
                Name = "userMaskLabel",
                AutoSize = true,
                Text = "User Mask:"
            };

            columnsComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FormattingEnabled = true,
                Location = new System.Drawing.Point(130, startY),
                Name = "columnsComboBox",
                Size = new System.Drawing.Size(120, 21),
                TabIndex = 0,
                MaxDropDownItems = 10
            };

            userMaskComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FormattingEnabled = true,
                Location = new System.Drawing.Point(130, startY + 30),
                Name = "userMaskComboBox",
                Size = new System.Drawing.Size(120, 21),
                DropDownHeight = 198,
                TabIndex = 0,
                MaxDropDownItems = 10
            };

            columnsComboBox.Items.Add("Automatic");
            for (int x = 5; x < 200; x++)
            {
                columnsComboBox.Items.Add(x.ToString());
            }

            userMaskComboBox.Items.AddRange(userMask4);
            userMaskComboBox.SelectedIndex = 0;
            columnsComboBox.SelectedIndex = 0;
            symbolPropertiesTabPage.Controls.Add(columnLabel);
            symbolPropertiesTabPage.Controls.Add(columnsComboBox);
            symbolPropertiesTabPage.Controls.Add(userMaskLabel);
            symbolPropertiesTabPage.Controls.Add(userMaskComboBox);
            columnsComboBox.SelectedIndexChanged += new EventHandler(ColumnsComboBox_SelectedIndexChanged);
            userMaskComboBox.SelectedIndexChanged += new EventHandler(UserMaskComboBox_SelectedIndexChanged);
        }

        /// <summary>
        /// Adds the Maxicode controls and event handlers to the tab page.
        /// </summary>
        /// <param name="index">Default selected index.</param>
        private void AddMaxiCodeControls(int index)
        {
            Label maxicodeModeLabel = new Label
            {
                AutoSize = true,
                Location = new System.Drawing.Point(10, 10 + 2),
                Name = "maxicodeModeLabel",
                Size = new System.Drawing.Size(84, 13),
                TabIndex = 0,
                Text = "Encoding Mode:"
            };

            maxicodeModeComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FormattingEnabled = true,
                Location = new System.Drawing.Point(10, 30),
                Name = "maxicodeModeComboBox",
                Size = new System.Drawing.Size(200, 21),
                TabIndex = 0,
                MaxDropDownItems = 10
            };

            symbolPropertiesTabPage.Controls.Add(maxicodeModeLabel);
            maxicodeModeComboBox.Items.AddRange(maxicodeModes);
            maxicodeModeComboBox.SelectedIndex = index;
            symbolPropertiesTabPage.Controls.Add(maxicodeModeComboBox);
            maxicodeModeComboBox.SelectedIndexChanged += new EventHandler(MaxicodeModeComboBox_SelectedIndexChanged);
        }


        /// <summary>
        /// Adds common 2D controls to the symbol properties tab page.
        /// </summary>
        private void AddGridMatrixControls()
        {
            int startY = 10;

            sizeLabel = new Label
            {
                Location = new System.Drawing.Point(10, startY + 2),
                Name = "sizesLabel",
                AutoSize = true,
                Text = "Size:"
            };

            errorCorrectionLabel = new Label
            {
                Location = new System.Drawing.Point(10, startY + 2 + 30),
                Name = "eccLabel",
                AutoSize = true,
                Text = "Error Correction:"
            };


            versionSizesComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FormattingEnabled = true,
                Location = new System.Drawing.Point(130, startY),
                Name = "sizesComboBox",
                Size = new System.Drawing.Size(120, 21),
                DropDownHeight = 198,
                TabIndex = 0,
                MaxDropDownItems = 10
            };

            errorCorrectionComboBox = new ComboBox
            {
                DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList,
                FormattingEnabled = true,
                Location = new System.Drawing.Point(130, startY + 30),
                Name = "errorCorrectionComboBox",
                Size = new System.Drawing.Size(120, 21),
                DropDownHeight = 198,
                TabIndex = 0,
                MaxDropDownItems = 10
            };

            versionSizesComboBox.Items.AddRange(gridMatrixSizes);
            errorCorrectionComboBox.Items.AddRange(gridMatrixErrorLevels);
            versionSizesComboBox.SelectedIndex = 0;
            errorCorrectionComboBox.SelectedIndex = 0;
            symbolPropertiesTabPage.Controls.Add(sizeLabel);
            symbolPropertiesTabPage.Controls.Add(errorCorrectionLabel);
            symbolPropertiesTabPage.Controls.Add(versionSizesComboBox);
            symbolPropertiesTabPage.Controls.Add(errorCorrectionComboBox);
            versionSizesComboBox.SelectedIndexChanged += new EventHandler(SymbolSizesComboBox_SelectedIndexChanged);
            errorCorrectionComboBox.SelectedIndexChanged += new EventHandler(ErrorCorrectionComboBox_SelectedIndexChanged);

        }

        /// <summary>
        /// Adds the Han Xin Code controls and event handlers to the tab page.
        /// </summary>
        private void AddHanXinControls()
        {
            int startY = 10;

            sizeLabel = new Label
            {
                Location = new System.Drawing.Point(10, startY + 2),
                Name = "sizesLabel",
                AutoSize = true,
                Text = "Size:"
            };

            errorCorrectionLabel = new Label
            {
                Location = new System.Drawing.Point(10, startY + 2 + 30),
                Name = "eccLabel",
                AutoSize = true,
                Text = "Error Correction:"
            };


            versionSizesComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FormattingEnabled = true,
                Location = new System.Drawing.Point(130, startY),
                Name = "sizesComboBox",
                Size = new System.Drawing.Size(120, 21),
                DropDownHeight = 198,
                TabIndex = 0,
                MaxDropDownItems = 10
            };

            errorCorrectionComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FormattingEnabled = true,
                Location = new System.Drawing.Point(130, startY + 30),
                Name = "errorCorrectionComboBox",
                Size = new System.Drawing.Size(120, 21),
                DropDownHeight = 198,
                TabIndex = 0,
                MaxDropDownItems = 10
            };

            userMaskLabel = new Label
            {
                Location = new System.Drawing.Point(10, startY + 2 + 60),
                Name = "userMaskLabel",
                AutoSize = true,
                Text = "User Mask:"
            };

            userMaskComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FormattingEnabled = true,
                Location = new System.Drawing.Point(130, startY + 60),
                Name = "userMaskComboBox",
                Size = new System.Drawing.Size(120, 21),
                DropDownHeight = 198,
                TabIndex = 0,
                MaxDropDownItems = 10
            };

            versionSizesComboBox.Items.AddRange(hanXinSizes);
            errorCorrectionComboBox.Items.AddRange(hanXinErrorLevels);
            userMaskComboBox.Items.AddRange(userMask4);
            userMaskComboBox.SelectedIndex = 0;
            versionSizesComboBox.SelectedIndex = 0;
            errorCorrectionComboBox.SelectedIndex = 0;
            symbolPropertiesTabPage.Controls.Add(sizeLabel);
            symbolPropertiesTabPage.Controls.Add(errorCorrectionLabel);
            symbolPropertiesTabPage.Controls.Add(versionSizesComboBox);
            symbolPropertiesTabPage.Controls.Add(errorCorrectionComboBox);
            symbolPropertiesTabPage.Controls.Add(userMaskLabel);
            symbolPropertiesTabPage.Controls.Add(userMaskComboBox);
            versionSizesComboBox.SelectedIndexChanged += new EventHandler(SymbolSizesComboBox_SelectedIndexChanged);
            errorCorrectionComboBox.SelectedIndexChanged += new EventHandler(ErrorCorrectionComboBox_SelectedIndexChanged);
            userMaskComboBox.SelectedIndexChanged += new EventHandler(UserMaskComboBox_SelectedIndexChanged);
        }


        private void AddITF14Controls()
        {
            noneRadioButton = new RadioButton
            {
                AutoSize = true,
                Location = new System.Drawing.Point(5, 19),
                Name = "noneRadioButton",
                Size = new System.Drawing.Size(68, 17),
                TabIndex = 0,
                TabStop = true,
                Text = "None",
                UseVisualStyleBackColor = true
            };

            horizonalRadioButton = new RadioButton
            {
                AutoSize = true,
                Location = new System.Drawing.Point(5, 39),
                Name = "horizonalRadioButton",
                Size = new System.Drawing.Size(46, 17),
                TabIndex = 1,
                TabStop = true,
                Text = "Horizontal Bars",
                UseVisualStyleBackColor = true
            };

            rectangleRadioButton = new RadioButton
            {
                AutoSize = true,
                Location = new System.Drawing.Point(5, 59),
                Name = "rectangleRadioButton",
                Size = new System.Drawing.Size(50, 17),
                TabIndex = 2,
                TabStop = true,
                Text = "Rectangle",
                UseVisualStyleBackColor = true,
                Checked = true
            };

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
            noneRadioButton.CheckedChanged += new EventHandler(noneRadioButton_CheckedChanged);
            horizonalRadioButton.CheckedChanged += new EventHandler(horizonalRadioButton_CheckedChanged);
            rectangleRadioButton.CheckedChanged += new EventHandler(rectangleRadioButton_CheckedChanged);
        }

        private void AddQRCodeControls()
        {
            int startY = 10;

            if (symbolID == Symbology.QRCode || symbolID == Symbology.RectangularMicroQRCode)
            {
                AddModeControls();
                if(symbolID == Symbology.RectangularMicroQRCode)
                {
                    hibcRadioButton.Enabled = false;
                }

                startY = 65;
            }

            sizeLabel = new Label
            {
                Location = new System.Drawing.Point(10, startY + 2),
                Name = "sizesLabel",
                AutoSize = true,
                Text = "Size:"
            };

            errorCorrectionLabel = new Label
            {
                Location = new System.Drawing.Point(10, startY + 2 + 30),
                Name = "eccLabel",
                AutoSize = true,
                Text = "Error Correction:"
            };


            versionSizesComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FormattingEnabled = true,
                Location = new System.Drawing.Point(130, startY),
                Name = "sizesComboBox",
                Size = new System.Drawing.Size(120, 21),
                DropDownHeight = 198,
                TabIndex = 0,
                MaxDropDownItems = 10
            };

            errorCorrectionComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FormattingEnabled = true,
                Location = new System.Drawing.Point(130, startY + 30),
                Name = "errorCorrectionComboBox",
                Size = new System.Drawing.Size(120, 21),
                DropDownHeight = 198,
                TabIndex = 0,
                MaxDropDownItems = 10
            };

            if (symbolID == Symbology.QRCode || symbolID == Symbology.MicroQRCode)
            {
                userMaskLabel = new Label
                {
                    Location = new System.Drawing.Point(10, startY + 2 + 60),
                    Name = "userMaskLabel",
                    AutoSize = true,
                    Text = "User Mask:"
                };

                userMaskComboBox = new ComboBox
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    FormattingEnabled = true,
                    Location = new System.Drawing.Point(130, startY + 60),
                    Name = "userMaskComboBox",
                    Size = new System.Drawing.Size(120, 21),
                    DropDownHeight = 198,
                    TabIndex = 0,
                    MaxDropDownItems = 10
                };

                if (symbolID == Symbology.QRCode)
                {
                    versionSizesComboBox.Items.AddRange(qrSizes);
                    errorCorrectionComboBox.Items.AddRange(qrErrorLevels);
                    userMaskComboBox.Items.AddRange(userMask8);
                }

                else
                {
                    versionSizesComboBox.Items.AddRange(mqrSizes);
                    errorCorrectionComboBox.Items.AddRange(mqrErrorLevels);
                    userMaskComboBox.Items.AddRange(userMask4);
                }

                userMaskComboBox.SelectedIndex = 0;
                symbolPropertiesTabPage.Controls.Add(userMaskLabel);
                symbolPropertiesTabPage.Controls.Add(userMaskComboBox);
                userMaskComboBox.SelectedIndexChanged += new EventHandler(UserMaskComboBox_SelectedIndexChanged);
            }

            else
            {
                versionSizesComboBox.Items.AddRange(rmqrSizes);
                errorCorrectionComboBox.Items.AddRange(rmqrErrorLevels);
            }

            versionSizesComboBox.SelectedIndex = 0;
            errorCorrectionComboBox.SelectedIndex = 0;
            symbolPropertiesTabPage.Controls.Add(sizeLabel);
            symbolPropertiesTabPage.Controls.Add(errorCorrectionLabel);
            symbolPropertiesTabPage.Controls.Add(versionSizesComboBox);
            symbolPropertiesTabPage.Controls.Add(errorCorrectionComboBox);
            versionSizesComboBox.SelectedIndexChanged += new EventHandler(SymbolSizesComboBox_SelectedIndexChanged);
            errorCorrectionComboBox.SelectedIndexChanged += new EventHandler(ErrorCorrectionComboBox_SelectedIndexChanged);
            
        }
        private void AddCode39Controls()
        {
            //int startY = -20;

            if (symbolID == Symbology.Code39)
            {
                AddModeControls();
                gs1RadioButton.Enabled = false;
            }

            /*if (symbolID != Symbology.Code93)
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

            showOptCheckDigitCheckBox = new CheckBox();
            showOptCheckDigitCheckBox.AutoSize = true;
            showOptCheckDigitCheckBox.Location = new System.Drawing.Point(10, startY + 30);
            showOptCheckDigitCheckBox.Name = "showCheckDigitCheckBox";
            showOptCheckDigitCheckBox.Size = new System.Drawing.Size(147, 17);
            showOptCheckDigitCheckBox.TabIndex = 1;
            showOptCheckDigitCheckBox.Text = "Show Check Digit(s) In Text";
            showOptCheckDigitCheckBox.UseVisualStyleBackColor = true;
            showOptCheckDigitCheckBox.Checked = false;
            symbolPropertiesTabPage.Controls.Add(showOptCheckDigitCheckBox);
            showOptCheckDigitCheckBox.Click += new System.EventHandler(this.showOptCheckDigitCheckBox_CheckedChange);*/
        }

        /// <summary>
        /// Adds mode selection controls to the symbol properties tab page.
        /// </summary>
        private void AddModeControls()
        {
            standardRadioButton = new RadioButton
            {
                AutoSize = true,
                Location = new System.Drawing.Point(5, 19),
                Name = "standardRadioButton",
                Size = new System.Drawing.Size(68, 17),
                TabIndex = 0,
                TabStop = true,
                Text = "Standard",
                UseVisualStyleBackColor = true,
                Checked = true
            };

            gs1RadioButton = new RadioButton
            {
                AutoSize = true,
                Location = new System.Drawing.Point(80, 19),
                Name = "gs1RadioButton",
                Size = new System.Drawing.Size(46, 17),
                TabIndex = 1,
                TabStop = true,
                Text = "GS1",
                UseVisualStyleBackColor = true
            };

            hibcRadioButton = new RadioButton
            {
                AutoSize = true,
                Location = new System.Drawing.Point(150, 19),
                Name = "hibcRadioButton",
                Size = new System.Drawing.Size(50, 17),
                TabIndex = 2,
                TabStop = true,
                Text = "HIBC",
                UseVisualStyleBackColor = true
            };

            modeGroupBox = new GroupBox
            {
                Location = new System.Drawing.Point(10, 10),
                Name = "modeGroupBox",
                Size = new System.Drawing.Size(200, 45),
                TabIndex = 10,
                TabStop = false,
                Text = "Mode"
            };

            modeGroupBox.Controls.Add(hibcRadioButton);
            modeGroupBox.Controls.Add(gs1RadioButton);
            modeGroupBox.Controls.Add(standardRadioButton);
            symbolPropertiesTabPage.Controls.Add(modeGroupBox);
            standardRadioButton.CheckedChanged += new EventHandler(StandardRadioButton_CheckedChanged);
            gs1RadioButton.CheckedChanged += new EventHandler(GS1RadioButton_CheckedChanged);
            hibcRadioButton.CheckedChanged += new EventHandler(HIBCRadioButton_CheckedChanged);
        }

        /// <summary>
        /// Adds the composite data controls to the symbol properties tab page.
        /// </summary>
        private void AddCompositeControls(int startY)
        {
            compositeGroupBox = new GroupBox
            {
                Location = new System.Drawing.Point(10, startY),
                Name = "compositeGroupBox",
                Size = new System.Drawing.Size(200, 45),
                TabIndex = 4,
                TabStop = false,
                Text = "Composite Type:"
            };

            ccaRadioButton = new RadioButton
            {
                AutoSize = true,
                Location = new System.Drawing.Point(5, 19),
                Name = "ccaRadioButton",
                Size = new System.Drawing.Size(46, 17),
                TabIndex = 1,
                TabStop = true,
                Text = "CCA",
                UseVisualStyleBackColor = true,
                Checked = true
            };

            ccbRadioButton = new RadioButton
            {
                AutoSize = true,
                Location = new System.Drawing.Point(80, 19),
                Name = "ccbRadioButton",
                Size = new System.Drawing.Size(46, 17),
                TabIndex = 2,
                TabStop = true,
                Text = "CCB",
                UseVisualStyleBackColor = true
            };

            cccRadioButton = new RadioButton
            {
                AutoSize = true,
                Location = new System.Drawing.Point(150, 19),
                Name = "cccRadioButton",
                Size = new System.Drawing.Size(46, 17),
                TabIndex = 3,
                TabStop = true,
                Text = "CCC",
                UseVisualStyleBackColor = true
            };

            compositeGroupBox.Controls.Add(ccaRadioButton);
            compositeGroupBox.Controls.Add(ccbRadioButton);
            compositeGroupBox.Controls.Add(cccRadioButton);
            symbolPropertiesTabPage.Controls.Add(compositeGroupBox);

            compositeDataLabel = new Label
            {
                AutoSize = true,
                Location = new System.Drawing.Point(10, startY + 50),
                Name = "compositeDataLabel",
                Size = new System.Drawing.Size(85, 13),
                TabIndex = 0,
                Text = "Composite Data:"
            };

            symbolPropertiesTabPage.Controls.Add(compositeDataLabel);

            compositeDataTextbox = new TextBox
            {
                ImeMode = ImeMode.NoControl,
                Location = new System.Drawing.Point(10, startY + 70),
                Multiline = true,
                Name = "compositeDataTextbox",
                ScrollBars = ScrollBars.Vertical,
                Size = new System.Drawing.Size(200, 39),
                Text = compositeText,
                TabIndex = 1
            };

            if (symbolID == Symbology.Code128)
            {
                compositeDataLabel.Enabled = false;
                compositeGroupBox.Enabled = false;
                compositeDataTextbox.Enabled = false;
            }

            symbolPropertiesTabPage.Controls.Add(compositeDataTextbox);
            cccRadioButton.CheckedChanged += new EventHandler(cccRadioButton_CheckedChanged);
            ccaRadioButton.CheckedChanged += new EventHandler(ccaRadioButton_CheckedChanged);
            ccbRadioButton.CheckedChanged += new EventHandler(ccbRadioButton_CheckedChanged);
        }


        private void AddEanUpcControls()
        {
            AddSupplimentDataControls();
            if (symbolID != Symbology.ISBN)
            {
                AddCompositeControls(60);
            }
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
            {
                errorLevelComboBox.Items.Add("Level " + x.ToString());
            }

            errorLevelComboBox.SelectedIndex = 0;
            symbolPropertiesTabPage.Controls.Add(errorLevelComboBox);
            errorLevelComboBox.SelectedIndexChanged += new EventHandler(this.ErrorCorrectionComboBox_SelectedIndexChanged);
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
            {
                columnsComboBox.Items.Add(x.ToString());
            }

            columnsComboBox.SelectedIndex = 0;
            symbolPropertiesTabPage.Controls.Add(columnsComboBox);
            columnsComboBox.SelectedIndexChanged += new EventHandler(this.ColumnsComboBox_SelectedIndexChanged);
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
            {
                columnsComboBox.Items.AddRange(pdfColumns);
            }
            else
            {
                columnsComboBox.Items.AddRange(mPdfColumns);
            }

            columnsComboBox.SelectedIndex = 0;
            symbolPropertiesTabPage.Controls.Add(columnsComboBox);
            columnsComboBox.SelectedIndexChanged += new EventHandler(this.ColumnsComboBox_SelectedIndexChanged);

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
                errorLevelComboBox.SelectedIndexChanged += new EventHandler(this.pdfErrorLevelComboBox_SelectedIndexChanged);

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
                pdfRowHeightComboBox.SelectedIndexChanged += new EventHandler(this.pdfRowHeightComboBox_SelectedIndexChanged);
            }
        }

        private void AddUPNQRControls()
        {
            int startY = 10;

            userMaskLabel = new Label
            {
                Location = new System.Drawing.Point(10, startY + 2),
                Name = "userMaskLabel",
                AutoSize = true,
                Text = "User Mask:"
            };

            userMaskComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FormattingEnabled = true,
                Location = new System.Drawing.Point(130, startY ),
                Name = "userMaskComboBox",
                Size = new System.Drawing.Size(120, 21),
                DropDownHeight = 198,
                TabIndex = 0,
                MaxDropDownItems = 10
            };

            userMaskComboBox.Items.AddRange(userMask8);
            userMaskComboBox.SelectedIndex = 0;
            symbolPropertiesTabPage.Controls.Add(userMaskLabel);
            symbolPropertiesTabPage.Controls.Add(userMaskComboBox);
            userMaskComboBox.SelectedIndexChanged += new EventHandler(UserMaskComboBox_SelectedIndexChanged);
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
            columnsComboBox.SelectedIndexChanged += new EventHandler(this.ColumnsComboBox_SelectedIndexChanged);
        }

        /// <summary>
        /// Adds the EAN/UPC/ISBN suppliment controls.
        /// </summary>
        private void AddSupplimentDataControls()
        {
            int startY = 10;

            Label supplimentLabel = new Label
            {
                AutoSize = true,
                Location = new System.Drawing.Point(10, startY),
                Name = "supplimentLabel",
                Size = new System.Drawing.Size(114, 13),
                TabIndex = 0
            };

            if (symbolID == Symbology.ISBN)
            {
                supplimentLabel.Text = "ISBN Suppliment";
            }

            else
            {
                supplimentLabel.Text = "EAN/UPC Suppliment:";
            }

            supplementDataTextBox = new TextBox
            {
                Location = new System.Drawing.Point(10, startY + 20),
                Name = "supplimentDataTextbox",
                Size = new System.Drawing.Size(242, 20),
                Text = supplementText,
                TabIndex = 1
            };

            symbolPropertiesTabPage.Controls.Add(supplimentLabel);
            symbolPropertiesTabPage.Controls.Add(supplementDataTextBox);
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
                        if (symbolPropertiesTabPage.Controls[i].Name == "codeSetCCheckBox")
                        {
                            codeSetCCheckBox.Click -= new EventHandler(CodeSetCCheckBox_CheckedChanged);
                        }

                        if (symbolPropertiesTabPage.Controls[i].Name == "squareOnlyCheckBox")
                        {
                            squareOnlyCheckBox.Click -= new EventHandler(SquareOnlyCheckBox_CheckedChanged);
                        }

                        if (symbolPropertiesTabPage.Controls[i].Name == "dmreCheckBox")
                        {
                            dmreCheckBox.Click -= new EventHandler(DmreCheckBox_CheckedChanged);
                        }

                        if (symbolPropertiesTabPage.Controls[i].Name == "showCheckDigitCheckBox")
                        {
                            showOptCheckDigitCheckBox.Click -= new EventHandler(showOptCheckDigitCheckBox_CheckedChange);
                        }

                        if (symbolPropertiesTabPage.Controls[i].Name == "useCheckDigitCheckBox")
                        {
                            optionalCheckDigitCheckBox.Click -= new EventHandler(optionalCheckDigitCheckBox_CheckedChanged);
                        }

                        if (symbolPropertiesTabPage.Controls[i].Name == "modeGroupBox")
                        {
                            hibcRadioButton.CheckedChanged -= new EventHandler(HIBCRadioButton_CheckedChanged);
                            hibcRadioButton.Dispose();
                            gs1RadioButton.CheckedChanged -= new EventHandler(GS1RadioButton_CheckedChanged);
                            gs1RadioButton.Dispose();
                            standardRadioButton.CheckedChanged -= new EventHandler(StandardRadioButton_CheckedChanged);
                            standardRadioButton.Dispose();
                        }

                        if (symbolPropertiesTabPage.Controls[i].Name == "compositeGroupBox")
                        {
                            ccaRadioButton.CheckedChanged -= new EventHandler(ccaRadioButton_CheckedChanged);
                            ccaRadioButton.Dispose();
                            ccbRadioButton.CheckedChanged -= new EventHandler(ccbRadioButton_CheckedChanged);
                            ccbRadioButton.Dispose();
                            cccRadioButton.CheckedChanged -= new EventHandler(cccRadioButton_CheckedChanged);
                            cccRadioButton.Dispose();
                        }

                        if (symbolPropertiesTabPage.Controls[i].Name == "bearerStyleGroupBox")
                        {
                            noneRadioButton.CheckedChanged -= new EventHandler(noneRadioButton_CheckedChanged);
                            noneRadioButton.Dispose();
                            horizonalRadioButton.CheckedChanged -= new EventHandler(horizonalRadioButton_CheckedChanged);
                            horizonalRadioButton.Dispose();
                            rectangleRadioButton.CheckedChanged -= new EventHandler(rectangleRadioButton_CheckedChanged);
                            rectangleRadioButton.Dispose();
                        }

                        // RadioButton events.
                        if (symbolPropertiesTabPage.Controls[i].Name == "autoSizeRadioButton")
                        {
                            autoResizeRadioButton.CheckedChanged -= new EventHandler(this.AutoResizeRadioButton_CheckedChanged);
                        }

                        if (symbolPropertiesTabPage.Controls[i].Name == "sizesRadioButton")
                        {
                            autoResizeRadioButton.CheckedChanged -= new EventHandler(this.SizesRadioButton_CheckedChanged);
                        }

                        if (symbolPropertiesTabPage.Controls[i].Name == "errorCorrectionRadioButton")
                        {
                            autoResizeRadioButton.CheckedChanged -= new EventHandler(this.ErrorCorrectionRadioButton_CheckedChanged);
                        }

                        // ComboBox events.
                        if (symbolPropertiesTabPage.Controls[i].Name == "maxicodeModeComboBox")
                        {
                            maxicodeModeComboBox.SelectedIndexChanged -= new EventHandler(MaxicodeModeComboBox_SelectedIndexChanged);
                        }

                        if (symbolPropertiesTabPage.Controls[i].Name == "columnsComboBox")
                        {
                            columnsComboBox.SelectedIndexChanged -= new EventHandler(ColumnsComboBox_SelectedIndexChanged);
                        }

                        if (symbolPropertiesTabPage.Controls[i].Name == "rowsComboBox")
                        {
                            rowsComboBox.SelectedIndexChanged -= new EventHandler(RowsComboBox_SelectedIndexChanged);
                        }

                        if (symbolPropertiesTabPage.Controls[i].Name == "separatorHeightComboBox")
                        {
                            separatorHeightComboBox.SelectedIndexChanged -= new EventHandler(SeparatorHeightComboBox_SelectedIndexChanged);
                        }

                        if (symbolPropertiesTabPage.Controls[i].Name == "pdfRowHeightComboBox")
                        {
                            pdfRowHeightComboBox.SelectedIndexChanged -= new EventHandler(pdfRowHeightComboBox_SelectedIndexChanged);
                        }

                        if (symbolPropertiesTabPage.Controls[i].Name == "symbolSizesComboBox")
                        {
                            versionSizesComboBox.SelectedIndexChanged -= new EventHandler(SymbolSizesComboBox_SelectedIndexChanged);
                        }

                        if (symbolPropertiesTabPage.Controls[i].Name == "errorCorrectionComboBox")
                        {
                            errorCorrectionComboBox.SelectedIndexChanged -= new EventHandler(ErrorCorrectionComboBox_SelectedIndexChanged);
                        }

                        if (symbolPropertiesTabPage.Controls[i].Name == "userMaskComboBox")
                        {
                            userMaskComboBox.SelectedIndexChanged -= new EventHandler(UserMaskComboBox_SelectedIndexChanged);
                        }



                        symbolPropertiesTabPage.Controls[i].Dispose();
                    }

                    symbolPropertiesTabPage.Controls.Clear();
                }

                // Finally remove the tab page.
                tabControl1.Controls.Remove(symbolPropertiesTabPage);
                symbolPropertiesTabPage = null;
            }
        }
    }
}
