using MarkdownSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace MarkdownSharpTests
{
    [TestClass]
    public class ConfigTest
    {
        /*[TestMethod]
        public void TestLoadFromConfiguration()
        {
            /// TODO Update the test loader to inject options
            var settings = new Dictionary<string, string>
            {
                { "Markdown.AutoHyperlink", "true" },
                { "Markdown.AutoNewlines", "true" },
                { "Markdown.EmptyElementSuffix", ">" },
                { "Markdown.LinkEmails", "false" },
                { "Markdown.StrictBoldItalic", "true" }
            }; // ConfigurationManager.AppSettings;

            var markdown = new Markdown(true);
            Assert.AreEqual(true, markdown.AutoHyperlink);
            Assert.AreEqual(true, markdown.AutoNewLines);
            Assert.AreEqual(">", markdown.EmptyElementSuffix);
            Assert.AreEqual(false, markdown.LinkEmails);
            Assert.AreEqual(true, markdown.StrictBoldItalic);
        }*/

        [TestMethod]
        public void TestNoLoadFromConfigFile()
        {
            foreach (var markdown in new[] { new Markdown(), new Markdown(false) })
            {
                Assert.AreEqual(false, markdown.AutoHyperlink);
                Assert.AreEqual(false, markdown.AutoNewLines);
                Assert.AreEqual(" />", markdown.EmptyElementSuffix);
                Assert.AreEqual(true, markdown.LinkEmails);
                Assert.AreEqual(false, markdown.StrictBoldItalic);
            }
        }

        [TestMethod]
        public void TestAutoHyperlink()
        {
            var markdown = new Markdown();
            Assert.IsFalse(markdown.AutoHyperlink);
            Assert.AreEqual("<p>foo http://example.com bar</p>\n", markdown.Transform("foo http://example.com bar"));
            markdown.AutoHyperlink = true;
            Assert.AreEqual("<p>foo <a href=\"http://example.com\">http://example.com</a> bar</p>\n", markdown.Transform("foo http://example.com bar"));
        }

        [TestMethod]
        public void TestAutoNewLines()
        {
            var markdown = new Markdown();
            Assert.IsFalse(markdown.AutoNewLines);
            Assert.AreEqual("<p>Line1\nLine2</p>\n", markdown.Transform("Line1\nLine2"));
            markdown.AutoNewLines = true;
            Assert.AreEqual("<p>Line1<br />\nLine2</p>\n", markdown.Transform("Line1\nLine2"));
        }

        [TestMethod]
        public void TestEmptyElementSuffix()
        {
            var markdown = new Markdown();
            Assert.AreEqual(" />", markdown.EmptyElementSuffix);
            Assert.AreEqual("<hr />\n", markdown.Transform("* * *"));
            markdown.EmptyElementSuffix = ">";
            Assert.AreEqual("<hr>\n", markdown.Transform("* * *"));
        }

        [TestMethod]
        public void TestLinkEmails()
        {
            var markdown = new Markdown();
            Assert.IsTrue(markdown.LinkEmails);
            Assert.AreEqual("<p><a href=\"&#", markdown.Transform("<aa@bb.com>").Substring(0, 14));
            markdown.LinkEmails = false;
            Assert.AreEqual("<p><aa@bb.com></p>\n", markdown.Transform("<aa@bb.com>"));
        }

        [TestMethod]
        public void TestStrictBoldItalic()
        {
            var markdown = new Markdown();
            Assert.IsFalse(markdown.StrictBoldItalic);
            Assert.AreEqual("<p>before<strong>bold</strong>after before<em>italic</em>after</p>\n", markdown.Transform("before**bold**after before_italic_after"));
            markdown.StrictBoldItalic = true;
            Assert.AreEqual("<p>before*bold*after before_italic_after</p>\n", markdown.Transform("before*bold*after before_italic_after"));
        }
    }
}
