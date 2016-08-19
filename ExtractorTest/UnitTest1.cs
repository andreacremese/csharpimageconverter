using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Extractor;

namespace UnitTestProject2 {
    [TestClass]
    public class StringReplacerTest {
        [TestMethod]
        public void DoesNotChangeInputIfNoImgStrings() {
            // arrange
            var testString = @"<html>
                  <head>
                  </head>
                  <body>
                    <img width=100 height=100 id=""1"" src=""cid:Party.jpg"">
                  </body>
                </html>";
            // act
            var replacer = new StringReplacer(testString);
            // assert
            Assert.AreEqual(0, replacer.attachments.Count);
            Assert.AreEqual(testString, replacer.body);
        }

        [TestMethod]
        public void WorksOnOneImage() {
            // arrange
            var testString = @"<html>
                  <head>
                  </head>
                  <body>
                   <img src=""data:image/png;base64,/9j/iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAAHElEQVQI12P4//8/w38GIAXDIBKE0DHxgljNBAAO9TXL0Y4OHwAAAABJRU5ErkJggg=="" style =\""height:310px; width:207px\"" />
                    </body>
                </html>";
            // act
            var replacer = new StringReplacer(testString);
            // assert
            Assert.AreEqual(1, replacer.attachments.Count, "wrong number of attachments found");
            Assert.AreNotEqual(-1, replacer.body.IndexOf(String.Format("src=\"cid:{0}\"", replacer.attachments[0].Item1)), "Img tag has not bee replaced");
            Assert.AreEqual(-1, replacer.body.IndexOf("/9j"), "base64 image still present");
            Assert.AreEqual(-1, replacer.body.IndexOf("2Q==\""), "base64 image still present");
            Assert.AreEqual(@"/9j/iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAAHElEQVQI12P4//8/w38GIAXDIBKE0DHxgljNBAAO9TXL0Y4OHwAAAABJRU5ErkJggg==", Convert.ToBase64String(replacer.attachments[0].Item2), "image not extracted or converted");
        }

        [TestMethod]
        public void WorksOnTwoImage() {
            // arrange
            var testString = @"<html>
                  <head>
                  </head>
                  <body>
                   <img src=""data:image/png;base64,/9j/iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAAHElEQVQI12P4//8/w38GIAXDIBKE0DHxgljNBAAO9TXL0Y4OHwAAAABJRU5ErkJggg=="" style =\""height:310px; width:207px\"" /> 
                   <img src=""data:image/png;base64,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"" />
                   </body>
                </html>";
            // act
            var replacer = new StringReplacer(testString);
            // assert
            Assert.AreEqual(2, replacer.attachments.Count, "wrong number of attachments found");
            Assert.AreNotEqual(-1, replacer.body.IndexOf(String.Format("src=\"cid:{0}\"", replacer.attachments[0].Item1)), "Img tag has not bee replaced");
            Assert.AreNotEqual(-1, replacer.body.IndexOf(String.Format("src=\"cid:{0}\"", replacer.attachments[1].Item1)), "Img tag has not bee replaced");
            Assert.AreEqual(@"/9j/iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAAHElEQVQI12P4//8/w38GIAXDIBKE0DHxgljNBAAO9TXL0Y4OHwAAAABJRU5ErkJggg==", Convert.ToBase64String(replacer.attachments[0].Item2), "image not extracted or converted");
            //Assert.AreEqual(@"/9j/iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAAHElEQVQI12P4//8/w38GIAXDIBKE0DHxgljNBAAO9TXL0Y4OHwAAAABJRU5ErkJggg==", Convert.ToBase64String(replacer.attachments[1].Item2), "image not extracted or converted");
        }
    }
}
