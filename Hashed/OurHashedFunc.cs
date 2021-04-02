using System;
using System.IO;
namespace Hashed{
    partial class OurBlock{

        const int blockSize = 444;
        const int nullBlockSize=36;
        const int quantityPointersNullBlock=8;
        const int firstPointerSize=4;
        const int pointerSize=4;

        Block block = new Block();

        NullBlock nullBlock = new NullBlock();

        public void AddOnEnd(string filename, int idRecordBook,string lastname,string name,string patronymic,int idGroup,int searchEndCheckResult)
        {
            int idRBHashed = HashFunction(idRecordBook);
            int quantityBlock = nullBlock.QuantityBlock;
            int start = nullBlock.GetPointersStart(idRBHashed);
            int end = nullBlock.GetPointersEnd(idRBHashed);
            if(end==0)
            {
                searchEndCheckResult=-1;
            }
            if(searchEndCheckResult==-2){
                byte[] blockBinary = new byte[blockSize];
                AddZapOnEnd(idRecordBook,lastname,name,patronymic,idGroup);
                blockBinary=Combine();
                using (BinaryWriter writer=new BinaryWriter(File.Open(filename, FileMode.Open)))
                {
                    writer.Seek(end,SeekOrigin.Begin);
                    writer.Write(blockBinary);
                }
            }
            else{
                using (BinaryWriter writer=new BinaryWriter(File.Open(filename, FileMode.Open)))
                {
                    writer.Write(quantityBlock+1);
                    nullBlock.SetQuantityBlock(quantityBlock+1);
                    writer.Seek(quantityBlock*blockSize+nullBlockSize,SeekOrigin.Begin);
                    block.SetZapMass(0,idRecordBook,InChar(lastname,30),InChar(name,20),InChar(patronymic,30),idGroup);
                    byte[] blockBinary=Combine(block.GetZapMass(0));
                    Array.Resize(ref blockBinary,blockSize);
                    writer.Write(blockBinary);
                    if(start==0)
                    {
                        start = quantityBlock*blockSize+nullBlockSize;
                        end = start;
                        nullBlock.SetPointersStart(idRBHashed,start);
                        nullBlock.SetPointersEnd(idRBHashed,end);
                        writer.Seek(idRBHashed*quantityPointersNullBlock+firstPointerSize,SeekOrigin.Begin);
                        writer.Write(start);
                        writer.Seek(idRBHashed*quantityPointersNullBlock+pointerSize+firstPointerSize,SeekOrigin.Begin);
                        writer.Write(end);
                    }
                    else
                    {
                        writer.Seek(end+440,SeekOrigin.Begin);
                        end = quantityBlock*blockSize+nullBlockSize;
                        writer.Write(end);
                        nullBlock.SetPointersEnd(idRBHashed,end);
                        writer.Seek(idRBHashed*quantityPointersNullBlock+8,SeekOrigin.Begin);
                        writer.Write(end);
                    }
                }
            }
        }

        public void Edit(string filename,int oldidRecordBook,int idRecordBook,string lastname,string name, string patronymic,int idGroup,int searchEndCheckResult)
        {
            Remove(oldidRecordBook, filename);
            AddOnEnd(filename, idRecordBook, lastname,name, patronymic, idGroup,searchEndCheckResult);
        }

