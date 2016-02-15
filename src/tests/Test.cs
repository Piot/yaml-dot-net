﻿using NUnit.Framework;
using System;
using yaml;
using System.Xml;

namespace tests {

	public static class AssertEx {
		static string ObjectToString (object expected) {
			var x = new System.Xml.Serialization.XmlSerializer(expected.GetType());
			var writer = new System.IO.StringWriter();
			x.Serialize(writer, expected);
			writer.Close();
			return writer.ToString();
		}

		public static void AreEqualByXml (object expected, object actual) {
			var expectedString = ObjectToString(expected);
			var actualString = ObjectToString(actual);

			Assert.AreEqual(expectedString, actualString);
		}
	}

	[TestFixture]
	public class Test {

		public struct SomeStruct {
			public uint inDaStruct;
		}

		public class TestSubKlass {
			public int answer;

			public string anotherAnswer { get; set; }

			public Object anotherObject { get; set; }
			public SomeStruct someStruct = new SomeStruct();
			public float f;
		}

		public class TestKlass {
			public int john;
			public string other;

			public string props { get; set; }
			public bool isItTrue;

			public TestSubKlass subClass;
		}

		[Test]
		public void TestDeserialize () {
			var testData = "john:34  \nsubClass: \n  answer: 42 \n  anotherAnswer: '99'\nother: 'hejsan svejsan' \nprops: 'hello,world'";
			var o = YamlDeserializer.Deserialize<TestKlass>(testData);
			Assert.AreEqual(34, o.john);
			Assert.AreEqual("hejsan svejsan", o.other);
			Assert.AreEqual("hello,world", o.props);
			Assert.AreEqual(42, o.subClass.answer);
			Assert.AreEqual("99", o.subClass.anotherAnswer);
		}

		[Test]
		public void TestDeserializeString () {
			var testData = "anotherAnswer: \"example\"";
			var o = YamlDeserializer.Deserialize<TestSubKlass>(testData);
			Assert.AreEqual("example", o.anotherAnswer);
		}

		[Test]
		public void TestSerialize () {
			var o = new TestKlass();
			o.john = 34;
			o.subClass = new TestSubKlass();
			o.subClass.answer = 42;
			o.subClass.f = -22.42f;
			o.subClass.someStruct.inDaStruct = 1;
			o.props = "props";
			o.isItTrue = true;
			// o.subClass.anotherObject = new Object();

			o.other = "other";
			var output = YamlSerializer.Serialize(o);
			var back = YamlDeserializer.Deserialize<TestKlass>(output);
			var backOutput = YamlSerializer.Serialize(back);
			AssertEx.AreEqualByXml(o, back);
			Assert.AreEqual(output, backOutput);
		}

		public class SomeColor {
			public string color;
		};
		[Test]
		public void TestString() {
			var s = "color:    'kind of red'";
			var c = YamlDeserializer.Deserialize<SomeColor>(s);
			Assert.AreEqual("kind of red", c.color);
		}
	}
}

