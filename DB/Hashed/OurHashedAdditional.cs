using System;
using System.IO;
using System.Text;
using System.Linq;
namespace Hashed{
    partial class OurBlock{

        public OurBlock(){}

        int HashFunction(int num)
        {
            return num%4;
        }

        void ByteArrToBlock(byte[] blockBinary)
        {
            byte[] byteArrByf30 = new byte[30];
            byte[] byteArrByf20 = new byte[20];
            byte[] intArrB = new byte[4];
            int idRB,idG,nextB,back;
            char[] lastName = new char[30];
            char[] name = new char[20];
            char[] patronymic = new char[30];
            for (int i=0;i<blockSize-8;i+=(blockSize-8)/5)
            {
                Array.Copy(blockBinary,i,intArrB,0,4);
                idRB = BitConverter.ToInt32(intArrB, 0);
                Array.Copy(blockBinary,i+4,byteArrByf30,0,30);
                lastName = Encoding.UTF8.GetChars(byteArrByf30);
                Array.Copy(blockBinary,i+34,byteArrByf20,0,20);
                name = Encoding.UTF8.GetChars(byteArrByf20);
                Array.Copy(blockBinary,i+54,byteArrByf30,0,30);
                patronymic  = Encoding.UTF8.GetChars(byteArrByf30);
                Array.Copy(blockBinary,i+84,intArrB,0,4);
                idG = BitConverter.ToInt32(intArrB, 0);
                block.SetZapMass(i/((blockSize-4)/5),idRB,lastName,name,patronymic,idG);
            }
            Array.Copy(blockBinary,blockSize-8,intArrB,0,4);
            nextB = BitConverter.ToInt32(intArrB,0);
            block.Next = nextB;
            Array.Copy(blockBinary,blockSize-4,intArrB,0,4);
            back= BitConverter.ToInt32(intArrB,0);
            block.Back = back;
        }

        public void ReadFullNullBlock(byte[] nullBlockBinary)
        {
            byte[] intArrB = new byte[4];
            int[] intArr = new int[9];
            for(int i=0;i<9;i++)
            {
                Array.Copy(nullBlockBinary,i*4,intArrB,0,4);
                intArr[i] = BitConverter.ToInt32(intArrB, 0);
            }
            nullBlock = new NullBlock(intArr[0],intArr[1],intArr[2],intArr[3],intArr[4],intArr[5],intArr[6],intArr[7],intArr[8]);
        }

        int FindStudent(int idRecordBook){
            for(int i=0;i<5;i++)
            {
                if(block.GetZapMass(i).IdRecordBook==idRecordBook)
                {
                    return i;
                }
            }
            return -1;
        }

        char[] InChar(string str, int length)
        {
            char[] charArr = new char[length];
            for (int i = 0; i < length&&str.Length > i; i++)
            {
                charArr[i] = str[i];
            }
            return charArr;
        }

        string InString(char[] charArr, int length)
        {
            string str="";
            try{
                for (int i = 0; i < length-1; i++)
                {
                    if(charArr[i]=='\0')
                    {
                        break;
                    }
                    str+= charArr[i];
                }
            }
            catch (Exception)
            {
                return str;
            }
            return str;
        }

        public void PrintBlock()
        {
            Console.WriteLine("\n----???????? ????????----");
            for(int i = 0; i < 5; i++){
                if(block.GetZapMass(i).IdRecordBook==0)
                {
                    Console.WriteLine("?????????? ???????????? ?? ??????????: {0}; ??????????",i+1);
                }
                else
                {
                    Console.WriteLine("?????????? ???????????? ?? ??????????: {0}; ?????????? ??????????????: {1}; ??????????????: {2}; ??????: {3}; ????????????????: {4}; ?????????? ????????????: {5};",i+1,block.GetZapMass(i).IdRecordBook,
                    InString(block.GetZapMass(i).Lastname,30),InString(block.GetZapMass(i).Lastname,20),InString(block.GetZapMass(i).Middlename,30),block.GetZapMass(i).IdGroup);
                }
            }
            Console.WriteLine();
        }

        public void PrintFindStudent(int i)
        {
            i = HashFunction(i);
            Console.WriteLine("?????????????? ???????????????? ???? ????????????: ?????????? ??????????????: {0}; ??????????????: {1}; ??????: {2}; ????????????????: {3}; ?????????? ????????????: {4};\n",
            block.GetZapMass(i).IdRecordBook, InString(block.GetZapMass(i).Lastname,30), InString(block.GetZapMass(i).Name,20),
            InString(block.GetZapMass(i).Middlename,30), block.GetZapMass(i).IdGroup);
        }

