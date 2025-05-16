using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;



namespace RıscV_Generator
{
    //RÜYA
    class CodePreProcessor
    {
        public List<string> CleanCode(string rawCode)
        {
            var cleanedLines = new List<string>();
            var lines = rawCode.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                trimmedLine = Regex.Replace(trimmedLine, @"\s+", " ");

                // Süslü parantezleri çevresindeki koddan ayır
                trimmedLine = trimmedLine.Replace("{", "\n{\n").Replace("}", "\n}\n");

                // Her satırı tekrar ayır çünkü yukarıda \n eklemiş olabiliriz
                var splitLines = trimmedLine.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var splitLine in splitLines)
                {
                    cleanedLines.Add(splitLine.Trim());
                }
            }

            return cleanedLines;
        }




        public List<string> ExtractVariables(string cleanedCode)
        {
            var variables = new List<string>();

            // Geliştirilmiş: Tüm int, float, vb. tanımlamalarını ve çoklu değişkenleri yakalar
            var variableRegex = new Regex(@"\b(int|float|char|double|long|short)\s+([^;]+);");

            var matches = variableRegex.Matches(cleanedCode);
            foreach (Match match in matches)
            {
                // match.Groups[2] => değişken kısmı: örn. "a, b, c"
                var variableSection = match.Groups[2].Value;

                // Tüm boşlukları ve olası fazlalıkları temizleyerek virgülden ayır
                var individualVariables = variableSection.Split(',');

                foreach (var variable in individualVariables)
                {
                    var cleanVar = variable.Trim();

                    // 'a = 5' gibi eşitlik varsa sadece isim kısmını al
                    if (cleanVar.Contains("="))
                        cleanVar = cleanVar.Split('=')[0].Trim();

                    if (!variables.Contains(cleanVar))
                        variables.Add(cleanVar);
                }
            }

            return variables;
        }

    }

    }