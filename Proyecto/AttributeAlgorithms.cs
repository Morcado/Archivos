using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Proyecto {
    public partial class DataBase {
        // Add an attribute to the end of file, and link with previous
        private bool AddAttribute(string name, char type, int length, int indexType) {
            long eIndex = -1, eAnt = -1;
            SearchEntity(comboBox1.Text, ref eIndex, ref eAnt);

            long aIndex = BitConverter.ToInt64(data.ToArray(), (int)eIndex + 38);
            long aAnt = -1;
            long lastAttributeAddess = data.Count;
            // Find last entity
            LastAttribute(ref aIndex, ref aAnt);

            // Add attribute name (30 bytes)
            byte[] byteName = Encoding.UTF8.GetBytes(name);
            data.AddRange(byteName);
            for (int i = byteName.Length; i < 30; i++) {
                data.Add(Convert.ToByte('~'));
            }

            // Add attribute address (8 bytes), data type (2 byte), data length (4 bytes)
            // index type (4 bytes), index address (8 bytes) and next attribute address (8 bytes)
            data.AddRange(BitConverter.GetBytes(lastAttributeAddess));
            data.AddRange(BitConverter.GetBytes(type));
            data.AddRange(BitConverter.GetBytes(length));
            data.AddRange(BitConverter.GetBytes(indexType));
            data.AddRange(BitConverter.GetBytes((long)-1));
            data.AddRange(BitConverter.GetBytes((long)-1));

            // Link last attribute with new attribute
            if (aAnt != -1) {
                // if it's first attribute
                ReplaceBytes(aAnt + 56, (long)lastAttributeAddess);
            }
            else {
                ReplaceBytes(eIndex + 38, (long)lastAttributeAddess);
            }
            return true;
        }

        // Modify attribute given the index of that attribute
        private void ModifyAttribute(long aIndex, string name, char type, int length, int indexType) {
            // Complete new name to 30 bytes
            byte[] byteName = Encoding.UTF8.GetBytes(name);
            List<byte> bn = byteName.ToList();
            for (int i = bn.Count; i < 30; i++) {
                bn.Add(Convert.ToByte('~'));
            }

            // Replace name, type, length and index type
            ReplaceBytes(aIndex, bn.ToArray());
            ReplaceBytes(aIndex + 38, BitConverter.GetBytes(type));
            ReplaceBytes(aIndex + 40, BitConverter.GetBytes(length));
            ReplaceBytes(aIndex + 44, BitConverter.GetBytes(indexType));
        }

        // If entity is found, return true and aIndex = attribute address
        // If entity is not found, return false and aIndex = -1 and aAnt = -1
        private bool SearchAttribute(string name, ref long aIndex, ref long aAnt, long eIndex) {
            aIndex = BitConverter.ToInt64(data.ToArray(), (int)eIndex + 38);
            string attributeName = ""; //Encoding.UTF8.GetString(data.ToArray(), (int)aAux, 30).Replace("~", ""); ;

            // Go through every attribute, if not found then aIndex = -1
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

        // Return last attribute address
        private void LastAttribute(ref long aIndex, ref long aAnt) {
            while (aIndex != -1) {
                aAnt = aIndex;
                aIndex = BitConverter.ToInt64(data.ToArray(), (int)aIndex + 56);
            }
        }

        // Deletes an attribute and link the previous with the next
        private bool DeleteAttribute(string name) {
            long aIndex = -1, eIndex = -1, aAnt = -1, eAnt = -1;
            SearchEntity(comboBox1.Text, ref eIndex, ref eAnt);

            if (SearchAttribute(name, ref aIndex, ref aAnt, eIndex)) {
                // If it is first element
                if (aAnt == -1) {
                    ReplaceBytes(eIndex + 38, BitConverter.ToInt64(data.ToArray(), (int)aIndex + 56));
                }
                else {
                    ReplaceBytes(aAnt + 56, BitConverter.ToInt64(data.ToArray(), (int)aIndex + 56));
                }
                return true;
            }
            return false;
        }
    }
}
