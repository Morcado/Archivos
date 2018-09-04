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
        private List<byte> data;
        private long lastEntityAddress;

        public DataBase() {
            InitializeComponent();
            button1.Enabled = false;
            button2.Enabled = false;
            button5.Enabled = false;
            data = new List<byte>();
        }

        private void NewFile(object sender, EventArgs e) {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Binary file (*.bin)|*.bin|All files (*.*)|*.*";
            save.InitialDirectory = Application.StartupPath + "\\example";

            if (save.ShowDialog() == DialogResult.OK) {

                try {
                    bw = new BinaryWriter(new FileStream(save.FileName, FileMode.Create));
                }
                catch (IOException ex) {
                    Console.WriteLine(ex.Message + "\n Cannot create file.");
                    return;
                }

                button1.Enabled = true;
                button2.Enabled = true;
                button5.Enabled = true;

                #region Columns in DataGrid
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
                #endregion

                data.AddRange(BitConverter.GetBytes((long)8));
            }
        }

        private void OpenFile(object sender, EventArgs e) {
            OpenFileDialog open = new OpenFileDialog();
            open.InitialDirectory = Application.StartupPath + "\\example";
            open.Filter = "Binary file (*.bin)|*.bin|All files (*.*)|*.*";
            open.DefaultExt = ".bin";

            if (open.ShowDialog() == DialogResult.OK) {
                try {
                    br = new BinaryReader(new FileStream(open.FileName, FileMode.Open));
                }
                catch (IOException ex) {
                    Console.WriteLine(ex.Message + "\n Cannot open file.");
                    return;
                }
            }
        }

        private void SaveFile(object sender, EventArgs e) {

        }

        private void btnAddEntity(object sender, EventArgs e) {
            AddModifyEntity dg = new AddModifyEntity(0);
            dg.ShowDialog();
            if (dg.DialogResult == DialogResult.OK) {
                InsertEntity(dg.name);
            }
        }

        // Add new entity on current data, as it is new, it will add to the end of file
        private void InsertEntity(string name) {
            //List<byte> block = new List<byte>();
            byte[] byteName = Encoding.UTF8.GetBytes(name);
            long address = data.Count;
            
            // add name
            if (byteName.Length < 30) {
                data.AddRange(byteName);
                for (int i = byteName.Length; i < 30; i++) {
                    data.Add(Convert.ToByte('~'));
                } 
            }

            // add entity address
            data.AddRange(BitConverter.GetBytes((long)address));
            // add attribute address
            data.AddRange(BitConverter.GetBytes((long)-1));
            // add data address
            data.AddRange(BitConverter.GetBytes((long)-1));
            // add next entity address
            data.AddRange(BitConverter.GetBytes((long)-1));

            long a = GetHeader();
            lastEntityAddress = data.Count;
            dataGridView1.Rows.Add(name, address, -1, -1, -1);
        }

        private long GetHeader() {
            return BitConverter.ToInt64(data.ToArray(), 0);
        }

        #region Attributes

        private void btnAddAtribute(object sender, EventArgs e) {
            AddModifyAttr dg = new AddModifyAttr(0);
            dg.ShowDialog();
        }

        #endregion

  
    }
}
