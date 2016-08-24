// <copyright file="Extension.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

namespace V4F.Character
{

    public static class Extension
    {
        public static int CalcMinDamageMelee(this Specification self)
        {
            return Mathf.FloorToInt(self.strength * self.minDamageMeleeFactor + self.minDamageMelee);
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
            var spec = actor.puppet.spec;
            var vdmg = 0.4f;

            if (self == AttributeType.HealthPoints)
            {
                var basic = Mathf.FloorToInt(actor[AttributeType.Vitality].value * spec.healthPointsFactor);
                return Mathf.FloorToInt((basic + add) * (1f + mul));
            }

            if (self == AttributeType.MaxDamageMelee)
            {
                var basic = Mathf.FloorToInt((actor[AttributeType.Vitality].value * vdmg + actor[AttributeType.Strength].value) * spec.maxDamageMeleeFactor);
                return Mathf.FloorToInt((basic + add) * (1f + mul));
            }

            if (self == AttributeType.MinDamageMelee)
            {
                var basic = Mathf.FloorToInt((actor[AttributeType.Vitality].value * vdmg + actor[AttributeType.Strength].value) * spec.minDamageMeleeFactor);
                return Mathf.FloorToInt((basic + add) * (1f + mul));
            }

            if (self == AttributeType.MaxDamageRange)
            {
                var basic = Mathf.FloorToInt((actor[AttributeType.Vitality].value * vdmg + actor[AttributeType.Dexterity].value) * spec.maxDamageRangeFactor);
                return Mathf.FloorToInt((basic + add) * (1f + mul));
            }

            if (self == AttributeType.MinDamageRange)
            {
                var basic = Mathf.FloorToInt((actor[AttributeType.Vitality].value * vdmg + actor[AttributeType.Dexterity].value) * spec.minDamageRangeFactor);
                return Mathf.FloorToInt((basic + add) * (1f + mul));
            }

            if (self == AttributeType.MaxDamageMagic)
            {
                var basic = Mathf.FloorToInt((actor[AttributeType.Vitality].value * vdmg + actor[AttributeType.Magic].value) * spec.maxDamageMagicFactor);
                return Mathf.FloorToInt((basic + add) * (1f + mul));
            }

            if (self == AttributeType.MinDamageMagic)
            {
                var basic = Mathf.FloorToInt((actor[AttributeType.Vitality].value * vdmg + actor[AttributeType.Magic].value) * spec.minDamageMagicFactor);
                return Mathf.FloorToInt((basic + add) * (1f + mul));
            }

            return 0;
        }
    }
	
}
