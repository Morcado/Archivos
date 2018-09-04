using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Proyecto {
    class EntityAttribute {
        // Size on binary = 63
        private string name;
        private long attributeAddress;
        private char dataType;
        private int dataLength;
        private int indexType;
        private long indexAddress;
        private long nextAttributeAddress;
    }
}
