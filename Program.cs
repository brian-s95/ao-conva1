using System.IO;
using aogrhx1;
using System;
using System.Collections.Generic;
using StbImageSharp;
using StbImageWriteSharp;

//Ruta graficos
var graficosPath = Path.Combine(Environment.CurrentDirectory, "graficos");

//Ruta Init
var initPath = Path.Combine(Environment.CurrentDirectory, "init");

//Ruta Salida
var ouputPath = Path.Combine(Environment.CurrentDirectory, "out");

//Lista
var GrhFiles = new string[] { "graficos.ind", "graficos1.ind", "graficos2.ind", "graficos3.ind" };

//
var count = 0;

//Creo la carpeta donde se guardan las imagenes
if (!Directory.Exists(ouputPath))
    Directory.CreateDirectory(ouputPath);

IGrhLoader loader;
bool guardarComoPng = false;
GrhData[] grhList = default;

Console.WriteLine("¿Esta usando la version 0.11.x o fenix?  Escriba Y/N");
var answer = Console.ReadLine();

if (answer.ToLower() == "y")
    loader = new GrhLoaderOld();
else
    loader = new GrhLoaderNew();

Console.WriteLine("¿Desea guardar las imagenes con el formato png(Argentum online utiliza por defecto bmp si no esta seguro ponga N)? Escriba Y/N");
answer = Console.ReadLine();

if (answer.ToLower() == "y")
    guardarComoPng = true;

//Pequeño parche para que busque el archivo correcto.
foreach (var fileName in GrhFiles)
{
    var path = Path.Combine(initPath, fileName);
    if (File.Exists(path))
    {
        grhList = loader.Load(path);
        break;
    }
}

var listaDeImagenes = new Dictionary<int, Size>();

foreach (var grh in grhList)
{
    if (grh.FileId == 0)
        continue;

    if (listaDeImagenes.TryGetValue(grh.FileId, out var imageSize))
    {
        imageSize.Width = Math.Max(imageSize.Width, grh.OffX + grh.Width);
        imageSize.Height = Math.Max(imageSize.Height, grh.OffY + grh.Height);

        listaDeImagenes[grh.FileId] = imageSize;
    }
    else
    {
        listaDeImagenes[grh.FileId] = new Size
        {
            Width = grh.OffX + grh.Width,
            Height = grh.OffY + grh.Height
        };
    }
}

Console.WriteLine($"Se encontraron en total {listaDeImagenes.Count} imagenes");
Console.WriteLine("...............................................................");
Console.WriteLine("...............................................................");

foreach (var image in listaDeImagenes)
{
    string filePath = string.Empty;

    if (File.Exists(graficosPath + $"/{image.Key}.bmp"))
    {
        filePath = graficosPath + $"/{image.Key}.bmp";
    }
    else if (File.Exists(graficosPath + $"/{image.Key}.png"))
    {
        filePath = graficosPath + $"/{image.Key}.png";
    }
    else
    {
        Console.WriteLine($"No se logro encontrar el archivo con el id {image.Key}");
        continue;
    }

    var buffer = File.ReadAllBytes(filePath);
    if (buffer.Length == 0)
        continue;

    var imageInfo = ImageResult.FromMemory(buffer, StbImageSharp.ColorComponents.RedGreenBlueAlpha);

    if (image.Value.Width > imageInfo.Width || image.Value.Height > imageInfo.Height)
    {
        Console.WriteLine($"Error en {image.Key}");
    }
    else
    {
        var pixels = imageInfo.Data;
        var newImagePixels = new byte[image.Value.Width * image.Value.Height * 4];

        for (int y = 0; y < image.Value.Height; y++)
        {
            for (int x = 0; x < image.Value.Width; x++)
            {
                var newPixelLocation = (x + y * image.Value.Width) * 4;
                var pixelLocation = (x + y * imageInfo.Width) * 4;

                newImagePixels[newPixelLocation] = pixels[pixelLocation];
                newImagePixels[newPixelLocation + 1] = pixels[pixelLocation + 1];
                newImagePixels[newPixelLocation + 2] = pixels[pixelLocation + 2];
                newImagePixels[newPixelLocation + 3] = pixels[pixelLocation + 3];
            }
        }

        if (guardarComoPng)
            filePath = Path.Combine(ouputPath, $"{image.Key}.png");
        else
            filePath = Path.Combine(ouputPath, $"{image.Key}.bmp");

        using var stream = File.OpenWrite(filePath);
        var writer = new ImageWriter();

        if (guardarComoPng)
            writer.WritePng(newImagePixels, image.Value.Width, image.Value.Height, StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha, stream);
        else
            writer.WriteBmp(newImagePixels, image.Value.Width, image.Value.Height, StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha, stream);
        
        count++;
    }
}

Console.WriteLine($"Se lograron convertir {count} imagenes!.");
Console.WriteLine("Presione cualquier tecla para salir.");
Console.ReadLine();