using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto {
    public partial class DataBase {
        private void AddEntry(List<string> output, List<char> types, List<int> sizes, int size) {
            string name = comboBox2.Text;
            // Si el registro ya tiene datos, inserta hasta el final
            if (register.Count > 0) {
                // Buscar penultimo elemento y enlaza con el nuevo

                long aux = BitConverter.ToInt64(data.ToArray(), (int)selectedEntityAdrs + 46);
                while (BitConverter.ToInt64(register.ToArray(), (int)aux + size + 8) != -1) {
                    aux = BitConverter.ToInt64(register.ToArray(), (int)aux + size + 8);                
                }
                ReplaceBytes(register, aux + size + 8, register.Count);
                register.AddRange(BitConverter.GetBytes((long)register.Count)); // Dirección N
            }
            else {
                // Hace que el dato del indice de la entidad apunte al primero
                ReplaceBytes(data, selectedEntityAdrs + 46, register.Count);
                WriteBinary(fileName);
                UpdateEntityTable();
                register.AddRange(BitConverter.GetBytes((long)register.Count)); // Dirección N
            }

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
                    switch (sizes[i]) {
                        case 2:
                            register.AddRange(BitConverter.GetBytes(Convert.ToInt16(output[i])));
                            break;
                        case 4:
                            register.AddRange(BitConverter.GetBytes(Convert.ToInt32(output[i])));
                            break;
                        case 8:
                            register.AddRange(BitConverter.GetBytes(Convert.ToInt64(output[i])));
                            break;

                        default:
                            break;
                    }
                }
            }

            // Agrega la dirección de la siguiente entidad en -1
            register.AddRange(BitConverter.GetBytes((long)-1));
            WriteRegBinary(name);
        }
    }
}
