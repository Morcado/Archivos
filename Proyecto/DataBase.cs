using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Proyecto {
    public partial class DataBase : Form {
        private BinaryWriter bw;
        private BinaryReader br;
        private string fileName;
        private List<byte> data;
        private long head;
        
        public DataBase() {
            InitializeComponent();
            button1.Enabled = false;
            button2.Enabled = false;
            button5.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button6.Enabled = false;
            data = new List<byte>();
        }

        // Creates a new bin file and set the head to -1
        private void NewFile(object sender, EventArgs e) {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Binary file (*.bin)|*.bin|All files (*.*)|*.*";
            save.InitialDirectory = Application.StartupPath + "\\examples";

            if (save.ShowDialog() == DialogResult.OK) {
                fileName = save.FileName;
                try {
                    bw = new BinaryWriter(new FileStream(fileName, FileMode.Create));
                    bw.Close();
                }
                catch (IOException ex) {
                    Console.WriteLine(ex.Message + "\n Cannot create file.");
                    return;
                }

                button1.Enabled = true;
                button2.Enabled = true;
                button5.Enabled = true;
                button3.Enabled = false;
                button4.Enabled = false;
                button6.Enabled = false;
                InitializeTables();

                // Add head
                data.AddRange(BitConverter.GetBytes((long)(-1)));
                head = -1;
                WriteBinary();
                textBox1.Text = head.ToString();
            }
        }

        // Open a bin file and save it to "data"
        private void OpenFile(object sender, EventArgs e) {
            OpenFileDialog open = new OpenFileDialog();
            open.InitialDirectory = Application.StartupPath + "\\examples";
            open.Filter = "Binary file (*.bin)|*.bin|All files (*.*)|*.*";
            open.DefaultExt = ".bin";

            if (open.ShowDialog() == DialogResult.OK) {
                fileName = open.FileName;
                try {
                    br = new BinaryReader(new FileStream(fileName, FileMode.Open));
                }
                catch (IOException ex) {
                    Console.WriteLine(ex.Message + "\n Cannot open file.");
                    return;
                }
                button1.Enabled = true;
                button2.Enabled = true;
                button5.Enabled = true;

                // Read all data into a byte array and store in "data"
                data = br.ReadBytes((int)(new FileInfo(open.FileName).Length)).ToList();
                head = BitConverter.ToInt64(data.ToArray(), 0);
                br.Close();
                InitializeTables();
                UpdateTable();
            }
        }

        // Updates the table when needed and the head, keep it sorted by address
        private void UpdateTable() {
            textBox1.Text = head.ToString();
            long aux = head;
            byte[] dataPrint = data.ToArray();
            dataGridView1.Rows.Clear();
            comboBox1.Items.Clear();
            while (aux != -1) {
                string name = Encoding.UTF8.GetString(dataPrint, (int)aux, 30);
                name = name.Replace("~", "");
                comboBox1.Items.Add(name);
                long dirEntity = BitConverter.ToInt64(dataPrint, (int)aux + 30);
                long dirAttrib = BitConverter.ToInt64(dataPrint, (int)aux + 38);
                long dirData = BitConverter.ToInt64(dataPrint, (int)aux + 46);
                long nextEntity = BitConverter.ToInt64(dataPrint, (int)aux + 54);
                dataGridView1.Rows.Add(name, dirEntity, dirAttrib, dirData, nextEntity);
                aux = BitConverter.ToInt64(dataPrint, (int)aux + 54);


            }
            dataGridView1.Sort(dataGridView1.Columns["Entity address"], ListSortDirection.Ascending);
        }


        // Initializes the table with main columns
        private void InitializeTables() {
            var col1 = new DataGridViewTextBoxColumn();
            var col2 = new DataGridViewTextBoxColumn();
            var col3 = new DataGridViewTextBoxColumn();
            var col4 = new DataGridViewTextBoxColumn();
            var col5 = new DataGridViewTextBoxColumn();

            col1.HeaderText = "Entity name";
            col1.Name = "Entity name";
            col1.MinimumWidth = 150;
            col1.Width = 150;
            col2.HeaderText = "Entity address";
            col2.Name = "Entity address";
            col2.MinimumWidth = 80;
            col2.Width = 80;
            col2.Resizable = DataGridViewTriState.False;
            col3.HeaderText = "Attribute address";
            col3.Name = "Attribute address";
            col3.MinimumWidth = 80;
            col3.Width = 80;
            col3.Resizable = DataGridViewTriState.False;
            col4.HeaderText = "Data address";
            col4.Name = "Data address";
            col4.MinimumWidth = 80;
            col4.Width = 80;
            col4.Resizable = DataGridViewTriState.False;
            col5.HeaderText = "Next entity address";
            col5.Name = "Next entity address";
            col5.MinimumWidth = 80;
            col5.Width = 80;
            col5.Resizable = DataGridViewTriState.False;

            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { col1, col2, col3, col4, col5 });
            var cl1 = new DataGridViewTextBoxColumn();
            var cl2 = new DataGridViewTextBoxColumn();
            var cl3 = new DataGridViewTextBoxColumn();
            var cl4 = new DataGridViewTextBoxColumn();
            var cl5 = new DataGridViewTextBoxColumn();
            var cl6 = new DataGridViewTextBoxColumn();
            var cl7 = new DataGridViewTextBoxColumn();
            cl1.HeaderText = "Attribute name";
            cl1.Name = "Attribute name";
            cl1.MinimumWidth = 120;
            cl1.Width = 120;
            cl2.HeaderText = "Attribute address";
            cl2.Name = "Attribute address";
            cl2.MinimumWidth = 60;
            cl2.Width = 60;
            cl2.Resizable = DataGridViewTriState.False;
            cl3.HeaderText = "Data type";
            cl3.Name = "Data type";
            cl3.MinimumWidth = 60;
            cl3.Width = 60;
            cl3.Resizable = DataGridViewTriState.False;
            cl4.HeaderText = "Data length";
            cl4.Name = "Data length";
            cl4.MinimumWidth = 60;
            cl4.Width = 60;
            cl4.Resizable = DataGridViewTriState.False;
            cl5.HeaderText = "Index type";
            cl5.Name = "Index type";
            cl5.MinimumWidth = 60;
            cl5.Width = 60;
            cl5.Resizable = DataGridViewTriState.False;
            cl6.HeaderText = "Index address";
            cl6.Name = "Index address";
            cl6.MinimumWidth = 60;
            cl6.Width = 60;
            cl6.Resizable = DataGridViewTriState.False;
            cl7.HeaderText = "Next attribute address";
            cl7.Name = "Index type";
            cl7.MinimumWidth = 60;
            cl7.Width = 60;
            cl7.Resizable = DataGridViewTriState.False;

            dataGridView2.Columns.AddRange(new DataGridViewColumn[] { cl1, cl2, cl3, cl4, cl5, cl6, cl7});
        }

        // Function called every time an entity or an attribute is added to the database
        private void WriteBinary() {
            try {
                bw = new BinaryWriter(new FileStream(fileName, FileMode.Create));
                bw.Write(data.ToArray());
                bw.Close();
            }
            catch (IOException ex) {
                MessageBox.Show("Error al guardar: " + ex.ToString());
            }
        }

        // When user click Add entity, add entity to the current "data"
        private void btnAddEntity(object sender, EventArgs e) {
            NameDialog dg = new NameDialog(0);
            dg.ShowDialog();
            if (dg.DialogResult == DialogResult.OK) {
                if (AddEntity(dg.name)) {
                    UpdateTable();
                    MessageBox.Show("Entity added", "Success");
                    WriteBinary();
                }
                else {
                    MessageBox.Show("Error: entity already in dictionary", "Error");
                }
            }
        }

        // Replace n bytes in data from the expecified index
        private void ReplaceBytes(long index, long newData) {
            byte[] newBytes = BitConverter.GetBytes(newData);
            // check
            for (int i = (int)index; i < index + newBytes.Length; i++) {
                data[i] = newBytes[i - (int)index];
            }
        }

        // Replace n bytes with an array of bytes
        private void ReplaceBytes(long index, byte[] newData) {
            // check
            for (int i = (int)index; i < index + newData.Length; i++) {
                data[i] = newData[i - (int)index];
            }
        }

        // Add attribute to a selected entity
        private void btnAddAtribute(object sender, EventArgs e) {
            AttributeDialog dg = new AttributeDialog(0);
            dg.ShowDialog();
            if (dg.DialogResult == DialogResult.OK) {
                // list of entities
                AddAttribute(dg.name, dg.type, dg.length, dg.indexType);
                UpdateAttribTable(comboBox1.Text);
                MessageBox.Show("Attribute added successfully");
                WriteBinary();
            }
        }

        // Remove entity
        private void btnRemoveEntity(object sender, EventArgs e) {
            NameDialog et = new NameDialog(3);
            et.ShowDialog();
            if (et.DialogResult == DialogResult.OK) {
                if (DeleteEntity(et.name)) {
                    WriteBinary();
                    UpdateTable();
                    MessageBox.Show("Entity deleted", "Success");
                }
                else {
                    MessageBox.Show("Error: entity not found", "Error");
                }
            }
        }

        // Modify entity
        private void btnModifyEntity(object sender, EventArgs e) {
            NameDialog et = new NameDialog(2);
            long index = -1, ant = -1;
            et.ShowDialog();
            if (et.DialogResult == DialogResult.OK) {
                if (SearchEntity(et.name, ref index, ref ant)) {
                    NameDialog etn = new NameDialog(0);
                    etn.ShowDialog();
                    if (etn.DialogResult == DialogResult.OK) {
                        if (ModifyEntity(etn.name, ref index)) {
                            MessageBox.Show("Entity modified successfully");
                            WriteBinary();
                            UpdateTable();
                        }
                        else {
                            MessageBox.Show("Error: entity not modified");
                        }
                    }
                }
                else {
                    MessageBox.Show("Error: entity not found");
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            if (comboBox1.SelectedIndex == -1) {
                button3.Enabled = false;
                button4.Enabled = false;
                button6.Enabled = false;
            }
            else {
                button3.Enabled = true;
                button4.Enabled = true;
                button6.Enabled = true;
            }
        }

        // Shows every attribute for the selected entity
        private void UpdateAttribTable(string entity) {
            long index = -1, ant = -1, dirAttrib;
            byte[] dataPrint = data.ToArray();
            dataGridView2.Rows.Clear();
            SearchEntity(entity, ref index, ref ant);
            // Add attributes of each
            dirAttrib = BitConverter.ToInt64(dataPrint, (int)index + 38);
            while (dirAttrib != -1) {
                string aName = Encoding.UTF8.GetString(dataPrint, (int)dirAttrib, 30).Replace("~", "");
                //dir atrib
                char aType = BitConverter.ToChar(dataPrint, (int)dirAttrib + 38);
                int aLength = BitConverter.ToInt32(dataPrint, (int)dirAttrib + 40);
                int iType = BitConverter.ToInt32(dataPrint, (int)dirAttrib + 44);
                long iAddress = BitConverter.ToInt64(dataPrint, (int)dirAttrib + 48);
                long nextAttribute = BitConverter.ToInt64(dataPrint, (int)dirAttrib + 56);
                dataGridView2.Rows.Add(aName, dirAttrib, aType, aLength, iType, iAddress, nextAttribute);
                dirAttrib = nextAttribute;
            }
            dataGridView2.Sort(dataGridView2.Columns["Attribute address"], ListSortDirection.Ascending);
        }

        private void comboBox1_TextChanged(object sender, EventArgs e) {
            UpdateAttribTable(comboBox1.Text);
        }
        
        // Modify attribute
        private void btnModifyAttribute(object sender, EventArgs e) {
            NameDialog ed = new NameDialog(4);
            long aIndex = -1, eIndex = -1, aAnt = -1, eAnt = -1;
            SearchEntity(comboBox1.Text, ref eIndex, ref eAnt);
            
            ed.ShowDialog();
            // If atttribute exists
            if (ed.DialogResult == DialogResult.OK) {
                if (SearchAttribute(ed.name, ref aIndex, ref aAnt, eIndex)) {
                    AttributeDialog ad = new AttributeDialog(1);
                    ad.ShowDialog();
                    // Get new attribute data
                    if (ad.DialogResult == DialogResult.OK) {
                        // Modify on data
                        ModifyAttribute(aIndex, ad.name, ad.type, ad.length, ad.indexType);
                        WriteBinary();
                        UpdateAttribTable(comboBox1.Text);
                        MessageBox.Show("Attribute modified successfully");
                    }
                }
                else {
                    MessageBox.Show("Attribute not found");
                }
            }
        }

        // Delete attribute
        private void btnRemoveAttribute(object sender, EventArgs e) {
            NameDialog ed = new NameDialog(5);
            ed.ShowDialog();
            if (ed.DialogResult == DialogResult.OK) {
                if (DeleteAttribute(ed.name)) {
                    UpdateAttribTable(comboBox1.Text);
                    UpdateTable();
                    MessageBox.Show("Attribute deleted");
                    WriteBinary();
                }
                else {
                    MessageBox.Show("Attribute not found");
                }
            }
        }
    }
}
