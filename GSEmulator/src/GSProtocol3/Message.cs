using System;
using System.IO;
using System.Text;
using System.Linq;
using NLog;

namespace GSEmulator.GSProtocol3
{
    /// <summary>
    /// A representation of a gamespy query's reply.
    /// 
    /// The message has the follow structure:
    ///  - 0x00 XX XX XX XX splitnum 0x00 YY PAYLOAD
    /// 
    /// Where:
    /// - The first 4 bytes are a timestamp, provided by the query
    /// - splitnum is that byte array representation of "splitnum"
    /// - YY is the 15th byte  and has dual information:
    ///   - The 7 most significat bits indicate the index of the message
    ///   - The least significat bit indicates if this is the last message
    /// </summary>
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

        /// <summary>
        /// Defines the index of this message. 
        /// </summary>
        public byte Index
        {
            get
            {
                // As per documentation the index is stored in the 7 most significant bits of the 15th byte
                return (byte)(mPack[14] & 0x7f);
            }
            set
            {
                byte val = (byte)(mPack[14] & 0x80);
                val |= (byte)(value & 0x7f);
                mPack[14] = val;
 
            }
        }

        /// <summary>
        /// Defines if this is or not the last message.    
        /// </summary>
        public bool IsLast
        {
            get
            {
                // As per documentation the index is stored in the least significant bit of the 15th byte
                return (mPack[14] & 0x80) == 0x80;
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

        /// <summary>
        /// Writes a single byte to the buffer and increments the position by 1;
        /// </summary>
        /// <param name="value">byte to be written in the buffer</param>
        /// <returns>1 if the byte was writen else it returns 0</returns>
        public int put(byte value)
        {
            if (mPosition < MAX_SIZE) {
                mPack[mPosition++] = value;

                return 1;
            }
            return 0;
        }

        /// <summary>
        /// Writes the byte array representation of the given string to the buffer and increments the position accordingly.
        /// </summary>
        /// <param name="value">String to be written in the buffer</param>
        /// <returns>The number of written bytes</returns>
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


        /// <summary>
        /// Writes the byte array to the buffer and increments the position accordingly.
        /// </summary>
        /// <param name="bytes">byte array to be written in the buffer</param>
        /// <returns>The number of written bytes</returns>
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

        /// <summary>
        /// Returns a trimmed byte array representation of this message.
        /// 
        /// It also guarantees that the 1400th byte is always zero.
        /// </summary>
        /// <returns>Returns a trimmed byte array representation of this message.</returns>
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
