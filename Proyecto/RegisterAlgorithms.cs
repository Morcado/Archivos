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
                    CompleteKey(idxAdrs, newAdrs);
                    return true;
                }
            }
            else {
                if (key.FK) {
                    resp = InsertForeignKey(output[key.FKAtribListIndex], ref idxAdrs);
                    if (resp) {
                        // Inserta registro
                        return true;
                    }
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

        /* Inserta un registro de forma secuencial en el archivo de registro de la entidad */

        // Elimina un registro dada su clave de busqueda, utiliza la primera por defecto si no la tiene
        private bool DeleteRegister(string output) {
            long rIndex = -1, rAnt = -1;
            long currentAdrs = register.Count;
            if (SearchRegistry(output, ref rIndex, ref rAnt, true)) {
                long next = BitConverter.ToInt64(register.ToArray(), (int)rIndex + registerSize + 8);
                if (rIndex == BitConverter.ToInt64(data.ToArray(), (int)selectedEntityAdrs + 46)) {
                    ReplaceBytes(data, selectedEntityAdrs + 46, BitConverter.GetBytes(next));
                    textBoxReg.Text = next.ToString();
                    UpdateEntityTable();
                }
                // ...en el centro o al final
                else {
                    ReplaceBytes(register, rAnt + registerSize + 8, BitConverter.GetBytes(next));
                }
                return true;
            }
            return false;
        }

        /* Modifica un registro en el archivo de datos. Si tiene clave de búsqueda entonces verifica para 
         * reordenar los elementos, si no tiene clave de búsqueda, entonces solamente modifica los valores en
         * donde está */
        private bool ModifyRegister(List<string> newData, ref long rIndex) {
            int pos = 8;
            long rIndex2 = -1, rAnt2 = -1;
            if (key.searchKey) {
                for (int i = 0; i < key.searchKeyAttribIndex; i++) {
                    pos += sizes[i];
                }
                string tmp = Encoding.UTF8.GetString(register.ToArray(), (int)rIndex + pos, key.searchKeySize).Replace("~", "");
                DeleteRegister(tmp);

                if (!SearchRegistry(newData[key.searchKeyAttribIndex], ref rIndex2, ref rAnt2, false)) {
                    pos = 8;
                    for (int i = 0; i < types.Count; i++) {
                        if (types[i] == 'C') {
                            byte[] byteName = Encoding.UTF8.GetBytes(newData[i]);
                            List<byte> bn = byteName.ToList();
                            for (int j = bn.Count; j < sizes[i]; j++) {
                                bn.Add(Convert.ToByte('~'));
                            }
                            ReplaceBytes(register, rIndex + pos, bn.ToArray());
                        }
                        else {
                            ReplaceBytes(register, rIndex + pos, BitConverter.GetBytes(Convert.ToInt32(newData[i])));
                        }
                        pos += sizes[i];
                        //ReplaceBytes();
                    }
                    long head = BitConverter.ToInt64(data.ToArray(), (int)selectedEntityAdrs + 46);
                    if (rIndex2 == head) {
                        // Reemplaza la cabecera del diccionario de datos de los registos
                        ReplaceBytes(register, rIndex + 8 + registerSize, BitConverter.GetBytes(rIndex2));

                        ReplaceBytes(data, selectedEntityAdrs + 46, BitConverter.GetBytes(rIndex));
                    }
                    else {
                        ReplaceBytes(register, rAnt2 + 8 + registerSize, BitConverter.GetBytes(rIndex));
                        if (rIndex2 != -1) {
                            long aux = BitConverter.ToInt64(register.ToArray(), (int)rIndex2);
                            ReplaceBytes(register, rIndex + 8 + registerSize, BitConverter.GetBytes(aux));
                        }
                    }
                    return true;
                }
            }
            else {
                for (int i = 0; i < types.Count; i++) {
                    if (types.Count == 'C') {
                        byte[] byteName = Encoding.UTF8.GetBytes(newData[i]);
                        List<byte> bn = byteName.ToList();
                        for (int j = bn.Count; j < sizes[i]; j++) {
                            bn.Add(Convert.ToByte('~'));
                        }
                        ReplaceBytes(register, rIndex + pos, byteName);
                    }
                    else {
                        ReplaceBytes(register, rIndex + pos, BitConverter.GetBytes(Convert.ToInt32(newData[i])));
                    }
                    pos += sizes[i];
                }
                return true;
            }
            return true;
        }


    }
}
