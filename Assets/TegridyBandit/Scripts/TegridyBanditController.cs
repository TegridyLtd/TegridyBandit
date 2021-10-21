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
    [System.Serializable] public class TegridyBanditController
    {
        public Bandit bandit;
        RowHolder[] prizeRows;

        public List<BanditResults> spinLog;
        public void BanditConfig(Bandit thisBandit)
        {
            bandit = thisBandit;
            if (bandit.logWins) spinLog = new List<BanditResults>();

            bandit.totalSpins = 0;
            bandit.totalLoses = 0;
            bandit.totalWins = 0;
            bandit.totalTake = 0;
            bandit.totalPayOut = 0;
            //setup the arrays
            if(bandit.prizePots == null|| bandit.prizePots.Length != bandit.prizes.Length) bandit.prizePots = new float[bandit.prizes.Length];
            
            
            prizeRows = new RowHolder[bandit.prizes.Length];
            for (int i = 0; i < prizeRows.Length; i++)
            {
                prizeRows[i] = new RowHolder();
                prizeRows[i].rows = new Rows[bandit.wheels.Length];
                for (int i2 = 0; i2 < prizeRows[i].rows.Length; i2++)
                {
                    prizeRows[i].rows[i2] = new Rows();
                    prizeRows[i].rows[i2].symbolID = new List<int>();
                }
            }
            //sort the prize rows
            for (int i = 0; i < bandit.wheels.Length; i++)
            {
                for (int i2 = 0; i2 < bandit.wheels[i].symbol.Length; i2++)
                {
                    prizeRows[bandit.wheels[i].symbol[i2].id].rows[i].symbolID.Add(i2);
                }
            }
        }
        public BanditResults SpinWheels(float stake)
        {
            //sort out the finances
            bandit.totalTake += stake;
            float houseCut = stake * bandit.houseRake;
            bandit.housePot += houseCut;
            float potCut = (stake - houseCut) / bandit.prizePots.Length;
            for (int i = 0; i < bandit.prizePots.Length; i++) bandit.prizePots[i] += potCut;

            //decide if the player should win
            bandit.totalSpins++;
            BanditResults result;
            if (Random.Range(0f,1f) < bandit.winChance) result = SpinWin(stake);
            else result = SpinLose();

            if (bandit.logWins) spinLog.Add(result);
            return result;
        }
        private BanditResults SpinLose() 
        {
            bandit.totalLoses++;
            BanditResults results = new BanditResults();
            results.winner = false;
            results.winnings = 0;
            results.wheelPositions = new int[bandit.wheels.Length];

            bool changed = false;
            int previous = 0;
            for (int i = 0; i < results.wheelPositions.Length; i++)
            {
                int thisSymbol = Random.Range(0, prizeRows.Length);
                if (i != 0 && thisSymbol != previous) changed = true;
                previous = thisSymbol;
                if (i - 1 == results.wheelPositions.Length && changed == false)
                {
                    if (thisSymbol > 0) thisSymbol = 0;
                    else thisSymbol++;
                }
                results.wheelPositions[i] = prizeRows[thisSymbol].rows[i].symbolID[Random.Range(0, prizeRows[thisSymbol].rows[i].symbolID.Count)];
            }
            return results;
        }
        private BanditResults SpinWin(float stake) 
        {
            List<int> pots = new List<int>();
            BanditResults results = new BanditResults();

            //check our pots for payout
            for (int i = 0; i < bandit.prizePots.Length; i++)
            {
                if (stake * bandit.prizes[i].prize <= bandit.prizePots[i]) 
                { 
                    pots.Add(i);
                }
            }
            //pick a pot or return a loser
            if (pots.Count > 0)
            {
                int pot = Random.Range(0, pots.Count);
                results.winner = true;
                results.prizeID = pot;
                results.winnings = stake * bandit.prizes[pot].prize;

                bandit.totalWins++;
                bandit.prizePots[pots[pot]] -= results.winnings;
                bandit.totalPayOut += results.winnings;
                results.wheelPositions = new int[bandit.wheels.Length];
                for(int i = 0; i < results.wheelPositions.Length; i++)
                {
                    results.wheelPositions[i] = prizeRows[pot].rows[i].symbolID[Random.Range(0, prizeRows[pot].rows[i].symbolID.Count)];
                }
            }
            else results = SpinLose();

            return results;
        }
    }
}
