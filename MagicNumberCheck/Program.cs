using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class FileAnalyzer {
    private static readonly string ConfigFilePath = "./extensions.json";
    private static void Main(string[] args) {
        string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string fullPath = Path.Combine(currentDirectory, ConfigFilePath);
        string texto1 = @"                  ,/////////.                   
            //  /             @@@//             
        ,/ /      @@@@@@@@@ /   &@/%@/          
      / /                         @///&@/       
    / / //@@@@%/ *   /////        @///@//@/     
   / /   @@@@@@   @@@@@@@@@#     .@/#@/@/#@/    
  / ,    @@@@                /  / @#@@/@@@&@/   
 / /                          *  @(@@@/@@@@/@/  
 ./                        // @@/@@@@@@@@@@@@/  
. /             ./,  ,@@@@@@//@@@@@@@@@@@@@@/#  
 ,/           / @@/%@@@@@@@@@@@@@@@@@@@@@@@&@/  
 / /    *   / @/@@@@@@@@@@@@@@@@@@@@@@@@@@@/@/  
  /        / @(@@/@@,@@@@@,@@@@@@@   %&@@@/@/   
   / ./ /  ,@/@@@@(/       /(@@@      @@@/@/    
    / */ /////@@@@@@@@#/#@@@@@@/,     @(@%*     
      /  /// @@@@@@@@@@@@@@@@@@@@@@@@/@#/       
         /  / .@@@#@       /#@@@@@/@@/          
            */   @@@@@@@@@@@@#@@#/              
                    ///////                     
";
        string texto2 = @"  ██▓███   ▒█████   ██▀███   ▄▄▄       ▒█████  
 ▓██░  ██▒▒██▒  ██▒▓██ ▒ ██▒▒████▄    ▒██▒  ██▒
 ▓██░ ██▓▒▒██░  ██▒▓██ ░▄█ ▒▒██  ▀█▄  ▒██░  ██▒
 ▒██▄█▓▒ ▒▒██   ██░▒██▀▀█▄  ░██▄▄▄▄██ ▒██   ██░
 ▒██▒ ░  ░░ ████▓▒░░██▓ ▒██▒ ▓█   ▓██▒░ ████▓▒░
 ▒▓▒░ ░  ░░ ▒░▒░▒░ ░ ▒▓ ░▒▓░ ▒▒   ▓▒█░░ ▒░▒░▒░ 
 ░▒ ░       ░ ▒ ▒░   ░▒ ░ ▒░  ▒   ▒▒ ░  ░ ▒ ▒░ 
 ░░       ░ ░ ░ ▒    ░░   ░   ░   ▒   ░ ░ ░ ▒  
              ░ ░     ░           ░  ░    ░ ░  ";
        string linha = "=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=";
        Console.WriteLine(texto1);
        Console.WriteLine(texto2);
        Console.WriteLine(linha);
        if (!File.Exists(fullPath)) {
            Console.WriteLine(linha);
            return;
        }
        if (args.Length > 0){
            string filePath = args[0];
            if(File.Exists(filePath)){
                string fileExtension = Path.GetExtension(filePath).TrimStart('.');
                string fileMimeType = GetFileMimeType(filePath, fileExtension);
                Console.WriteLine($"[*] O arquivo '{Path.GetFileName(filePath)}' é do tipo '{fileMimeType}'.");
                Console.WriteLine(linha);
            }else{
                Console.WriteLine("[*] O arquivo não existe.");
                Console.WriteLine(linha);
            }
        }else{
            Console.WriteLine("[*] Arraste um arquivo para cima do executável.");
            Console.WriteLine(linha);
        }
        Console.ReadLine();
    }
    private static string GetFileMimeType(string filePath, string fileExtension) {
        var config = LoadConfig();
        if(config.TryGetValue(fileExtension, out var fileInfo)){
            byte[] fileBytes = ReadFileBytes(filePath);
            string fileSignature = BitConverter.ToString(fileBytes).Replace("-", "");
            var signatures = fileInfo["signs"].ToObject<string[]>();
            foreach(var signature in signatures){
                string[] parts = signature.Split(',');
                int offset = int.Parse(parts[0]);
                string expectedSignature = parts[1];
                if(fileSignature.Length >= (offset + expectedSignature.Length) &&
                    fileSignature.Substring(offset, expectedSignature.Length) == expectedSignature) {
                    return fileInfo["mime"].ToString();
                }
            }
        }
        return "Tipo de arquivo desconhecido";
    }
    private static byte[] ReadFileBytes(string filePath) {
        using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read)) {
            byte[] bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, bytes.Length);
            return bytes;
        }
    }
    private static JObject LoadConfig() {
        string json = File.ReadAllText(ConfigFilePath);
        return JsonConvert.DeserializeObject<JObject>(json);
    }
}
