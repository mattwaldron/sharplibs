using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic;
using NUnit.Framework;
using SharpLibs;

namespace SharpLibsTests
{
    [TestFixture]
    public class CryptoTests
    {
        [Test]
        public void TextToBase64()
        {
            var text = "49276d206b696c6c696e6720796f757220627261696e206c696b65206120706f69736f6e6f7573206d757368726f6f6d";
            var base64 = Conversions.BytesToBase64(Conversions.HexStringToBytes(text));
            Assert.AreEqual("SSdtIGtpbGxpbmcgeW91ciBicmFpbiBsaWtlIGEgcG9pc29ub3VzIG11c2hyb29t", base64);
        }

        [Test]
        public void FixedXor()
        {
            var t1 = Conversions.HexStringToBytes("1c0111001f010100061a024b53535009181c");
            var t2 = Conversions.HexStringToBytes("686974207468652062756c6c277320657965");
            var xor = Crypto.RepeatingKeyXor(t1, t2);
            var xorText = Conversions.BytesToHexString(xor);
            StringAssert.AreEqualIgnoringCase("746865206b696420646f6e277420706c6179", xorText);
        }

        [Test]
        public void FindSingleByteKey()
        {
            var cipherText = "1b37373331363f78151b7f2b783431333d78397828372d363c78373e783a393b3736";
            var bytes = Conversions.HexStringToBytes(cipherText);

            var englishLetterHist = Crypto.EnglishAlphabetHistogram();
            var maxCorrelation = 0;
            byte bestKey = 0;
            for (var i = 0; i < 256; i++)
            {
                var plainBytes = Crypto.RepeatingKeyXor(new[] {(byte)i}, bytes);
                var hist = Crypto.ByteHistogram(plainBytes);
                var corr = Crypto.HistogramCorrelation(hist, englishLetterHist);
                if (corr > maxCorrelation)
                {
                    maxCorrelation = corr;
                    bestKey = (byte)i;
                }
            }

            Debug.WriteLine($"Key found to be {bestKey}");
            Debug.WriteLine($"Message = {new string(Crypto.RepeatingKeyXor(new [] {bestKey}, bytes).Select(Convert.ToChar).ToArray())}");
        }

        [Test]
        public void RepeatingKeyEncrypt()
        {
            var bytes = Conversions.TextToBytes("Burning 'em, if you ain't quick and nimble");
            var key = Conversions.TextToBytes("ICE");
            var cipherBytes = Crypto.RepeatingKeyXor(key, bytes);
            var encrypted = Conversions.BytesToHexString(cipherBytes);
            StringAssert.AreEqualIgnoringCase("0b3637272a2b2e63622c2e69692a23693a2a3c6324202d623d63343c2a26226324272765272a282b2f20", encrypted);
        }

        [Test]
        public void HammingDistanceCheck()
        {
            var test = Conversions.TextToBytes("this is a test");
            var wokka = Conversions.TextToBytes("wokka wokka!!!");
            var dist = Crypto.HammingDistance(test, wokka);
            Assert.AreEqual(37, dist);
        }

        [Test]
        public void BreakRepeatingKeyXor()
        {
            var keysize = 0;
            var input = 
        }

    }
}
