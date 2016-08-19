using System;
using System.Collections.Generic;

namespace Extractor {

    public class StringReplacer {
        private const String starter = "src=\"data:image/png;base64,";

        private String input;
        public  String body;
        /// <summary>
        /// list of tuple: imageName, byte array
        /// </summary>
        public  List<Tuple<String,byte[]>> attachments = new List<Tuple<String,byte[]>>();
        // note that a tuple used rather than keyvalue pair for performance http://www.dotnetperls.com/tuple-keyvaluepair

        /// <summary>
        /// Replaces the base64 images in the body (if present) with cid references, 
        /// extracting out the images as byte arrays in a separate list.
        /// </summary>
        /// <param name="emailBody"></param>
        public StringReplacer(String emailBody) {
            input = emailBody;
            replace();
        }

        /// <summary>
        /// replaces the img tags base64 with references to cid
        /// </summary>
        /// <returns></returns>
        private void replace() {
            Int32 imageStartIndex = input.IndexOf(starter);
            if (imageStartIndex == -1) {
                // the image starter is not present, return
                body = input;
                return;
            }
            Int32 imageClosingIndex = -1;
            body = String.Empty;
            do {
                // add to the body the part of the input from the closing index of the previous image to the next starting index
                body += input.Substring(imageClosingIndex + 1, imageStartIndex - imageClosingIndex + 1);
                String imageName = String.Concat(imageStartIndex.ToString(), ".jpg");
                body += String.Format(@" src=""cid:{0}"" ", imageName);
                
                // find the next closign index for next iteration
                imageClosingIndex = input.IndexOf(@"""", imageStartIndex + starter.Length + 1);
                // the image goes form image start index to image closing index -1
                var b64 = input.Substring(imageStartIndex, imageClosingIndex - imageStartIndex);

                var cut = b64.IndexOf("/9j/");
                b64 = cut != -1 ? b64.Substring(cut) : b64.Substring(b64.IndexOf("base64,") + "base64,".Length);

                var file = Convert.FromBase64String(b64);
                attachments.Add(new Tuple<String,byte[]>(imageName, file));

                // find the next image tag
                imageStartIndex = input.IndexOf(starter,imageStartIndex +1);
                if (imageStartIndex == -1) {
                    // if no more image is found, copy the rest of the input and return
                    body += " " + input.Substring(imageClosingIndex + 3);
                    break;
                }
            } while (true);

            return;

        }
    }
}
