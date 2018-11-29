using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Proyecto {
	public partial class DataBase {
		/* Esta funcion busca un registro, dado el nombre. Regresa la dirección del registro 
		 * si existe, y la dirección del anterior. Si el registro no existe, regresa -1. El
		 * registro se busca dada una clave de busqueda. Sin la clave no se puede buscar nada*/
		private bool SearchRegistry(string name, ref long rIndex, ref long rAnt, bool delete, bool findWithNoKey) {
			rIndex = BitConverter.ToInt64(data.ToArray(), (int)selectedEntityAdrs + 46);
			byte[] dataR = register.ToArray();
			string keyName = "";
			int keyNum;
			/* Recorre todos los registros utilizando la clave de busqueda
			 * hasta que el siguiente0 indice es -1 */
			if (key.searchKey || findWithNoKey) {
				if (rIndex != -1 && dataR.Length > 0) {
					if (key.searchKeyIsChar) {
						keyName = Encoding.UTF8.GetString(dataR, (int)rIndex + 8 + key.searchKeyPos, key.searchKeySize).Replace("~", "");
						while (String.Compare(name, keyName) == 1 && rIndex != -1) {
							rAnt = rIndex;
							rIndex = BitConverter.ToInt64(dataR, (int)rIndex + 8 + registerSize);
							if (rIndex != -1) {
								keyName = Encoding.UTF8.GetString(dataR, (int)rIndex + 8 + key.searchKeyPos, key.searchKeySize).Replace("~", "");
							}
						}
						if (delete) {
							if (rIndex == -1) {
								return false;
							}
							return true;
						}
						return false;
					}
					else {
						int name2 = Convert.ToInt32(name);
						keyNum = BitConverter.ToInt32(dataR, (int)rIndex + 8);
						while (keyNum < name2 && rIndex != -1) {
							rAnt = rIndex;
							rIndex = BitConverter.ToInt64(dataR, (int)rIndex + registerSize + 8);
							if (rIndex != -1) {
								keyNum = BitConverter.ToInt32(dataR, (int)rIndex + 8);
							}
						}
						if (delete) {
							return true;
						}
						return false;
					}
				}
				rIndex = -1;
			}
			else {
				// Buscar sin clave de busqueda
				if (register.Count > 0) {
					long sIndex = Convert.ToInt64(name);
					while (rIndex < sIndex) {
						rAnt = rIndex;
						rIndex = BitConverter.ToInt64(register.ToArray(), (int)rIndex + registerSize + 8);
					}
					if (sIndex == rIndex) {
						return true;
					}
				}
			}
			return false;
		}

		// Recibe la direccion del registro anterior del que se va a insertar
		private long InsertRegister(List<string> output, long prevRegAdrs) {
			long newAdrs = register.Count;
			register.AddRange(BitConverter.GetBytes(newAdrs)); // Dirección del registro
			for (int i = 0; i < output.Count; i++) {
				if (types[i] == 'C') {
					// Completa el nombre de acuerdo al tamaño que tiene en el atributo
					byte[] bname = Encoding.UTF8.GetBytes(output[i]);
					register.AddRange(bname);
					for (int j = bname.Length; j < sizes[i]; j++) {
						register.Add(Convert.ToByte('~'));
					}
				}
				else {
					register.AddRange(BitConverter.GetBytes(Convert.ToInt32(output[i])));
				}
			}
			register.AddRange(BitConverter.GetBytes((long)-1));

			/* Si hay anterior, entonces el registro va en el centro o al final
			 * siempre despues del anterior */
			if (prevRegAdrs != -1) {
				long regAdrs = BitConverter.ToInt64(register.ToArray(), (int)prevRegAdrs + 8 + registerSize);
				//Enlaza si va igual que la cabeza, se actualiza la cabecera
				if (regAdrs == BitConverter.ToInt64(data.ToArray(), (int)selectedEntityAdrs + 46)) {
					ReplaceBytes(register, newAdrs + 8 + registerSize, BitConverter.GetBytes(regAdrs));
					ReplaceBytes(data, selectedEntityAdrs + 46, BitConverter.GetBytes(newAdrs));
				}
				else {
					// Si se inserta después de la cabecera, se inserta entre las entidades en las que va
					ReplaceBytes(register, prevRegAdrs + 8 + registerSize, BitConverter.GetBytes(newAdrs));
					if (regAdrs != -1) {
						long aux = BitConverter.ToInt64(register.ToArray(), (int)regAdrs);
						ReplaceBytes(register, newAdrs + 8 + registerSize, BitConverter.GetBytes(aux));
					}
				}
			}
			// Si no hay anterior, es el primer registro o va antes de la cabecera
			else {
				long oldHead = BitConverter.ToInt64(data.ToArray(), (int)selectedEntityAdrs + 46);
				ReplaceBytes(data, selectedEntityAdrs + 46, BitConverter.GetBytes(newAdrs));
				if (newAdrs != 0) {
					ReplaceBytes(register, newAdrs + 8 + registerSize, BitConverter.GetBytes(oldHead));
				}
			}
			return newAdrs;
		}

		// Recibe la dirección del registro que se va a modificar.
		private long ReplaceRegister(List<string> output, long newAdrs, long prevRegAdrs) {
			int pos = 8;
			for (int i = 0; i < types.Count; i++) {
				if (types[i] == 'C') {
					byte[] byteName = Encoding.UTF8.GetBytes(output[i]);
					List<byte> bn = byteName.ToList();
					for (int j = bn.Count; j < sizes[i]; j++) {
						bn.Add(Convert.ToByte('~'));
					}
					ReplaceBytes(register, newAdrs + pos, bn.ToArray());
				}
				else {
					ReplaceBytes(register, newAdrs + pos, BitConverter.GetBytes(Convert.ToInt32(output[i])));
				}
				pos += sizes[i];
				//ReplaceBytes();
			}

			if (prevRegAdrs != -1) {
				long head = BitConverter.ToInt64(data.ToArray(), (int)selectedEntityAdrs + 46);
				long regAdrs = BitConverter.ToInt64(register.ToArray(), (int)prevRegAdrs + 8 + registerSize);

				if (regAdrs == head) {
					// Reemplaza la cabecera del diccionario de datos de los registos
					ReplaceBytes(register, newAdrs + 8 + registerSize, BitConverter.GetBytes(regAdrs));
					ReplaceBytes(data, selectedEntityAdrs + 46, BitConverter.GetBytes(newAdrs));
				}
				else {
					ReplaceBytes(register, prevRegAdrs + 8 + registerSize, BitConverter.GetBytes(newAdrs));
					if (regAdrs != -1) {
						long aux = BitConverter.ToInt64(register.ToArray(), (int)regAdrs);
						ReplaceBytes(register, newAdrs + 8 + registerSize, BitConverter.GetBytes(aux));
					}
				}
			}
			else {
				long newRIndex = -1, newRAnt = -1;
				SearchRegistry(output[key.searchKeyAttribIndex], ref newRIndex, ref newRAnt, false, false);

				long oldHead = BitConverter.ToInt64(data.ToArray(), (int)selectedEntityAdrs + 46);

				if (newRAnt == -1) {
					ReplaceBytes(data, selectedEntityAdrs + 46, BitConverter.GetBytes(newAdrs));
					if (newAdrs != 0) {
						ReplaceBytes(register, newAdrs + 8 + registerSize, BitConverter.GetBytes(oldHead));
					}
				}
				else {
					ReplaceBytes(register, newRAnt + 8 + registerSize, BitConverter.GetBytes(newAdrs));
					ReplaceBytes(register, newAdrs + 8 + registerSize, BitConverter.GetBytes(newRIndex));
				}
			}
			return newAdrs;
		}

		/* Inserta un registro de forma ordenada en el archivo de registro de la entidad. Si el tipo de indice
		 * es primario o secundario, agrega el indice incluso si ya existe la clave de busqueda*/
		private bool AddRegister(List<string> output) {
			long rIndex = -1, rAnt = -1, newAdrs = -1;
			long prevIdxAdrs = -1, idxAdrsPK = -1, blockAdrs = -1, idxAdrsFK = -1, hsAdrs = -1;
			bool resp = false, resp2 = false;
			long currentRegAdrs = register.Count;
			
			
			if (key.PK) {
				resp = InsertPrimaryKey(output[key.PKAtribListIndex], ref prevIdxAdrs, ref idxAdrsPK, ref blockAdrs);

				// Si se inserto primario, y tiene secundario, entonces inserta el secundario
				if (resp) {
					if (key.FK) {
						resp2 = InsertForeignKey(output[key.FKAtribListIndex], ref idxAdrsFK);
					}
					// Inserta registro y inserta la dirección en el indice
					// Si es el primer registro, entonces inserta al principio

					if (key.searchKey) {
						resp = SearchRegistry(output[key.searchKeyAttribIndex], ref rIndex, ref rAnt, false, false);
						if (!resp) {
							newAdrs = InsertRegister(output, rAnt);
						}
					}
					else { 
						resp = SearchRegistry(output[key.searchKeyAttribIndex], ref rIndex, ref rAnt, false, true);

						// Esta mal, buscar el registro anterior al pasado
						if (register.Count >= 8 + registerSize + 8) {
							rAnt = register.Count - 8 - registerSize - 8;
							newAdrs = InsertRegister(output, rAnt);
						}
						else {
							newAdrs = InsertRegister(output, -1);
						}
					}
			   
					CompletePK(idxAdrsPK, newAdrs);
					if (key.FK) {
						CompleteFK(idxAdrsFK, newAdrs);
					}
					return true;
				}
				return false;
			}
			if (key.Hash) {
				// Insertar con hash
				resp = InsertHashKey(output[key.HashAtribListIndex], ref hsAdrs);
				if (resp) {
					if (key.FK) {
						resp2 = InsertForeignKey(output[key.FKAtribListIndex], ref idxAdrsFK);
					}

					if (key.searchKey) {
						if (!SearchRegistry(output[key.searchKeyAttribIndex], ref rIndex, ref rAnt, false, false)) {
							newAdrs = InsertRegister(output, rAnt);
						}
					}
					else {
						if (!SearchRegistry(output[key.searchKeyAttribIndex], ref rIndex, ref rAnt, false, true)) {
							newAdrs = InsertRegister(output, rAnt);
						}
					}
					CompleteHash(hsAdrs, newAdrs);
					if (key.FK) {
						CompleteFK(idxAdrsFK, newAdrs);
					}
					return true;
				}
			}
			if (key.FK) {
				return InsertForeignKey(output[key.FKAtribListIndex], ref idxAdrsPK);
			}

			if (key.searchKey) {
				if (key.searchKey) {
					if (!SearchRegistry(output[key.searchKeyAttribIndex], ref rIndex, ref rAnt, false, false)) {
						InsertRegister(output, rAnt);

						return true;
					}
				}
				else {
					if (!SearchRegistry(output[key.searchKeyAttribIndex], ref rIndex, ref rAnt, false, true)) {
						InsertRegister(output, rAnt);
						return true;
					}
				}
			}
			return false;
		}

		// Elimina un registro dada su clave de busqueda, utiliza la primera por defecto si no la tiene
		// Regresa la direccion del registro, y recibe la direccion del registro y del anterior
		private long DeleteRegister(long rIndex, long rAnt) {
			long next = BitConverter.ToInt64(register.ToArray(), (int)rIndex + registerSize + 8);

			// Remplaza la cabecera
			if (rIndex == BitConverter.ToInt64(data.ToArray(), (int)selectedEntityAdrs + 46)) {
				ReplaceBytes(data, selectedEntityAdrs + 46, BitConverter.GetBytes(next));
				textBoxReg.Text = next.ToString();
				UpdateEntityTable();
			}
			// Remplaza en el centro o al final
			else {
				ReplaceBytes(register, rAnt + registerSize + 8, BitConverter.GetBytes(next));
			}


			long prevBlock = -1, blockAdrs = -1, idxAdrs = -1;
			if (key.PK) {
				int ss = 8;
				string keyname = "";

				for (int i = 0; i < key.PKAtribListIndex; i++) {
					ss += sizes[i];
				}

				if (key.PKIsChar) {
					keyname = Encoding.UTF8.GetString(register.ToArray(), (int)rIndex + ss, sizes[key.PKAtribListIndex]).Replace("~", "");
				}
				else {
					keyname = BitConverter.ToInt32(register.ToArray(), (int)rIndex + ss).ToString();
				}
				if (FindPK(keyname, ref prevBlock, ref idxAdrs, ref blockAdrs)) {
					ShiftPKUp(idxAdrs, blockAdrs);
				}
			}
			idxAdrs = blockAdrs = -1;
			if (key.FK) {
				string keyname = "";
				int ss = 8;
				for (int i = 0; i < key.FKAtribListIndex; i++) {
					ss += sizes[i];
				}

				if (key.FKIsChar) {
					keyname = Encoding.UTF8.GetString(register.ToArray(), (int)rIndex + ss, sizes[key.FKAtribListIndex]).Replace("~", "");
				}
				else {
					keyname = BitConverter.ToInt32(register.ToArray(), (int)rIndex + ss).ToString();
				}

				if (FindFK(keyname, ref idxAdrs, ref blockAdrs, rIndex, true)) {
					ShiftSecondFKUp(idxAdrs, blockAdrs);
				}
			}
			if (key.Hash) {
				//Borrar hash
			}
			return rIndex;
		}

		/* Modifica un registro en el archivo de datos. Si tiene clave de búsqueda entonces verifica para 
		 * reordenar los elementos, si no tiene clave de búsqueda, entonces solamente modifica los valores en
		 * donde está */
		private bool ModifyRegister(List<string> oldReg, List<string> newData, long rIndexO, long rAntO) {
			long rIndex = -1, rAnt = -1;
			long idxAdrs = -1, blockAdrs = -1, prevIdxAdrs = -1;

			// Si el nuevo registro no está, entonces se elimina el anterior
			if (!SearchRegistry(newData[key.searchKeyAttribIndex], ref rIndex, ref rAnt, false, true)) {
				DeleteRegister(rIndexO, rAntO);
				ReplaceRegister(newData, rIndexO, rAntO);
				if (key.PK) {
					InsertPrimaryKey(newData[key.PKAtribListIndex], ref prevIdxAdrs, ref idxAdrs, ref blockAdrs);
					CompletePK(idxAdrs, rIndexO);
				}
				if (key.FK) {
					idxAdrs = -1;
					InsertForeignKey(newData[key.FKAtribListIndex], ref idxAdrs);
					CompleteFK(idxAdrs, rIndexO);
				}
				if (key.Hash) {
					//InsertHashFunc();
					//CompleteHash();
				}
				return true;
			}
			return false;
		}
	}
}
