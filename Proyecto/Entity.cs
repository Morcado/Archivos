using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Proyecto {
    class Entity {
        // Size on binary = 62 bytes;
        private string name;
        private long entityAddress;
        private long attributeAddress;
        private long dataAddress;
        private long nextEntityAddress;
        private List<EntityAttribute> attributes;

        public Entity() {
            attributes = new List<EntityAttribute>();
        }
    }
}