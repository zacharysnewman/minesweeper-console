using System.Collections;
using System.Collections.Generic;

namespace Events
{
    namespace ConsoleEvents
    {
        // Console commands: From Entry > Renderer
        public class KeyCommandEvent : EventSync { }
    }
    namespace InputEvents
    {
        public class GenerateMapEvent : EventSync<MapInformation> { }
        public class ActivateTileEvent : EventSync<Coords, bool> { }
    }
    namespace StateEvents
    {
        public class StateChangedEvent : EventSync<State> { }
    }
}