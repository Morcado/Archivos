using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Proyecto {
	public partial class DataBase {
		private const int boxSize = 20;

		private void CreateHashStructure() {
			index.AddRange(BitConverter.GetBytes(prefix)); // Agrega el prefijo con 0
			// Bytes
			for (int i = 0; i < 64; i++) {
				index.AddRange(new byte[64]);
				index.AddRange(BitConverter.GetBytes((long)-1));
			}
			ReplaceBytes(index, key.HashAdrsOnFile + 4, Encoding.UTF8.GetBytes("")); // El primer valor es vacio
			long bxAdrs = CreateBox();
			ReplaceBytes(index, key.HashAdrsOnFile + 68, BitConverter.GetBytes(bxAdrs)); // El primer valor es la primera caja
		}

		/* Crea una caja de 5 elementos y regresa la dirección de la caja */
		private long CreateBox() {
			long boxAdrs = index.Count;
			index.AddRange(BitConverter.GetBytes(0));  // Prefijo
			// Datos y direcciones de los registros
			for (int i = 0; i < boxSize; i++) { // ******50******
				index.AddRange(BitConverter.GetBytes(-1)); // ******-1******
				index.AddRange(BitConverter.GetBytes((long)-1));
			}

			return boxAdrs;
		}

		// Establece el prefijo de la caja con el nunmero especificado
		private void SetBoxPrefix(int pref, long bxAdrs) {
			ReplaceBytes(index, bxAdrs, BitConverter.GetBytes(pref));
		}

		// Obtiene el prefijo de la caja especificada
		private int GetBoxPrefix(long bxAdrs) {
			return BitConverter.ToInt32(index.ToArray(), (int)bxAdrs);
		}

		// Limpia una caja, dejando los valores en -1
		private void ClearBox(long bxAdrs, List<int> entries, List<long> addresses) {
			long aux = bxAdrs + 4;
			int cont = 0;
			int key = BitConverter.ToInt32(index.ToArray(), (int)aux);
			long rAdrs = BitConverter.ToInt64(index.ToArray(), (int)aux + 4);
			while (key != -1) {
				ReplaceBytes(index, aux, BitConverter.GetBytes(-1));
				ReplaceBytes(index, aux + 4, BitConverter.GetBytes((long)-1));

				aux += 12;
				cont++;

				entries.Add(key);
				addresses.Add(rAdrs);
				if (cont == boxSize) {
					break;
				}
				key = BitConverter.ToInt32(index.ToArray(), (int)aux);
				rAdrs = BitConverter.ToInt64(index.ToArray(), (int)aux + 4);
			}
		}

		/* Duplica o desdobla la tabla de los bits de la asociación dinámica. Se duplica cada valor con sus respectivos
		 * apuntadores al doble y se van agregando 0 y 1 alternadamente al final de cada uno. Después se actualiza el 
		 * prefijo sumándole 1 */
		private void UnfoldBits() {
			// Cuando no hay nada de prefijos, se agrega solo 0 y 1
			if (prefix == 0) {
				prefix++;
				// Se duplica primero el unico valor de la tabla con su apuntador
				ReplaceBytes(index, key.HashAdrsOnFile + 4 + 72, index.GetRange(key.HashAdrsOnFile + 4, 72).ToArray());
				// Se renombran los bits de la tabla a 0 y 1
				ReplaceBytes(index, key.HashAdrsOnFile + 4, Encoding.UTF8.GetBytes("0"));
				ReplaceBytes(index, key.HashAdrsOnFile + 4 + 72, Encoding.UTF8.GetBytes("1"));
				ReplaceBytes(index, key.HashAdrsOnFile, BitConverter.GetBytes(prefix));// Se actualiza el prefijo
			}
			// Cuando hay por lo menos dos valores en la tabla se puede duplicar
			else {
				/* Se duplican los valores copiandolos dos veces al final de la tabla de nuevo tamaño */
				int sCont = (int)Math.Pow(2, prefix++);
				int lCont = (int)Math.Pow(2, prefix);
				for (int i = lCont - 1; i >= 0; i -= 2) {
					ReplaceBytes(index, (72 * i) + 4, index.GetRange(((sCont - 1) * 72) + 4, 72).ToArray());
					ReplaceBytes(index, (72 * (i - 1)) + 4, index.GetRange(((sCont - 1) * 72) + 4, 72).ToArray());
					sCont--;
				}
				
				bool stch = true; ;
				for (int i = 0; i < lCont; i++) { // Se agregan 0 y 1 alternados en cada linea
					string bites = Encoding.UTF8.GetString(index.ToArray(), (72 * i) + 4, prefix - 1) + (stch ? "0" : "1");
					ReplaceBytes(index, (72 * i) + 4, Encoding.UTF8.GetBytes(bites));
					stch = !stch;
				}
				ReplaceBytes(index, key.HashAdrsOnFile, BitConverter.GetBytes(prefix)); // Actualiza el prefijo
			}
			textBox1.Text = prefix.ToString();
		}

		// Inserta una hash, primero busca el cajon donde debe estar
		private bool InsertHashKey(string v, ref long hsAdrs) {
			// Convierte un digito de un numero en binario
			int c = 0;
			string binary = IntToBinaryString(v[c++]);
			while (binary.Length < prefix) {
				binary += IntToBinaryString(v[c++]);
			}
			/* Implementar un ciclo cuando se requieran mas digitos en binario */

			if (prefix == 0) {
				long bxAdrs = BitConverter.ToInt64(index.ToArray(), key.HashAdrsOnFile + 68);
				if (!FindInBox(ref hsAdrs, bxAdrs, v)) {
					if (BoxIsFull(bxAdrs)) {
						UnfoldBits();
						long nBox = CreateBox();
						ReplaceBytes(index, key.HashAdrsOnFile + 68 + 72, BitConverter.GetBytes(nBox));
						SetBoxPrefix(1, bxAdrs);
						SetBoxPrefix(1, nBox);

						List<int> entries = new List<int>();
						List<long> addresses = new List<long>();
						ClearBox(bxAdrs, entries, addresses);

						ReinsertHash(entries, addresses, bxAdrs, nBox, "0");
						hsAdrs = -1;
						InsertHashKey(v, ref hsAdrs);
					}
					else {
						InsertInBox(v, hsAdrs, bxAdrs);
						return true;
					}
				}
				return false;
			}
			else {
				long adrs = key.HashAdrsOnFile + 4;
				for (int i = 0; i < Math.Pow(2, prefix); i++) {
					string hashKey = Encoding.UTF8.GetString(index.ToArray(), (int)adrs, prefix);//BitConverter.ToInt32(index.ToArray(), (int)adrs);
					if (binary.IndexOf(hashKey) == 0) {
						// Obtiene la dirección de la caja
						long bxAdrs = BitConverter.ToInt64(index.ToArray(), (int)adrs + 64);
						if (!FindInBox(ref hsAdrs, bxAdrs, v)) {
							// CreateBox
							if (BoxIsFull(bxAdrs)) {
								// Contar cuantos apuntadores apuntan a la caja actual
								int boxPrefix = GetBoxPrefix(bxAdrs);
								if (boxPrefix == prefix) {
									UnfoldBits();
									long newBoxAdrs = CreateBox();
									// Hace que apunte a la nueva caja

									hashKey += "0";
									long nAdrs = key.HashAdrsOnFile + 4;
									for (int j = 0; j < Math.Pow(2, prefix); j++) {
										string tmp = Encoding.UTF8.GetString(index.ToArray(), (int)nAdrs, prefix);
										if (tmp.IndexOf(hashKey) == 0) {
											break;
										}
										nAdrs += 72;
									}
									ReplaceBytes(index, nAdrs + 64 + 72, BitConverter.GetBytes(newBoxAdrs));
									SetBoxPrefix(prefix, bxAdrs);
									SetBoxPrefix(prefix, newBoxAdrs);
									/*Hacer que el apuntador de abajo
									apunte a la nueva caja*/

									List<int> entries = new List<int>();
									List<long> addresses = new List<long>();
									//string bxBin = Encoding.UTF8.GetString(index.ToArray(), (int)adrs + 72 + 72, prefix);
									//string nbxString = Encoding.UTF8.GetString(index.ToArray(), (int), prefix);

									ClearBox(bxAdrs, entries, addresses);

									ReinsertHash(entries, addresses, bxAdrs, newBoxAdrs, hashKey);

									hsAdrs = -1;
									InsertHashKey(v, ref hsAdrs);
									return true;
								}
								else {
									int boxPointer = GetPointers(adrs, bxAdrs);

									//int bPrefix = GetBoxPrefix(bxAdrs);
									long newBoxAdrs = CreateBox();
									SetBoxPrefix(boxPrefix + 1, bxAdrs);
									SetBoxPrefix(boxPrefix + 1, newBoxAdrs);

									long secondHalf = adrs + (72 * boxPointer / 2);
									for (int k = 0; k < boxPointer / 2; k++) {
										ReplaceBytes(index, secondHalf + 64, BitConverter.GetBytes(newBoxAdrs));
										secondHalf += 72;
									}

									List<int> entries = new List<int>();
									List<long> addresses = new List<long>();
									string bxBin = Encoding.UTF8.GetString(index.ToArray(), (int)adrs, prefix - boxPrefix + 1);
									//string nbxString = Encoding.UTF8.GetString(index.ToArray(), (int), prefix);

									ClearBox(bxAdrs, entries, addresses);

									ReinsertHash(entries, addresses, bxAdrs, newBoxAdrs, hashKey);
									hsAdrs = -1;
									InsertHashKey(v, ref hsAdrs);
									return true;
								}
							}
							// Insertar de forma ordenada
							else {
								InsertInBox(v, hsAdrs, bxAdrs);
								return true;
							}
						}
						return false;
					}
					adrs += 72;
				}
			}
			return false;

		}

		private int GetPointers(long adrs, long bxAdrs) {
			int cont = 0;
			long gbAdrs = 0;
			do {
				cont++;
				gbAdrs = BitConverter.ToInt64(index.ToArray(), (int)adrs + 64);
				adrs += 72;
			} while (gbAdrs == bxAdrs);
			return cont - 1;
		}

		private void ReinsertHash(List<int> entries, List<long> addresses, long bxAdrs, long nBox, string bxString) {
			for (int i = 0; i < entries.Count; i++) {
				int c = 0;
				string bin = IntToBinaryString(entries[i].ToString()[c++]);
				while (bin.Length < bxString.Length && entries[i].ToString().Length < c) {
					bin += IntToBinaryString(entries[i].ToString()[c++]);
				}
				long hsAdrs = -1;
				if (bin.IndexOf(bxString) == 0) {
					
					FindInBox(ref hsAdrs, bxAdrs, entries[i].ToString());
					InsertInBox(entries[i].ToString(), hsAdrs, bxAdrs);
				}
				else {
					FindInBox(ref hsAdrs, nBox, entries[i].ToString());
					InsertInBox(entries[i].ToString(), hsAdrs, nBox);
				}
				CompleteHash(hsAdrs, addresses[i]);
			}

		}

		// Inserta en una caja de forma ordenada, busca el lugar donde debe de ir
		public void InsertInBox(string k, long hsAdrs, long bxAdrs) {
			int kk = Convert.ToInt32(k);
			ShiftHashDown(hsAdrs, bxAdrs);
			ReplaceBytes(index, hsAdrs, BitConverter.GetBytes(kk));
		}

		// Recorre los indices para insertar un indice de manera oerdenadaa
		private void ShiftHashDown(long hsAdrs, long boxAdrs) {

			int value = BitConverter.ToInt32(index.ToArray(), (int)hsAdrs);
			if (value == -1) {
				return;
			}

			index.RemoveRange((int)boxAdrs + (12 * (boxSize - 1)), 12);
			index.InsertRange((int)hsAdrs, new byte[12]);
		}
		/* Verifica que una caja está llena */
		private bool BoxIsFull(long bxAdrs) {
			long hsAdrs = bxAdrs + 4;
			for (int i = 0; i < boxSize; i++, hsAdrs += 12) {
				int key = BitConverter.ToInt32(index.ToArray(), (int)hsAdrs);
				if (key == -1) {
					return false;
				}
			}
			return true;
		}

		/* Convierte un digito en una cadena de binario */
		private string IntToBinaryString(char num) {
			string binary = Convert.ToString(Convert.ToInt32(num.ToString()), 2);
			while (binary.Length < 4) {
				binary = "0" + binary;
			}
			return binary;
		}

		/* Recibe un bloque de la hash para buscar una clave. Si la encuentra regresa true y la dirección del indice
		 * de la clave. La clave se busca ordenada */
		private bool FindInBox(ref long hsAdrs, long blockAdrs, string v) {

			hsAdrs = blockAdrs + 4;
			int keyToSearch = Convert.ToInt32(v);
			int data = BitConverter.ToInt32(index.ToArray(), (int)hsAdrs);
			int ount = 0;
			while (keyToSearch > data && data != -1 && ount < boxSize) {
				ount++;
				if (ount == boxSize) {
					break;
				}
				hsAdrs += 12;
				// Si encuentra el valor, regresa true
				data = BitConverter.ToInt32(index.ToArray(), (int)hsAdrs);
				if (data == keyToSearch) {
					return true;
				}
			}
			// Si no se encuentra, regresa la dirección donde debe de estar
			return false;
		}
		
		public void CompleteHash(long idxAdrs, long newAdrs) {
			ReplaceBytes(index, idxAdrs + 4, BitConverter.GetBytes(newAdrs));
		}
	}
}
