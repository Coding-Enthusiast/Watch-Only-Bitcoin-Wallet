using System;
using System.Collections.Generic;
using System.Linq;

namespace BitcoinLibrary
{
    public static class SegWitAddress
    {
        public enum NetworkType
        {
            MainNet,
            TestNet
        }
        private const string CharSet = "qpzry9x8gf2tvdw0s3jn54khce6mua7l";
        private static readonly uint[] Generator = { 0x3b6a57b2, 0x26508e6d, 0x1ea119fa, 0x3d4233dd, 0x2a1462b3 };



        private static uint Polymod(byte[] values)
        {
            uint chk = 1;
            foreach (var value in values)
            {
                var top = chk >> 25;
                chk = value ^ ((chk & 0x1ffffff) << 5);
                foreach (var i in Enumerable.Range(0, 5))
                {
                    chk ^= ((top >> i) & 1) == 1 ? Generator[i] : 0;
                }
            }
            return chk;
        }


        private static byte[] ExpandHrp(string hrp)
        {
            int len = hrp.Length;
            byte[] hrpExpand = new byte[(2 * len) + 1];
            for (int i = 0; i < len; i++)
            {
                hrpExpand[i] = (byte)("bc"[i] >> 5);
                hrpExpand[i + len + 1] = (byte)("bc"[i] & 31);
            }
            return hrpExpand;
        }


        private static bool VerifyChecksum(byte[] data, string hrp)
        {
            var values = ExpandHrp(hrp).Concat(data).ToArray();
            var polymod = Polymod(values) ^ 1;
            if (polymod != 0)
            {
                return false;
            }
            return true;
        }


        private static byte[] ConvertBits(IEnumerable<byte> data, int fromBits, int toBits, bool pad = true)
        {
            var acc = 0;
            var bits = 0;
            var maxv = (1 << toBits) - 1;
            var ret = new List<byte>();
            foreach (var value in data)
            {
                if ((value >> fromBits) > 0)
                {
                    return null;
                }
                acc = (acc << fromBits) | value;
                bits += fromBits;
                while (bits >= toBits)
                {
                    bits -= toBits;
                    ret.Add((byte)((acc >> bits) & maxv));
                }
            }
            if (pad)
            {
                if (bits > 0)
                {
                    ret.Add((byte)((acc << (toBits - bits)) & maxv));
                }
            }
            else if (bits >= fromBits || (byte)(((acc << (toBits - bits)) & maxv)) != 0)
            {
                return null;
            }
            return ret.ToArray();
        }


        /// <summary>
        /// Checks to see if a given string (bitcoin address) is a valid Bech32 SegWit address.
        /// </summary>
        /// <param name="btcAddress">Bitcoin address to check</param>
        /// <returns>True if Bech32 encoded</returns>
        public static VerificationResult Verify(string btcAddress, NetworkType nt)
        {
            VerificationResult result = new VerificationResult() { IsVerified = false };

            string hrp = (nt == NetworkType.MainNet) ? "bc" : "tb";
            if (!btcAddress.StartsWith(hrp, StringComparison.InvariantCultureIgnoreCase))
            {
                result.Error = "Invalid Human Readable Part!";
                return result;
            }
            // Reject short or long
            if (btcAddress.Length < 14 && btcAddress.Length > 74)
            {
                result.Error = "Invalid length!";
                return result;
            }
            // Reject mix case (Invariant is used to pass the "Turkey test")
            if (!btcAddress.ToUpperInvariant().Equals(btcAddress) && !btcAddress.ToLowerInvariant().Equals(btcAddress))
            {
                result.Error = "Mix case is not allowed!";
                return result;
            }
            // For checksum purposes only lower case is used.
            btcAddress = btcAddress.ToLowerInvariant();
            // Check separator
            int separatorPos = btcAddress.LastIndexOf("1", StringComparison.OrdinalIgnoreCase);
            if (separatorPos < 1 || separatorPos + 7 > btcAddress.Length)
            {
                result.Error = "Separator is either missing or misplaced!";
                return result;
            }
            // Check characters
            if (btcAddress.Substring(separatorPos + 1).ToList().Any(x => !CharSet.Contains(x)))
            {
                result.Error = "Invalid characters!";
                return result;
            }
            // Check Human Readable Part
            string hrpGot = btcAddress.Substring(0, separatorPos);
            if (!hrp.Equals(hrpGot))
            {
                result.Error = "Invalid Human Readable Part!";
                return result;
            }


            string dataStr = btcAddress.Substring(separatorPos + 1);
            byte[] dataBa = new byte[dataStr.Length];
            for (int i = 0; i < dataStr.Length; i++)
            {
                dataBa[i] = (byte)CharSet.IndexOf(dataStr[i]);
            }

            // Verify checksum
            if (!VerifyChecksum(dataBa, hrp))
            {
                result.Error = "Invalid checksum!";
                return result;
            }

            byte[] dataNew = dataBa.Take(dataBa.Length - 6).ToArray();
            byte[] decoded = ConvertBits(dataNew.Skip(1), 5, 8, false);
            if (decoded == null)
            {
                result.Error = "Invalid Bech32 string!";
                return result;
            }
            if (decoded.Length < 2 || decoded.Length > 40)
            {
                result.Error = "Invalid decoded data length!";
                return result;
            }

            byte witnessVerion = dataNew[0];
            if (witnessVerion > 16)
            {
                result.Error = "Invalid decoded witness version!";
                return result;
            }

            if (witnessVerion == 0 && decoded.Length != 20 && decoded.Length != 32)
            {
                result.Error = "Invalid length of witness program!";
                return result;
            }


            result.IsVerified = true;
            return result;
        }



    }
}
