using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Proyecto {
    public partial class AttributeDialog : Form {
        public string name;
        public char type;
        public int length;
        public int indexType;

        // Modify index type so you can only add 1 and only one search index

        public AttributeDialog(int type) {
            InitializeComponent();
            textBox1.Select();
            button2.CausesValidation = false;
            switch (type) {
                // New atribute
                case 0:
                    this.Text = "New attribute";
                    break;
                case 1:
                    this.Text = "Modify attribute";
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            name = textBox1.Text;
            type = comboBox2.Text[0];
            length = Convert.ToInt32(numericUpDown1.Value);
            indexType = Convert.ToInt32(comboBox3.Text[0]) - 48;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
