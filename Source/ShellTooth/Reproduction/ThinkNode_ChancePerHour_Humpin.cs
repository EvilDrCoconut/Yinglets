﻿using System;
using Verse;
using RimWorld;
using UnityEngine;
using Verse.AI;

namespace ShellTooth
{
	public class ThinkNode_ChancePerHour_Humpin : ThinkNode_ChancePerHour
	{
		protected override float MtbHours(Pawn pawn)
		{
			Pawn partnerInMyBed = new Pawn();
			if (pawn.CurrentBed() == null || !pawn.GetComp<YingComp>().isDesignatedBreeder || pawn.CurrentBed().CurOccupants.EnumerableCount() != 2)
			{
				return -1f;
			}

			foreach (Pawn pawns in pawn.CurrentBed().CurOccupants)
			{
				if (pawns != pawn && pawns.GetComp<YingComp>().isDesignatedBreeder)
				{
					partnerInMyBed = pawns;
				}
			}
			float MTB = GetHumpinMtbHours(pawn, partnerInMyBed);
			return MTB;
		}
		public float GetHumpinMtbHours(Pawn pawn, Pawn partner)
		{
			if (pawn.Dead || partner.Dead)
			{
				return -1f;
			}
			if (DebugSettings.alwaysDoLovin)
			{
				return 0.1f;
			}
			if (pawn.needs.food.Starving || partner.needs.food.Starving)
			{
				return -1f;
			}
			if (pawn.health.hediffSet.BleedRateTotal > 0f || partner.health.hediffSet.BleedRateTotal > 0f)
			{
				return -1f;
			}
			float num = HumpinMtbSinglePawnFactor(pawn);
			if (num <= 0f)
			{
				return -1f;
			}
			float num2 = HumpinMtbSinglePawnFactor(partner);
			if (num2 <= 0f)
			{
				return -1f;
			}
			float num3 = 0.02f;
			num3 *= num;
			num3 *= num2;
			num3 /= Mathf.Max(pawn.relations.SecondaryLovinChanceFactor(partner), 0.1f);
			num3 /= Mathf.Max(partner.relations.SecondaryLovinChanceFactor(pawn), 0.1f);
			num3 *= GenMath.LerpDouble(-100f, 100f, 1.3f, 0.7f, (float)pawn.relations.OpinionOf(partner));
			num3 *= GenMath.LerpDouble(-100f, 100f, 1.3f, 0.7f, (float)partner.relations.OpinionOf(pawn));
			if (pawn.health.hediffSet.HasHediff(HediffDefOf.PsychicLove, false))
			{
				num3 /= 4f;
			}
			return num3;
		}
		private float HumpinMtbSinglePawnFactor(Pawn pawn)
		{
			float num = 1f;
			num /= 1f - pawn.health.hediffSet.PainTotal;
			float level = pawn.health.capacities.GetLevel(PawnCapacityDefOf.Consciousness);
			if (level < 0.5f)
			{
				num /= level * 2f;
			}
			return num / GenMath.FlatHill(0f, 2f, 4f, 12f, 20f, 0.2f, pawn.ageTracker.AgeBiologicalYearsFloat);
		}
	}
}