        byte[] Combine(Zap zap)
        {
            byte[] idRecordBookB = BitConverter.GetBytes(zap.IdRecordBook);
            byte[] lastnameB = Encoding.UTF8.GetBytes(zap.Lastname);
            byte[] nameB = Encoding.UTF8.GetBytes(zap.Name);
            byte[] middlenameB = Encoding.UTF8.GetBytes(zap.Middlename);
            byte[] idIdGroupB = BitConverter.GetBytes(zap.IdGroup);
            return idRecordBookB.Concat(lastnameB.Concat(nameB.Concat(middlenameB.Concat(idIdGroupB)))).ToArray();
        }

        byte[] Combine(byte[] start, byte[] second)
        {
            return start.Concat(second).ToArray();
        }

        byte[] Combine()
        {
            byte[] byteBlock = new byte[0];
            for(int i = 0;i<5;i++)
            {
                byte[] byteZap=Combine(block.GetZapMass(i));
                byteBlock=Combine(byteBlock,byteZap);
            }
            byte[] nextB = BitConverter.GetBytes(block.Next);
            byteBlock=Combine(byteBlock,nextB);
            byte[] back = BitConverter.GetBytes(block.Back);
            return Combine(byteBlock,back);
        }

        void AddZapOnEnd(int idRecordBook,string lastname,string name,string patronymic,int idGroup)
        {
            for(int i=0;i<5;i++)
            {
                if(block.GetZapMass(i).IdRecordBook==0)
                {
                    block.SetZapMass(i,idRecordBook,InChar(lastname,30),InChar(name,20),InChar(patronymic,30),idGroup);
                    break;
                }
            }
        }

        public int SearchEndCheck(int idRecordBook,string filename)
        {
            int idRBHashed = HashFunction(idRecordBook);
            int start = nullBlock.GetPointersStart(idRBHashed);
            using (var reader = File.Open(filename, FileMode.Open))
            {
                byte[] blockBinary = new byte[blockSize];
                int numZapFound;
                while(start!=0)
                {
                    reader.Seek(start, SeekOrigin.Begin);
                    reader.Read(blockBinary, 0, blockSize);
                    ByteArrToBlock(blockBinary);
                    if((numZapFound=FindStudent(idRecordBook))!=-1)
                    {
                        reader.Close();
                        return numZapFound;
                    }
                    start=block.Next;
                }
                if (FindStudent(0)==-1)
                {
                    return -1;
                }
            }
            return -2;
        }

        public int SearchForDell(int idRecordBook,string filename)
        {
            int idRBHashed = HashFunction(idRecordBook);
            int start = nullBlock.GetPointersStart(idRBHashed);
            using (var reader = File.Open(filename, FileMode.Open))
            {
                byte[] blockBinary = new byte[blockSize];
                while(start!=0)
                {
                    reader.Seek(start, SeekOrigin.Begin);
                    reader.Read(blockBinary, 0, blockSize);
                    ByteArrToBlock(blockBinary);
                    if(FindStudent(idRecordBook)!=-1)
                    {
                        int end = nullBlock.GetPointersEnd(idRBHashed);
                        if(start!=end)
                        {
                            reader.Seek(end, SeekOrigin.Begin);
                            reader.Read(blockBinary, 0, blockSize);
                            ByteArrToBlock(blockBinary);
                        }
                        reader.Close();
                        return start;
                    }
                    start=block.Next;
                }
            }
            return -1;
        }

        public int SearchForChange(int oldIdRecordBook,int idRecordBook,string filename)
        {
            int oldIdRBHashed = HashFunction(oldIdRecordBook);
            int idRBHashed = HashFunction(idRecordBook);
            if(oldIdRBHashed==idRBHashed)
            {
                int start = nullBlock.GetPointersStart(idRBHashed);
                int addr = 0;
                using (var reader = File.Open(filename, FileMode.Open))
                {
                    bool foundOldId=false;
                    byte[] blockBinary = new byte[blockSize];
                    byte[] findBlockBinary = new byte[blockSize];
                    while(start!=0)
                    {
                        reader.Seek(start, SeekOrigin.Begin);
                        reader.Read(blockBinary, 0, blockSize);
                        ByteArrToBlock(blockBinary);
                        if(FindStudent(oldIdRecordBook)!=-1)
                        {
                            foundOldId=true;
                            findBlockBinary=blockBinary;
                            addr = start;
                        }
                        if(FindStudent(idRecordBook)!=-1)
                        {
                            Console.WriteLine("???????????? ???? ?????????????? ???? ???????????? ???????????????? ?????? ???????? ?? ????");
                            return -3;
                        }
                        start=block.Next;
                    }
                    if(foundOldId)
                    {
                        ByteArrToBlock(findBlockBinary);
                        return addr;
                    }
                    Console.WriteLine("???????????? ?????????????? ???? ???????????? ???????????????? ?????? ???????? ?? ????");
                    return -3;
                }
            }
            return -3;
        }
    }
}
