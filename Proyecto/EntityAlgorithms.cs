using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Proyecto {
    public partial class DataBase {

        /* Busca una entidad en data, si no la encuentra regresa falso
         * index tiene la direccion de la entidad, ant la direccion de la anterior
         * Si la lista esta vacia: return false, index = -1, ant = ,1
         * Si es el primer elemento: return true, index = head, ant = -1
         * Si va despues del primer elemento: return true, index = address */
        private bool SearchEntity(string name, ref long index, ref long ant) {
            string entityName = "";
            // if its not empty
            if (head != -1) {
            index = BitConverter.ToInt64(data.ToArray(), 0);
                entityName = Encoding.UTF8.GetString(data.ToArray(), (int)index, 30).Replace("~", "");
                while (String.Compare(name, entityName) == 1 && index != -1) {
                    ant = index;
                    index = BitConverter.ToInt64(data.ToArray(), (int)index + 54);
                    if (index != -1) {
                        entityName = Encoding.UTF8.GetString(data.ToArray(), (int)index, 30).Replace("~", "");
                    }
                }

                return name == entityName ? true : false;
            }
            return false;
        }

        /* Agrega una nueva entidad al final del archivo. Las entidades son agregadas ordenadas y siempre se inicializan
         sus valores en -1. Los valores que se agregan son el nombre y la dirección actual, que es el tamaño actual del
         archivo*/
        private bool AddEntity(string name) {
            long index = -1, ant = -1;
            byte[] byteName = Encoding.UTF8.GetBytes(name);
            long currentAddress = data.Count;

            // Si la entidad no se encuentra
            if (SearchEntity(name, ref index, ref ant) == false) {
                // Agrega nombre
                data.AddRange(byteName);
                for (int i = byteName.Length; i < 30; i++) {
                    data.Add(Convert.ToByte('~'));
                }

                // Agrega la dirección de la entidad, la dirección de los atributos, la dirección de los registros
                // y la dirección de la siguiente entidad, todos en -1
                data.AddRange(BitConverter.GetBytes(currentAddress));
                data.AddRange(BitConverter.GetBytes((long)-1));
                data.AddRange(BitConverter.GetBytes((long)-1));
                data.AddRange(BitConverter.GetBytes((long)-1));
                
                // Si se inserta al principio, se actualiza la cabecera
                if (index == head) {
                    ReplaceBytes(data, currentAddress + 54, index);
                    ReplaceBytes(data, 0, currentAddress);
                    head = currentAddress;
                }
                else {
                    // Si se inserta después de la cabecera, se inserta entre las entidades en las que va
                    ReplaceBytes(data, ant + 54, currentAddress);
                    if (index != -1) {
                        ReplaceBytes(data, currentAddress + 54, BitConverter.ToInt64(data.ToArray(), (int)index + 30));
                    }
                }
                return true;
            }
            return false;
        }

        /* Elimina una entidad en el archivo. Sólo elimina la referencia de ésta. Si se elimina al principio
         * se actualiza la cabecera con el siguiente. Si se elimina después del principio, se enlaza la entidad
         anterior con la siguiente. */
        private bool DeleteEntity(string name) {
            long index = -1, ant = -1;
            byte[] byteName = Encoding.UTF8.GetBytes(name);
            // Si se encuentra la entidad
            if (SearchEntity(name, ref index, ref ant) == true) {
                // ...al principio
                if (index == head) {
                    head = BitConverter.ToInt64(data.ToArray(), (int)index + 54);
                }
                // ...en el centro o al final
                else {
                    ReplaceBytes(data, ant + 54, BitConverter.ToInt64(data.ToArray(), (int)index + 54));
                }
                return true;
            }
            return false;
        }

        /* Modifica una entidad en el archivo. Los datos se reemplazan en la dirección que tiene la entidad
         * en el archivo. Se modifican sólo el nombre. El tamaño del archivo no es modificado. Si la entidad
         * queda en otro lugar, los apuntadores se actualizan a la ubicación correcta*/
        private bool ModifyEntity(string newName, ref long index) {
            long aux = index, index2 = -1, ant2 = -1;

            // Completa el tamaño del nombre (30 bytes)
            List<char> nName = newName.ToList();
            for (int i = nName.Count; i < 30; i++) {
                nName.Add('~');
            }
            // "Elimina" la entidad en i
            string tmp = Encoding.UTF8.GetString(data.ToArray(), (int)index, 30).Replace("~", "");
            DeleteEntity(tmp);


            // Inserta la entidad como si fuera nueva, manteniendo su dirección actual
            if (SearchEntity(newName, ref index2, ref ant2) == false) {

                // Cambia el nombre de la entidad
                byte[] nDName = Encoding.UTF8.GetBytes(nName.ToArray());
                ReplaceBytes(data, index, nDName);
                // Si se inserta en la cabecera
                if (index2 == head) {
                    ReplaceBytes(data, aux + 54, index2);
                    ReplaceBytes(data, 0, aux);
                    head = aux;
                }
                else {
                    // Si se inserta después de la cabecera
                    ReplaceBytes(data, ant2 + 54, aux);
                    if (index2 != -1) {
                        ReplaceBytes(data, aux + 54, BitConverter.ToInt64(data.ToArray(), (int)index2 + 30));
                    }
                    else {
                        ReplaceBytes(data, aux + 54, -1);
                    }
                }
            }
            return true;
        }
    }
}
