﻿using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System;
using Random = UnityEngine.Random;

namespace CriminalTown {

    public enum SpriteType {
        None = -1,
        Characters,
        Items,
        Robberies,
        People,
        Places
    }

    public enum CharacterSpriteType {
        ComMale,
        ComFemale,
        Special
    }

    public enum EventStatus {
        Success,
        Fail,
        InProgress
    }

    public class NightEventButtonDetails {
        public string ButtonText;
        public NightEventNode NextEventNode;
        public int Effect;
        public int PoliceEffect;
        public int HospitalEffect;
        public int HealthAffect;
        public int PoliceKnowledge;
        public Dictionary<int, int> Awards;
        public int Money;
    }

    public class NightEventNode {
        public string TitleText;
        public string Description;
        public SpriteType SpriteType;
        public CharacterSpriteType CharSpriteType;
        public int SpriteId;
        public List<NightEventButtonDetails> Buttons;
    }

    public class NightEvent {
        public NightEventNode RootNode;
        public NightEventNode Success;
        public NightEventNode Fail;
    }

    public class NightRobberyData {
        private EventStatus m_eventStatus;
        private readonly Robbery m_robbery;

        public NightEvent nightEvent;

        private int m_money;
        private readonly Dictionary<int, int> m_awards;

        private float chance;
        private float hospitalChance;
        private float policeChance;
        private int healthAffect;
        private int policeKnowledge;

        public EventStatus Status {
            get {
                return m_eventStatus;
            }
        }

        public Robbery Robbery {
            get {
                return m_robbery;
            }
        }

        public int Money {
            get {
                return m_money;
            }
        }

        public Dictionary<int, int> Awards {
            get {
                return m_awards;
            }
        }

        public float Chance {
            get {
                return chance;
            }
        }

        //Constructor
        public NightRobberyData(Robbery robbery) {
            this.m_robbery = robbery;
            nightEvent = NightEventsOptions.GetRandomEvent(robbery.RobberyType);
            chance = RobberiesOptions.CalculatePreliminaryChance(robbery);
            policeChance = UnityEngine.Random.Range(0, 51);
            hospitalChance = UnityEngine.Random.Range(0, 51);
            m_money = RobberiesOptions.GetRobberyMoneyRewardAtTheCurrentMoment(robbery.RobberyType);
            m_awards = RobberiesOptions.GetRobberyAwardsAtTheCurrentMoment(robbery.RobberyType);
            policeKnowledge = 1;
        }

        public void ApplyChoice(NightEventButtonDetails buttonDetails) {
            chance += buttonDetails.Effect;
            hospitalChance += buttonDetails.HospitalEffect;
            policeChance += buttonDetails.PoliceEffect;
            policeKnowledge += buttonDetails.PoliceKnowledge;
            m_money += buttonDetails.Money;
            healthAffect += buttonDetails.HealthAffect;

            if (buttonDetails.Awards != null)
                foreach (int bKey in buttonDetails.Awards.Keys) {
                    if (m_awards.ContainsKey(bKey))
                        m_awards[bKey] += buttonDetails.Awards[bKey];
                    else
                        m_awards.Add(bKey, buttonDetails.Awards[bKey]);
                }
        }

        public void SetAsSuccesfull() {
            m_eventStatus = EventStatus.Success;
        }

        public void SetAsFailed() {
            m_eventStatus = EventStatus.Fail;
        }
    }

    public class NightEventsOptions : MonoBehaviour {
        [SerializeField]
        private Sprite[] m_placesSprites = new Sprite[5];
        [SerializeField]
        private Sprite[] m_peopleSprites = new Sprite[10];

        private static Dictionary<RobberyType, List<int>> m_usedEvents;
        private static Dictionary<RobberyType, NightEvent[]> m_nightEventsCollection;

        public static NightEvent GetRandomEvent(RobberyType robberyType) {
            int eventsCount = m_nightEventsCollection[robberyType].Length;
            int randomNumber = Random.Range(0, eventsCount);
            
            //Avoid repetitions while it is possible
            if (m_usedEvents.ContainsKey(robberyType)) {
                if (m_usedEvents[robberyType].Count < eventsCount) {
                    while (m_usedEvents[robberyType].Contains(eventsCount)) {
                        randomNumber++;
                        if (randomNumber >= eventsCount) {
                            randomNumber = 0;
                        }
                    }
                }
                m_usedEvents[robberyType].Add(randomNumber);
            } else {
                m_usedEvents.Add(robberyType, new List<int> {randomNumber});
            }
            
            return m_nightEventsCollection[robberyType][randomNumber];
        }
        
        public static Sprite GetNightEventSprite(SpriteType spriteType, int spriteId, CharacterSpriteType charSpriteType = 0) {
            switch (spriteType) {
                case SpriteType.Characters:
                    switch (charSpriteType) {
                        case CharacterSpriteType.ComMale:
                            return WM1.charactersOptions.ComMaleSprites[spriteId];
                        case CharacterSpriteType.ComFemale:
                            return WM1.charactersOptions.ComFemaleSprites[spriteId];
                        case CharacterSpriteType.Special:
                            return WM1.charactersOptions.SpecialSprites[spriteId];
                        default:
                            return null;
                    }
                case SpriteType.Items:
                    return WM1.itemsOptions.itemsSprites[spriteId];
                case SpriteType.Robberies:
                    return WM1.robberiesOptions.RobberySprites[spriteId];
                case SpriteType.People:
                    return WM1.nightEventsOptions.m_peopleSprites[spriteId];
                case SpriteType.Places:
                    return WM1.nightEventsOptions.m_placesSprites[spriteId];
                default:
                    return null;
            }
        }

        public static void InitializeNightEventsCollection() {
            m_nightEventsCollection = NightEventsSerialization.GetNightEventsCollection();
            m_usedEvents = new Dictionary<RobberyType, List<int>>();
        }

        public int GetCharacterSpriteId(RobberyType robberyType, int locationNum) {
            return 0;
        }

        public static void ClearUsedEvents() {
            m_usedEvents.Clear();
        }
    }

}