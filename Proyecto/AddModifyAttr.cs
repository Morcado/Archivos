using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Proyecto {
    public partial class AddModifyAttr : Form {
        public AddModifyAttr(int type) {
            InitializeComponent();
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

        private void DialogAdd_Load(object sender, EventArgs e) {
        }
    }
}
