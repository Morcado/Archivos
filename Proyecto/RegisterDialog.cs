using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Proyecto {
    public partial class RegisterDialog : Form {
        private int cant;
        public List<string> output;
        public RegisterDialog(List<string> inputs) {
            InitializeComponent();
            cant = inputs.Count;
            int y = 12;
            for (int i = 0; i < cant; i++) {
                TextBox a = new TextBox();
                Label b = new Label();
                b.Text = inputs[i];
                b.Location = new Point(20, y + 3);
                a.Name = i.ToString();
                a.Size = new Size(100, 20);
                a.Location = new Point(145, y);
                Controls.Add(a);
                Controls.Add(b);
                y += 26;
            }
            button1.Location = new Point(168, y);
            button2.Location = new Point(75, y);
            this.Size = new Size(280, y + 26 + 20 + 33);
        }

        private void button2_Click(object sender, EventArgs e) {
            Close();
        }

        private void button1_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.OK;
            output = new List<string>();
            for (int i = 0; i < cant; i++) {
                output.Add(((TextBox)Controls[i.ToString()]).Text);
            }
            Close();
        }

        
    }
}
