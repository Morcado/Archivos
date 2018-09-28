using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Proyecto {
    public partial class DataBase {
        /* Agrega un atributo al final del archivo y lo enlaza con el anterior. Se agrega el nombre del atributo,
         * el tipo, el tamaño del atributo y el tipo de índice, especificados por el usuario */
        private bool AddAttribute(string name, char type, int length, int indexType) {
            long aIndex = BitConverter.ToInt64(data.ToArray(), (int)selectedEntityAdrs + 38);
            long aAnt = -1;
            long lastAttributeAddess = data.Count;
            // Regresa el último atributo registrado de la entidad seleccionada
            LastAttribute(ref aIndex, ref aAnt);

            // Agrega el nombre del atributo (30 bytes)
            byte[] byteName = Encoding.UTF8.GetBytes(name);
            data.AddRange(byteName);
            for (int i = byteName.Length; i < 30; i++) {
                data.Add(Convert.ToByte('~'));
            }

            /* Direccion del atributo (8 bytes), tipo de dato (2 byte), longitud de dato (4 bytes)
            tipo de indice (4 bytes), direccion del indice (8 bytes) direccion del siguiente atributo (8 bytes) */
            data.AddRange(BitConverter.GetBytes(lastAttributeAddess));
            data.AddRange(BitConverter.GetBytes(type));
            data.AddRange(BitConverter.GetBytes(length));
            data.AddRange(BitConverter.GetBytes(indexType));
            data.AddRange(BitConverter.GetBytes((long)-1));
            data.AddRange(BitConverter.GetBytes((long)-1));

            // Enlaza el ultimo atributo con el nuevo
            if (aAnt != -1) {
                ReplaceBytes(data, aAnt + 56, (long)lastAttributeAddess);
            }
            else {
                ReplaceBytes(data, selectedEntityAdrs + 38, (long)lastAttributeAddess);
            }
            return true;
        }

        /* Modifica el atributo dada la dirección del atributo. Se reemplazan los datos en la dirección actual
         * que tiene el atributo. El tamaño del archivo no se modifica. El orden de los atributos no se modifica */
        private void ModifyAttribute(long aIndex, string name, char type, int length, int indexType) {
            // Completa los 30 bytes del nombre
            byte[] byteName = Encoding.UTF8.GetBytes(name);
            List<byte> bn = byteName.ToList();
            for (int i = bn.Count; i < 30; i++) {
                bn.Add(Convert.ToByte('~'));
            }

            // Remplaza nombre, tipo, longitud e indice
            ReplaceBytes(data, aIndex, bn.ToArray());
            ReplaceBytes(data, aIndex + 38, BitConverter.GetBytes(type));
            ReplaceBytes(data, aIndex + 40, BitConverter.GetBytes(length));
            ReplaceBytes(data, aIndex + 44, BitConverter.GetBytes(indexType));
        }

        /* Busca un atributo en la entidad seleccionada.
         * Si la entidad se encuentra, regresa true y aIndex = direccion del atributo
           Si no se encuentra, regresa false y aIndex = -1 y aAnt = -1 */
        private bool SearchAttribute(string name, ref long aIndex, ref long aAnt) {
            aIndex = BitConverter.ToInt64(data.ToArray(), (int)selectedEntityAdrs + 38);
            string attributeName = ""; //Encoding.UTF8.GetString(data.ToArray(), (int)aAux, 30).Replace("~", ""); ;

            // Si no encuentra el atributo regresa -1
            while (aIndex != -1) {
                attributeName = Encoding.UTF8.GetString(data.ToArray(), (int)aIndex, 30).Replace("~", "");
                if (attributeName == name) {
                    break;
                }
                aAnt = aIndex;
                aIndex = BitConverter.ToInt64(data.ToArray(), (int)aIndex + 56);
            }
            
            if (aIndex != -1) {
                return true;
            }
            else {
                return false;
            }
        }

        // Regresa la direccion del ultimo atributo de la entidad seleccionada
        private void LastAttribute(ref long aIndex, ref long aAnt) {
            while (aIndex != -1) {
                aAnt = aIndex;
                aIndex = BitConverter.ToInt64(data.ToArray(), (int)aIndex + 56);
            }
        }

        /* Elimina un atributo y enlaza el anterior con el siguiente. Si es el primer atributo que fue agregado,
         * se actualiza la dirección del primer atributo de la entidad, si se elimina al final, el anterior 
         * es marcado como el último*/
        private bool DeleteAttribute(string name) {
            long aIndex = -1, aAnt = -1;

            if (SearchAttribute(name, ref aIndex, ref aAnt)) {
                // Si es el primer elemento, cambia la direccion del primer atributo de la entidad
                if (aAnt == -1) {
                    ReplaceBytes(data, selectedEntityAdrs + 38, BitConverter.ToInt64(data.ToArray(), (int)aIndex + 56));
                }
                // Si es en medio o al final
                else {
                    ReplaceBytes(data, aAnt + 56, BitConverter.ToInt64(data.ToArray(), (int)aIndex + 56));
                }
                return true;
            }
            return false;
        }
    }
}
