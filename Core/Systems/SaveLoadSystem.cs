using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using RogueSharp.Random;

namespace sharpRoguelike.Core.Systems
{
    //boring - serialisation boilerplate stuff. at least its not cpp. 
    //so all classes that can be serialised need to be marked with [serliazalble] on 
    //the top - which is pretty much everything in the whole game, with exception 
    //to any of the API stuff, like RLColor, or - ITile or whatever -so these things 
    //have to be kind of manually done. i have just extracted these parts to structs and
    //seralised the structs - and then reapplied the structs back to api language after 
    public class SaveLoadSystem
    {
        public string savePathName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDoc‌​uments), "ALCHYMIA");

        public SaveLoadSystem()
        {
            System.IO.Directory.CreateDirectory(savePathName);
            saveName = System.IO.Path.Combine(savePathName, "Game.sav");
            seedName = System.IO.Path.Combine(savePathName,  "GameSeed.sav");
        }

        public string saveName;
        public string seedName ;
        public void SaveGame()
        {
            Game.Player.SaveEntityColor();
            Game.DungeonMap.SaveEntityColors();

            FileStream stream =  File.Create(saveName);
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, Game.seed);
            formatter.Serialize(stream, Game.Player);
            formatter.Serialize(stream, Game.DungeonMap.s_cells);
            formatter.Serialize(stream, Game.DungeonMap.Doors);
            formatter.Serialize(stream, Game.DungeonMap.Monsters);
            formatter.Serialize(stream, Game.DungeonMap.Items);
            formatter.Serialize(stream, Game.DungeonMap.Surfaces);
            formatter.Serialize(stream, Game.DungeonMap.StairsDown);
            stream.Close();

        }

        public void SaveSeed()
        {
            FileStream stream = File.Create(seedName);
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, Game.seed);
            formatter.Serialize(stream, Game.mapLevel);
            formatter.Serialize(stream, Game.Random);

            stream.Close();

        }

        public void LoadGame()
        {
            var formatter = new BinaryFormatter();
            FileStream stream = File.OpenRead(saveName);
            Game.seed = (int)formatter.Deserialize(stream);
            Game.Player = (Entity)formatter.Deserialize(stream);
            Game.DungeonMap.s_cells = (List<SerialiseableCells>)formatter.Deserialize(stream);
            Game.DungeonMap.Doors = (List<Door>)formatter.Deserialize(stream);
            Game.DungeonMap.Monsters = (List<Entity>)formatter.Deserialize(stream);
            Game.DungeonMap.Items = (List<Entity>)formatter.Deserialize(stream);
            Game.DungeonMap.Surfaces = (List<Entity>)formatter.Deserialize(stream);
            Game.DungeonMap.StairsDown = (Stairs)formatter.Deserialize(stream);
            stream.Close();
        }

        public void LoadSeed()
        {
            var formatter = new BinaryFormatter();
            FileStream stream = File.OpenRead(seedName);
            Game.seed = (int)formatter.Deserialize(stream);
            Game.mapLevel = (int)formatter.Deserialize(stream);
            Game.Random = (RandomWrapper)formatter.Deserialize(stream);
            Game.Random.random = new DotNetRandom(Game.seed);
            Game.Random.Restore();
            stream.Close();
        }
    }
}
