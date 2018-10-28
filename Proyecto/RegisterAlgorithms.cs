using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto {
    public partial class DataBase {

        /* Esta funcion busca un registro, dado el nombre. Regresa la dirección del registro 
         * si existe, y la dirección del anterior. Si el registro no existe, regresa -1. El
         * registro se busca dada una clave de busqueda. Sin la clave no se puede buscar nada*/
        private bool SearchRegistry(string name, ref long rIndex, ref long rAnt) {
            rIndex = BitConverter.ToInt64(data.ToArray(), (int)selectedEntityAdrs + 46);
            byte[] dataR = register.ToArray();
            string keyName = "";
            int keyNum;
            /* Recorre todos los registros utilizando la clave de busqueda
             * hasta que el siguiente indice es -1 */
            if (rIndex != -1) {
                if (key.searchKeyIsChar) {
                    keyName = Encoding.UTF8.GetString(dataR, (int)rIndex + 8 + key.searchKeyPos, key.searchKeySize).Replace("~", "");
                    while (String.Compare(name, keyName) == 1 && rIndex != -1) {
                        rAnt = rIndex;
                        rIndex = BitConverter.ToInt64(dataR, (int)rIndex + 8 + registerSize);
                        if (rIndex != -1) {
                            keyName = Encoding.UTF8.GetString(dataR, (int)rIndex + 8 + key.searchKeyPos, key.searchKeySize).Replace("~", "");
                        }
                    }
                    // Si tiene indice primario, entonces se regresa true, wip
                    /*if (key.type == 2) {
                        return true;
                    }*/
                    return keyName == name ? true : false;
                }
                else {
                    int name2 = Convert.ToInt32(name);
                    keyNum = BitConverter.ToInt32(dataR, (int)rIndex);
                    while (keyNum < name2 && rIndex != -1) {
                        rAnt = rIndex;
                        rIndex = BitConverter.ToInt64(dataR, (int)rIndex + key.searchKeySize + 8);
                        if (rIndex != -1) {
                            keyNum = BitConverter.ToInt32(dataR, (int)rIndex);
                        }
                    }
                    // Si tiene indice primario, entonces se regresa true, wip
                    /*if (key.type == 2) {
                        return true;
                    }*/
                    return keyNum == name2 ? true : false;
                }
            }

            return false;
        }

        /* Inserta un registro de forma ordenada en el archivo de registro de la entidad. Si el tipo de indice
         * es primario o secundario, agrega el indice incluso si ya existe la clave de busqueda*/
        private long AddOrderedEntry(List<string> output, List<char> types, List<int> sizes) {
            long rIndex = -1, rAnt = -1;
            long currentRegAdrs = register.Count;
            if (!SearchRegistry(output[key.searchKeyAttribIndex], ref rIndex, ref rAnt)) {
                
                register.AddRange(BitConverter.GetBytes((long)register.Count)); // Dirección N
                                                                                // Agrega los nuevos datos al final del archivo
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
                // Agrega la dirección de la siguiente entidad en -1
                register.AddRange(BitConverter.GetBytes(rIndex));

                // Enlaza si va igual que la cabeza, se actualiza la cabecera
                if (rIndex == BitConverter.ToInt64(data.ToArray(), (int)selectedEntityAdrs + 46)) {
                    ReplaceBytes(register, currentRegAdrs + 8 + registerSize, BitConverter.GetBytes(rIndex));
                    ReplaceBytes(data, selectedEntityAdrs + 46, BitConverter.GetBytes(currentRegAdrs));
                }
                else {
                    // Si se inserta después de la cabecera, se inserta entre las entidades en las que va
                    ReplaceBytes(register, rAnt + 8 + registerSize, BitConverter.GetBytes(currentRegAdrs));
                    if (rIndex != -1) {
                        long aux = BitConverter.ToInt64(data.ToArray(), (int)rIndex);
                        ReplaceBytes(register, currentRegAdrs + 8 + registerSize, BitConverter.GetBytes(aux));
                    }
                }
                return currentRegAdrs;
            }
            return -1;
        }

        /* Inserta un registro de forma secuencial en el archivo de registro de la entidad */
        private long AddSecuentialEntry(List<string> output, List<char> types, List<int> sizes) {
            // Si el registro ya tiene datos, inserta hasta el final
            long regAdrs = register.Count;
            if (regAdrs > 0) {
                // Buscar penultimo elemento y enlaza con el nuevo

                long aux = BitConverter.ToInt64(data.ToArray(), (int)selectedEntityAdrs + 46);
                while (BitConverter.ToInt64(register.ToArray(), (int)aux + registerSize + 8) != -1) {
                    aux = BitConverter.ToInt64(register.ToArray(), (int)aux + registerSize + 8);                
                }
                ReplaceBytes(register, aux + registerSize + 8, BitConverter.GetBytes(regAdrs));
            }
            else {
                // Hace que el dato del indice de la entidad apunte al primero
                ReplaceBytes(data, selectedEntityAdrs + 46, BitConverter.GetBytes(regAdrs));
                WriteDictionary();
                UpdateEntityTable();
            }
            register.AddRange(BitConverter.GetBytes(regAdrs)); // Dirección N

            // Agrega los nuevos datos al final del archivo
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

            // Agrega la dirección de la siguiente entidad en -1
            register.AddRange(BitConverter.GetBytes((long)-1));
            return regAdrs;
        }

        // Elimina un registro dada su clave de busqueda, utiliza la primera por defecto si no la tiene
        private bool DeleteRegister(string output) {
            long rIndex = -1, rAnt = -1;
            long currentAdrs = register.Count;
            if (SearchRegistry(output, ref rIndex, ref rAnt)) {
                long next = BitConverter.ToInt64(register.ToArray(), (int)rIndex + registerSize + 8);
                if (rIndex == BitConverter.ToInt64(data.ToArray(), (int)selectedEntityAdrs + 46)) {
                    ReplaceBytes(data, selectedEntityAdrs + 46, BitConverter.GetBytes(next));
                }
                // ...en el centro o al final
                else {
                    ReplaceBytes(register, rAnt + registerSize + 8, BitConverter.GetBytes(next));
                }
                return true;
            }
            return false;
        }
    }
}
