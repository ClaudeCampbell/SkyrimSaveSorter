using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace SkyrimSaveSorter
{
    public class Sorter
    {

        public static ProgressBar bar { get; set; }

        public static void sortSaves()
        {
            List<SaveObject> unsortedSaves = createSaveList();

            List<SaveObject> saves = unsortedSaves.OrderByDescending(o => o.CreationDate).ToList();

            getMoveStatus(ref saves);

            resetbar(saves.Count);

            foreach (SaveObject save in saves)
            {
                if (save.Move)
                {
                    try
                    {
                        string essMove = Properties.Settings.Default.ArchiveFolder + "\\" + Path.GetFileName(save.ESSPath);
                        if (File.Exists(essMove))
                        {
                            File.Delete(essMove);
                        }
                        File.Move(save.ESSPath, essMove);

                        if (!String.IsNullOrEmpty(save.SKSEPath) && File.Exists(save.SKSEPath))
                        {
                            string skseMove = Properties.Settings.Default.ArchiveFolder + "\\" + Path.GetFileName(save.SKSEPath);
                            if (File.Exists(skseMove))
                            {
                                File.Delete(skseMove);
                            }                   
                            File.Move(save.SKSEPath, skseMove);
                        }
                    } 
                    catch(Exception ex)
                    {

                    }


                }

                stepBar();
            }
            MessageBox.Show("Done");
        }

        //Sets the move property for a list of saves
        static void getMoveStatus(ref List<SaveObject> saves)
        {
            List<string> uniqueNames = new List<string>();

           List<CharacterBySaves> savesPerChar = new List<CharacterBySaves>();

            
            //saves.OrderBy(o => o.CreationDate);

            foreach (SaveObject save in saves)
            {
                if (save.isAutosave)
                {
                    save.Move = false;
                    continue;
                }
                if (!uniqueNames.Contains(save.Character))
                {
                    uniqueNames.Add(save.Character);
                    savesPerChar.Add(new CharacterBySaves { name = save.Character, numberOfSaves = 1 });
                    //save.SaveNumber++;
                    save.Move = false;
                }
                else
                {

                    
                    int index = -1;
                    for(int i = 0; i < savesPerChar.Count; i++)
                    {
                        if(String.Equals(savesPerChar[i].name, save.Character, StringComparison.OrdinalIgnoreCase))
                        {
                            index = i;
                            break;
                        }
                    }
                    if(index >= 0)
                    {
                        savesPerChar[index].numberOfSaves++;
                        save.SaveNumber = savesPerChar[index].numberOfSaves;
                    }
                    
                }
            }
            foreach(SaveObject save in saves)
            {
                if (save.isAutosave) { continue; }
                else if (save.Move) { continue; }
                else if (DateTime.Compare(save.CreationDate,Properties.Settings.Default.OldestDate) < 0) { save.Move = randomMove(); } //I have it set to keep 10% of other saves, not really useful or necessary, but it was part of the idea I had at the time so this will stay for now
                else if (save.SaveNumber >= Properties.Settings.Default.NumberOfSaves) { save.Move = randomMove(); }
            }
        }

        static List<SaveObject> createSaveList()
        {
            List<SaveObject> saveList = new List<SaveObject>();

            string[] saves = Directory.GetFiles(Properties.Settings.Default.SaveFolder);

            foreach (string save in saves)
            {
                string type = Path.GetExtension(save);
                if (String.Equals(type, ".ess", StringComparison.OrdinalIgnoreCase))
                {
                    SaveObject mainSave = new SaveObject();
                    mainSave.ESSPath = save;
                    mainSave.SKSEPath = Path.ChangeExtension(save, "skse");
                    if(!save.Contains("autosave") || !save.Contains("quicksave"))
                    {
                        mainSave.isAutosave = false;

                        using (FileStream fs = File.Open(save, FileMode.Open))
                        {
                            byte[] b = new byte[128];
                            fs.Read(b, 0, 128);

                            char[] t = System.Text.Encoding.UTF8.GetString(b).ToCharArray();

                            int nameLength = b[25]; //name length is given at 25

                            StringBuilder n = new StringBuilder(); 

                            for(int i = 0; i < nameLength; i++)
                            {
                                n.Append(t[27 + i]);
                            }
                            mainSave.Character = n.ToString();

                            n.Clear();

                            int locLength = b[27+nameLength+4]; //this is where the location name length should be given

                            for (int i = 0; i < locLength; i++)
                            {
                                n.Append(t[27 + nameLength + 6 + i]);
                            }
                            mainSave.Location = n.ToString();
                        }

                        //if the file was moved then creation time will be earlier than actual creation
                        if (File.GetLastWriteTime(save) >= File.GetCreationTime(save)) { mainSave.CreationDate = File.GetCreationTime(save); }
                        else mainSave.CreationDate = File.GetLastWriteTime(save);
                                                  
                    }
                    else
                    {
                        mainSave.isAutosave = true;
                    }

                    saveList.Add(mainSave);
                }
                
            }
            return saveList;
        }

        //deletes tmp save files (typically files broken during saving)
        public static void deleteTmp()
        {
            string[] saves = Directory.GetFiles(Properties.Settings.Default.SaveFolder);

            resetbar(saves.Length);

            foreach (string save in saves)
            {
                if(Path.GetExtension(save) == ".tmp")
                {
                    try
                    {
                        File.Delete(save);
                        
                    }
                    catch(Exception ex)
                    {

                    }                  
                }
                stepBar();
            }
        }

        // Returns true 90% of the time to keep a random number of old saves
        static bool randomMove()
        {
            Random r = new Random();
            int rg = r.Next(0, 100);

            return rg <= 90;
        }


        // Resets the progress bar   
        static void resetbar(int barlength)
        {
            bar.Invoke(new Action(() => { bar.Minimum = 1; }));
            bar.Invoke(new Action(() => { bar.Maximum = barlength; }));
            bar.Invoke(new Action(() => { bar.Value = 1; }));
            bar.Invoke(new Action(() => { bar.Step = 1; }));
        }

        // Steps the progress bar by 1
        static void stepBar()
        {
            bar.Invoke(new Action(() => { bar.PerformStep(); }));
        }
    }

    class SaveObject
    {
        public bool isAutosave { get; set; }

        public string Character { get; set; }
        public string Location { get; set; }
        public string ESSPath { get; set; }
        public string SKSEPath { get; set; }

        public DateTime CreationDate { get; set; }

        public int SaveNumber { get; set; }

        public bool Move { get; set; }
    }
    
    class CharacterBySaves
    {
        public string name { get; set; }
        public int numberOfSaves { get; set; }
    }
}
