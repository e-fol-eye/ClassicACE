using System;
using System.Collections.Generic;

using log4net;

using ACE.Common;
using ACE.Database.Models.Shard;
using ACE.Server.WorldObjects;

namespace ACE.Server.Managers
{
    public class CampManager
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Player Player { get; }

        private ICollection<CharacterPropertiesCampRegistry> runtimeCamps { get; set; } = new HashSet<CharacterPropertiesCampRegistry>();

        public string Name
        {
            get
            {
                if (Player != null)
                    return Player.Name;
                else
                    return "";
            }

        }
        public uint IDtoUseForCampRegistry
        {
            get
            {
                if (Player != null)
                    return Player.Guid.Full;
                else
                    return 1;
            }
        }

        public static bool Debug = false;

        /// <summary>
        /// Constructs a new CampManager for a Player
        /// </summary>
        public CampManager(Player player)
        {
            Player = player;
        }

        /// <summary>
        /// This will return a clone of the camps collection. You should not mutate the results.
        /// This is mostly used for information/debugging
        /// </summary>
        /// <returns></returns>
        public ICollection<CharacterPropertiesCampRegistry> GetCamps()
        {
            return Player.Character.GetCamps(Player.CharacterDatabaseLock);
        }

        /// <summary>
        /// Returns TRUE if a player has started a particular quest
        /// </summary>
        public bool HasCamp(uint campId)
        {
            var hasCamp = GetCamp(campId) != null;

            if (Debug)
                Console.WriteLine($"{Name}.CampManager.HasCamp({campId}): {hasCamp}");

            return hasCamp;
        }

        /// <summary>
        /// Returns a camp for this player
        /// </summary>
        public CharacterPropertiesCampRegistry GetCamp(uint campId)
        {
            return Player.Character.GetCamp(campId, Player.CharacterDatabaseLock);
        }

        private CharacterPropertiesCampRegistry GetOrCreateCamp(uint campId, out bool campRegistryWasCreated)
        {
            var camp = Player.Character.GetOrCreateCamp(campId, Player.CharacterDatabaseLock, out campRegistryWasCreated);
            camp.CharacterId = IDtoUseForCampRegistry;
            return camp;
        }


        /// <summary>
        /// Returns TRUE if player has reached the maximum # of interactions for this camp
        /// </summary>
        public bool HasMaxInteractions(uint campId)
        {
            return HasMaxInteractions(GetCamp(campId));
        }

        public bool HasMaxInteractions(CharacterPropertiesCampRegistry camp)
        {
            if (camp == null)
                return false;
            return camp.NumInteractions == GetMaxInteractions(camp.CampId);
        }

        /// <summary>
        /// Returns the current # of interactions for this camp
        /// </summary>
        public uint GetCurrentInteractions(uint campId)
        {
            var camp = GetCamp(campId);
            if (camp == null)
                return 0;

            return camp.NumInteractions;
        }

        /// <summary>
        /// Initialize a camp with the provided number to the player's registry
        /// </summary>
        public CharacterPropertiesCampRegistry SetCampInteractions(uint campId, uint campInteractions = 0)
        {
            var camp = GetOrCreateCamp(campId, out _);
            SetCampInteractions(camp, campInteractions);
            return camp;
        }

        public void SetCampInteractions(CharacterPropertiesCampRegistry camp, uint campInteractions = 0)
        {
            var maxInteractions = GetMaxInteractions(camp.CampId);

            camp.NumInteractions = Math.Min(campInteractions, maxInteractions);
            camp.LastDecayTime = (uint)Time.GetUnixTime();

            if (Debug)
                Console.WriteLine($"{Name}.CampManager.SetCampInteractions({camp.CampId}): set camp interactions to {camp.NumInteractions}");

            Player.CharacterChangesDetected = true;
        }

        /// <summary>
        /// Increment the number of interactions of a camp
        /// </summary>
        public CharacterPropertiesCampRegistry Increment(uint campId, uint amount = 1)
        {
            var camp = GetOrCreateCamp(campId, out _);
            Increment(camp, amount);
            return camp;
        }

        public void Increment(CharacterPropertiesCampRegistry camp, uint amount = 1)
        {
            var maxAmount = GetMaxInteractions(camp.CampId);
            var numInteractionsToIncrement = Math.Min(amount, maxAmount);

            camp.LastDecayTime = (uint)Time.GetUnixTime();
            if (HasMaxInteractions(camp.CampId))
            {
                if (Debug)
                    Console.WriteLine($"{Name}.CampManager.Update({camp.CampId}): can not update existing camp. HasMaxInteractions({camp.CampId}) is true.");
                return;
            }

            if (maxAmount == uint.MaxValue && camp.NumInteractions + numInteractionsToIncrement < camp.NumInteractions) // Check for overflow.
                camp.NumInteractions = maxAmount;
            else if (camp.NumInteractions + numInteractionsToIncrement >= maxAmount)
                camp.NumInteractions = maxAmount;
            else
                camp.NumInteractions += numInteractionsToIncrement;

            if (Debug)
                Console.WriteLine($"{Name}.CampManager.Update({camp.CampId}): updated camp interactions({camp.NumInteractions})");

            Player.CharacterChangesDetected = true;
        }

