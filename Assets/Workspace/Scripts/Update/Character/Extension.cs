// <copyright file="Extension.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

namespace V4F.Character
{

    public static class Extension
    {
        public static int CalcMinDamageMelee(this Specification self)
        {
            return Mathf.FloorToInt(self.vitality * self.minDamageMeleeFactor + self.minDamageMelee);
        }

        public static int CalcMaxDamageMelee(this Specification self)
        {
            return Mathf.FloorToInt(self.strength * self.maxDamageMeleeFactor + self.maxDamageMelee);
        }

        public static int CalcMinDamageRange(this Specification self)
        {
            return Mathf.FloorToInt(self.dexterity * self.minDamageRangeFactor + self.minDamageRange);
        }

        public static int CalcMaxDamageRange(this Specification self)
        {
            return Mathf.FloorToInt(self.dexterity * self.maxDamageRangeFactor + self.maxDamageRange);
        }

        public static int CalcMinDamageMagic(this Specification self)
        {
            return Mathf.FloorToInt(self.magic * self.minDamageMagicFactor + self.minDamageMagic);
        }

        public static int CalcMaxDamageMagic(this Specification self)
        {
            return Mathf.FloorToInt(self.magic * self.maxDamageMagicFactor + self.maxDamageMagic);
        }

        public static int CalcHealthPoints(this Specification self)
        {
            return Mathf.FloorToInt(self.vitality * self.healthPointsFactor + self.healthPoints);
        }

        public static int Сalculate(this AttributeType self, Actor actor, int add, float mul)
        {
            if (self == AttributeType.HealthPoints)
            {
                var basic = Mathf.FloorToInt(actor[AttributeType.Vitality].value * actor.puppet.spec.healthPointsFactor);
                return Mathf.FloorToInt((basic + add) * (1f + mul));
            }

            return 0;
        }
    }
	
}
