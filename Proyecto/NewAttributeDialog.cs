using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Proyecto {
    public partial class NewAttributeDialog : Form {
        public string name;
        public char type;
        public int length;
        public int indexType;
        // Modificar para que solo pueda agregar un indice de 1

        public NewAttributeDialog(int type) {
            InitializeComponent();
            textBox1.Select();
            button2.CausesValidation = false;
            switch (type) {
                // New atribute
                case 0:
                    Text = "New attribute";
                    break;
                case 1:
                    Text = "Modify attribute";
                    break;
            }
            if (true) {

            }
        }

        private void button1_Click(object sender, EventArgs e) {
            if (textBox1.Text != "" && comboBox2.SelectedIndex != -1 && numericUpDown1.Value != 0 && comboBox3.SelectedIndex != -1) {

                name = textBox1.Text;
                type = comboBox2.Text[0];
                length = Convert.ToInt32(numericUpDown1.Value);
                indexType = Convert.ToInt32(comboBox3.Text[0]) - 48;
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            Close();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) {
            // Si se elige que sea entero, se limita a solo el valor 4
            if (comboBox2.SelectedIndex == 0) {
                numericUpDown1.Value = 4;
                numericUpDown1.Enabled = false;
            }
            else {
                numericUpDown1.Enabled = true;
            }
        }
    }
}
