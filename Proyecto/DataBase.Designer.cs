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
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.textBoxEnt = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonEnt2 = new System.Windows.Forms.Button();
            this.buttonEnt3 = new System.Windows.Forms.Button();
            this.buttonEnt1 = new System.Windows.Forms.Button();
            this.entityTable = new System.Windows.Forms.DataGridView();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.attributeTable = new System.Windows.Forms.DataGridView();
            this.buttonAtt2 = new System.Windows.Forms.Button();
            this.buttonAtt3 = new System.Windows.Forms.Button();
            this.buttonAtt1 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxAtt = new System.Windows.Forms.ComboBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxReg = new System.Windows.Forms.TextBox();
            this.buttonReg4 = new System.Windows.Forms.Button();
            this.buttonReg3 = new System.Windows.Forms.Button();
            this.buttonReg2 = new System.Windows.Forms.Button();
            this.buttonReg1 = new System.Windows.Forms.Button();
            this.registerTable = new System.Windows.Forms.DataGridView();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBoxReg = new System.Windows.Forms.ComboBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.entityTable)).BeginInit();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.attributeTable)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.registerTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(732, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.NewFile);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenFile);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(100, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(0, 27);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(732, 411);
            this.tabControl1.TabIndex = 4;
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
            this.tabPage2.Size = new System.Drawing.Size(724, 385);
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
            this.entityTable.Size = new System.Drawing.Size(523, 323);
            this.entityTable.TabIndex = 3;
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
            this.tabPage1.Size = new System.Drawing.Size(724, 385);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Attribute";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // attributeTable
            // 
            this.attributeTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.attributeTable.Location = new System.Drawing.Point(176, 38);
            this.attributeTable.Name = "attributeTable";
            this.attributeTable.Size = new System.Drawing.Size(523, 325);
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
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.label7);
            this.tabPage3.Controls.Add(this.textBoxReg);
            this.tabPage3.Controls.Add(this.buttonReg4);
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
            this.tabPage3.Size = new System.Drawing.Size(724, 385);
            this.tabPage3.TabIndex = 3;
            this.tabPage3.Text = "Register";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(21, 187);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(63, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "First register";
            // 
            // textBoxReg
            // 
            this.textBoxReg.Location = new System.Drawing.Point(100, 184);
            this.textBoxReg.Name = "textBoxReg";
            this.textBoxReg.ReadOnly = true;
            this.textBoxReg.Size = new System.Drawing.Size(51, 20);
            this.textBoxReg.TabIndex = 12;
            this.textBoxReg.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // buttonReg4
            // 
            this.buttonReg4.Location = new System.Drawing.Point(21, 152);
            this.buttonReg4.Name = "buttonReg4";
            this.buttonReg4.Size = new System.Drawing.Size(130, 23);
            this.buttonReg4.TabIndex = 11;
            this.buttonReg4.Text = "Delete entry";
            this.buttonReg4.UseVisualStyleBackColor = true;
            // 
            // buttonReg3
            // 
            this.buttonReg3.Location = new System.Drawing.Point(21, 123);
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
            this.registerTable.Size = new System.Drawing.Size(523, 325);
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
            // tabPage4
            // 
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(724, 385);
            this.tabPage4.TabIndex = 4;
            this.tabPage4.Text = "Index";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.Icon = ((System.Drawing.Icon)(resources.GetObject("errorProvider1.Icon")));
            // 
            // DataBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(732, 437);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "DataBase";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.entityTable)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.attributeTable)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.registerTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button buttonEnt3;
        private System.Windows.Forms.Button buttonEnt1;
        private System.Windows.Forms.DataGridView entityTable;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.DataGridView attributeTable;
        private System.Windows.Forms.Button buttonAtt3;
        private System.Windows.Forms.Button buttonAtt1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxAtt;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Button buttonAtt2;
        private System.Windows.Forms.Button buttonEnt2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxEnt;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.DataGridView registerTable;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBoxReg;
        private System.Windows.Forms.Button buttonReg3;
        private System.Windows.Forms.Button buttonReg1;
        private System.Windows.Forms.Button buttonReg4;
        private System.Windows.Forms.Button buttonReg2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxReg;
        private System.Windows.Forms.TabPage tabPage4;
    }
}

