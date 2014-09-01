﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace FontInfo
{
    public class FontInfo
    {
        public enum fontType { TTF, OTF, FON, TTC, PFB, PFA, ERR};
        private fontType type = fontType.ERR;
        public fontType Type
        {
            get { return type; }
        }

        private BinaryReader reader;

        public FontInfo(string file)
        {
            if (!File.Exists(file)) throw new FileNotFoundException();
            this.reader = new BinaryReader(new FileStream(file, FileMode.Open, FileAccess.Read));
            byte[] magic_num = reader.ReadBytes(4);
            Console.WriteLine("A:" + magic_num[0] + ", " + magic_num[1] + ", " + magic_num[2] + ", " + magic_num[3]);
            Console.WriteLine("B:" + magic_num.Intersect(new byte[] { 0x00, 0x01, 0x00, 0x00 }).Count());

            if (magic_num.SequenceEqual(new byte[] {0x00, 0x01, 0x00, 0x00}))
            {
                type = fontType.TTF;
            }
            else if (magic_num.SequenceEqual(new byte[] {0x4F, 0x54, 0x54, 0x4F}))
            {
                type = fontType.OTF;
            }
            else if (magic_num.SequenceEqual(new byte[] { 0x74, 0x74, 0x63, 0x66 }))
            {
                type = fontType.TTC;
            }
        }

        public void readInfo()
        {
            switch (this.type)
            {
                case fontType.OTF:
                    parseOTF();
                    break;
                default:
                    break;
            }
        }

        //From: http://www.microsoft.com/typography/otspec/otff.htm
        private void parseOTF()
        {
            //read the OffSet Table (everything after sfnt version)
            UInt16 numTables = reverse(reader.ReadUInt16());
            Console.WriteLine("numTables: " + numTables);
            UInt16 searchRange = reverse(reader.ReadUInt16());
            UInt16 entrySelector = reverse(reader.ReadUInt16());
            UInt16 rangeShift = reverse(reader.ReadUInt16());
            List<FTable> tables = new List<FTable>();
            for (int t = 0; t < numTables; t++)
            {
                tables.Add(FTable.parseTable(reader));
                if (new string(tables.Last().tag) == "name")
                {
                    new FTable_Name(reader, tables.Last());
                }
            }

        }

        public static UInt32 reverse(UInt32 value)
        {
            return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
                   (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
        }

        public static UInt16 reverse(UInt16 value)
        {
            return (UInt16)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);
        }
    }
#region OTF/TTF

    /* Name table */
    class FTable_Name
    {
        public FTable entry;
        public UInt16 format;
        public UInt16 count;
        public UInt16 stringOffset;
        public UInt16 langTagCount;
        public List<NameRecord> nameRecords;
        public List<LangTagRecord> langTagRecords;

        public FTable_Name(BinaryReader r, FTable entry)
        {
            nameRecords = new List<NameRecord>();
            this.entry = entry;
            //store current position.
            long start = r.BaseStream.Position;
            //seek the table.
            r.BaseStream.Seek(entry.offset, SeekOrigin.Begin);
            format = FontInfo.reverse(r.ReadUInt16());
            count = FontInfo.reverse(r.ReadUInt16());
            stringOffset = FontInfo.reverse(r.ReadUInt16());
            //read name records...
            for (int i = 0; i < count; i++)
            {
                nameRecords.Add(new NameRecord(r, entry.offset + stringOffset));
                Console.WriteLine(nameRecords.Last().str);
            }
            //Format v1? Then we have lang tags too.
            if (format == 1)
            {
                langTagRecords = new List<LangTagRecord>();
                langTagCount = FontInfo.reverse(r.ReadUInt16());
                for (int i = 0; i < langTagCount; i++)
                {
                    langTagRecords.Add(new LangTagRecord(r, entry.offset + stringOffset));
                    Console.WriteLine(langTagRecords.Last().str);
                }
            }
            r.BaseStream.Seek(start, SeekOrigin.Begin);
        }

        //Name records...
        public class NameRecord
        {
            public UInt16 platformID;
            public UInt16 encodingID;
            public UInt16 languageID;
            public UInt16 nameID;
            public UInt16 length;
            public UInt16 offset;
            public string str;

            //accept a BinaryReader and offset for string data.
            public NameRecord(BinaryReader r, long soffset)
            {
                platformID = FontInfo.reverse(r.ReadUInt16());
                encodingID = FontInfo.reverse(r.ReadUInt16());
                languageID = FontInfo.reverse(r.ReadUInt16());
                nameID = FontInfo.reverse(r.ReadUInt16());
                length = FontInfo.reverse(r.ReadUInt16());
                offset = FontInfo.reverse(r.ReadUInt16());
                long ipos = r.BaseStream.Position;
                r.BaseStream.Seek(soffset + offset, SeekOrigin.Begin);
                str = new string(r.ReadChars(length));
                //seek back to start of next name record.
                r.BaseStream.Seek(ipos, SeekOrigin.Begin);
            }
        }

        public class LangTagRecord
        {
            public UInt16 length;
            public UInt16 offset;

            public string str;

            //accept a BinaryReader and offset for string data.
            public LangTagRecord(BinaryReader r, long soffset)
            {
                length = FontInfo.reverse(r.ReadUInt16());
                offset = FontInfo.reverse(r.ReadUInt16());
                long ipos = r.BaseStream.Position;
                r.BaseStream.Seek(soffset + offset, SeekOrigin.Begin);
                str = new string(r.ReadChars(length));
                r.BaseStream.Seek(ipos, SeekOrigin.Begin);
            }
        }
    }

    /* Table entries */
    class FTable
    {
        public char[] tag;
        public UInt32 checkSum;
        public UInt32 offset;
        public UInt32 length;
        public byte[] payload;

        private FTable()
        { }

        public static FTable parseTable(BinaryReader reader)
        {
            FTable res = new FTable();
            res.tag = reader.ReadChars(4);
            Console.WriteLine(new string(res.tag));

            res.checkSum = FontInfo.reverse(reader.ReadUInt32());
            res.offset = FontInfo.reverse(reader.ReadUInt32());
            res.length = FontInfo.reverse(reader.ReadUInt32());
            Console.WriteLine("Length: " + res.length);
            //res.payload = reader.ReadBytes((int)res.length - 16);
            return res;
        }
        
        public void readPayload(BinaryReader reader)
        {
            long ipos = reader.BaseStream.Position;
            reader.BaseStream.Seek(this.offset, SeekOrigin.Begin);
            this.payload = reader.ReadBytes((int)this.length);
            reader.BaseStream.Seek(ipos, SeekOrigin.Begin);
        }
    }
#endregion
}