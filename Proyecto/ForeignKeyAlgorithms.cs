using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto {
    public partial class DataBase {
        /* Crea una estructura básica del índice seccundario. Crea una lista de 50 elementos
         * con el tamaño de la clave y el apuntador a la lista secundaria de esa clave */
        private void CreateFKStructure() {
            for (int i = 0; i < 50; i++) {
                index.AddRange(new byte[key.FKSize]);
                index.AddRange(BitConverter.GetBytes((long)-1));
            }
        }
        /* Regresa la dirección del indice donde se va a insertar la clave de busqueda.
         * Se conoce previamente la clave, si no se encuentra, entonces regresa falso.
         * Regresa true si se pudo insertar, si no regresa false */
        private bool InsertForeignKey(string keyName, ref long indexAdrs) {
            long idxAdrs = -1, aux = -1;
            // Si no se encuentra la lista secundaria
            if (!FindFKKey(keyName, ref idxAdrs)) {
                aux = index.Count;
                CreateForeignLargeList();
                // Enlaza la sublista con la lista principal
                ReplaceBytes(index, idxAdrs, Encoding.UTF8.GetBytes(keyName));
                ReplaceBytes(index, idxAdrs + key.FKSize, BitConverter.GetBytes(aux));
            }
            indexAdrs = aux;
            return true;
        }

        private void CreateForeignLargeList() {
            for (int i = 0; i < 50; i++) {
                index.AddRange(BitConverter.GetBytes((long)-1));
            }
        }

        private bool FindFK(string keyName, ref long idxAdrs) {
            long block = -1, prevBlock = -1;
            byte[] indexP = index.ToArray();

            for (int i = 0; i < 50; i++) {
                idxAdrs = i * (key.FKSize + 8);
                block = BitConverter.ToInt64(indexP, (int)idxAdrs + key.FKSize);
                if (block != -1) {



                    prevBlock = block;
                }
            }

            return false;
        }

        /* Busca en el archivo de índice la clave foranea para insertar un dato. Primero
         * busca en la lista de 50, si se encuentra el índice con la clave de búsqueda. Si
         * no se encuentra, regresa la dirección del archivo de índice donde va a estar
         * Si se encuentra, regresa la dirección del archivo de indice del ultimo registro
         * con esa clave de búsqueda*/
        private bool FindFKKey(string keyName, ref long idxAdrs) {
            byte[] indexPrint = index.ToArray();
            byte[] registerPrint = register.ToArray();
            int keyInt = -1;
            if (!key.FKIsChar) {
                keyInt = Convert.ToInt32(keyName);
            }

            long largeAdrs = key.FKAdrsOnFile;
            long subListAdrs =  BitConverter.ToInt64(indexPrint, (int)largeAdrs + key.FKSize);

            //Busca en la lista principal de 50 la clave
            while (subListAdrs != -1) {
                string name = Encoding.UTF8.GetString(indexPrint, (int)largeAdrs, key.FKSize).Replace("~", "");
                // Si se encuentra busca en la sublista del elemento de la lista
                if (name == keyName) {
                    long mediumAdrs = subListAdrs;
                    long regAdrs = BitConverter.ToInt64(indexPrint, (int)mediumAdrs);
                    // Mientras no encuentre un registro
                    while (regAdrs != -1) {
                        mediumAdrs += 8;
                        regAdrs = BitConverter.ToInt64(indexPrint, (int)mediumAdrs);
                    }
                    idxAdrs = regAdrs;
                    return true;
                }
                largeAdrs += key.FKSize + 8;
                subListAdrs = BitConverter.ToInt64(index.ToArray(), (int)largeAdrs + key.FKSize);
            }
            idxAdrs = largeAdrs;
            return false;
        }

        private void ShiftFKDown(long indAux, long blockDir) {

        }

        private void ShiftFKUp(long indAux, long blockDir) {

        }
    }
}