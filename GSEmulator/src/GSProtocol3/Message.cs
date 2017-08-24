using System;
using System.IO;
using System.Text;
using System.Linq;
using NLog;

namespace GSEmulator.GSProtocol3
{
    class Message
    {
        private static Logger LOGGER = LogManager.GetCurrentClassLogger();
        public static readonly int MAX_SIZE = 1400;
        public static readonly int TYPE_HEADERS = 0;
        public static readonly int TYPE_PLAYERS = 1;
        public static readonly int TYPE_TEAM = 2;
        private static readonly byte[] SPLITNUM = Encoding.ASCII.GetBytes("splitnum");
        private readonly byte[] mPack;
        private int mPosition;

        public Message(byte[] timestamp)
        {
            if (timestamp.Length != 4)
            {
                throw new ArgumentOutOfRangeException("Timestamp needs to be exactly 4 bytes");
            }

            // Initializes a new non-resizable
            mPack = new byte[MAX_SIZE];
            mPosition = 0;

            // Confirm position is at zero
            put(0);

            // TimeStamp Reply
            put(timestamp[0]);
            put(timestamp[1]);
            put(timestamp[2]);
            put(timestamp[3]);

            // SplitNum
            put(SPLITNUM);

            // SplitNum terminator
            put(0);

            // Number of message
            // We don't know yet lets just put zero
            put(0);
        }


        public byte Index
        {
            get
            {
                return (byte)(mPack[14] & 0x7f);
            }
            set
            {
                byte val = (byte)(mPack[14] & 0x80);
                val |= (byte)(value & 0x7f);
                mPack[14] = val;
 
            }
        }

        public bool IsLast
        {
            get
            {
                bool isLast = (mPack[14] & 0x80) == 0x80;
                return isLast;
            }
            set
            {
                int val = mPack[14];

                if (value)
                    mPack[14] = (byte)(val | 0x80);
                else
                    mPack[14] = (byte)(val & 0x7f);
            }
        }

        public int put(byte value)
        {
            if (mPosition < MAX_SIZE) {
                mPack[mPosition++] = value;

                return 1;
            }
            return 0;
        }

        public int put(string value)
        {
            int written = 0;
            var bytes = Encoding.ASCII.GetBytes(value);

            foreach (byte b in bytes)
            {
                if (put(b) != 1)
                    break;
                else
                    written++;
            }
            return written;
        }

        public int put(byte[] bytes)
        {
            int written = 0;
            foreach (byte b in bytes)
            {
                if (put(b) != 1)
                    break;
                else
                    written++;
            }
            return written;
        }

        public byte[] GetBytes()
        {
            mPack[MAX_SIZE - 1] = 0;


            return mPack
                .Take(mPosition)
                .ToArray();
        }


        public override string ToString()
        {
            string ret = "";
            for (int i = 0; i < mPosition; i++)
            {
                ret += String.Format(" {0:X2}", mPack[i]);
            }

            return ret;
        }
    }
}
