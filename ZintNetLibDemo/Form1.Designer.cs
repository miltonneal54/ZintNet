namespace ZintNetLibTest
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.generateButton = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pNGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bMPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gIFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tIFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imagePanel = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label18 = new System.Windows.Forms.Label();
            this.symbologyComboBox = new System.Windows.Forms.ComboBox();
            this.barcodeDataTextBox = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.outputTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.commomPropertiesTabPage = new System.Windows.Forms.TabPage();
            this.textAlignComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textPositionComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.rotateTextBox = new System.Windows.Forms.TextBox();
            this.showTextCheckBox = new System.Windows.Forms.CheckBox();
            this.barHeightNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.textMarginNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.rotateButton = new System.Windows.Forms.Button();
            this.textColorButton = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.label14 = new System.Windows.Forms.Label();
            this.barcodeColorButton = new System.Windows.Forms.Button();
            this.label15 = new System.Windows.Forms.Label();
            this.multiplierNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label16 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.commomPropertiesTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.barHeightNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textMarginNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.multiplierNumericUpDown)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // generateButton
            // 
            this.generateButton.Location = new System.Drawing.Point(73, 443);
            this.generateButton.Name = "generateButton";
            this.generateButton.Size = new System.Drawing.Size(170, 21);
            this.generateButton.TabIndex = 3;
            this.generateButton.Text = "Generate Barcode";
            this.generateButton.UseVisualStyleBackColor = true;
            this.generateButton.Click += new System.EventHandler(this.generateButton_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1026, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.printToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // printToolStripMenuItem
            // 
            this.printToolStripMenuItem.Name = "printToolStripMenuItem";
            this.printToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.printToolStripMenuItem.Text = "Print";
            this.printToolStripMenuItem.Click += new System.EventHandler(this.PrintToolStripMenuItemClick);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pNGToolStripMenuItem,
            this.bMPToolStripMenuItem,
            this.gIFToolStripMenuItem,
            this.tIFToolStripMenuItem});
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.saveAsToolStripMenuItem.Text = "Save As . . .";
            // 
            // pNGToolStripMenuItem
            // 
            this.pNGToolStripMenuItem.Name = "pNGToolStripMenuItem";
            this.pNGToolStripMenuItem.Size = new System.Drawing.Size(99, 22);
            this.pNGToolStripMenuItem.Text = "PNG";
            this.pNGToolStripMenuItem.Click += new System.EventHandler(this.pNGToolStripMenuItem_Click);
            // 
            // bMPToolStripMenuItem
            // 
            this.bMPToolStripMenuItem.Name = "bMPToolStripMenuItem";
            this.bMPToolStripMenuItem.Size = new System.Drawing.Size(99, 22);
            this.bMPToolStripMenuItem.Text = "BMP";
            this.bMPToolStripMenuItem.Click += new System.EventHandler(this.bMPToolStripMenuItem_Click);
            // 
            // gIFToolStripMenuItem
            // 
            this.gIFToolStripMenuItem.Name = "gIFToolStripMenuItem";
            this.gIFToolStripMenuItem.Size = new System.Drawing.Size(99, 22);
            this.gIFToolStripMenuItem.Text = "GIF";
            this.gIFToolStripMenuItem.Click += new System.EventHandler(this.gIFToolStripMenuItem_Click);
            // 
            // tIFToolStripMenuItem
            // 
            this.tIFToolStripMenuItem.Name = "tIFToolStripMenuItem";
            this.tIFToolStripMenuItem.Size = new System.Drawing.Size(99, 22);
            this.tIFToolStripMenuItem.Text = "TIFF";
            this.tIFToolStripMenuItem.Click += new System.EventHandler(this.tIFToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(129, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // imagePanel
            // 
            this.imagePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.imagePanel.BackColor = System.Drawing.Color.White;
            this.imagePanel.Location = new System.Drawing.Point(320, 38);
            this.imagePanel.Name = "imagePanel";
            this.imagePanel.Size = new System.Drawing.Size(694, 290);
            this.imagePanel.TabIndex = 11;
            this.imagePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.ImagePanelPaint);
            this.imagePanel.Resize += new System.EventHandler(this.ImagePanelResize);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.label18);
            this.groupBox1.Controls.Add(this.symbologyComboBox);
            this.groupBox1.Controls.Add(this.barcodeDataTextBox);
            this.groupBox1.Controls.Add(this.label20);
            this.groupBox1.Location = new System.Drawing.Point(12, 38);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(289, 119);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Barcode Input";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(13, 16);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(61, 13);
            this.label18.TabIndex = 7;
            this.label18.Text = "Symbology:";
            // 
            // symbologyComboBox
            // 
            this.symbologyComboBox.DropDownHeight = 198;
            this.symbologyComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.symbologyComboBox.FormattingEnabled = true;
            this.symbologyComboBox.IntegralHeight = false;
            this.symbologyComboBox.Location = new System.Drawing.Point(16, 34);
            this.symbologyComboBox.Name = "symbologyComboBox";
            this.symbologyComboBox.Size = new System.Drawing.Size(258, 21);
            this.symbologyComboBox.TabIndex = 0;
            this.symbologyComboBox.SelectedIndexChanged += new System.EventHandler(this.SymbologyComboBoxSelectedIndexChanged);
            // 
            // barcodeDataTextBox
            // 
            this.barcodeDataTextBox.Location = new System.Drawing.Point(16, 76);
            this.barcodeDataTextBox.Name = "barcodeDataTextBox";
            this.barcodeDataTextBox.Size = new System.Drawing.Size(258, 20);
            this.barcodeDataTextBox.TabIndex = 1;
            this.barcodeDataTextBox.WordWrap = false;
            this.barcodeDataTextBox.TextChanged += new System.EventHandler(this.barcodeDataTextBox_TextChanged);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(14, 60);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(76, 13);
            this.label20.TabIndex = 0;
            this.label20.Text = "Barcode Data:";
            // 
            // outputTextBox
            // 
            this.outputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.outputTextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputTextBox.Location = new System.Drawing.Point(320, 347);
            this.outputTextBox.Multiline = true;
            this.outputTextBox.Name = "outputTextBox";
            this.outputTextBox.ReadOnly = true;
            this.outputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.outputTextBox.Size = new System.Drawing.Size(694, 117);
            this.outputTextBox.TabIndex = 4;
            this.outputTextBox.WordWrap = false;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(317, 331);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Binary Output:";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.commomPropertiesTabPage);
            this.tabControl1.Location = new System.Drawing.Point(8, 19);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(270, 234);
            this.tabControl1.TabIndex = 2;
            // 
            // commomPropertiesTabPage
            // 
            this.commomPropertiesTabPage.Controls.Add(this.textAlignComboBox);
            this.commomPropertiesTabPage.Controls.Add(this.label3);
            this.commomPropertiesTabPage.Controls.Add(this.textPositionComboBox);
            this.commomPropertiesTabPage.Controls.Add(this.label2);
            this.commomPropertiesTabPage.Controls.Add(this.rotateTextBox);
            this.commomPropertiesTabPage.Controls.Add(this.showTextCheckBox);
            this.commomPropertiesTabPage.Controls.Add(this.barHeightNumericUpDown);
            this.commomPropertiesTabPage.Controls.Add(this.label6);
            this.commomPropertiesTabPage.Controls.Add(this.textMarginNumericUpDown);
            this.commomPropertiesTabPage.Controls.Add(this.label8);
            this.commomPropertiesTabPage.Controls.Add(this.rotateButton);
            this.commomPropertiesTabPage.Controls.Add(this.textColorButton);
            this.commomPropertiesTabPage.Controls.Add(this.label13);
            this.commomPropertiesTabPage.Controls.Add(this.button4);
            this.commomPropertiesTabPage.Controls.Add(this.label14);
            this.commomPropertiesTabPage.Controls.Add(this.barcodeColorButton);
            this.commomPropertiesTabPage.Controls.Add(this.label15);
            this.commomPropertiesTabPage.Controls.Add(this.multiplierNumericUpDown);
            this.commomPropertiesTabPage.Controls.Add(this.label16);
            this.commomPropertiesTabPage.Location = new System.Drawing.Point(4, 22);
            this.commomPropertiesTabPage.Name = "commomPropertiesTabPage";
            this.commomPropertiesTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.commomPropertiesTabPage.Size = new System.Drawing.Size(262, 208);
            this.commomPropertiesTabPage.TabIndex = 0;
            this.commomPropertiesTabPage.Text = "Common Properties";
            this.commomPropertiesTabPage.UseVisualStyleBackColor = true;
            // 
            // textAlignComboBox
            // 
            this.textAlignComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.textAlignComboBox.FormattingEnabled = true;
            this.textAlignComboBox.Items.AddRange(new object[] {
            "Centered",
            "Left",
            "Right",
            "Stretched"});
            this.textAlignComboBox.Location = new System.Drawing.Point(115, 176);
            this.textAlignComboBox.Name = "textAlignComboBox";
            this.textAlignComboBox.Size = new System.Drawing.Size(138, 21);
            this.textAlignComboBox.TabIndex = 30;
            this.textAlignComboBox.SelectedIndexChanged += new System.EventHandler(this.textAlignComboBox_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 179);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 13);
            this.label3.TabIndex = 29;
            this.label3.Text = "Text Alignment:";
            // 
            // textPositionComboBox
            // 
            this.textPositionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.textPositionComboBox.FormattingEnabled = true;
            this.textPositionComboBox.Items.AddRange(new object[] {
            "Under barcode",
            "Above barcode"});
            this.textPositionComboBox.Location = new System.Drawing.Point(114, 144);
            this.textPositionComboBox.Name = "textPositionComboBox";
            this.textPositionComboBox.Size = new System.Drawing.Size(138, 21);
            this.textPositionComboBox.TabIndex = 15;
            this.textPositionComboBox.SelectedIndexChanged += new System.EventHandler(this.textPositionComboBox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 147);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Text Position:";
            // 
            // rotateTextBox
            // 
            this.rotateTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rotateTextBox.CausesValidation = false;
            this.rotateTextBox.Enabled = false;
            this.rotateTextBox.Location = new System.Drawing.Point(93, 112);
            this.rotateTextBox.Name = "rotateTextBox";
            this.rotateTextBox.ReadOnly = true;
            this.rotateTextBox.Size = new System.Drawing.Size(31, 20);
            this.rotateTextBox.TabIndex = 28;
            this.rotateTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // showTextCheckBox
            // 
            this.showTextCheckBox.AutoSize = true;
            this.showTextCheckBox.Checked = true;
            this.showTextCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showTextCheckBox.Location = new System.Drawing.Point(142, 115);
            this.showTextCheckBox.Name = "showTextCheckBox";
            this.showTextCheckBox.Size = new System.Drawing.Size(77, 17);
            this.showTextCheckBox.TabIndex = 8;
            this.showTextCheckBox.Text = "Show Text";
            this.showTextCheckBox.UseVisualStyleBackColor = true;
            this.showTextCheckBox.Click += new System.EventHandler(this.showTextCheckBox_CheckedChanged);
            // 
            // barHeightNumericUpDown
            // 
            this.barHeightNumericUpDown.DecimalPlaces = 2;
            this.barHeightNumericUpDown.Location = new System.Drawing.Point(70, 49);
            this.barHeightNumericUpDown.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.barHeightNumericUpDown.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.barHeightNumericUpDown.Name = "barHeightNumericUpDown";
            this.barHeightNumericUpDown.Size = new System.Drawing.Size(54, 20);
            this.barHeightNumericUpDown.TabIndex = 3;
            this.barHeightNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.barHeightNumericUpDown.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.barHeightNumericUpDown.ValueChanged += new System.EventHandler(this.barHeightNumericUpDown_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 53);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 13);
            this.label6.TabIndex = 27;
            this.label6.Text = "Height:";
            // 
            // textMarginNumericUpDown
            // 
            this.textMarginNumericUpDown.DecimalPlaces = 1;
            this.textMarginNumericUpDown.Location = new System.Drawing.Point(206, 19);
            this.textMarginNumericUpDown.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.textMarginNumericUpDown.Name = "textMarginNumericUpDown";
            this.textMarginNumericUpDown.Size = new System.Drawing.Size(46, 20);
            this.textMarginNumericUpDown.TabIndex = 2;
            this.textMarginNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textMarginNumericUpDown.ValueChanged += new System.EventHandler(this.textMarginNumericUpDown_ValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(139, 21);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(66, 13);
            this.label8.TabIndex = 26;
            this.label8.Text = "Text Margin:";
            // 
            // rotateButton
            // 
            this.rotateButton.Location = new System.Drawing.Point(11, 112);
            this.rotateButton.Name = "rotateButton";
            this.rotateButton.Size = new System.Drawing.Size(59, 20);
            this.rotateButton.TabIndex = 7;
            this.rotateButton.Text = "Rotate";
            this.rotateButton.UseVisualStyleBackColor = true;
            this.rotateButton.Click += new System.EventHandler(this.rotateButton_Click);
            // 
            // textColorButton
            // 
            this.textColorButton.BackColor = System.Drawing.Color.Black;
            this.textColorButton.FlatAppearance.BorderSize = 0;
            this.textColorButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.textColorButton.ForeColor = System.Drawing.Color.Black;
            this.textColorButton.Location = new System.Drawing.Point(222, 80);
            this.textColorButton.Name = "textColorButton";
            this.textColorButton.Size = new System.Drawing.Size(30, 20);
            this.textColorButton.TabIndex = 6;
            this.textColorButton.UseVisualStyleBackColor = false;
            this.textColorButton.Click += new System.EventHandler(this.textColorButton_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(139, 84);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(58, 13);
            this.label13.TabIndex = 25;
            this.label13.Text = "Text Color:";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(193, 49);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(60, 20);
            this.button4.TabIndex = 4;
            this.button4.Text = "Select";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.fontButton_Click);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(139, 53);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(55, 13);
            this.label14.TabIndex = 23;
            this.label14.Text = "Text Font:";
            // 
            // barcodeColorButton
            // 
            this.barcodeColorButton.BackColor = System.Drawing.Color.Black;
            this.barcodeColorButton.FlatAppearance.BorderSize = 0;
            this.barcodeColorButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.barcodeColorButton.ForeColor = System.Drawing.Color.Black;
            this.barcodeColorButton.Location = new System.Drawing.Point(94, 80);
            this.barcodeColorButton.Name = "barcodeColorButton";
            this.barcodeColorButton.Size = new System.Drawing.Size(30, 20);
            this.barcodeColorButton.TabIndex = 5;
            this.barcodeColorButton.UseVisualStyleBackColor = false;
            this.barcodeColorButton.Click += new System.EventHandler(this.barcodeColorButton_Click);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(9, 84);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(77, 13);
            this.label15.TabIndex = 18;
            this.label15.Text = "Barcode Color:";
            // 
            // multiplierNumericUpDown
            // 
            this.multiplierNumericUpDown.DecimalPlaces = 2;
            this.multiplierNumericUpDown.Location = new System.Drawing.Point(70, 19);
            this.multiplierNumericUpDown.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.multiplierNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.multiplierNumericUpDown.Name = "multiplierNumericUpDown";
            this.multiplierNumericUpDown.Size = new System.Drawing.Size(54, 20);
            this.multiplierNumericUpDown.TabIndex = 1;
            this.multiplierNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.multiplierNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.multiplierNumericUpDown.ValueChanged += new System.EventHandler(this.multiplierNumericUpDown_ValueChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(9, 21);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(51, 13);
            this.label16.TabIndex = 15;
            this.label16.Text = "Multiplier:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tabControl1);
            this.groupBox2.Location = new System.Drawing.Point(12, 174);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(289, 261);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Symbol Properties";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Thistle;
            this.ClientSize = new System.Drawing.Size(1026, 471);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.generateButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.outputTextBox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.imagePanel);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ZintNET Demo";
            this.Load += new System.EventHandler(this.Form1Load);
            this.Shown += new System.EventHandler(this.Form1Shown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.commomPropertiesTabPage.ResumeLayout(false);
            this.commomPropertiesTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.barHeightNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textMarginNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.multiplierNumericUpDown)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button generateButton;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Panel imagePanel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.ComboBox symbologyComboBox;
        private System.Windows.Forms.TextBox barcodeDataTextBox;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox outputTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage commomPropertiesTabPage;
        private System.Windows.Forms.TextBox rotateTextBox;
        private System.Windows.Forms.CheckBox showTextCheckBox;
        private System.Windows.Forms.NumericUpDown barHeightNumericUpDown;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown textMarginNumericUpDown;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button rotateButton;
        private System.Windows.Forms.Button textColorButton;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button barcodeColorButton;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.NumericUpDown multiplierNumericUpDown;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ToolStripMenuItem pNGToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bMPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gIFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tIFToolStripMenuItem;
        private System.Windows.Forms.ComboBox textPositionComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox textAlignComboBox;
        private System.Windows.Forms.Label label3;
    }
}

