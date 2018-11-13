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
        private bool allAttributes;
        public List<string> output;
        /* Ubica cada text box para pedir los datos de todos los atributos de la entidad*/
        public RegisterDialog(List<string> inputs, int keyToDelete, bool allAttributes, bool searchKey, string text) {
            int controlPos = keyToDelete == -1 ? 0 : keyToDelete;
            InitializeComponent();
            this.allAttributes = allAttributes;
            Text = text;

            cant = inputs.Count;
            int y = 12;

            if (allAttributes) {
                for (int i = 0; i < cant; i++) {
                    TextBox a = new TextBox {
                        Name = i.ToString(),
                        Location = new Point(145, y)
                    };
                    Label b = new Label {
                        Text = inputs[i],
                        Location = new Point(20, y + 3)
                    };
                    Controls.Add(a);
                    Controls.Add(b);
                    y += 26;
                }
            }
            else {
                TextBox a = new TextBox {
                    Name = "key",
                    Location = new Point(145, y)
                };
                Label b = new Label {
                    Location = new Point(20, y + 3)
                };
                b.Text = searchKey ? inputs[keyToDelete] : "Register address";
                Controls.Add(a);
                Controls.Add(b);
                y += 26;
            }

            button1.Location = new Point(168, y);
            button2.Location = new Point(75, y);
            Size = new Size(280, y + 26 + 20 + 33);
        }

        private void Button1_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.OK;
            output = new List<string>();
            if (allAttributes) {
                for (int i = 0; i < cant; i++) {
                    output.Add(((TextBox)Controls[i.ToString()]).Text);
                }
            }
            else {
                output.Add(((TextBox)Controls["key"]).Text);
            }
            Close();
        }   

        private void Button2_Click(object sender, EventArgs e) => Close();
    }
}