        /// <summary>
        /// Decrement the number of interactions of a camp
        /// </summary>
        public CharacterPropertiesCampRegistry Decrement(uint campId, uint amount = 1)
        {
            var camp = GetCamp(campId);
            Decrement(camp);

            return camp;
        }

        public void Decrement(CharacterPropertiesCampRegistry camp, uint amount = 1)
        {
            if (camp != null)
            {
                // update existing camp
                if (camp.NumInteractions >= amount)
                    camp.NumInteractions -= amount;
                else
                    camp.NumInteractions = 0;

                if (Debug)
                    Console.WriteLine($"{Name}.CampManager.Decrement({camp.CampId}): updated camp interactions to ({camp.NumInteractions})");

                Player.CharacterChangesDetected = true;
            }
        }

        /// <summary>
        /// Removes an existing camp from the Player's registry
        /// </summary>
        public void Erase(uint campId)
        {
            if (Debug)
                Console.WriteLine($"{Name}.CampManager.Erase({campId})");

            if (Player.Character.EraseCamp(campId, Player.CharacterDatabaseLock))
                Player.CharacterChangesDetected = true;
        }

        /// <summary>
        /// Removes an all quests from registry
        /// </summary>
        public void EraseAll()
        {
            if (Debug)
                Console.WriteLine($"{Name}.CampManager.EraseAll");

            Player.Character.EraseAllQuests(out var campIdsErased, Player.CharacterDatabaseLock);

            if (campIdsErased.Count > 0)
                Player.CharacterChangesDetected = true;
        }

        /// <summary>
        /// Shows the current camps for a Player
        /// </summary>
        public void ShowCamps(Player player)
        {
            Console.WriteLine("ShowCamps");

            var camps = GetCamps();

            if (camps.Count == 0)
            {
                Console.WriteLine("No camps for " + Name);
                return;
            }

            foreach (var camp in camps)
            {
                Console.WriteLine("CampId: " + camp.CampId);
                Console.WriteLine("NumInteractions: " + camp.NumInteractions);
                Console.WriteLine("LastDecayTime: " + camp.LastDecayTime);
                Console.WriteLine("Player ID: " + camp.CharacterId.ToString("X8"));
                Console.WriteLine("----");
            }
        }

        /// <summary>
        /// Returns the maximum # of interactions for this camp
        /// </summary>
        public uint GetMaxInteractions(uint campId)
        {
            if (campId == 0) // Rest camp
                return 3000;
            else if (campId > 0x0000FFFF) // Area Camp
                return 500;
            else
                return 2000; // Type Camp
        }

        public CharacterPropertiesCampRegistry CheckDecay(uint campId)
        {
            var camp = GetCamp(campId);
            CheckDecay(camp);

            return camp;
        }

        public void CheckDecay(CharacterPropertiesCampRegistry camp)
        {
            if (camp == null)
                return;

            float decayRate = 60.0f; // The amount of seconds it takes for a interaction to decay.

            double secondsSinceLastCheck = Time.GetUnixTime() - camp.LastDecayTime;
            uint amountToDecay = (uint)Math.Max(Math.Floor(secondsSinceLastCheck / decayRate), 0);
            if (amountToDecay > 0)
            {
                camp.LastDecayTime = (uint)Time.GetUnixTime();

                if (camp.NumInteractions >= amountToDecay)
                    camp.NumInteractions -= amountToDecay;
                else
                    camp.NumInteractions = 0;

                if (Debug)
                    Console.WriteLine($"{Name}.CampManager.CheckDecay({camp.CampId}): updated camp interactions to ({camp.NumInteractions})");

                Player.CharacterChangesDetected = true;
            }

        }

        public void HandleCampInteraction(Creature creature, out float typeCampBonus, out float areaCampBonus, out float restCampBonus)
        {
            typeCampBonus = 0;
            areaCampBonus = 0;
            restCampBonus = 0;

            if (creature == null)
            {
                log.Error($"{Name}.CampManager.HandleCampInteraction: input creature is null!");
                return;
            }

            uint typeCampId = (uint)creature.CreatureType;
            if (typeCampId != 0)
            {
                var typeCamp = GetOrCreateCamp(typeCampId, out _);
                if (typeCamp != null)
                {
                    CheckDecay(typeCamp);
                    typeCampBonus = 1.0f - ((float)typeCamp.NumInteractions / GetMaxInteractions(typeCamp.CampId));
                    Increment(typeCamp);
                }
            }

            uint areaCampId;
            if (creature.CurrentLandblock.IsDungeon)
                areaCampId = creature.CurrentLandblock.Id.Raw & 0xFFFF0000;
            else
                areaCampId = creature.CurrentLandblock.Id.Raw & 0xF0F00000;
            if (areaCampId != 0)
            {
                var areaCamp = GetOrCreateCamp(areaCampId, out _);
                if (areaCamp != null)
                {
                    CheckDecay(areaCamp);
                    areaCampBonus = 1.0f - ((float)areaCamp.NumInteractions / GetMaxInteractions(areaCamp.CampId));
                    Increment(areaCamp);
                }
            }

            var restCamp = GetOrCreateCamp(0, out _);
            if (restCamp != null)
            {
                CheckDecay(restCamp);
                restCampBonus = 1.0f - ((float)restCamp.NumInteractions / GetMaxInteractions(restCamp.CampId));
                Increment(restCamp);
            }
        }
    }
}
