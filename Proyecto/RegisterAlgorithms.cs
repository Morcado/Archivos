using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Proyecto {
    public partial class DataBase {
        /* Esta funcion busca un registro, dado el nombre. Regresa la dirección del registro 
         * si existe, y la dirección del anterior. Si el registro no existe, regresa -1. El
         * registro se busca dada una clave de busqueda. Sin la clave no se puede buscar nada*/
        private bool SearchRegistry(string name, ref long rIndex, ref long rAnt, bool delete) {
            rIndex = BitConverter.ToInt64(data.ToArray(), (int)selectedEntityAdrs + 46);
            byte[] dataR = register.ToArray();
            string keyName = "";
            int keyNum;
            /* Recorre todos los registros utilizando la clave de busqueda
             * hasta que el siguiente0 indice es -1 */
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
                            keyNum = BitConverter.ToInt32(dataR, (int)rIndex);
                        }
                    }
                    if (delete) {
                        return true;
                    }
                    return false;
                }
            }
            rIndex = -1;
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
                long oldHead = BitConverter.ToInt64(data.ToArray(), (int)selectedEntityAdrs + 46);
                ReplaceBytes(data, selectedEntityAdrs + 46, BitConverter.GetBytes(newAdrs));
                if (newAdrs != 0) {
                    ReplaceBytes(register, newAdrs + 8 + registerSize, BitConverter.GetBytes(oldHead));
                }
            }
            return newAdrs;
        }

        /* Inserta un registro de forma ordenada en el archivo de registro de la entidad. Si el tipo de indice
         * es primario o secundario, agrega el indice incluso si ya existe la clave de busqueda*/
        private bool AddRegister(List<string> output) {
            long rIndex = -1, rAnt = -1;
            long prevIdxAdrs = -1, idxAdrs = -1, blockAdrs = -1;
            bool resp = false, resp2 = false;
            long currentRegAdrs = register.Count;

            if (key.PK) {
                resp = InsertPrimaryKey(output[key.PKAtribListIndex], ref prevIdxAdrs, ref idxAdrs, ref blockAdrs);

                // Si se inserto primario, y tiene secundario, entonces inserta el secundario
                if (resp) {
                    if (key.FK) {
                        resp2 = InsertForeignKey(output[key.FKAtribListIndex], ref idxAdrs);
                    }
                    // Inserta registro y inserta la dirección en el indice
                    // Si es el primer registro, entonces inserta al principio

                    // Si no habia bloque, entonces inserta solo el primero
                    long prevReg = -1, newAdrs = -1;

                    if (key.searchKey && prevIdxAdrs != -1) { 
                        prevReg = BitConverter.ToInt64(index.ToArray(), (int)prevIdxAdrs + key.PKSize);
                    }
                    else {
                        if (currentRegAdrs != 0) {
                            prevReg = currentRegAdrs - 8 - registerSize - 8;
                        }
                    }
               
                    newAdrs = InsertRegister(output, prevReg);
                    CompletePK(idxAdrs, newAdrs);
                    return true;
                }
            }
            else {
                if (key.FK) {
                    return InsertForeignKey(output[key.FKAtribListIndex], ref idxAdrs);
                }
                else {
                    if (!SearchRegistry(output[key.searchKeyAttribIndex], ref rIndex, ref rAnt, false)) {
                        InsertRegister(output, rAnt);
                        return true;
                    }
                }
            }
            return false;
        }

        // Obtiene la direccion del indice anterior en un registro con clave de busqueda
        private long GetPrevIdxAdrs(long idxAdrs, long blockAdrs, long prevBlock) {
            if (blockAdrs == -1 && prevBlock != -1) {
                return GetLastPK(prevBlock);
            }
            /* Si el bloque anterior no existe, entonces se checa que el indice no sea el primero
             * Si no es el primero, el se obtiene el índice inmediato anterior */
            else {
                if (idxAdrs != blockAdrs) {
                    return idxAdrs - 8 - key.PKSize;
                }
                // Si es el primer indice (idx = block), entonces no hay anterior
                else {
                    if (prevBlock == -1) {
                        return -1;
                    }
                }
            }
            return -1;
        }

        // Elimina un registro dada su clave de busqueda o indice, utiliza la primera por defecto si no la tiene
        private bool DeleteRegister(string output) {
            long rIndex = -1, rAnt = -1;
            long currentAdrs = register.Count;

            // Si tiene indice primario, busca el índice para elimiar el registri
            if (key.PK) {
                long prevBlock = -1, blockAdrs = -1, idxAdrs = -1;
                long prevIdxAdrs = -1;
                if (FindPK(output, ref prevBlock, ref idxAdrs, ref blockAdrs)) {
                    // Si tiene clave de busqueda, encuentra el registro anterior
                    long regAdrs = BitConverter.ToInt64(index.ToArray(), (int)idxAdrs + key.PKSize);
                    long prevRegAdrs = -1;
                    long nextRegAdrs = BitConverter.ToInt64(register.ToArray(), (int)regAdrs + registerSize + 8);

                    // Si tiene clave de busqueda, busca el registro anterior de acuerdo a los bloques
                    if (key.searchKey) {
                        prevIdxAdrs= GetPrevIdxAdrs(idxAdrs, blockAdrs, prevBlock);
                        if (prevIdxAdrs != -1) {
                            prevRegAdrs = BitConverter.ToInt64(register.ToArray(), (int)prevIdxAdrs + key.PKSize);
                        }
                    }
                    // Sino, busca el registro anterior solamente llendo hacia atras en el arreglo, 
                    // si es que se puede, si no entonces no hay registro anterior
                    else {
                        if (regAdrs >= 8 + registerSize + 8) {
                            prevRegAdrs = regAdrs - 8 - registerSize - 8;
                        }
                    }

                    if (regAdrs == BitConverter.ToInt64(data.ToArray(), (int)selectedEntityAdrs + 46)) {
                        ReplaceBytes(data, selectedEntityAdrs + 46, BitConverter.GetBytes(nextRegAdrs));
                        textBoxReg.Text = nextRegAdrs.ToString();
                        UpdateEntityTable();
                    }
                    else {
                        ReplaceBytes(register, prevRegAdrs + registerSize + 8, BitConverter.GetBytes(nextRegAdrs));
                    }

                    ShiftPKUp(idxAdrs, blockAdrs);
                    UpdateMainPKTable();
                    WriteIndexFile(comboBoxReg.Text);
                    return true;
                }
            }
            else {
                if (key.FK) {
                    if (key.searchKey) {

                    }
                    else {

                    }
                }
                else {
                    
                }
            }

            //if (SearchRegistry(output, ref rIndex, ref rAnt, true)) {
            //    long next = BitConverter.ToInt64(register.ToArray(), (int)rIndex + registerSize + 8);
            //    if (rIndex == BitConverter.ToInt64(data.ToArray(), (int)selectedEntityAdrs + 46)) {
            //        ReplaceBytes(data, selectedEntityAdrs + 46, BitConverter.GetBytes(next));
            //        textBoxReg.Text = next.ToString();
            //        UpdateEntityTable();
            //    }
            //    // ...en el centro o al final
            //    else {
            //        ReplaceBytes(register, rAnt + registerSize + 8, BitConverter.GetBytes(next));
            //    }
            //    return true;
            //}
            return false;
        }

        /* Modifica un registro en el archivo de datos. Si tiene clave de búsqueda entonces verifica para 
         * reordenar los elementos, si no tiene clave de búsqueda, entonces solamente modifica los valores en
         * donde está */
        private bool ModifyRegister(List<string> newData, ref long rIndex, long prevIndex) {
            return true;
        }
    }
}
