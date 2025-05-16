using RıscV_Generator;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
class RiscVCodeGenerator
{
    private RegisterAllocator registerAllocator = new RegisterAllocator();
    private int labelCount = 0; //Label sayacım
    private List<string> output = new List<string>();//Sonuçlarımı depolamak için 

    public List<string> CodeGenerator(CodePreProcessor codePreProcessor, string rawCode)
    {
        var cleanCode = codePreProcessor.CleanCode(rawCode);  //kodumu temizleyen fonksiyon çağırımı
        var variables = codePreProcessor.ExtractVariables(string.Join("\n", cleanCode));  //değişkenkerli algılayan fonksiyon çağırımı
       

        foreach (var variable in variables)
        {
            registerAllocator.Allocate(variable); // tüm değişkenlere register ata
        }

        for (int i = 0; i < cleanCode.Count; i++)
        {
            var line = cleanCode[i]; // temizlenmiş koddaki satırları tuttuğum değişken

            if (line.StartsWith("if"))
            {
                var condition = ExtractCondition(line);
                string label = $"label_{labelCount++}";

                output.Add($"{condition.branchInstr} {condition.leftReg}, {condition.rightReg}, {label}");
                output.Add($"j label_{labelCount}");
                output.Add($"{label}:");

                // if bloğu işleniyor
                i++;
                if (i < cleanCode.Count && cleanCode[i] == "{")
                {
                    i++; // { sonrası ilk satıra geç
                    while (i < cleanCode.Count && cleanCode[i] != "}")
                    {
                        ProcessAssignment(cleanCode[i]);
                        i++;
                    }
                }
                else if (i < cleanCode.Count && cleanCode[i].Contains("="))
                {
                    ProcessAssignment(cleanCode[i]);
                }

                output.Add("j END");
            }

            
            else if (line.StartsWith("else"))
            {
                string label = $"label_{labelCount++}";
                output.Add($"# else bloğu");
                output.Add($"{label}:");

                i++;
                if (i < cleanCode.Count && cleanCode[i] == "{")
                {
                    i++; // { sonrası ilk satıra geç
                    while (i < cleanCode.Count && cleanCode[i] != "}")
                    {
                        ProcessAssignment(cleanCode[i]);
                        i++;
                    }
                }
                else if (i < cleanCode.Count && cleanCode[i].Contains("="))
                {
                    ProcessAssignment(cleanCode[i]);
                }
            }


        }

        output.Add("END:");
        return output;
    }
    //Ayşe

    private void ProcessAssignment(string line)
    {
        var parts = line.Split('=');
        if (parts.Length != 2)
            throw new Exception("Geçersiz atama ifadesi: " + line);

        var left = parts[0].Trim();
        var right = parts[1].Replace(";", "").Trim();

        string regLeft = registerAllocator.Allocate(left);

        // Eğer right kısmı sabit sayıysa
        if (int.TryParse(right, out int number))
        {
            output.Add($"li {regLeft}, {number}   # {left} = {number}");
        }
        // Eğer right kısmı tek bir değişkense
        else if (Regex.IsMatch(right, @"^[a-zA-Z_]\w*$"))
        {
            string regRight = registerAllocator.Allocate(right);
            output.Add($"mv {regLeft}, {regRight}   # {left} = {right}");
        }
        // Eğer right kısmı bir aritmetik ifade ise (örneğin: a + b)
        else
        {
            // Basit aritmetik işlemleri tanı (örn. a + b, x - y)
            var match = Regex.Match(right, @"^(\w+)\s*([\+\-\*/])\s*(\w+)$");
            if (!match.Success)
                throw new Exception("Geçersiz aritmetik ifade: " + right);

            string operand1 = match.Groups[1].Value;
            string op = match.Groups[2].Value;
            string operand2 = match.Groups[3].Value;

            string reg1 = registerAllocator.Allocate(operand1);
            string reg2 = registerAllocator.Allocate(operand2);

            string instr = op switch
            {
                "+" => "add",
                "-" => "sub",
                "*" => "mul",
                "/" => "div",
                _ => throw new Exception("Bilinmeyen işlem: " + op)
            };

            output.Add($"{instr} {regLeft}, {reg1}, {reg2}   # {left} = {operand1} {op} {operand2}");
        }
    }


    //Ecem
    private (string leftReg, string rightReg, string branchInstr) ExtractCondition(string line) //3 değer geri dönücek conditionu ayırmamız için 
    {
        var match = Regex.Match(line, @"\((\w+)\s*([!<>=]=?|==)\s*(\w+)\)"); //Rgex ile formata uyuyo mu diye baktık

        if (!match.Success)
        {
            throw new Exception("Koşul hatalı");

        }
        string left = match.Groups[1].Value;  //lefthandside  regexteki ilk parantz için eşleşen kısım
        string op = match.Groups[2].Value;    //operation     ikinci parantez ile eşleşen bölüm
        string right = match.Groups[3].Value;  //righthandside üçünü parantez ile eşleşen bölüm


        string leftReg = registerAllocator.Allocate(left);
        string rightReg = registerAllocator.Allocate(right);
        //değişkenler registerlarda

        string branchInstr = op switch
        {
            ">" => "bgt", // a > b
            "<" => "blt", // a < b
            "==" => "beq", // a == b
            "!=" => "bne", // a != b
            ">=" => "bge", // a >= b
            "<=" => "ble", // a <= b
            _ => throw new Exception("Bilinmeyen karşılaştırma operatörü")
        };

        return (leftReg, rightReg, branchInstr);
    }



    

}
