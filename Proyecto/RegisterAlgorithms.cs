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
                if (key.isChar) {
                    keyName = Encoding.UTF8.GetString(dataR, (int)rIndex + 8 + key.pos, key.size).Replace("~", "");
                    while (String.Compare(name, keyName) == 1 && rIndex != -1) {
                        rAnt = rIndex;
                        rIndex = BitConverter.ToInt64(dataR, (int)rIndex + 8 + registerSize);
                        //keyName = Encoding.UTF8.GetString(dataR, (int)rIndex + 8 + key.pos, key.size).Replace("~", "");
                        if (rIndex != -1) {
                            keyName = Encoding.UTF8.GetString(dataR, (int)rIndex + 8 + key.pos, key.size).Replace("~", "");
                        }
                    }
                    return keyName == name ? true : false;
                }
                else {
                    int name2 = Convert.ToInt32(name);
                    keyNum = BitConverter.ToInt32(dataR, (int)rIndex);
                    while (keyNum < name2 && rIndex != -1) {
                        rAnt = rIndex;
                        rIndex = BitConverter.ToInt64(dataR, (int)rIndex + key.size + 8);
                        if (rIndex != -1) {
                            keyNum = BitConverter.ToInt32(dataR, (int)rIndex);
                        }
                    }
                    return keyNum == name2 ? true : false;
                }
            }

            return false;
        }

        /* Inserta un registro de forma ordenada en el archivo de registro de la entidad */
        private bool AddOrderedEntry(List<string> output, List<char> types, List<int> sizes) {
            long rIndex = -1, rAnt = -1;
            long currentRegAdrs = register.Count;
            if (!SearchRegistry(output[key.attribIndex], ref rIndex, ref rAnt)) {

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
                    ReplaceBytes(register, currentRegAdrs + 8 + registerSize, rIndex);
                    ReplaceBytes(data, selectedEntityAdrs + 46, currentRegAdrs);
                }
                else {
                    // Si se inserta después de la cabecera, se inserta entre las entidades en las que va
                    ReplaceBytes(register, rAnt + 8 + registerSize, currentRegAdrs);
                    if (rIndex != -1) {
                        ReplaceBytes(register, currentRegAdrs + 8 + registerSize, BitConverter.ToInt64(data.ToArray(), startIndex: (int)rIndex));
                    }
                }
                return true;
            }
            return false;
        }

        /* Inserta un registro de forma secuencial en el archivo de registro de la entidad */
        private void AddSecuentialEntry(List<string> output, List<char> types, List<int> sizes) {
            string name = comboBoxReg.Text;
            // Si el registro ya tiene datos, inserta hasta el final
            if (register.Count > 0) {
                // Buscar penultimo elemento y enlaza con el nuevo

                long aux = BitConverter.ToInt64(data.ToArray(), (int)selectedEntityAdrs + 46);
                while (BitConverter.ToInt64(register.ToArray(), (int)aux + registerSize + 8) != -1) {
                    aux = BitConverter.ToInt64(register.ToArray(), (int)aux + registerSize + 8);                
                }
                ReplaceBytes(register, aux + registerSize + 8, register.Count);
            }
            else {
                // Hace que el dato del indice de la entidad apunte al primero
                ReplaceBytes(data, selectedEntityAdrs + 46, register.Count);
                WriteBinary(dictionary);
                UpdateEntityTable();
            }
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
            register.AddRange(BitConverter.GetBytes((long)-1));
            WriteRegBinary(name);
        }
    }
}
