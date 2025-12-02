using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace webp2png;

class Program
{
    static void Main(string[] args)
    {
        string path = "";
        if (args.Length < 1)
        {
            Console.WriteLine("请输入路径：");
            path = Console.ReadLine();
        }
        else
        {
            path = args[0];
        }
        DirectoryInfo directory = new DirectoryInfo(path);
        
        // 递归搜索所有WebP文件
        FileInfo[] webpFiles = directory.GetFiles("*.webp", SearchOption.AllDirectories);
        
        foreach (FileInfo file in webpFiles)
        {
            Console.WriteLine(file.FullName);
            string originalName = file.FullName;
            string targetName = originalName.Replace(".webp", ".png");
            Convert2Png(originalName, targetName);
            // 这里添加你的处理代码
        }
        

    }

    static void Convert2Png(string webpPath, string outputPath)
    {
        
        using (Image image = Image.Load(webpPath))
        {
            // 配置 PNG 编码器（可选）
            PngEncoder encoder = new PngEncoder()
            {
                CompressionLevel = PngCompressionLevel.BestCompression, // 选择压缩级别
                ColorType = PngColorType.RgbWithAlpha // 设置颜色类型
            };
    
            // 保存为 PNG 格式
            image.Save(outputPath, encoder);
        }
    }
}