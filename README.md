# ImageToRGB16ELI
ImageToRGB16ELI is a console program. The program accepts images in jpg format as input, formats and saves them in ELI format.
Microsoft .NET Framework 3.5 is used to perform the functionality. Microsoft Visual Studio 2010 was used to develop the program.  
The program functions in the line of Microsoft Windows XP / Wista / Windows7 operating systems.

Description of the ELI file format:

At the beginning of the file there is a header that describes the image parameters:

At the beginning of the file there is a header that describes the image parameters:

offset   Length          Name               Description
0           4            signature          Signature, "ELI\0"
4           4            header_length      Header length in bytes
8           4            data_offset        Offset to the image from the beginning of 							 the file in bytes
12(0x0C)    4            reserved           Reserved, must be 0
16(0x10)    4            image_width        Image width pixels
20(0x14)    4            image_height       Image height in pixels
24(0x18)    4            bit_count          Bits per pixel
28(0x1C)    4            line_length        Length of one image line in bytes
32(0x20)    480 + 512xN  Reserved

The offset to the start of the image data must be a multiple of 512 bytes.
