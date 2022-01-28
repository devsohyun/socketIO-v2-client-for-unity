using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SocketMessages
{
    public class PlayerEvent
    {
        public string cmd;
    }

    public class SocketEmitMessageList : MonoBehaviour
    {
        [System.NonSerialized]
        public string playerMessage = "";
    }

}