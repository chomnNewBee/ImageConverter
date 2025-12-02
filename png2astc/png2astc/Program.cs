using System.Diagnostics;
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
        FileInfo[] webpFiles = directory.GetFiles("*.png", SearchOption.AllDirectories);
        
        foreach (FileInfo file in webpFiles)
        {
            Console.WriteLine(file.FullName);
            string originalName = file.FullName;
            string targetName = originalName.Replace(".png", ".astc");
            Convert2ASTC(originalName, targetName);
            
        }
        
        // 递归搜索所有WebP文件
        FileInfo[] webpFilesjpg = directory.GetFiles("*.jpg", SearchOption.AllDirectories);
        
        foreach (FileInfo file in webpFilesjpg)
        {
            Console.WriteLine(file.FullName);
            string originalName = file.FullName;
            string targetName = originalName.Replace(".jpg", ".astc");
            Convert2ASTC(originalName, targetName);
            
        }
        

    }

    static void Convert2ASTC(string inputPath, string outputPath)
    {
        // 1. 指定astcenc工具的路径 (根据你的实际情况修改)
        // Windows示例: @"tools\astcenc.exe"
        // Linux/macOS示例: @"/usr/bin/astcenc"
        string astcencPath = @"astcenc-avx2.exe";

        // 2. 检查工具是否存在
        if (!File.Exists(astcencPath))
        {
            // 提供详细的错误信息和指引
            string errorMessage = $"未找到astcenc工具。\n" +
                                  $"请从官方仓库下载：https://github.com/ARM-software/astc-encoder\n" +
                                  $"下载后，请将可执行文件放在程序运行目录的 'tools' 文件夹中，\n" +
                                  $"或者修改代码中的 'astcencPath' 变量指向正确的位置。\n" +
                                  $"当前查找路径: {Path.GetFullPath(astcencPath)}";
            throw new FileNotFoundException(errorMessage);
        }

        // 3. 设置压缩参数[citation:1]
        // -cl : 压缩为LDR线性颜色格式
        // 6x6 : 块大小 (每个6x6像素块压缩为128位，约3.56 bits/px)[citation:1]
        // -medium : 质量预设 (在速度和质量间取得平衡)[citation:1]
        string arguments = $"-cl \"{inputPath}\" \"{outputPath}\" 6x6 -medium";

        // 4. 配置并启动进程
        var startInfo = new ProcessStartInfo
        {
            FileName = astcencPath,
            Arguments = arguments,
            UseShellExecute = false,          // 不启用操作系统外壳启动进程
            CreateNoWindow = true,            // 不创建新窗口
            RedirectStandardOutput = true,    // 重定向输出，便于捕获
            RedirectStandardError = true      // 重定向错误
        };

        // 5. 执行转换
        try
        {
            using (var process = Process.Start(startInfo))
            {
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
            
                process.WaitForExit(); // 等待转换完成

                if (process.ExitCode != 0)
                {
                    throw new Exception($"astcenc转换失败 (退出代码: {process.ExitCode}):\n{error}");
                }
            
                // 可选：打印成功信息
                Console.WriteLine($"成功转换: {Path.GetFileName(inputPath)}");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"执行astcenc时发生错误: {ex.Message}", ex);
        }
    }
}