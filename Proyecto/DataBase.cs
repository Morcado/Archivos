using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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
		public int PKAdrsOnFile;
		public int PKType;

		public bool FK;
		public int FKAtribListIndex;
		public bool FKIsChar;
		public int FKSize;
		public long attribFKIndexAdrs;
		public int FKAdrsOnFile;

		public bool Hash { get; internal set; }
		public long attribHashIndexAdrs { get; internal set; }
		public int HashAdrsOnFile { get; internal set; }
		public int HashAtribListIndex { get; internal set; }
		public int Prefix { get; set; }
	}

	public partial class DataBase : Form {
		/* Variables de los arreglos y de los archivos */
		private BinaryWriter bw;
		private BinaryReader br;
		private List<byte> data;
		private List<byte> register;
		private List<byte> index;
		private string dictionaryName;

		/* Variables auxiliares de las entidades */
		private long head;
		private long selectedEntityAdrs;

		/* Variables auxiliares de los índices */
		private IndexFlag key;
		private int pagePK = 1;
		private int pageFK = 1;

		/* Variables auxiliares de los atributos*/
		List<string> inputs;
		List<char> types;
		List<int> sizes;

		/* Variables auxiliares de los registros */
		private int registerSize;
		private int prefix;

		public DataBase() {
			InitializeComponent();
			prefix = 0;
			registerSize = 0;
			key.searchKeyPos = -1;
			key.searchKey = false;
			key.PK = false;
			key.FK = false;

			selectedEntityAdrs = -1;
			data = new List<byte>();
			register = new List<byte>();
			index = new List<byte>();

			panel1.AutoScroll = false;
			panel1.HorizontalScroll.Enabled = false;
			panel1.HorizontalScroll.Visible = false;
			panel1.HorizontalScroll.Maximum = 0;
			panel1.AutoScroll = true;
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
				data = br.ReadBytes((int)new FileInfo(open.FileName).Length).ToList();
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


		/* Carga un archivo separado por comas y crea una base de datos con la entidad alumno y 
		 * atributo de clave y nombre, se puede modficar el tipo de indices que tiene y la organizacion*/
		private void OpenCSV(object sender, EventArgs e) {
			//MessageBox.Show("In progress...");
			OpenFileDialog open = new OpenFileDialog {
				InitialDirectory = Application.StartupPath + "\\examples",
				Filter = "CSV (*.csv)|*.csv|All files (*.*)|*.*",
				DefaultExt = ".bin"
			};

			if (open.ShowDialog() == DialogResult.OK) {
				data.Clear();
				register.Clear();
				index.Clear();
				

				try {
					// Crea el archivo del diccionario de datos
					bw = new BinaryWriter(new FileStream(Application.StartupPath + "\\examples\\" + open.SafeFileName + ".dat", FileMode.Create));
					bw.Close();
					bw = new BinaryWriter(new FileStream(Application.StartupPath + "\\examples\\" + open.SafeFileName + ".idx", FileMode.Create));
					bw.Close();
				}
				catch (IOException ex) {
					Console.WriteLine(ex.Message + "\n Cannot create file.");
					return;
				}
				InitializeTables();

				dictionaryName = open.FileName;
				data.AddRange(BitConverter.GetBytes((long)(-1)));
				head = -1;

				selectedEntityAdrs = 8;

				AddEntity("Alumno.csv");
				AddAttribute("Clave", 'I', 4, 2);
				AddAttribute("Nombre", 'C', 40, 3);
				types = new List<char> {'I', 'C'};
				sizes = new List<int> {4, 40};


				long idxAdrs = index.Count;
				key.PK = key.FK = true;
				key.PKIsChar = false;
				key.FKIsChar = true;
				key.PKSize = 4;
				key.FKSize = 40;
				key.attribPKIndexAdrs = 70; 
				key.attribFKIndexAdrs = 134;
				key.PKAtribListIndex = 0;
				key.FKAtribListIndex = 1;

				registerSize = 44;
				InitializeFKIndexTable();
				InitializePKIndexTable();
				InitializeRegisterTable();

				if (key.PK || key.FK || key.Hash) {
					if (key.PK) {
						key.PKAdrsOnFile = index.Count;
						CreatePKStructure();
						ReplaceBytes(data, key.attribPKIndexAdrs + 48, BitConverter.GetBytes(idxAdrs));
						InitializePKIndexTable();
					}
					idxAdrs = index.Count;
					if (key.FK) {
						key.FKAdrsOnFile = index.Count;
						CreateFKStructure();
						ReplaceBytes(data, key.attribFKIndexAdrs + 48, BitConverter.GetBytes(idxAdrs));
						InitializeFKIndexTable();
						// Si el proyecto lo requiere, implementar un ciclo en donde se vayan agregando todos
						// los índices secundarios en el archivo de indices
					}
					if (key.Hash) {
						key.HashAdrsOnFile = index.Count;
						CreateHashStructure();
						ReplaceBytes(data, key.attribHashIndexAdrs + 48, BitConverter.GetBytes(idxAdrs));
						InitializeHashTable();
					}

				}

				StreamReader reader;
				try {
					reader = new StreamReader(open.FileName);

				}
				catch (IOException ex) {
					MessageBox.Show(ex.ToString());
					return;
				}
				while (!reader.EndOfStream) {
					string line = reader.ReadLine();
					List<string> values = line.Split(',').ToList();
					values[0] = values[0].Substring(2);
					AddRegister(values);
						
				}
				reader.Close();
				
				UpdateEntityTable();
				UpdateAttribTable();
				UpdateRegisterTable();
				WriteDictionary();
				WriteRegisterFile(open.SafeFileName);
				if (key.PK || key.FK || key.Hash) {
					if (key.PK) {
						UpdateMainPKTable();
					}
					if (key.FK) {
						UpdateMainFKTable();
					}
					if (key.Hash) {
						UpdateHashTable();
					}
					WriteIndexFile(open.SafeFileName);
				}
			}
		}

		// Cierra la aplicación
		private void ExitToolStripMenuItem_Click(object sender, EventArgs e) {
			Application.Exit();
		}

		#endregion
		#region Binary Data Operations
		
		// Reemplaza n bytes especificados por el índice en el archivo, recibe un byte[]
		private void ReplaceBytes(List<byte> dat, long index, byte[] newData) {
			// check
			for (int i = (int)index; i < index + newData.Length; i++) {
				dat[i] = newData[i - (int)index];
			}
		}

		/* Guarda el archivo en binario cada vez que se haga un cambio en cualquier estructura de datos. 
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
		#region User Interface

		// Actualiza la tabla de atributos eligiendo una entidad del combobox1
		private void ComboBoxAtt_TextChanged(object sender, EventArgs e) {
			long eIndex = -1, eAnt = -1;
			byte[] dataP = data.ToArray();
			SearchEntity(comboBoxAtt.Text, ref eIndex, ref eAnt);
			selectedEntityAdrs = eIndex;
			UpdateAttribTable();
			// Si existe el archivo, entonces se guardan todos los atributos en memoria y metadatos
			inputs = new List<string>();
			types = new List<char>();
			sizes = new List<int>();

			if (comboBoxAtt.Text != "") {
				buttonAtt1.Enabled = buttonAtt2.Enabled = buttonAtt3.Enabled = buttonAtt4.Enabled = true;
			}
			else {
				buttonAtt1.Enabled = buttonAtt2.Enabled = buttonAtt3.Enabled = buttonAtt4.Enabled = false;
			}

			// Si la entidad no apunta a -1, entonces tiene elementos :v
			long aIndex = BitConverter.ToInt64(data.ToArray(), (int)selectedEntityAdrs + 38);
			int i = 0, auxSearchKey = 0;
			registerSize = 0;
			while (aIndex != -1) {
				string formInput = Encoding.UTF8.GetString(dataP, (int)aIndex, 30).Replace("~", "");
				char aType = BitConverter.ToChar(data.ToArray(), (int)aIndex + 38);
				int aLength = BitConverter.ToInt32(data.ToArray(), (int)aIndex + 40);
				int iType = BitConverter.ToInt32(data.ToArray(), (int)aIndex + 44);

				types.Add(aType);
				sizes.Add(aLength);
				inputs.Add(formInput);

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
						key.PKType = 1;
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
					case 4:
						key.Hash = true;
						key.attribHashIndexAdrs = aIndex;
						key.HashAtribListIndex = i;
						break;
					default:
						break;
				}
				registerSize += aLength;
				i++;
				aIndex = BitConverter.ToInt64(data.ToArray(), (int)aIndex + 56);
			}
			if (File.Exists(Application.StartupPath + "\\examples\\" + comboBoxAtt.Text + ".dat")) {
				ReadRegisterFile(comboBoxAtt.Text);
				buttonAtt4.Enabled = false;
				buttonAtt5.Enabled = true;
				buttonReg3.Enabled = true;
				buttonReg4.Enabled = true;
				buttonReg5.Enabled = true;
				label5.Text = comboBoxAtt.Text;
				InitializeRegisterTable();
				UpdateRegisterTable();
			}
			// Si existe un archivo de indice para el registro, lo lee y actualiza la tabla
			else {
				registerTable.Columns.Clear();
				buttonAtt4.Enabled = true;
				buttonAtt5.Enabled = false;
				buttonReg3.Enabled = false;
				//buttonReg4.Enabled = false;
				buttonReg5.Enabled = false;
				label5.Text = "Register";
			}
			if (File.Exists(Application.StartupPath + "\\examples\\" + comboBoxAtt.Text + ".idx")) {
				ReadIndexFile(comboBoxAtt.Text);
				if (key.PK) {
					InitializePKIndexTable();
					UpdateMainPKTable();
				}
				if (key.FK) {
					key.FKAdrsOnFile = BitConverter.ToInt32(data.ToArray(), (int)key.attribFKIndexAdrs + 48);
					InitializeFKIndexTable();
					UpdateMainFKTable();
				}
				if (key.Hash) {
					key.HashAdrsOnFile = 0;
					InitializeHashTable();
					UpdateHashTable();
					UpdateHashBoxes();
				}
			}
			else {
				mainFKTable.Columns.Clear();
				mainPKTable.Columns.Clear();
				secondFKTable.Columns.Clear();
				secondPKTable.Columns.Clear();
			}
		}

		// Ajusta el tamaño de las tablas cuando se redimensiona el área cliente
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

		#endregion
		#region Tables
		/* Inicializa la tabla de entidades y de atributos con sus columnas por defecto */
		private void InitializeTables() {

			DataGridViewTextBoxColumn col1 = new DataGridViewTextBoxColumn {
				HeaderText = "Entity name", Name = "Entity name",
				MinimumWidth = 150, Width = 150, Resizable = DataGridViewTriState.False
			};
			DataGridViewTextBoxColumn col2 = new DataGridViewTextBoxColumn {
				HeaderText = "Entity address", Name = "Entity address",
				MinimumWidth = 80, Width = 80, Resizable = DataGridViewTriState.False
			};
			DataGridViewTextBoxColumn col3 = new DataGridViewTextBoxColumn {
				HeaderText = "Attribute address", Name = "Attribute address",
				MinimumWidth = 80, Width = 80, Resizable = DataGridViewTriState.False
			};
			DataGridViewTextBoxColumn col4 = new DataGridViewTextBoxColumn {
				HeaderText = "Data address", Name = "Data address",
				MinimumWidth = 80, Width = 80, Resizable = DataGridViewTriState.False
			};
			DataGridViewTextBoxColumn col5 = new DataGridViewTextBoxColumn {
				HeaderText = "Next entity address", Name = "Next entity address",
				MinimumWidth = 80, Width = 80, Resizable = DataGridViewTriState.False
			};

			entityTable.Columns.AddRange(new DataGridViewColumn[] { col1, col2, col3, col4, col5 });
			DataGridViewTextBoxColumn cl1 = new DataGridViewTextBoxColumn {
				HeaderText = "Attribute name", Name = "Attribute name",
				MinimumWidth = 120, Width = 120, Resizable = DataGridViewTriState.False
			};
			DataGridViewTextBoxColumn cl2 = new DataGridViewTextBoxColumn {
				HeaderText = "Attribute address", Name = "Attribute address",
				MinimumWidth = 60, Width = 60, Resizable = DataGridViewTriState.False
			};
			DataGridViewTextBoxColumn cl3 = new DataGridViewTextBoxColumn {
				HeaderText = "Data type", Name = "Data type", MinimumWidth = 60,
				Width = 60, Resizable = DataGridViewTriState.False
			};
			DataGridViewTextBoxColumn cl4 = new DataGridViewTextBoxColumn {
				HeaderText = "Data length", Name = "Data length",
				MinimumWidth = 60, Width = 60, Resizable = DataGridViewTriState.False
			};
			DataGridViewTextBoxColumn cl5 = new DataGridViewTextBoxColumn {
				HeaderText = "Index type", Name = "Index type",
				MinimumWidth = 60, Width = 60, Resizable = DataGridViewTriState.False
			};
			DataGridViewTextBoxColumn cl6 = new DataGridViewTextBoxColumn {
				HeaderText = "Index address", Name = "Index address",
				MinimumWidth = 60, Width = 60, Resizable = DataGridViewTriState.False
			};
			DataGridViewTextBoxColumn cl7 = new DataGridViewTextBoxColumn {
				HeaderText = "Next attribute address", Name = "Index type",
				MinimumWidth = 60, Width = 60, Resizable = DataGridViewTriState.False
			};

			attributeTable.Columns.AddRange(new DataGridViewColumn[] { cl1, cl2, cl3, cl4, cl5, cl6, cl7 });
		}

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

		private void UpdateRegisterTable() {
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
			registerTable.Sort(registerTable.Columns["Register address"], ListSortDirection.Ascending);
		}

		/* Actualiza la tabla de atributos de la entidad seleccionada. Primero se busca la entidad en el archivo, 
		 * después se localiza la dirección de los atributos de esa entidad y se recorren hasta que el siguiente sea -1.
		 * Los atributos son ordenados en la tabla por su dirección en el archivo*/
		private void UpdateAttribTable() {
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
		#endregion
		#region Entity

		/* Evento del boton que agrega una entidad en el archivo
		 * Las entidades son insertadas ordenadas y primero se verifica que no exista la entidad antes de
		 * insertarla. El largo maximo del nombre de la entidad es de 30 caracteres */
		private void BtnAddEntity(object sender, EventArgs e) {
			NewEntityDialog dg = new NewEntityDialog(0);
			dg.ShowDialog();
			if (dg.DialogResult == DialogResult.OK) {
				if (AddEntity(dg.name)) {
					UpdateEntityTable();
					index.Clear();
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
		private void BtnRemoveEntity(object sender, EventArgs e) {
			NewEntityDialog et = new NewEntityDialog(3);
			et.ShowDialog();
			if (et.DialogResult == DialogResult.OK) {
				if (DeleteEntity(et.name)) {
					UpdateEntityTable();
					WriteDictionary();
					comboBoxAtt.Items.Remove(et.name);
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
		private void BtnModifyEntity(object sender, EventArgs e) {
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
		/* Agrega un atributo a la entidad seleccionada. Primero se verifica que el atributo no exista.
		 * Los atributos se agregan secuencialmente al final del archivo */
		private void BtnAddAtribute(object sender, EventArgs e) {
			NewAttributeDialog dg = new NewAttributeDialog(0);
			dg.ShowDialog();
			if (dg.DialogResult == DialogResult.OK) {
				AddAttribute(dg.name, dg.type, dg.length, dg.indexType);
				ComboBoxAtt_TextChanged(this, null);
				UpdateAttribTable();
				MessageBox.Show("Attribute added successfully");
				WriteDictionary();
			}
		}

		/* Modifica un atributo de la entidad seleccionada. Primero se verifica que el atributo exista y
		después se modifica la dirección, por lo tanto el tamaño del archivo no se modifica. */
		private void BtnModifyAttribute(object sender, EventArgs e) {
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
						UpdateAttribTable();
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
		private void BtnRemoveAttribute(object sender, EventArgs e) {
			NewEntityDialog ed = new NewEntityDialog(5);
			ed.ShowDialog();
			if (ed.DialogResult == DialogResult.OK) {
				if (DeleteAttribute(ed.name)) {
					UpdateAttribTable();
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
		/* Crea un archivo de datos para la entidad seleccionada. Sólo se puede crear si no existe */
		private void BtnCreateRegisterFile(object sender, EventArgs e) {
			try {
				bw = new BinaryWriter(new FileStream(Application.StartupPath + "\\examples\\" + comboBoxAtt.Text + ".dat", FileMode.Create));
				bw.Close();
				buttonAtt4.Enabled = false;
				buttonAtt5.Enabled = true;
				buttonReg3.Enabled = true;
				buttonReg5.Enabled = true;
				label5.Text = comboBoxAtt.Text;
				index.Clear();
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
				File.Delete(Application.StartupPath + "\\examples\\" + comboBoxAtt.Text + ".dat");
				File.Delete(Application.StartupPath + "\\examples\\" + comboBoxAtt.Text + ".idx");
				buttonAtt4.Enabled = true;
				buttonAtt5.Enabled = false;
				buttonReg3.Enabled = false;
				buttonReg5.Enabled = false;
				label5.Text = "Register";

				register.Clear();
				index.Clear();
				mainFKTable.Columns.Clear();
				mainPKTable.Columns.Clear();
				secondFKTable.Columns.Clear();
				secondPKTable.Columns.Clear();
				registerTable.Columns.Clear();
				if (key.PK) {
					ReplaceBytes(data, key.attribPKIndexAdrs + 48, BitConverter.GetBytes((long)-1));
				}
				if (key.FK) {
					ReplaceBytes(data, key.attribFKIndexAdrs + 48, BitConverter.GetBytes((long)-1));

				}
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

		// Modifica un registro buscando su clave de búsqueda, y si no la tiene busca por defecto la primera
		private void BtnModifyRegister(object sender, EventArgs e) {
			RegisterDialog rd = new RegisterDialog(inputs, key.searchKeyAttribIndex, false, key.searchKey, "Modify register");
			if (rd.ShowDialog() == DialogResult.OK) {
				long rIndex = -1, rAnt = -1;
				if (SearchRegistry(rd.output[0], ref rIndex, ref rAnt, true, false)) {
					RegisterDialog rg2 = new RegisterDialog(inputs, -1, true, key.searchKey, "Modify Register");
					if (rg2.ShowDialog() == DialogResult.OK) {
						if (ModifyRegister(rd.output, rg2.output, rIndex, rAnt)) {
							UpdateRegisterTable();
							WriteRegisterFile(comboBoxAtt.Text);
							WriteDictionary(); // optimizar
							if (key.PK || key.FK) {
								if (key.PK) {
									UpdateMainPKTable();
								}
								if (key.FK) {
									UpdateMainFKTable();
								}
								WriteIndexFile(comboBoxAtt.Text);
							}

							MessageBox.Show("Deleted");
						}
						else {
							MessageBox.Show("Not Deleted");
						}
					}
				}
			}
		}

		/* El botón hace que se agrege un registro en la entidad seleccionada. Los registros
		 * son agregados secuencialmente */
		private void BtnAddRegister(object sender, EventArgs e) {

			/* Muestra el diálogo para pedir cada dato de la entidad
			 * El dialogo obtiene el nombre de la clave de busqueda de algun registro si esta existe */
			RegisterDialog rd = new RegisterDialog(inputs, -1, true, key.searchKey, "Add register");
			if (rd.ShowDialog() == DialogResult.OK) {
				/* Guarda los resultados en una lista de string generica
				 * Los resultados despues son convertidos a su tipo de dato especificado
				 * por el tipo de dato en cada atributo */

				List<string> forms = rd.output;
				/* Si el registro a insertar contiene indice primario o secundario o ambos
				 * entonces tiene que crear el archivo de índices, y sólo lo crea una vez. */
				long idxAdrs = index.Count;
				bool resp;

				if ((key.PK || key.FK || key.Hash) && !File.Exists(Application.StartupPath + "\\examples\\" + comboBoxAtt.Text + ".idx")) {
					CreateIndexFile();
					if (key.PK) {
						key.PKAdrsOnFile = index.Count;
						CreatePKStructure();
						ReplaceBytes(data, key.attribPKIndexAdrs + 48, BitConverter.GetBytes(idxAdrs));
						InitializePKIndexTable();
					}
					idxAdrs = index.Count;
					if (key.FK) {
						key.FKAdrsOnFile = index.Count;
						CreateFKStructure();
						ReplaceBytes(data, key.attribFKIndexAdrs + 48, BitConverter.GetBytes(idxAdrs));
						InitializeFKIndexTable();
						// Si el proyecto lo requiere, implementar un ciclo en donde se vayan agregando todos
						// los índices secundarios en el archivo de indices
					}
					if (key.Hash) {
						key.HashAdrsOnFile = index.Count;
						CreateHashStructure();
						ReplaceBytes(data, key.attribHashIndexAdrs+ 48, BitConverter.GetBytes(idxAdrs));
						InitializeHashTable();
					}

				}


				resp = AddRegister(forms);
				if (resp) {
					/* Optimizar */
					UpdateAttribTable();
					WriteDictionary();
					/* Optimizar */

					UpdateRegisterTable();
					WriteRegisterFile(comboBoxAtt.Text);
					if (key.PK || key.FK || key.Hash) {
						if (key.PK) {
							UpdateMainPKTable();
						}
						if (key.FK) {
							UpdateMainFKTable();
						}
						if (key.Hash) {
							UpdateHashTable();
							UpdateHashBoxes();
						}
						WriteIndexFile(comboBoxAtt.Text);
					}
				}
				else {
					MessageBox.Show("Error, register already exists");
				}
			}
		}

		#endregion
		// Borra el registro proporcionando la clave de búsqueda si la tiene, si no la tiene
		// se ingresa el primer atributo del índice
		private void BtnDeleteRegister(object sender, EventArgs e) {
			RegisterDialog rd = new RegisterDialog(inputs, key.searchKeyAttribIndex, false, key.searchKey, "Delete register");
			if (rd.ShowDialog() == DialogResult.OK) {
				List<string> change = rd.output;
				long rAnt = -1, rIndex = -1;
				if (SearchRegistry(rd.output[0], ref rIndex, ref rAnt, true, false)) {
					DeleteRegister(rIndex, rAnt);
					UpdateEntityTable();
					UpdateRegisterTable();
					UpdateAttribTable();
					WriteRegisterFile(comboBoxAtt.Text);
					WriteDictionary(); // optimizar
					if (key.PK || key.FK) {
						if (key.PK) {
							UpdateMainPKTable();
						}
						if (key.FK) {
							UpdateMainFKTable();
						}
						WriteIndexFile(comboBoxAtt.Text);
					}
					MessageBox.Show("Register deleted sucessfully", "Success");
				}
				else {
					MessageBox.Show("Register not found", "Error");
				}
			}
		}

		private void InitializeFKIndexTable() {
			mainFKTable.Columns.Clear();
			secondFKTable.Columns.Clear();

			var cl1 = new DataGridViewTextBoxColumn { Width = 80 };
			cl1.HeaderText = "Foreign Key";
			cl1.Name = "Foreign Key";

			mainFKTable.Columns.Add(cl1);

			var cl2 = new DataGridViewTextBoxColumn {
				Resizable = DataGridViewTriState.False, HeaderText = "Sublist address",
				Name = "Sublist address", Width = 60
			};
			mainFKTable.Columns.Add(cl2);
			for (int i = 0; i < 50; i++) {
				mainFKTable.Rows.Add(-1, -1);
			}

			var cl4 = new DataGridViewTextBoxColumn {
				Resizable = DataGridViewTriState.False, HeaderText = "Address",
				Name = "Address", Width = 60
			};
			secondFKTable.Columns.Add(cl4);
		}

		private void CreateIndexFile() {
			try {
				bw = new BinaryWriter(new FileStream(Application.StartupPath + "\\examples\\" + comboBoxAtt.Text + ".idx", FileMode.Create));
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
				Width = 80
			};
			cl1.HeaderText = "Primary Key";
			cl1.Name = "Primary Key";
			
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
				Width = 120, HeaderText = "Search Key", Name = "Search Key",
				Resizable = DataGridViewTriState.False
			};

			secondPKTable.Columns.Add(cl3);

			var cl4 = new DataGridViewTextBoxColumn {
				Resizable = DataGridViewTriState.False, HeaderText = "Address",
				Name = "Address", Width = 60
			};
			secondPKTable.Columns.Add(cl4);

			numericUpDown2.Maximum = key.PKIsChar ? 26 : 9;
		}

		private void NextPKPage(object sender, EventArgs e) {
			int cant = !key.PKIsChar ? 9 : 26;
			if (pagePK < cant) {
				buttonPrevPKPage.Enabled = true;
				pagePK++;
				pagePKNumber.Text = !key.PKIsChar ? pagePK.ToString() : Convert.ToChar(pagePK + 64).ToString();

			}
			if (pagePK == cant) {
				buttonNextPKPage.Enabled = false;
			}
			UpdateSecondPKTable();
		}

		private void PrevPKPage(object sender, EventArgs e) {
			if (pagePK > 1) {
				buttonNextPKPage.Enabled = true;
				pagePK--;
				pagePKNumber.Text = !key.PKIsChar ? pagePK.ToString() : Convert.ToChar(pagePK + 64).ToString();
			}
			if (pagePK == 1) {
				buttonPrevPKPage.Enabled = false;
				
			}
			UpdateSecondPKTable();
		}

		private void UpdateSecondPKTable() {
			if (index.Count > 0) {
				secondPKTable.Rows.Clear();
				long adrs = -1;
				adrs = !key.PKIsChar
					? BitConverter.ToInt64(index.ToArray(), ((pagePK - 1) * 12) + 4)
					: BitConverter.ToInt64(index.ToArray(), ((pagePK - 1) * 10) + 2);
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
			byte[] indexPrint = index.ToArray();
			secondPKTable.Rows.Clear();
			long adrs = -1;
			int aum = !key.PKIsChar ? 9 : 26;
			for (int i = 0, inc = 0, inc2 = 0; i < aum; i++, inc += key.PKSize, inc2 += key.PKSize) {
				adrs = !key.PKIsChar 
					? BitConverter.ToInt64(indexPrint, i * 12 + 4) 
					: BitConverter.ToInt64(indexPrint, i * 10 + 2);
				mainPKTable.Rows[i].Cells[1].Value = adrs;
			}
			adrs = !key.PKIsChar
				? BitConverter.ToInt64(indexPrint, (pagePK - 1) * 12 + 4)
				: BitConverter.ToInt64(indexPrint, (pagePK - 1) * 10 + 2);
			if (adrs != -1) {
				int code = -1;
				string scode = "";
				long regAdrs = BitConverter.ToInt64(indexPrint, (int)adrs + key.PKSize);
				do {
					if (!key.PKIsChar) {
						code = BitConverter.ToInt32(indexPrint, (int)adrs);
						secondPKTable.Rows.Add(code, regAdrs);
					}
					else {
						scode = Encoding.UTF8.GetString(indexPrint, (int)adrs, key.PKSize).Replace("~", "");
						secondPKTable.Rows.Add(scode, regAdrs);
					}
					adrs += key.PKSize + 8;
					regAdrs = BitConverter.ToInt64(indexPrint, (int)adrs + key.PKSize);
				} while (regAdrs != -1);
			}
		}

		private void UpdateMainFKTable() {
			byte[] indexPrint = index.ToArray();
		 
			secondFKTable.Rows.Clear();
			long adrs = -1;
			for (int i = 0; i < 50; i++) {
				if (key.FKIsChar) {
					string name = Encoding.UTF8.GetString(indexPrint, key.FKAdrsOnFile + (i * (key.FKSize + 8)), key.FKSize).Replace("~", "").TrimEnd('\0');
					mainFKTable.Rows[i].Cells[0].Value = name;
				}
				else {
					int name = BitConverter.ToInt32(indexPrint, key.FKAdrsOnFile + (i * (key.FKSize + 8)));
					mainFKTable.Rows[i].Cells[0].Value = name;
				}
				adrs = BitConverter.ToInt64(indexPrint, key.FKAdrsOnFile + (i * (key.FKSize + 8)) + key.FKSize);
				mainFKTable.Rows[i].Cells[1].Value = adrs;
			}
			adrs = BitConverter.ToInt64(indexPrint, key.FKAdrsOnFile + ((pageFK - 1)* (key.FKSize + 8)) + key.FKSize);
			if (adrs != -1) {

				long regAdrs = BitConverter.ToInt64(indexPrint, (int)adrs);
				do {
					secondFKTable.Rows.Add(regAdrs);
					adrs += 8;
					regAdrs = BitConverter.ToInt64(indexPrint, (int)adrs);
				} while (regAdrs != -1);
			}

		}

		private void PrevFKPage(object sender, EventArgs e) {
			if (pageFK > 1) {
				buttonNextFKPage.Enabled = true;
				pageFK--;
				pageFKNumber.Text =  pageFK.ToString();
			}
			if (pageFK == 1) {
				buttonPrevFKPage.Enabled = false; 
			}
			UpdateSecondFKTable();
		}

		private void NextFKPage(object sender, EventArgs e) {
			int cant = 50;
			if (pageFK < cant) {
				buttonPrevFKPage.Enabled = true;
				pageFK++;
				pageFKNumber.Text = pageFK.ToString();
			}
			if (pageFK == cant) {
				buttonNextFKPage.Enabled = false;
			}
			UpdateSecondFKTable();
		}

		private void UpdateSecondFKTable() {
			byte[] indexPrint = index.ToArray();
			if (index.Count > 0) {
				secondFKTable.Rows.Clear();
				long adrs = -1;
				adrs = BitConverter.ToInt64(indexPrint, key.FKAdrsOnFile + ((pageFK - 1) * (key.FKSize + 8)) + key.FKSize);
				if (adrs != -1) {
					
					long regAdrs = BitConverter.ToInt64(indexPrint, (int)adrs);
					do {
						secondFKTable.Rows.Add(regAdrs);
						adrs += 8;
						regAdrs = BitConverter.ToInt64(indexPrint, (int)adrs);
					} while (regAdrs != -1);
				}
			}
		}

		// Elige una pagina del indice secundario
		private void Button1_Click(object sender, EventArgs e) {
			pageFK = Convert.ToInt32(numericUpDown1.Value);
			pageFKNumber.Text = pageFK.ToString();
			if (pageFK == 50) {
				buttonNextFKPage.Enabled = false;
			}
			if (pageFK == 1) {
				buttonPrevFKPage.Enabled = false;
			}
			UpdateSecondFKTable();
		}

		private void Button2_Click(object sender, EventArgs e) {
			pagePK = Convert.ToInt32(numericUpDown2.Value);
			pagePKNumber.Text = key.PKIsChar ? Convert.ToChar(pagePK + 64).ToString() : pagePK.ToString();
			if (pagePK == 50) {
				buttonNextPKPage.Enabled = false;
			}
			if (pagePK == 1) {
				buttonPrevPKPage.Enabled = false;
			}
			if (pagePK > 1 && pagePK < 50) {
				buttonNextPKPage.Enabled = true;
				buttonPrevPKPage.Enabled = true;
			}
			UpdateSecondPKTable();
		}

		/* Crea la tabla de hash, sin agregar valores inicializandolo en -1 */
		private void InitializeHashTable() {
			hashTable.Columns.Clear();
			DataGridViewTextBoxColumn cl1 = new DataGridViewTextBoxColumn { Width = 80 };
			cl1.HeaderText = "Bits";
			cl1.Name = "Bits";

			hashTable.Columns.Add(cl1);

			DataGridViewTextBoxColumn cl2 = new DataGridViewTextBoxColumn { Width = 80 };
			cl2.HeaderText = "Box Address";
			cl2.Name = "Box Address";

			hashTable.Columns.Add(cl2);

			for (int i = 0; i < 64; i++) {
				hashTable.Rows.Add(-1, -1);
			}

			prefix = BitConverter.ToInt32(index.ToArray(), key.HashAdrsOnFile);
		}

		/* Actualiza todas las cajas de la hash dinamica, mostrando las que fueron creadas. Las muestra dinámicamente
		 * en un groupbox, y son varios datagrid */ 
		private void UpdateHashBoxes() {
			byte[] indexPrint = index.ToArray();
			long adrs = key.HashAdrsOnFile + 4, prevBxAdrs = -1;
			int yStart = 10;
			panel1.Controls.Clear();
			for (int i = 0; i < Math.Pow(2, prefix); i++) {
				long bxAdrs = BitConverter.ToInt64(indexPrint, (int)adrs + 64);
				if (bxAdrs == prevBxAdrs) {
					continue;
				}

				prevBxAdrs = bxAdrs;
				if (bxAdrs != -1) {
					Label l1 = new Label {
						Text = bxAdrs.ToString(),
						Location = new Point(200, yStart + 5),
					};
					TextBox tb = new TextBox {
						ReadOnly = true,
						Text = BitConverter.ToInt32(indexPrint, (int)bxAdrs).ToString(), // asignar un prefijo a cada uno
						Location = new Point(10, yStart),
						Width = 60,
						TextAlign = HorizontalAlignment.Center
					};
					yStart += 20;
					DataGridView dg = new DataGridView {
						Location = new Point(10, yStart), //30
						Name = "1",
						Size = new Size(220, 150),
						ColumnHeadersVisible = false,
						AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells,
						ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize,
						Visible = true
					};

					DataGridViewTextBoxColumn cl1 = new DataGridViewTextBoxColumn { Width = 80 };
					cl1.HeaderText = "Key";
					cl1.Name = "Key";

					dg.Columns.Add(cl1);

					DataGridViewTextBoxColumn cl2 = new DataGridViewTextBoxColumn { Width = 80 };
					cl2.HeaderText = "Register Address";
					cl2.Name = "Register Address";

					dg.Columns.Add(cl2);

					yStart += 160;
					//long datasAdrs = BitConverter.ToInt64(indexPrint, (int)adrs + 64);
					bxAdrs += 4;
					int data = BitConverter.ToInt32(indexPrint, (int)bxAdrs);
					long regAdrs = BitConverter.ToInt32(indexPrint, (int)bxAdrs + 4);
					int count = 0;
					while (/*data != -1 &&*/ count < boxSize) { // *****sin count, imprimir dinamicamente*****
						dg.Rows.Add(data, regAdrs);
						count++;
						if (count == boxSize) {
							break;
						}
						bxAdrs += 12;
						data = BitConverter.ToInt32(indexPrint, (int)bxAdrs);
						regAdrs = BitConverter.ToInt32(indexPrint, (int)bxAdrs + 4);
					}

					panel1.Controls.Add(tb);
					panel1.Controls.Add(dg);
					panel1.Controls.Add(l1);
					adrs += 72;
				}
			}
		}

		/* Actualiza la tabla principal de la tabla hash, muestra los bits y la direccion de la caja de cada bit */
		private void UpdateHashTable() {
			byte[] indexPrint = index.ToArray();
			hashTable.Rows.Clear();
			long adrs = key.HashAdrsOnFile + 4;
			if (prefix == 0) {
				long hashAdrs = BitConverter.ToInt64(indexPrint, (int)adrs + 64);
				hashTable.Rows.Add("", hashAdrs);
			}
			else {
				for (int i = 0; i < Math.Pow(2, prefix); i++) {
					string bit = Encoding.UTF8.GetString(indexPrint, (int)adrs, prefix);
					long hashAdrs = BitConverter.ToInt64(indexPrint, (int)adrs + 64);
					hashTable.Rows.Add(bit, hashAdrs);
					adrs += 72;
				}
			}
			textBox1.Text = prefix.ToString();
		}
	}
}
