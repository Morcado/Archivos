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
        private int modify;
        public List<string> output;
        /* Ubica cada text box para pedir los datos de todos los atributos de la entidad*/
        public RegisterDialog(List<string> inputs, int keyToDelete, int modify) {
            int controlPos = keyToDelete == -1 ? 0 : keyToDelete;
            InitializeComponent();
            this.modify = modify;
            switch (modify) {
                case 0:
                    Text = "Add register";
                    break;
                case 1:
                    Text = "Modify register";
                    break;
                case 2:
                    Text = "Delete register";
                    break;
                default:
                    break;
            }
            cant = inputs.Count;
            int y = 12;
            for (int i = 0; i < cant; i++) {
                TextBox a = new TextBox();
                Label b = new Label();
                b.Text = inputs[i];
                b.Location = new Point(20, y + 3);
                a.Name = i.ToString();
                a.Location = new Point(145, y);
                if (modify == 0 || modify == 1) {
                    Controls.Add(a);
                    Controls.Add(b);
                    y += 26;
                }
                else {
                    if (controlPos == i) {
                        //a.Location = new Point(145, y);
                        //b.Location = new Point(20, y + 3);
                        a.Name = "key";
                        Controls.Add(a);
                        Controls.Add(b);
                        y += 26;
                    }
                }
            }
            button1.Location = new Point(168, y);
            button2.Location = new Point(75, y);
            Size = new Size(280, y + 26 + 20 + 33);
        }

        private void button1_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.OK;
            output = new List<string>();
            if (modify == 0 || modify == 1) {
                for (int i = 0; i < cant; i++) {
                    output.Add(((TextBox)Controls[i.ToString()]).Text);
                }
            }
            else {
                output.Add(((TextBox)Controls["key"]).Text);
            }
            Close();
        }   

        private void button2_Click(object sender, EventArgs e) => Close();
    }
}
