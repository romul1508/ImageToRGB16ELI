Copyright (c) 2021 Roman Ermakov
All rights reserved.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFR INGEMENT. IN NO EVENT SHALL 
THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR 
OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;

namespace ImageToRGB16ELI
{
    /// <summary>
    /// The program allows you to convert raster images to ELI format
    /// author: Ermakov Roman
    /// e-mail: romul1508@gmail.com
    /// sinc. 05.11.2021
    /// Copyright 2021 Roman Ermakov
    /// </summary>
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // Original file
            string fromFileName = @"input.jpg";

            // Result file
            string toFileName = @"output.jpg";
            //-------------------------------
            // Signature
            string sign = "ELI";        // will need to be converted to UTf-8 (4 bytes)

            // Required sizes

            // Header length in bytes
            int head_len = 32;          // (4 bytes)

            // Offset to the image from the beginning of the file in bytes
            int data_offset = 512;      // (4 bytes)

            int reserved = 0;           // Reserved (4 bytes)

            int sizeX = 512;            // image width in pixels (4 bytes)
            int sizeY = 512;            // image height in pixels (4 bytes)

            int bit_count = 16;         // Bits per pixel
            int line_length = 1024;     // Length of one image line in bytes

            byte[] reserv_null_arraj = new byte[476];
            //---------------------------------
            // Load the original image
            Image image = Image.FromFile(fromFileName);

            // Assuming the original image is in image, let's draw it.
            // Create a bitmap of the desired size.
            Bitmap bmp_result = new Bitmap(sizeX, sizeY, PixelFormat.Format16bppRgb565);
            bmp_result.MakeTransparent();

            using (Graphics g = Graphics.FromImage(bmp_result))
            {
                g.DrawImage(image, 0, 0, sizeX, sizeY);
                g.Flush();
            }

            // We save the result in ELI format
            File.Delete(toFileName);
            //----------------------
            FileStream stream = new FileStream(toFileName, FileMode.Create, FileAccess.Write);

            // Convert a unicode sign string to UTF-8 format        
            UnicodeEncoding streamEncoding = new UnicodeEncoding();

            byte[] signStrBuffer = streamEncoding.GetBytes(sign);

            Encoding utf8 = Encoding.GetEncoding("UTF-8");

            byte[] utf8SignStrBytes = Encoding.Convert(
                        Encoding.GetEncoding("UTF-16"), utf8, signStrBuffer);

            byte empty = 0;

            // Writing binary data
            BinaryWriter binWrit = new BinaryWriter(stream);
            binWrit.Write(utf8SignStrBytes);
            binWrit.Write(empty);            
            binWrit.Write(head_len);
            binWrit.Write(data_offset);
            binWrit.Write(reserved);
            binWrit.Write(sizeX);
            binWrit.Write(sizeY);
            binWrit.Write(bit_count);
            binWrit.Write(line_length);
            binWrit.Write(reserved);
            binWrit.Write(reserv_null_arraj);

            ushort red_mask = 0xF800;
            ushort green_mask = 0x7E0;
            ushort blue_mask = 0x1F;

            // we write the RGB data of the image pixels            
            for (int i = 0; i < bmp_result.Height; ++i)
            {
                for (int j = 0; j < bmp_result.Width; ++j)
                {
                    Color c = bmp_result.GetPixel(j, i);
                    
                    int val32 = c.ToArgb();
                    // Console.WriteLine("val32 = {0}", val32);
                    
                    byte r = c.R;
                    // Console.WriteLine("r = {0}, byte r = c.R", r);
                    
                    byte g = c.G;
                    // Console.WriteLine("g = {0}, byte g = c.G", g);                    
                    
                    byte b = c.B;
                    // Console.WriteLine("b = {0}, byte b = c.B", b);
        
                    ushort pixel565 = (ushort)((r << 11) | (g << 5) | b);
                    Console.WriteLine("pixel565 = {0}", pixel565);

                    binWrit.Write(pixel565);
                }
            }
            //---------------------------
            // We close everything
            binWrit.Close();
            stream.Close();
            image.Dispose();
        }
    }
}
