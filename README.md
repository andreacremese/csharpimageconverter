# c# Base64 extractor for html emails

Ck text editor embeds images as base 64 among text in html. 
If sent by email some clients (at the time of writing, Office on Win) are not able to handle inline base64 images.
This quick class extracts the image from the html text, replaces it with a cid reference and attaches the image (converted as a base64) to the email.

Unit tests included.