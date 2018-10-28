using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Proyecto {
    public partial class DataBase {
        private void CreateFKStructure() {
            for (int i = 0; i < 50; i++) {
                index.AddRange(new byte[key.FKSize]);
                index.AddRange(BitConverter.GetBytes((long)-1));
            }
        }

        private bool InsertForeignKey(string keyName, List<string> forms, List<char> types, List<int> sizes) {
            long registerAdrs = -1, auxRegAdrs = -1;

            if (!FindPKKey(keyName, ref auxRegAdrs)) {

            }
            return false;
        }

        private bool FindFKKEY(string keyname, ref long auxRegAdrs) {
            return false;
        }
    }
}
