using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Proyecto {
    public partial class NameDialog : Form {
        public string name;
        public NameDialog(int type) {
            
            InitializeComponent();
            switch (type) {
                // New atribute
                case 0:
                    this.Text = "New entity";
                    label2.Text = "New entity name:";
                    break;
                case 2:
                    this.Text = "Modify entity";
                    break;
                case 3:
                    this.Text = "Delete entity";
                    break;
                case 4:
                    this.Text = "Modify attribute";
                    label2.Text = "Attribute name";
                    break;
                case 5:
                    this.Text = "Delete attribute";
                    label2.Text = "Attribute name";
                    break;
            }
            button2.CausesValidation = false;
        }

        private void button1_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.OK;
            name = textBox1.Text;
            Close();
        }


    }
}
