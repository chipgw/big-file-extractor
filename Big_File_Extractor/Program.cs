using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Big_File_Extractor
{
    class Program
    {
        const string applicationfileversion = "1.0.0.0";

        static string GetStringToChar(BinaryReader file, char end = '\0')
        {
            string data = "";
            for (int I = 0; ; I++)
            {
                char character = file.ReadChar();
                data += character;
                if (data[I] == end)
                    break;
            }
            return data;
        }

        static int GetBinaryNumber(StreamReader file, int places = 4)
        {
            int filecount = 0;
            for (int i = 0; i < places; i++)
            {
                filecount *= 256;
                filecount += file.BaseStream.ReadByte();
            }
            return filecount;
        }

        static int GetBinaryNumber(BinaryReader file, int places = 4)
        {
            int filecount = 0;
            for (int i = 0; i < places; i++)
            {
                filecount *= 256;
                filecount += file.BaseStream.ReadByte();
            }
            return filecount;
        }
        static void Main(string[] args)
        {
            Console.WriteLine("------------------------------------------------------------------------------");
            Console.WriteLine("BIG FILE EXTRACTOR v{0}", applicationfileversion);
            Console.WriteLine("DRay Software 2010");
            Console.WriteLine("------------------------------------------------------------------------------");
            Console.WriteLine();
            if (args.Length > 0)
            {
                Console.WriteLine("Command Line Arguments: {0}", args);
                Console.WriteLine();
                for (int i = 0; i < args.Length; i++)
                {
                    try
                    {
                        Console.WriteLine("------------------------------------------------------------------");
                        // if the file does not exist throw an error
                        if (!File.Exists(args[i]))
                        {
                            throw new NonexistentFileError(args[i]);
                        }

                        //open the file
                        FileStream streambigFile = new FileStream(args[i], FileMode.Open);
                        BinaryReader bigFile = new BinaryReader(streambigFile);
                        

                        // create a variable for the first 4 chars in the file
                        char[] big4head = bigFile.ReadChars(4);

                        // checkes to make sure the header is right
                        if (big4head[0] != 'B' || big4head[1] != 'I' || big4head[2] != 'G' || (big4head[3] != '4' && big4head[3] != 'F'))
                        {
                            throw new BigHeaderError(new string(big4head));
                        }
                        int temporary = bigFile.ReadInt32();

                        // get the amount of files and put it into an integer
                        int filecount = GetBinaryNumber(bigFile);

                        int headerEnd = GetBinaryNumber(bigFile);


                        // write info
                        Console.WriteLine("File #{0} info: {1}", (i + 1), args[i]);
                        Console.WriteLine("Containes {0} Files", filecount);
                        Console.WriteLine("End of header at adress: {0}", headerEnd);
                        Console.WriteLine("Contents:");

                        ContentFileInfo[] contents = new ContentFileInfo[filecount];

                        for (int f = 0; f < filecount; f++)
                        {
                            contents[f] = new ContentFileInfo(GetBinaryNumber(bigFile), GetBinaryNumber(bigFile), GetStringToChar(bigFile));
                            contents[f].fileName = contents[f].fileName.Remove(contents[f].fileName.Length - 1);
                        }

                        // create a folder for output
                        string outFolder = args[i].Remove(args[i].Length - 4) + "\\";
                        while(!Directory.Exists(outFolder))
                        {
                            try
                            {
                                Directory.CreateDirectory(outFolder);
                            }
                            catch
                            {
                                Console.WriteLine("can not create folder \"{0}\" Please Enter a Path for Output", outFolder);
                                outFolder = Console.ReadLine();
                            }
                        }
                        while (outFolder.IndexOf('\\') > 0)
                        {
                            outFolder = outFolder.Insert(outFolder.IndexOf('\\'), "/");
                            outFolder = outFolder.Remove(outFolder.IndexOf('\\'), 1);
                        }
                        foreach (ContentFileInfo currentfile in contents)
                        {
                            bigFile.BaseStream.Position = currentfile.startAddress;

                            Console.WriteLine("File Start Address: {0}", currentfile.startAddress);
                            Console.WriteLine("File Length: {0}", currentfile.dataLength);
                            Console.WriteLine("File Name: {0}", currentfile.fileName);
                            Console.WriteLine();

                            string outFile = outFolder + currentfile.fileName;

#region Directory Creation
                            while (outFile.IndexOf('\\') > 0 || outFile.IndexOf('/') > 0)
                            {
                                char SlashType = '\\';

                                if (!(outFile.IndexOf('\\') > 0) || (outFile.IndexOf('\\') > outFile.IndexOf('/') && outFile.IndexOf('/') > 0))
                                {
                                    SlashType = '/';
                                }
                                
                                try
                                {
                                    Directory.CreateDirectory(outFile.Remove(outFile.IndexOf(SlashType)).Replace("<insertslashhere>", "\\"));
                                }
                                catch (IOException)
                                {
                                    while (true)
                                    {
                                        string input;
                                        do
                                        {
                                            Console.WriteLine("File Exists with the name\"{0}\" Delete File? Y/N", outFile.Remove(outFile.IndexOf(SlashType)).Replace("<insertslashhere>", "\\"));
                                            input = Console.ReadLine();
                                        } while (input.Length < 1);

                                        input = input.ToUpper();
                                        if (input[0] == 'Y')
                                        {
                                            Console.WriteLine("Deleting file \"{0}\"", outFile.Remove(outFile.IndexOf(SlashType)).Replace("<insertslashhere>", "\\"));
                                            File.Delete(outFile.Remove(outFile.IndexOf(SlashType)).Replace("<insertslashhere>", "\\"));
                                            Directory.CreateDirectory(outFile.Remove(outFile.IndexOf(SlashType)).Replace("<insertslashhere>", "\\"));
                                            break;
                                        }
                                        else if (input[0] == 'N')
                                        {
                                            Console.WriteLine("Canceling extract of {0}", outFile);
                                            goto endoffile;
                                        }
                                    }
                                }
                                //catch(Exception err)
                                //{
                                    //Console.WriteLine("ERROR: Canceling extract of {0}", outFile);
                                    //goto endoffile;
                                    //throw err;
                                //}
                                outFile = outFile.Insert(outFile.IndexOf(SlashType), "<insertslashhere>");
                                outFile = outFile.Remove(outFile.IndexOf(SlashType), 1);
                            }
                            outFile = outFile.Replace("<insertslashhere>", "\\");
#endregion
                            if (File.Exists(outFile))
                            {
                                bool done = false;
                                while (!done)
                                {
                                    string input;
                                    do
                                    {
                                        Console.WriteLine("File Named \"" + outFile + "\" Already Existes. Overwrite File? Y/N");
                                        input = Console.ReadLine();
                                    } while (input.Length < 1);

                                    input = input.ToUpper();
                                    if (input[0] == 'Y')
                                    {
                                        Console.WriteLine("Overwriting file \"{0}\"", outFile);
                                        done = true;
                                        break;
                                    }
                                    else if (input[0] == 'N')
                                    {
                                        Console.WriteLine("Skipping File \"" + outFile + "\", File Exists");
                                        done = true;
                                        goto endoffile;
                                    }
                                }
                            }
                            StreamWriter output = new StreamWriter(outFile, false);

                            for (int c = 0; c < currentfile.dataLength; c++)
                            {
                                output.BaseStream.WriteByte(bigFile.ReadByte());
                            }
                            output.Close();
                        endoffile:
                            continue;
                        }
                        bigFile.Close();
                        streambigFile.Close();
                    }
                    #region Catch_Statments
                    catch (BigHeaderError err)
                    {
                        Console.WriteLine("ERROR: BIG header is invalid, looking for 'BIG4' or 'BIGF' found '{0}'", err.foundheader);
                    }
                    catch (NonexistentFileError err)
                    {
                        Console.WriteLine("ERROR: paremeter #{0}: File \"{1}\" does not exist", (i + 1), err.filename);
                    }
                    catch (BigContentsError err)
                    {
                        Console.WriteLine(err.description);
                    }
                    catch(Exception err)
                    {
                        try
                        {
                            Console.WriteLine("An Unhandled Exeption Has Occurred, skipping file {0}\nsee error.log for more information", (i + 1), err.Message);
                            StreamWriter log = new StreamWriter("error.log", false);
                            log.WriteLine("Big File Extractor Error Log");
                            log.WriteLine("Big File Extractor Version {0}", applicationfileversion);
                            log.WriteLine("Error Data");
                            log.WriteLine("--------------------------------------------------------------------------------------");
                            log.WriteLine(err.ToString());
                            log.WriteLine("--------------------------------------------------------------------------------------");
                            log.WriteLine("End Error Data");
                            log.Close();
                        }
                        catch(Exception error)
                        {
                            Console.WriteLine("Unable to write log\nError Data\n{0}\nLog File Error\n{1}".ToString(),err.ToString(), error.ToString());
                        }
                    }
                    #endregion
                }
            }
            else
                Console.WriteLine("No Args Supplied");
            Console.WriteLine("Operation Complete Press any key to exit");
            Console.ReadKey();
        }
    }
}
