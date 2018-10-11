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
    public struct SearchKey {
        public int size;
        public int pos;
        public int attribIndex;
        public bool isChar;
    }

    public partial class DataBase : Form {
        private BinaryWriter bw;
        private BinaryReader br;
        private string dictionary;
        private List<byte> data;
        private List<byte> register;
        private long head;
        private long selectedEntityAdrs;
        private int registerSize;
        private SearchKey key;

        public DataBase() {
            InitializeComponent();
            registerSize = 0;
            key.pos = -1;
            selectedEntityAdrs = -1;
            buttonEnt1.Enabled = false;
            buttonEnt3.Enabled = false;
            buttonAtt1.Enabled = false;
            buttonAtt3.Enabled = false;
            buttonEnt2.Enabled = false;
            buttonAtt2.Enabled = false;
            buttonReg3.Enabled = false;
            buttonReg4.Enabled = false;
            buttonReg1.Enabled = false;
            buttonReg2.Enabled = false;
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
                dictionary = save.FileName;
                try {
                    // Crea el archivo del diccionario de datos
                    bw = new BinaryWriter(new FileStream(dictionary, FileMode.Create));
                    bw.Close();
                }
                catch (IOException ex) {
                    Console.WriteLine(ex.Message + "\n Cannot create file.");
                    return;
                }

                // Inicializa los botones de la interfaz del usuario
                buttonEnt1.Enabled = true;
                buttonEnt3.Enabled = true;
                buttonEnt2.Enabled = true;
                buttonAtt1.Enabled = false;
                buttonAtt3.Enabled = false;
                buttonAtt2.Enabled = false;
                entityTable.Columns.Clear();
                attributeTable.Columns.Clear();
                registerTable.Columns.Clear();
                InitializeTables();
                data.Clear();
                // Añade la cabecera con el valor de -1 y la escribe en el archivo
                data.AddRange(BitConverter.GetBytes((long)(-1)));
                head = -1;
                WriteBinary(dictionary);
                textBoxEnt.Text = head.ToString();
            }
        }

        // Abre un archivo binario y lo guarda en "data"
        private void OpenFile(object sender, EventArgs e) {
            OpenFileDialog open = new OpenFileDialog();
            open.InitialDirectory = Application.StartupPath + "\\examples";
            open.Filter = "Binary file (*.bin)|*.bin|All files (*.*)|*.*";
            open.DefaultExt = ".bin";

            if (open.ShowDialog() == DialogResult.OK) {
                dictionary = open.FileName;
                try {
                    br = new BinaryReader(new FileStream(dictionary, FileMode.Open));
                }
                catch (IOException ex) {
                    Console.WriteLine(ex.Message + "\n Cannot open file.");
                    return;
                }
                buttonEnt1.Enabled = true;
                buttonEnt3.Enabled = true;
                buttonEnt2.Enabled = true;

                // Lee todos los datos en un byte array y los almacena en una lista de bytes "data"
                data = br.ReadBytes((int)(new FileInfo(open.FileName).Length)).ToList();
                // Obtiene la cabecera
                head = BitConverter.ToInt64(data.ToArray(), 0);
                br.Close();
                entityTable.Columns.Clear();
                attributeTable.Columns.Clear();
                registerTable.Columns.Clear();
                InitializeTables();
                UpdateEntityTable();
            }
        }

        // Cierra la aplicación
        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        #endregion
        #region Binary File Operations
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

            entityTable.Columns.AddRange(new DataGridViewColumn[] { col1, col2, col3, col4, col5 });
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

            attributeTable.Columns.AddRange(new DataGridViewColumn[] { cl1, cl2, cl3, cl4, cl5, cl6, cl7 });
        }

        // Reemplaza n bytes especificados por el índice en el archivo, recibe un long
        private void ReplaceBytes(List<byte> dat, long index, long newData) {
            byte[] newBytes = BitConverter.GetBytes(newData);
            // check
            for (int i = (int)index; i < index + newBytes.Length; i++) {
                dat[i] = newBytes[i - (int)index];
            }
        }

        // Reemplaza n bytes especificados por el índice en el archivo, recibe un byte[]
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
        #region User Validations

        // Actualiza la tabla de atributos eligiendo una entidad del combobox1
        private void comboBoxAtt_TextChanged(object sender, EventArgs e) {
            long eIndex = -1, eAnt = -1;
            SearchEntity(comboBoxAtt.Text, ref eIndex, ref eAnt);
            selectedEntityAdrs = eIndex;

            if (comboBoxAtt.Text != "") {
                UpdateAttribTable(comboBoxAtt.Text);
                buttonAtt1.Enabled = true;
                buttonAtt2.Enabled = true;
                buttonAtt3.Enabled = true;
            }
            else {
                buttonAtt1.Enabled = false;
                buttonAtt2.Enabled = false;
                buttonAtt3.Enabled = false;
            }
        }
        /* Se hace validación en la pagina de los atributos
         * Valida la entidad y si el archivo de datos está creado. Los botones se activan cuando ya está creado
         * Si no está creado se desactivan los botones*/
        private void comboBoxReg_TextChanged(object sender, EventArgs e) {
            long eIndex = -1, eAnt = -1;
            SearchEntity(comboBoxReg.Text, ref eIndex, ref eAnt);
            selectedEntityAdrs = eIndex;

            if (File.Exists(Application.StartupPath + "\\examples\\" + comboBoxReg.Text + ".dat")) {
                register = ReadRegisterBinary(comboBoxReg.Text);
                buttonReg1.Enabled = false;
                buttonReg2.Enabled = true;
                buttonReg3.Enabled = true;
                buttonReg4.Enabled = true;
                label5.Text = comboBoxReg.Text;
                InitializeRegisterTable();
                // Si la entidad no apunta a -1, entonces tiene elementos :v
                long aIndex = BitConverter.ToInt64(data.ToArray(), (int)selectedEntityAdrs + 38);
                if (BitConverter.ToInt64(data.ToArray(), (int)selectedEntityAdrs + 46) != -1) {
                    List<char> types = new List<char>();
                    List<int> sizes = new List<int>();
                    int i = 0, auxSearchKey = 0;
                    registerSize = 0;
                    while (aIndex != -1) {
                        char aType = BitConverter.ToChar(data.ToArray(), (int)aIndex + 38);
                        int aLength = BitConverter.ToInt32(data.ToArray(), (int)aIndex + 40);
                        int iType = BitConverter.ToInt32(data.ToArray(), (int)aIndex + 44);

                        types.Add(aType);
                        sizes.Add(aLength);

                        // Obtener la clave de busqueda si existe
                        if (iType != 1) {
                            auxSearchKey += aLength;
                        }
                        else { 
                            key.pos = auxSearchKey;
                            key.size = aLength;
                            key.attribIndex = i;
                            key.isChar = aType == 'C' ? true : false;
                        }

                        registerSize += aLength;
                        i++;
                        aIndex = BitConverter.ToInt64(data.ToArray(), (int)aIndex + 56);
                    }
                    UpdateRegisterTable(types, sizes);
                }
            }
            else {
                registerTable.Columns.Clear();
                buttonReg1.Enabled = true;
                buttonReg2.Enabled = false;
                buttonReg3.Enabled = false;
                buttonReg4.Enabled = false;
                label5.Text = "Register";
            }
        }

        #endregion
        #region Entity

        /* Actualiza la tabla de entidades y la cabecera con la lista de bytes, mantiene ordenados
        los datos en la tabla, por su direccion en el archivo*/
        private void UpdateEntityTable() {
            textBoxEnt.Text = head.ToString();
            long aux = head;
            byte[] dataPrint = data.ToArray();

            entityTable.Rows.Clear();
            // Recorre las direcciones de las entidades hasta que la siguiente sea -1 (llegue al final)
            while (aux != -1) {
                /* Obtiene el nombre, la dirección de la entidad, la dirección de los atributos 
                 la dirección del primer dato, y la dirección de la siguiente entidad */
                string name = Encoding.UTF8.GetString(dataPrint, (int)aux, 30).Replace("~", "");
                comboBoxAtt.Items.Add(name);
                comboBoxReg.Items.Add(name);
                long dirEntity = BitConverter.ToInt64(dataPrint, (int)aux + 30);
                long dirAttrib = BitConverter.ToInt64(dataPrint, (int)aux + 38);
                long dirData = BitConverter.ToInt64(dataPrint, (int)aux + 46);
                long nextEntity = BitConverter.ToInt64(dataPrint, (int)aux + 54);
                entityTable.Rows.Add(name, dirEntity, dirAttrib, dirData, nextEntity);
                aux = BitConverter.ToInt64(dataPrint, (int)aux + 54);
            }
            // Ordena las entidades por la dirección en el archivo
            entityTable.Sort(entityTable.Columns["Entity address"], ListSortDirection.Ascending);
        }

        /* Evento del boton que agrega una entidad en el archivo
         * Las entidades son insertadas ordenadas y primero se verifica que no exista la entidad antes de
         * insertarla. El largo maximo del nombre de la entidad es de 30 caracteres */
        private void btnAddEntity(object sender, EventArgs e) {
            NewEntityDialog dg = new NewEntityDialog(0);
            dg.ShowDialog();
            if (dg.DialogResult == DialogResult.OK) {
                if (AddEntity(dg.name)) {
                    UpdateEntityTable();
                    MessageBox.Show("Entity added", "Success");
                    WriteBinary(dictionary);
                }
                else {
                    MessageBox.Show("Error: entity already in dictionary", "Error");
                }
            }
        }

        /* Elimina una entidad del archivo, sólo elimina la referencia, el archivo
        queda del mismo tamaño. Primero verifica que la entidad exista, si existe la elimina */
        private void btnRemoveEntity(object sender, EventArgs e) {
            NewEntityDialog et = new NewEntityDialog(3);
            et.ShowDialog();
            if (et.DialogResult == DialogResult.OK) {
                if (DeleteEntity(et.name)) {
                    UpdateEntityTable();
                    WriteBinary(dictionary);
                    comboBoxAtt.Items.Remove(et.name);
                    comboBoxReg.Items.Remove(et.name);
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
            NewEntityDialog et = new NewEntityDialog(2);
            long index = -1, ant = -1;

            et.ShowDialog();
            if (et.DialogResult == DialogResult.OK) {
                if (SearchEntity(et.name, ref index, ref ant)) {
                    // Se obtiene el nuevo nombre de la entidad
                    NewEntityDialog etn = new NewEntityDialog(0);
                    etn.ShowDialog();
                    if (etn.DialogResult == DialogResult.OK) {
                        if (ModifyEntity(etn.name, ref index)) {
                            MessageBox.Show("Entity modified successfully");
                            WriteBinary(dictionary);
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
        
        /* Actualiza la tabla de atributos de la entidad seleccionada. Primero se busca la entidad en el archivo, 
         * después se localiza la dirección de los atributos de esa entidad y se recorren hasta que el siguiente sea -1.
         * Los atributos son ordenados en la tabla por su dirección en el archivo*/
        private void UpdateAttribTable(string entity) {
            byte[] dataPrint = data.ToArray();
            long dirAttrib = BitConverter.ToInt64(dataPrint, (int)selectedEntityAdrs + 38);
            
            attributeTable.Rows.Clear();
            while (dirAttrib != -1) {
                /* Se obtiene el nombre del atributo, el tipo, el tamaño, el tipo de indice, la dirección del indice
                 y la dirección del siguiente atributo */
                string aName = Encoding.UTF8.GetString(dataPrint, (int)dirAttrib, 30).Replace("~", "");
                char aType = BitConverter.ToChar(dataPrint, (int)dirAttrib + 38);
                int aLength = BitConverter.ToInt32(dataPrint, (int)dirAttrib + 40);
                int iType = BitConverter.ToInt32(dataPrint, (int)dirAttrib + 44);
                long iAddress = BitConverter.ToInt64(dataPrint, (int)dirAttrib + 48);
                long nextAttribute = BitConverter.ToInt64(dataPrint, (int)dirAttrib + 56);
                attributeTable.Rows.Add(aName, dirAttrib, aType, aLength, iType, iAddress, nextAttribute);
                dirAttrib = nextAttribute;
            }
            attributeTable.Sort(attributeTable.Columns["Attribute address"], ListSortDirection.Ascending);
        }

        /* Agrega un atributo a la entidad seleccionada. Primero se verifica que el atributo no exista.
         * Los atributos se agregan secuencialmente al final del archivo */
        private void btnAddAtribute(object sender, EventArgs e) {
            NewAttributeDialog dg = new NewAttributeDialog(0);
            dg.ShowDialog();
            if (dg.DialogResult == DialogResult.OK) {
                AddAttribute(dg.name, dg.type, dg.length, dg.indexType);
                //AddAttribute(dg.attribute);
                UpdateAttribTable(comboBoxAtt.Text);
                MessageBox.Show("Attribute added successfully");
                WriteBinary(dictionary);
            }
        }

        /* Modifica un atributo de la entidad seleccionada. Primero se verifica que el atributo exista y
        después se modifica la dirección, por lo tanto el tamaño del archivo no se modifica. */
        private void btnModifyAttribute(object sender, EventArgs e) {
            NewEntityDialog ed = new NewEntityDialog(4);
            long aIndex = -1, aAnt = -1;
            
            ed.ShowDialog();
            if (ed.DialogResult == DialogResult.OK) {
                if (SearchAttribute(ed.name, ref aIndex, ref aAnt)) {
                    NewAttributeDialog ad = new NewAttributeDialog(1);
                    ad.ShowDialog();
                    /* Obtiene los nuevos datos del atributo. Se reemplazan el nombre, el tipo
                     * el tamaño, y el tipo de índice*/
                    if (ad.DialogResult == DialogResult.OK) {
                        // Se modifica en el arreglo de binario
                        ModifyAttribute(aIndex, ad.name, ad.type, ad.length, ad.indexType);
                        WriteBinary(dictionary);
                        UpdateAttribTable(comboBoxAtt.Text);
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
            NewEntityDialog ed = new NewEntityDialog(5);
            ed.ShowDialog();
            if (ed.DialogResult == DialogResult.OK) {
                if (DeleteAttribute(ed.name)) {
                    UpdateAttribTable(comboBoxAtt.Text);
                    UpdateEntityTable();
                    MessageBox.Show("Attribute deleted");
                    WriteBinary(dictionary);
                }
                else {
                    MessageBox.Show("Attribute not found");
                }
            }
        }

        #endregion
        #region Register

        /* Esta función pone las cabeceras de la tabla de acuerdo al archivo de registros
         * de la entidad seleccionada */
        private void InitializeRegisterTable() {
            long aIndex;
            // Obtiene la dirección de los registros de la entidad
            textBoxReg.Text = BitConverter.ToInt64(data.ToArray(), (int)selectedEntityAdrs + 46).ToString();
            registerTable.Columns.Clear();
            // Recorre todos los atributos, y los agrega como columnas
            aIndex = BitConverter.ToInt64(data.ToArray(), (int)selectedEntityAdrs + 38);
            registerTable.Columns.Add("Registry address", "Registry address");
            while (aIndex != -1) {
                string name = Encoding.UTF8.GetString(data.ToArray(), (int)aIndex, 30).Replace("~", "");
                aIndex = BitConverter.ToInt64(data.ToArray(), (int)aIndex + 56);
                registerTable.Columns.Add(name, name);
            }
            registerTable.Columns.Add("Next registry address", "Next registry address");

        }

        /* Crea un archivo de datos para la entidad seleccionada. Sólo se puede crear si no existe */
        private void btnCreateRegisterFile(object sender, EventArgs e) {
            try {
                bw = new BinaryWriter(new FileStream(Application.StartupPath + "\\examples\\" + comboBoxReg.Text + ".dat", FileMode.Create));
                bw.Close();
                buttonReg1.Enabled = false;
                buttonReg2.Enabled = true;
                buttonReg3.Enabled = true;
                buttonReg4.Enabled = true;
                label5.Text = comboBoxReg.Text;
                InitializeRegisterTable();
               
            }
            catch (IOException ex) {
                Console.WriteLine(ex.Message + "\n Cannot create file.");
                return;
            }
        }
        
        /* Elimina un archivo de registro de la entidad seleccionada si es que éste existe. Se validan los botones */
        private void BtnDeleteRegisterFile(object sender, EventArgs e) {
            try {
                File.Delete(Application.StartupPath + "\\examples\\" + comboBoxReg.Text + ".dat");
                buttonReg1.Enabled = true;
                buttonReg2.Enabled = false;
                buttonReg3.Enabled = false;
                buttonReg4.Enabled = false;
                label5.Text = "Register";
                register.Clear();
                registerTable.Columns.Clear();
                // Actualiza la cabecera de los registros de la entidad en -1
                ReplaceBytes(data, selectedEntityAdrs + 46, -1);
                WriteBinary(dictionary);
                UpdateEntityTable();
            }
            catch (Exception ex) {
                MessageBox.Show("Cannot delete file\n" + ex.ToString());
                throw;
            }
        }

        /* El botón hace que se agrege un registro en la entidad seleccionada. Los registros
         * son agregados secuencialmente */
        private void BtnAddRegister(object sender, EventArgs e) {
            List<string> inputs = new List<string>();
            List<char> types = new List<char>();
            List<int> sizes = new List<int>();
            byte[] dataP = data.ToArray();
            // Obtiene todos los metadatos de los atributos para poder agregar registros
            // Obtiene los nombres, los tipos y la longitud de los atributos
            long aux = BitConverter.ToInt64(data.ToArray(), (int)selectedEntityAdrs + 38);
            while (aux != -1) {
                inputs.Add(Encoding.UTF8.GetString(dataP, (int)aux, 30).Replace("~", ""));
                types.Add(BitConverter.ToChar(dataP, (int)aux + 38));
                sizes.Add(BitConverter.ToInt32(dataP, (int)aux + 40));
                aux = BitConverter.ToInt64(dataP, (int)aux + 56);
            }
            /* Muestra el diálogo para pedir cada dato de la entidad
             * El dialogo obtiene el nombre de la clave de busqueda de algun registro si esta existe */
            RegisterDialog rd = new RegisterDialog(inputs);
            if (rd.ShowDialog() == DialogResult.OK) {
                /* Guarda los resultados en una lista de string generica
                 * Los resultados despues son convertidos a su tipo de dato especificado
                 * por el tipo de dato en cada atributo */
                List<string> forms = rd.output;
                // Escribe la entrada en el archivo
                if (key.attribIndex != -1) {
                    AddOrderedEntry(forms, types, sizes);
                    WriteBinary(dictionary);
                    UpdateEntityTable();
                }
                else {
                    AddSecuentialEntry(forms, types, sizes);
                }
                // Actualiza la tabla de registros
                UpdateRegisterTable(types, sizes);
                WriteRegBinary(comboBoxReg.Text);
            }
        }

        private void UpdateRegisterTable(List<char> types, List<int> sizes) {
            // Recorrer los registros desde el primero, ya sea que sea el primero o en el centro
            long aux = BitConverter.ToInt64(data.ToArray(), (int)selectedEntityAdrs + 46);
            textBoxReg.Text = aux.ToString();
            long integ = 0;
            string name = "";
            byte[] registerArr = register.ToArray();
            List<string> row = new List<string>();
            registerTable.Rows.Clear();
            int rowCont = 0;
            while (aux != -1 && register.Count > 0) {
                registerTable.Rows.Add();
                long adrs = BitConverter.ToInt64(registerArr, (int)aux);
                registerTable.Rows[rowCont].Cells[0].Value = adrs;
                int fixSize = 8;
                for (int i = 0; i < sizes.Count; i++) {
                    if (types[i] == 'C') {
                        name = Encoding.UTF8.GetString(registerArr, fixSize + (int)aux, sizes[i]).Replace("~", "");
                        row.Add(name);
                        registerTable.Rows[rowCont].Cells[i + 1].Value = name;
                    }
                    else {
                        integ = BitConverter.ToInt32(registerArr, fixSize + (int)aux);
                        registerTable.Rows[rowCont].Cells[i + 1].Value = integ;
                    }
                    fixSize += sizes[i];

                }
                if (aux != -1) {
                    adrs = BitConverter.ToInt64(registerArr, fixSize + (int)aux);
                    registerTable.Rows[rowCont].Cells[sizes.Count + 1].Value = adrs;
                }
                else {
                    registerTable.Rows[rowCont].Cells[sizes.Count + 1].Value = (long)-1;
                }
                rowCont++;
                aux = BitConverter.ToInt64(register.ToArray(), (int)aux + fixSize);
            }            
        }
        #endregion
    }
}
