using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Proyecto {
    public partial class DataBase {
        /* Searches for an entity in "data". If entity does not exists, return false
           And return "ant". Otherwise returns true and "i" and "ant" have current address and previous address
           If list is empty: return false, index = -1, ant = ,1
           If found first element: return true, index = head, ant = -1
           If found after first element: return true, index = address */
        private bool SearchEntity(string name, ref long index, ref long ant) {
            index = BitConverter.ToInt64(data.ToArray(), 0);
            string entityName = "";
            // if its not empty
            if (head != -1) {
                entityName = Encoding.UTF8.GetString(data.ToArray(), (int)index, 30).Replace("~", "");
                while (String.Compare(name, entityName) == 1 && index != -1) {
                    ant = index;
                    index = BitConverter.ToInt64(data.ToArray(), (int)index + 54);
                    if (index != -1) {
                        entityName = Encoding.UTF8.GetString(data.ToArray(), (int)index, 30).Replace("~", "");
                    }
                }

                if (name == entityName) {
                    return true;
                }
                else {
                    return false;
                }
            }
            return false;
        }

        // Add new entity on current data, as it is new, it will add to the end of file
        private bool AddEntity(string name) {
            long index = -1, ant = -1;
            byte[] byteName = Encoding.UTF8.GetBytes(name);
            long currentAddress = data.Count;

            if (SearchEntity(name, ref index, ref ant) == false) {

                // Add name
                data.AddRange(byteName);
                for (int i = byteName.Length; i < 30; i++) {
                    data.Add(Convert.ToByte('~'));
                }

                // Add entity address, attribute address, data address and entity address
                data.AddRange(BitConverter.GetBytes((long)currentAddress));
                data.AddRange(BitConverter.GetBytes((long)-1));
                data.AddRange(BitConverter.GetBytes((long)-1));
                data.AddRange(BitConverter.GetBytes((long)-1));
                

                // If insert at beginning and head must be updated
                if (index == head) {
                    ReplaceBytes(currentAddress + 54, index);
                    ReplaceBytes(0, currentAddress);
                    head = currentAddress;
                }
                else {
                    // If insert after head
                    ReplaceBytes(ant + 54, currentAddress);
                    if (index != -1) {
                        ReplaceBytes(currentAddress + 54, BitConverter.ToInt64(data.ToArray(), (int)index + 30));
                    }
                }
                return true;
            }
            return false;
        }

        /* Detele an entity in any place in the file. Redirects the previos pointer to
           the next one the entity is pointing to  */
        private bool DeleteEntity(string name) {
            long index = -1, ant = -1;
            byte[] byteName = Encoding.UTF8.GetBytes(name);
            // If entity is found
            if (SearchEntity(name, ref index, ref ant) == true) {
                // ...In head
                if (index == head) {
                    head = BitConverter.ToInt64(data.ToArray(), (int)index + 54);
                }
                // ...In middle or end
                else {
                    ReplaceBytes(ant + 54, BitConverter.ToInt64(data.ToArray(), (int)index + 54));
                }
                return true;
            }
            return false;
        }

        // Modify an entity
        private bool ModifyEntity(string newName, ref long index) {
            long aux = index, index2 = -1, ant2 = -1;

            // Complete the space that name uses (30 bytes)
            List<char> nName = newName.ToList();
            for (int i = nName.Count; i < 30; i++) {
                nName.Add('~');
            }
            // "Delete" entity at "i"
            string tmp = Encoding.UTF8.GetString(data.ToArray(), (int)index, 30).Replace("~", "");
            DeleteEntity(tmp);


            // Insert entity as it is new, but it keeps its own address
            if (SearchEntity(newName, ref index2, ref ant2) == false) {

                // Change entity name. Entity is on "i"
                byte[] nDName = Encoding.UTF8.GetBytes(nName.ToArray());
                ReplaceBytes(index, nDName);

                if (index2 == head) {
                    ReplaceBytes(aux + 54, index2);
                    ReplaceBytes(0, aux);
                    head = aux;
                }
                else {
                    // If insert after head
                    ReplaceBytes(ant2 + 54, aux);
                    if (index2 != -1) {
                        ReplaceBytes(aux + 54, BitConverter.ToInt64(data.ToArray(), (int)index2 + 30));
                    }
                    else {
                        ReplaceBytes(aux + 54, -1);
                    }
                }
            }
            return true;
        }
    }
}
