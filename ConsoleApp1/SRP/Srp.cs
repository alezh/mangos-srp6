using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.SRP
{
    public class Srp
    {
        #region Modulus(N), salt(s), generator(g), multiplier(k), Identity Hash ,Verifier(v)
        public BigInteger Modulus
        {
            get;
            private set;
        }

        public BigInteger Salt
        {
            get;
            private set;
        }

        public BigInteger Generator
        {
            get;
            private set;
        }

        public BigInteger Multiplier
        {
            get;
            private set;
        }

        public byte[] IdentityHash
        {
            get;
            private set;
        }
        public BigInteger Verifier
        {
            get;
            private set;
        }
        public BigInteger SaltedIdentityHash
        {
            get;
            private set;
        }

        public BigInteger Scrambler
        {
            get;
            private set;
        }
        #endregion

        public Srp(byte[] identityhash, string modulus, string generator)
        {
            Modulus = modulus.CreateBigInteger(16);
            Generator = generator.CreateBigInteger(16);
            IdentityHash = identityhash;
        }

        /// <summary>
        /// 构建SRP6
        /// </summary>
        /// <param name="identityhash"></param>
        /// <param name="modulus"></param>
        /// <param name="generator"></param>
        /// <param name="saltSize"></param>
        /// <param name="salt"></param>
        public Srp(byte[] identityhash, string modulus, string generator, int saltSize, string salt = "") :
            this(identityhash, modulus, generator)
        {
            if (string.IsNullOrWhiteSpace(salt))
            {
                Salt = SrpHash.CreateBigInteger(256, new Random());
            }
            else
            {
                Salt = salt.CreateBigInteger(16);
            }
            ComputeVerifier();
        }

        public void ComputeVerifier()
        {
            var I = SrpHash.CreateBigInteger(IdentityHash.ToHexString(), 16);
            var ii = I.ToUnsignedByteArray();
            Array.Reverse(ii);
            byte[] x = SrpHash.Concatenate(Salt.ToUnsignedByteArray(), ii).HashEncode();
            Array.Reverse(x);
            SaltedIdentityHash = x.ToHexString().CreateBigInteger(16);
            Verifier = Generator.ModPow(SaltedIdentityHash, Modulus);
        }

        public bool ProofVerifier(String v)
        {
            var vi = v.CreateBigInteger(16);
            if (Verifier.CompareTo(vi) == 0)
            {
                return true;
            }
            return false;
        }
    }
}
