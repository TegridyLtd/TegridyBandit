/////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2021 Tegridy Ltd                                          //
// Author: Darren Braviner                                                 //
// Contact: db@tegridygames.co.uk                                          //
/////////////////////////////////////////////////////////////////////////////
//                                                                         //
// This program is free software; you can redistribute it and/or modify    //
// it under the terms of the GNU General Public License as published by    //
// the Free Software Foundation; either version 2 of the License, or       //
// (at your option) any later version.                                     //
//                                                                         //
// This program is distributed in the hope that it will be useful,         //
// but WITHOUT ANY WARRANTY.                                               //
//                                                                         //
/////////////////////////////////////////////////////////////////////////////
//                                                                         //
// You should have received a copy of the GNU General Public License       //
// along with this program; if not, write to the Free Software             //
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston,              //
// MA 02110-1301 USA                                                       //
//                                                                         //
/////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Tegridy.Bandit
{
    #region Main Controller
    [System.Serializable] public class Bandit
    {
        [Header("BanditConfig")]
        public Prize[] prizes;
        public Wheel[] wheels;
        public bool logWins;
        [Range(0, 1)] public float houseRake;
        [Range(0, 1)] public float winChance;

        [Header("Info")]
        public float totalTake;
        public float totalPayOut;
        public int totalSpins;
        public int totalWins;
        public int totalLoses;
        public float housePot;
        public float[] prizePots;
    }
    [System.Serializable] public class BanditResults
    {
        public bool winner;
        public float winnings;
        public int prizeID;
        public int[] wheelPositions;
    }
    [System.Serializable] public class Wheel
    {
        public float offset;
        public Slot[] symbol;
    }
    [System.Serializable] public class Slot
    {
        public int id;
        public float position;
    }
    [System.Serializable] public class Prize
    {
        public int prize;
        public AudioClip[] prizeSound;
    }
    [System.Serializable] public class RowHolder
    {
        public Rows[] rows;
    }
    [System.Serializable] public class Rows
    {
        public List<int> symbolID;
    }
    #endregion
    [System.Serializable] public class GUIPrize
    {
        public Sprite picture;
        public Prize prize;
    }
    [System.Serializable] public class GUIMachine
    {
        public TegridyBanditDigitalGUI gui;
        public TegridyBanditController controller;

        public int currentStake;
        public bool spinning;
        public int[] currentPic;
    }
    [System.Serializable] public class GUITotalAll
    {
        [Header("Info")]
        public float totalTake;
        public float totalPayOut;
        public int totalSpins;
        public int totalWins;
        public int totalLoses;
        public float housePot;
        public float prizePots;
    }

}
