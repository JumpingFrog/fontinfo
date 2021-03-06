﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace FontInfo
{
    [TestFixture]
    class FontInfoTest
    {
        [SetUp]
        public void Init()
        {
            //
        }

        [Test]
        public void Test1()
        {
            FontInfo fi = new FontInfo("Not a file");
            Assert.NotNull(fi);
        }

        [Test]
        public void TestTTF()
        {
            Console.WriteLine("TIMES");
            FontInfo fi = new FontInfo(@"C:\Windows\Fonts\times.ttf");
            fi.readInfo();
            Console.WriteLine(fi.ToString());

            Console.WriteLine("AMIRI");
            FontInfo fi2 = new FontInfo(@"C:\Windows\Fonts\amiri-regular.ttf");
            fi2.readInfo();
            Console.WriteLine(fi2.ToString());

            Console.WriteLine("AMIRI BOLD");
            FontInfo fi3 = new FontInfo(@"C:\Windows\Fonts\amiri-bold.ttf");
            fi3.readInfo();
            Console.WriteLine(fi3.ToString());

            Console.WriteLine("Proxima Nova Cond Sbold");
            FontInfo fi4 = new FontInfo(@"C:\Windows\Fonts\Proxima Nova Cond Sbold.ttf");
            fi4.readInfo();
            Console.WriteLine(fi4.ToString());

            Console.WriteLine("advee___");
            FontInfo fi5 = new FontInfo(@"C:\3B2WIN\advee___.ttf");
            fi5.readInfo();
            Console.WriteLine(fi5.ToString());

            Console.WriteLine("Arimo-Bold.ttf");
            FontInfo fi6 = new FontInfo(@"C:\Windows\Fonts\Arimo-Bold.ttf");
            fi6.readInfo();
            Console.WriteLine(fi6.ToString());




            /*
            FontInfo fi2 = new FontInfo(@"C:\Windows\Fonts\MyriadPro-Bold.otf");
            FontInfo fi3 = new FontInfo(@"C:\Windows\Fonts\Sproketf.PFB");
            FontInfo fi4 = new FontInfo(@"C:\Windows\Fonts\CAMBRIA.TTC");
            Assert.AreEqual(FontInfo.fontType.TTF, fi.Type);
            Assert.AreEqual(FontInfo.fontType.OTF, fi2.Type);
            Assert.AreEqual(FontInfo.fontType.PFB, fi3.Type);
            Assert.AreEqual(FontInfo.fontType.TTC, fi4.Type);*/
        }

        [Test]
        public void TestOTF()
        {
            //FontInfo fi = new FontInfo(@"C:\Windows\Fonts\MyriadPro-Bold.otf");
            FontInfo fi2 = new FontInfo(@"C:\Windows\Fonts\PoplarStd.otf");
            //fi.readInfo();
            fi2.readInfo();
            Console.WriteLine(fi2.ToString());
            //Assert.AreEqual(FontInfo.fontType.OTF, fi.Type);
        }

        [Test]
        public void TestPFB()
        {
            FontInfo fi = new FontInfo(@"C:\Windows\Fonts\serifab.pfb");
            fi.readInfo();
            Console.WriteLine(fi.ToString());

            FontInfo fi2 = new FontInfo(@"C:\Windows\Fonts\SEAGULLH.pfb");
            fi2.readInfo();
            Console.WriteLine(fi2.ToString());

            FontInfo fi3 =  new FontInfo(@"C:\Windows\Fonts\BNBI___.PFB");
            fi3.readInfo();
            Console.WriteLine(fi3.ToString());

            FontInfo fi4 = new FontInfo(@"C:\Windows\Fonts\50ROSSL.PFB");
            fi4.readInfo();
            Console.WriteLine(fi4.ToString());

            Assert.AreEqual(FontInfo.fontType.PFA, fi.Type);
        }

        [Test]
        public void Test3B2()
        {
            FontInfo fi = new FontInfo(@"C:\3B2WIN\AVENIR.FNT");
            fi.readInfo();

            foreach (var f in fi.ThreeB2Names)
            {
                Console.WriteLine(f);
            }

            FontInfo fi2 = new FontInfo(@"C:\3B2WIN\xmaths.FNT");
            fi2.readInfo();

            foreach (var f in fi2.ThreeB2Names)
            {
                Console.WriteLine(f);
            }
        }
    }
}
