using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TaleWorlds.Library;

namespace ArenaExpansion {
    class MWAXConfig {
        public string MWAXWeaponStage() {
            XElement xml = XElement.Load(BasePath.Name + "Modules/mewhi-ArenaExpansion/ModuleData/mwaxconfig.xml");

            return xml.Element("WeaponStage").Value;
        }
    }
}
