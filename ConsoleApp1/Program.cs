using ConsoleApp1.SRP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            String modulus = "894B645E89E1535BBDAD5B8B290650530801B18EBFBF5E8FAB3C82872A3E9BB7";
            String Username = "alezh";
            String Password = "123456";
            String s1 = "9F3FD912540799D46FF931E589EF80C1C20040DE95A43160A5E53AD87559DFA7";
            String v1 = "8246DCF7CBB4E11700D0AEAC1A4FC2D20734D6DD8A1E0033A5A2E11710F048EF";
            String keyvalue = String.Format("{0}:{1}", Username.ToUpper(), Password.ToUpper());
            var ri = Encoding.UTF8.GetBytes(keyvalue).HashEncode();            

            Srp srp = new Srp(ri, modulus, "7", 32, s1);

            bool verifie = srp.ProofVerifier(v1);
            Console.WriteLine(verifie);
            Console.ReadKey();
        }
    }
}
