﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Proyecto {
    public partial class AddModifyEntity : Form {
        public string name;
        public AddModifyEntity(int type) {
            
            InitializeComponent();
            switch (type) {
                // New atribute
                case 0:
                    this.Text = "New entity";
                    break;
                case 1:
                    this.Text = "Modify entity";
                    break;
                case 3:
                    this.Text = "New file";
                    this.label2.Text = "Name: ";
                    break;
            }
            button2.CausesValidation = false;
        }

        private void button1_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.OK;
            name = textBox1.Text;
            Close();
        }

        private void textBox1_Validating(object sender, CancelEventArgs e) {
            if (textBox1.Text.Trim() == "") {
                errorProvider1.SetError(textBox1, "Name is required");
                e.Cancel = true;
                return;
            }
            // Name is Valid
            errorProvider1.SetError(textBox1, "");
        }
    }
}
