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
using UnityEngine;
using TMPro;
namespace Tegridy.Bandit
{
    public class TegridyBanditDigitalInfo : MonoBehaviour
    {
        public TextMeshProUGUI display;
        TegridyBanditDigital control;

        private void Awake()
        {
            control = FindObjectOfType<TegridyBanditDigital>();
            StartCoroutine(WaitForUpdate());
        }
        IEnumerator WaitForUpdate()
        {
            //Make sure the controller has started up
            while (!control.started)  { yield return new WaitForSeconds(0.01f); }

            //Build the string and display the results
            display.text = TotalsDisplay(OutputTotals(control.machines));
            
            //wait until something changes
            float cash = control.playerCash;
            yield return new WaitUntil(() => cash != control.playerCash);
            StartCoroutine(WaitForUpdate());
        }
        public string TotalsDisplay(GUITotalAll bandit)
        {
            //build a text string to be displayed in the GUI
            string stats = TegridyBanditLanguage.totalTake + bandit.totalTake.ToString("F2");
            stats += "<br>" + TegridyBanditLanguage.totalPayout + bandit.totalPayOut.ToString("F2");
            stats += "<br>" + TegridyBanditLanguage.totalHousePot + bandit.housePot.ToString("F2");
            stats += "<br>" + TegridyBanditLanguage.totalSpins + bandit.totalSpins.ToString();
            stats += "<br>" + TegridyBanditLanguage.totalWin + bandit.totalWins.ToString();
            stats += "<br>" + TegridyBanditLanguage.totalLoses + bandit.totalLoses.ToString();
            stats += "<br>" + TegridyBanditLanguage.prizePotsTotal + bandit.prizePots.ToString("F2");
            return stats;
        }
        public GUITotalAll OutputTotals(GUIMachine[] machines)
        {
            //Calculate the totals of all machines
            GUITotalAll totals = new GUITotalAll();
            foreach (GUIMachine machine in machines)
            {
                totals.totalTake += machine.controller.bandit.totalTake;
                totals.totalPayOut += machine.controller.bandit.totalPayOut;
                totals.totalSpins += machine.controller.bandit.totalSpins;
                totals.totalWins += machine.controller.bandit.totalWins;
                totals.totalLoses += machine.controller.bandit.totalLoses;
                totals.housePot += machine.controller.bandit.housePot;
                foreach (float prizePot in machine.controller.bandit.prizePots)
                {
                    totals.prizePots += prizePot;
                }
            }
            return totals;
        }
    }
}
