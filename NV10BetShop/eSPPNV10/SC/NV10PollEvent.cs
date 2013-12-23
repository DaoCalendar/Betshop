using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eSSPNV10.SC
{
    public class NV10PollEvent
    {
        public enum NV10PollEventType
        {
            Ok = 0x00,
            SlaveReset = 0xF1,
            ReadNote = 0xEF,
            CreditNote = 0xEE,
            NoteRejecting = 0xED,
            NoteRejected = 0xEC,
            NoteStacking = 0xCC,
            NoteStacked = 0xEB,
            SafeNoteJam = 0xEA,
            UnsafeNoteJam = 0xE9,
            Disabled = 0xE8,
            FraudAttempt = 0xE6,
            StackerFull = 0xE7,
            NoteClearedFromFront = 0xE1,
            NoteClearedToCashBox = 0xE2,
            NotePathOpen = 0xE0,
            ChannelDisable = 0xB5,
            Initialising = 0xB6
        }

        public static readonly List<NV10PollEventType> dataEvents = new List<NV10PollEventType>()
        {
            NV10PollEventType.ReadNote,
            NV10PollEventType.CreditNote,
            NV10PollEventType.FraudAttempt,
            NV10PollEventType.NoteClearedFromFront,
            NV10PollEventType.NoteClearedToCashBox
        };

        private NV10PollEventType _type;
        private int _data;

        public NV10PollEventType EventType
        {
            get
            {
                return _type;
            }
        }

        public int EventData
        {
            get
            {
                return _data;
            }
        }

        public NV10PollEvent(NV10PollEventType type, int data)
        {
            _type = type;
            _data = data;
        }

        public static NV10PollEvent Parse(NV10Response response)
        {
            NV10PollEvent resp = null;
            NV10PollEventType type = NV10PollEventType.Ok;
            int data = -1;
            if (response._data.Length != 0)
            {
                type = (NV10PollEventType)response._data[0];                
                if (dataEvents.Contains(type))
                {
                    data = response._data[1];
                }
            }
            return new NV10PollEvent(type, data);
        }
    }
}
