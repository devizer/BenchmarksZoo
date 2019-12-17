//
// System.Buffer.cs
//
// Authors:
//   Paolo Molaro (lupus@ximian.com)
//   Dan Lewis (dihlewis@yahoo.co.uk)
//
// (C) 2001 Ximian, Inc.  http://www.ximian.com
//

//
// Copyright (C) 2004 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;

namespace BenchmarksZoo.ClassicAlgorithms.Cryptography
{


	public sealed class ByteBuffer
	{

		private ByteBuffer()
		{
		}

		public static unsafe void BlockCopy(byte[] src, int srcOffset, byte[] dest, int destOffset, int count)
		{
			ValidateArguments(src, srcOffset, dest, destOffset, count);
			
			fixed(byte* argSrc = &src[0])
			fixed (byte* argDest = &dest[0])
			{
				byte* ptrSrc = argSrc + srcOffset;
				byte* ptrDest = argDest + destOffset;
				while (count >= 8)
				{
					*(long*) ptrDest = *(long*) ptrSrc;
					ptrSrc += 8;
					ptrDest += 8;
					count -= 8;
				}
				
				while (count-- > 0)
					*ptrDest++ = *ptrSrc;
			}
		}

		public static void BlockCopy_Slow(byte[] src, int srcOffset, byte[] dest, int destOffset, int count)
		{
			ValidateArguments(src, srcOffset, dest, destOffset, count);

			for (int i = 0; i < count; i++)
			{
				dest[destOffset + i] = src[srcOffset + i];
			}
		}

		private static void ValidateArguments(byte[] src, int srcOffset, byte[] dest, int destOffset, int count)
		{
			if (src == null)
				throw new ArgumentNullException("src");

			if (dest == null)
				throw new ArgumentNullException("dest");

			if (srcOffset < 0)
				throw new ArgumentOutOfRangeException("srcOffset",
					"Non-negative number required.");

			if (destOffset < 0)
				throw new ArgumentOutOfRangeException("destOffset",
					"Non-negative number required.");

			if (count < 0)
				throw new ArgumentOutOfRangeException("count",
					"Non-negative number required.");

			if ((srcOffset > src.Length - count) || (destOffset > dest.Length - count))
				throw new ArgumentException(
					"Offset and length were out of bounds for the array or count is greater than" +
					"the number of elements from index to the end of the source collection.");
		}
	}
}
