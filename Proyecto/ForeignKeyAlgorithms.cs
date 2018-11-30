using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Proyecto {
	public partial class DataBase {
		private const int pkms = 200;

		/* Crea una estructura básica del índice seccundario. Crea una lista de 50 elementos
		 * con el tamaño de la clave y el apuntador a la lista secundaria de esa clave */
		private void CreateFKStructure() {
			for (int i = 0; i < pkms; i++) {
				index.AddRange(new byte[key.FKSize]);
				index.AddRange(BitConverter.GetBytes((long)-1));
			}
		}
		/* Regresa la dirección del indice donde se va a insertar la clave de busqueda.
		 * Se conoce previamente la clave, si no se encuentra, entonces regresa falso.
		 * Regresa true si se pudo insertar, si no regresa false */
		private bool InsertForeignKey(string keyName, ref long idxAdrs) {
			long aux = -1, blockAdrs = -1;
			// Si no se encuentra la lista secundaria
			if (!FindFK(keyName, ref idxAdrs, ref blockAdrs, -1, false)) {
				ShiftFKDown(idxAdrs, 8, blockAdrs);

				aux = index.Count;
				CreateForeignLargeList();
				// Enlaza la sublista con la lista principal
				if (key.FKIsChar) {
					ReplaceBytes(index, idxAdrs, Encoding.UTF8.GetBytes(keyName));
				}
				else {
					ReplaceBytes(index, idxAdrs, BitConverter.GetBytes(Convert.ToInt64(keyName)));
				}
				ReplaceBytes(index, idxAdrs + key.FKSize, BitConverter.GetBytes(aux));

				idxAdrs = aux;
			}
	
			return true;
		}

		private void CompleteFK(long idxAdrs, long newAdrs) {
			ReplaceBytes(index, idxAdrs, BitConverter.GetBytes(newAdrs));
		}

		private void CreateForeignLargeList() {
			for (int i = 0; i < 50; i++) {
				index.AddRange(BitConverter.GetBytes((long)-1));
			}
		}

		/* Busca en el archivo de índice la clave foranea para insertar un dato. Primero
		 * busca en la lista de 50, si se encuentra el índice con la clave de búsqueda. Si
		 * no se encuentra, regresa la dirección del archivo de índice donde va a estar
		 * Si se encuentra, regresa la dirección del archivo de indice del ultimo registro
		 * con esa clave de búsqueda*/
		private bool FindFK(string keyName, ref long idxAdrs, ref long blockAdrs, long delReg, bool del) {
			byte[] indexPrint = index.ToArray();
			byte[] registerPrint = register.ToArray();
			int keyInt = -1;
			if (!key.FKIsChar) {
				keyInt = Convert.ToInt32(keyName);
			}

			long largeAdrs = key.FKAdrsOnFile;
			long subListAdrs =  BitConverter.ToInt64(indexPrint, (int)largeAdrs + key.FKSize);

			if (key.FKIsChar) {
				//Busca en la lista principal de 50 la clave
				while (subListAdrs != -1) {
					string name = Encoding.UTF8.GetString(indexPrint, (int)largeAdrs, key.FKSize).Replace("~", "").TrimEnd('\0');
					// Si se encuentra busca en la sublista del elemento de la lista
					if (string.Compare(name, keyName) == 0) {
						long mediumAdrs = subListAdrs;
						long regAdrs = BitConverter.ToInt64(indexPrint, (int)mediumAdrs);
						// Mientras no encuentre un registro
						if (del && regAdrs == delReg) {
							idxAdrs = mediumAdrs;
							blockAdrs = mediumAdrs;
							return true;
						}
						while (regAdrs != -1) {
							mediumAdrs += 8;
							regAdrs = BitConverter.ToInt64(indexPrint, (int)mediumAdrs);
							if (del && regAdrs == delReg) {
								idxAdrs = mediumAdrs;
								blockAdrs = subListAdrs;
								return true;
							}
						}
						idxAdrs = mediumAdrs;
						blockAdrs = subListAdrs;
						return true;
					}
					else {
						if (string.Compare(name, keyName) > 0) {
							idxAdrs = largeAdrs;
							blockAdrs = largeAdrs;
							return false;
						}
					}
					largeAdrs += key.FKSize + 8;
					subListAdrs = BitConverter.ToInt64(index.ToArray(), (int)largeAdrs + key.FKSize);
				}
			}
			else {
				while (subListAdrs != -1) {
					int name = BitConverter.ToInt32(indexPrint, (int)largeAdrs);
					if (name == Convert.ToInt32(keyName)) {
						long mediumAdrs = subListAdrs;
						long regAdrs = BitConverter.ToInt64(indexPrint, (int)mediumAdrs);
						// Mientras no encuentre un registro
						if (del && regAdrs == delReg) {
							idxAdrs = mediumAdrs;
							blockAdrs = largeAdrs;
							return true;
						}
						while (regAdrs != -1) {
							mediumAdrs += 8;
							regAdrs = BitConverter.ToInt64(indexPrint, (int)mediumAdrs);
							if (del && regAdrs == delReg) {
								idxAdrs = mediumAdrs;
								blockAdrs = subListAdrs;
								return true;
							}
						}
						idxAdrs = mediumAdrs;
						blockAdrs = subListAdrs;
						return true;
					}
					else {
						if (name < Convert.ToInt32(keyName)) {
							idxAdrs = largeAdrs;
							blockAdrs = largeAdrs;
							return false;
						}
					}


					largeAdrs += key.FKSize + 8;
					subListAdrs = BitConverter.ToInt64(indexPrint, (int)largeAdrs + key.FKSize);
				}
			}
			idxAdrs = largeAdrs;
			return false;
		}

		// Recorre la lista principal hacia abajo
		private void ShiftFKDown(long indAux, int pointer, long blockDir) {
			if (indAux == -1) {
				return;
			}
			index.RemoveRange((int)key.FKAdrsOnFile + ((key.FKSize + pointer) * (pkms - 1)), key.FKSize + pointer);
			index.InsertRange((int)indAux, new byte[key.FKSize + pointer]);
		}

		// Recorre la lista principal hacia abajo
		private void ShiftMainFKUp(long indAux) {
			if (indAux == -1) {
				return;
			}
			index.InsertRange(key.FKAdrsOnFile + ((key.FKSize + 8) * pkms - 1), new byte[key.FKSize + 8]);
			index.RemoveRange((int)indAux, key.FKSize + 8);
		}
	   // Recorre la lista secundaria hacia arriba
		private void ShiftSecondFKUp(long indAux, long blockDir) {
			if (indAux == -1) {
				return;
			}
			index.InsertRange((int)blockDir + 8 * 49, new byte[8]);
			index.RemoveRange((int)indAux, 8);
		}
	}
}