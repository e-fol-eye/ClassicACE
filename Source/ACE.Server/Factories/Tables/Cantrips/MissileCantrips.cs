using System.Collections.Generic;

using log4net;

using ACE.Entity.Enum;
using ACE.Server.Factories.Entity;

namespace ACE.Server.Factories.Tables
{
    public static class MissileCantrips
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly List<SpellId> spells = new List<SpellId>()
        {
            SpellId.CANTRIPSTRENGTH1,
            SpellId.CANTRIPENDURANCE1,
            SpellId.CANTRIPCOORDINATION1,
            SpellId.CANTRIPQUICKNESS1,      // added, according to spellSelectionGroup6

            SpellId.CANTRIPBLOODTHIRST1,
            SpellId.CANTRIPHEARTTHIRST1,
            SpellId.CANTRIPDEFENDER1,
            SpellId.CANTRIPSWIFTHUNTER1,

            SpellId.CantripDirtyFightingProwess1,
            SpellId.CantripRecklessnessProwess1,
            SpellId.CantripSneakAttackProwess1,
        };

        private static readonly int NumLevels = 4;

        // original api
        public static readonly SpellId[][] Table = new SpellId[spells.Count][];

        static MissileCantrips()
        {
            // takes ~0.3ms
            BuildSpells();

            if (Common.ConfigManager.Config.Server.WorldRuleset == Common.Ruleset.Infiltration)
            {
                missileCantrips = new ChanceTable<SpellId>(ChanceTableType.Weight)
                {
                    ( SpellId.CANTRIPMISSILEWEAPONSAPTITUDE1, 1.0f ),        // gets mutated into weapon skill aptitude,

                    ( SpellId.CANTRIPDEFENDER1,               0.7f ),
                    ( SpellId.CANTRIPSTRENGTH1,               0.7f ),
                    ( SpellId.CANTRIPCOORDINATION1,           0.7f ),

                    ( SpellId.CANTRIPBLOODTHIRST1,            0.6f ),
                    ( SpellId.CANTRIPSWIFTHUNTER1,            0.6f ),
                    ( SpellId.CANTRIPQUICKNESS1,              0.6f ),

                    ( SpellId.CANTRIPENDURANCE1,              0.5f ),

                    ( SpellId.CANTRIPARCANEPROWESS1,          0.4f ),
                    ( SpellId.CANTRIPIMPREGNABILITY1,         0.4f ),

                    ( SpellId.CANTRIPINVULNERABILITY1,        0.3f ),
                    ( SpellId.CANTRIPMAGICRESISTANCE1,        0.3f ),

                    ( SpellId.CANTRIPALCHEMICALPROWESS1,      0.1f ),
                    ( SpellId.CANTRIPARMOREXPERTISE1,         0.1f ),
                    ( SpellId.CANTRIPCOOKINGPROWESS1,         0.1f ),
                    ( SpellId.CANTRIPDECEPTIONPROWESS1,       0.1f ),
                    ( SpellId.CANTRIPFEALTY1,                 0.1f ),
                    ( SpellId.CANTRIPFLETCHINGPROWESS1,       0.1f ),
                    ( SpellId.CANTRIPHEALINGPROWESS1,         0.1f ),
                    ( SpellId.CANTRIPITEMEXPERTISE1,          0.1f ),
                    ( SpellId.CANTRIPJUMPINGPROWESS1,         0.1f ),
                    ( SpellId.CANTRIPLEADERSHIP1,             0.1f ),
                    ( SpellId.CANTRIPLOCKPICKPROWESS1,        0.1f ),
                    ( SpellId.CANTRIPMAGICITEMEXPERTISE1,     0.1f ),
                    ( SpellId.CANTRIPMONSTERATTUNEMENT1,      0.1f ),
                    ( SpellId.CANTRIPPERSONATTUNEMENT1,       0.1f ),
                    ( SpellId.CANTRIPSPRINT1,                 0.1f ),
                    ( SpellId.CANTRIPWEAPONEXPERTISE1,        0.1f ),

                    ( SpellId.CANTRIPARMOR1,                  0.1f ),
                    ( SpellId.CANTRIPACIDWARD1,               0.1f ),
                    ( SpellId.CANTRIPBLUDGEONINGWARD1,        0.1f ),
                    ( SpellId.CANTRIPFLAMEWARD1,              0.1f ),
                    ( SpellId.CANTRIPFROSTWARD1,              0.1f ),
                    ( SpellId.CANTRIPPIERCINGWARD1,           0.1f ),
                    ( SpellId.CANTRIPSLASHINGWARD1,           0.1f ),
                    ( SpellId.CANTRIPSTORMWARD1,              0.1f ),

                    ( SpellId.CANTRIPFOCUS1,                  0.1f ),
                    ( SpellId.CANTRIPWILLPOWER1,              0.1f ),
                };
            }
            else if (Common.ConfigManager.Config.Server.WorldRuleset == Common.Ruleset.CustomDM)
            {
                missileCantrips = new ChanceTable<SpellId>(ChanceTableType.Weight)
                {
                    ( SpellId.CANTRIPMISSILEWEAPONSAPTITUDE1, 1.0f ),        // gets mutated into weapon skill aptitude,

                    ( SpellId.CANTRIPDEFENDER1,               0.7f ),
                    ( SpellId.CANTRIPSTRENGTH1,               0.7f ),
                    ( SpellId.CANTRIPCOORDINATION1,           0.7f ),

                    ( SpellId.CANTRIPBLOODTHIRST1,            0.6f ),
                    ( SpellId.CANTRIPSWIFTHUNTER1,            0.6f ),
                    ( SpellId.CANTRIPQUICKNESS1,              0.6f ),

                    ( SpellId.CANTRIPENDURANCE1,              0.5f ),

                    ( SpellId.CANTRIPARCANEPROWESS1,          0.4f ),
                    ( SpellId.CANTRIPIMPREGNABILITY1,         0.4f ),

                    ( SpellId.CANTRIPINVULNERABILITY1,        0.3f ),
                    ( SpellId.CANTRIPMAGICRESISTANCE1,        0.3f ),

                    ( SpellId.CANTRIPALCHEMICALPROWESS1,      0.1f ),
                    ( SpellId.CANTRIPARMOREXPERTISE1,         0.1f ),
                    ( SpellId.CANTRIPCOOKINGPROWESS1,         0.1f ),
                    ( SpellId.CANTRIPDECEPTIONPROWESS1,       0.1f ),
                    ( SpellId.CANTRIPFEALTY1,                 0.1f ),
                    ( SpellId.CANTRIPFLETCHINGPROWESS1,       0.1f ),
                    ( SpellId.CANTRIPHEALINGPROWESS1,         0.1f ),
                    ( SpellId.CANTRIPITEMEXPERTISE1,          0.1f ),
                    ( SpellId.CANTRIPJUMPINGPROWESS1,         0.1f ),
                    ( SpellId.CANTRIPLEADERSHIP1,             0.1f ),
                    ( SpellId.CANTRIPLOCKPICKPROWESS1,        0.1f ),
                    ( SpellId.CANTRIPMAGICITEMEXPERTISE1,     0.1f ),
                    ( SpellId.CANTRIPMONSTERATTUNEMENT1,      0.1f ),
                    ( SpellId.CANTRIPSPRINT1,                 0.1f ),
                    ( SpellId.CANTRIPWEAPONEXPERTISE1,        0.1f ),

                    ( SpellId.CANTRIPARMOR1,                  0.1f ),
                    ( SpellId.CANTRIPACIDWARD1,               0.1f ),
                    ( SpellId.CANTRIPBLUDGEONINGWARD1,        0.1f ),
                    ( SpellId.CANTRIPFLAMEWARD1,              0.1f ),
                    ( SpellId.CANTRIPFROSTWARD1,              0.1f ),
                    ( SpellId.CANTRIPPIERCINGWARD1,           0.1f ),
                    ( SpellId.CANTRIPSLASHINGWARD1,           0.1f ),
                    ( SpellId.CANTRIPSTORMWARD1,              0.1f ),

                    ( SpellId.CANTRIPFOCUS1,                  0.1f ),
                    ( SpellId.CANTRIPWILLPOWER1,              0.1f ),
                };
            }
        }
        private static void BuildSpells()
        {
            for (var i = 0; i < spells.Count; i++)
                Table[i] = new SpellId[NumLevels];

            for (var i = 0; i < spells.Count; i++)
            {
                var spell = spells[i];

                var spellLevels = SpellLevelProgression.GetSpellLevels(spell);

                if (spellLevels == null)
                {
                    log.Error($"MissileCantrips - couldn't find {spell}");
                    continue;
                }

                if (spellLevels.Count != NumLevels)
                {
                    log.Error($"MissileCantrips - expected {NumLevels} levels for {spell}, found {spellLevels.Count}");
                    continue;
                }

                for (var j = 0; j < NumLevels; j++)
                    Table[i][j] = spellLevels[j];
            }
        }

