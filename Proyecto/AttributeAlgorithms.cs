using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Proyecto {
    public partial class DataBase {
        private bool AddAttribute(string name, char type, int length, int indexType, long eIndex) {
            long aIndex = BitConverter.ToInt64(data.ToArray(), (int)eIndex + 38);
            long aAnt = -1;

            // Find last entity
            LastEntity(ref aIndex, ref aAnt);

            // Add attribute address (30 bytes)
            byte[] byteName = Encoding.UTF8.GetBytes(name);
            data.AddRange(byteName);
            for (int i = byteName.Length; i < 30; i++) {
                data.Add(Convert.ToByte('~'));
            }

            // Add attribute address (8 bytes)
            data.AddRange(BitConverter.GetBytes((long)data.Count));

            // Add data type (1 byte)
            data.AddRange(BitConverter.GetBytes(type));

            // Add data length (4 bytes)
            data.AddRange(BitConverter.GetBytes(length));

            // Add index type (4 bytes)
            data.AddRange(BitConverter.GetBytes(indexType));

            // Add index address (8 bytes)
            data.AddRange(BitConverter.GetBytes((long)-1));

            // Add next attribute address (8 bytes)
            data.AddRange(BitConverter.GetBytes((long)-1));

            // Link last attribute with new attribute
            if (aAnt != -1) {
                ReplaceBytes(aAnt, (long)data.Count);
            }
            else {
                ReplaceBytes(eIndex + 38, (long)data.Count);
            }
            return true;
        }

        // Update the aIndex from the last entity and the previous
        private void LastEntity(ref long aIndex, ref long aAnt) {
            while (aIndex != -1) {
                aAnt = aIndex;
                aIndex = BitConverter.ToInt64(data.ToArray(), (int)aIndex + 55);
            }
        }
    }
}
