﻿using System;
using System.Text;

using NUnit.Framework;

using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
    [TestFixture]
    public class Blake2bDigestTest 
	    : SimpleTest
    {
        private static readonly string[,] keyedTestVectors = { // input/message, key, hash
            // Vectors from BLAKE2 web site: https://blake2.net/blake2b-test.txt
            {
                "",
                "000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f202122232425262728292a2b2c2d2e2f303132333435363738393a3b3c3d3e3f",
                "10ebb67700b1868efb4417987acf4690ae9d972fb7a590c2f02871799aaa4786b5e996e8f0f4eb981fc214b005f42d2ff4233499391653df7aefcbc13fc51568"
            },
            {
                "00",
                "000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f202122232425262728292a2b2c2d2e2f303132333435363738393a3b3c3d3e3f",
                "961f6dd1e4dd30f63901690c512e78e4b45e4742ed197c3c5e45c549fd25f2e4187b0bc9fe30492b16b0d0bc4ef9b0f34c7003fac09a5ef1532e69430234cebd"
            },
            {
                "0001",
                "000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f202122232425262728292a2b2c2d2e2f303132333435363738393a3b3c3d3e3f",
                "da2cfbe2d8409a0f38026113884f84b50156371ae304c4430173d08a99d9fb1b983164a3770706d537f49e0c916d9f32b95cc37a95b99d857436f0232c88a965"
            },
            {
                "000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f202122232425262728292a2b2c2d2e2f303132333435363738393a3b3c3d3e3f404142434445464748494a4b4c4d",
                "000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f202122232425262728292a2b2c2d2e2f303132333435363738393a3b3c3d3e3f",
                "f1aa2b044f8f0c638a3f362e677b5d891d6fd2ab0765f6ee1e4987de057ead357883d9b405b9d609eea1b869d97fb16d9b51017c553f3b93c0a1e0f1296fedcd"
            },
            {
                "000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f202122232425262728292a2b2c2d2e2f303132333435363738393a3b3c3d3e3f404142434445464748494a4b4c4d4e4f505152535455565758595a5b5c5d5e5f606162636465666768696a6b6c6d6e6f707172737475767778797a7b7c7d7e7f808182838485868788898a8b8c8d8e8f909192939495969798999a9b9c9d9e9fa0a1a2a3",
                "000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f202122232425262728292a2b2c2d2e2f303132333435363738393a3b3c3d3e3f",
                "c230f0802679cb33822ef8b3b21bf7a9a28942092901d7dac3760300831026cf354c9232df3e084d9903130c601f63c1f4a4a4b8106e468cd443bbe5a734f45f"
            },
            {
                "000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f202122232425262728292a2b2c2d2e2f303132333435363738393a3b3c3d3e3f404142434445464748494a4b4c4d4e4f505152535455565758595a5b5c5d5e5f606162636465666768696a6b6c6d6e6f707172737475767778797a7b7c7d7e7f808182838485868788898a8b8c8d8e8f909192939495969798999a9b9c9d9e9fa0a1a2a3a4a5a6a7a8a9aaabacadaeafb0b1b2b3b4b5b6b7b8b9babbbcbdbebfc0c1c2c3c4c5c6c7c8c9cacbcccdcecfd0d1d2d3d4d5d6d7d8d9dadbdcdddedfe0e1e2e3e4e5e6e7e8e9eaebecedeeeff0f1f2f3f4f5f6f7f8f9fafbfcfdfe",
                "000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f202122232425262728292a2b2c2d2e2f303132333435363738393a3b3c3d3e3f",
                "142709d62e28fcccd0af97fad0f8465b971e82201dc51070faa0372aa43e92484be1c1e73ba10906d5d1853db6a4106e0a7bf9800d373d6dee2d46d62ef2a461"
            }
        };

        // from: http://fossies.org/linux/john/src/rawBLAKE2_512_fmt_plug.c
        private static readonly string[,] unkeyedTestVectors = { // hash, input/message
            // digests without leading $BLAKE2$
            {
                "4245af08b46fbb290222ab8a68613621d92ce78577152d712467742417ebc1153668f1c9e1ec1e152a32a9c242dc686d175e087906377f0c483c5be2cb68953e",
                "blake2"
            },
            {
                "021ced8799296ceca557832ab941a50b4a11f83478cf141f51f933f653ab9fbcc05a037cddbed06e309bf334942c4e58cdf1a46e237911ccd7fcf9787cbc7fd0",
                "hello world"
            },
            {
                "1f7d9b7c9a90f7bfc66e52b69f3b6c3befbd6aee11aac860e99347a495526f30c9e51f6b0db01c24825092a09dd1a15740f0ade8def87e60c15da487571bcef7",
                "verystrongandlongpassword"
            },
            {
                "a8add4bdddfd93e4877d2746e62817b116364a1fa7bc148d95090bc7333b3673f82401cf7aa2e4cb1ecd90296e3f14cb5413f8ed77be73045b13914cdcd6a918",
                "The quick brown fox jumps over the lazy dog"
            },
            {
                "786a02f742015903c6c6fd852552d272912f4740e15847618a86e217f71f5419d25e1031afee585313896444934eb04b903a685b1448b755d56f701afe9be2ce",
                ""
            },
            {
                "ba80a53f981c4d0d6a2797b69f12f6e94c212f14685ac4b74b12bb6fdbffa2d17d87c5392aab792dc252d5de4533cc9518d38aa8dbf1925ab92386edd4009923",
                "abc"
            },
        };

        public override string Name
        {
            get { return "BLAKE2b"; }
        }

        private void offsetTest(
            IDigest digest,
            byte[] input,
            byte[] expected)
        {
            byte[] resBuf = new byte[expected.Length + 11];

            digest.BlockUpdate(input, 0, input.Length);

            digest.DoFinal(resBuf, 11);

            if (!AreEqual(Arrays.CopyOfRange(resBuf, 11, resBuf.Length), expected))
            {
                Fail("Offset failed got " + Hex.ToHexString(resBuf));
            }
        }

        public override void PerformTest()
        {
            // test keyed test vectors:

            Blake2bDigest blake2bkeyed = new Blake2bDigest(Hex.Decode(keyedTestVectors[0, 1]));
            for (int tv = 0; tv < keyedTestVectors.GetLength(0); tv++)
            {
                byte[] input = Hex.Decode(keyedTestVectors[tv, 0]);
                blake2bkeyed.Reset();

                blake2bkeyed.BlockUpdate(input, 0, input.Length);
                byte[] keyedHash = new byte[64];
                blake2bkeyed.DoFinal(keyedHash, 0);

                if (!Arrays.AreEqual(Hex.Decode(keyedTestVectors[tv, 2]), keyedHash))
                {
                    Fail("BLAKE2b mismatch on test vector ", keyedTestVectors[tv, 2], Hex.ToHexString(keyedHash));
                }

                offsetTest(blake2bkeyed, input, keyedHash);
            }

            Blake2bDigest blake2bunkeyed = new Blake2bDigest();
            // test unkeyed test vectors:
            for (int i = 0; i < unkeyedTestVectors.GetLength(0); i++)
            {
                // test update(byte b)
                byte[] unkeyedInput = Encoding.UTF8.GetBytes(unkeyedTestVectors[i, 1]);
                for (int j = 0; j < unkeyedInput.Length; j++)
                {
                    blake2bunkeyed.Update(unkeyedInput[j]);
                }

                byte[] unkeyedHash = new byte[64];
                blake2bunkeyed.DoFinal(unkeyedHash, 0);
                blake2bunkeyed.Reset();

                if (!Arrays.AreEqual(Hex.Decode(unkeyedTestVectors[i, 0]), unkeyedHash))
                {
                    Fail("BLAKE2b mismatch on test vector ", unkeyedTestVectors[i, 0], Hex.ToHexString(unkeyedHash));
                }
            }

            CloneTest();
            ResetTest();
            DoTestNullKeyVsUnkeyed();
            DoTestLengthConstruction();

            DigestTest.SpanConsistencyTests(this, new Blake2bDigest(512));
        }

        private void CloneTest()
        {
            Blake2bDigest blake2bCloneSource = new Blake2bDigest(Hex.Decode(keyedTestVectors[3, 1]), 16,
                Hex.Decode("000102030405060708090a0b0c0d0e0f"), Hex.Decode("101112131415161718191a1b1c1d1e1f"));
            byte[] expected = Hex.Decode("b6d48ed5771b17414c4e08bd8d8a3bc4");

            CheckClone(blake2bCloneSource, expected);

            // just digest size
            blake2bCloneSource = new Blake2bDigest(160);
            expected = Hex.Decode("64202454e538279b21cea0f5a7688be656f8f484");
            CheckClone(blake2bCloneSource, expected);

            // null salt and personalisation
            blake2bCloneSource = new Blake2bDigest(Hex.Decode(keyedTestVectors[3, 1]), 16, null, null);
            expected = Hex.Decode("2b4a081fae2d7b488f5eed7e83e42a20");
            CheckClone(blake2bCloneSource, expected);

            // null personalisation
            blake2bCloneSource = new Blake2bDigest(Hex.Decode(keyedTestVectors[3, 1]), 16, Hex.Decode("000102030405060708090a0b0c0d0e0f"), null);
            expected = Hex.Decode("00c3a2a02fcb9f389857626e19d706f6");
            CheckClone(blake2bCloneSource, expected);

            // null salt
            blake2bCloneSource = new Blake2bDigest(Hex.Decode(keyedTestVectors[3, 1]), 16, null, Hex.Decode("101112131415161718191a1b1c1d1e1f"));
            expected = Hex.Decode("f445ec9c062a3c724f8fdef824417abb");
            CheckClone(blake2bCloneSource, expected);
        }

        private void CheckClone(Blake2bDigest blake2bCloneSource, byte[] expected)
        {
            byte[] message = Hex.Decode(keyedTestVectors[3, 0]);

            blake2bCloneSource.BlockUpdate(message, 0, message.Length);

            byte[] hash = new byte[blake2bCloneSource.GetDigestSize()];

            Blake2bDigest digClone = new Blake2bDigest(blake2bCloneSource);

            blake2bCloneSource.DoFinal(hash, 0);
            if (!AreEqual(expected, hash))
            {
                Fail("clone source not correct");
            }

            digClone.DoFinal(hash, 0);
            if (!AreEqual(expected, hash))
            {
                Fail("clone not correct");
            }
        }

        private void DoTestLengthConstruction()
        {
            try
            {
                new Blake2bDigest(-1);
                Fail("no exception");
            }
            catch (ArgumentException e)
            {
                IsEquals("BLAKE2b digest bit length must be a multiple of 8 and not greater than 512", e.Message);
            }

            try
            {
                new Blake2bDigest(9);
                Fail("no exception");
            }
            catch (ArgumentException e)
            {
                IsEquals("BLAKE2b digest bit length must be a multiple of 8 and not greater than 512", e.Message);
            }

            try
            {
                new Blake2bDigest(520);
                Fail("no exception");
            }
            catch (ArgumentException e)
            {
                IsEquals("BLAKE2b digest bit length must be a multiple of 8 and not greater than 512", e.Message);
            }

            try
            {
                new Blake2bDigest(null, -1, null, null);
                Fail("no exception");
            }
            catch (ArgumentException e)
            {
                IsEquals("Invalid digest length (required: 1 - 64)", e.Message);
            }

            try
            {
                new Blake2bDigest(null, 65, null, null);
                Fail("no exception");
            }
            catch (ArgumentException e)
            {
                IsEquals("Invalid digest length (required: 1 - 64)", e.Message);
            }
        }

        private void DoTestNullKeyVsUnkeyed()
        {
            byte[] abc = Strings.ToByteArray("abc");

            for (int i = 1; i != 64; i++)
            {
                Blake2bDigest dig1 = new Blake2bDigest(i * 8);
                Blake2bDigest dig2 = new Blake2bDigest(null, i, null, null);

                byte[] out1 = new byte[i];
                byte[] out2 = new byte[i];

                dig1.BlockUpdate(abc, 0, abc.Length);
                dig2.BlockUpdate(abc, 0, abc.Length);

                dig1.DoFinal(out1, 0);
                dig2.DoFinal(out2, 0);

                IsTrue(Arrays.AreEqual(out1, out2));
            }
        }

        private void ResetTest()
        {
            // Generate a non-zero key
            byte[] key = new byte[32];
            for (byte i = 0; i < key.Length; i++)
            {
                key[i] = i;
            }
            // Generate some non-zero input longer than the key
            byte[] input = new byte[key.Length + 1];
            for (byte i = 0; i < input.Length; i++)
            {
                input[i] = i;
            }
            // Hash the input
            Blake2bDigest digest = new Blake2bDigest(key);
            digest.BlockUpdate(input, 0, input.Length);
            byte[] hash = new byte[digest.GetDigestSize()];
            digest.DoFinal(hash, 0);
            // Using a second instance, hash the input without calling doFinal()
            Blake2bDigest digest1 = new Blake2bDigest(key);
            digest1.BlockUpdate(input, 0, input.Length);
            // Reset the second instance and hash the input again
            digest1.Reset();
            digest1.BlockUpdate(input, 0, input.Length);
            byte[] hash1 = new byte[digest.GetDigestSize()];
            digest1.DoFinal(hash1, 0);
            // The hashes should be identical
            if (!Arrays.AreEqual(hash, hash1))
            {
                Fail("state was not reset");
            }
        }

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
		}

        [Test, Explicit]
        public void Bench()
        {
            var digest = new Blake2bDigest();

            byte[] data = new byte[1024];
            for (int i = 0; i < 1024; ++i)
            {
                for (int j = 0; j < 1024; ++j)
                {
                    // NOTE: .NET Core 3.1 has Span<T>, but is tested against our .NET Standard 2.0 assembly.
//#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
#if NET6_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                    digest.BlockUpdate(data);
#else
                    digest.BlockUpdate(data, 0, 1024);
#endif
                }

                // NOTE: .NET Core 3.1 has Span<T>, but is tested against our .NET Standard 2.0 assembly.
//#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
#if NET6_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                digest.DoFinal(data);
#else
                digest.DoFinal(data, 0);
#endif
            }
        }
    }
}
