using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Listener
{
    internal class ModelData
    {
        const int subsequence_1 = 2;
        const int subsequence_2 = 8;
        const int subsequence_3 = 4;
        const int subsequence_4 = 1;
        const int subsequence_5 = 8;
        public ushort m_LenMess { get; set; }
        public long m_Unixtime { get; set; }
        public uint m_EmulyatorID { get; set; }
        public List<byte> m_Types { get; set; } = new List<byte>();
        public List<double> m_Values { get; set; } = new List<double>();

        public ModelData(NetworkStream stream)
        {
            List<byte> localBytes = new List<byte>();
            for (int i = 0; i < subsequence_1; i++)
            {
                localBytes.Add((byte)(stream.ReadByte()));
            }
            m_LenMess = BitConverter.ToUInt16(localBytes.ToArray(), 0);
            localBytes.Clear();

            for (int i = 0; i < subsequence_2; i++)
            {
                localBytes.Add((byte)(stream.ReadByte()));
            }
            m_Unixtime = BitConverter.ToInt64(localBytes.ToArray(), 0);
            localBytes.Clear();

            for (int i = 0; i < subsequence_3; i++)
            {
                localBytes.Add((byte)(stream.ReadByte()));
            }
            m_EmulyatorID = BitConverter.ToUInt32(localBytes.ToArray(), 0);
            localBytes.Clear();

            for (int i = 0; i < (m_LenMess - subsequence_2 - subsequence_3)/(subsequence_4 + subsequence_5); i++)
            {
                m_Types.Add((byte)(stream.ReadByte()));
                for (int j = 0;j< subsequence_5; j++)
                {
                    localBytes.Add((byte)(stream.ReadByte()));
                }
                m_Values.Add(BitConverter.ToDouble(localBytes.ToArray(), 0));
                localBytes.Clear();
            }
        }
    }
    
}
