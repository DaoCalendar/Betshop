using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace eSSPNV10
{
    internal class SSPCryptManager
    {
        private readonly SSPEncryptor _key = new SSPEncryptor();
        private UInt32 _cryptCounter = 0;
        public Boolean Inited { get; set; }
        
        //private readonly Logger _logger;
        internal static readonly Byte STEX = 0x7E;

        public SSPCryptManager()//SSPTransportManager inTransport)//, Logger inLogger)
        {
            Inited = false;
            //_transport = inTransport;
            //_logger = inLogger;
        }


        //private SSPCommand SendData(SSPData inData)
        //{
        //    SSPCommand res;
        //    Debug.Print(String.Format("{0}\t{1}\t{2}\n", this, ">", inData));
        //    res = _transport.SendData(inData);
        //    Debug.Print(String.Format("{0}\t{1}\t{2}\n", this, "<", res.SmileData));
        //    return res;
        //}

        public Byte[] Crypt(SSPData inData)
        {
            SSPCryptedData crData = new SSPCryptedData(inData, (UInt32)_cryptCounter);
            _cryptCounter++;   


            Byte[] crypted = _key.Crypt(crData.RawData);
            Byte[] toSend = new Byte[crypted.Length + 1];

            toSend[0] = SSPCryptManager.STEX;
            Array.Copy(crypted, 0, toSend, 1, crypted.Length);
            return toSend;
        }

        public SSPData Decrypt(Byte[] inData)
        {
            //List<Byte> decr = new List<byte>(res.Data);
            //decr.RemoveAt(0);
            if (inData[0] != STEX)
            {
                throw new Exception("STEX not received");
            }

            List<Byte> data2decr = new List<Byte>(inData);
            data2decr.RemoveAt(0);
            Byte[] result = _key.Decrypt(data2decr.ToArray());
            if (result.Length == 0)
            {
                throw new Exception("decryption error");
                //result = _key.Decrypt(data2decr.ToArray());
            }
            SSPCryptedData crypData = new SSPCryptedData(result);

            //Console.WriteLine("exp: {0}; was: {1}", _cryptCounter, crypData.Counter);
            //_cryptCounter++;
            if (crypData.Counter != _cryptCounter)// && (crypData.Counter != 1))
            {
                //throw new Exception("Invalid crypt counter");
                Console.WriteLine("exp: {0}; was: {1}", _cryptCounter, crypData.Counter);
                if (crypData.Counter < _cryptCounter)
                {
                    _cryptCounter = crypData.Counter;
                }
            }
            //else
            //{
            //   _cryptCounter++;
            //}

            return new SSPData(crypData.Data);
        }

        internal SSPSetGeneratorData GetSSPGeneratorData()
        {
            return new SSPSetGeneratorData(_key.Generator.ToByteArray());
        }

        internal SSPSetModulusData GetSSPModulusData()
        {
            return new SSPSetModulusData(_key.Modulus.ToByteArray());
        }

        internal SSPSetExchangeKeyData GetSSPExchangeData()
        {
            return new SSPSetExchangeKeyData(_key.HostExchangeKey.ToByteArray());
        }

        internal void SolveSyncKey(Byte[] inData)
        {
            _key.SolveSynchKey(new SSPSecurityData(inData).GetSecureData());
        } 
       
        public override string ToString()
        {
            return "SSPCryptManager";
        }
    }
}
