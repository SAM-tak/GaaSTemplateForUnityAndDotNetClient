using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using MessagePack;

namespace YourGameServer.Models // Unity cannot accpect 'namespace YourProjectName.Models;' yet
{
    [Serializable]
    [MessagePackObject]
    public struct LUID : IEquatable<LUID>
    {
        [Key(0)]
        public ulong id0;
        [Key(1)]
        public ushort id1;

        private static readonly char[] validCharacters = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789".ToCharArray(); // length = 32
        public const int StringPresentationLength = 16;
        // 0-31 = 5bit * 16 = 80bit  元は64bit幅のIDだから、80bit確保できれば十分だと思う

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isUnique"></param>
        /// <returns></returns>
        public static LUID NewLUID(Func<LUID, bool> isUnique)
        {
            LUID result = new();
            do {
                // RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue) returns weird value
                var tmp = RandomNumberGenerator.GetInt32(0x1FFFF + 1);
                result.id0 = (ulong)((long)(tmp & 0xF) << 60
                    | (long)RandomNumberGenerator.GetInt32(0x3FFFFFFF + 1) << 30
                    | (long)RandomNumberGenerator.GetInt32(0x3FFFFFFF + 1));
                result.id1 = (ushort)(tmp >> 4);
            } while(isUnique != null && !isUnique(result));
            return result;
        }

        public static async Task<LUID> NewLUIDAsync(Func<LUID, Task<bool>> isUnique)
        {
            LUID result = new();
            do {
                // RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue) returns weird value
                var tmp = RandomNumberGenerator.GetInt32(0x1FFFF + 1);
                result.id0 = (ulong)((long)(tmp & 0xF) << 60
                    | (long)RandomNumberGenerator.GetInt32(0x3FFFFFFF + 1) << 30
                    | (long)RandomNumberGenerator.GetInt32(0x3FFFFFFF + 1));
                result.id1 = (ushort)(tmp >> 4);
            } while(isUnique != null && !await isUnique(result));
            return result;
        }

        public static async Task<string> NewLUIDStringAsync(Func<string, Task<bool>> isUnique)
        {
            string result;
            do {
                // RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue) returns weird value
                var tmp = RandomNumberGenerator.GetInt32(0x1FFFF + 1);
                result = new LUID {
                    id0 = (ulong)((long)(tmp & 0xF) << 60
                    | (long)RandomNumberGenerator.GetInt32(0x3FFFFFFF + 1) << 30
                    | (long)RandomNumberGenerator.GetInt32(0x3FFFFFFF + 1)),
                    id1 = (ushort)(tmp >> 4)
                }.ToString();
            } while(isUnique != null && !await isUnique(result));
            return result;
        }

        public static LUID FromString(string source)
        {
            var ret = new LUID();
            int state = 0;
            foreach(var c in source) {
                if(validCharacters.Contains(c)) {
                    var i = Array.IndexOf(validCharacters, c);
                    if(state < 3) {
                        ret.id1 |= (ushort)(i << (5 * (2 - state) + 1));
                    }
                    else if(state == 3) {
                        ret.id1 |= (ushort)(i >> 4);
                        ret.id0 |= (ulong)i << 60;
                    }
                    else if(state < StringPresentationLength) {
                        ret.id0 |= (ulong)i << (5 * (15 - state));
                    }
                    ++state;
                    if(state == StringPresentationLength) break;
                }
                else if(!char.IsWhiteSpace(c) && c != '-') {
                    throw new FormatException($"Illegal character found. {c}");
                }
            }
            if(state < StringPresentationLength) {
                throw new ArgumentException($"source string is too short. {source}");
            }
            return ret;
        }

        public bool Equals(LUID other)
        {
            return id0 == other.id0 && id1 == other.id1;
        }

        public override bool Equals(object obj)
        {
            return obj is LUID luid && Equals(luid);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(id0, id1);
        }

        public override string ToString()
        {
            var mid = (id1 & 0x1) << 4 | (int)(id0 >> (5 * 12)) & 0x1F;
            return new string(new char[] {
                validCharacters[id1 >> (5 * 2 + 1)],
                validCharacters[(id1 >> (5 + 1)) & 0x1F],
                validCharacters[(id1 >> 1) & 0x1F],
                validCharacters[mid],

                validCharacters[(id0 >> (5 * 11)) & 0x1F],
                validCharacters[(id0 >> (5 * 10)) & 0x1F],
                validCharacters[(id0 >> (5 * 9)) & 0x1F],
                validCharacters[(id0 >> (5 * 8)) & 0x1F],

                validCharacters[(id0 >> (5 * 7)) & 0x1F],
                validCharacters[(id0 >> (5 * 6)) & 0x1F],
                validCharacters[(id0 >> (5 * 5)) & 0x1F],
                validCharacters[(id0 >> (5 * 4)) & 0x1F],

                validCharacters[(id0 >> (5 * 3)) & 0x1F],
                validCharacters[(id0 >> (5 * 2)) & 0x1F],
                validCharacters[(id0 >> 5) & 0x1F],
                validCharacters[id0 & 0x1F]
            });
        }

        public static bool operator ==(LUID left, LUID right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(LUID left, LUID right)
        {
            return !(left == right);
        }
    }
}