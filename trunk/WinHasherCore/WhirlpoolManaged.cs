/* WhirlpoolManaged.cs
 * 
 * PROGRAMMER:    Jeffrey T. Darlington
 * DATE:          September 21, 2007
 * PROJECT:       WinHasher core library
 * .NET VERSION:  2.0
 * REQUIRES:      See using statements
 * REQUIRED BY:   All WinHasher programs
 * 
 * This class implements the Whirlpool cryptographic hash (the 2003 revision, not the original
 * 2000 "Whirlpool-0" version or the 2001 "Whirlpool-T" revision).  It is a subclass of
 * System.Security.Cryptography.HashAlgorithm, so it should be usable as a drop-in hash engine
 * with every other hash in WinHasher.
 * 
 * This is essentially a port of the Legion of the Bouncy Castle (http://www.bouncycastle.org/csharp/)
 * implementation of Whirlpool, taking their .NET 1.1 code with their own APIs and rewrapping
 * it as a subclass of the .NET 2.0 HashAlgorithm subclass.  As such, most of the code is actually
 * pretty much the same as the BC original; all I've really done is hide some of their public
 * methods that no longer apply and wrap the essential HashAlgorithm methods around them to fit
 * in the .NET 2.0 interface.  The BC code is actually a port of the orginal Java Whirlpool
 * code by Paulo S. L. M. Barreto and Vincent Rijmen to C#.  See
 * http://paginas.terra.com.br/informatica/paulobarreto/WhirlpoolPage.html
 * Barreto and Rijmen's code is in the public domain; the Bouncy Castle code is copyrighted but
 * released under a very permissive license (http://www.bouncycastle.org/csharp/licence.html).
 * Some of the original comments are retained, with additional comments by me.
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
    public sealed class WhirlpoolManaged : HashAlgorithm
    {
		// JTD: This is the block size:
        private const int BYTE_LENGTH = 64;

        // JTD: Some constants.  Digest length is the length of the resulting hash in bits; you can
        // guess what the bytes version is.  Whirlpool always uses 10 rounds.
        private const int DIGEST_LENGTH        = 512;
        private const int DIGEST_LENGTH_BYTES  = DIGEST_LENGTH / 8;
		private const int ROUNDS               = 10;
		private const int REDUCTION_POLYNOMIAL = 0x011d; // 2^8 + 2^4 + 2^3 + 2 + 1;

        // JTD: The substitution box:
		private static readonly int[] SBOX =
		{
			0x18, 0x23, 0xc6, 0xe8, 0x87, 0xb8, 0x01, 0x4f, 0x36, 0xa6, 0xd2, 0xf5, 0x79, 0x6f, 0x91, 0x52,
			0x60, 0xbc, 0x9b, 0x8e, 0xa3, 0x0c, 0x7b, 0x35, 0x1d, 0xe0, 0xd7, 0xc2, 0x2e, 0x4b, 0xfe, 0x57,
			0x15, 0x77, 0x37, 0xe5, 0x9f, 0xf0, 0x4a, 0xda, 0x58, 0xc9, 0x29, 0x0a, 0xb1, 0xa0, 0x6b, 0x85,
			0xbd, 0x5d, 0x10, 0xf4, 0xcb, 0x3e, 0x05, 0x67, 0xe4, 0x27, 0x41, 0x8b, 0xa7, 0x7d, 0x95, 0xd8,
			0xfb, 0xee, 0x7c, 0x66, 0xdd, 0x17, 0x47, 0x9e, 0xca, 0x2d, 0xbf, 0x07, 0xad, 0x5a, 0x83, 0x33,
			0x63, 0x02, 0xaa, 0x71, 0xc8, 0x19, 0x49, 0xd9, 0xf2, 0xe3, 0x5b, 0x88, 0x9a, 0x26, 0x32, 0xb0,
			0xe9, 0x0f, 0xd5, 0x80, 0xbe, 0xcd, 0x34, 0x48, 0xff, 0x7a, 0x90, 0x5f, 0x20, 0x68, 0x1a, 0xae,
			0xb4, 0x54, 0x93, 0x22, 0x64, 0xf1, 0x73, 0x12, 0x40, 0x08, 0xc3, 0xec, 0xdb, 0xa1, 0x8d, 0x3d,
			0x97, 0x00, 0xcf, 0x2b, 0x76, 0x82, 0xd6, 0x1b, 0xb5, 0xaf, 0x6a, 0x50, 0x45, 0xf3, 0x30, 0xef,
			0x3f, 0x55, 0xa2, 0xea, 0x65, 0xba, 0x2f, 0xc0, 0xde, 0x1c, 0xfd, 0x4d, 0x92, 0x75, 0x06, 0x8a,
			0xb2, 0xe6, 0x0e, 0x1f, 0x62, 0xd4, 0xa8, 0x96, 0xf9, 0xc5, 0x25, 0x59, 0x84, 0x72, 0x39, 0x4c,
			0x5e, 0x78, 0x38, 0x8c, 0xd1, 0xa5, 0xe2, 0x61, 0xb3, 0x21, 0x9c, 0x1e, 0x43, 0xc7, 0xfc, 0x04,
			0x51, 0x99, 0x6d, 0x0d, 0xfa, 0xdf, 0x7e, 0x24, 0x3b, 0xab, 0xce, 0x11, 0x8f, 0x4e, 0xb7, 0xeb,
			0x3c, 0x81, 0x94, 0xf7, 0xb9, 0x13, 0x2c, 0xd3, 0xe7, 0x6e, 0xc4, 0x03, 0x56, 0x44, 0x7f, 0xa9,
			0x2a, 0xbb, 0xc1, 0x53, 0xdc, 0x0b, 0x9d, 0x6c, 0x31, 0x74, 0xf6, 0x46, 0xac, 0x89, 0x14, 0xe1,
			0x16, 0x3a, 0x69, 0x09, 0x70, 0xb6, 0xd0, 0xed, 0xcc, 0x42, 0x98, 0xa4, 0x28, 0x5c, 0xf8, 0x86
		};

        // JTD: The round constants:
		private static readonly long[] C0 = new long[256];
		private static readonly long[] C1 = new long[256];
		private static readonly long[] C2 = new long[256];
		private static readonly long[] C3 = new long[256];
		private static readonly long[] C4 = new long[256];
		private static readonly long[] C5 = new long[256];
		private static readonly long[] C6 = new long[256];
		private static readonly long[] C7 = new long[256];

		private readonly long[] _rc = new long[ROUNDS + 1];

		/*
			* increment() can be implemented in this way using 2 arrays or
			* by having some temporary variables that are used to set the
			* value provided by EIGHT[i] and carry within the loop.
			*
			* not having done any timing, this seems likely to be faster
			* at the slight expense of 32*(sizeof short) bytes
			*/
		private static readonly short[] EIGHT = new short[BITCOUNT_ARRAY_SIZE];

        // JTD: Static initialization:
		static WhirlpoolManaged()
		{
			EIGHT[BITCOUNT_ARRAY_SIZE - 1] = 8;

			for (int i = 0; i < 256; i++)
			{
				int v1 = SBOX[i];
				int v2 = maskWithReductionPolynomial(v1 << 1);
				int v4 = maskWithReductionPolynomial(v2 << 1);
				int v5 = v4 ^ v1;
				int v8 = maskWithReductionPolynomial(v4 << 1);
				int v9 = v8 ^ v1;

				C0[i] = packIntoLong(v1, v1, v4, v1, v8, v5, v2, v9);
				C1[i] = packIntoLong(v9, v1, v1, v4, v1, v8, v5, v2);
				C2[i] = packIntoLong(v2, v9, v1, v1, v4, v1, v8, v5);
				C3[i] = packIntoLong(v5, v2, v9, v1, v1, v4, v1, v8);
				C4[i] = packIntoLong(v8, v5, v2, v9, v1, v1, v4, v1);
				C5[i] = packIntoLong(v1, v8, v5, v2, v9, v1, v1, v4);
				C6[i] = packIntoLong(v4, v1, v8, v5, v2, v9, v1, v1);
				C7[i] = packIntoLong(v1, v4, v1, v8, v5, v2, v9, v1);
			}
		}

        /// <summary>
        /// The WhirlpoolManaged constructor
        /// </summary>
		public WhirlpoolManaged()
		{
            // JTD: Added 9/21/2007.  It's probably inefficient to have to values holding the
            // same thing, but (a) I don't want to muck with the BC code too much lest I break
            // it, and (b) HashSizeValue is the .NET 2.0 HashAlgorithm interface for this value.
            HashSizeValue = DIGEST_LENGTH;

			_rc[0] = 0L;
			for (int r = 1; r <= ROUNDS; r++)
			{
				int i = 8 * (r - 1);
				_rc[r] = (long)((ulong)C0[i] & 0xff00000000000000L) ^
					(C1[i + 1] & (long) 0x00ff000000000000L) ^
					(C2[i + 2] & (long) 0x0000ff0000000000L) ^
					(C3[i + 3] & (long) 0x000000ff00000000L) ^
					(C4[i + 4] & (long) 0x00000000ff000000L) ^
					(C5[i + 5] & (long) 0x0000000000ff0000L) ^
					(C6[i + 6] & (long) 0x000000000000ff00L) ^
					(C7[i + 7] & (long) 0x00000000000000ffL);
			}
		}

        /// <summary>
        /// Packs a serious of integer values into a single long.  The ints are used to prevent
        /// sign extension; the inputs are really bytes (0..255).
        /// </summary>
        /// <param name="b7"></param>
        /// <param name="b6"></param>
        /// <param name="b5"></param>
        /// <param name="b4"></param>
        /// <param name="b3"></param>
        /// <param name="b2"></param>
        /// <param name="b1"></param>
        /// <param name="b0"></param>
        /// <returns></returns>
		private static long packIntoLong(int b7, int b6, int b5, int b4, int b3, int b2,
            int b1, int b0)
		{
			return
				((long)b7 << 56) ^
				((long)b6 << 48) ^
				((long)b5 << 40) ^
				((long)b4 << 32) ^
				((long)b3 << 24) ^
				((long)b2 << 16) ^
				((long)b1 <<  8) ^
				b0;
		}

        /// <summary>
        /// (?) Ints are used to prevent sign extension.  The values that are really being used are
		///	actually just 0..255
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
		private static int maskWithReductionPolynomial(int input)
		{
			int rv = input;
			if (rv >= 0x100L) // high bit set
			{
				rv ^= REDUCTION_POLYNOMIAL; // reduced by the polynomial
			}
			return rv;
		}

		// --------------------------------------------------------------------------------------//

		// -- buffer information --
		private const int BITCOUNT_ARRAY_SIZE = 32;
		private byte[]  _buffer    = new byte[64];
		private int     _bufferPos;
		private short[] _bitCount  = new short[BITCOUNT_ARRAY_SIZE];

		// -- internal hash state --
		private long[] _hash  = new long[8];
		private long[] _K = new long[8]; // the round key
		private long[] _L = new long[8];
		private long[] _block = new long[8]; // mu (buffer)
		private long[] _state = new long[8]; // the current "cipher" state



        // JTD: Commented out 9/21/2007; a copy constructor is not needed, nor required by
        // HashAlgorithm
        //public WhirlpoolManaged(WhirlpoolManaged originalDigest)
        //{
        //    Array.Copy(originalDigest._rc, 0, _rc, 0, _rc.Length);

        //    Array.Copy(originalDigest._buffer, 0, _buffer, 0, _buffer.Length);

        //    this._bufferPos = originalDigest._bufferPos;
        //    Array.Copy(originalDigest._bitCount, 0, _bitCount, 0, _bitCount.Length);

        //    // -- internal hash state --
        //    Array.Copy(originalDigest._hash, 0, _hash, 0, _hash.Length);
        //    Array.Copy(originalDigest._K, 0, _K, 0, _K.Length);
        //    Array.Copy(originalDigest._L, 0, _L, 0, _L.Length);
        //    Array.Copy(originalDigest._block, 0, _block, 0, _block.Length);
        //    Array.Copy(originalDigest._state, 0, _state, 0, _state.Length);
        //}

        // JTD: Commented out 9/21/2007; not needed
        //public string AlgorithmName
        //{
        //    get { return "Whirlpool"; }
        //}

        // JTD: Converted to private 9/21/2007, does same basic thing as HashSize, but returns
        // bytes.  Used in original code, but not needed outside.
        /// <summary>
        /// Get the length of the digest in bytes; same as HashSize
        /// </summary>
        /// <returns>The length of the digest in bytes</returns>
        //public int GetDigestSize()
        private int GetDigestSize()
		{
			return DIGEST_LENGTH_BYTES;
		}

        // JTD: Converted to private 9/21/2007, does the same basic thing as HashFinal(), so we'll
        // wrap HashFinal() around this.
        /// <summary>
        /// Finalize the hash digest
        /// </summary>
        /// <param name="output">The final digest as an array of raw bytes</param>
        /// <param name="outOff">The offset</param>
        /// <returns>The size of the digest in bytes</returns>
        //public int DoFinal(byte[] output, int outOff)
        private int DoFinal(byte[] output, int outOff)
		{
			// sets output[outOff] .. output[outOff+DIGEST_LENGTH_BYTES]
			finish();
			for (int i = 0; i < 8; i++)
			{
				convertLongToByteArray(_hash[i], output, outOff + (i * 8));
			}
			Reset();
			return GetDigestSize();
		}

		/**
			* Reset the chaining variables
			*/
        // JTD: Converted to private 9/21/2007, does the same basic thing as Initialize() (?)
        // so we'll just wrap Initialize() around it.
		/// <summary>
		/// Reset the hash engine to its default state; same as Initialize()
		/// </summary>
        //public void Reset()
        private void Reset()
		{
			// set variables to null, blank, whatever
			_bufferPos = 0;
			Array.Clear(_bitCount, 0, _bitCount.Length);
			Array.Clear(_buffer, 0, _buffer.Length);
			Array.Clear(_hash, 0, _hash.Length);
			Array.Clear(_K, 0, _K.Length);
			Array.Clear(_L, 0, _L.Length);
			Array.Clear(_block, 0, _block.Length);
			Array.Clear(_state, 0, _state.Length);
		}

		/// <summary>
        /// Takes a buffer of information and fills the block
		/// </summary>
        private void processFilledBuffer()
		{
			// copies into the block...
			for (int i = 0; i < _state.Length; i++)
			{
				_block[i] = bytesToLongFromBuffer(_buffer, i * 8);
			}
			processBlock();
			_bufferPos = 0;
			Array.Clear(_buffer, 0, _buffer.Length);
		}

        /// <summary>
        /// Take a sequence of bytes from the buffer and convert them to a long
        /// </summary>
        /// <param name="buffer">The buffer to read the bytes from</param>
        /// <param name="startPos">The starting position to read from the buffer</param>
        /// <returns>The long value as read from the buffer</returns>
		private static long bytesToLongFromBuffer(byte[] buffer, int startPos)
		{
			long rv = (((buffer[startPos + 0] & 0xffL) << 56) |
				((buffer[startPos + 1] & 0xffL) << 48) |
				((buffer[startPos + 2] & 0xffL) << 40) |
				((buffer[startPos + 3] & 0xffL) << 32) |
				((buffer[startPos + 4] & 0xffL) << 24) |
				((buffer[startPos + 5] & 0xffL) << 16) |
				((buffer[startPos + 6] & 0xffL) <<  8) |
				((buffer[startPos + 7]) & 0xffL));

			return rv;
		}

        /// <summary>
        /// Convert a long value into a sequence of bytes
        /// </summary>
        /// <param name="inputLong">The long value to convert</param>
        /// <param name="outputArray">The byte array to stuff the long into</param>
        /// <param name="offSet">The offset of the byte array to start placing the bytes into</param>
		private static void convertLongToByteArray(long inputLong, byte[] outputArray, int offSet)
		{
			for (int i = 0; i < 8; i++)
			{
				outputArray[offSet + i] = (byte)((inputLong >> (56 - (i * 8))) & 0xff);
			}
		}

        /// <summary>
        /// ???
        /// </summary>
		private void processBlock()
		{
			// buffer contents have been transferred to the _block[] array via
			// processFilledBuffer

			// compute and apply K^0
			for (int i = 0; i < 8; i++)
			{
				_state[i] = _block[i] ^ (_K[i] = _hash[i]);
			}

			// iterate over the rounds
			for (int round = 1; round <= ROUNDS; round++)
			{
				for (int i = 0; i < 8; i++)
				{
					_L[i] = 0;
					_L[i] ^= C0[(int)(_K[(i - 0) & 7] >> 56) & 0xff];
					_L[i] ^= C1[(int)(_K[(i - 1) & 7] >> 48) & 0xff];
					_L[i] ^= C2[(int)(_K[(i - 2) & 7] >> 40) & 0xff];
					_L[i] ^= C3[(int)(_K[(i - 3) & 7] >> 32) & 0xff];
					_L[i] ^= C4[(int)(_K[(i - 4) & 7] >> 24) & 0xff];
					_L[i] ^= C5[(int)(_K[(i - 5) & 7] >> 16) & 0xff];
					_L[i] ^= C6[(int)(_K[(i - 6) & 7] >>  8) & 0xff];
					_L[i] ^= C7[(int)(_K[(i - 7) & 7]) & 0xff];
				}

				Array.Copy(_L, 0, _K, 0, _K.Length);

				_K[0] ^= _rc[round];

				// apply the round transformation
				for (int i = 0; i < 8; i++)
				{
					_L[i] = _K[i];

					_L[i] ^= C0[(int)(_state[(i - 0) & 7] >> 56) & 0xff];
					_L[i] ^= C1[(int)(_state[(i - 1) & 7] >> 48) & 0xff];
					_L[i] ^= C2[(int)(_state[(i - 2) & 7] >> 40) & 0xff];
					_L[i] ^= C3[(int)(_state[(i - 3) & 7] >> 32) & 0xff];
					_L[i] ^= C4[(int)(_state[(i - 4) & 7] >> 24) & 0xff];
					_L[i] ^= C5[(int)(_state[(i - 5) & 7] >> 16) & 0xff];
					_L[i] ^= C6[(int)(_state[(i - 6) & 7] >> 8) & 0xff];
					_L[i] ^= C7[(int)(_state[(i - 7) & 7]) & 0xff];
				}

				// save the current state
				Array.Copy(_L, 0, _state, 0, _state.Length);
			}

			// apply Miuaguchi-Preneel compression
			for (int i = 0; i < 8; i++)
			{
				_hash[i] ^= _state[i] ^ _block[i];
			}

		}

        // JTD: Converted to private 9/21/2007; no HashAlgorithm equivalent
        /// <summary>
        /// ???
        /// </summary>
        /// <param name="input">???</param>
		//public void Update(byte input)
		private void Update(byte input)
		{
			_buffer[_bufferPos] = input;

			//Console.WriteLine("adding to buffer = "+_buffer[_bufferPos]);

			++_bufferPos;

			if (_bufferPos == _buffer.Length)
			{
				processFilledBuffer();
			}

			increment();
		}

        /// <summary>
        /// ???
        /// </summary>
		private void increment()
		{
			int carry = 0;
			for (int i = _bitCount.Length - 1; i >= 0; i--)
			{
				int sum = (_bitCount[i] & 0xff) + EIGHT[i] + carry;

				carry = sum >> 8;
				_bitCount[i] = (short)(sum & 0xff);
			}
		}

        // JTD:  Converted to private 9/21/2007, does the same basic thing as HashCore() (?),
        // so wrap HashCore() around it.
        /// <summary>
        /// Update a block of bytes; same as HashCore()
        /// </summary>
        /// <param name="input">The byte array to update</param>
        /// <param name="inOff">The starting offset</param>
        /// <param name="length">The length of the array to process</param>
        //public void BlockUpdate(byte[] input, int inOff, int length)
        private void BlockUpdate(byte[] input, int inOff, int length)
		{
			while (length > 0)
			{
				Update(input[inOff]);
				++inOff;
				--length;
			}

		}

        /// <summary>
        /// ???
        /// </summary>
		private void finish()
		{
			/*
				* this makes a copy of the current bit length. at the expense of an
				* object creation of 32 bytes rather than providing a _stopCounting
				* boolean which was the alternative I could think of.
				*/
			byte[] bitLength = copyBitLength();

			_buffer[_bufferPos++] |= 0x80;

			if (_bufferPos == _buffer.Length)
			{
				processFilledBuffer();
			}

			/*
				* Final block contains
				* [ ... data .... ][0][0][0][ length ]
				*
				* if [ length ] cannot fit.  Need to create a new block.
				*/
			if (_bufferPos > 32)
			{
				while (_bufferPos != 0)
				{
					Update((byte)0);
				}
			}

			while (_bufferPos <= 32)
			{
				Update((byte)0);
			}

			// copy the length information to the final 32 bytes of the
			// 64 byte block....
			Array.Copy(bitLength, 0, _buffer, 32, bitLength.Length);

			processFilledBuffer();
		}

        /// <summary>
        /// ???
        /// </summary>
        /// <returns>???</returns>
		private byte[] copyBitLength()
		{
			byte[] rv = new byte[BITCOUNT_ARRAY_SIZE];
			for (int i = 0; i < rv.Length; i++)
			{
				rv[i] = (byte)(_bitCount[i] & 0xff);
			}
			return rv;
		}

        // Commented out JTD 9/21/2007; not needed, same thing has HashSize
        //public int GetByteLength()
        //{
        //    return BYTE_LENGTH;
        //}

        #region HashAlgorithm Overrides

        // These properties are provided by HashAlgorithm and are expected to be overloaded
        // by subclasses if they differ from the base.  HashAlgorithm defines block sizes
        // as 1 by default; Whirlpool uses a block size of 64.  So we need to override these
        // to present the right values.

        /// <summary>
        /// The input block size
        /// </summary>
        public override int InputBlockSize
        {
            get { return BYTE_LENGTH; }
        }

        /// <summary>
        /// The ouput block size
        /// </summary>
        public override int OutputBlockSize
        {
            get { return BYTE_LENGTH; }
        }

        #endregion

        #region HashAlgorithm Method Implementations

        // The following abstract methods must be implemented by all subclasses of HashAlgorithm.
        // These are heart and soul of how a .NET HashAlgorithm works; all the other concrete
        // methods like ComputeHash() call these.  Also note they are usually called in this order:
        //   *  HashCore(/* Inputs as appropriate */);
        //   *  HashValue = HashFinal();
        //   *  Initialize();
        //   *  return HashValue;
        // This information is derived from the following:
        // http://www.koders.com/csharp/fid982B41B76FF0AD12F3B48324704427E24F42EAD8.aspx?s=md5

        /// <summary>
        /// Routes data written to the object into the hash algorithm for computing the hash
        /// </summary>
        /// <param name="array">The input to compute the hash code for</param>
        /// <param name="ibStart">The offset into the byte array from which to begin using data</param>
        /// <param name="cbSize">The number of bytes in the byte array to use as data</param>
        protected override void  HashCore(byte[] array, int ibStart, int cbSize)
        {
 	        // The BC code has already implemented this, so we'll just wrap around it:
            BlockUpdate(array, ibStart, cbSize);
        }

        /// <summary>
        /// Finalizes the hash computation after the last data is processed by the
        /// cryptographic stream object
        /// </summary>
        /// <returns>The computed hash code</returns>
        protected override byte[]  HashFinal()
        {
            // The BC code has already implemented this, but not quite in the way HashAlgorithm
            // expects.  DoFinal() does the real work, but it returns an int, the size of the
            // digest in bytes.  We don't care about that value, just the byte array.  So create
            // a new byte array, fill it, and pass it back out.  Note that we don't set the
            // HashAlgorithm.Hash property here; that's actually done by the calling code, like
            // HashAgorithm.ComputeHash().  However, every time HashFinal() is called, it's
            // setting HashValue, which is a protected field that's wrapped with the public
            // property Hash.  Confused yet?
            byte[] theHash = new byte[DIGEST_LENGTH_BYTES];
            DoFinal(theHash, 0);
            return theHash;
        }

        /// <summary>
        /// Initializes an implementation of the HashAlgorithm class
        /// </summary>
        public override void  Initialize()
        {
            // The BC code has already implemented this, so we'll just wrap around it:
            Reset();
        }

        #endregion
    }
}
