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
        private List<byte> register;
        private long head;
        private long selectedEntityAdrs;

        public DataBase() {
            InitializeComponent();
            selectedEntityAdrs = -1;
            button1.Enabled = false;
            button2.Enabled = false;
            button5.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button6.Enabled = false;
            button9.Enabled = false;
            button10.Enabled = false;
            button7.Enabled = false;
            button8.Enabled = false;
            data = new List<byte>();
            register = new List<byte>();
        }

        #region New, Open, Exit

        /* Crea un nuevo archivo binario y establece la cabecera en -1. El nombre del
         * archivo es elegido, y el diccionario de datos se guarda con este nombre */
        private void NewFile(object sender, EventArgs e) {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Binary file (*.bin)|*.bin|All files (*.*)|*.*";
            save.InitialDirectory = Application.StartupPath + "\\examples";

            if (save.ShowDialog() == DialogResult.OK) {
                fileName = save.FileName;
                try {
                    // Crea el archivo del diccionario de datos
                    bw = new BinaryWriter(new FileStream(fileName, FileMode.Create));
                    bw.Close();
                }
                catch (IOException ex) {
                    Console.WriteLine(ex.Message + "\n Cannot create file.");
                    return;
                }

                // Inicializa los botones de la interfaz del usuario
                button1.Enabled = true;
                button2.Enabled = true;
                button5.Enabled = true;
                button3.Enabled = false;
                button4.Enabled = false;
                button6.Enabled = false;
                InitializeTables();

                // Añade la cabecera con el valor de -1 y la escribe en el archivo
                data.AddRange(BitConverter.GetBytes((long)(-1)));
                head = -1;
                WriteBinary(fileName);
                textBox1.Text = head.ToString();
            }
        }

        // Abre un archivo binario y lo guarda en "data"
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

                // Lee todos los datos en un byte array y los almacena en una lista de bytes "data"
                data = br.ReadBytes((int)(new FileInfo(open.FileName).Length)).ToList();
                // Obtiene la cabecera
                head = BitConverter.ToInt64(data.ToArray(), 0);
                br.Close();
                InitializeTables();
                UpdateEntityTable();
            }
        }

        // Cierra la aplicación
        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        #endregion
        #region Extras
        /* Inicializa la tabla de entidades y de atributos con sus columnas por defecto */
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

            dataGridView2.Columns.AddRange(new DataGridViewColumn[] { cl1, cl2, cl3, cl4, cl5, cl6, cl7 });
        }

        // Reemplaza n bytes especificados por el índice en el archivo
        private void ReplaceBytes(List<byte> dat, long index, long newData) {
            byte[] newBytes = BitConverter.GetBytes(newData);
            // check
            for (int i = (int)index; i < index + newBytes.Length; i++) {
                dat[i] = newBytes[i - (int)index];
            }
        }

        // Reemplaza n bytes especificados por el índice en el archivo
        private void ReplaceBytes(List<byte> dat, long index, byte[] newData) {
            // check
            for (int i = (int)index; i < index + newData.Length; i++) {
                dat[i] = newData[i - (int)index];
            }
        }

        /* Guarda el archivo en binario cada vez que se agrega una entidad o atributo. 
         Este método guarda todo el arreglo de bytes en el archivo, sin hacer ninguna conversión */
        private void WriteBinary(string name) {
            try {
                bw = new BinaryWriter(new FileStream(name, FileMode.Create));
                bw.Write(data.ToArray());
                bw.Close();
            }
            catch (IOException ex) {
                MessageBox.Show("Error al guardar: " + ex.ToString());
            }
        }

        private void WriteRegBinary(string name) {
            try {
                bw = new BinaryWriter(new FileStream(Application.StartupPath + "\\examples\\" + name + ".dat", FileMode.Create));
                bw.Write(register.ToArray());
                bw.Close();
            }
            catch (IOException ex) {
                MessageBox.Show("Error al guardar: " + ex.ToString());
            }
        }

        private List<byte> ReadRegisterBinary(string name) {
            try {
                br = new BinaryReader(new FileStream(Application.StartupPath + "\\examples\\" + name + ".dat", FileMode.Open));
                register = br.ReadBytes((int)(new FileInfo(Application.StartupPath + "\\examples\\" + name + ".dat").Length)).ToList();
                br.Close();
            }
            catch (IOException ex) {
                Console.WriteLine(ex.Message + "\n Cannot open file.");
            }
            return register;
        }

        #endregion
        #region Entity

        /* Actualiza la tabla de entidades y la cabecera con la lista de bytes, mantiene ordenados
        los datos en la tabla, por su direccion en el archivo*/
        private void UpdateEntityTable() {
            textBox1.Text = head.ToString();
            long aux = head;
            byte[] dataPrint = data.ToArray();
            dataGridView1.Rows.Clear();
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            // Recorre las direcciones de las entidades hasta que la siguiente sea -1 (llegue al final)
            while (aux != -1) {
                /* Obtiene el nombre, la dirección de la entidad, la dirección de los atributos 
                 la dirección del primer dato, y la dirección de la siguiente entidad */
                string name = Encoding.UTF8.GetString(dataPrint, (int)aux, 30);
                name = name.Replace("~", "");
                comboBox1.Items.Add(name);
                comboBox2.Items.Add(name);
                long dirEntity = BitConverter.ToInt64(dataPrint, (int)aux + 30);
                long dirAttrib = BitConverter.ToInt64(dataPrint, (int)aux + 38);
                long dirData = BitConverter.ToInt64(dataPrint, (int)aux + 46);
                long nextEntity = BitConverter.ToInt64(dataPrint, (int)aux + 54);
                dataGridView1.Rows.Add(name, dirEntity, dirAttrib, dirData, nextEntity);
                aux = BitConverter.ToInt64(dataPrint, (int)aux + 54);
            }
            // Ordena las entidades por la dirección en el archivo
            dataGridView1.Sort(dataGridView1.Columns["Entity address"], ListSortDirection.Ascending);
        }

        /* Evento del boton que agrega una entidad en el archivo
         * Las entidades son insertadas ordenadas y primero se verifica que no exista la entidad antes de
         * insertarla. El largo maximo del nombre de la entidad es de 30 caracteres */
        private void btnAddEntity(object sender, EventArgs e) {
            NameDialog dg = new NameDialog(0);
            dg.ShowDialog();
            if (dg.DialogResult == DialogResult.OK) {
                if (AddEntity(dg.name)) {
                    UpdateEntityTable();
                    MessageBox.Show("Entity added", "Success");
                    WriteBinary(fileName);
                }
                else {
                    MessageBox.Show("Error: entity already in dictionary", "Error");
                }
            }
        }


        /* Elimina una entidad del archivo, sólo elimina la referencia, el archivo
        queda del mismo tamaño. Primero verifica que la entidad exista, si existe la elimina */
        private void btnRemoveEntity(object sender, EventArgs e) {
            NameDialog et = new NameDialog(3);
            et.ShowDialog();
            if (et.DialogResult == DialogResult.OK) {
                if (DeleteEntity(et.name)) {
                    UpdateEntityTable();
                    WriteBinary(fileName);
                    MessageBox.Show("Entity deleted", "Success");
                }
                else {
                    MessageBox.Show("Error: entity not found", "Error");
                }
            }
        }

        /* Modifica una entidad. Se reemplazan los datos en la misma dirección del archivo y
         * por lo tanto el tamaño del archivo no se modifica. Primero se verifica que la entidad existe,
         * y después se piden los nuevos datos para reemplazar a los viejos */
        private void btnModifyEntity(object sender, EventArgs e) {
            NameDialog et = new NameDialog(2);
            long index = -1, ant = -1;
            et.ShowDialog();
            if (et.DialogResult == DialogResult.OK) {
                if (SearchEntity(et.name, ref index, ref ant)) {
                    // Se obtiene el nuevo nombre de la entidad
                    NameDialog etn = new NameDialog(0);
                    etn.ShowDialog();
                    if (etn.DialogResult == DialogResult.OK) {
                        if (ModifyEntity(etn.name, ref index)) {
                            MessageBox.Show("Entity modified successfully");
                            WriteBinary(fileName);
                            UpdateEntityTable();
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

        #endregion
        #region Attribute
        /* Se valida que se eliga una entidad del combo box para poder agregar, eliminar o modificar atributos */
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            if (comboBox1.SelectedIndex == -1 || BitConverter.ToInt64(data.ToArray(), (int)selectedEntityAdrs + 46) != -1) {
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

        /* Actualiza la tabla de atributos de la entidad seleccionada. Primero se busca la entidad en el archivo, 
         * después se localiza la dirección de los atributos de esa entidad y se recorren hasta que el siguiente sea -1.
         * Los atributos son ordenados en la tabla por su dirección en el archivo*/
        private void UpdateAttribTable(string entity) {
            long dirAttrib;
            byte[] dataPrint = data.ToArray();
            dataGridView2.Rows.Clear();
            dirAttrib = BitConverter.ToInt64(dataPrint, (int)selectedEntityAdrs + 38);
            while (dirAttrib != -1) {
                /* Se obtiene el nombre del atributo, el tipo, el tamaño, el tipo de indice, la dirección del indice
                 y la dirección del siguiente atributo */
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

        // Actualiza la tabla de atributos eligiendo una entidad del combobox1
        private void comboBox1_TextChanged(object sender, EventArgs e) {
            long eIndex = -1, eAnt = -1;
            SearchEntity(comboBox1.Text, ref eIndex, ref eAnt);
            selectedEntityAdrs = eIndex;
            UpdateAttribTable(comboBox1.Text);

            if (comboBox1.Text != "" && BitConverter.ToInt64(data.ToArray(), (int)selectedEntityAdrs + 46) != -1) {
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

        /* Agrega un atributo a la entidad seleccionada. Primero se verifica que el atributo no exista.
         * Los atributos se agregan secuencialmente al final del archivo */
        private void btnAddAtribute(object sender, EventArgs e) {
            AttributeDialog dg = new AttributeDialog(0);
            dg.ShowDialog();
            if (dg.DialogResult == DialogResult.OK) {
                AddAttribute(dg.name, dg.type, dg.length, dg.indexType);
                UpdateAttribTable(comboBox1.Text);
                MessageBox.Show("Attribute added successfully");
                WriteBinary(fileName);
            }
        }

        /* Modifica un atributo de la entidad seleccionada. Primero se verifica que el atributo exista y
        después se modifica la dirección, por lo tanto el tamaño del archivo no se modifica. */
        private void BtnModifyAttribute(object sender, EventArgs e) {
            NameDialog ed = new NameDialog(4);
            long aIndex = -1, aAnt = -1;
            
            ed.ShowDialog();
            if (ed.DialogResult == DialogResult.OK) {
                if (SearchAttribute(ed.name, ref aIndex, ref aAnt)) {
                    AttributeDialog ad = new AttributeDialog(1);
                    ad.ShowDialog();
                    /* Obtiene los nuevos datos del atributo. Se reemplazan el nombre, el tipo
                     * el tamaño, y el tipo de índice*/
                    if (ad.DialogResult == DialogResult.OK) {
                        // Se modifica en el arreglo de binario
                        ModifyAttribute(aIndex, ad.name, ad.type, ad.length, ad.indexType);
                        WriteBinary(fileName);
                        UpdateAttribTable(comboBox1.Text);
                        MessageBox.Show("Attribute modified successfully");
                    }
                }
                else {
                    MessageBox.Show("Attribute not found");
                }
            }
        }

        /* Elimina la referencia del atributo en el archivo de la entidad seleccionada. El tamaño del
         * archivo no se modifica. Se enlaza el atributo anterior con el siguiente */
        private void btnRemoveAttribute(object sender, EventArgs e) {
            NameDialog ed = new NameDialog(5);
            ed.ShowDialog();
            if (ed.DialogResult == DialogResult.OK) {
                if (DeleteAttribute(ed.name)) {
                    UpdateAttribTable(comboBox1.Text);
                    UpdateEntityTable();
                    MessageBox.Show("Attribute deleted");
                    WriteBinary(fileName);
                }
                else {
                    MessageBox.Show("Attribute not found");
                }
            }
        }

        #endregion
        #region Register

        /* Crea un archivo de datos para la entidad seleccionada. Sólo se puede crear si no existe */
        private void btnCreateRegisterFile(object sender, EventArgs e) {
            try {
                bw = new BinaryWriter(new FileStream(Application.StartupPath + "\\examples\\" + comboBox2.Text + ".dat", FileMode.Create));
                bw.Close();
                button9.Enabled = false;
                button10.Enabled = true;
                button7.Enabled = true;
                button8.Enabled = true;
                label5.Text = comboBox2.Text;
                InitializeRegisterTable();
               
            }
            catch (IOException ex) {
                Console.WriteLine(ex.Message + "\n Cannot create file.");
                return;
            }
            
        }

        /* Valida la entidad y si el archivo de datos está creado. Los botones se activan cuando ya está creado
         * Si no está creado se desactivan los botones */
        private void comboBox2_TextChanged(object sender, EventArgs e) {
            if (comboBox2.Text != "") {
                long eIndex = -1, eAnt = -1;
                SearchEntity(comboBox2.Text, ref eIndex, ref eAnt);
                selectedEntityAdrs = eIndex;
                if (File.Exists(Application.StartupPath + "\\examples\\" + comboBox2.Text + ".dat")) {
                    register = ReadRegisterBinary(comboBox2.Text);
                    button9.Enabled = false;
                    button10.Enabled = true;
                    button7.Enabled = true;
                    button8.Enabled = true;
                    label5.Text = comboBox2.Text;
                    InitializeRegisterTable();
                    // Si la entidad no apunta a -1, entonces tiene elementos :v
                    long aIndex = BitConverter.ToInt64(data.ToArray(), (int)selectedEntityAdrs + 38);
                    if (BitConverter.ToInt64(data.ToArray(), (int)selectedEntityAdrs + 46) != -1) {
                        List<char> types = new List<char>();
                        List<int> sizes = new List<int>();
                        while (aIndex != -1) {
                            //inputs.Add(Encoding.UTF8.GetString(data.ToArray(), (int)aux, 30).Replace("~", ""));
                            types.Add(BitConverter.ToChar(data.ToArray(), (int)aIndex + 38));
                            sizes.Add(BitConverter.ToInt32(data.ToArray(), (int)aIndex + 40));
                            aIndex = BitConverter.ToInt64(data.ToArray(), (int)aIndex + 56);
                        }
                        UpdateRegisterTable(types, sizes);
                    }
                }
                else {
                    dataGridView3.Columns.Clear();
                    button9.Enabled = true;
                    button10.Enabled = false;
                    button7.Enabled = false;
                    button8.Enabled = false;
                    label5.Text = "Register";
                }
            }
        }

        /* Elimina un archivo de registro de la entidad seleccionada si es que éste existe. Se validan los botones */
        private void BtnDeleteRegisterFile(object sender, EventArgs e) {
            try {
                File.Delete(Application.StartupPath + "\\examples\\" + comboBox2.Text + ".dat");
                button9.Enabled = true;
                button10.Enabled = false;
                button7.Enabled = false;
                button8.Enabled = false;
                label5.Text = "Register";
                register.Clear();
                dataGridView3.Columns.Clear();
                // Actualiza la cabecera de los registros de la entidad en -1
                ReplaceBytes(data, selectedEntityAdrs + 46, -1);
            }
            catch (Exception ex) {
                MessageBox.Show("Cannot delete file\n" + ex.ToString());
                throw;
            }
        }

        /* Esta función pone las cabeceras de la tabla de acuerdo al archivo de registros
         de la entidad seleccionada */
        private void InitializeRegisterTable() {
            long aIndex;
            // Obtiene la dirección de los registros de la entidad
            textBox2.Text = BitConverter.ToInt64(data.ToArray(), (int)selectedEntityAdrs + 46).ToString();
            dataGridView3.Columns.Clear();
            // Recorre todos los atributos, y los agrega como columnas
            aIndex = BitConverter.ToInt64(data.ToArray(), (int)selectedEntityAdrs + 38);
            dataGridView3.Columns.Add("Registry address", "Registry address");
            while (aIndex != -1) {
                string name = Encoding.UTF8.GetString(data.ToArray(), (int)aIndex, 30).Replace("~", "");
                aIndex = BitConverter.ToInt64(data.ToArray(), (int)aIndex + 56);
                dataGridView3.Columns.Add(name, name);
            }
            dataGridView3.Columns.Add("Next registry address", "Next registry address");
            
        }
        #endregion

        /* El botón hace que se agrege un registro en la entidad seleccionada. Los registros
         * son agregados secuencialmente */
        private void BtnAddRegister(object sender, EventArgs e) {
            List<string> inputs = new List<string>();
            List<char> types = new List<char>();
            List<int> sizes = new List<int>();
            int size = 0;
            // Obtiene todos los metadatos de los atributos para poder agregar registros
            // Obtiene los nombres, los tipos y la longitud de los atributos
            long aux = BitConverter.ToInt64(data.ToArray(), (int)selectedEntityAdrs + 38);
            while (aux != -1) {
                inputs.Add(Encoding.UTF8.GetString(data.ToArray(), (int)aux, 30).Replace("~", ""));
                types.Add(BitConverter.ToChar(data.ToArray(), (int)aux + 38));
                sizes.Add(BitConverter.ToInt32(data.ToArray(), (int)aux + 40));
                size += sizes.Last();// BitConverter.ToInt32(data.ToArray(), (int)aux + 40);
                aux = BitConverter.ToInt64(data.ToArray(), (int)aux + 56);
            }
            // Muestra el diálogo para pedir cada dato de la entidad
            RegisterDialog rd = new RegisterDialog(inputs);
            if (rd.ShowDialog() == DialogResult.OK) {
                /* Guarda los resultados en una lista de string generica
                 * Los resultados despues son convertidos a su tipo de dato especificado
                 * por el tipo de dato en cada atributo */
                List<string> forms = rd.output;
                // Escribe la entrada en el archivo
                AddEntry(forms, types, sizes, size);
                // Actualiza la tabla de registros
                UpdateRegisterTable(types, sizes);
            }
        }

        private void UpdateRegisterTable(List<char> types, List<int> sizes) {
            // Recorrer los registros desde el primero, ya sea que sea el primero o en el centro
            long aux = BitConverter.ToInt64(data.ToArray(), (int)selectedEntityAdrs + 46);
            
            long integ = 0;
            string name = "";
            byte[] registerArr = register.ToArray();
            List<string> row = new List<string>();
            dataGridView3.Rows.Clear();
            int rowCont = 0;
            while (aux != -1) {
                dataGridView3.Rows.Add();
                long adrs = BitConverter.ToInt16(registerArr, (int)aux);
                dataGridView3.Rows[rowCont].Cells[0].Value = adrs;
                int fixSize = 8;
                for (int i = 0; i < sizes.Count; i++) {
                    if (types[i] == 'C') {
                        name = Encoding.UTF8.GetString(registerArr, fixSize + (int)aux, sizes[i]).Replace("~", "");
                        row.Add(name);
                        dataGridView3.Rows[rowCont].Cells[i + 1].Value = name;
                    }
                    else {
                        switch (sizes[i]) {
                            case 2:
                                integ = BitConverter.ToInt16(registerArr, fixSize + (int)aux);
                                break;
                            case 4:
                                integ = BitConverter.ToInt32(registerArr, fixSize + (int)aux);
                                break;
                            case 8:
                                integ = BitConverter.ToInt64(registerArr, fixSize + (int)aux);
                                break;
                            default:
                                break;
                        }
                        dataGridView3.Rows[rowCont].Cells[i + 1].Value = integ;
                    }
                    fixSize += sizes[i];

                }
                if (aux != -1) {
                    adrs = BitConverter.ToInt64(registerArr, fixSize + (int)aux);
                    dataGridView3.Rows[rowCont].Cells[sizes.Count + 1].Value = adrs;
                }
                else {
                    dataGridView3.Rows[rowCont].Cells[sizes.Count + 1].Value = (long)-1;
                }
                rowCont++;
                aux = BitConverter.ToInt64(register.ToArray(), (int)aux + fixSize);
            }
        }

        private void tabPage1_Click(object sender, EventArgs e) {

        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e) {
            if(tabControl1.TabPages[0].Text == e.TabPage.Text || tabControl1.TabPages[1].Text == e.TabPage.Text) {
                if (comboBox1.Text != "" && BitConverter.ToInt64(data.ToArray(), (int)selectedEntityAdrs + 46) != -1) {
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
            else {
                if (tabControl1.TabPages[0].Text == e.TabPage.Text) {

                }
            }
        }
    }
}
