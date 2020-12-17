using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace Hasebni.SharedKernal.ExtensionMethod
{
    public static class ExtensionMethods
    {
        public static double RoundClose(this double value, double close)
        {
            return Math.Round(value / close, MidpointRounding.AwayFromZero) * close;
        }

        public static double ToDouble(this string s)
        {
            return Double.TryParse(s, out double d) ? d : 0;
        }


        internal static readonly char[] chars =
           "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

        public static string GetUniqueKey(int size)
        {
            byte[] data = new byte[4 * size];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(data);
            }
            StringBuilder result = new StringBuilder(size);
            for (int i = 0; i < size; i++)
            {
                var rnd = BitConverter.ToUInt32(data, i * 4);
                var idx = rnd % chars.Length;

                result.Append(chars[idx]);
            }

            return result.ToString();
        }

        public static string GetSixNumberToken()
        {
            Random random = new Random();

            var token = "";
            int c = 0;
            while(c < 6)
            {
                int x = random.Next(0,9);
                if (x != 1)
                {
                    token += x.ToString();
                    c++;
                }
            }
            return token;
        }

        public static DateTime FixFormatDate(this string dateTime)
        {
            string[] datetimearray = dateTime.Split(".");
            DateTime newdate = DateTime.ParseExact(datetimearray[0],
                         "yyyy-MM-dd HH:mm:ss", CultureInfo.GetCultureInfo("en-Us"));
            return newdate;
        }

        public static int RandomValue(int a,int b)
        {
            Random random = new Random();
            return random.Next(a, b + 1);
        }
    }
}
