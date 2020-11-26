using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace sharpRoguelike.Core.Systems
{
    public class SaveLoadSystem
    {
        public string saveName = "Game.sav";
        public string seedName = "GameSeed.sav";
        public void SaveGame()
        {
            Game.Player.SaveEntityColor();
            Game.DungeonMap.SaveEntityColors();

            FileStream stream =  File.Create(saveName);
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, Game.seed);
            formatter.Serialize(stream, Game.Player);
            formatter.Serialize(stream, Game.DungeonMap.Doors);
            formatter.Serialize(stream, Game.DungeonMap.Monsters);
            formatter.Serialize(stream, Game.DungeonMap.Items);
            formatter.Serialize(stream, Game.DungeonMap.Surfaces);
            formatter.Serialize(stream, Game.DungeonMap.StairsDown);
 

            Game.Player.color = Colors.Player;
            Game.Player.symbol = '@';
            Game.Player.name = "Rogue";
            stream.Close();

        }

        public void SaveSeed()
        {
            FileStream stream = File.Create(seedName);
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, Game.seed);
            stream.Close();

        }

        public void LoadGame()
        {
            var formatter = new BinaryFormatter();
            FileStream stream = File.OpenRead(saveName);
            Game.seed = (int)formatter.Deserialize(stream);
            Game.Player = (Player)formatter.Deserialize(stream);
            Game.DungeonMap.Doors = (List<Door>)formatter.Deserialize(stream);
            Game.DungeonMap.Monsters = (List<Monster>)formatter.Deserialize(stream);
            Game.DungeonMap.Items = (List<Entity>)formatter.Deserialize(stream);
            Game.DungeonMap.Surfaces = (List<Entity>)formatter.Deserialize(stream);
            Game.DungeonMap.StairsDown = (Stairs)formatter.Deserialize(stream);
            stream.Close();
        }

        public void LoadSeed()
        {
            var formatter = new BinaryFormatter();
            FileStream stream = File.OpenRead(saveName);
            Game.seed = (int)formatter.Deserialize(stream);
            stream.Close();
        }
    }
}