        private static ChanceTable<SpellId> missileCantrips = new ChanceTable<SpellId>()
        {
            ( SpellId.CANTRIPMISSILEWEAPONSAPTITUDE1, 0.10f ),

            ( SpellId.CANTRIPDEFENDER1,               0.07f ),
            ( SpellId.CANTRIPSTRENGTH1,               0.07f ),
            ( SpellId.CANTRIPCOORDINATION1,           0.07f ),

            ( SpellId.CANTRIPBLOODTHIRST1,            0.06f ),
            ( SpellId.CANTRIPSWIFTHUNTER1,            0.06f ),
            ( SpellId.CANTRIPQUICKNESS1,              0.06f ),

            ( SpellId.CANTRIPENDURANCE1,              0.05f ),

            ( SpellId.CANTRIPARCANEPROWESS1,          0.04f ),
            ( SpellId.CANTRIPIMPREGNABILITY1,         0.04f ),

            ( SpellId.CANTRIPINVULNERABILITY1,        0.03f ),
            ( SpellId.CANTRIPMAGICRESISTANCE1,        0.03f ),

            ( SpellId.CantripSummoningProwess1,       0.02f ),

            ( SpellId.CANTRIPALCHEMICALPROWESS1,      0.01f ),
            ( SpellId.CANTRIPARMOREXPERTISE1,         0.01f ),
            ( SpellId.CANTRIPCOOKINGPROWESS1,         0.01f ),
            ( SpellId.CANTRIPDECEPTIONPROWESS1,       0.01f ),
            ( SpellId.CANTRIPFEALTY1,                 0.01f ),
            ( SpellId.CANTRIPFLETCHINGPROWESS1,       0.01f ),
            ( SpellId.CANTRIPHEALINGPROWESS1,         0.01f ),
            ( SpellId.CANTRIPITEMEXPERTISE1,          0.01f ),
            ( SpellId.CANTRIPJUMPINGPROWESS1,         0.01f ),
            ( SpellId.CANTRIPLEADERSHIP1,             0.01f ),
            ( SpellId.CANTRIPLOCKPICKPROWESS1,        0.01f ),
            ( SpellId.CANTRIPMAGICITEMEXPERTISE1,     0.01f ),
            ( SpellId.CANTRIPMONSTERATTUNEMENT1,      0.01f ),
            ( SpellId.CANTRIPPERSONATTUNEMENT1,       0.01f ),
            ( SpellId.CANTRIPSPRINT1,                 0.01f ),
            ( SpellId.CANTRIPWEAPONEXPERTISE1,        0.01f ),

            ( SpellId.CantripDirtyFightingProwess1,   0.01f ),
            ( SpellId.CantripRecklessnessProwess1,    0.01f ),
            ( SpellId.CantripSalvaging1,              0.01f ),
            ( SpellId.CantripSneakAttackProwess1,     0.01f ),

            ( SpellId.CANTRIPARMOR1,                  0.01f ),
            ( SpellId.CANTRIPACIDWARD1,               0.01f ),
            ( SpellId.CANTRIPBLUDGEONINGWARD1,        0.01f ),
            ( SpellId.CANTRIPFLAMEWARD1,              0.01f ),
            ( SpellId.CANTRIPFROSTWARD1,              0.01f ),
            ( SpellId.CANTRIPPIERCINGWARD1,           0.01f ),
            ( SpellId.CANTRIPSLASHINGWARD1,           0.01f ),
            ( SpellId.CANTRIPSTORMWARD1,              0.01f ),

            ( SpellId.CANTRIPFOCUS1,                  0.01f ),
            ( SpellId.CANTRIPWILLPOWER1,              0.01f ),
        };

        public static SpellId Roll()
        {
            return missileCantrips.Roll();
        }
    }
}
