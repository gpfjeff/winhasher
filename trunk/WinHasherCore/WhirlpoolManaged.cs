/* WhirlpoolManaged.cs
 * 
 * PROGRAMMER:    Jeffrey T. Darlington
 * DATE:          September, 2007
 * PROJECT:       WinHasher core library
 * .NET VERSION:  2.0
 * REQUIRES:      See using statements
 * REQUIRED BY:   All WinHasher programs
 * 
 * This class is an attemp to implement the Whirlpool cryptographic hash.  It is a subclass of
 * System.Security.Cryptography.HashAlgorithm, so it should be usable as a drop-in hash engine
 * with every other hash in WinHasher.
 * 
 * This code is essentially a port of the Java Whirlpool-0 code from Jacksum
 * (http://www.jonelo.de/java/jacksum/index.html) by Johann N. Loefflmann, which is in turn
 * heavily based on the GNU Crypto library.  Both are released under the GPL.
 * 
 * This program is Copyright 2007, Jeffrey T. Darlington.
 * E-mail:  jeff@gpf-comics.com
 * Web:     http://www.gpf-comics.com/
 * 
 * This program is free software; you can redistribute it and/or modify it under the terms of
 * the GNU General Public License as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
 * without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * See theGNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with this program;
 * if not, write to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
 * Boston, MA  02110-1301, USA.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace com.gpfcomics.WinHasher.Core
{
    public class WhirlpoolManaged : HashAlgorithm
    {
        private static int BLOCK_SIZE = 64;

        // The digest of the 0-bit long message.
        private static string DIGEST0 =
            "470F0409ABAA446E49667D4EBE12A14387CEDBD10DD17B8243CAD550A089DC0F" +
            "EEA7AA40F6C2AAAB71C6EBD076E43C7CFCA0AD32567897DCB5969861049A0F5A";

        // Default number of rounds:
        private static int R = 10;

        private static string Sd = // p. 19 [WHIRLPOOL]
            "\u1823\uc6E8\u87B8\u014F\u36A6\ud2F5\u796F\u9152" +
            "\u60Bc\u9B8E\uA30c\u7B35\u1dE0\ud7c2\u2E4B\uFE57" +
            "\u1577\u37E5\u9FF0\u4AdA\u58c9\u290A\uB1A0\u6B85" +
            "\uBd5d\u10F4\ucB3E\u0567\uE427\u418B\uA77d\u95d8" +
            "\uFBEE\u7c66\udd17\u479E\ucA2d\uBF07\uAd5A\u8333" +
            "\u6302\uAA71\uc819\u49d9\uF2E3\u5B88\u9A26\u32B0" +
            "\uE90F\ud580\uBEcd\u3448\uFF7A\u905F\u2068\u1AAE" +
            "\uB454\u9322\u64F1\u7312\u4008\uc3Ec\udBA1\u8d3d" +
            "\u9700\ucF2B\u7682\ud61B\uB5AF\u6A50\u45F3\u30EF" +
            "\u3F55\uA2EA\u65BA\u2Fc0\udE1c\uFd4d\u9275\u068A" +
            "\uB2E6\u0E1F\u62d4\uA896\uF9c5\u2559\u8472\u394c" +
            "\u5E78\u388c\ud1A5\uE261\uB321\u9c1E\u43c7\uFc04" +
            "\u5199\u6d0d\uFAdF\u7E24\u3BAB\ucE11\u8F4E\uB7EB" +
            "\u3c81\u94F7\uB913\u2cd3\uE76E\uc403\u5644\u7FA9" +
            "\u2ABB\uc153\udc0B\u9d6c\u3174\uF646\uAc89\u14E1" +
            "\u163A\u6909\u70B6\ud0Ed\ucc42\u98A4\u285c\uF886";

        //private static long[] T0 = new long[256];
        //private static long[] T1 = new long[256];
        //private static long[] T2 = new long[256];
        //private static long[] T3 = new long[256];
        //private static long[] T4 = new long[256];
        //private static long[] T5 = new long[256];
        //private static long[] T6 = new long[256];
        //private static long[] T7 = new long[256];
        //private static long[] rc = new long[R];
        private static ulong[] T0 = new ulong[256];
        private static ulong[] T1 = new ulong[256];
        private static ulong[] T2 = new ulong[256];
        private static ulong[] T3 = new ulong[256];
        private static ulong[] T4 = new ulong[256];
        private static ulong[] T5 = new ulong[256];
        private static ulong[] T6 = new ulong[256];
        private static ulong[] T7 = new ulong[256];
        private static ulong[] rc = new ulong[R];

        /** caches the result of the correctness test, once executed. */
        private static bool valid = false;

        /** The 512-bit context as 8 longs. */
        //private long H0, H1, H2, H3, H4, H5, H6, H7;
        private ulong H0, H1, H2, H3, H4, H5, H6, H7;

        /** Work area for computing the round key schedule. */
        //private long k00, k01, k02, k03, k04, k05, k06, k07;
        //private long Kr0, Kr1, Kr2, Kr3, Kr4, Kr5, Kr6, Kr7;
        private ulong k00, k01, k02, k03, k04, k05, k06, k07;
        private ulong Kr0, Kr1, Kr2, Kr3, Kr4, Kr5, Kr6, Kr7;

        /** work area for transforming the 512-bit buffer. */
        //private long n0, n1, n2, n3, n4, n5, n6, n7;
        //private long nn0, nn1, nn2, nn3, nn4, nn5, nn6, nn7;
        private ulong n0, n1, n2, n3, n4, n5, n6, n7;
        private ulong nn0, nn1, nn2, nn3, nn4, nn5, nn6, nn7;

        /** work area for holding block cipher's intermediate values. */
        //private long w0, w1, w2, w3, w4, w5, w6, w7;
        private ulong w0, w1, w2, w3, w4, w5, w6, w7;

        public WhirlpoolManaged()
        {
            HashSizeValue = 512;
            Initialize();
        }

        public override int InputBlockSize
        {
            get { return BLOCK_SIZE; }
        }

        public override int OutputBlockSize
        {
            get { return BLOCK_SIZE; }
        }

        public override byte[] Hash
        {
            get { return HashFinal(); }
        }

        public bool PassSelfTest
        {
            get
            {
                if (valid) return valid;
                else
                {
                    byte[] theHash = ComputeHash(new byte[0]);
                    StringBuilder sOutput = new StringBuilder(theHash.Length);
                    for (int i = 0; i < theHash.Length; i++)
                    {
                        sOutput.Append(theHash[i].ToString("x2"));
                    }
                    valid = sOutput.ToString().ToUpper().CompareTo(DIGEST0) == 0;
                    return valid;
                }
            }
        }

        #region HashAlgorithm Implemented Methods

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            //throw new HashEngineException("The method or operation is not implemented.");
            int offset = ibStart;
            // apply mu to the input
            n0 = (ulong)((array[offset++] & 0xFFL) << 56 | (array[offset++] & 0xFFL) << 48 |
                 (array[offset++] & 0xFFL) << 40 | (array[offset++] & 0xFFL) << 32 |
                 (array[offset++] & 0xFFL) << 24 | (array[offset++] & 0xFFL) << 16 |
                 (array[offset++] & 0xFFL) <<  8 | (array[offset++] & 0xFFL));
            n1 = (ulong)((array[offset++] & 0xFFL) << 56 | (array[offset++] & 0xFFL) << 48 |
                 (array[offset++] & 0xFFL) << 40 | (array[offset++] & 0xFFL) << 32 |
                 (array[offset++] & 0xFFL) << 24 | (array[offset++] & 0xFFL) << 16 |
                 (array[offset++] & 0xFFL) <<  8 | (array[offset++] & 0xFFL));
            n2 = (ulong)((array[offset++] & 0xFFL) << 56 | (array[offset++] & 0xFFL) << 48 |
                 (array[offset++] & 0xFFL) << 40 | (array[offset++] & 0xFFL) << 32 |
                 (array[offset++] & 0xFFL) << 24 | (array[offset++] & 0xFFL) << 16 |
                 (array[offset++] & 0xFFL) <<  8 | (array[offset++] & 0xFFL));
            n3 = (ulong)((array[offset++] & 0xFFL) << 56 | (array[offset++] & 0xFFL) << 48 |
                 (array[offset++] & 0xFFL) << 40 | (array[offset++] & 0xFFL) << 32 |
                 (array[offset++] & 0xFFL) << 24 | (array[offset++] & 0xFFL) << 16 |
                 (array[offset++] & 0xFFL) <<  8 | (array[offset++] & 0xFFL));
            n4 = (ulong)((array[offset++] & 0xFFL) << 56 | (array[offset++] & 0xFFL) << 48 |
                 (array[offset++] & 0xFFL) << 40 | (array[offset++] & 0xFFL) << 32 |
                 (array[offset++] & 0xFFL) << 24 | (array[offset++] & 0xFFL) << 16 |
                 (array[offset++] & 0xFFL) <<  8 | (array[offset++] & 0xFFL));
            n5 = (ulong)((array[offset++] & 0xFFL) << 56 | (array[offset++] & 0xFFL) << 48 |
                 (array[offset++] & 0xFFL) << 40 | (array[offset++] & 0xFFL) << 32 |
                 (array[offset++] & 0xFFL) << 24 | (array[offset++] & 0xFFL) << 16 |
                 (array[offset++] & 0xFFL) <<  8 | (array[offset++] & 0xFFL));
            n6 = (ulong)((array[offset++] & 0xFFL) << 56 | (array[offset++] & 0xFFL) << 48 |
                 (array[offset++] & 0xFFL) << 40 | (array[offset++] & 0xFFL) << 32 |
                 (array[offset++] & 0xFFL) << 24 | (array[offset++] & 0xFFL) << 16 |
                 (array[offset++] & 0xFFL) <<  8 | (array[offset++] & 0xFFL));
            n7 = (ulong)((array[offset++] & 0xFFL) << 56 | (array[offset++] & 0xFFL) << 48 |
                 (array[offset++] & 0xFFL) << 40 | (array[offset++] & 0xFFL) << 32 |
                 (array[offset++] & 0xFFL) << 24 | (array[offset++] & 0xFFL) << 16 |
                 (array[offset++] & 0xFFL) <<  8 | (array[offset++] & 0xFFL));

            // transform K into the key schedule Kr; 0 <= r <= R
            k00 = H0;
            k01 = H1;
            k02 = H2;
            k03 = H3;
            k04 = H4;
            k05 = H5;
            k06 = H6;
            k07 = H7;

            nn0 = n0 ^ k00;
            nn1 = n1 ^ k01;
            nn2 = n2 ^ k02;
            nn3 = n3 ^ k03;
            nn4 = n4 ^ k04;
            nn5 = n5 ^ k05;
            nn6 = n6 ^ k06;
            nn7 = n7 ^ k07;

            // intermediate cipher output
            w0 = w1 = w2 = w3 = w4 = w5 = w6 = w7 = 0L;

            for (int r = 0; r < R; r++)
            {
                // 1. compute intermediate round key schedule by applying ro[rc]
                // to the previous round key schedule --rc being the round constant
                Kr0 = T0[(int)((k00 >> 56) & 0xFFL)] ^ T1[(int)((k07 >> 48) & 0xFFL)] ^
                      T2[(int)((k06 >> 40) & 0xFFL)] ^ T3[(int)((k05 >> 32) & 0xFFL)] ^
                      T4[(int)((k04 >> 24) & 0xFFL)] ^ T5[(int)((k03 >> 16) & 0xFFL)] ^
                      T6[(int)((k02 >>  8) & 0xFFL)] ^ T7[(int)( k01        & 0xFFL)] ^
                      rc[r];

                Kr1 = T0[(int)((k01 >> 56) & 0xFFL)] ^ T1[(int)((k00 >> 48) & 0xFFL)] ^
                      T2[(int)((k07 >> 40) & 0xFFL)] ^ T3[(int)((k06 >> 32) & 0xFFL)] ^
                      T4[(int)((k05 >> 24) & 0xFFL)] ^ T5[(int)((k04 >> 16) & 0xFFL)] ^
                      T6[(int)((k03 >>  8) & 0xFFL)] ^ T7[(int)( k02        & 0xFFL)];

                Kr2 = T0[(int)((k02 >> 56) & 0xFFL)] ^ T1[(int)((k01 >> 48) & 0xFFL)] ^
                      T2[(int)((k00 >> 40) & 0xFFL)] ^ T3[(int)((k07 >> 32) & 0xFFL)] ^
                      T4[(int)((k06 >> 24) & 0xFFL)] ^ T5[(int)((k05 >> 16) & 0xFFL)] ^
                      T6[(int)((k04 >>  8) & 0xFFL)] ^ T7[(int)( k03        & 0xFFL)];

                Kr3 = T0[(int)((k03 >> 56) & 0xFFL)] ^ T1[(int)((k02 >> 48) & 0xFFL)] ^
                      T2[(int)((k01 >> 40) & 0xFFL)] ^ T3[(int)((k00 >> 32) & 0xFFL)] ^
                      T4[(int)((k07 >> 24) & 0xFFL)] ^ T5[(int)((k06 >> 16) & 0xFFL)] ^
                      T6[(int)((k05 >>  8) & 0xFFL)] ^ T7[(int)( k04        & 0xFFL)];

                Kr4 = T0[(int)((k04 >> 56) & 0xFFL)] ^ T1[(int)((k03 >> 48) & 0xFFL)] ^
                      T2[(int)((k02 >> 40) & 0xFFL)] ^ T3[(int)((k01 >> 32) & 0xFFL)] ^
                      T4[(int)((k00 >> 24) & 0xFFL)] ^ T5[(int)((k07 >> 16) & 0xFFL)] ^
                      T6[(int)((k06 >>  8) & 0xFFL)] ^ T7[(int)( k05        & 0xFFL)];

                Kr5 = T0[(int)((k05 >> 56) & 0xFFL)] ^ T1[(int)((k04 >> 48) & 0xFFL)] ^
                      T2[(int)((k03 >> 40) & 0xFFL)] ^ T3[(int)((k02 >> 32) & 0xFFL)] ^
                      T4[(int)((k01 >> 24) & 0xFFL)] ^ T5[(int)((k00 >> 16) & 0xFFL)] ^
                      T6[(int)((k07 >>  8) & 0xFFL)] ^ T7[(int)( k06        & 0xFFL)];

                Kr6 = T0[(int)((k06 >> 56) & 0xFFL)] ^ T1[(int)((k05 >> 48) & 0xFFL)] ^
                      T2[(int)((k04 >> 40) & 0xFFL)] ^ T3[(int)((k03 >> 32) & 0xFFL)] ^
                      T4[(int)((k02 >> 24) & 0xFFL)] ^ T5[(int)((k01 >> 16) & 0xFFL)] ^
                      T6[(int)((k00 >>  8) & 0xFFL)] ^ T7[(int)( k07        & 0xFFL)];

                Kr7 = T0[(int)((k07 >> 56) & 0xFFL)] ^ T1[(int)((k06 >> 48) & 0xFFL)] ^
                      T2[(int)((k05 >> 40) & 0xFFL)] ^ T3[(int)((k04 >> 32) & 0xFFL)] ^
                      T4[(int)((k03 >> 24) & 0xFFL)] ^ T5[(int)((k02 >> 16) & 0xFFL)] ^
                      T6[(int)((k01 >>  8) & 0xFFL)] ^ T7[(int)( k00        & 0xFFL)];

                k00 = Kr0;
                k01 = Kr1;
                k02 = Kr2;
                k03 = Kr3;
                k04 = Kr4;
                k05 = Kr5;
                k06 = Kr6;
                k07 = Kr7;

                // 2. incrementally compute the cipher output
                w0 = T0[(int)((nn0 >> 56) & 0xFFL)] ^ T1[(int)((nn7 >> 48) & 0xFFL)] ^
                     T2[(int)((nn6 >> 40) & 0xFFL)] ^ T3[(int)((nn5 >> 32) & 0xFFL)] ^
                     T4[(int)((nn4 >> 24) & 0xFFL)] ^ T5[(int)((nn3 >> 16) & 0xFFL)] ^
                     T6[(int)((nn2 >>  8) & 0xFFL)] ^ T7[(int)( nn1        & 0xFFL)] ^
                     Kr0;
                w1 = T0[(int)((nn1 >> 56) & 0xFFL)] ^ T1[(int)((nn0 >> 48) & 0xFFL)] ^
                     T2[(int)((nn7 >> 40) & 0xFFL)] ^ T3[(int)((nn6 >> 32) & 0xFFL)] ^
                     T4[(int)((nn5 >> 24) & 0xFFL)] ^ T5[(int)((nn4 >> 16) & 0xFFL)] ^
                     T6[(int)((nn3 >>  8) & 0xFFL)] ^ T7[(int)( nn2        & 0xFFL)] ^
                     Kr1;
                w2 = T0[(int)((nn2 >> 56) & 0xFFL)] ^ T1[(int)((nn1 >> 48) & 0xFFL)] ^
                     T2[(int)((nn0 >> 40) & 0xFFL)] ^ T3[(int)((nn7 >> 32) & 0xFFL)] ^
                     T4[(int)((nn6 >> 24) & 0xFFL)] ^ T5[(int)((nn5 >> 16) & 0xFFL)] ^
                     T6[(int)((nn4 >>  8) & 0xFFL)] ^ T7[(int)( nn3        & 0xFFL)] ^
                     Kr2;
                w3 = T0[(int)((nn3 >> 56) & 0xFFL)] ^ T1[(int)((nn2 >> 48) & 0xFFL)] ^
                     T2[(int)((nn1 >> 40) & 0xFFL)] ^ T3[(int)((nn0 >> 32) & 0xFFL)] ^
                     T4[(int)((nn7 >> 24) & 0xFFL)] ^ T5[(int)((nn6 >> 16) & 0xFFL)] ^
                     T6[(int)((nn5 >>  8) & 0xFFL)] ^ T7[(int)( nn4        & 0xFFL)] ^
                     Kr3;
                w4 = T0[(int)((nn4 >> 56) & 0xFFL)] ^ T1[(int)((nn3 >> 48) & 0xFFL)] ^
                     T2[(int)((nn2 >> 40) & 0xFFL)] ^ T3[(int)((nn1 >> 32) & 0xFFL)] ^
                     T4[(int)((nn0 >> 24) & 0xFFL)] ^ T5[(int)((nn7 >> 16) & 0xFFL)] ^
                     T6[(int)((nn6 >>  8) & 0xFFL)] ^ T7[(int)( nn5        & 0xFFL)] ^
                     Kr4;
                w5 = T0[(int)((nn5 >> 56) & 0xFFL)] ^ T1[(int)((nn4 >> 48) & 0xFFL)] ^
                     T2[(int)((nn3 >> 40) & 0xFFL)] ^ T3[(int)((nn2 >> 32) & 0xFFL)] ^
                     T4[(int)((nn1 >> 24) & 0xFFL)] ^ T5[(int)((nn0 >> 16) & 0xFFL)] ^
                     T6[(int)((nn7 >>  8) & 0xFFL)] ^ T7[(int)( nn6        & 0xFFL)] ^
                     Kr5;
                w6 = T0[(int)((nn6 >> 56) & 0xFFL)] ^ T1[(int)((nn5 >> 48) & 0xFFL)] ^
                     T2[(int)((nn4 >> 40) & 0xFFL)] ^ T3[(int)((nn3 >> 32) & 0xFFL)] ^
                     T4[(int)((nn2 >> 24) & 0xFFL)] ^ T5[(int)((nn1 >> 16) & 0xFFL)] ^
                     T6[(int)((nn0 >>  8) & 0xFFL)] ^ T7[(int)( nn7        & 0xFFL)] ^
                     Kr6;
                w7 = T0[(int)((nn7 >> 56) & 0xFFL)] ^ T1[(int)((nn6 >> 48) & 0xFFL)] ^
                     T2[(int)((nn5 >> 40) & 0xFFL)] ^ T3[(int)((nn4 >> 32) & 0xFFL)] ^
                     T4[(int)((nn3 >> 24) & 0xFFL)] ^ T5[(int)((nn2 >> 16) & 0xFFL)] ^
                     T6[(int)((nn1 >>  8) & 0xFFL)] ^ T7[(int)( nn0        & 0xFFL)] ^
                     Kr7;

                nn0 = w0;
                nn1 = w1;
                nn2 = w2;
                nn3 = w3;
                nn4 = w4;
                nn5 = w5;
                nn6 = w6;
                nn7 = w7;
            }

            // apply the Miyaguchi-Preneel hash scheme
            H0 ^= w0 ^ n0;
            H1 ^= w1 ^ n1;
            H2 ^= w2 ^ n2;
            H3 ^= w3 ^ n3;
            H4 ^= w4 ^ n4;
            H5 ^= w5 ^ n5;
            H6 ^= w6 ^ n6;
            H7 ^= w7 ^ n7;
        }

        protected override byte[] HashFinal()
        {
            //throw new HashEngineException("The method or operation is not implemented.");
                  // apply inverse mu to the context
            byte[] result = new byte[] {
                //(byte)(H0 >>> 56), (byte)(H0 >>> 48), (byte)(H0 >>> 40), (byte)(H0 >>> 32),
                //(byte)(H0 >>> 24), (byte)(H0 >>> 16), (byte)(H0 >>>  8), (byte) H0,
                //(byte)(H1 >>> 56), (byte)(H1 >>> 48), (byte)(H1 >>> 40), (byte)(H1 >>> 32),
                //(byte)(H1 >>> 24), (byte)(H1 >>> 16), (byte)(H1 >>>  8), (byte) H1,
                //(byte)(H2 >>> 56), (byte)(H2 >>> 48), (byte)(H2 >>> 40), (byte)(H2 >>> 32),
                //(byte)(H2 >>> 24), (byte)(H2 >>> 16), (byte)(H2 >>>  8), (byte) H2,
                //(byte)(H3 >>> 56), (byte)(H3 >>> 48), (byte)(H3 >>> 40), (byte)(H3 >>> 32),
                //(byte)(H3 >>> 24), (byte)(H3 >>> 16), (byte)(H3 >>>  8), (byte) H3,
                //(byte)(H4 >>> 56), (byte)(H4 >>> 48), (byte)(H4 >>> 40), (byte)(H4 >>> 32),
                //(byte)(H4 >>> 24), (byte)(H4 >>> 16), (byte)(H4 >>>  8), (byte) H4,
                //(byte)(H5 >>> 56), (byte)(H5 >>> 48), (byte)(H5 >>> 40), (byte)(H5 >>> 32),
                //(byte)(H5 >>> 24), (byte)(H5 >>> 16), (byte)(H5 >>>  8), (byte) H5,
                //(byte)(H6 >>> 56), (byte)(H6 >>> 48), (byte)(H6 >>> 40), (byte)(H6 >>> 32),
                //(byte)(H6 >>> 24), (byte)(H6 >>> 16), (byte)(H6 >>>  8), (byte) H6,
                //(byte)(H7 >>> 56), (byte)(H7 >>> 48), (byte)(H7 >>> 40), (byte)(H7 >>> 32),
                //(byte)(H7 >>> 24), (byte)(H7 >>> 16), (byte)(H7 >>>  8), (byte) H7
                (byte)(H0 >> 56), (byte)(H0 >> 48), (byte)(H0 >> 40), (byte)(H0 >> 32),
                (byte)(H0 >> 24), (byte)(H0 >> 16), (byte)(H0 >>  8), (byte) H0,
                (byte)(H1 >> 56), (byte)(H1 >> 48), (byte)(H1 >> 40), (byte)(H1 >> 32),
                (byte)(H1 >> 24), (byte)(H1 >> 16), (byte)(H1 >>  8), (byte) H1,
                (byte)(H2 >> 56), (byte)(H2 >> 48), (byte)(H2 >> 40), (byte)(H2 >> 32),
                (byte)(H2 >> 24), (byte)(H2 >> 16), (byte)(H2 >>  8), (byte) H2,
                (byte)(H3 >> 56), (byte)(H3 >> 48), (byte)(H3 >> 40), (byte)(H3 >> 32),
                (byte)(H3 >> 24), (byte)(H3 >> 16), (byte)(H3 >>  8), (byte) H3,
                (byte)(H4 >> 56), (byte)(H4 >> 48), (byte)(H4 >> 40), (byte)(H4 >> 32),
                (byte)(H4 >> 24), (byte)(H4 >> 16), (byte)(H4 >>  8), (byte) H4,
                (byte)(H5 >> 56), (byte)(H5 >> 48), (byte)(H5 >> 40), (byte)(H5 >> 32),
                (byte)(H5 >> 24), (byte)(H5 >> 16), (byte)(H5 >>  8), (byte) H5,
                (byte)(H6 >> 56), (byte)(H6 >> 48), (byte)(H6 >> 40), (byte)(H6 >> 32),
                (byte)(H6 >> 24), (byte)(H6 >> 16), (byte)(H6 >>  8), (byte) H6,
                (byte)(H7 >> 56), (byte)(H7 >> 48), (byte)(H7 >> 40), (byte)(H7 >> 32),
                (byte)(H7 >> 24), (byte)(H7 >> 16), (byte)(H7 >>  8), (byte) H7
            };
            return result;
        }

        public override void Initialize()
        {
            //int ROOT = 0x11d; // para. 2.1 [WHIRLPOOL]
            ulong ROOT = 0x11d; // para. 2.1 [WHIRLPOOL]
            //int i, r, j;
            uint i, r, j;
            //long s, s2, s3, s4, s5, s8, s9, t;
            ulong s, s2, s3, s4, s5, s8, s9, t;
            //char c;
            ushort c;
            byte[] S =  new byte[256];
            char[] Sda = Sd.ToCharArray();
            for (i = 0; i < 256; i++)
            {
                //c = Sda[i >>> 1];
                //c = Sda[UnsignedRightShift(i, 1)];
                c = Convert.ToUInt16(Sda[i >> 1]);
                //s = ((i & 1) == 0 ? c >>> 8 : c) & 0xFFL;
                s = (ulong)((i & 1) == 0 ? c >> 8 : c) & 0xFFL;
                s2 = s << 1;
                if (s2 > 0xFFL) s2 ^= ROOT;
                s3 = s2 ^ s;
                s4 = s2 << 1;
                if (s4 > 0xFFL) s4 ^= ROOT;
                s5 = s4 ^ s;
                s8 = s4 << 1;
                if (s8 > 0xFFL) s8 ^= ROOT;
                s9 = s8 ^ s;

                S[i] = (byte) s;
                T0[i] = t = s  << 56 | s  << 48 | s3 << 40 | s  << 32 |
                    s5 << 24 | s8 << 16 | s9 <<  8 | s5;
                //T1[i] = t >>>  8 | t << 56;
                //T2[i] = t >>> 16 | t << 48;
                //T3[i] = t >>> 24 | t << 40;
                //T4[i] = t >>> 32 | t << 32;
                //T5[i] = t >>> 40 | t << 24;
                //T6[i] = t >>> 48 | t << 16;
                //T7[i] = t >>> 56 | t << 8;
                //T1[i] = UnsignedRightShift(t,  8) | t << 56;
                //T2[i] = UnsignedRightShift(t, 16) | t << 48;
                //T3[i] = UnsignedRightShift(t, 24) | t << 40;
                //T4[i] = UnsignedRightShift(t, 32) | t << 32;
                //T5[i] = UnsignedRightShift(t, 40) | t << 24;
                //T6[i] = UnsignedRightShift(t, 48) | t << 16;
                //T7[i] = UnsignedRightShift(t, 56) | t << 8;
                T1[i] = t >>  8 | t << 56;
                T2[i] = t >> 16 | t << 48;
                T3[i] = t >> 24 | t << 40;
                T4[i] = t >> 32 | t << 32;
                T5[i] = t >> 40 | t << 24;
                T6[i] = t >> 48 | t << 16;
                T7[i] = t >> 56 | t << 8;
            }

            for (r = 1, i = 0, j = 0; r < R + 1; r++)
            {
                rc[i++] = (ulong)((S[j++] & 0xFFL) << 56 | (S[j++] & 0xFFL) << 48 |
                    (S[j++] & 0xFFL) << 40 | (S[j++] & 0xFFL) << 32 |
                    (S[j++] & 0xFFL) << 24 | (S[j++] & 0xFFL) << 16 |
                    (S[j++] & 0xFFL) <<  8 | (S[j++] & 0xFFL));
            }
        }

        #endregion

        #region Private Methods

        //private int UnsignedRightShift(int op, int numBits)
        //{
        //    int shifted = op >> numBits;
        //    return shifted;
        //}

        #endregion
    }
}
