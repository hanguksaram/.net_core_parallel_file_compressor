using System;

namespace Gzip.Test
{
    internal static class Utils
    {
        internal static byte[] GetBytesToStore(int length)
        {
            int lengthToStore = System.Net.IPAddress.HostToNetworkOrder(length);
            byte[] lengthInBytes = BitConverter.GetBytes(lengthToStore);
            string base64Enc = Convert.ToBase64String(lengthInBytes);
            byte[] finalStore = System.Text.Encoding.ASCII.GetBytes(base64Enc);
            return finalStore;
        }
        internal static int GetLengthFromBytes(byte[] intToParse)
        {
            string base64Enc = System.Text.Encoding.ASCII.GetString(intToParse);
            byte[] normStr = Convert.FromBase64String(base64Enc);
            int length = BitConverter.ToInt32(normStr, 0);

            return System.Net.IPAddress.NetworkToHostOrder(length);
        }
    }
}