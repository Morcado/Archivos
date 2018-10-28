namespace Proyecto {
    partial class DataBase {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataBase));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openCSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.comboBoxFK = new System.Windows.Forms.ComboBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.secondFKTable = new System.Windows.Forms.DataGridView();
            this.mainFKTable = new System.Windows.Forms.DataGridView();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.comboBoxPK = new System.Windows.Forms.ComboBox();
            this.buttonNextPage = new System.Windows.Forms.Button();
            this.buttonPrevPage = new System.Windows.Forms.Button();
            this.secondPKTable = new System.Windows.Forms.DataGridView();
            this.mainPKTable = new System.Windows.Forms.DataGridView();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.buttonReg4 = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxReg = new System.Windows.Forms.TextBox();
            this.buttonReg5 = new System.Windows.Forms.Button();
            this.buttonReg3 = new System.Windows.Forms.Button();
            this.buttonReg2 = new System.Windows.Forms.Button();
            this.buttonReg1 = new System.Windows.Forms.Button();
            this.registerTable = new System.Windows.Forms.DataGridView();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBoxReg = new System.Windows.Forms.ComboBox();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.attributeTable = new System.Windows.Forms.DataGridView();
            this.buttonAtt2 = new System.Windows.Forms.Button();
            this.buttonAtt3 = new System.Windows.Forms.Button();
            this.buttonAtt1 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxAtt = new System.Windows.Forms.ComboBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.textBoxEnt = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonEnt2 = new System.Windows.Forms.Button();
            this.buttonEnt3 = new System.Windows.Forms.Button();
            this.buttonEnt1 = new System.Windows.Forms.Button();
            this.entityTable = new System.Windows.Forms.DataGridView();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.pageNumber = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.secondFKTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mainFKTable)).BeginInit();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.secondPKTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mainPKTable)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.registerTable)).BeginInit();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.attributeTable)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.entityTable)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.openCSVToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.NewFile);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenFile);
            // 
            // openCSVToolStripMenuItem
            // 
            this.openCSVToolStripMenuItem.Name = "openCSVToolStripMenuItem";
            this.openCSVToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openCSVToolStripMenuItem.Text = "Open CSV";
            this.openCSVToolStripMenuItem.Click += new System.EventHandler(this.OpenCSV);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.Icon = ((System.Drawing.Icon)(resources.GetObject("errorProvider1.Icon")));
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.label12);
            this.tabPage5.Controls.Add(this.label13);
            this.tabPage5.Controls.Add(this.comboBoxFK);
            this.tabPage5.Controls.Add(this.button3);
            this.tabPage5.Controls.Add(this.button4);
            this.tabPage5.Controls.Add(this.secondFKTable);
            this.tabPage5.Controls.Add(this.mainFKTable);
            this.tabPage5.Controls.Add(this.label14);
            this.tabPage5.Controls.Add(this.label15);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(792, 385);
            this.tabPage5.TabIndex = 5;
            this.tabPage5.Text = "Foreign Key";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(18, 77);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(71, 13);
            this.label12.TabIndex = 21;
            this.label12.Text = "Change page";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(18, 22);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(65, 13);
            this.label13.TabIndex = 22;
            this.label13.Text = "Select entity";
            // 
            // comboBoxFK
            // 
            this.comboBoxFK.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFK.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboBoxFK.FormattingEnabled = true;
            this.comboBoxFK.Location = new System.Drawing.Point(21, 38);
            this.comboBoxFK.Name = "comboBoxFK";
            this.comboBoxFK.Size = new System.Drawing.Size(130, 21);
            this.comboBoxFK.TabIndex = 20;
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold);
            this.button3.Location = new System.Drawing.Point(92, 93);
            this.button3.Margin = new System.Windows.Forms.Padding(0);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(59, 21);
            this.button3.TabIndex = 18;
            this.button3.Text = "→";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold);
            this.button4.Location = new System.Drawing.Point(21, 93);
            this.button4.Margin = new System.Windows.Forms.Padding(0);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(59, 21);
            this.button4.TabIndex = 19;
            this.button4.Text = "←";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // secondFKTable
            // 
            this.secondFKTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.secondFKTable.Location = new System.Drawing.Point(388, 38);
            this.secondFKTable.Name = "secondFKTable";
            this.secondFKTable.Size = new System.Drawing.Size(273, 325);
            this.secondFKTable.TabIndex = 16;
            // 
            // mainFKTable
            // 
            this.mainFKTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.mainFKTable.Location = new System.Drawing.Point(176, 38);
            this.mainFKTable.Name = "mainFKTable";
            this.mainFKTable.Size = new System.Drawing.Size(185, 325);
            this.mainFKTable.TabIndex = 17;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(385, 22);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(32, 13);
            this.label14.TabIndex = 14;
            this.label14.Text = "Page";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(173, 22);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(62, 13);
            this.label15.TabIndex = 15;
            this.label15.Text = "Foreign key";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.pageNumber);
            this.tabPage4.Controls.Add(this.label11);
            this.tabPage4.Controls.Add(this.label10);
            this.tabPage4.Controls.Add(this.comboBoxPK);
            this.tabPage4.Controls.Add(this.buttonNextPage);
            this.tabPage4.Controls.Add(this.buttonPrevPage);
            this.tabPage4.Controls.Add(this.secondPKTable);
            this.tabPage4.Controls.Add(this.mainPKTable);
            this.tabPage4.Controls.Add(this.label9);
            this.tabPage4.Controls.Add(this.label8);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(792, 385);
            this.tabPage4.TabIndex = 4;
            this.tabPage4.Text = "Primary Key";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(18, 77);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(71, 13);
            this.label11.TabIndex = 13;
            this.label11.Text = "Change page";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(18, 22);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(65, 13);
            this.label10.TabIndex = 13;
            this.label10.Text = "Select entity";
            // 
            // comboBoxPK
            // 
            this.comboBoxPK.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPK.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboBoxPK.FormattingEnabled = true;
            this.comboBoxPK.Location = new System.Drawing.Point(21, 38);
            this.comboBoxPK.Name = "comboBoxPK";
            this.comboBoxPK.Size = new System.Drawing.Size(130, 21);
            this.comboBoxPK.TabIndex = 12;
            this.comboBoxPK.TextChanged += new System.EventHandler(this.comboBoxPK_TextChanged);
            // 
            // buttonNextPage
            // 
            this.buttonNextPage.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold);
            this.buttonNextPage.Location = new System.Drawing.Point(92, 93);
            this.buttonNextPage.Margin = new System.Windows.Forms.Padding(0);
            this.buttonNextPage.Name = "buttonNextPage";
            this.buttonNextPage.Size = new System.Drawing.Size(59, 21);
            this.buttonNextPage.TabIndex = 11;
            this.buttonNextPage.Text = "→";
            this.buttonNextPage.UseVisualStyleBackColor = true;
            this.buttonNextPage.Click += new System.EventHandler(this.nextPKPage);
            // 
            // buttonPrevPage
            // 
            this.buttonPrevPage.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold);
            this.buttonPrevPage.Location = new System.Drawing.Point(21, 93);
            this.buttonPrevPage.Margin = new System.Windows.Forms.Padding(0);
            this.buttonPrevPage.Name = "buttonPrevPage";
            this.buttonPrevPage.Size = new System.Drawing.Size(59, 21);
            this.buttonPrevPage.TabIndex = 11;
            this.buttonPrevPage.Text = "←";
            this.buttonPrevPage.UseVisualStyleBackColor = true;
            this.buttonPrevPage.Click += new System.EventHandler(this.prevPKPage);
            // 
            // secondPKTable
            // 
            this.secondPKTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.secondPKTable.Location = new System.Drawing.Point(388, 38);
            this.secondPKTable.Name = "secondPKTable";
            this.secondPKTable.Size = new System.Drawing.Size(273, 325);
            this.secondPKTable.TabIndex = 10;
            // 
            // mainPKTable
            // 
            this.mainPKTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.mainPKTable.Location = new System.Drawing.Point(176, 38);
            this.mainPKTable.Name = "mainPKTable";
            this.mainPKTable.Size = new System.Drawing.Size(185, 325);
            this.mainPKTable.TabIndex = 10;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(385, 22);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(32, 13);
            this.label9.TabIndex = 9;
            this.label9.Text = "Page";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(173, 22);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(61, 13);
            this.label8.TabIndex = 9;
            this.label8.Text = "Primary key";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.buttonReg4);
            this.tabPage3.Controls.Add(this.label7);
            this.tabPage3.Controls.Add(this.textBoxReg);
            this.tabPage3.Controls.Add(this.buttonReg5);
            this.tabPage3.Controls.Add(this.buttonReg3);
            this.tabPage3.Controls.Add(this.buttonReg2);
            this.tabPage3.Controls.Add(this.buttonReg1);
            this.tabPage3.Controls.Add(this.registerTable);
            this.tabPage3.Controls.Add(this.label5);
            this.tabPage3.Controls.Add(this.label6);
            this.tabPage3.Controls.Add(this.comboBoxReg);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(792, 385);
            this.tabPage3.TabIndex = 3;
            this.tabPage3.Text = "Register";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // buttonReg4
            // 
            this.buttonReg4.Location = new System.Drawing.Point(21, 178);
            this.buttonReg4.Name = "buttonReg4";
            this.buttonReg4.Size = new System.Drawing.Size(130, 23);
            this.buttonReg4.TabIndex = 14;
            this.buttonReg4.Text = "Modify register";
            this.buttonReg4.UseVisualStyleBackColor = true;
            this.buttonReg4.Click += new System.EventHandler(this.BtnModifyRegister);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(20, 126);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(63, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "First register";
            // 
            // textBoxReg
            // 
            this.textBoxReg.Location = new System.Drawing.Point(100, 123);
            this.textBoxReg.Name = "textBoxReg";
            this.textBoxReg.ReadOnly = true;
            this.textBoxReg.Size = new System.Drawing.Size(51, 20);
            this.textBoxReg.TabIndex = 12;
            this.textBoxReg.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // buttonReg5
            // 
            this.buttonReg5.Location = new System.Drawing.Point(21, 207);
            this.buttonReg5.Name = "buttonReg5";
            this.buttonReg5.Size = new System.Drawing.Size(130, 23);
            this.buttonReg5.TabIndex = 11;
            this.buttonReg5.Text = "Delete register";
            this.buttonReg5.UseVisualStyleBackColor = true;
            this.buttonReg5.Click += new System.EventHandler(this.BtnDeleteRegister);
            // 
            // buttonReg3
            // 
            this.buttonReg3.Location = new System.Drawing.Point(21, 149);
            this.buttonReg3.Name = "buttonReg3";
            this.buttonReg3.Size = new System.Drawing.Size(130, 23);
            this.buttonReg3.TabIndex = 11;
            this.buttonReg3.Text = "Insert register";
            this.buttonReg3.UseVisualStyleBackColor = true;
            this.buttonReg3.Click += new System.EventHandler(this.BtnAddRegister);
            // 
            // buttonReg2
            // 
            this.buttonReg2.Location = new System.Drawing.Point(21, 94);
            this.buttonReg2.Name = "buttonReg2";
            this.buttonReg2.Size = new System.Drawing.Size(130, 23);
            this.buttonReg2.TabIndex = 9;
            this.buttonReg2.Text = "Delete Register File";
            this.buttonReg2.UseVisualStyleBackColor = true;
            this.buttonReg2.Click += new System.EventHandler(this.BtnDeleteRegisterFile);
            // 
            // buttonReg1
            // 
            this.buttonReg1.Location = new System.Drawing.Point(21, 65);
            this.buttonReg1.Name = "buttonReg1";
            this.buttonReg1.Size = new System.Drawing.Size(130, 23);
            this.buttonReg1.TabIndex = 9;
            this.buttonReg1.Text = "Create Register File";
            this.buttonReg1.UseVisualStyleBackColor = true;
            this.buttonReg1.Click += new System.EventHandler(this.btnCreateRegisterFile);
            // 
            // registerTable
            // 
            this.registerTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.registerTable.Location = new System.Drawing.Point(176, 38);
            this.registerTable.Name = "registerTable";
            this.registerTable.Size = new System.Drawing.Size(593, 325);
            this.registerTable.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(173, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Register";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(18, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Select entity";
            // 
            // comboBoxReg
            // 
            this.comboBoxReg.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxReg.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboBoxReg.FormattingEnabled = true;
            this.comboBoxReg.Location = new System.Drawing.Point(21, 38);
            this.comboBoxReg.Name = "comboBoxReg";
            this.comboBoxReg.Size = new System.Drawing.Size(130, 21);
            this.comboBoxReg.TabIndex = 5;
            this.comboBoxReg.TextChanged += new System.EventHandler(this.comboBoxReg_TextChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.attributeTable);
            this.tabPage1.Controls.Add(this.buttonAtt2);
            this.tabPage1.Controls.Add(this.buttonAtt3);
            this.tabPage1.Controls.Add(this.buttonAtt1);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.comboBoxAtt);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(792, 385);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Attribute";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // attributeTable
            // 
            this.attributeTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.attributeTable.Location = new System.Drawing.Point(176, 38);
            this.attributeTable.Name = "attributeTable";
            this.attributeTable.Size = new System.Drawing.Size(589, 325);
            this.attributeTable.TabIndex = 4;
            // 
            // buttonAtt2
            // 
            this.buttonAtt2.Location = new System.Drawing.Point(21, 94);
            this.buttonAtt2.Name = "buttonAtt2";
            this.buttonAtt2.Size = new System.Drawing.Size(130, 23);
            this.buttonAtt2.TabIndex = 3;
            this.buttonAtt2.Text = "Modify attribute";
            this.buttonAtt2.UseVisualStyleBackColor = true;
            this.buttonAtt2.Click += new System.EventHandler(this.btnModifyAttribute);
            // 
            // buttonAtt3
            // 
            this.buttonAtt3.Location = new System.Drawing.Point(21, 123);
            this.buttonAtt3.Name = "buttonAtt3";
            this.buttonAtt3.Size = new System.Drawing.Size(130, 23);
            this.buttonAtt3.TabIndex = 2;
            this.buttonAtt3.Text = "Delete attribute";
            this.buttonAtt3.UseVisualStyleBackColor = true;
            this.buttonAtt3.Click += new System.EventHandler(this.btnRemoveAttribute);
            // 
            // buttonAtt1
            // 
            this.buttonAtt1.Location = new System.Drawing.Point(21, 65);
            this.buttonAtt1.Name = "buttonAtt1";
            this.buttonAtt1.Size = new System.Drawing.Size(130, 23);
            this.buttonAtt1.TabIndex = 1;
            this.buttonAtt1.Text = "Add attribute";
            this.buttonAtt1.UseVisualStyleBackColor = true;
            this.buttonAtt1.Click += new System.EventHandler(this.btnAddAtribute);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(173, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Select entity";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select entity";
            // 
            // comboBoxAtt
            // 
            this.comboBoxAtt.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxAtt.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboBoxAtt.FormattingEnabled = true;
            this.comboBoxAtt.Location = new System.Drawing.Point(21, 38);
            this.comboBoxAtt.Name = "comboBoxAtt";
            this.comboBoxAtt.Size = new System.Drawing.Size(130, 21);
            this.comboBoxAtt.TabIndex = 0;
            this.comboBoxAtt.TextChanged += new System.EventHandler(this.comboBoxAtt_TextChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.textBoxEnt);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.buttonEnt2);
            this.tabPage2.Controls.Add(this.buttonEnt3);
            this.tabPage2.Controls.Add(this.buttonEnt1);
            this.tabPage2.Controls.Add(this.entityTable);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(792, 385);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Entity";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // textBoxEnt
            // 
            this.textBoxEnt.Location = new System.Drawing.Point(58, 130);
            this.textBoxEnt.Name = "textBoxEnt";
            this.textBoxEnt.ReadOnly = true;
            this.textBoxEnt.Size = new System.Drawing.Size(94, 20);
            this.textBoxEnt.TabIndex = 4;
            this.textBoxEnt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 133);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Head";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(173, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Entities";
            // 
            // buttonEnt2
            // 
            this.buttonEnt2.Location = new System.Drawing.Point(22, 67);
            this.buttonEnt2.Name = "buttonEnt2";
            this.buttonEnt2.Size = new System.Drawing.Size(130, 23);
            this.buttonEnt2.TabIndex = 1;
            this.buttonEnt2.Text = "Modify entity";
            this.buttonEnt2.UseVisualStyleBackColor = true;
            this.buttonEnt2.Click += new System.EventHandler(this.btnModifyEntity);
            // 
            // buttonEnt3
            // 
            this.buttonEnt3.Location = new System.Drawing.Point(22, 96);
            this.buttonEnt3.Name = "buttonEnt3";
            this.buttonEnt3.Size = new System.Drawing.Size(130, 23);
            this.buttonEnt3.TabIndex = 2;
            this.buttonEnt3.Text = "Remove entity";
            this.buttonEnt3.UseVisualStyleBackColor = true;
            this.buttonEnt3.Click += new System.EventHandler(this.btnRemoveEntity);
            // 
            // buttonEnt1
            // 
            this.buttonEnt1.Location = new System.Drawing.Point(22, 38);
            this.buttonEnt1.Name = "buttonEnt1";
            this.buttonEnt1.Size = new System.Drawing.Size(130, 23);
            this.buttonEnt1.TabIndex = 0;
            this.buttonEnt1.Text = "Add entity";
            this.buttonEnt1.UseVisualStyleBackColor = true;
            this.buttonEnt1.Click += new System.EventHandler(this.btnAddEntity);
            // 
            // entityTable
            // 
            this.entityTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.entityTable.Location = new System.Drawing.Point(176, 38);
            this.entityTable.Name = "entityTable";
            this.entityTable.Size = new System.Drawing.Size(593, 323);
            this.entityTable.TabIndex = 3;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Location = new System.Drawing.Point(0, 27);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(800, 411);
            this.tabControl1.TabIndex = 4;
            // 
            // pageNumber
            // 
            this.pageNumber.AutoSize = true;
            this.pageNumber.Location = new System.Drawing.Point(414, 22);
            this.pageNumber.Name = "pageNumber";
            this.pageNumber.Size = new System.Drawing.Size(13, 13);
            this.pageNumber.TabIndex = 14;
            this.pageNumber.Text = "1";
            // 
            // DataBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 437);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "DataBase";
            this.Text = "Form1";
            this.Resize += new System.EventHandler(this.DataBase_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.secondFKTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mainFKTable)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.secondPKTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mainPKTable)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.registerTable)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.attributeTable)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.entityTable)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openCSVToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox textBoxEnt;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonEnt2;
        private System.Windows.Forms.Button buttonEnt3;
        private System.Windows.Forms.Button buttonEnt1;
        private System.Windows.Forms.DataGridView entityTable;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.DataGridView attributeTable;
        private System.Windows.Forms.Button buttonAtt2;
        private System.Windows.Forms.Button buttonAtt3;
        private System.Windows.Forms.Button buttonAtt1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxAtt;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button buttonReg4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxReg;
        private System.Windows.Forms.Button buttonReg5;
        private System.Windows.Forms.Button buttonReg3;
        private System.Windows.Forms.Button buttonReg2;
        private System.Windows.Forms.Button buttonReg1;
        private System.Windows.Forms.DataGridView registerTable;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBoxReg;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox comboBoxPK;
        private System.Windows.Forms.Button buttonNextPage;
        private System.Windows.Forms.Button buttonPrevPage;
        private System.Windows.Forms.DataGridView secondPKTable;
        private System.Windows.Forms.DataGridView mainPKTable;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox comboBoxFK;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.DataGridView secondFKTable;
        private System.Windows.Forms.DataGridView mainFKTable;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label pageNumber;
    }
}

