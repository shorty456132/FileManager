using System;
using System.Text;
using Crestron.SimplSharp;                          				// For Basic SIMPL# Classes
using Crestron.SimplSharp.CrestronIO;
using System.Collections;
using System.Collections.Generic;

namespace FileManager2
{
    public class FileManagement
    {
        public delegate void ReportNamesDelegate(SimplSharpString s,ushort index);
        public ReportNamesDelegate ReportNames { get; set; }

        string[] Names = new string[10] { "Default1", "Default2", "Default3", "Default4", "Default5", "Default6", "Default7", "Default8", "Default9", "Default10" };
        public  List<string> listOfNames = new List<string>();
        
        
        public string _FileLocation { get; set; }
        /// <summary>
        /// SIMPL+ can only execute the default constructor. If you have variables that require initialization, please
        /// use an Initialize method
        /// </summary>
        public FileManagement()
        {
            listOfNames.Capacity = 10;
        }

        public void Initialize(string FileLocation)
        {
            _FileLocation = FileLocation;

            if (!File.Exists(_FileLocation))
            {
                CrestronConsole.PrintLine("Could not find {0}. Creating File with default values...", FileLocation);
                try
                {                   
                    listOfNames.Clear();

                    for (int i = 0; i < 10; i++)
                    {
                        listOfNames.Add(Names[i]);
                    }
                    using (StreamWriter sw = new StreamWriter(_FileLocation,false))
                    {
                        foreach (var Name in listOfNames)
                        {
                            sw.WriteLine(Name);
                        }
                    }
                }
                catch (Exception e)
                {
                    CrestronConsole.PrintLine("Error Creating file: {0}", e);
                }

                RetrieveNamesFromFile();
            }
            else
            {
                RetrieveNamesFromFile();
            }
        } 

        public void saveNameToFile(short index, string name)
        {
            listOfNames[index - 1].Remove(0,listOfNames[index - 1].Length);
            listOfNames[index - 1] = name;

            try
            {
                using (StreamWriter sw = new StreamWriter(_FileLocation,false))
                {
                    foreach (var Name in listOfNames)
                    {                        
                        sw.WriteLine(Name);
                    }

                    sw.Close();
                }
            }
            catch (Exception e)
            {
                CrestronConsole.PrintLine("Error Writing Name to file: {0}", e);
            }

            RetrieveNamesFromFile();
        }

        public void RetrieveNamesFromFile()
        {
            Array.Clear(Names, 0, Names.Length);
            listOfNames.Clear();
            try
            {
                using (StreamReader streamR = new StreamReader(_FileLocation))
                {
                    while (!streamR.EndOfStream)
                    {
                        //Read all the data into a string
                        string Lines = streamR.ReadToEnd();
                        //Splt the data into an array of Names with a delimiter of \n
                        Names = Lines.Split('\n');
                    }
                    Array.Resize(ref Names, Names.Length - 1);

                    int i = 1;
                    foreach (var name in Names)
                    {
                        //avoid adding multiple CR after saving mutliple times
                        listOfNames.Add(name.Replace("\r", ""));    

                        if (ReportNames != null)
                            ReportNames(name,(ushort)i);

                        //increase the iteration to send the index to S+
                        i++;
                    }
                }
            }
            catch (Exception e)
            {
                CrestronConsole.PrintLine("Error Reading file: {0}", e);
            }
        }
    }
}
