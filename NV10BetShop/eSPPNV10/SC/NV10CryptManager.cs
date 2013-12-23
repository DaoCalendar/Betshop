using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Security.Cryptography;

namespace eSSPNV10.SC
{
    public class NV10CryptManager
    {
        internal readonly BigInteger Generator;
        internal readonly BigInteger Modulus;
        private readonly BigInteger _hostSecret;
        private readonly Int32 PRIME_LEN = 4;
        private static readonly Int32 CRC_LEN = 2;
        internal static readonly Byte STEX = 0x7E;
        private UInt32 _cryptCounter = 0;

        private static readonly Random _rnd = new Random(DateTime.Now.Millisecond);
        private readonly AesManaged _aesManaged = new AesManaged();
        private ICryptoTransform _transCry = null;
        private ICryptoTransform _transDec = null;

        public Boolean Inited { get; set; }

        internal BigInteger HostExchangeKey
        {
            get
            {
                return BigInteger.ModPow(Generator, _hostSecret, Modulus);
            }
        }

        public NV10CryptManager()
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            _hostSecret = new BigInteger(rnd.Next(0x00, 0xFFFFFFF));

            Int32 mod = rnd.Next(5, (MainResource.primes.Length / PRIME_LEN) - 1);
            Int32 gen = rnd.Next(1, mod);

            Modulus = (UInt64)BitConverter.ToUInt32(MainResource.primes, mod * PRIME_LEN);
            Generator = (UInt64)BitConverter.ToUInt32(MainResource.primes, gen * PRIME_LEN);
        }

        internal void SolveSynchKey(UInt64 inExchangeKey)
        {
            if (inExchangeKey == 0)
            {
                throw new ArgumentException("Slave exchange key is 0");
            }

            _aesManaged.BlockSize = 128;
            _aesManaged.IV = new Byte[16];
            _aesManaged.KeySize = 128;
            _aesManaged.Mode = CipherMode.ECB;
            _aesManaged.Padding = PaddingMode.None;
            //ISO10126;

            Byte[] synchKey = new Byte[16];

            Byte[] keyGen = BigInteger.ModPow(new BigInteger(inExchangeKey), _hostSecret, Modulus).ToByteArray();
            //(UInt64)GetKey(BitConverter.GetBytes(inExchangeKey), _hostSecret, Modulus);
            UInt64 keyFix = 0x0123456701234567;

            Byte[] tempH = new Byte[8];
            if (keyGen.Length <= 8)
            {
                Array.Copy(keyGen, 0, tempH, 0, keyGen.Length);
            }
            else
            {
                Array.Copy(keyGen, keyGen.Length - 8, tempH, 0, 8);
            }

            Byte[] tempL = new Byte[8];
            tempL = BitConverter.GetBytes(keyFix);


            Array.Copy(tempH, 0, synchKey, 8, 8);
            Array.Copy(tempL, 0, synchKey, 0, 8);

            _aesManaged.Key = synchKey;
            _transCry = _aesManaged.CreateEncryptor();
            _transDec = _aesManaged.CreateDecryptor();
        }

        internal Byte[] prepareBytesForCrypting(Byte[] dataForCrypt, UInt32 inCounter)
        {
            List<Byte> _data = new List<Byte>(1 + 8);
            if (dataForCrypt == null)
            {
                throw new ArgumentNullException("dataForCrypt");
            }

            _data.Add((Byte)dataForCrypt.Length);
            _data.AddRange(BitConverter.GetBytes(inCounter));
            _data.AddRange(dataForCrypt);
            
            while ((_data.Count + CRC_LEN) % 16 != 0)
            {
                _data.Add((Byte)_rnd.Next(0x00, 0xFF));
            }

            _data.AddRange(NV10CRCTools.GetCRC(_data.ToArray()));
            return _data.ToArray();
        }

        public Byte[] Crypt(Byte[] inData)
        {
            Byte[] crData = prepareBytesForCrypting(inData, (UInt32)_cryptCounter);
            _cryptCounter++;


            Byte[] crypted = CryptFinal(crData);
            Byte[] toSend = new Byte[crypted.Length + 1];

            toSend[0] = NV10CryptManager.STEX;
            Array.Copy(crypted, 0, toSend, 1, crypted.Length);
            return toSend;
        }


        internal Byte[] CryptFinal(byte[] inputBuffer)
        {
            if (_transCry == null)
            {
                throw new NullReferenceException("_cryTrans is null");
            }
            Byte[] outputBuffer = _transCry.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
            //new Byte[128];
            //int len = _transCry.TransformBlock(inputBuffer, 0, inputBuffer.Length, outputBuffer, 0);
            //Byte[] result = new Byte[len];
            //Array.Copy(outputBuffer, result, len);
            return outputBuffer;
        }

        public Byte[] Decrypt(Byte[] inData)
        {
            //List<Byte> decr = new List<byte>(res.Data);
            //decr.RemoveAt(0);
            if (inData[0] != STEX)
            {
                throw new Exception("STEX not received");
            }

            List<Byte> data2decr = new List<Byte>(inData);
            data2decr.RemoveAt(0);
            Byte[] result = DecryptFinal(data2decr.ToArray());
            if (result.Length == 0)
            {
                throw new Exception("decryption error");
                //result = _key.Decrypt(data2decr.ToArray());
            }
            if (!NV10CRCTools.IsCRCValid(result))
            {
                throw new Exception("CRC is invalid while decrypting!");
            }

            Byte crypDataCounter = result[1];
            if (crypDataCounter != _cryptCounter)// && (crypData.Counter != 1))
            {
                if (crypDataCounter < _cryptCounter)
                {
                    _cryptCounter = crypDataCounter;
                }
            }
            //else
            //{
            //   _cryptCounter++;
            //}

            //return new SSPData(crypData.Data);
            return result;
        }

        internal Byte[] DecryptFinal(byte[] inputBuffer)
        {
            if (_transDec == null)
            {
                throw new NullReferenceException("_transDec is null");
            }
            Byte[] outputBuffer;// = new Byte[128];
            //int len = _transDec.TransformBlock(inputBuffer, 0, inputBuffer.Length, outputBuffer, 0);
            outputBuffer = _transDec.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
            //TransformBlock(inputBuffer, 0, inputBuffer.Length, outputBuffer, 0);
            //Byte[] result = new Byte[len];
            //. Array.Copy(outputBuffer, result, len);
            //return result;
            return outputBuffer;
        }

    }
}
