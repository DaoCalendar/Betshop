using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Numerics;

namespace eSSPNV10
{
    internal class SSPEncryptor
    {
        internal readonly BigInteger Generator = 139791917;
        internal readonly BigInteger Modulus = 769202429;
        private readonly BigInteger _hostSecret = 0xFF000FF;
        //private readonly BigInteger _hostSecret;

        private readonly AesManaged _aesManaged = new AesManaged();
        private ICryptoTransform _transCry = null;
        private ICryptoTransform _transDec = null;
        private readonly Int32 PRIME_LEN = 4;

        public SSPEncryptor()
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            //_hostSecret = new BigInteger(rnd.Next(0x00, 0xFFFFFFF));

            Int32 mod = rnd.Next(5, (MainResource.primes.Length / PRIME_LEN) - 1);
            Int32 gen = rnd.Next(1, mod);

            //Modulus = (UInt64)BitConverter.ToUInt32(MainResource.primes, mod * PRIME_LEN);
            //Generator = (UInt64)BitConverter.ToUInt32(MainResource.primes, gen * PRIME_LEN);
        }

        internal BigInteger HostExchangeKey
        {
            get
            {
                return BigInteger.ModPow(Generator, _hostSecret, Modulus);
            }
        }

        internal void SolveKey(UInt64 inKey)
        {
            if (inKey == 0)
            {
                throw new ArgumentException("key is 0");
            }

            _aesManaged.BlockSize = 128;
            _aesManaged.IV = new Byte[16];
            _aesManaged.KeySize = 128;
            _aesManaged.Mode = CipherMode.ECB;
            _aesManaged.Padding = PaddingMode.None;//ISO10126;

            Byte[] synchKey = new Byte[16];

            UInt64 keyGen = inKey;
                //(UInt64)GetKey(BitConverter.GetBytes(inExchangeKey), _hostSecret, Modulus);
            UInt64 keyFix = 0x0123456701234567;

            Byte[] tempH = new Byte[8];
            tempH = BitConverter.GetBytes(keyGen);

            Byte[] tempL = new Byte[8];
            tempL = BitConverter.GetBytes(keyFix);


            Array.Copy(tempH, 0, synchKey, 8, 8);
            Array.Copy(tempL, 0, synchKey, 0, 8);

            _aesManaged.Key = synchKey;
            _transCry = _aesManaged.CreateEncryptor();
            _transDec = _aesManaged.CreateDecryptor();
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

        internal Byte[] Crypt(byte[] inputBuffer)
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

        internal Byte[] Decrypt(byte[] inputBuffer)
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
