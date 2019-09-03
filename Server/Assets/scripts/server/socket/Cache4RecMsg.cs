using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nsocket
{
    public class Cache4RecMsg
    {
        /// <summary>
        /// 对于server来说可以是ip+port
        /// 对于client这个参数可以随意写，也可以不用
        /// </summary>
        public string SessionID;

        public short Cmd;

        public byte[] Data;

    }
}