        public void Remove(int idRecordBook,string filename)
        {
            BlockAddr replaceBlock  = new BlockAddr();
            BlockAddr delBlock  = new BlockAddr();
            int idRBHashed = HashFunction(idRecordBook);
            int quantityBlock = nullBlock.QuantityBlock;
            int end = nullBlock.GetPointersEnd(idRBHashed);
            using (var reader = File.Open(filename, FileMode.Open))
            {
                byte[] blockBinary = new byte[blockSize];
                reader.Seek(end, SeekOrigin.Begin);
                reader.Read(blockBinary, 0, blockSize);
                ByteArrToBlock(blockBinary);
            }
            int numStu=FindStudent(idRecordBook);
            int numlast=FindStudent(0);
            Console.WriteLine(numStu);
            Console.WriteLine(numlast);
            if (numStu==0&&numlast==1)
            {
                Console.WriteLine("ZDES");
                replaceBlock =SearchInfoOnBlock(idRecordBook,filename,replaceBlock );
                using (BinaryWriter writer=new BinaryWriter(File.Open(filename, FileMode.Open)))
                {  
                    if (replaceBlock.Back!=0)
                    {
                        writer.Seek(replaceBlock.Back+440,SeekOrigin.Begin);
                        writer.Write(0);
                    }
                    else
                    {
                        writer.Seek(idRBHashed*quantityPointersNullBlock+firstPointerSize,SeekOrigin.Begin);
                        nullBlock.SetPointersStart(idRBHashed,0);
                        writer.Write(0);
                    }
                    writer.Seek(idRBHashed*quantityPointersNullBlock+pointerSize+firstPointerSize,SeekOrigin.Begin);
                    writer.Write(replaceBlock.Back);
                    nullBlock.SetPointersEnd(idRBHashed,replaceBlock.Back);
                }
                byte[] blockBinary = new byte[blockSize];
                using (var reader = File.Open(filename, FileMode.Open))
                {
                    reader.Seek((quantityBlock-1)*blockSize+nullBlockSize, SeekOrigin.Begin);
                    reader.Read(blockBinary, 0, blockSize);
                    ByteArrToBlock(blockBinary);
                }
                delBlock =SearchInfoOnBlock(block.GetZapMass(0).IdRecordBook,filename,delBlock );
                if(replaceBlock .Addr!=(quantityBlock-1)*blockSize+nullBlockSize)
                {
                    using (BinaryWriter writer=new BinaryWriter(File.Open(filename, FileMode.Open)))
                    {
                        writer.Seek(replaceBlock.Addr,SeekOrigin.Begin);
                        writer.Write(blockBinary); 
                        if (delBlock.Back!=0)
                        {
                            writer.Seek(delBlock.Back+440,SeekOrigin.Begin);
                            writer.Write(replaceBlock .Addr);
                        }
                        else
                        {
                            writer.Seek((block.GetZapMass(0).IdRecordBook)*quantityPointersNullBlock+firstPointerSize,SeekOrigin.Begin);
                            nullBlock.SetPointersStart(HashFunction(block.GetZapMass(0).IdRecordBook),replaceBlock .Addr);
                            writer.Write(replaceBlock .Addr);
                        }
                        writer.Seek(HashFunction(block.GetZapMass(0).IdRecordBook)*quantityPointersNullBlock+pointerSize+firstPointerSize,SeekOrigin.Begin);
                        writer.Write(replaceBlock .Addr);
                        nullBlock.SetPointersEnd(HashFunction(block.GetZapMass(0).IdRecordBook),replaceBlock .Addr);
                    }
                }
                FileStream fileStream = new FileStream(filename, FileMode.Open);
                fileStream.SetLength((quantityBlock-1)*blockSize+nullBlockSize);
                fileStream.Close();
                using (BinaryWriter writer=new BinaryWriter(File.Open(filename, FileMode.Open)))
                {
                    nullBlock.SetQuantityBlock(quantityBlock-1);
                    writer.Write(quantityBlock-1);
                }
            }
            else if (numlast==1)
            {
                                Console.WriteLine("ZDES");
                replaceBlock =SearchInfoOnBlock(0,filename,replaceBlock );
                using (BinaryWriter writer=new BinaryWriter(File.Open(filename, FileMode.Open)))
                {  
                    if (replaceBlock.Back!=0)
                    {
                        writer.Seek(replaceBlock.Back+440,SeekOrigin.Begin);
                        writer.Write(0);
                    }
                    else
                    {
                        writer.Seek(idRBHashed*quantityPointersNullBlock+firstPointerSize,SeekOrigin.Begin);
                        nullBlock.SetPointersStart(idRBHashed,0);
                        writer.Write(0);
                    }
                    writer.Seek(idRBHashed*quantityPointersNullBlock+pointerSize+firstPointerSize,SeekOrigin.Begin);
                    writer.Write(replaceBlock.Back);
                    nullBlock.SetPointersEnd(idRBHashed,replaceBlock.Back);
                }
                byte[] blockBinary = new byte[blockSize];
                Zap lastZap = new Zap();
                using (var reader = File.Open(filename, FileMode.Open))
                {
                    reader.Seek((quantityBlock-1)*blockSize+nullBlockSize, SeekOrigin.Begin);
                    reader.Read(blockBinary, 0, blockSize);
                    ByteArrToBlock(blockBinary);
                    lastZap = block.GetZapMass(0);
                }
                delBlock =SearchInfoOnBlock(idRecordBook,filename,delBlock);

                    using (BinaryWriter writer=new BinaryWriter(File.Open(filename, FileMode.Open)))
                    {
                        Console.WriteLine(FindStudent(idRecordBook));
                        block.SetZapMass(FindStudent(idRecordBook),lastZap);
                        blockBinary = Combine();
                        writer.Seek(delBlock.Addr,SeekOrigin.Begin);
                        writer.Write(blockBinary); 
                        /*if (delBlock.Back!=0)
                        {
                            writer.Seek(delBlock.Back+440,SeekOrigin.Begin);
                            writer.Write(replaceBlock .Addr);
                        }
                        else
                        {
                            writer.Seek((block.GetZapMass(0).IdRecordBook)*quantityPointersNullBlock+firstPointerSize,SeekOrigin.Begin);
                            nullBlock.SetPointersStart(HashFunction(block.GetZapMass(0).IdRecordBook),replaceBlock .Addr);
                            writer.Write(replaceBlock .Addr);
                        }
                        writer.Seek(HashFunction(block.GetZapMass(0).IdRecordBook)*quantityPointersNullBlock+pointerSize+firstPointerSize,SeekOrigin.Begin);
                        writer.Write(replaceBlock .Addr);
                        nullBlock.SetPointersEnd(HashFunction(block.GetZapMass(0).IdRecordBook),replaceBlock .Addr);*/
                    }
                
                FileStream fileStream = new FileStream(filename, FileMode.Open);
                fileStream.SetLength((quantityBlock-1)*blockSize+nullBlockSize);
                fileStream.Close();
                using (BinaryWriter writer=new BinaryWriter(File.Open(filename, FileMode.Open)))
                {
                    nullBlock.SetQuantityBlock(quantityBlock-1);
                    writer.Write(quantityBlock-1);
                }
            }
            else
            {
                numlast--;
                Zap lastZap = new Zap(block.GetZapMass(numlast));
                if(numStu==-1)
                {
                    block.SetZapMass(numlast,0,InChar("0",30),InChar("0",20), InChar("0",30),0);
                    using (BinaryWriter writer=new BinaryWriter(File.Open(filename, FileMode.Open)))
                    {
                        writer.Seek(end,SeekOrigin.Begin);
                        byte[] blockBinary = Combine();
                        writer.Write(blockBinary);
                    }
                    replaceBlock =SearchInfoOnBlock(idRecordBook,filename,replaceBlock );
                    numStu=FindStudent(idRecordBook);
                    block.SetZapMass(numStu, lastZap); 
                    using (BinaryWriter writer=new BinaryWriter(File.Open(filename, FileMode.Open)))
                    {
                        writer.Seek(replaceBlock.Addr,SeekOrigin.Begin);
                        byte[] blockBinary = Combine();
                        writer.Write(blockBinary);
                    }
                }
                else
                {
                    block.SetZapMass(numStu, block.GetZapMass(numlast)); 
                    block.SetZapMass(numlast,0,InChar("0",30),InChar("0",20), InChar("0",30),0);
                    using (BinaryWriter writer=new BinaryWriter(File.Open(filename, FileMode.Open)))
                    {
                        writer.Seek(end,SeekOrigin.Begin);
                        byte[] blockBinary = Combine();
                        writer.Write(blockBinary);
                    }
                }
            }
        }

        public int Search(int idRecordBook,string filename)
        {
            int idRBHashed = HashFunction(idRecordBook);
            int quantityBlock = nullBlock.QuantityBlock;
            int start = nullBlock.GetPointersStart(idRBHashed);
            int end = nullBlock.GetPointersEnd(idRBHashed);
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
                    start=block.Nextb;
                }
            }
            return -1;
        }
    }
}
