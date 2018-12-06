using System;
using System.Text;
namespace Game
{
public class RofBuffRow : IRofBase
{
public int ID { get; private set; }
public string Name { get; private set; }
public long AvatarID { get; private set; }
public float Scale { get; private set; }
public int ReadBody(byte[] rData, int nOffset)
{
if (BitConverter.IsLittleEndian){Array.Reverse(rData, nOffset, 4);}
ID = (int)BitConverter.ToUInt32(rData, nOffset); nOffset += 4;
if (BitConverter.IsLittleEndian){Array.Reverse(rData, nOffset, 4);}
int nNameLen = (int)BitConverter.ToUInt32(rData, nOffset); nOffset += 4;
Name = Encoding.UTF8.GetString(rData, nOffset, nNameLen); nOffset += nNameLen;
if (BitConverter.IsLittleEndian){Array.Reverse(rData, nOffset, 8);}
AvatarID = (long)BitConverter.ToUInt64(rData, nOffset); nOffset += 8;
if (BitConverter.IsLittleEndian){Array.Reverse(rData, nOffset, 4);}
Scale = BitConverter.ToSingle(rData, nOffset); nOffset += 4;
return nOffset;
}
}
}
