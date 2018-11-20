using System;
using System.Windows.Forms;

namespace Proyecto {
	public partial class NewEntityDialog : Form {
		public string name;

		public NewEntityDialog(int type) {
			InitializeComponent();
			switch (type) {
				// New atribute
				case 0:
					Text = "New entity";
					label2.Text = "New entity name:";
					break;
				case 2:
					Text = "Modify entity";
					break;
				case 3:
					Text = "Delete entity";
					break;
				case 4:
					Text = "Modify attribute";
					label2.Text = "Attribute name";
					break;
				case 5:
					Text = "Delete attribute";
					label2.Text = "Attribute name";
					break;
			}
			button2.CausesValidation = false;
		}

		private void Button1_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.OK;
			name = textBox1.Text;
			Close();
		}
	}
}
