using System;
using System.Collections.Generic;
using System.Text;

namespace dMobile.Resources.Types
{
    public class Message
    {
        private int messageIndex;
        private string messageValue;

        public int MessageIndex { get => messageIndex; set => messageIndex = value; }
        public string MessageValue { get => messageValue; set => messageValue = value; }
    }
}
