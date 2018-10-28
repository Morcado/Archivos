using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Proyecto {
    public struct IndexFlag {
        public int searchKeySize;
        public int searchKeyPos;

        public bool searchKey;
        public int searchKeyAttribIndex;
        public bool searchKeyIsChar;

        public bool PK;
        public int PKAtribListIndex;
        public bool PKIsChar;
        public int PKSize;
        public long attribPKIndexAdrs;

        public bool FK;
        public int FKAtribListIndex;
        public bool FKIsChar;
        public int FKSize;
        public long attribFKIndexAdrs;
    }

    public partial class DataBase : Form {
        private BinaryWriter bw;
        private BinaryReader br;
        private string dictionaryName;
        private List<byte> data;
        private List<byte> register;
        private List<byte> index;
        private long head;
        private long selectedEntityAdrs;
        private int registerSize;
        private IndexFlag key;
        private int page = 1;

        public DataBase() {
            InitializeComponent();
            registerSize = 0;

            key.searchKeyPos = -1;
            key.searchKey = false;
            key.PK = false;
            key.FK = false;
            buttonPrevPage.Enabled = false;

            selectedEntityAdrs = -1;
            buttonEnt1.Enabled = false;
            buttonEnt2.Enabled = false;
            buttonEnt3.Enabled = false;
            buttonAtt1.Enabled = false;
            buttonAtt2.Enabled = false;
            buttonAtt3.Enabled = false;
            buttonReg1.Enabled = false;
            buttonReg2.Enabled = false;
            buttonReg3.Enabled = false;
            buttonReg4.Enabled = false;
            buttonReg5.Enabled = false;
            data = new List<byte>();
            register = new List<byte>();
            index = new List<byte>();
        }

        #region New, Open, Exit

        /* Crea un nuevo archivo binario y establece la cabecera en -1. El nombre del
         * archivo es elegido, y el diccionario de datos se guarda con este nombre */
        private void NewFile(object sender, EventArgs e) {
            SaveFileDialog save = new SaveFileDialog {
                Filter = "Binary file (*.bin)|*.bin|All files (*.*)|*.*",
                InitialDirectory = Application.StartupPath + "\\examples"
            };

            if (save.ShowDialog() == DialogResult.OK) {
                dictionaryName = save.FileName;

                try {
                    // Crea el archivo del diccionario de datos
                    bw = new BinaryWriter(new FileStream(save.FileName, FileMode.Create));
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
                WriteDictionary();
                textBoxEnt.Text = head.ToString();
            }
        }

        // Abre un archivo binario y lo guarda en "data"
        private void OpenFile(object sender, EventArgs e) {
            OpenFileDialog open = new OpenFileDialog {
                InitialDirectory = Application.StartupPath + "\\examples",
                Filter = "Binary file (*.bin)|*.bin|All files (*.*)|*.*",
                DefaultExt = ".bin"
            };

            if (open.ShowDialog() == DialogResult.OK) {
                dictionaryName = open.FileName;
                try {
                    br = new BinaryReader(new FileStream(open.FileName, FileMode.Open));
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

        private void OpenCSV(object sender, EventArgs e) {
            MessageBox.Show("In progress...");
            OpenFileDialog open = new OpenFileDialog();
            open.InitialDirectory = Application.StartupPath + "\\examples";
            open.Filter = "CSV (*.csv)|*.csv|All files (*.*)|*.*";
            open.DefaultExt = ".bin";

            //if (open.ShowDialog() == DialogResult.OK) {
            //   var Lines = File.ReadLines(open.FileName).Select(a => a.Split(';'));
                //var CSV = from line in Lines select (line.Split(',')).ToArray();
            //}
        }

        // Cierra la aplicación
        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        #endregion
        #region Binary File Operations
        /* Inicializa la tabla de entidades y de atributos con sus columnas por defecto */
        private void InitializeTables() {

            DataGridViewTextBoxColumn col1 = new DataGridViewTextBoxColumn {
                HeaderText = "Entity name",
                Name = "Entity name",
                MinimumWidth = 150,
                Width = 150,
                Resizable = DataGridViewTriState.False
            };
            DataGridViewTextBoxColumn col2 = new DataGridViewTextBoxColumn {
                HeaderText = "Entity address",
                Name = "Entity address",
                MinimumWidth = 80,
                Width = 80,
                Resizable = DataGridViewTriState.False
            };
            DataGridViewTextBoxColumn col3 = new DataGridViewTextBoxColumn {
                HeaderText = "Attribute address",
                Name = "Attribute address",
                MinimumWidth = 80,
                Width = 80,
                Resizable = DataGridViewTriState.False
            };
            DataGridViewTextBoxColumn col4 = new DataGridViewTextBoxColumn {
                HeaderText = "Data address",
                Name = "Data address",
                MinimumWidth = 80,
                Width = 80,
                Resizable = DataGridViewTriState.False
            };
            DataGridViewTextBoxColumn col5 = new DataGridViewTextBoxColumn {
                HeaderText = "Next entity address",
                Name = "Next entity address",
                MinimumWidth = 80,
                Width = 80,
                Resizable = DataGridViewTriState.False
            };

            entityTable.Columns.AddRange(new DataGridViewColumn[] { col1, col2, col3, col4, col5 });
            DataGridViewTextBoxColumn cl1 = new DataGridViewTextBoxColumn {
                HeaderText = "Attribute name",
                Name = "Attribute name",
                MinimumWidth = 120,
                Width = 120,
                Resizable = DataGridViewTriState.False
            };
            DataGridViewTextBoxColumn cl2 = new DataGridViewTextBoxColumn {
                HeaderText = "Attribute address",
                Name = "Attribute address",
                MinimumWidth = 60,
                Width = 60,
                Resizable = DataGridViewTriState.False
            };
            DataGridViewTextBoxColumn cl3 = new DataGridViewTextBoxColumn {
                HeaderText = "Data type",
                Name = "Data type",
                MinimumWidth = 60,
                Width = 60,
                Resizable = DataGridViewTriState.False
            };
            DataGridViewTextBoxColumn cl4 = new DataGridViewTextBoxColumn {
                HeaderText = "Data length",
                Name = "Data length",
                MinimumWidth = 60,
                Width = 60,
                Resizable = DataGridViewTriState.False
            };
            DataGridViewTextBoxColumn cl5 = new DataGridViewTextBoxColumn {
                HeaderText = "Index type",
                Name = "Index type",
                MinimumWidth = 60,
                Width = 60,
                Resizable = DataGridViewTriState.False
            };
            DataGridViewTextBoxColumn cl6 = new DataGridViewTextBoxColumn {
                HeaderText = "Index address",
                Name = "Index address",
                MinimumWidth = 60,
                Width = 60,
                Resizable = DataGridViewTriState.False
            };
            DataGridViewTextBoxColumn cl7 = new DataGridViewTextBoxColumn {
                HeaderText = "Next attribute address",
                Name = "Index type",
                MinimumWidth = 60,
                Width = 60,
                Resizable = DataGridViewTriState.False
            };

            attributeTable.Columns.AddRange(new DataGridViewColumn[] { cl1, cl2, cl3, cl4, cl5, cl6, cl7 });
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
        private void WriteDictionary() {
            try {
                bw = new BinaryWriter(new FileStream(dictionaryName, FileMode.Create));
                bw.Write(data.ToArray());
                bw.Close();
            }
            catch (IOException ex) {
                MessageBox.Show("Error al guardar: " + ex.ToString());
            }
        }

        private void WriteRegisterFile(string name) {
            try {
                bw = new BinaryWriter(new FileStream(Application.StartupPath + "\\examples\\" + name + ".dat", FileMode.Create));
                bw.Write(register.ToArray());
                bw.Close();
            }
            catch (IOException ex) {
                MessageBox.Show("Error al guardar: " + ex.ToString());
            }
        }

        private void WriteIndexFile(string name) {
            try {
                bw = new BinaryWriter(new FileStream(Application.StartupPath + "\\examples\\" + name + ".idx", FileMode.Create));
                bw.Write(index.ToArray());
                bw.Close();
            }
            catch (IOException ex) {
                MessageBox.Show("Error al guardar: " + ex.ToString());
            }
        }

        private void ReadRegisterFile(string name) {
            try {
                br = new BinaryReader(new FileStream(Application.StartupPath + "\\examples\\" + name + ".dat", FileMode.Open));
                register = br.ReadBytes((int)(new FileInfo(Application.StartupPath + "\\examples\\" + name + ".dat").Length)).ToList();
                br.Close();
            }
            catch (IOException ex) {
                Console.WriteLine(ex.Message + "\n Cannot open file.");
            }
        }

        private void ReadIndexFile(string name) {
            try {
                br = new BinaryReader(new FileStream(Application.StartupPath + "\\examples\\" + name + ".idx", FileMode.Open));
                index = br.ReadBytes((int)(new FileInfo(Application.StartupPath + "\\examples\\" + name + ".idx").Length)).ToList();
                br.Close();
            }
            catch (IOException ex) {
                Console.WriteLine(ex.Message + "\n Cannot open file.");
            }
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
                ReadRegisterFile(comboBoxReg.Text);
                buttonReg1.Enabled = false;
                buttonReg2.Enabled = true;
                buttonReg3.Enabled = true;
                buttonReg4.Enabled = true;
                buttonReg5.Enabled = true;
                label5.Text = comboBoxReg.Text;
                InitializeRegisterTable();
                // Si la entidad no apunta a -1, entonces tiene elementos :v
                long aIndex = BitConverter.ToInt64(data.ToArray(), (int)selectedEntityAdrs + 38);

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

                    switch (iType) {
                        case 1:
                            key.searchKey = true;
                            key.searchKeyPos = auxSearchKey;
                            key.searchKeySize = aLength;
                            key.searchKeyAttribIndex = i;
                            key.searchKeyIsChar = aType == 'C' ? true : false;
                            break;
                        case 2:
                            key.PK = true;
                            key.PKAtribListIndex = i;
                            key.PKIsChar = aType == 'C' ? true : false;
                            key.PKSize = aLength;
                            key.attribPKIndexAdrs = aIndex;
                            break;
                        case 3:
                            key.FK = true;
                            key.FKAtribListIndex = i;
                            key.FKIsChar = aType == 'C' ? true : false;
                            key.FKSize = aLength;
                            key.attribFKIndexAdrs = aIndex;
                            break;
                        default:
                            break;
                    }
                    registerSize += aLength;
                    i++;
                    aIndex = BitConverter.ToInt64(data.ToArray(), (int)aIndex + 56);
                }
                UpdateRegisterTable(types, sizes);
                if (File.Exists(Application.StartupPath + "\\examples\\" + comboBoxReg.Text + ".dat")) {
                    ReadIndexFile(comboBoxReg.Text);
                }
            }
            else {
                registerTable.Columns.Clear();
                buttonReg1.Enabled = true;
                buttonReg2.Enabled = false;
                buttonReg3.Enabled = false;
                buttonReg4.Enabled = false;
                buttonReg5.Enabled = false;
                label5.Text = "Register";
            }

            if (key.FK) {
                InitializeFKIndexTable();
            }
            if (key.PK) {
                InitializePKIndexTable();
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
                comboBoxPK.Items.Add(name);
                comboBoxFK.Items.Add(name);
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
                    WriteDictionary();
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
                    WriteDictionary();
                    comboBoxAtt.Items.Remove(et.name);
                    comboBoxReg.Items.Remove(et.name);
                    comboBoxPK.Items.Add(et.name);
                    comboBoxFK.Items.Add(et.name);
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
                            WriteDictionary();
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
                WriteDictionary();
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
                        WriteDictionary();
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
                    WriteDictionary();
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
            var cl1 = new DataGridViewTextBoxColumn {
                HeaderText = "Register address",
                Name = "Register address",
                Width = 60,
                Resizable = DataGridViewTriState.False
            };
            registerTable.Columns.Add(cl1);
            while (aIndex != -1) {
                string name = Encoding.UTF8.GetString(data.ToArray(), (int)aIndex, 30).Replace("~", "");
                aIndex = BitConverter.ToInt64(data.ToArray(), (int)aIndex + 56);
                registerTable.Columns.Add(name, name);
            }
            var cl2 = new DataGridViewTextBoxColumn {
                Resizable = DataGridViewTriState.False,
                HeaderText = "Next register address",
                Name = "Next register address",
                Width = 60
            };
            registerTable.Columns.Add(cl2);
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

        /* Crea un archivo de datos para la entidad seleccionada. Sólo se puede crear si no existe */
        private void btnCreateRegisterFile(object sender, EventArgs e) {
            try {
                bw = new BinaryWriter(new FileStream(Application.StartupPath + "\\examples\\" + comboBoxReg.Text + ".dat", FileMode.Create));
                bw.Close();
                buttonReg1.Enabled = false;
                buttonReg2.Enabled = true;
                buttonReg3.Enabled = true;
                buttonReg5.Enabled = true;
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
                File.Delete(Application.StartupPath + "\\examples\\" + comboBoxReg.Text + ".idx");
                buttonReg1.Enabled = true;
                buttonReg2.Enabled = false;
                buttonReg3.Enabled = false;
                buttonReg5.Enabled = false;
                label5.Text = "Register";
                register.Clear();
                index.Clear();
                registerTable.Columns.Clear();
                // Actualiza la cabecera de los registros de la entidad en -1
                ReplaceBytes(data, selectedEntityAdrs + 46, BitConverter.GetBytes((long)-1));
                WriteDictionary();
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
            /* Obtiene todos los metadatos de los atributos para poder agregar registros
             * Obtiene los nombres, los tipos y la longitud de los atributos */
            
            long aux = BitConverter.ToInt64(data.ToArray(), (int)selectedEntityAdrs + 38);
            while (aux != -1) {
                inputs.Add(Encoding.UTF8.GetString(dataP, (int)aux, 30).Replace("~", ""));
                types.Add(BitConverter.ToChar(dataP, (int)aux + 38));
                sizes.Add(BitConverter.ToInt32(dataP, (int)aux + 40));
                aux = BitConverter.ToInt64(dataP, (int)aux + 56);
            }
            /* Muestra el diálogo para pedir cada dato de la entidad
             * El dialogo obtiene el nombre de la clave de busqueda de algun registro si esta existe */
            RegisterDialog rd = new RegisterDialog(inputs, -1, 0);
            if (rd.ShowDialog() == DialogResult.OK) {
                /* Guarda los resultados en una lista de string generica
                 * Los resultados despues son convertidos a su tipo de dato especificado
                 * por el tipo de dato en cada atributo */

                List<string> forms = rd.output;
                /* Escribe la entrada en el archivo, agrega los registros de forma secuencial
                 * solo si tiene el tipo de indice 0, en los demás los agrega ordenados */

                /* Si el registro a insertar contiene indice primario o secundario o ambos
                 * entonces tiene que crear el archivo de índices, y sólo lo crea una vez. */
                long idxAdrs = index.Count;
                if ((key.PK || key.FK) && !File.Exists(Application.StartupPath + "\\examples\\" + comboBoxReg.Text + ".idx")) {
                    CreateIndexFile();
                    if (key.PK) {
                        CreatePKStructure();
                        ReplaceBytes(data, key.attribPKIndexAdrs + 48, BitConverter.GetBytes(idxAdrs));
                    }
                    idxAdrs = index.Count;
                    if (key.FK) {
                        CreateFKStructure();
                        ReplaceBytes(data, key.attribFKIndexAdrs + 48, BitConverter.GetBytes(idxAdrs));
                        // Si el proyecto lo requiere, implementar un ciclo en donde se vayan agregando todos
                        // los índices secundarios en el archivo de indices
                    }
                }

                if (key.PK) {
                    bool resp = InsertPrimaryKey(rd.output[key.PKAtribListIndex], forms, types, sizes);
                    if (resp) {
                        UpdateMainPKTable();
                        WriteIndexFile(comboBoxReg.Text);
                    }
                }
                if (key.FK) {
                    bool resp = InsertForeignKey(rd.output[key.FKAtribListIndex], forms, types, sizes);
                    if (resp) {
                        UpdateMainFKTable();
                        WriteIndexFile(comboBoxReg.Text);
                    }
                }
                /* Actualizar la tabla de registros */
                UpdateEntityTable();
                UpdateRegisterTable(types, sizes);
                WriteRegisterFile(comboBoxReg.Text);
            }
        }




        #endregion
        // Delete register file
        private void BtnDeleteRegister(object sender, EventArgs e) {
            byte[] dataP = data.ToArray();
            List<string> inputs = new List<string>();
            List<char> types = new List<char>();
            List<int> sizes = new List<int>();
            // Obtiene todos los metadatos de los atributos para poder agregar registros
            // Obtiene los nombres, los tipos y la longitud de los atributos
            long aux = BitConverter.ToInt64(data.ToArray(), (int)selectedEntityAdrs + 38);
            while (aux != -1) {
                inputs.Add(Encoding.UTF8.GetString(dataP, (int)aux, 30).Replace("~", ""));
                types.Add(BitConverter.ToChar(dataP, (int)aux + 38));
                sizes.Add(BitConverter.ToInt32(dataP, (int)aux + 40));
                aux = BitConverter.ToInt64(dataP, (int)aux + 56);
            }

            RegisterDialog rd = new RegisterDialog(inputs, key.searchKeyAttribIndex, 2);
            if (rd.ShowDialog() == DialogResult.OK) {
                List<string> change = rd.output;
                if (DeleteRegister(rd.output[0])) {
                    UpdateRegisterTable(types, sizes);
                    MessageBox.Show("Register deleted sucessfully", "Success");
                }
                else {
                    MessageBox.Show("Register not found", "Error");
                }
            }
        }

        private void BtnModifyRegister(object sender, EventArgs e) {

        }

        private void DataBase_Resize(object sender, EventArgs e) {
            tabControl1.Height = ClientSize.Height - 28;
            tabControl1.Width = ClientSize.Width;
            entityTable.Height = ClientSize.Height - 116;
            entityTable.Width = ClientSize.Width - 210;
            attributeTable.Height = ClientSize.Height - 116;
            attributeTable.Width = ClientSize.Width - 210;
            registerTable.Height = ClientSize.Height - 116;
            registerTable.Width = ClientSize.Width - 210;
        }

        private void comboBoxPK_TextChanged(object sender, EventArgs e) {
            long eIndex = -1, eAnt = -1;
            SearchEntity(comboBoxReg.Text, ref eIndex, ref eAnt);
            selectedEntityAdrs = eIndex;

            if (File.Exists(Application.StartupPath + "\\examples\\" + comboBoxReg.Text + ".idx")) {
                long aIndex = BitConverter.ToInt64(data.ToArray(), (int)selectedEntityAdrs + 38);

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

                    switch (iType) {
                        case 1:
                            key.searchKey = true;
                            key.searchKeyPos = auxSearchKey;
                            key.searchKeySize = aLength;
                            key.searchKeyAttribIndex = i;
                            key.searchKeyIsChar = aType == 'C' ? true : false;
                            break;
                        case 2:
                            key.PK = true;
                            key.PKAtribListIndex = i;
                            key.PKIsChar = aType == 'C' ? true : false;
                            key.PKSize = aLength;
                            key.attribPKIndexAdrs = aIndex;
                            break;
                        case 3:
                            key.FK = true;
                            key.FKAtribListIndex = i;
                            key.FKIsChar = aType == 'C' ? true : false;
                            key.PKSize = aLength;
                            key.attribFKIndexAdrs = aIndex;
                            break;
                        default:
                            break;
                    }
                    registerSize += aLength;
                    i++;
                    aIndex = BitConverter.ToInt64(data.ToArray(), (int)aIndex + 56);
                }

            }
        }

        private void InitializeFKIndexTable() {
            mainFKTable.Columns.Clear();
            secondFKTable.Columns.Clear();

            var cl1 = new DataGridViewTextBoxColumn {
                Width = 60,
                Resizable = DataGridViewTriState.False
            };
            cl1.HeaderText = key.PKIsChar ? "Character" : "Number";
            cl1.Name = key.PKIsChar ? "Character" : "Number";

            mainFKTable.Columns.Add(cl1);

            var cl2 = new DataGridViewTextBoxColumn {
                Resizable = DataGridViewTriState.False,
                HeaderText = "Sublist address",
                Name = "Sublist address",
                Width = 60
            };
            mainFKTable.Columns.Add(cl2);
            for (int i = 0; i < 50; i++) {
                mainFKTable.Rows.Add(-1, -1);
            }
        }

        private void CreateIndexFile() {
            try {
                bw = new BinaryWriter(new FileStream(Application.StartupPath + "\\examples\\" + comboBoxReg.Text + ".idx", FileMode.Create));
                bw.Close();
            }
            catch (IOException ex) {
                Console.WriteLine(ex.Message + "\n Cannot create file.");
            }
        }

        private void UpdatePKTable() {
            mainPKTable.Rows.Clear();
            for (int i = 0; i < 9; i++) {
                long adrs = BitConverter.ToInt64(index.ToArray(), i * (key.PKSize + 8) + key.PKSize);
                mainPKTable.Rows.Add(i + 1, adrs);
            }
        }

        private void InitializePKIndexTable() {
            // Obtiene la dirección de los registros de la entidad
            mainPKTable.Columns.Clear();
            secondPKTable.Columns.Clear();
            // Recorre todos los atributos, y los agrega como columnas
            var cl1 = new DataGridViewTextBoxColumn {
                Width = 60,
                Resizable = DataGridViewTriState.False
            };
            cl1.HeaderText = key.PKIsChar ? "Character" : "Number";
            cl1.Name = key.PKIsChar ? "Character" : "Number";
            
            mainPKTable.Columns.Add(cl1);
            
            var cl2 = new DataGridViewTextBoxColumn {
                Resizable = DataGridViewTriState.False,
                HeaderText = "Sublist address",
                Name = "Sublist address",
                Width = 60
            };
            mainPKTable.Columns.Add(cl2);

            if (!key.PKIsChar) { 
                for (int i = 0; i < 9; i++) {
                    mainPKTable.Rows.Add(i + 1, -1);
                }
            }
            else {
                char name = 'A';
                for (int i = 0; i < 26; i++) {
                    mainPKTable.Rows.Add(name++, -1);
                }
            }

            var cl3 = new DataGridViewTextBoxColumn {
                Width = 120,
                HeaderText = "Search Key",
                Name = "Search Key",
                Resizable = DataGridViewTriState.False
            };

            secondPKTable.Columns.Add(cl3);

            var cl4 = new DataGridViewTextBoxColumn {
                Resizable = DataGridViewTriState.False,
                HeaderText = "Address",
                Name = "Address",
                Width = 60
            };
            secondPKTable.Columns.Add(cl4);
        }

        private void nextPKPage(object sender, EventArgs e) {
            int cant = !key.PKIsChar ? 9 : 26;
            if (page < cant) {
                buttonPrevPage.Enabled = true;
                page++;
                pageNumber.Text = !key.PKIsChar ? page.ToString() : Convert.ToChar(page + 64).ToString();

            }
            if (page == cant) {
                buttonNextPage.Enabled = false;
            }
            UpdateSecondPKTable();
        }

        private void prevPKPage(object sender, EventArgs e) {
            int cant = !key.PKIsChar ? 9 : 26;
            if (page > 1) {
                buttonNextPage.Enabled = true;
                page--;
                pageNumber.Text = !key.PKIsChar ? page.ToString() : Convert.ToChar(page + 64).ToString();

            }
            if (page == 1) {
                buttonPrevPage.Enabled = false;
                
            }
            UpdateSecondPKTable();
        }

        private void UpdateSecondPKTable() {
            if (index.Count > 0) {
                secondPKTable.Rows.Clear();
                long adrs = -1;
                if (!key.PKIsChar) {
                    adrs = BitConverter.ToInt64(index.ToArray(), (page - 1) * 12 + 4); 
                }
                else {
                    adrs = BitConverter.ToInt64(index.ToArray(), (page - 1) * 10 + 2);
                }
                if (adrs != -1) {
                    int code = -1;
                    string scode = "";
                    long regAdrs = BitConverter.ToInt64(index.ToArray(), (int)adrs + key.PKSize);
                    do {

                        if (!key.PKIsChar) {
                            code = BitConverter.ToInt32(index.ToArray(), (int)adrs);
                            secondPKTable.Rows.Add(code, regAdrs);
                        }
                        else {
                            scode = Encoding.UTF8.GetString(index.ToArray(), (int)adrs, key.PKSize).Replace("~", "");
                            secondPKTable.Rows.Add(scode, regAdrs);
                        }
                        adrs += key.PKSize + 8;
                        regAdrs = BitConverter.ToInt64(index.ToArray(), (int)adrs + key.PKSize);
                    } while (regAdrs != -1);
                }
            }
        }

        /* Actualiza las tablas de indice primario, la tabla de los numeros del 1 al 9 o de letras
         * de la a - z*/
        private void UpdateMainPKTable() {
            secondPKTable.Rows.Clear();
            long adrs = -1;
            int aum = !key.PKIsChar ? 9 : 26;
            for (int i = 0, inc = 0, inc2 = 0; i < aum; i++, inc += key.PKSize, inc2 += key.PKSize) {
                adrs = !key.PKIsChar 
                    ? BitConverter.ToInt64(index.ToArray(), i * 12 + 4) 
                    : BitConverter.ToInt64(index.ToArray(), i * 10 + 2);
                mainPKTable.Rows[i].Cells[1].Value = adrs;
            }
            adrs = !key.PKIsChar
                ? BitConverter.ToInt64(index.ToArray(), (page - 1) * 12 + 4)
                : BitConverter.ToInt64(index.ToArray(), (page - 1) * 10 + 2);
            if (adrs != -1) {
                int code = -1;
                string scode = "";
                long regAdrs = BitConverter.ToInt64(index.ToArray(), (int)adrs + key.PKSize);
                do {
                    if (!key.PKIsChar) {
                        code = BitConverter.ToInt32(index.ToArray(), (int)adrs);
                        secondPKTable.Rows.Add(code, regAdrs);
                    }
                    else {
                        scode = Encoding.UTF8.GetString(index.ToArray(), (int)adrs, key.PKSize).Replace("~", "");
                        secondPKTable.Rows.Add(scode, regAdrs);
                    }
                    adrs += key.PKSize + 8;
                    regAdrs = BitConverter.ToInt64(index.ToArray(), (int)adrs + key.PKSize);
                } while (regAdrs != -1);
            }
        }

        private void UpdateMainFKTable() {
            throw new NotImplementedException();
        }
    }
}
