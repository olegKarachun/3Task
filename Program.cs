using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace _3Task
{
    class Program
    {
        /*Метод для создания ключа*/
        private static string CreateSalt()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[32];
            rng.GetBytes(buff);

            return Convert.ToBase64String(buff);
        }

        /*Метод для создания HMAC*/
        static string HMACHASH(string str, string key)
        {
            byte[] bkey = Encoding.Default.GetBytes(key);
            using (var hmac = new HMACSHA256(bkey))
            {
                byte[] bstr = Encoding.Default.GetBytes(str);
                var bhash = hmac.ComputeHash(bstr);
                return BitConverter.ToString(bhash).Replace("-", string.Empty).ToLower();
            }
        }

        /*Метод для проверки на дубликаты*/
        private static bool CheckOnSet(string[] ar)
        {
            /*Так как в сете нету дубликатов, мы сравниваем длинны массива 
             * аргументов и сета из него и делаем вывод о дубликатах
             */
            bool answer; 
            SortedSet<string> mySortedSet = new SortedSet<string>();
            for (int i = 0; i<ar.Length; i++)
            {
                mySortedSet.Add(ar[i]);
            }
            answer = mySortedSet.Count == ar.Length;
            return answer;
        }

        /*Метод для проверки аргументов командной строки*/
        private static bool CheckArgs(string[] ar)
        {
            bool answer;
            answer = ar.Length % 2 == 1 && ar.Length>=3 && CheckOnSet(ar) && ar.Length < 6;
            return answer;
        }

        /*Метод для создания и вывода меню*/
        private static void CreateMenu(string[] ar)
        {
            Console.WriteLine("Возможные варианты");
            for(int i = 0; i<ar.Length; i++)
            {
                int k = i;
                Console.WriteLine(++k + " - " + ar[i]);
            }
            Console.WriteLine("0 - Exit");
        }

        /*Метод для реализации хода компьютера*/
        private static string DoMove(string[] ar)
        {
            Random rnd = new Random();
            int move = rnd.Next(0, ar.Length);
            return ar[move];
        }

        /*Метод для определения победителя*/
        private static string GetWinner(string pc, string user, string[]ar)
        {
            /*Определяем где находится в массиве выбранное значение,
             * если правее медианы или является ей, то определяем проигрышное множество слева.
             * Если левее медианы, то определяем проигрышное множество справа.
             * Если ход ПК принадлежит выигрышному множеству и оно существует или
             * ход ПК не принадлежит проигрышному множеству и оно существует, то 
             * выигрывает ПК, иначе Игрок*/

            if (pc == user)
            {
                return "Ничья";
            };

            int mid = ar.Length / 2;
            string[] win = new string[mid];
            string[] lose = new string[mid];
                     
            if(Array.IndexOf(ar, user) >= mid)
            {                
                for (int i = 0; i < mid; i++)
                {
                    lose[i] = ar[Array.IndexOf(ar, user) - i - 1];
                }
            }
            else 
            {         
                for (int i = 0; i < mid; i++)
                {
                    win[i] = ar[Array.IndexOf(ar, user) + i + 1];
                }
            }

            if (Array.IndexOf(win, pc)>=0 && !String.IsNullOrEmpty(win[0])||
                Array.IndexOf(lose, pc) < 0 && !String.IsNullOrEmpty(lose[0]))
            {
                return "Ты проиграл((";
            }
            else
            {
                return "Ты победил!!!";
            }
            
        }

        /*Метод для проверки выбраного пункта меню*/
        private static void CheckInput(string[] args, string key, string PcMove)
        {           
            
            CreateMenu(args);//вывод меню
            try
            {
                Console.Write("Сделай свой ход: ");
                string userAnswer = Console.ReadLine();
                if (userAnswer == "0") System.Environment.Exit(0);
                string value = args[Convert.ToInt32(userAnswer) - 1];
                Console.WriteLine("Твой ход: " + value);
                Console.WriteLine("Мой ход: " + PcMove);
                Console.WriteLine(GetWinner(PcMove, value, args));//определение победителя
                Console.WriteLine("HMAC key: "+key);//ключ
            }
            catch
            {
                CheckInput(args, key, PcMove);
            }
            
        }
        
        static void Main(string[] args)
        {
            if (!CheckArgs(args))
            {
                Console.WriteLine("Ввел неправильные аргументы");
                System.Environment.Exit(0);
            }
            else
            {                
                string PcMove = DoMove(args); //ход компьютера
                string key = CreateSalt(); //ключ хода
                Console.WriteLine("HMAC: "+HMACHASH(PcMove, key)); //вывод HMAC ключа хода

                CheckInput(args, key, PcMove);
            }
        }
    }
}