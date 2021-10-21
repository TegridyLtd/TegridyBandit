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

using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Tegridy.Bandit
{
    public class TegridyBanditDigitalGUI : MonoBehaviour
    {
        [Header("GUI Objects")]
        public GUIPrize[] prizeSettings;
        public Image[] wheels;
        public Button close;
        public Button spin;

        public TextMeshProUGUI stake;
        public Button stakeUp;
        public Button stakeDown;
        public int stakeIncrement;
        public int stakeMax;
        public TextMeshProUGUI info;
        public TextMeshProUGUI cash;

        [Header("Bandit Config")]
        [Range(0, 1)] public float houseRake;
        [Range(0, 1)] public float winChance;

        [Header("Audio Settings")]
        public AudioClip[] music;
        public AudioClip[] spinClick;
        public AudioClip[] spinSound;
        public AudioClip[] stopSound;
        public AudioClip[] winSound;
        public AudioClip[] stakeUpSound;
        public AudioClip[] stakeDownSound;

        [Header("Time Settings")]
        [Range(1, 10)] public int minImageSwaps;
        [Range(10, 100)] public int maxImageSwaps;
        [Range(0.01f, 1)] public float swapDelay;
        [Range(0, 5)] public float spinDelay;
    } 
}