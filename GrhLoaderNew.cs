using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace aogrhx1
{
    public class GrhLoaderNew : IGrhLoader
    {
        public GrhData[] Load(string path)
        {
            var bytes = File.ReadAllBytes(path);

            using var ms = new MemoryStream(bytes);
            using var reader = new BinaryReader(ms);

            //version
            reader.ReadInt32();

            var grhCount = reader.ReadInt32();

            var GrhList = new GrhData[grhCount + 1];

            while(ms.Position < ms.Length)
            {
                var grhId = reader.ReadInt32();
                var grh = new GrhData();

                var framesCount = (int)reader.ReadInt16();
                if(framesCount > 1)
                {
                    for (int i = 1; i <= framesCount; i++)
                    {
                        reader.ReadInt32();
                    }
                    reader.ReadSingle();
                }
                else
                {
                    grh.FileId = reader.ReadInt32();
                    grh.OffX = reader.ReadInt16();
                    grh.OffY = reader.ReadInt16();
                    grh.Width = reader.ReadInt16();
                    grh.Height = reader.ReadInt16();
                }

                GrhList[grhId] = grh;
            }

            return GrhList;
        }
    }
}