using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Proyecto {
    public partial class DataBase {
        /* Crea la estructura en el archivo de indice básica para índice
         * primario, crea una lista principal de 9 o 26 elementos dependiendo si
         * el tipo de índice es entero o cadena */
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
         * que se va a insertar. Primero busca el índice en lista principal. Y después busca en una
         * sublista si es que tiene. Al encontrar el índice donde debe de ir, reordena los indices */
        private bool InsertPrimaryKey(string keyName, ref long prevIdxAdrs, ref long idxAdrs, ref long blockAdrs) {
            /* Si el índice no existe, inserta la clave de búsqueda sin la dirección del
             * registro */
            long prevBlock = -1;
            if (!FindPK(keyName, ref prevBlock, ref idxAdrs, ref blockAdrs)) {
                /* Regresa la dirección del indice del dato del archivo de índices */

                /* Si la dirección es -1, entonces no existe el índice y tiene que crear una
                 * sublista de 100 indices, y la enlaza con la lista principal */
                if (idxAdrs == -1) {
                    idxAdrs = index.Count;
                    CreatePrimaryLargeList();
                    if (!key.PKIsChar) { // Enlaza con la lista principal
                        ReplaceBytes(index, (Convert.ToInt64(keyName[0] - 48 - 1) * 12) + 4, BitConverter.GetBytes(idxAdrs));
                    }
                    else {
                        ReplaceBytes(index, (Convert.ToInt64(keyName[0] - 97) * 10) + 2, BitConverter.GetBytes(idxAdrs));
                    }
                }
                /* Si el indice existe, entonces recorre los índices hacia abajo dejando un
                 * lugar para el índice donde va, ordenadamente. Si el índice es*/
                else {
                    ShiftPKDown(idxAdrs, blockAdrs);
                }

                // Buscar el registro anterior, usando el bloque anterior si existe
                if (blockAdrs == -1 && prevBlock != -1) {
                    prevIdxAdrs = GetLastPK(prevBlock);
                }
                /* Si el bloque anterior no existe, entonces se checa que el indice no sea el primero
                 * Si no es el primero, el se obtiene el índice inmediato anterior */
                else {
                    if (idxAdrs != blockAdrs) {
                        prevIdxAdrs = idxAdrs - 8 - key.PKSize;
                    }
                    // Si es el primer indice (idx = block), entonces no hay anterior
                    else {
                        if (prevBlock == -1) {
                            prevIdxAdrs = -1;
                        }
                    }
                }

                // Inserta solo la clave del indice primario
                InsertHalfKey(keyName, idxAdrs);
                return true;
            }
            return false;
        }

        // Busca la ultima llave en el bloque de llaves especificado
        private long GetLastPK(long block) {
            byte[] indexP = index.ToArray();
            for (int i = 0; i < 100; i++, block += key.PKSize + 8) {
                if (BitConverter.ToInt64(indexP, (int)block + key.PKSize) == -1) {
                    return block - key.PKSize - 8;
                }
            }
            return -1;
        }

        // Inserta la dirección del registro en el índice incompleto
        private void CompletePK(long idxAdrs, long regAdrs) {
            ReplaceBytes(index, idxAdrs + key.PKSize, BitConverter.GetBytes(regAdrs));
        }

        /* Inserta media llave de clave primaria, se proporciona la direccion del índice
         * y la clave del registro. Queda pendiente la direccion del registro */
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

        /* Busca una clave del indice primario para insertar registro
         * prevBlock != -1 y idxAdrs == 1, no existe una sublista de 100
         * prevBlock == -1 y idxAdrs != -1, existe en una sublista */
        private bool FindPK(string keyName, ref long prevBlock, ref long idxAdrs, ref long block) {
            // Incremento del ciclo, y tamaño del indice con apuntador (char = 10, int = 12)
            int inc = key.PKIsChar ? 26 : 9, idxTam = key.PKIsChar ? 10 : 12;
            int v = key.PKIsChar ? Char.IsUpper(keyName[0]) ? 97 : 65 : 48;
            // v = key.PKIsChar ? 65 : 48 si es minuscula
            long prevIdxAdrs = -1;

            byte[] indexP = index.ToArray();

            for (int i = 0; i < inc; i++) {
                idxAdrs = i * idxTam; // Dirección del índice en el archivo
                block = BitConverter.ToInt64(indexP, (int)idxAdrs + key.PKSize); // Dirección del bloque de ese índice
                if (block != -1) {
                    if (i == keyName[0] - v - 1) { // Si encuentra el primer digito de la clave
                        idxAdrs = BitConverter.ToInt64(indexP, (int)idxAdrs + key.PKSize);
                        if (key.PKIsChar) {
                            // Busca en la sublista por la clave de índice
                            for (int j = 0; j < 100; j++, prevIdxAdrs = idxAdrs, idxAdrs += key.PKSize + 8) {
                                var s = Encoding.UTF8.GetString(indexP, (int)idxAdrs, key.PKSize);
                                // Si no está, entonces esta vacío
                                if (!s.Any(x => Char.IsLetter(x))) {
                                    return false;
                                }
                                // Si es igual, entonces lo encontro
                                if (String.Compare(keyName, s) == 0) {
                                    return true;
                                }
                                // Si es menor, entonces va en el punto en el que se quedo
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
                // Si no se encontró en la lista principal, llego al final, entonces no existe
                if (i == keyName[0] - v - 1) {
                    idxAdrs = -1;
                    break;
                }
                
            }

            return false;
        }        

        // Mueve una lista de 100 indices hacia abajo comenzando del índice dado
        private void ShiftPKDown(long indAux, long blockDir) {

            if (key.PKIsChar) {
                string value = Encoding.UTF8.GetString(index.ToArray(), (int)indAux, key.PKSize).Replace("~", "");
                // Si el valor es vacio, es porque va a insertar en ese lugar
                if (value.Any(x => char.IsLetter(x))) {
                    return;
                }
                // Si el valor no es vacio, es porque el indice va en la siguiente posicion
                else {
                    index.RemoveRange((int)blockDir + (key.PKSize + 8) * 99, key.PKSize + 8);
                    index.InsertRange((int)indAux, new byte[key.PKSize + 8]);
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

        // Mueve una lista de 100 indices hacia arriba comenzando del índice dado, el indice se elimina
        private void ShiftPKUp(long indAux, long blockDir) {
            if (key.PKIsChar) {
                string value = Encoding.UTF8.GetString(index.ToArray(), (int)indAux, key.PKSize).Replace("~", "");
                // Si el valor es vacio, es porque va a insertar en ese lugar, no hace nada
                if (value.Any(x => char.IsLetter(x))) {
                    return;
                }
                // Si el valor no es vacio, es porque el indice va en la siguiente posicion
                else {
                    index.InsertRange((int)blockDir + (key.PKSize + 8) * 99, new byte[key.PKSize + 8]);
                    index.RemoveRange((int)indAux, key.PKSize + 8);
                }
            }
            else {
                int value = BitConverter.ToInt32(index.ToArray(), (int)indAux);
                if (value == -1) {
                    return;
                }
                else {
                    index.InsertRange((int)blockDir + (key.PKSize + 8) * 99, new byte[key.PKSize + 8]);
                    index.RemoveRange((int)indAux, key.PKSize + 8);
                }
            }
        }

        /* Crea una sblista lista de 100 y la agrega al final del archivo de indice*/
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
