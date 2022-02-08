using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace aogrhx1
{
    public class GrhLoaderOld : IGrhLoader
    {
        /*

        Public Type tCabecera 'Cabecera de los con
            desc As String * 255
            CRC As Long
            MagicWord As Long
        End Type
        */
        private const int NumeroDeBMPs = 65000; //ni idea el valor de esto, me dio pajar descargar la 0.11.5...
 
        public GrhData[] Load(string path)
        {
            var bytes = File.ReadAllBytes(path);

            using var ms = new MemoryStream(bytes);
            using var reader = new BinaryReader(ms);

            //header
            reader.ReadBytes(255 + 4 + 4);

            //
            reader.ReadBytes(10);

            var grhList = new GrhData[NumeroDeBMPs + 1];

            while (ms.Position < ms.Length)
            {
                var grhId = reader.ReadInt16();
                var grh = new GrhData();

                var framesCount = reader.ReadInt16();
                if (framesCount > 1)
                {
                    for (int i = 1; i <= framesCount; i++)
                    {
                        reader.ReadInt16();
                    }
                    reader.ReadInt16();
                }
                else
                {
                    grh.FileId = reader.ReadInt16();
                    grh.OffX = reader.ReadInt16();
                    grh.OffY = reader.ReadInt16();
                    grh.Width = reader.ReadInt16();
                    grh.Height = reader.ReadInt16();
                }

                grhList[grhId] = grh;
            }

            return grhList;
        }
    }
}