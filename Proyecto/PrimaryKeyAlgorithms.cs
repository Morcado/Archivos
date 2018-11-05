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

        /* Regresa el índice en el archivo de índices donde va la direccción del registro
         * que se va a insertarPrimero busca el índice en la
         * lista de los 10 (26). Al encontrar el índice donde debe de ir, tiene que 
         * reordenar los índices en orden*/
        private bool InsertPrimaryKey(string keyName, ref long prevIdxAdrs, ref long idxAdrs, ref long blockAdrs) {
            // Busca la llave para ver si no se encuentra, si se encuentra entonces
            // No la agrega. 

            /* Si el índice no existe, inserta la clave de búsqueda sin la dirección del
             * registro */
            long prevBlock = -1;
            //if (!FindPKKey(keyName, ref auxRegAdrs, ref blockAdrs)) {
            if (!FindPK(keyName, ref prevBlock, ref idxAdrs, ref blockAdrs)) { 
                /* Regresa la dirección del dato de la dirección del registro de datos */

                // Si es -1, entonces no existe la lista de 100, y la tiene que crear y enlazar
                if (idxAdrs == -1) {
                    idxAdrs = index.Count;
                    CreatePrimaryLargeList();
                    if (!key.PKIsChar) {
                        // enlaza la sublista con la lista prrincipal
                        ReplaceBytes(index, (Convert.ToInt64(keyName[0] - 48 - 1) * 12) + 4, BitConverter.GetBytes(idxAdrs));
                    }
                    else {
                        // Enlaza la sublista con la lista principal
                        ReplaceBytes(index, (Convert.ToInt64(keyName[0] - 97) * 10) + 2, BitConverter.GetBytes(idxAdrs));
                    }
                }
                // Sino, entonces se van a recorrer los atributos. Si el ant es el ultimo
                // entonces shift down no hace nada, y solo avanza al siguiente
                else {
                    ShiftDown(idxAdrs, blockAdrs);
                }
                // Buscar el registro anterior, usando el prevBlock
                if (blockAdrs == -1 && prevBlock != -1) {
                    prevIdxAdrs = GetLastPK(prevBlock);
                }
                else {
                    if (idxAdrs != blockAdrs) {
                        prevIdxAdrs = idxAdrs - 8 - key.PKSize;
                    }
                    else {
                        if (prevBlock == -1) {
                            prevIdxAdrs = -1;

                        }
                        // Inserta el principio, busca prevNBock
                    }
                }

                InsertHalfKey(keyName, idxAdrs);
                return true;
            }
            return false;
        }
        
        // Busca la ultima llave de el bloque de llaves especificado
        private long GetLastPK(long block) {
            byte[] indexP = index.ToArray();
            for (int i = 0; i < 100; i++, block += key.PKSize + 8) {
                if (BitConverter.ToInt64(indexP, (int)block + key.PKSize) == -1) {
                    return block - key.PKSize - 8;
                }
            }
            return -1;
        }

        private void CompleteKey(long idxAdrs, long regAdrs) {
            ReplaceBytes(index, idxAdrs + key.PKSize, BitConverter.GetBytes(regAdrs));
        }

        /* Inserta media llave de clave primaria, se proporciona la direccion y la clave, se 
          * queda pendiente la direccion del registro */
        private void InsertHalfKey(string keyName, long idxAdrs) {
            if (key.PKIsChar) {
                List<byte> cad = Encoding.UTF8.GetBytes(keyName).ToList();
                for (int i = cad.Count; i < key.PKSize; i++) {
                    cad.Add(Convert.ToByte('~'));
                }
                ReplaceBytes(index, idxAdrs, cad.ToArray());
            }
            else {
                int insertDat = Convert.ToInt32(keyName);
                ReplaceBytes(index, idxAdrs, BitConverter.GetBytes(insertDat));
            }
        }

        // Busca una clave del indice primario para insertar registro
        // prevBlock != -1 y idxAdrs == 1, no existe el bloque de 100
        // prevBlock == -1 y idxAdrs != -1, existe en un bloque
        private bool FindPK(string keyName, ref long prevBlock, ref long idxAdrs, ref long block) {
            // Incremento del ciclo, y tamaño del indice con apuntador (char = 10, int = 12)
            int inc = key.PKIsChar ? 26 : 9, idxTam = key.PKIsChar ? 10 : 12;
            int v = key.PKIsChar ? 97 : 48;

            long regAdrs = -1;
            long prevIdxAdrs = -1;

            byte[] indexP = index.ToArray();

            for (int i = 0; i < inc; i++) {
                idxAdrs = i * idxTam;
                block = BitConverter.ToInt64(indexP, (int)idxAdrs + 4);
                if (block != -1) {
                    if (i == keyName[0] - v - 1) {
                        idxAdrs = BitConverter.ToInt64(indexP, (int)idxAdrs + 4);
                        if (key.PKIsChar) {
                            for (int j = 0; j < 100; j++, prevIdxAdrs = idxAdrs, idxAdrs += key.PKSize + 8) {
                                var s = Encoding.UTF8.GetString(indexP, (int)idxAdrs, key.PKSize);
                                if (!s.Any(x => Char.IsLetter(x))) {
                                    return false;
                                }
                                if (String.Compare(keyName, s) == 0) {
                                    return true;
                                }
                                if (String.Compare(keyName, s) < 0) {
                                    return false;
                                }
                            }
                        }
                        else {
                            for (int j = 0; j < 100; j++, prevIdxAdrs = idxAdrs, idxAdrs += key.PKSize + 8) {
                                var s = BitConverter.ToInt32(indexP, (int)idxAdrs);
                                if (s == -1) {
                                    return false;
                                }
                                if (Convert.ToInt32(keyName) == s) {
                                    return true;
                                }
                                if (Convert.ToInt32(keyName) < s) {
                                    return false;
                                }
                            }
                        }
                    }
                    prevBlock = block;
                }
                if (i == keyName[0] - v - 1) {
                    idxAdrs = -1;
                    break;
                }
                
            }

            return false;
        }

        /* Método que encuentra una clave de búsqueda si se encuentra y regresar 
         * Si la llave no se encuentra, regresar el índice donde  se va a ubicar el registro a insertar 
         * Si a dirección es -1 y el resultado es false, entonces tiene que crear un
         * la estructura de 100 de esa dirección y actualizar */
        private bool FindPKKey(string keyName, ref long idxAdrs, ref long block) {
            int keyInt = -1;
            long largeAdrs = -1;
            if (!key.PKIsChar) {
                keyInt = Convert.ToInt32(keyName);
            }
            largeAdrs = !key.PKIsChar
                ? BitConverter.ToInt64(index.ToArray(), ((keyName[0] - 48 - 1) * 12) + 4)
                : BitConverter.ToInt64(index.ToArray(), ((keyName[0] - 97) * 10) + 2);

            /* Verificar con el caracter, modificar, pendiente */
            // Si hay una lista secundaria en la lista principal
            long blockD = -1;
            if (largeAdrs != -1) {
                blockD = largeAdrs;
                if (key.PKIsChar) {
                    long ant = -1;
                    for (int j = 0; j < 100; j++, largeAdrs += key.PKSize + 8) {
                        
                        string valor = Encoding.UTF8.GetString(index.ToArray(), (int)largeAdrs, key.PKSize).Replace("~", "");
                        // Si la cadena es mayor que la que ya estaba
                        if (String.Compare(keyName, valor) < 0) {
                            //ShiftDown();
                            idxAdrs = ant == -1 ? blockD : ant;
                            block = blockD;
                            return false;
                        }
                        else {
                            // Si ya se encuentra la clave de busqueda, no se hace nada
                            if (keyName == valor) {
                                return true;
                            }
                            else {
                                // Si llego a un valor vacio, entonces solo inserta al final
                                if (!valor.Any(x => Char.IsLetter(x))){
                                    block = -1;
                                    idxAdrs = largeAdrs;
                                    return false;
                                }
                            }
                        }
                        ant = largeAdrs;
                    }
                }
                else {
                    long ant = -1;
                    for (int j = 0; j < 100; j++, largeAdrs += key.PKSize + 8) {

                        int valor = BitConverter.ToInt32(index.ToArray(), (int)largeAdrs);

                        // Regresa la dirección del índice que ya existe
                        if (keyInt < valor) {
                            idxAdrs = ant == -1 ? blockD : ant;
                            block = blockD;
                            return false;
                        }
                        else {
                            if (valor == keyInt) {
                                return true;
                            }
                            else {
                                // Regresa la dirección del índice que no existe
                                if (valor == -1) {
                                    idxAdrs = largeAdrs;
                                    return false;
                                }
                            }
                        }
                        ant = largeAdrs;
                    }
                }
            }
            return false;
        }

        // Mueve una lista de 100 indices hacia abajo comenzando del índice dado
        private void ShiftDown(long indAux, long blockDir) {

            if (key.PKIsChar) {
                string value = Encoding.UTF8.GetString(index.ToArray(), (int)indAux, key.PKSize).Replace("~", "");
                // Si el valor es vacio, es porque va a insertar en ese lugar
                if (value.Any(x => char.IsLetter(x))) {
                    return;
                }
                // Si el valor no es vacio, es porque el indice va en la siguiente posicion
                else {
                    index.RemoveRange((int)blockDir + (key.PKSize + 8) * 99, key.PKSize + 8);
                    //long indShift = indAux + key.PKSize + 8;
                    index.InsertRange((int)indAux, new byte[key.PKSize + 8]);
                    //string sTemp1 = Encoding.UTF8.GetString(index.ToArray(), (int)indShift, key.PKSize);
                    //long iTemp1 = BitConverter.ToInt64(index.ToArray(), )
                }
            }
            else {
                int value = BitConverter.ToInt32(index.ToArray(), (int)indAux);
                if (value == -1) {
                    return;
                }
                else {
                    index.RemoveRange((int)blockDir + (key.PKSize + 8) * 99, key.PKSize + 8);
                    index.InsertRange((int)indAux, new byte[key.PKSize + 8]);
                }
            }
        }

        // Mueve una lista de 100 indices hacia arriba comenzando del índice dado
        private void ShiftUp(long indAux, long blockDir) {
            //if (key.PKIsChar) {
            //    string value = Encoding.UTF8.GetString(index.ToArray(), (int)indAux, key.PKSize).Replace("~", "");
            //    // Si el valor es vacio, es porque va a insertar en ese lugar, no hace nada
            //    if (value.Any(x => char.IsLetter(x))) {
            //        return;
            //    }
            //    // Si el valor no es vacio, es porque el indice va en la siguiente posicion
            //    else {
            //        index.InsertRange((int)blockDir + (key.PKSize + 8) * 99);
            //        //long indShift = indAux + key.PKSize + 8;
            //        index.RemoveRange((int)indAux, new byte[key.PKSize + 8], key.PKSize + 8);
            //        //string sTemp1 = Encoding.UTF8.GetString(index.ToArray(), (int)indShift, key.PKSize);
            //        //long iTemp1 = BitConverter.ToInt64(index.ToArray(), )
            //    }
            //}
            //else {
            //    int value = BitConverter.ToInt32(index.ToArray(), (int)indAux);
            //    if (value == -1) {
            //        return;
            //    }
            //    else {
            //        index.RemoveRange((int)blockDir + (key.PKSize + 8) * 99, key.PKSize + 8);
            //        index.InsertRange((int)indAux, new byte[key.PKSize + 8]);
            //    }
            //}
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
