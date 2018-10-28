using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Proyecto {
    public partial class DataBase {
        /* Crea la estructura en el archivo de indice básica para índice
         * primario, crea 10 registros primarios de índice si el índice es
         * de tipo "Entero" o crea 26 si es de tipo "Cadena" */
        private void CreatePKStructure() {
            /* Si el índice es de entero, crea 10 registros y los agrega al idx*/
            if (!key.PKIsChar) {
                for (int i = 0; i < 9; i++) {
                    index.AddRange(BitConverter.GetBytes(i + 1));
                    index.AddRange(BitConverter.GetBytes((long)-1));
                }
            }
            /* Si no, agrega 26 registros al idx*/
            else {
                char letra = 'A';
                for (int i = 0; i < 26; i++) {
                    index.AddRange(BitConverter.GetBytes(letra));
                    index.AddRange(BitConverter.GetBytes((long)-1));
                    letra++;
                }
            }
        }

        /* Agrega un índice primario al archivo de índices. Primero busca el índice en la
         * lista de los 10 (26). Al encontrar el índice donde debe de ir, tiene que 
         * reordenar los índices en orden*/
        private bool InsertPrimaryKey(string keyName, List<string> forms, List<char> types, List<int> sizes) {
            // Busca la llave para ver si no se encuentra, si se encuentra entonces
            // No la agrega. 
            long registerAdrs = -1, auxRegAdrs = -1;

            /* Si el índice no existe, inserta la clave de búsqueda sin la dirección del
             * registro */
            if (!FindPKKey(keyName, ref auxRegAdrs)) {
            /* Regresa la dirección del dato de la dirección del registro de datos */
                int insertDat = -1;
                if (!key.PKIsChar) {
                    insertDat = Convert.ToInt32(keyName);
                }
                if (auxRegAdrs == -1) {
                    auxRegAdrs = index.Count + key.PKSize;
                    long blockAdrs = index.Count;
                    CreatePrimaryLargeList();

                    if (!key.PKIsChar) {
                        ReplaceBytes(index, Convert.ToInt64(keyName[0] - 48 - 1) * 12 + 4, BitConverter.GetBytes(blockAdrs));
                    }
                    else {
                        ReplaceBytes(index, Convert.ToInt64(keyName[0] - 97) * 10 + 2, BitConverter.GetBytes(blockAdrs));
                    }
                }
                else {
                    auxRegAdrs += key.PKSize;
                }

                if (!key.PKIsChar) {
                    ReplaceBytes(index, auxRegAdrs - key.PKSize, BitConverter.GetBytes(insertDat));
                }
                else {
                    List<byte> cad = Encoding.UTF8.GetBytes(keyName).ToList();
                    for (int i = cad.Count; i < key.PKSize; i++) {
                        cad.Add(Convert.ToByte('~'));
                    }
                    ReplaceBytes(index, auxRegAdrs - key.PKSize, cad.ToArray());
                }

                if (key.searchKey) {
                    registerAdrs = AddOrderedEntry(forms, types, sizes);
                    WriteDictionary();
                    UpdateEntityTable();
                }
                else {
                    registerAdrs = AddSecuentialEntry(forms, types, sizes);
                }

                /* Agrega la dirección del registro al índice */
                ReplaceBytes(index, auxRegAdrs, BitConverter.GetBytes(registerAdrs));
                return true;
            }
            return false;
        }
        
        /* Método que encuentra una clave de búsqueda si se encuentra y regresar 
         * Si la llave no se encuentra, regresar el índice donde 
         * se va a ubicar el registro a insertar 
         * Si a dirección es -1 y el resultado es false, entonces tiene que crear un
         * la estrucctura de 100 de esa direccíón y actualizar*/
        private bool FindPKKey(string keyName, ref long idxAdrs) {

            int firstKeyInt = -1, keyInt = -1;
            long largeAdrs = -1;
            if (!key.PKIsChar) {
                firstKeyInt = Convert.ToInt32(keyName[0]) - 48;
                keyInt = Convert.ToInt32(keyName);
            }

            if (!key.PKIsChar) {
                largeAdrs = BitConverter.ToInt64(index.ToArray(), (keyName[0] - 48 - 1) * 12 + 4);
            }
            else {
                largeAdrs = BitConverter.ToInt64(index.ToArray(), (keyName[0] - 97) * 10 + 2);
            }
                        
            // Si hay una lista secundaria en la lista principal
            if (largeAdrs != -1) {
                for (int j = 0; j < 100; j++, largeAdrs += key.PKSize + 8) {
                    int valor = BitConverter.ToInt32(index.ToArray(), (int)largeAdrs);
                    // Regresa la dirección del índice que ya existe
                    if (valor == keyInt) {
                        idxAdrs = BitConverter.ToInt32(index.ToArray(), (int)largeAdrs + key.PKSize);
                        return true;
                    }
                    // Regresa la dirección del índice que no eciste
                    if (valor == -1) {
                        idxAdrs = largeAdrs;
                        return false;
                    }
                }
            }
            return false;
        }



        /* Crea una lista de 100 y la agrega al final */
        private void CreatePrimaryLargeList() {
            // Crea 100 elementos enteros (4 + 8) bytes
            if (!key.PKIsChar) {
                for (int i = 0; i < 100; i++) {
                    index.AddRange(BitConverter.GetBytes(-1));
                    index.AddRange(BitConverter.GetBytes((long)-1));
                }
            }
            // Crea 100 elementos cadena (tamaño + 8) bytes
            else {
                for (int i = 0; i < 100; i++) {
                    index.AddRange(new byte[key.PKSize]);
                    index.AddRange(BitConverter.GetBytes((long)-1));
                }
            }
        }
    }
}
