using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RıscV_Generator
{
    //Sevde
    //Registerlara eklemek için bir class
     class RegisterAllocator
    {
        private Dictionary<string,string> registerMap= new Dictionary<string,string>();
        //Değişkneleri hangi registerlarda olucağını eşleştirip ,saklıyorum.

        private string[] registers = { "t0", "t1", "t2", "t3", "t4", "t5", "t6" };
        //registerlarım
        private int index;

        public String Allocate(string variable)
        {
            if (!registerMap.ContainsKey(variable))
            { 
                //Eğer keyi (benim değişkenimi) içermiyosa bu map
                if(index >= registers.Length)
                {
                    //ve index register arrayimi aşmışsa ,yani boş register kalmamışsa
                    throw new Exception("Yetersiz register");
                }

                registerMap[variable] = registers[index++];
                // kaldığımız yerdeki indexten sonrakindeki registerı alır ve mapin key değerine koyduğumuz variable ın value suna saklar.

            }
            return registerMap[variable];

        }

        public Dictionary<string, string> GetMap() => registerMap; //dışardan erişilebilir yaptık
    }
}
