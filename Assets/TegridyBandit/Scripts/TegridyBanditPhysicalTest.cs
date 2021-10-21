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
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
namespace Tegridy.Bandit 
{
    public class TegridyBanditPhysicalTest : MonoBehaviour
    {
        public int bulkSpin;
        public bool autoSpin;
        public Button spinButton;
        public TextMeshProUGUI stats;

        public TegridyBanditPhysical testMachine;
        public List<BanditResults> results = new List<BanditResults>();

        private void Start()
        {
            if (spinButton != null) spinButton.onClick.AddListener(() => Spin());
            testMachine.StartUp();
            //do some bulk spins
            int count = 0;
            while (count < bulkSpin)
            {
                count++;
                results.Add(testMachine.controller.SpinWheels(100));
            }
            DisplayStats();
        }

        void Update()
        {
            if (autoSpin) Spin();
        }

        private void Spin()
        {
            BanditResults newResults = testMachine.SpinWheel(100);
            if (newResults != null) results.Add(newResults);
            if (stats != null) DisplayStats();
        }

        private void DisplayStats()
        {

            stats.text = "House Take = " + (testMachine.controller.bandit.houseRake * 100).ToString() + "%";
            stats.text += "<br>WinChance = " + (testMachine.controller.bandit.winChance * 100).ToString() + "%<br>";
            stats.text += "<br>Total Takings = " + testMachine.controller.bandit.totalTake.ToString("F1");
            stats.text += "<br>Total Payout = " + testMachine.controller.bandit.totalPayOut.ToString("F1");
            stats.text += "<br>House Takings = " + testMachine.controller.bandit.housePot.ToString("F1");
            stats.text += "<br>Total Spins = " + testMachine.controller.bandit.totalSpins.ToString();
            stats.text += "<br>Total Wins = " + testMachine.controller.bandit.totalWins.ToString();
            stats.text += "<br>Total Loses = " + testMachine.controller.bandit.totalLoses.ToString();
            stats.text += "<br><br><b>Prize Pots</b>";
            for (int i = 0; i < testMachine.controller.bandit.prizePots.Length; i++)
            {
                stats.text += "<br>Pot " + i + " total = " + testMachine.controller.bandit.prizePots[i].ToString("F1");
            }
        }
    }
}
