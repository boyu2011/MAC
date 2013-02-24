using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO.IsolatedStorage;

namespace MAC
{
    partial class Program
    {
        static void Main(string[] args)
        {
            bool isCreate = false;
            string inputFile = "";
            string outputFile = "";
            string keyString = "";
            bool isSha256 = false;

            //
            // Verify commandline arguments
            //

            var options = new Options();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                // Values are available here
                if (!options.Verbose)
                {
                    return;
                }
                else
                {
                    // Create and Authenticate, one of them, must occur.
                    if ((options.Create == true && options.Authenticate == true) ||
                        (options.Create == false && options.Authenticate == false))
                    {
                        Console.WriteLine("Create and Authenticate, one of them, must occur.");
                        return;
                    }

                    if (options.Create)
                        isCreate = true;
                    else
                        isCreate = false;

                    inputFile = options.InputFile;

                    if (options.Create)
                    {
                        if (!string.IsNullOrEmpty(options.OutputFile))
                            outputFile = options.OutputFile;
                        else
                        {
                            Console.WriteLine("If create is assigned, then output file must be assigned.");
                            return;
                        }
                    }

                    // sha256 and sha512, one of them, must occur.
                    if ((options.Sha256 == true && options.Sha512 == true) ||
                        (options.Sha256 == false && options.Sha512 == false))
                    {
                        Console.WriteLine("sha256 and sha512, one of them, must occur.");
                        return;
                    }

                    if (options.Sha256)
                        isSha256 = true;
                    else
                        isSha256 = false;

                    keyString = options.Key;
                }
            }

            // key
            System.Text.ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] key = encoding.GetBytes(keyString);

            // create pcket
            if (isCreate)
            {

                FileStream inputStream = File.OpenRead(inputFile);
                inputStream.Position = 0;

                byte [] hshValue;
                if (isSha256)
                {
                    HMACSHA256 myhmacsha26 = new HMACSHA256(key);
                    hshValue = myhmacsha26.ComputeHash(inputStream);
                }
                else
                {
                    HMACSHA512 myhmacsha512 = new HMACSHA512(key);
                    hshValue = myhmacsha512.ComputeHash(inputStream);
                }
                
                inputStream.Position = 0;
                FileStream outputStream = File.Create(outputFile);
                outputStream.Write(hshValue, 0, hshValue.Length);

                int bytesRead;
                byte[] buffer = new byte[1024];
                do
                {
                    bytesRead = inputStream.Read(buffer, 0, 1024);
                    outputStream.Write(buffer, 0, bytesRead);
                } while (bytesRead > 0);

                //myhmacsha26.Clear();
        
            }
            // authenticate
            else
            {
                bool ok = true;

                FileStream inputStream = File.OpenRead(inputFile);
                inputStream.Position = 0;

                byte[] computedHash;

                byte[] storeHash;
                if (isSha256)
                {
                    HMACSHA256 hmacsha256 = new HMACSHA256(key);
                    storeHash = new byte[hmacsha256.HashSize / 8];

                    inputStream.Read(storeHash, 0, storeHash.Length);

                    computedHash = hmacsha256.ComputeHash(inputStream);
                }
                else
                {
                    HMACSHA512 hmacsha512 = new HMACSHA512(key);
                    storeHash = new byte[hmacsha512.HashSize / 8];

                    inputStream.Read(storeHash, 0, storeHash.Length);

                    computedHash = hmacsha512.ComputeHash(inputStream);
                }

                for (int i = 0; i < storeHash.Length; i++)
                {
                    if (computedHash[i] != storeHash[i])
                    {
                        Console.WriteLine("Enjected");
                        return;
                    }
                }
                Console.WriteLine("Auth");
            }            
        }
    }
}
